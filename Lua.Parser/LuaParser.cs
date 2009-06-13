// LuaParser.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.Parser
{


public class LuaParser
{
	// Errors.

	TextWriter	errorWriter;
	bool		hasError;


	// Lexer.
	
	string		sourceName;
	LuaLexer	lexer;
	Token		lookahead;
	Token		token;


	// Parse state.

	FunctionAST					function;
	Block						block;
	LoopScope					loopScope;
	ParseStack< Expression >	expression;



	// Public.

	public LuaParser( TextWriter errorWriter, TextReader sourceReader, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceName		= sourceName;
		lexer				= new LuaLexer( errorWriter, sourceReader, sourceName );
		Next();

		function			= null;
		block				= null;
		loopScope			= null;
		expression			= new ParseStack< Expression >();
	}


	public bool HasError
	{
		get { return hasError || lexer.HasError; }
	}


	public FunctionAST Parse()
	{
		FunctionAST functionAST = chunk();
		if ( ! hasError )
		{
			return functionAST;
		}
		else
		{
			return null;
		}
	}




	// Chunk

	FunctionAST chunk()
	{
		/*	goal chunk
				: block eof
				;
		*/

		Debug.Assert( function == null );
		Debug.Assert( block == null );
		Debug.Assert( loopScope == null );

		function = new FunctionAST( "<chunk>", function );
		block = new Block( new SourceSpan(), null, "function" );
		function.SetBlock( block );

		function.SetVararg();

		blockstat();

		Token eof = Check( TokenKind.EOF );

		block.SetSourceSpan( new SourceSpan( new SourceLocation( sourceName, 0, 0 ), eof.SourceSpan.End ) );
		block = block.Parent;
		FunctionAST result = function;
		function = function.Parent;

		Debug.Assert( function == null );
		Debug.Assert( block == null );
		Debug.Assert( loopScope == null );
		Debug.Assert( expression.Count == 0 );
		
		return result;
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

	void blockstat()
	{
		/*  block
				: ( stat ';'? )*  ( laststat ';'? )?
				;
		*/

		bool isLastStatement = false;
		while ( ! isLastStatement && ! block_follow_set() )
		{
			isLastStatement = statement();
			Debug.Assert( block != null );
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
	
		/*	scope
			{
		...
			}
		*/

		Token matchDo = Check( TokenKind.Do );
		block = new Block( new SourceSpan(), block, "do" );
		block.Parent.Statement( block );
		
		blockstat();
	
		Token endDo = Check( TokenKind.End, matchDo );
		block.SetSourceSpan( new SourceSpan( matchDo.SourceSpan.Start, endDo.SourceSpan.End ) );
		block = block.Parent;
	}


	void whilestat()
	{
		/*	whilestat
				: 'while' exp 'do' block 'end'
				;
		*/

		/*	whileContinue:
				bfalse <condition> whileBreak
				block
					...
				end
				b loopContinue
			whileBreak:
		*/

		Token matchWhile = Check( TokenKind.While );
		exp();
		Expression condition = PopValue();
		Token doToken = Check( TokenKind.Do );

		SourceSpan s			= new SourceSpan( matchWhile.SourceSpan.Start, doToken.SourceSpan.End );
		LabelAST whileBreak		= new LabelAST( "whileBreak" );		function.Label( whileBreak );
		LabelAST whileContinue	= new LabelAST( "whileContinue" );	function.Label( whileContinue );
		

		block.Statement( new MarkLabel( s, whileContinue ) );
		block.Statement( new Test( s, condition, whileBreak ) );
		
		block = new Block( new SourceSpan(), block, "while" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, whileBreak, whileContinue );
				
		blockstat();

		Token endWhile = Check( TokenKind.End, matchWhile );

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( doToken.SourceSpan.Start, endWhile.SourceSpan.End ) );
		block = block.Parent;
		
		block.Statement( new Branch( endWhile.SourceSpan, whileContinue ) );
		block.Statement( new MarkLabel( endWhile.SourceSpan, whileBreak ) );
	}


	void repeatstat()
	{
		/*	repeatstat
				: 'repeat' block 'until' exp
				;
		*/

		/*	repeat:
				block
					...
			repeatContinue:
					bfalse <condition> loopTop
				end
			repeatBreak:
		*/

		Token matchRepeat = Check( TokenKind.Repeat );

		SourceSpan s			= matchRepeat.SourceSpan;
		LabelAST repeat			= new LabelAST( "repeat" );			function.Label( repeat );
		LabelAST repeatBreak	= new LabelAST( "repeatBreak" );	function.Label( repeatBreak );
		LabelAST repeatContinue	= new LabelAST( "repeatContinue" );	function.Label( repeatContinue );
		
		block.Statement( new MarkLabel( s, repeat ) );

		block = new Block( new SourceSpan(), block, "repeat" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, repeatBreak, repeatContinue );

		blockstat();
	
		Token until = Check( TokenKind.Until, matchRepeat );
		exp();
		Expression condition = PopValue();

		s = new SourceSpan( until.SourceSpan.Start, condition.SourceSpan.End );

		block.Statement( new MarkLabel( s, repeatContinue ) );
		block.Statement( new Test( s, condition, repeat ) );

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( matchRepeat.SourceSpan.Start, s.End ) );
		block = block.Parent;

		block.Statement( new MarkLabel( s, repeatBreak ) );
	}


	void ifstat()
	{
		/*	ifstat
				: 'if' exp 'then' block ( 'elseif' exp 'then' block )* ( 'else' block )? 'end'
				;
		*/

		/*		bfalse <condition> nextClause
				block
					...
				end
				b endIf
			ifClause:
				bfalse <condition> nextClause
				block
					...
				end
				b endIf
			ifClause:
				block
					...
				end
			ifEnd:
		*/

		Token match = Check( TokenKind.If );
		exp();
		Expression condition = PopValue();
		Token thenToken = Check( TokenKind.Then );

		SourceSpan s		= new SourceSpan( match.SourceSpan.Start, thenToken.SourceSpan.End );
		LabelAST ifClause	= new LabelAST( "ifClause" );	function.Label( ifClause );
		LabelAST ifEnd		= new LabelAST( "ifEnd" );		function.Label( ifEnd );

		block.Statement( new Test( s, condition, ifClause ) );

		block = new Block( new SourceSpan(), block, "if" );
		block.Parent.Statement( block );

		blockstat();

		while ( Get() == TokenKind.Elseif )
		{
			Token elseIf = Check( TokenKind.Elseif );

			block.SetSourceSpan( new SourceSpan( thenToken.SourceSpan.Start, elseIf.SourceSpan.End ) );
			block = block.Parent;

			block.Statement( new Branch( elseIf.SourceSpan, ifEnd ) );
			block.Statement( new MarkLabel( elseIf.SourceSpan, ifClause ) );

			exp();
			condition = PopValue();
			thenToken = Check( TokenKind.Then );

			s			= new SourceSpan( elseIf.SourceSpan.Start, thenToken.SourceSpan.End );
			ifClause	= new LabelAST( "ifClause" );	function.Label( ifClause );
			
			block.Statement( new Test( s, condition, ifClause ) );

			block = new Block( new SourceSpan(), block, "elseif" );
			block.Parent.Statement( block );

			blockstat();
		}

		if ( Get() == TokenKind.Else )
		{
			Token elseToken = Check( TokenKind.Else );

			block.SetSourceSpan( new SourceSpan( thenToken.SourceSpan.Start, elseToken.SourceSpan.End ) );
			block = block.Parent;

			block.Statement( new Branch( elseToken.SourceSpan, ifEnd ) );
			block.Statement( new MarkLabel( elseToken.SourceSpan, ifClause ) );

			block = new Block( new SourceSpan(), block, "else" );
			block.Parent.Statement( block );

			blockstat();

			Token endIf = Check( TokenKind.End, match );

			block.SetSourceSpan( new SourceSpan( elseToken.SourceSpan.Start, endIf.SourceSpan.End ) );
			block = block.Parent;
			
			block.Statement( new MarkLabel( endIf.SourceSpan, ifEnd ) );
		}
		else
		{
			Token endIf = Check( TokenKind.End, match );

			block.SetSourceSpan( new SourceSpan( thenToken.SourceSpan.Start, endIf.SourceSpan.End ) );
			block = block.Parent;

			block.Statement( new MarkLabel( endIf.SourceSpan, ifEnd ) );
		}
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
			Error( "{0} expected", LuaLexer.GetTokenName( TokenKind.Identifier ) );
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
			Error( "{0}, {1} or {2} expected", LuaLexer.GetTokenName( TokenKind.In ),
				LuaLexer.GetTokenName( TokenKind.Comma ), LuaLexer.GetTokenName( TokenKind.EqualSign ) );
			return;
		}
		
	}


	void fornum( Token matchFor )
	{
		/*	fornum
				: IDENTIFIER '=' exp ',' exp ( ',' exp )? 'do' block
				;
		*/

		/*		block
					local (for index)	= tonumber( <index> )
					local (for limit)	= tonumber( <limit> )
					local (for step)	= tonumber( <step> )
			fornum:
					bfalse (    ( (for step) > 0 and (for index) <= (for limit) )
							 or ( (for step) < 0 and (for index) >= (for limit) ) ) fornumBreak
					block
						local <index> = (for index)
						...
			fornumContinue:
					end
					(for index) = (for index) + (for step)
					b loopTop
			fornumBreak:
				end
		*/

		Token name = Check( TokenKind.Identifier );
		Check( TokenKind.EqualSign );
		exp();
		Expression start = PopValue();
		Check( TokenKind.Comma );
		exp();
		Expression limit = PopValue();

		Expression step = null;
		if ( Test( TokenKind.Comma ) )
		{
			exp();
			step = PopValue();
		}
		else
		{
			step = new Literal( matchFor.SourceSpan, (int)1 );
		}

		Token doToken = Check( TokenKind.Do );

		
		// Construct AST.

		SourceSpan s			= new SourceSpan( matchFor.SourceSpan.Start, doToken.SourceSpan.End );
		LabelAST fornum			= new LabelAST( "fornum" );			function.Label( fornum );
		LabelAST fornumBreak	= new LabelAST( "fornumBreak" );	function.Label( fornumBreak );
		LabelAST fornumContinue	= new LabelAST( "fornumContinue" );	function.Label( fornumContinue );

		block = new Block( new SourceSpan(), block, "fornum" );
		block.Parent.Statement( block );

		
		// Delcare internal variables.

		Variable forIndex = new Variable( "(for index)" );
		function.Local( forIndex ); block.Local( forIndex );
		Variable forLimit = new Variable( "(for limit)" );
		function.Local( forLimit ); block.Local( forLimit );
		Variable forStep  = new Variable( "(for step)" );
		function.Local( forStep ); block.Local( forStep );

		Expression startExpression = new ToNumber( start.SourceSpan, start );
		Expression limitExpression = new ToNumber( limit.SourceSpan, limit );
		Expression stepExpression  = new ToNumber( step.SourceSpan, step );

		block.Statement( new Declare( start.SourceSpan, forIndex, startExpression ) );
		block.Statement( new Declare( limit.SourceSpan, forLimit, limitExpression ) );
		block.Statement( new Declare( step.SourceSpan, forStep, stepExpression ) );


		// Test expression.

		Expression condition =
			new Logical( s, LogicalOp.Or,
				new Logical( s, LogicalOp.And, 
					new Comparison( s, ComparisonOp.GreaterThan,
						new LocalRef( s, forStep ),
						new Literal( s, 0.0 ) ),
					new Comparison( s, ComparisonOp.LessThanOrEqual,
						new LocalRef( s, forIndex ),
						new LocalRef( s, forLimit ) ) ),
				new Logical( s, LogicalOp.And,
					new Comparison( s, ComparisonOp.LessThan,
						new LocalRef( s, forStep ),
						new Literal( s, 0.0 ) ),
					new Comparison( s, ComparisonOp.GreaterThanOrEqual,
						new LocalRef( s, forIndex ),
						new LocalRef( s, forLimit ) ) ) );


		// Loop body.

		block.Statement( new MarkLabel( s, fornum ) );
		block.Statement( new Test( s, condition, fornumBreak ) );

		block = new Block( new SourceSpan(), block, "fornumBody" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, fornumBreak, fornumContinue );
		

		// Declare index variable.

		Variable userIndex = new Variable( (string)name.Value );
		function.Local( userIndex ); block.Local( userIndex );
		Expression indexExpression = new LocalRef( s, forIndex );
		block.Statement( new Declare( s, userIndex, indexExpression ) );


		// Loop body.

		blockstat();

		Token endFor = Check( TokenKind.End, matchFor );


		// Close AST.

		s = endFor.SourceSpan;

		block.Statement( new MarkLabel( s, fornumContinue ) );

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( doToken.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;

		
		// Increment index.

		Expression indexVariable = new LocalRef( s, forIndex );
		Expression incrementExpression  =
			new Binary( s, BinaryOp.Add,
				new LocalRef( s, forIndex ),
				new LocalRef( s, forStep ) );

		block.Statement( new Assign( s, indexVariable, incrementExpression ) );


		// Finish loop.

		block.Statement( new Branch( s, fornum ) );
		block.Statement( new MarkLabel( s, fornumBreak ) );

		block.SetSourceSpan( new SourceSpan( matchFor.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;
	}


	void forlist( Token matchFor )
	{
		/*	forlist
				: IDENTIFIER ( ',' IDENTIFIER )* 'in' explist 'do' block
				;
		*/

		/*		block
					local (for generator), (for state), (for control) = <expressionlist>
			forlistContinue:
					block
						local <variablelist> = (for generator) ( (for state), (for control) )
						(for control) = <variablelist>[ 0 ]
						btrue ( (for control) == nil ) forlistBreak
						...
					end
					b forlistContinue
			forlistBreak:
				end
		*/	
		
		IList< Token > namelist = new List< Token >();
		namelist.Add( Check( TokenKind.Identifier ) );
		
		while ( Test( TokenKind.Comma ) )
		{
			namelist.Add( Check( TokenKind.Identifier ) );
		}

		Check( TokenKind.In );
		int expressioncount = explist();

		Token doToken = Check( TokenKind.Do );


		// Build AST

		SourceSpan s				= new SourceSpan( matchFor.SourceSpan.Start, doToken.SourceSpan.End );
		LabelAST forlistBreak		= new LabelAST( "forlistBreak" );		function.Label( forlistBreak );
		LabelAST forlistContinue	= new LabelAST( "forlistContinue" );	function.Label( forlistContinue );

		block = new Block( new SourceSpan(), block, "forlist" );
		block.Parent.Statement( block );


		// Declare internal variables.

		Token[] internalNameList = new Token[]
		{
			new Token( s, TokenKind.Identifier, "(for generator)" ),
			new Token( s, TokenKind.Identifier, "(for state)" ),
			new Token( s, TokenKind.Identifier, "(for control)" ),
		};
		LocalStatementAST( s, internalNameList, expressioncount );

		Debug.Assert( block.Locals.Count == 3 );
		Variable forGenerator	= block.Locals[ 0 ];
		Variable forState		= block.Locals[ 1 ];
		Variable forControl		= block.Locals[ 2 ];
		Debug.Assert( forGenerator.Name	== "(for generator)" );
		Debug.Assert( forState.Name		== "(for state)" );
		Debug.Assert( forControl.Name	== "(for control)" );


		// Loop block

		block.Statement( new MarkLabel( s, forlistContinue ) );

		block = new Block( new SourceSpan(), block, "forlistBody" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, forlistBreak, forlistContinue );


		// Generator expression.

		Expression generator =
			new Call( s,
				new LocalRef( s, forGenerator ),
				new Expression[] {
					new LocalRef( s, forState ),
					new LocalRef( s, forControl ) },
				null );

		PushExpression( generator );
		LocalStatementAST( s, namelist, 1 );

		Debug.Assert( block.Locals.Count == namelist.Count );
		Variable userControl = block.Locals[ 0 ];


		// Update control and test.

		Expression condition =
			new Comparison( s, ComparisonOp.NotEqual,
				new LocalRef( s, forControl ),
				new Literal( s, null ) );

		Expression controlVariable	= new LocalRef( s, forControl );
		Expression updateExpression	= new LocalRef( s, userControl );

		block.Statement( new Assign( s, controlVariable, updateExpression ) );
		block.Statement( new Test( s, condition, forlistBreak ) );


		// Loop body.

		blockstat();

		Token endFor = Check( TokenKind.End, matchFor );


		// Close AST.

		s = endFor.SourceSpan;

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( doToken.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;

		block.Statement( new Branch( s, forlistContinue ) );
		block.Statement( new MarkLabel( s, forlistBreak ) );

		block.SetSourceSpan( new SourceSpan( matchFor.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;
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

		Token variableName = Check( TokenKind.Identifier );
		Expression variable = Lookup( variableName );
		StringBuilder functionName = new StringBuilder( (string)variableName.Value );

		while ( Get() == TokenKind.FullStop )
		{
			Token fullStop = Check( TokenKind.FullStop );
			Token key = Check( TokenKind.Identifier );
			
			variable = new Index(
				new SourceSpan( variable.SourceSpan.Start, key.SourceSpan.End ),
				variable,
				new Literal( key.SourceSpan, (string)key.Value ) );

			functionName.Append( "." );
			functionName.Append( (string)key.Value );
		}

		Token? methodName = null;
		if ( Get() == TokenKind.Colon )
		{
			Token colon = Check( TokenKind.Colon );
			Token methodNameToken = Check( TokenKind.Identifier );

			variable = new Index(
				new SourceSpan( variable.SourceSpan.Start, methodNameToken.SourceSpan.End ),
				variable,
				new Literal( methodNameToken.SourceSpan, (string)methodNameToken.Value ) );

			functionName.Append( ":" );
			functionName.Append( (string)methodNameToken.Value );

			methodName = methodNameToken;
		}

		funcbody( matchFunction, functionName.ToString(), methodName );
		Expression f = PopValue();
	
		block.Statement( new Assign( variable.SourceSpan, variable, f ) );
	}


	void localfunc()
	{
		/*	localfunc
				: 'local' 'function' IDENTIFIER funcbody
				;
		*/

		Check( TokenKind.Local );
		Token matchFunction = Check( TokenKind.Function );
		Token localName = Check( TokenKind.Identifier );

		funcbody( matchFunction, (string)localName.Value, null );
		Expression f = PopValue();

		Variable local = new Variable( (string)localName.Value );
		function.Local( local ); block.Local( local );
		block.Statement( new Declare( localName.SourceSpan, local, f ) );
	}


	void funcbody( Token matchFunction, string functionName, Token? methodName )
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

		// Function.

		function = new FunctionAST( functionName, function );
		function.Parent.ChildFunction( function );
		block = new Block( new SourceSpan(), block, "function" );
		function.SetBlock( block );


		// Parameters.

		if ( Get() == TokenKind.LeftParenthesis || Get() == TokenKind.NewlineLeftParenthesis )
		{
			Next();
		}
		else
		{
			Error( "{0} expected", LuaLexer.GetTokenName( TokenKind.LeftParenthesis ) );
		}

		if ( methodName.HasValue )
		{
			Variable self = new Variable( "self" );
			function.Parameter( self );
		}

		if ( Get() != TokenKind.RightParenthesis )
		{
			while ( true )
			{
				// param

				if ( Get() == TokenKind.Identifier )
				{
					Token parameterToken = Check( TokenKind.Identifier );
					Variable parameter = new Variable( (string)parameterToken.Value );
					function.Parameter( parameter );
				}
				else if ( Get() == TokenKind.Ellipsis )
				{
					Check( TokenKind.Ellipsis );
					function.SetVararg();
					break;
				}
				else
				{
					Error( "{0} or {1} expected", LuaLexer.GetTokenName( TokenKind.Identifier ),
						LuaLexer.GetTokenName( TokenKind.Ellipsis ) );
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


		// Statements.

		blockstat();

		Token endFunction = Check( TokenKind.End, matchFunction );


		// End function.

		Statement lastStatement = null;
		if ( block.Statements.Count > 0 )
		{
			lastStatement = block.Statements[ block.Statements.Count - 1 ];
		}

		if (    !( lastStatement is Return )
			 && !( lastStatement is ReturnMultipleValues ) )
		{
			block.Statement( new Return( endFunction.SourceSpan,
				new Literal( endFunction.SourceSpan, null ) ) );
		}

		SourceSpan s = new SourceSpan( matchFunction.SourceSpan.Start, endFunction.SourceSpan.End );

		block.SetSourceSpan( s );
		block = block.Parent;
		FunctionAST result = function;
		function = function.Parent;

		
		// Push a function expression.

		PushExpression( new FunctionClosure( s, result ) );

	}


	void localstat()
	{
		/*	localstat
				: 'local' IDENTIFIER ( ',' IDENTIFIER )* ( '=' explist )?
				;
		*/

		Token local = Check( TokenKind.Local );
		SourceLocation end;


		// Have to declare new variables after evaluating the explist, so
		// that local x = x will find the value of x in the enclosing scope.

		IList< Token > namelist = new List< Token >();
		while ( true )
		{
			// IDENTIFIER

			Token name = Check( TokenKind.Identifier );
			namelist.Add( name );
			end = name.SourceSpan.End;
						

			// ','

			if ( ! Test( TokenKind.Comma ) )
			{
				break;
			}
		}


		// explist

		int expressioncount = 0;
		if ( Test( TokenKind.EqualSign ) )
		{
			expressioncount = explist();
			end = expression.Peek().SourceSpan.End;
		}


		// Perform assignment.

		SourceSpan s = new SourceSpan( local.SourceSpan.Start, end );
		LocalStatementAST( s, namelist, expressioncount );
	}

	void LocalStatementAST( SourceSpan s, IList< Token > namelist, int expressioncount )
	{
		
		// Declare locals.

		IList< Variable > locallist = new Variable[ namelist.Count ];
		for ( int variable = 0; variable < namelist.Count; ++variable )
		{
			Variable local = new Variable( (string)namelist[ variable ].Value );
			function.Local( local ); block.Local( local );
			locallist[ variable ] = local;
		}
		

		// Assign.

		if ( expressioncount == 0 )
		{

			// Just declare all the variables.

			for ( int local = 0; local < locallist.Count; ++local )
			{
				block.Statement( new Declare( s, locallist[ local ],
					new Literal( s, null ) ) );
			}

		}
		else if ( namelist.Count > expressioncount )
		{

			// Check for multiple results on the last expression.
						
			Expression multipleValues = PopMultipleValues();
			if ( multipleValues != null )
			{
				expressioncount -= 1;
			}


			// Assign expressions.

			IList< Expression > expressionlist = PopValues( expressioncount );	
			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Expression value = expressionlist[ expression ];
				block.Statement( new Declare( s, locallist[ expression ], value ) );
			}


			// Deal with remaining variables.

			if ( multipleValues != null )
			{
				if ( multipleValues is Vararg )
				{
					// Assign from vararg.

					for ( int local = expressionlist.Count; local < locallist.Count; ++local )
					{
						block.Statement( new Declare( s, locallist[ local ],
							new VarargElement( s, local - expressionlist.Count ) ) );
					}
				}
				else
				{
					// Evaluate multiple values.

					block.Statement( new Assign( s, new ValueList( s ), multipleValues ) );


					// Assign from value list.

					for ( int local = expressionlist.Count; local < locallist.Count; ++local )
					{
						block.Statement( new Declare( s, locallist[ local ],
							new ValueListElement( s, local - expressionlist.Count ) ) );
					}
				}
			}
			else
			{
				// No extra values, just declare.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					block.Statement( new Declare( s, locallist[ local ],
						new Literal( s, null ) ) );
				}
			}

		}
		else
		{
			
			// Assign locals.

			IList< Expression > expressionlist = PopValues( expressioncount );
			for ( int local = 0; local < locallist.Count; ++local )
			{
				Expression value = expressionlist[ local ];
				block.Statement( new Declare( s, locallist[ local ], value ) );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = locallist.Count; expression < expressionlist.Count; ++expression )
			{
				Expression value = expressionlist[ expression ];
				block.Statement( new Evaluate( s, value ) );
			}

		}

	}



	enum ExpressionType
	{
		None,
		FunctionCall,
		Assignable,
	}
		
	void exprstat()
	{
		/*	exprstat
				: primaryexp { not a function call } ( ',' primaryexp )* '=' explist
				| primaryexp { function call }
				;
		*/

		
		ExpressionType expressionType = primaryexp();
		
		if ( expressionType == ExpressionType.FunctionCall )
		{
			Expression e = PopValue();
			block.Statement( new Evaluate( e.SourceSpan, e ) );
			return;
		}
		
		
		bool assignmentError = false;
		if ( expressionType != ExpressionType.Assignable )
		{
			Error( "Expression is not assignable" );
			assignmentError = true;
		}


		Expression firstVariable = expression.Peek();
		int variablecount = 1;
		while ( Test( TokenKind.Comma ) )
		{
			variablecount += 1;
			expressionType = primaryexp();
			if ( expressionType != ExpressionType.Assignable )
			{
				Error( "Expression is not assignable" );
				assignmentError = true;
			}
		}

		
		Token equalSign = Check( TokenKind.EqualSign );
		int expressioncount = explist();

		if ( assignmentError )
		{
			PopValues( expressioncount );
			PopValues( variablecount );
			return;
		}


		// Build AST

		SourceSpan s = new SourceSpan( firstVariable.SourceSpan.Start, expression.Peek().SourceSpan.End );
		AssignStatementAST( s, variablecount, expressioncount );
	}

	void AssignStatementAST( SourceSpan s, int variablecount, int expressioncount )
	{
		Debug.Assert( variablecount > 0 && expressioncount > 0 );


		// Simpler code when there are no dependencies between expressions
		
		if ( variablecount == 1 && expressioncount == 1 )
		{
			Expression expression	= PopValue();
			Expression variable		= PopValue();			
			block.Statement( new Assign( s, variable, expression ) );	
			return;
		}


		// Pop expressions and check for multiple values.

		Expression multipleValues = null;
		if ( variablecount > expressioncount )
		{
			multipleValues = PopMultipleValues();
			if ( multipleValues != null )
			{
				expressioncount -= 1;
			}
		}

		IList< Expression > expressionlist = PopValues( expressioncount );

		
		// Transform assignment expressions.

		IList< Expression > vlist = PopValues( variablecount );
		IList< Expression > variablelist = new Expression[ variablecount ];
		for ( int variable = 0; variable < variablelist.Count; ++variable )
		{
			Index index = vlist[ variable ] as Index;
			if ( index != null )
			{
				Temporary temporaryTable	= new Temporary( index.Table.SourceSpan );
				Temporary temporaryKey		= new Temporary( index.Key.SourceSpan );

				block.Statement( new Assign( index.SourceSpan, temporaryTable, index.Table ) );
				block.Statement( new Assign( index.SourceSpan, temporaryKey, index.Key ) );

				variablelist[ variable ] = new Index( index.SourceSpan, temporaryTable, temporaryKey );
			}
			else
			{
				variablelist[ variable ] = vlist[ variable ];
			}
		}
		

		// Assign.

		if ( variablelist.Count > expressionlist.Count )
		{
			// Evaluate expression list and assign to temporaries.

			IList< Expression > temporarylist = new Expression[ expressionlist.Count ];
			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Expression e			= expressionlist[ expression ];
				Expression temporary	= new Temporary( e.SourceSpan );
			
				block.Statement( new Assign( s, temporary, e ) );
				
				temporarylist[ expression ] = temporary;
			}


			// Evaluate last (multiple value) expression.

			if ( multipleValues != null )
			{
				block.Statement( new Assign( s, new ValueList( s ), multipleValues ) );
			}


			// Perform assignments.

			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				block.Statement( new Assign( s,
					variablelist[ expression ], temporarylist[ expression ] ) );
			}
			
			if ( multipleValues != null )
			{
				if ( multipleValues is Vararg )
				{
					// Assign from vararg.

					for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
					{
						block.Statement( new Assign( s, variablelist[ variable ],
							new VarargElement( s, variable - expressionlist.Count ) ) );
					}
				}
				else
				{
					// Evaluate multiple values.

					block.Statement( new Assign( s, new ValueList( s ), multipleValues ) );


					// Assign from value list.

					for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
					{
						block.Statement( new Assign( s, variablelist[ variable ],
							new ValueListElement( s, variable - expressionlist.Count ) ) );
					}
				}
			}
			else
			{
				// No extra values, assign null.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					block.Statement( new Assign( s, variablelist[ variable ],
						new Literal( s, null ) ) );
				}
			}
		}
		else
		{

			// Evaluate expression list and assign to temporaries.

			IList< Expression > temporarylist = new Expression[ variablelist.Count ];
			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				Expression e			= expressionlist[ variable ];
				Expression temporary	= new Temporary( e.SourceSpan );

				block.Statement( new Assign( s, temporary, e ) );
				
				temporarylist[ variable ] = temporary;
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = variablelist.Count; expression < expressionlist.Count; ++expression )
			{
				Expression e = expressionlist[ expression ];
				block.Statement( new Evaluate( e.SourceSpan, e ) );
			}


			// Preform assignments.

			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				block.Statement( new Assign( s, variablelist[ variable ], temporarylist[ variable ] ) );
			}

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

		if ( expressioncount == 0 )
		{
			// Return null.

			SourceSpan s = returnToken.SourceSpan;
			block.Statement( new Return( s, new Literal( s, null ) ) );
		}
		else
		{
			SourceSpan s = new SourceSpan( returnToken.SourceSpan.Start, expression.Peek().SourceSpan.End );


			// Check for multiple results.

			Expression resultValues = PopMultipleValues();
			if ( resultValues != null )
			{
				expressioncount -= 1;
			}
			

			// Return.

			if ( resultValues == null && expressioncount == 1 )
			{
				Expression result = PopValue();
				block.Statement( new Return( s, result ) );
			}
			else
			{
				IList< Expression > results = PopValues( expressioncount );
				block.Statement( new ReturnMultipleValues( s, results, resultValues ) );
				if ( ! function.ReturnsMultipleValues )
				{
					function.SetReturnsMultipleValues();
				}
			}
		}
	}


	void breakstat()
	{
		/*	breakstat
				: 'break'
				;
		*/

		Token breakToken = Check( TokenKind.Break );

		if ( ( loopScope == null ) || ( loopScope.Function != function ) )
		{
			Error( "No loop to break" );
			return;
		}

		block.Statement( new Branch( breakToken.SourceSpan, loopScope.Break ) );
	}


	void continuestat()
	{
		/*	continuestat
				: 'continue'
				;
		*/

		Token continueToken = Check( TokenKind.Continue );

		if ( ( loopScope == null ) || ( loopScope.Function != function ) )
		{
			Error( "No loop to continue" );
			return;
		}

		block.Statement( new Branch( continueToken.SourceSpan, loopScope.Continue ) );
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
			Expression operand = PopValue();

			SourceSpan s = new SourceSpan( operatorToken.SourceSpan.Start, operand.SourceSpan.End );
			Expression e = null;

			switch ( operatorToken.Kind )
			{
			case TokenKind.HyphenMinus:			e = new Unary( s, UnaryOp.Minus, operand );								break;
			case TokenKind.NumberSign:			e = new Unary( s, UnaryOp.Length, operand );							break;
			case TokenKind.Not:					e = new Not( s, operand );												break;
			}

			Debug.Assert( e != null );
			PushExpression( e );
		}
		else
		{
			simpleexp();
		}


		// Check for binary operators.

		Operator binaryOperator;
		while ( binaryOperators.TryGetValue( Get(), out binaryOperator ) && binaryOperator.LeftPriority > limit )
		{
			Expression left = PopValue();
			Token operatorToken = Next();
			subexpr( binaryOperator.RightPriority );
			Expression right = PopValue();

			SourceSpan s = new SourceSpan( left.SourceSpan.Start, right.SourceSpan.End );
			Expression e = null;
			
			switch ( operatorToken.Kind )
			{
			case TokenKind.PlusSign:			e = new Binary( s, BinaryOp.Add, left, right );							break;
			case TokenKind.HyphenMinus:			e = new Binary( s, BinaryOp.Subtract, left, right );					break;
			case TokenKind.Asterisk:			e = new Binary( s, BinaryOp.Multiply, left, right );					break;
			case TokenKind.Solidus:				e = new Binary( s, BinaryOp.Divide, left, right );						break;
			case TokenKind.ReverseSolidus:		e = new Binary( s, BinaryOp.IntegerDivide, left, right );				break;
			case TokenKind.PercentSign:			e = new Binary( s, BinaryOp.Modulus, left, right );						break;
			case TokenKind.CircumflexAccent:	e = new Binary( s, BinaryOp.RaiseToPower, left, right );				break;
			case TokenKind.Concatenate:			e = new Binary( s, BinaryOp.Concatenate, left, right );					break;

			case TokenKind.LogicalEqual:		e = new Comparison( s, ComparisonOp.Equal, left, right );				break;
			case TokenKind.NotEqual:			e = new Comparison( s, ComparisonOp.NotEqual, left, right );			break;
			case TokenKind.LessThanSign:		e = new Comparison( s, ComparisonOp.LessThan, left, right );			break;
			case TokenKind.GreaterThanSign:		e = new Comparison( s, ComparisonOp.GreaterThan, left, right );			break;
			case TokenKind.LessThanOrEqual:		e = new Comparison( s, ComparisonOp.LessThanOrEqual, left, right );		break;
			case TokenKind.GreaterThanOrEqual:	e = new Comparison( s, ComparisonOp.GreaterThanOrEqual, left, right );	break;

			case TokenKind.And:					e = new Logical( s, LogicalOp.And, left, right );						break;
			case TokenKind.Or:					e = new Logical( s, LogicalOp.Or, left, right );						break;
			}

			Debug.Assert( e != null );
			PushExpression( e );
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
			PushExpression( new Literal( stringToken.SourceSpan, stringToken.Value ) );
			break;

		case TokenKind.Number:
			Token numberToken = Check( TokenKind.Number );
			PushExpression( new Literal( numberToken.SourceSpan, numberToken.Value ) );
			break;

		case TokenKind.LeftCurlyBracket:
			constructor();
			break;

		case TokenKind.Function:
			functionexp();
			break;

		case TokenKind.Nil:
			Token nilToken = Check( TokenKind.Nil );
			PushExpression( new Literal( nilToken.SourceSpan, null ) );
			break;

		case TokenKind.True:
			Token trueToken = Check( TokenKind.True );
			PushExpression( new Literal( trueToken.SourceSpan, true ) );
			break;

		case TokenKind.False:
			Token falseToken = Check( TokenKind.False );
			PushExpression( new Literal( falseToken.SourceSpan, false ) );
			break;

		case TokenKind.Ellipsis:
			Token ellipsisToken = Check( TokenKind.Ellipsis );
			if ( function.IsVararg )
			{
				PushExpression( new Vararg( ellipsisToken.SourceSpan ) );
			}
			else
			{
				PushExpression( new Literal( ellipsisToken.SourceSpan, null ) );
				Error( "Cannot use '{0}' outside a vararg function", LuaLexer.GetTokenName( TokenKind.Ellipsis ) );
			}
			break;

		default:
			primaryexp();
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
		Temporary constructorValue = new Temporary( new SourceSpan() );
		Constructor constructor = new Constructor( new SourceSpan(), constructorValue );
		block.Statement( constructor );

		SourceSpan s;
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
				Expression value = PopValue();

				constructor.IncrementHashCount();
				constructor.Statement(
					new Assign( new SourceSpan( key.SourceSpan.Start, value.SourceSpan.End ),
						new Index( key.SourceSpan,
							constructorValue,
							new Literal( key.SourceSpan, (string)key.Value ) ),
						value ) );
			}
			else if ( Get() == TokenKind.LeftSquareBracket )
			{
				// recfield

				Token leftBracket = Check( TokenKind.LeftSquareBracket );
				exp();
				Expression key = PopValue();
				Token rightBracket = Check( TokenKind.RightSquareBracket );
				Token equalSign = Check( TokenKind.EqualSign );
				exp();
				Expression value = PopValue();

				s = new SourceSpan( leftBracket.SourceSpan.Start, rightBracket.SourceSpan.End );

				constructor.IncrementHashCount();
				constructor.Statement(
					new Assign( new SourceSpan( leftBracket.SourceSpan.Start, value.SourceSpan.End ),
						new Index( s, constructorValue, key ),
						value ) );
			}
			else
			{
				// listfield

				Token token = GetToken();
				exp();
				if ( Get() != TokenKind.RightCurlyBracket && Lookahead() != TokenKind.RightCurlyBracket )
				{
					// normal field.

					Expression value = PopValue();

					constructor.IncrementArrayCount();
					constructor.Statement(
						new Assign( value.SourceSpan,
							new Index( value.SourceSpan,
								constructorValue,
								new Literal( value.SourceSpan, arrayKey ) ),
							value ) );

					arrayKey += 1;
				}
				else
				{
					// last field.

					Expression values = PopMultipleValues();
					if ( values == null )
					{
						Expression value = PopValue();

						constructor.IncrementArrayCount();
						constructor.Statement(
							new Assign( value.SourceSpan,
								new Index( value.SourceSpan,
									constructorValue,
									new Literal( value.SourceSpan, arrayKey ) ),
								value ) );
					}
					else
					{
						block.Statement( new IndexMultipleValues( values.SourceSpan, constructorValue, arrayKey, values ) );
					}
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
		s = new SourceSpan( matchConstructor.SourceSpan.Start, endConstructor.SourceSpan.End );
		constructor.SetSourceSpan( s );
		constructorValue.SetSourceSpan( s );
		PushExpression( constructorValue );
	}


	void functionexp()
	{
		/*	functionexp
				: 'function' funcbody
				;
		*/

		Token matchFunction = Check( TokenKind.Function );
		funcbody( matchFunction, null, null );
	}


	ExpressionType primaryexp()
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

		ExpressionType expressionType = prefixexp();
		
		while ( true )
		{
			Token name;

			switch ( Get() )
			{
			// lookup.
			case TokenKind.FullStop:
			{
				expressionType = ExpressionType.Assignable;
				
				Expression table = PopValue();
				Token fullStop = Check( TokenKind.FullStop );
				name = Check( TokenKind.Identifier );
				
				SourceSpan s = new SourceSpan( table.SourceSpan.Start, name.SourceSpan.End );
				Expression key = new Literal( name.SourceSpan, (string)name.Value );

				PushExpression( new Index( s, table, key ) ); 
				break;
			}
				

			// array lookup.
			case TokenKind.LeftSquareBracket:
			{
				expressionType = ExpressionType.Assignable;
			
				Expression table = PopValue();
				Token leftBracket = Check( TokenKind.LeftSquareBracket );
				exp();
				Expression key = PopValue();
				Token rightBracket = Check( TokenKind.RightSquareBracket );

				SourceSpan s = new SourceSpan( table.SourceSpan.Start, rightBracket.SourceSpan.End );

				PushExpression( new Index( s, table, key ) );
				break;
			}

			// call
			case TokenKind.LeftParenthesis:
			case TokenKind.NewlineLeftParenthesis:
			case TokenKind.LeftCurlyBracket:
			case TokenKind.String:
			{
				expressionType = ExpressionType.FunctionCall;
			
				Expression function = PopValue();
				callexpr( function, null );
				
				break;
			}

			// selfcall
			case TokenKind.Colon:
			{
				expressionType = ExpressionType.FunctionCall;

				Expression function = PopValue();
				Check( TokenKind.Colon );
				name = Check( TokenKind.Identifier );
				callexpr( function, (string)name.Value );
				
				break;
			}

			// no more postfixes
			default:
			{
				return expressionType;
			}

			}

		}
	}


	void callexpr( Expression function, string methodName )
	{
		/*	funcargs
				: { no newline } '(' ( explist )? ')'
				| constructor
				| string
				;
		*/


		int argumentcount = 0;
		
		SourceLocation end;

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
			Token rightParenthesis = Check( TokenKind.RightParenthesis, matchParenthesis );
			end = rightParenthesis.SourceSpan.End;
			break;

		case TokenKind.LeftCurlyBracket:
			constructor();
			end = expression.Peek().SourceSpan.End;
			argumentcount = 1;
			break;
		
		case TokenKind.String:
			Token stringToken = Check( TokenKind.String );
			PushExpression( new Literal( stringToken.SourceSpan, stringToken.Value ) );
			end = stringToken.SourceSpan.End;
			argumentcount = 1;
			break;

		default:
			Error( "Function arguments expected" );
			return;
		}


		SourceSpan s = new SourceSpan( function.SourceSpan.Start, end );
	
		// Check for multiple results.
		Expression values = null;
		if ( argumentcount > 0 )
		{
			values = PopMultipleValues();
			if ( values != null )
			{
				argumentcount -= 1;
			}
		}

		// Create a call or self-call expression.
		if ( methodName == null )
		{
			PushExpression( new Call( s, function, PopValues( argumentcount ), values ) );
		}
		else
		{
			PushExpression( new CallSelf( s, function, methodName, PopValues( argumentcount ), values ) );
		}
	}


	ExpressionType prefixexp()
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
		{
			Token matchParenthesis = Next();
			exp();
			Token rightParenthesis = Check( TokenKind.RightParenthesis, matchParenthesis );

			Expression e = PopValue();
			e.SetSourceSpan( new SourceSpan( matchParenthesis.SourceSpan.Start, rightParenthesis.SourceSpan.End ) );
			PushExpression( new Nested( e.SourceSpan, e ) );
		
			return ExpressionType.None;
		}
			
		case TokenKind.Identifier:
		{
			Token nameToken = Check( TokenKind.Identifier );
			PushExpression( Lookup( nameToken ) );
			return ExpressionType.Assignable;
		}

		default:
		{
			PushExpression( new Literal( GetToken().SourceSpan, null ) );
			Error( "Unexpected token '{0}'", LuaLexer.GetTokenName( Get() ) );
			return ExpressionType.None;
		}

		}

	}





	// Scopes.

	class LoopScope
	{
		public FunctionAST	Function	{ get; private set; }
		public LoopScope	Parent		{ get; private set; }
		public LabelAST		Break		{ get; private set; }
		public LabelAST		Continue	{ get; private set; }


		public LoopScope( FunctionAST function, LoopScope parent, LabelAST b, LabelAST c )
		{
			Function	= function;
			Parent		= parent;
			Break		= b;
			Continue	= c;
		}
	}


	Expression Lookup( Token nameToken )
	{
		string name = (string)nameToken.Value;

		// Search through scopes.
		FunctionAST f = function;
		for ( Block b = block; b != null; b = b.Parent )
		{
			for ( int i = b.Locals.Count - 1; i >= 0; --i )
			{
				Variable variable = b.Locals[ i ];

				if ( variable.Name == name )
				{
					if ( f == function )
					{
						// Is a local.
						return new LocalRef( nameToken.SourceSpan, variable );
					}
					else
					{
						// Is an upval.
						variable.SetUpVal();
						function.UpVal( variable );
						return new UpValRef( nameToken.SourceSpan, variable );
					}
				}
			}

			// Check parameters.
			if ( b == f.Block )
			{
				for ( int i = f.Parameters.Count - 1; i >= 0; --i )
				{
					Variable variable = f.Parameters[ i ];

					if ( variable.Name == name )
					{
						if ( f == function )
						{
							// Is a local.
							return new LocalRef( nameToken.SourceSpan, variable );
						}
						else
						{
							// Is an upval.
							variable.SetUpVal();
							function.UpVal( variable );
							return new UpValRef( nameToken.SourceSpan, variable );
						}
					}
				}

				
				// Continue search in outer function.
				f = f.Parent;
			}
		}

		// Is a global.
		return new GlobalRef( nameToken.SourceSpan, name );
	}




	// Converting expressions to single or multiple results.

	class Nested
		:	Expression
	{
		public Expression Expression { get; private set; }


		public Nested( SourceSpan s, Expression expression )
			:	base( s )
		{
			Expression = expression;
		}


		public override void Accept( IExpressionVisitor v )
		{
			throw new NotSupportedException( "Nested expressions are internal parser objects." );
		}

	}


	void PushExpression( Expression e )
	{
		expression.Push( e );
	}

	
	Expression PopValue()
	{
		Expression value = expression.Pop();

		if ( value is Nested )
		{
			value = ( (Nested)value ).Expression;
		}

		if ( value is Vararg )
		{
			value = new VarargElement( value.SourceSpan, 0 );
		}

		return value;
	}


	IList< Expression > PopValues( int count )
	{
		Expression[] result = new Expression[ count ];

		for ( int i = count - 1; i >= 0; --i )
		{
			result[ i ] = PopValue();
		}

		return Array.AsReadOnly( result );
	}


	Expression PopMultipleValues()
	{
		Expression value = expression.Pop();

		if ( value is Call )
		{
			return value;
		}

		if ( value is CallSelf )
		{
			return value;
		}

		if ( value is Vararg )
		{
			return value;
		}

		expression.Push( value );
		return null;
	}




	
	// Token reading.

	TokenKind Get()
	{
		return GetToken().Kind;
	}

	Token GetToken()
	{
		return token;
	}

	TokenKind Lookahead()
	{
		return LookaheadToken().Kind;
	}

	Token LookaheadToken()
	{
		if ( lookahead.Kind == TokenKind.None )
		{
			lookahead = lexer.ReadToken();
		}
		return lookahead;
	}
	
	Token Next()
	{
		Token current = token;

		if ( lookahead.Kind == TokenKind.None )
		{
			token = lexer.ReadToken();
		}
		else
		{
			token		= lookahead;
			lookahead	= new Token();
		}
		
		return current;
	}

	bool Test( TokenKind kind )
	{
		if ( Get() == kind )
		{
			Next();
			return true;
		}
		return false;
	}

	Token Check( TokenKind kind )
	{
		if ( Get() != kind )
		{
			Error( "'{0}' expected", LuaLexer.GetTokenName( kind ) );
			return GetToken();
		}

		return Next();
	}

	Token Check( TokenKind kind, Token matchTo )
	{
		if ( Get() != kind )
		{
			Error( "'{0}' expected to close", LuaLexer.GetTokenName( kind ) );
			Error( matchTo.SourceSpan.Start, "    '{0}' here", LuaLexer.GetTokenName( matchTo.Kind ) );
			return GetToken();
		}

		return Next();
	}



	// Error reporting.

	void Error( string format, params object[] args )
	{
		Error( token.SourceSpan.Start, format, args );
	}

	void Error( SourceLocation l, string format, params object[] args )
	{
		Console.Error.WriteLine( "{0}({1},{2}): error: {3}",
			l.SourceName, l.Line, l.Column, System.String.Format( format, args ) );
		hasError = true;
	}



	
	// Parse stack.

	class ParseStack< T >
	{
		List< T >	list;
		int			top;

		public ParseStack()
		{
			list	= new List< T >();
			top		= 0;
		}

		public int Count
		{
			get { return top; }
		}

		public void Push( T item )
		{
			list.RemoveRange( top, list.Count - top );
			list.Add( item );
			top = list.Count;
		}

		public T Pop()
		{
			top -= 1;
			return list[ top ];
		}

		public IList< T > Pop( int count )
		{
			top -= count;
			return new StackSlice( list, top, count );
		}

		public T Peek()
		{
			return list[ top - 1 ];
		}

		public T Peek( int count )
		{
			return list[ top - 1 - count ];
		}


		// Access to a subrange of the stack, without copying it.

		class StackSlice
			:	IList< T >
		{
			List< T >	list;
			int			start;
			int			count;

			public int	Count		{ get { return count; } }
			public bool	IsReadOnly	{ get { return true; } }

			public T this[ int index ]
			{
				get { return list[ start + index ]; }
				set { list[ start + index ] = value; }
			}
			
			public StackSlice( List< T > list, int start, int count )
			{
				Debug.Assert( start >= 0 );
				Debug.Assert( start + count <= list.Count );

				this.list	= list;
				this.start	= start;
				this.count	= count;
			}

			public int IndexOf( T item )
			{
				return list.IndexOf( item, start, count );
			}

			public bool Contains( T item )
			{
				return list.Contains( item );
			}

			public void CopyTo( T[] array, int arrayIndex )
			{
				list.CopyTo( start, array, arrayIndex, count );
			}

			public IEnumerator<T> GetEnumerator()
			{
				for ( int i = 0; i < count; ++i )
				{
					yield return this[ i ];
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Insert( int index, T item )	{ throw new NotSupportedException( "Collection is read-only" ); }
			public void RemoveAt( int index )		{ throw new NotSupportedException( "Collection is read-only" ); }
			public void Add( T item )				{ throw new NotSupportedException( "Collection is read-only" ); }
			public void Clear()						{ throw new NotSupportedException( "Collection is read-only" ); }
			public bool Remove( T item )			{ throw new NotSupportedException( "Collection is read-only" ); }

		}

	}


}


}

