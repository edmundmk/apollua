// IRCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate.IR;


namespace Lua.Compiler.Intermediate
{


/*	An intermediate representation is built from the results of parsing.  Each
	function is represented as a list of statements, referencing a tree-based
	representation of expressions.

	All control structures map to configurations of two structural statements:
	blocks (which be continued to emulate loops or broken to skip unnecessary
	clauses) and tests (which are like a single if statement).
	
	Expressions are transformed to insert temporaries in order that:
	  ->  The details of multiple returns, expression lists, and multiple
	      assignment are mapped to simple operations.
	  ->  After every operation, the evaluation stack is empty.
	  ->  We can implement continuations using the technique described in
	      http://www.ccs.neu.edu/scheme/pubs/stackhack4.html,  We hoist
	      function calls out of expressions and complex assignments so that
	      after every function call the only thing on the evaluation stack
	      is the result of the call.
*/
	      

sealed partial class IRCompiler
	:	IParserActions
{
	Stack< IRCode > code;


	public IRCompiler()
	{
		code = new Stack< IRCode >();
	}



	// Helpers.

	void Statement( IRStatement statement )
	{
		code.Peek().Statement( statement );
	}

	void Transform( ref IRExpression expression )
	{
		expression = expression.Transform( code.Peek() );
	}

	void TransformMultipleValues( ref IRExpression expression, out ExtraArguments extraArguments )
	{
		expression = expression.TransformMultipleValues( code.Peek(), out extraArguments );
	}

	void TransformIndependentAssignment( ref IRExpression variable )
	{
		variable = variable.Transform( code.Peek() );
	}

	void TransformDependentAssignment( ref IRExpression variable )
	{
		variable = variable.TransformDependentAssignment( code.Peek() );
	}

	void TransformAssignmentValue( IRExpression variable, ref IRExpression value )
	{
		if ( variable.IsComplexAssignment )
		{
			value = value.TransformSingleValue( code.Peek() );
		}
		else
		{
			value = value.Transform( code.Peek() );
		}
	}



	
	
	IList< IRExpression > CastExpressionList( IList< Expression > list )
	{
		// Create a typecasted copy.

		List< IRExpression > copy = new List< IRExpression >( list.Count );
		for ( int expression = 0; expression < list.Count; ++expression )
		{
			copy.Add( (IRExpression)list[ expression ] );
		}

		return copy;
	}


}


}

