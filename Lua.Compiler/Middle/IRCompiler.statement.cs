// IRCompiler.statement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front;
using Lua.Compiler.Front.AST;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Middle.IR;
using Lua.Compiler.Middle.IR.Expression;
using Lua.Compiler.Middle.IR.Statement;


namespace Lua.Compiler.Middle
{


sealed partial class IRCompiler
	:	IParserActions
{

	public void Local( SourceLocation l, Scope s, IList< string > namelist, IList< Expression > elist )
	{
		IRScope scope = (IRScope)s;
		IList< IRExpression > expressionlist = CastExpressionList( elist );


		// Declare locals.

		IList< IRLocal > locallist = new List< IRLocal >( namelist.Count );
		for ( int variable = 0; variable < namelist.Count; ++variable )
		{
			IRLocal local = new IRLocal( namelist[ variable ] );
			scope.Declare( local );
			code.Peek().DeclareLocal( local );
			locallist.Add( local );
		}
		

		// Assign.

		if ( expressionlist.Count == 0 )
		{
			// Just declare all the variables.

			for ( int local = 0; local < locallist.Count; ++local )
			{
				Statement( new Declare( l, locallist[ local ] ) );
			}
		}
		else if ( locallist.Count > expressionlist.Count )
		{
			// Assign expressions.

			for ( int expression = 0; expression < expressionlist.Count - 1; ++expression )
			{
				Transform( expressionlist[ expression ] );
				Statement( new DeclareAssign( l, locallist[ expression ], expressionlist[ expression ] ) );
			}


			// Check for multiple results on the last expression.

			ExtraArguments extraArguments =
				MultipleResultsExpression.TransformLastExpression( code.Peek(), expressionlist );


			// Assign remaining names.

			if ( extraArguments == ExtraArguments.None )
			{
				// Assign last expression.
				
				int last = expressionlist.Count - 1;
				Transform( expressionlist[ last ] );
				Statement( new DeclareAssign( l, locallist[ last ], expressionlist[ last ] ) );


				// No extra values, just declare.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					Statement( new Declare( l, locallist[ local ] ) );
				}
			}
			else if ( extraArguments == ExtraArguments.UseValueList )
			{
				// Assign from value list.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					Statement( new DeclareAssign( l, locallist[ local ],
						new ValueListElementExpression( l, local - expressionlist.Count ) ) );
				}

			}
			else if ( extraArguments == ExtraArguments.UseVararg )
			{
				// Assign from vararg.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					Statement( new DeclareAssign( l, locallist[ local ],
						new VarargElementExpression( l, local - expressionlist.Count ) ) );
				}
			}
		}
		else
		{

			// Assign locals.

			for ( int local = 0; local < locallist.Count; ++local )
			{
				Transform( expressionlist[ local ] );
				Statement( new DeclareAssign( l, locallist[ local ], expressionlist[ local ] ) );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = locallist.Count; expression < expressionlist.Count; ++expression )
			{
				Transform( expressionlist[ expression ] );
				Statement( new Evaluate( l, expressionlist[ expression ] ) );
			}

		}
	}



	public void Assignment( SourceLocation l, Scope scope, IList< Expression > variablelist, IList< Expression > elist )
	{
		IList< IRExpression> expressionlist = CastExpressionList( elist );


		// Transform assignment expressions.

		for ( int variable = 0; variable < variablelist.Count; ++variable )
		{
			TransformAssign( (IRExpression)variablelist[ variable ] );
		}



		// Assign.

		if ( variablelist.Count > expressionlist.Count )
		{
			// Evaluate expression list and assign to temporaries.

			IList< IRExpression > temporarylist = new List< IRExpression >( expressionlist.Count );
			for ( int expression = 0; expression < expressionlist.Count - 1; ++expression )
			{
				IRExpression temporary = new TemporaryExpression( l );
				Transform( expressionlist[ expression ] );
				Statement( new Assign( l, temporary, expressionlist[ expression ] ) );
				temporarylist.Add( temporary );
			}



			// Check for multiple results on the last expression.

			ExtraArguments extraArguments =
				MultipleResultsExpression.TransformLastExpression( code.Peek(), expressionlist );


			// Evaluate final expression.

			if ( extraArguments == ExtraArguments.None )
			{
				int last = expressionlist.Count - 1;
				IRExpression temporary = new TemporaryExpression( l );
				Transform( expressionlist[ last ] );
				Statement( new Assign( l, temporary, expressionlist[ last ] ) );
				temporarylist.Add( temporary );
			}


			// Perform assignments.

			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Statement( new Assign( l,
					(IRExpression)variablelist[ expression ], temporarylist[ expression ] ) );
			}

			if ( extraArguments == ExtraArguments.UseValueList )
			{
				// Assign from value list.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					Statement( new Assign( l, (IRExpression)variablelist[ variable ],
						new ValueListElementExpression( l, variable - expressionlist.Count ) ) );
				}

			}
			else if ( extraArguments == ExtraArguments.UseVararg )
			{
				// Assign from vararg.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					Statement( new Assign( l, (IRExpression)variablelist[ variable ],
						new VarargElementExpression( l, variable - expressionlist.Count ) ) );
				}
			}
		}
		else
		{
			// Evaluate expression list and assign to temporaries.

			IList< IRExpression > temporarylist = new List< IRExpression >( expressionlist.Count );
			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				IRExpression temporary = new TemporaryExpression( l );
				Transform( expressionlist[ variable ] );
				Statement( new Assign( l, temporary, expressionlist[ variable ] ) );
				temporarylist.Add( temporary );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = variablelist.Count; expression < expressionlist.Count; ++expression )
			{
				Transform( expressionlist[ expression ] );
				Statement( new Evaluate( l, expressionlist[ expression ] ) );
			}


			// Preform assignments.

			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				Statement( new Assign( l,
					(IRExpression)variablelist[ variable ], temporarylist[ variable ] ) );
			}

		}

	}



	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		// Evaluate single call expression.

		IRExpression callExpression = (IRExpression)call;
		Transform( callExpression );
		Statement( new Evaluate( l, callExpression ) );
	}



	public void Break( SourceLocation l, Scope loopScope )
	{
		// Break loop scope.

		IRScope scope = (IRScope)loopScope;
		scope.Break( l, code.Peek() );
	}



	public void Continue( SourceLocation l, Scope loopScope )
	{
		// Continue loop scope.

		IRScope scope = (IRScope)loopScope;
		scope.Continue( l, code.Peek() );
	}



	public void Return( SourceLocation l, Scope functionScope, IList< Expression > elist )
	{
		IList< IRExpression> expressionlist = CastExpressionList( elist );
		
		if ( expressionlist.Count == 1 )
		{
			// Return a single result or tail call.

			Transform( expressionlist[ 0 ] );
			Statement( new Return( l, expressionlist[ 0 ] ) );
		}
		else
		{
			// Return multiple results (convert last value to extra arguments).

			MultipleResultsExpression.TransformExpressionList( code.Peek(), expressionlist );
			ExtraArguments extraArguments =
				MultipleResultsExpression.TransformLastExpression( code.Peek(), expressionlist );
			if ( extraArguments != ExtraArguments.None )
			{
				expressionlist.RemoveAt( expressionlist.Count - 1 );
			}
			Statement( new ReturnMultipleResults( l, expressionlist, extraArguments ) );
		}
	}

}


}

