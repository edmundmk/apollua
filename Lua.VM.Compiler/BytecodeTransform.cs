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

#if false

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

	
static class BytecodeTransform
{

	public static FunctionAST Transform( FunctionAST function )
	{
		return Transform( function, null );
	}

	public static FunctionAST Transform( FunctionAST function, FunctionAST parent )
	{
		// Copy function.

		FunctionAST f = new FunctionAST( function.Name, parent );
		
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

		-->

			scope
			{
				local (for index)	= tonumber( <index> )
				local (for limit)	= tonumber( <limit> )
				local (for step)	= tonumber( <step> )
				forprep (for index), (for limit), (for step)
				block for
				{
					scope
					{
						block forbody
						{
							forindex <index>
		...
						}
						for (for index), (for limit), (for step), <index> continue for
					}
				}
			}
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


}

#endif
}
