// IRCompiler.scope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lua.Compiler.Frontend;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate.CompilerScope;
using Lua.Compiler.Intermediate.IR;
using Lua.Compiler.Intermediate.IR.Expression;
using Lua.Compiler.Intermediate.IR.Statement;


namespace Lua.Compiler.Intermediate
{


sealed partial class IRCompiler
	:	IParserActions
{

	public Scope Function( SourceLocation l, Scope scope, IList< string > parameternamelist, bool isVararg )
	{
		IRCompilerScope functionScope	= new FunctionScope( isVararg );

		// Link a new IR into.

		IRCode functionCode;
		if ( code.Count > 0 )
		{
			IRCode parent = code.Peek();
			functionCode = new IRCode( parent );
			parent.ChildFunction( functionCode );
		}
		else
		{
			functionCode = new IRCode( null );
		}


		// Set up parameters.

		for ( int parameter = 0; parameter < parameternamelist.Count; ++parameter )
		{
			IRLocal local = new IRLocal( parameternamelist[ parameter ] );
			functionScope.Declare( local );
			functionCode.DeclareParameter( local );
		}
		
		if ( isVararg )
		{
			functionCode.MarkVararg();
		}
		

		code.Push( functionCode );
		return functionScope;
	}

	public Code EndFunction( SourceLocation l, Scope end )
	{
		IRCode functionCode = code.Peek();


		// Add return statement if required.

		if ( ! functionCode.EndsWithReturnStatement() )
		{
			Return( l, end, new List< Expression >() );
		}


		// This function is finished.

		code.Pop();


		return functionCode;
	}



	/*	scope
		{
		...
		}
	*/


	public Scope Do( SourceLocation l, Scope scope )
	{
		Statement( new BeginScope( l ) );
		return new DoScope();
	}

	public void EndDo( SourceLocation l, Scope end )
	{
		Statement( new EndScope( l ) );
	}



	/*	block if
		{
			scope
			{
				test <condition>
				{
	...	
					break if
				}
			}
			scope
			{
				test <condition>
				{
	...
					break if
				}
			}
			scope
			{
		...
			}
		}
	*/

	public Scope If( SourceLocation l, Scope scope, Expression c )
	{
		IRExpression condition = (IRExpression)c;

		Statement( new BeginBlock( l, "if" ) );
		Statement( new BeginScope( l ) );
		Transform( ref condition );
		Statement( new BeginTest( l, condition ) );
		
		return new IfScope();
	}

	public Scope ElseIf( SourceLocation l, Scope scope, Expression c )
	{
		IRExpression condition = (IRExpression)c;

		Statement( new Break( l, "if" ) );
		Statement( new EndTest( l ) );
		Statement( new EndScope( l ) );
		Statement( new BeginScope( l ) );
		Transform( ref condition );
		Statement( new BeginTest( l, condition ) );

		return new IfScope();
	}

	public Scope Else( SourceLocation l, Scope scope )
	{
		Statement( new Break( l, "if" ) );
		Statement( new EndTest( l ) );
		Statement( new EndScope( l ) );
		Statement( new BeginScope( l ) );

		return new DoScope();
	}

	public void EndIf( SourceLocation l, Scope end )
	{
		IRCompilerScope clauseScope = (IRCompilerScope)end;

		if ( clauseScope.IsIfScope )
		{
			Statement( new Break( l, "if" ) );
			Statement( new EndTest( l ) );
		}

		Statement( new EndScope( l ) );
		Statement( new EndBlock( l ) );
	}



	/*	block while
		{
			scope
			{
				test <condition>
				{
	...
					continue while
				}
			}
		}
	*/

	public Scope While( SourceLocation l, Scope scope, Expression c )
	{
		IRExpression condition = (IRExpression)c;

		Statement( new BeginBlock( l, "while" ) );
		Statement( new BeginScope( l ) );
		Transform( ref condition );
		Statement( new BeginTest( l, condition ) );

		return new LoopScope( "while" );
	}

	public void EndWhile( SourceLocation l, Scope end )
	{
		Statement( new Continue( l, "while" ) );
		Statement( new EndTest( l ) );
		Statement( new EndScope( l ) );
		Statement( new EndBlock( l ) );
	}



	/*	block repeat
		{
			scope
			{
				block repeatbody
				{
	...
				}
				test <condition>
				{
					continue repeat
				}
			}
		}
	*/

	public Scope Repeat( SourceLocation l, Scope scope )
	{
		Statement( new BeginBlock( l, "repeat" ) );
		Statement( new BeginScope( l ) );
		Statement( new BeginBlock( l, "repeatbody" ) );

		return new RepeatScope( "repeat", "repeatbody" );
	}

	public void Until( SourceLocation l, Scope scope, Expression c )
	{
		IRExpression condition = (IRExpression)c;

		Statement( new EndBlock( l ) );
		Transform( ref condition );
		Statement( new BeginTest( l, condition ) );
		Statement( new Continue( l, "repeat" ) );
		Statement( new EndTest( l ) );
		Statement( new EndScope( l ) );
		Statement( new EndBlock( l ) );
	}



	/*	scope
		{
			local (for index)	= tonumber( <index> )
			local (for limit)	= tonumber( <limit> )
			local (for step)	= tonumber( <step> )
			block for
			{
				scope
				{
					test (    ( (for step) > 0 and (for index) <= (for limit) )
						   or ( (for step) < 0 and (for index) >= (for limit) ) )
					{
						block forbody
						{
							local <index> = (for index)
	...
						}
						(for index) = (for index) + (for step)
						continue for
					}
				}
			}
		}
	*/

	public Scope For( SourceLocation l, Scope scope, string varname, Expression st, Expression li, Expression sp )
	{
		IRExpression start = (IRExpression)st;
		IRExpression limit = (IRExpression)li;
		IRExpression step  = (IRExpression)sp;


		Statement( new BeginScope( l ) );
		

		// Delcare internal variables.

		IRLocal forIndex = new IRLocal( "(for local)" );
		IRLocal forLimit = new IRLocal( "(for limit)" );
		IRLocal forStep  = new IRLocal( "(for step)" );

		IRExpression startExpression = new ToNumberExpression( l, start );
		IRExpression limitExpression = new ToNumberExpression( l, limit );
		IRExpression stepExpression  = new ToNumberExpression( l, step );

		Transform( ref startExpression );
		Statement( new DeclareAssign( l, forIndex, startExpression ) );
		Transform( ref limitExpression );
		Statement( new DeclareAssign( l, forLimit, limitExpression ) );
		Transform( ref stepExpression );
		Statement( new DeclareAssign( l, forStep, stepExpression ) );


		// Test expression.

		IRExpression test =
			new OrExpression( l,
				new AndExpression( l,
					new ComparisonExpression( l,
						new LocalExpression( l, forStep ),
						new LiteralExpression( l, 0.0 ),
						TokenKind.GreaterThanSign ),
					new ComparisonExpression( l,
						new LocalExpression( l, forIndex ),
						new LocalExpression( l, forLimit ),
						TokenKind.LessThanOrEqual ) ),
				new AndExpression( l,
					new ComparisonExpression( l,
						new LocalExpression( l, forStep ),
						new LiteralExpression( l, 0.0 ),
						TokenKind.LessThanSign ),
					new ComparisonExpression( l,
						new LocalExpression( l, forIndex ),
						new LocalExpression( l, forLimit ),
						TokenKind.GreaterThanOrEqual ) ) );


		// Loop body.

		Statement( new BeginBlock( l, "for" ) );
		Statement( new BeginScope( l ) );
		Transform( ref test );
		Statement( new BeginTest( l, test ) );
		Statement( new BeginBlock( l, "forbody" ) );
		

		// Declare index variable.

		IRCompilerScope forScope = new ForScope( "for", "forbody", forIndex, forLimit, forStep );
		IRLocal userIndex = new IRLocal( varname );
		forScope.Declare( userIndex );
		code.Peek().DeclareLocal( userIndex );
		IRExpression indexExpression = new LocalExpression( l, forIndex );
		Transform( ref indexExpression );
		Statement( new DeclareAssign( l, userIndex, indexExpression ) );
		

		return forScope;
	}

	public void EndFor( SourceLocation l, Scope end )
	{
		ForScope forScope = (ForScope)end;


		Statement( new EndBlock( l ) );	// forbody

		// Increment index.

		IRExpression indexVariable = new LocalExpression( l, forScope.ForIndex );
		IRExpression incrementExpression  =
			new BinaryExpression( l,
				new LocalExpression( l, forScope.ForIndex ),
				new LocalExpression( l, forScope.ForStep ),
				TokenKind.PlusSign );

		TransformIndependentAssignment( ref indexVariable );
		TransformAssignmentValue( indexVariable, ref incrementExpression );
		Statement( new Assign( l, indexVariable, incrementExpression ) );



		// Finish loop.

		Statement( new Continue( l, "for" ) );
		Statement( new EndTest( l ) );
		Statement( new EndScope( l ) );
		Statement( new EndBlock( l ) );	
		Statement( new EndScope( l ) );
	}




	/*	scope
		{
			local (for generator), (for state), (for control) = <expressionlist>
			block forin
			{
				scope
				{
					local <variablelist> = (for generator) ( (for state), (for control) )
					(for control) = <variablelist>[ 0 ]
					test ( (for control) == nil )
					{
						break forin
					}
	...
					continue forin
				}
			}
		}
	*/		

	public Scope ForIn( SourceLocation l, Scope scope, IList< string > variablenamelist, IList< Expression > expressionlist )
	{
		Statement( new BeginScope( l ) );


		// Declare internal variables.

		IRCompilerScope internalScope = new DoScope();
		Local( l, internalScope, new string[]{ "(for generator)", "(for state)", "(for control)" }, expressionlist );
		
		Debug.Assert( internalScope.Locals.Count == 3 );
		IRLocal forGenerator	= (IRLocal)internalScope.Locals[ 0 ];
		IRLocal forState		= (IRLocal)internalScope.Locals[ 1 ];
		IRLocal forControl		= (IRLocal)internalScope.Locals[ 2 ];
		
		Debug.Assert( forGenerator.Name	== "(for generator)" );
		Debug.Assert( forState.Name		== "(for state)" );
		Debug.Assert( forControl.Name	== "(for control)" );


		// For loop block.

		Statement( new BeginBlock( l, "forin" ) );
		Statement( new BeginScope( l ) );


		// Generator expressin.

		IRExpression generator =
			new CallExpression( l,
				new LocalExpression( l, forGenerator ),
				new IRExpression[]{
					new LocalExpression( l, forState ),
					new LocalExpression( l, forControl ) } );


		// Declare user variables.

		IRCompilerScope forInScope = new LoopScope( "forin" );
		Local( l, forInScope, variablenamelist, new IRExpression[]{ generator } );

		Debug.Assert( forInScope.Locals.Count == variablenamelist.Count );
		IRLocal userControl = (IRLocal)forInScope.Locals[ 0 ];
		Debug.Assert( userControl.Name == variablenamelist[ 0 ] );



		// Test expression.

		IRExpression test =
			new ComparisonExpression( l,
				new LocalExpression( l, forControl ),
				new LiteralExpression( l, null ),
				TokenKind.LogicalEqual );


		// Update control and test.

		IRExpression controlVariable	= new LocalExpression( l, forControl );
		IRExpression updateExpression	= new LocalExpression( l, userControl );

		TransformIndependentAssignment( ref controlVariable );
		TransformAssignmentValue( controlVariable, ref updateExpression );
		Statement( new Assign( l, controlVariable, updateExpression ) );
		Transform( ref test );
		Statement( new BeginTest( l, test ) );
		Statement( new Break( l, "forin" ) );
		Statement( new EndTest( l ) );

		
		return forInScope;
	}

	public void EndForIn( SourceLocation l, Scope end )
	{
		Statement( new Continue( l, "forin" ) );
		Statement( new EndScope( l ) );
		Statement( new EndBlock( l ) );
		Statement( new EndScope( l ) );
	}

}


}

