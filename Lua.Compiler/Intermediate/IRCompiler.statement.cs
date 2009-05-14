// IRCompiler.statement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
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

	public void Local( SourceLocation l, Scope s, IList< string > namelist, IList< Expression > elist )
	{
		IRCompilerScope scope = (IRCompilerScope)s;
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
				IRExpression value = expressionlist[ expression ];
				Transform( ref value );
				Statement( new DeclareAssign( l, locallist[ expression ], value ) );
			}


			// Check for multiple results on the last expression.

			int last = expressionlist.Count - 1;
			IRExpression lastValue = expressionlist[ last ];
			ExtraArguments extraArguments;
			TransformMultipleValues( ref lastValue, out extraArguments );


			// Assign remaining names.

			if ( extraArguments == ExtraArguments.None )
			{
				// Assign last expression.
				
				Transform( ref lastValue );
				Statement( new DeclareAssign( l, locallist[ last ], lastValue ) );


				// No extra values, just declare.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					Statement( new Declare( l, locallist[ local ] ) );
				}
			}
			else if ( extraArguments == ExtraArguments.UseValueList )
			{
				// Assign from value list.

				for ( int local = expressionlist.Count - 1; local < locallist.Count; ++local )
				{
					Statement( new DeclareAssign( l, locallist[ local ],
						new ValueListElementExpression( l, local - expressionlist.Count + 1 ) ) );
				}

			}
			else if ( extraArguments == ExtraArguments.UseVararg )
			{
				// Assign from vararg.

				for ( int local = expressionlist.Count - 1; local < locallist.Count; ++local )
				{
					Statement( new DeclareAssign( l, locallist[ local ],
						new VarargElementExpression( l, local - expressionlist.Count + 1 ) ) );
				}
			}
		}
		else
		{

			// Assign locals.

			for ( int local = 0; local < locallist.Count; ++local )
			{
				IRExpression value = expressionlist[ local ];
				Transform( ref value );
				Statement( new DeclareAssign( l, locallist[ local ], value ) );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = locallist.Count; expression < expressionlist.Count; ++expression )
			{
				IRExpression value = expressionlist[ expression ];
				Transform( ref value );
				Statement( new Evaluate( l, value ) );
			}

		}
	}



	public void Assignment( SourceLocation l, Scope scope, IList< Expression > vlist, IList< Expression > elist )
	{
		IList< IRExpression > variablelist = CastExpressionList( vlist );
		IList< IRExpression> expressionlist = CastExpressionList( elist );



		// Simpler code when there are no dependencies between expressions

		if ( variablelist.Count == 1 && expressionlist.Count == 1 )
		{
			IRExpression variable	= variablelist[ 0 ];
			IRExpression value		= expressionlist[ 0 ];
			TransformIndependentAssignment( ref variable );
			TransformAssignmentValue( variable, ref value );
			Statement( new Assign( l, variable, value ) );
			return;
		}




		// Transform assignment expressions.

		for ( int variable = 0; variable < variablelist.Count; ++variable )
		{
			IRExpression v = variablelist[ variable ];
			TransformDependentAssignment( ref v );
			variablelist[ variable ] = v;
		}



		// Assign.

		if ( variablelist.Count > expressionlist.Count )
		{
			// Evaluate expression list and assign to temporaries.

			IList< IRExpression > temporarylist = new List< IRExpression >( expressionlist.Count );
			for ( int expression = 0; expression < expressionlist.Count - 1; ++expression )
			{
				IRExpression temporary	= new TemporaryExpression( l );
				IRExpression value		= expressionlist[ expression ];
				TransformAssignmentValue( temporary, ref value );
				Statement( new Assign( l, temporary, value ) );
				temporarylist.Add( temporary );
			}


			// Check for multiple results on the last expression.

			int last = expressionlist.Count - 1;
			IRExpression lastValue = expressionlist[ last ];
			ExtraArguments extraArguments;
			TransformMultipleValues( ref lastValue, out extraArguments );


			// Evaluate final expression.

			if ( extraArguments == ExtraArguments.None )
			{
				IRExpression temporary	= new TemporaryExpression( l );
				TransformAssignmentValue( temporary, ref lastValue );
				Statement( new Assign( l, temporary, lastValue ) );
				temporarylist.Add( temporary );
			}


			// Perform assignments.

			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Statement( new Assign( l, variablelist[ expression ], temporarylist[ expression ] ) );
			}

			if ( extraArguments == ExtraArguments.None )
			{
				// Assign last variable.

				Statement( new Assign( l, variablelist[ last ], temporarylist[ last ] ) );
			}
			else if ( extraArguments == ExtraArguments.UseValueList )
			{
				// Assign from value list.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					Statement( new Assign( l, variablelist[ variable ],
						new ValueListElementExpression( l, variable - expressionlist.Count ) ) );
				}

			}
			else if ( extraArguments == ExtraArguments.UseVararg )
			{
				// Assign from vararg.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					Statement( new Assign( l, variablelist[ variable ],
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
				IRExpression temporary	= new TemporaryExpression( l );
				IRExpression value		= expressionlist[ variable ];
				TransformAssignmentValue( temporary, ref value );
				Statement( new Assign( l, temporary, value ) );
				temporarylist.Add( temporary );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = variablelist.Count; expression < expressionlist.Count; ++expression )
			{
				IRExpression e = expressionlist[ expression ];
				Transform( ref e );
				Statement( new Evaluate( l, e ) );
			}


			// Preform assignments.

			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				Statement( new Assign( l, variablelist[ variable ], temporarylist[ variable ] ) );
			}

		}

	}



	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		// Evaluate single call expression.

		IRExpression callExpression = (IRExpression)call;
		Transform( ref callExpression );
		Statement( new Evaluate( l, callExpression ) );
	}



	public void Break( SourceLocation l, Scope loopScope )
	{
		// Break loop scope.

		IRCompilerScope scope = (IRCompilerScope)loopScope;
		scope.Break( l, code.Peek() );
	}



	public void Continue( SourceLocation l, Scope loopScope )
	{
		// Continue loop scope.

		IRCompilerScope scope = (IRCompilerScope)loopScope;
		scope.Continue( l, code.Peek() );
	}



	public void Return( SourceLocation l, Scope functionScope, IList< Expression > elist )
	{
		IList< IRExpression> expressionlist = CastExpressionList( elist );
		
		if ( expressionlist.Count == 0 )
		{
			// Return null.

			IRExpression nullExpression = new LiteralExpression( l, null );
			Transform( ref nullExpression );
			Statement( new Return( l, nullExpression ) );
		}
		else if ( expressionlist.Count == 1 )
		{
			IRExpression value = expressionlist[ 0 ];
			Transform( ref value );

			if ( value.IsSingleValue )
			{
				// Return a single result.
							
				Statement( new Return( l, value ) );
			}
			else
			{
				// Return multiple values (possibly a tail call)

				expressionlist[ 0 ] = value;
				Statement( new ReturnMultipleResults( l, expressionlist, ExtraArguments.None ) );
			}
		}
		else
		{
			// Return multiple results (convert last value to extra arguments).

			for ( int expression = 0; expression < expressionlist.Count - 1; ++expression )
			{
				IRExpression e = expressionlist[ expression ];
				Transform( ref e );
				expressionlist[ expression ] = e;
			}

			int last = expressionlist.Count - 1;
			IRExpression lastValue = expressionlist[ last ];
			ExtraArguments extraArguments;
			TransformMultipleValues( ref lastValue, out extraArguments );

			if ( extraArguments == ExtraArguments.None )
			{
				Transform( ref lastValue );
				expressionlist[ last ] = lastValue;
			}
			else
			{
				expressionlist.RemoveAt( last );
			}

			Statement( new ReturnMultipleResults( l, expressionlist, extraArguments ) );
		}
	}

}


}

