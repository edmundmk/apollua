// BytecodeTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Parser.AST;
using Lua.Compiler.Parser.AST.Expressions;
using Lua.VM.Compiler.AST;
using Lua.VM.Compiler.AST.Expressions;


namespace Lua.VM.Compiler
{


/*	The AST produced by the parser is very close to the bytecode already.  However there
	are a few changes we want to make:

		->	String concatenation operations are merged to concatenate full lists.
*/


public class VMTransform
	:	FunctionTransform
	,	IVMExpressionVisitor
{

	// String concatenation operations are merged to concatenate full lists.

	public override void Visit( Binary e )
	{
		if ( e.Op == BinaryOp.Concatenate )
		{
			List< Expression > operands = new List< Expression >();
			ConcatenateList( operands, e );
			SourceSpan s = new SourceSpan( operands[ 0 ].SourceSpan.Start,
									operands[ operands.Count - 1 ].SourceSpan.End );
			result = new OpcodeConcat( s, operands.AsReadOnly() );
		}
		else
		{
			base.Visit( e );
		}
	}


	void ConcatenateList( List< Expression > operands, Expression e )
	{
		Binary binary = e as Binary;
		if ( ( binary != null ) && ( binary.Op == BinaryOp.Concatenate ) )
		{
			ConcatenateList( operands, binary.Left );
			ConcatenateList( operands, binary.Right );
		}
		else
		{
			operands.Add( Transform( e ) );
		}
	}



	public virtual void Visit( OpcodeConcat e )
	{
		Expression[] operands = new Expression[ e.Operands.Count ];
		for ( int operand = 0; operand < e.Operands.Count; ++operand )
		{
			operands[ operand ] = Transform( e.Operands[ operand ] );
		}
		result = new OpcodeConcat( e.SourceSpan, Array.AsReadOnly( operands ) );
	}

}


}
