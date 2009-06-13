// Logical.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{

public class Logical
	:	Expression
{
	public LogicalOp	Op			{ get; private set; }
	public Expression	Left		{ get; private set; }
	public Expression	Right		{ get; private set; }


	public Logical( SourceSpan s, LogicalOp op, Expression left, Expression right )
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