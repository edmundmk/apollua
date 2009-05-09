// Parser.rules.cs
//
// Lua is copyright © 1994-2008 Lua.org, PUC-Rio
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Front.Parser
{


partial class Parser
{
	IParserActions				actions;
	ParseStack< Expression >	expression;
	ParseStack< string >		name;
	ParseStack< Scope >			scope;





	// Chunk

	Code chunk( IParserActions actions )
	{
		/*	goal chunk
				: block eof
				;
		*/

		Next();
		this.actions	= actions;
		this.expression	= new ParseStack< Expression >();
		this.name		= new ParseStack< string >();
		this.scope		= new ParseStack< Scope >();
		

		SourceLocation startLocation = new SourceLocation( lexer.SourceName, 0, 0 );
		scope.Push( actions.Function( startLocation, null, name.Pop( 0 ), true ) );

		block();

		Token eof = Check( TokenKind.EOF );

		Code objectCode = actions.EndFunction( eof.Location, scope.Pop() );

		Debug.Assert( this.expression.Count == 0 );
		Debug.Assert( this.name.Count == 0 );
		Debug.Assert( this.scope.Count == 0 );
		
		this.actions	= null;
		this.expression	= null;
		this.name		= null;
		this.scope		= null;

		return objectCode;
	}



	// Blocks
	

	bool block_follow_set()
	{
		switch ( Get() )
		{
		case TokenKind.Else:
		case TokenKind.Elseif:
		case TokenKind.End:
		case TokenKind.Until:
		case TokenKind.EOF:
			return true;

		default:
			return false;
		}
	}

	void block()
	{
		/*  block
				: ( stat ';'? )*  ( laststat ';'? )?
				;
		*/

		bool isLastStatement = false;
		while ( ! isLastStatement && ! block_follow_set() )
		{
			isLastStatement = statement();
			Test( TokenKind.Semicolon );
		}
	}



	// Statements


	bool statement()
	{
		/*	statement
				: stat		{ returns false }
				| laststat	{ returns true }
				;

			stat
				: exprstat
				| dostat
				| whilestat
				| repeatstat
				| ifstat
				| forstat
				| funcstat
				| localfunc
				| localstat
				;

			laststat
				: retstat
				| breakstat
				| continuestat
				;
		*/

		switch ( Get() )
		{
		case TokenKind.Do:
			dostat();
			return false;

		case TokenKind.While:
			whilestat();
			return false;

		case TokenKind.Repeat:
			repeatstat();
			return false;

		case TokenKind.If:
			ifstat();
			return false;

		case TokenKind.For:
			forstat();
			return false;

		case TokenKind.Function:
			funcstat();
			return false;

		case TokenKind.Local:
			if ( Lookahead() == TokenKind.Function )
			{
				localfunc();
			}
			else
			{
				localstat();
			}
			return false;

		case TokenKind.Return:
			retstat();
			return true;

		case TokenKind.Break:
			breakstat();
			return true;

		case TokenKind.Continue:
			continuestat();
			return true;

		default:
			exprstat();
			return false;
		}
	}


	void dostat()
	{
		/*	dostat
				: 'do' block 'end'
				;
		*/

		Token matchDo = Check( TokenKind.Do );

		scope.Push( actions.Do( matchDo.Location, scope.Peek() ) );
		
		block();
	
		Token endDo = Check( TokenKind.End, matchDo );

		actions.EndDo( endDo.Location, scope.Pop() );
	}


	void whilestat()
	{
		/*	whilestat
				: 'while' exp 'do' block 'end'
				;
		*/


		Token matchWhile = Check( TokenKind.While );
		exp();

		scope.Push( actions.While( matchWhile.Location, scope.Peek(), expression.Pop() ) );

		Check( TokenKind.Do );
		block();

		Token endWhile = Check( TokenKind.End, matchWhile );

		actions.EndWhile( endWhile.Location, scope.Pop() );
	}


	void repeatstat()
	{
		/*	repeatstat
				: 'repeat' block 'until' exp
				;
		*/

		Token matchRepeat = Check( TokenKind.Repeat );

		scope.Push( actions.Repeat( matchRepeat.Location, scope.Peek() ) );

		block();
		Token until = Check( TokenKind.Until, matchRepeat );
		exp();

		actions.Until( until.Location, scope.Pop(), expression.Pop() );
	}


	void ifstat()
	{
		/*	ifstat
				: 'if' exp 'then' block ( 'elseif' exp 'then' block )* ( 'else' block )? 'end'
				;
		*/

		Token match = Check( TokenKind.If );
		exp();

		scope.Push( actions.If( match.Location, scope.Peek(), expression.Pop() ) );

		Check( TokenKind.Then );
		block();

		while ( Get() == TokenKind.Elseif )
		{
			Token elseIf = Check( TokenKind.Elseif );

			exp();

			scope.Push( actions.ElseIf( elseIf.Location, scope.Pop(), expression.Pop() ) );

			Check( TokenKind.Then );
			block();
		}

		if ( Get() == TokenKind.Else )
		{
			Token elseToken = Check( TokenKind.Else );

			scope.Push( actions.Else( elseToken.Location, scope.Pop() ) );

			block();
		}

		Token endIf = Check( TokenKind.End, match );

		actions.EndIf( endIf.Location, scope.Pop() );
	}


	void forstat()
	{
		/*	forstat
				: 'for' fornum 'end'
				| 'for' forlist 'end'
				;
		*/

		Token matchFor = Check( TokenKind.For );
		
		if ( Get() != TokenKind.Identifier )
		{
			Error( "{0} expected", Lexer.GetTokenName( TokenKind.Identifier ) );
			return;
		}

		switch ( Lookahead() )
		{
		case TokenKind.EqualSign:
			fornum( matchFor );
			break;

		case TokenKind.Comma:
		case TokenKind.In:
			forlist( matchFor );
			break;

		default:
			Error( "{0}, {1} or {2} expected", Lexer.GetTokenName( TokenKind.In ),
				Lexer.GetTokenName( TokenKind.Comma ), Lexer.GetTokenName( TokenKind.EqualSign ) );
			return;
		}
		
	}


	void fornum( Token matchFor )
	{
		/*	fornum
				: IDENTIFIER '=' exp ',' exp ( ',' exp )? 'do' block
				;
		*/

		Token name = Check( TokenKind.Identifier );
		Check( TokenKind.EqualSign );
		exp();
		Expression start = expression.Pop();
		Check( TokenKind.Comma );
		exp();
		Expression limit = expression.Pop();

		Expression step = null;
		if ( Test( TokenKind.Comma ) )
		{
			exp();
			step = expression.Pop();
		}
		else
		{
			step = actions.LiteralExpression( matchFor.Location, (double)1 );
		}

		scope.Push( actions.For( matchFor.Location, scope.Peek(), (string)name.Value, start, limit, step ) );

		Check( TokenKind.Do );
		block();

		Token endFor = Check( TokenKind.End, matchFor );

		actions.EndFor( endFor.Location, scope.Pop() );
	}


	void forlist( Token matchFor )
	{
		/*	forlist
				: IDENTIFIER ( ',' IDENTIFIER )* 'in' explist 'do' block
				;
		*/

		
		name.Push( (string)Check( TokenKind.Identifier ).Value );
		int namecount = 1;
		
		while ( Test( TokenKind.Comma ) )
		{
			name.Push( (string)Check( TokenKind.Identifier ).Value );
			namecount += 1;
		}

		Check( TokenKind.In );
		int expressioncount = explist();

		scope.Push( actions.ForIn( matchFor.Location, scope.Peek(), name.Pop( namecount ), expression.Pop( expressioncount ) ) );

		Check( TokenKind.Do );
		block();

		Token endFor = Check( TokenKind.End, matchFor );

		actions.EndForIn( endFor.Location, scope.Pop() );
	}


	void funcstat()
	{
		/*	funcstat
				: 'function' funcname funcbody
				;
		
			funcname
				: IDENTIFIER ( '.' IDENTIFIER )* ( ':' IDENTIFIER )
				;
		*/

		Token matchFunction = Check( TokenKind.Function );
		
		Expression variable = Lookup( Check( TokenKind.Identifier ) );

		while ( Get() == TokenKind.FullStop )
		{
			Token fullStop = Check( TokenKind.FullStop );
			Token key = Check( TokenKind.Identifier );
			variable = actions.LookupExpression( fullStop.Location,
				variable, actions.LiteralExpression( key.Location, (string)key.Value ) );
		}

		bool implicitSelf = false;
		if ( Get() == TokenKind.Colon )
		{
			implicitSelf = true;

			Token colon = Check( TokenKind.Colon );
			Token key = Check( TokenKind.Identifier );
			variable = actions.LookupExpression( colon.Location,
				variable, actions.LiteralExpression( key.Location, (string)key.Value ) );
		}

		expression.Push( variable );
		funcbody( matchFunction, implicitSelf );
	
		IList< Expression > expressionlist	= expression.Pop( 1 );
		IList< Expression > variablelist	= expression.Pop( 1 );
		
		actions.Assignment( matchFunction.Location, scope.Peek(), variablelist, expressionlist );
	}


	void localfunc()
	{
		/*	localfunc
				: 'local' 'function' IDENTIFIER funcbody
				;
		*/

		Check( TokenKind.Local );
		Token matchFunction = Check( TokenKind.Function );
		
		name.Push( (string)Check( TokenKind.Identifier ).Value );

		funcbody( matchFunction, false );

		IList< Expression >	expressionlist	= expression.Pop( 1 );
		IList< string >		namelist		= name.Pop( 1 );
		
		actions.Local( matchFunction.Location, scope.Peek(), namelist, expressionlist );
	}


	void funcbody( Token matchFunction, bool implicitSelf )
	{
		/*	funcbody
				: '(' parlist ')' block 'end'
				;
		
			parlist
				: ( param ( ',' param )* )?
				;

			param
				: IDENTIFIER
				| '...'
				;
		*/

		if ( Get() == TokenKind.LeftParenthesis || Get() == TokenKind.NewlineLeftParenthesis )
		{
			Next();
		}
		else
		{
			Error( "{0} expected", Lexer.GetTokenName( TokenKind.LeftParenthesis ) );
		}

		int parametercount = 0;

		if ( implicitSelf )
		{
			name.Push( "self" );
			parametercount += 1;
		}

		bool isVararg = false;
		if ( Get() != TokenKind.RightParenthesis )
		{
			while ( true )
			{
				// param

				if ( Get() == TokenKind.Identifier )
				{
					name.Push( (string)Check( TokenKind.Identifier ).Value );
					parametercount += 1;
				}
				else if ( Get() == TokenKind.Ellipsis )
				{
					Check( TokenKind.Ellipsis );
					isVararg = true;
					break;
				}
				else
				{
					Error( "{0} or {1} expected", Lexer.GetTokenName( TokenKind.Identifier ),
						Lexer.GetTokenName( TokenKind.Ellipsis ) );
					break;
				}

				// ','

				if ( ! Test( TokenKind.Comma ) )
				{
					break;
				}
			}
		}

		Check( TokenKind.RightParenthesis );

		scope.Push( actions.Function( matchFunction.Location, 
			scope.Peek(), name.Pop( parametercount ), isVararg ) );
		
		block();

		Token endFunction = Check( TokenKind.End, matchFunction );

		expression.Push( actions.FunctionExpression( matchFunction.Location,
			actions.EndFunction( endFunction.Location, scope.Pop() ) ) );
	}


	void localstat()
	{
		/*	localstat
				: 'local' IDENTIFIER ( ',' IDENTIFIER )* ( '=' explist )?
				;
		*/

		Token local = Check( TokenKind.Local );


		// Have to declare new variables after evaluating the explist, so
		// that local x = x will find the value of x in the enclosing scope.

		int variablecount = 0;
		while ( true )
		{
			// IDENTIFIER

			name.Push( (string)Check( TokenKind.Identifier ).Value );
			variablecount += 1;
			

			// ','

			if ( ! Test( TokenKind.Comma ) )
			{
				break;
			}
		}


		// explist

		int expressioncount = 0;
		if ( Get() == TokenKind.EqualSign )
		{
			Check( TokenKind.EqualSign );
			expressioncount = explist();
		}


		// Perform assignment.

		IList< Expression >	expressionlist	= expression.Pop( expressioncount );
		IList< string >		namelist		= name.Pop( variablecount );
		
		actions.Local( local.Location, scope.Peek(), namelist, expressionlist );
	}


	void exprstat()
	{
		/*	exprstat
				: primaryexp { not a function call } ( ',' primaryexp )* '=' explist
				| primaryexp { function call }
				;
		*/

		
		Token callToken;
		bool isFunctionCall = primaryexp( out callToken );
		if ( ! isFunctionCall )
		{
			int variablecount	= 1;
			while ( Test( TokenKind.Comma ) )
			{
				variablecount += 1;
				primaryexp( out callToken );
			}

			Token equalSign = Check( TokenKind.EqualSign );
			int expressioncount = explist();

			IList< Expression > expressionlist	= expression.Pop( expressioncount );
			IList< Expression > variablelist	= expression.Pop( variablecount );
			
			actions.Assignment( equalSign.Location, scope.Peek(), variablelist, expressionlist );
		}
		else
		{
			actions.CallStatement( callToken.Location, scope.Peek(), expression.Pop() );
		}		
	}



	void retstat()
	{
		/*	retstat
				: 'return' ( explist )?
				;
		*/

		Token returnToken = Check( TokenKind.Return );

		int expressioncount = 0;
		if ( ! block_follow_set() && Get() != TokenKind.Semicolon )
		{
			expressioncount = explist();
		}

		for ( int scopecount = 0; scopecount < scope.Count; ++scopecount )
		{
			Scope functionScope = scope.Peek( scopecount );
			if ( functionScope.IsFunctionScope )
			{
				actions.Return( returnToken.Location, functionScope, expression.Pop( expressioncount ) );
				return;
			}
		}

		expression.Pop( expressioncount );
		Error( "No function to return from" );
	}


	void breakstat()
	{
		/*	breakstat
				: 'break'
				;
		*/

		Token breakToken = Check( TokenKind.Break );

		for ( int scopecount = 0; scopecount < scope.Count; ++scopecount )
		{
			Scope loopScope = scope.Peek( scopecount );
			if ( loopScope.IsLoopScope )
			{
				actions.Break( breakToken.Location, loopScope );
				return;
			}
		}

		Error( "No loop to break" );
	}


	void continuestat()
	{
		/*	continuestat
				: 'continue'
				;
		*/

		Token continueToken = Check( TokenKind.Continue );

		for ( int scopecount = 0; scopecount < scope.Count; ++scopecount )
		{
			Scope loopScope = scope.Peek( scopecount );
			if ( loopScope.IsLoopScope )
			{
				actions.Continue( continueToken.Location, loopScope );
				return;
			}
		}

		Error( "No loop to continue" );
	}




	// Expressions


	struct Operator
	{
		public int	LeftPriority	{ get; private set; }
		public int	RightPriority	{ get; private set; }


		public Operator( int leftPriority, int rightPriority )
			:	this()
		{
			LeftPriority	= leftPriority;
			RightPriority	= rightPriority;
		}
	}

	static readonly Dictionary< TokenKind, Operator > unaryOperators = new Dictionary< TokenKind, Operator >
	{
		{ TokenKind.Not,				new Operator( 8, 8 ) },
		{ TokenKind.HyphenMinus,		new Operator( 8, 8 ) },
		{ TokenKind.NumberSign,			new Operator( 8, 8 ) },
	};
	
	static readonly Dictionary< TokenKind, Operator > binaryOperators = new Dictionary< TokenKind, Operator >
	{
		{ TokenKind.CircumflexAccent,	new Operator( 10, 9 ) },

		{ TokenKind.Solidus,			new Operator( 7, 7 ) },
		{ TokenKind.PercentSign,		new Operator( 7, 7 ) },
		{ TokenKind.Asterisk,			new Operator( 7, 7 ) },

		{ TokenKind.PlusSign,			new Operator( 6, 6 ) },
		{ TokenKind.HyphenMinus,		new Operator( 6, 6 ) },

		{ TokenKind.Concatenate,		new Operator( 5, 4 ) },

		{ TokenKind.LogicalEqual,		new Operator( 3, 3 ) },
		{ TokenKind.NotEqual,			new Operator( 3, 3 ) },
		{ TokenKind.GreaterThanSign,	new Operator( 3, 3 ) },
		{ TokenKind.GreaterThanOrEqual,	new Operator( 3, 3 ) },
		{ TokenKind.LessThanSign,		new Operator( 3, 3 ) },
		{ TokenKind.LessThanOrEqual,	new Operator( 3, 3 ) },

		{ TokenKind.And,				new Operator( 2, 2 ) },
		{ TokenKind.Or,					new Operator( 1, 1 ) },
	};


	int explist()
	{
		/*	explist
				: exp ( ',' exp )*
				;
		*/

		int expressioncount = 0;
		while ( true )
		{
			// exp

			exp();
			expressioncount += 1;
			

			// ','

			if ( ! Test( TokenKind.Comma ) )
			{
				break;
			}
		}


		return expressioncount;
	}


	void exp()
	{
		subexpr( 0 );
	}

	
	void subexpr( int limit )
	{
		/*	subexpr	{ shift-reduce conflict on binop is resolved using operator precedence }
				:	( simplexp | unop subexpr ) ( binop subexpr )*
				;
		*/

		
		// Check for unary operator.

		Operator unaryOperator;
		if ( unaryOperators.TryGetValue( Get(), out unaryOperator ) )
		{
			Token operatorToken = Next();
			subexpr( unaryOperator.RightPriority );

			expression.Push( actions.UnaryExpression( operatorToken.Location,
				expression.Pop(), operatorToken.Kind ) );
		}
		else
		{
			simpleexp();
		}


		// Check for binary operators.

		Operator binaryOperator;
		while ( binaryOperators.TryGetValue( Get(), out binaryOperator ) && binaryOperator.LeftPriority > limit )
		{
			Expression left = expression.Pop();

			Token operatorToken = Next();
			subexpr( binaryOperator.RightPriority );

			expression.Push( actions.BinaryExpression( operatorToken.Location,
				left, expression.Pop(), operatorToken.Kind ) );
		}
	}


	void simpleexp()
	{
		/*	simpleexp
				: primaryexp
				| STRING
				| NUMBER
				| constructor
				| functionexp
				| 'nil'
				| 'true'
				| 'false'
				| '...'
				;
		*/

		switch ( Get() )
		{
		case TokenKind.String:
			Token stringToken = Check( TokenKind.String );
			expression.Push( actions.LiteralExpression(
				stringToken.Location, (string)stringToken.Value ) );
			break;

		case TokenKind.Number:
			Token numberToken = Check( TokenKind.Number );
			expression.Push( actions.LiteralExpression(
				numberToken.Location, (double)numberToken.Value ) );
			break;

		case TokenKind.LeftCurlyBracket:
			constructor();
			break;

		case TokenKind.Function:
			functionexp();
			break;

		case TokenKind.Nil:
			Token nilToken = Check( TokenKind.Nil );
			expression.Push( actions.LiteralExpression(
				nilToken.Location, null ) );
			break;

		case TokenKind.True:
			Token trueToken = Check( TokenKind.True );
			expression.Push( actions.LiteralExpression(
				trueToken.Location, (bool)true ) );
			break;

		case TokenKind.False:
			Token falseToken = Check( TokenKind.False );
			expression.Push( actions.LiteralExpression(
				falseToken.Location, (bool)false ) );
			break;

		case TokenKind.Ellipsis:
			Token ellipsisToken = Check( TokenKind.Ellipsis );
			for ( int scopecount = 0; scopecount < scope.Count; ++scopecount )
			{
				Scope functionScope = scope.Peek( scopecount );
				if ( functionScope.IsFunctionScope )
				{
					if ( functionScope.IsVarargFunctionScope )
					{
						expression.Push( actions.VarargsExpression(
							ellipsisToken.Location, functionScope ) );
					}
					else
					{
						expression.Push( actions.LiteralExpression( ellipsisToken.Location, null ) );
						Error( "Cannot use {0} outside a vararg function", Lexer.GetTokenName( TokenKind.Ellipsis ) );
					}
					break;
				}
			}			
			break;

		default:
			Token callToken;
			primaryexp( out callToken );
			break;
		}
	}


	void constructor()
	{
		/*	constructor
				: '{' ( fieldlist )? '}' 
				;

			fieldlist
				: field ( fieldsep field )* fieldsep?
				;

			field
				: listfield
				| recfield
				;

			listfield
				: exp
				;

			recfield
				: IDENTIFIER '=' exp
				| '[' exp ']' '=' exp
				;

			fieldsep
				: ',' | ';'
				;
		*/

		Token matchConstructor = Check( TokenKind.LeftCurlyBracket );

		scope.Push( actions.Constructor( matchConstructor.Location, scope.Peek() ) );

		int arrayKey = 1;

		while ( true )
		{
			if ( Get() == TokenKind.RightCurlyBracket )
				break;


			// field

			if ( Get() == TokenKind.Identifier && Lookahead() == TokenKind.EqualSign )
			{
				// recfield

				Token key = Check( TokenKind.Identifier );
				Token equalSign = Check( TokenKind.EqualSign );
				exp();
				actions.Field( equalSign.Location, scope.Peek(),
					actions.LiteralExpression( key.Location, (string)key.Value ), expression.Pop() );
			}
			else if ( Get() == TokenKind.LeftSquareBracket )
			{
				// recfield

				Check( TokenKind.LeftSquareBracket );
				exp();
				Expression key = expression.Pop();
				Check( TokenKind.RightSquareBracket );
				Token equalSign = Check( TokenKind.EqualSign );
				exp();
				actions.Field( equalSign.Location, scope.Peek(), key, expression.Pop() );
			}
			else
			{
				// listfield

				Token token = GetToken();
				exp();
				if ( Get() != TokenKind.RightCurlyBracket && Lookahead() != TokenKind.RightCurlyBracket )
				{
					actions.Field( token.Location, scope.Peek(), arrayKey, expression.Pop() );
					arrayKey += 1;
				}
				else
				{
					actions.LastField( token.Location, scope.Peek(), arrayKey, expression.Pop() );
				}
			}


			// fieldsep

			if ( Get() == TokenKind.Comma || Get() == TokenKind.Semicolon )
			{
				Next();
			}
			else
			{
				break;
			}
		}

		Token endConstructor = Check( TokenKind.RightCurlyBracket, matchConstructor );

		expression.Push( actions.EndConstructor( endConstructor.Location, scope.Pop() ) );
	}


	void functionexp()
	{
		/*	functionexp
				: 'function' funcbody
				;
		*/

		Token matchFunction = Check( TokenKind.Function );

		funcbody( matchFunction, false );
	}


	bool primaryexp( out Token callToken )
	{
		/*	primaryexp
				: prefixexp ( postfix )*	{ return result of last postfix, or false }
				;

			postfix
				: '.' IDENTIFIER
				| '[' exp ']'
				| funcargs					{ returns true }		
				| ':' IDENTIFIER funcargs	{ returns true }
				;
		*/

		prefixexp();
		
		Expression left;
		bool isFunctionCall = false;

		while ( true )
		{
			Token name;
			int argumentcount = 0;

			callToken = GetToken();

			switch ( Get() )
			{
			// lookup.
			case TokenKind.FullStop:
				isFunctionCall = false;
				left = expression.Pop();
				Token fullStop = Check( TokenKind.FullStop );
				name = Check( TokenKind.Identifier );
				expression.Push( actions.LookupExpression( fullStop.Location,
					left, actions.LiteralExpression( name.Location, (string)name.Value ) ) );
				break;

			// array lookup.
			case TokenKind.LeftSquareBracket:
				isFunctionCall = false;
				left = expression.Pop();
				Token bracket = Check( TokenKind.LeftSquareBracket );
				exp();
				Check( TokenKind.RightSquareBracket );
				expression.Push( actions.LookupExpression(
					bracket.Location, left, expression.Pop() ) );
				break;

			// call
			case TokenKind.LeftParenthesis:
			case TokenKind.NewlineLeftParenthesis:
			case TokenKind.LeftCurlyBracket:
			case TokenKind.String:
				isFunctionCall = true;
				left = expression.Pop();
				argumentcount = funcargs();
				expression.Push( actions.CallExpression(
					callToken.Location, left, expression.Pop( argumentcount ) ) );
				break;

			// selfcall
			case TokenKind.Colon:
				isFunctionCall = true;
				left = expression.Pop();
				Check( TokenKind.Colon );
				name = Check( TokenKind.Identifier );
				callToken = GetToken();
				argumentcount = funcargs();
				expression.Push( actions.SelfCallExpression( callToken.Location,
					left, name.Location, (string)name.Value, expression.Pop( argumentcount ) ) );
				break;

			// no more postfixes
			default:
				return isFunctionCall;
			}
		}
	}


	int funcargs()
	{
		/*	funcargs
				: { no newline } '(' ( explist )? ')'
				| constructor
				| string
				;
		*/


		int argumentcount = 0;

		switch ( Get() )
		{
		case TokenKind.NewlineLeftParenthesis:
			// encountering this ambiguity is an error
			Error( "Newline between expression and function call arguments is ambiguous (remove newline or add a semicolon)" );
			// continue as if the newline didn't exist.
			goto case TokenKind.LeftParenthesis;

		case TokenKind.LeftParenthesis:
			Token matchParenthesis = Next();
			if ( Get() != TokenKind.RightParenthesis )
			{
				argumentcount = explist();
			}
			Check( TokenKind.RightParenthesis, matchParenthesis );
			break;

		case TokenKind.LeftCurlyBracket:
			constructor();
			argumentcount = 1;
			break;
		
		case TokenKind.String:
			Token stringToken = Check( TokenKind.String );
			expression.Push( actions.LiteralExpression( stringToken.Location, (string)stringToken.Value ) );
			argumentcount = 1;
			break;

		default:
			Error( "Function arguments expected" );
			break;
		}

		return argumentcount;
	}


	void prefixexp()
	{
		/*	prefixexp
				: IDENTIFIER
				| '(' exp ')'
				;
		*/

		switch ( Get() )
		{
		case TokenKind.LeftParenthesis:
		case TokenKind.NewlineLeftParenthesis:
			Token matchParenthesis = Next();
			exp();
			Check( TokenKind.RightParenthesis, matchParenthesis );
			expression.Push( actions.NestedExpression( matchParenthesis.Location, expression.Pop() ) );
			break;
			
		case TokenKind.Identifier:
			Token name = Check( TokenKind.Identifier );
			expression.Push( Lookup( name ) );
			break;

		default:
			expression.Push( actions.LiteralExpression( GetToken().Location, null ) );
			Error( "Unexpected token {0}", Lexer.GetTokenName( Get() ) );
			break;
		}


	}



}



}


