// BytecodeTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.CLR.Compiler
{


/*	The Lua VM has some specialized bytecodes to implement language features.  However
	our very general AST has moved away from the specifics of the language.  So transform
	some constructions back to more specific statements so the compiler can be as
	simple as possible.

	The specific transformations are:

		->  "for" blocks implemented using FoorLoop and ForPrep
		->  "forin" blocks implemented using TForLoop
		->  Concatenation expressions merged to concatenate whole lists.
		->  SetList used to set multiple array-like keys at once.

*/

#if false
static class BytecodeTransform
{

	public static FunctionAST Transform( FunctionAST function )
	{
		return Transform( function, null, null );
	}

	public static FunctionAST Transform( FunctionAST function, FunctionAST parent, Block parentBlock )
	{
		



		// Copy function.

		Block b = new Block( function.Block.SourceSpan, parentBlock, function.Block.Name );
		FunctionAST f = new FunctionAST( function.Name, parent, b );
		
		foreach ( Variable upval in function.UpVals )
		{
			f.UpVal( upval );
		}

		foreach ( Variable parameter in function.Parameters )
		{
			f.Parameter( parameter );
		}

		if ( function.IsVararg )
		{
			f.SetVararg();
		}

		foreach ( Variable local in function.Locals )
		{
			f.Local( local );
		}


		// Transform general statements into specific ones.
		
		
		/*		block
					local (for index)	= tonumber( <index> )
					local (for limit)	= tonumber( <limit> )
					local (for step)	= tonumber( <step> )
			fornum:
					bfalse (    ( (for step) > 0 and (for index) <= (for limit) )
						 or ( (for step) < 0 and (for index) >= (for limit) ) ) loopBreak
					block
						local <index> = (for index)
						...
			fornumContinue:
					end
					(for index) = (for index) + (for step)
					b loopTop
			fornumBreak:
				end

			-->

				block
					local (for index)	= tonumber( <index> )
					local (for limit)	= tonumber( <limit> )
					local (for step)	= tonumber( <step> )
					forprep (for index), (for limit), (for step) fornumTest
			fornum:
					block
						forindex <index>
						...
			fornumContinue:
					end
			fornumTest:
					for (for index), (for limit), (for step), <index> fornum
			fornumBreak:
				end
		*/




		
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
		
		-->
				
			scope
			{
				local (for generator), (for state), (for control) = <expressionlist>
				tforloopjump
				block forin
				{
					scope
					{
						block forinbody
						{
							forindex <variablelist>
		...
						}				
						tforloop (for generator), (for state), (for control), <variablelist>
					}
				}
			}
		*/




		// Return transformed expression.

		return f;
	}


	sealed class StatementTransform
		:	IStatementVisitor
	{
		Block				block;
		ExpressionTransform	e;
		Statement			result;


		public StatementTransform( Block b )
		{
			block	= b;
			result	= null;
		}


		public Statement Transform( Statement s )
		{
			s.Accept( this );
			Statement r = result;
			result = null;
			return r;
		}

		public Statement TransformBlock( Block b )
		{
			block = new Block( b.SourceSpan, block, b.Name );
			
			// Locals.
			foreach ( Variable local in b.Locals )
			{
				block.Local( local );
			}

			// Statements.
			foreach ( Statement statement in b.Statements )
			{
				block.Statement( Transform( statement ) );
			}
			
			// Return.
			b = block;
			block = block.Parent;
			return b;
		}

	

		public override void Visit( Assign s )
		{
		}

		public override void Visit( Block s )
		{
			if ( s.Name == "fornum" )
			{
				result = TransformForNum( s );
			}
			else if ( s.Name == "forlist" )
			{
				result = TransformForList( s );
			}
			else
			{
				result = TransformBlock( s );
			}
		}

		public override void Visit( Constructor s )
		{
		}

		public override void Visit( Branch s )
		{
		}

		public override void Visit( Declare s )
		{
		}

		public override void Visit( Evaluate s )
		{
		}

		public override void Visit( IndexMultipleValues s )
		{
		}

		public override void Visit( MarkLabel s )
		{
		}

		public override void Visit( Return s )
		{
		}

		public override void Visit( ReturnMultipleValues s )
		{
		}

		public override void Visit( Test s )
		{
		}



		Statement TransformForNum( Block fornum )
		{
			return null;
		}


		Statement TransformForList( Block forlist )
		{
			return null;
		}






	}

	sealed class ExpressionTransform
		:	IExpressionVisitor
	{
		Expression result;
		

		public Expression Transform( Expression e )
		{
			e.Accept( this );
			Expression r = result;
			result = null;
			return r;
		}

	}


}
#endif
}
