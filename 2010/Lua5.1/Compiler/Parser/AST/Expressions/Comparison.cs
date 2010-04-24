// Comparison.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class Comparison
	:	Expression
{
	public ComparisonOp		Op			{ get; private set; }
	public Expression		Left		{ get; private set; }
	public Expression		Right		{ get; private set; }


	public Comparison( SourceSpan s, ComparisonOp op, Expression left, Expression right )
		:	base( s )
	{
		Op		= op;
		Left	= left;
		Right	= right;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}
