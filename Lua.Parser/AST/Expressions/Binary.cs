// Binary.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class Binary
	:	Expression
{
	public BinaryOp		Op			{ get; private set; }
	public Expression	Left		{ get; private set; }
	public Expression	Right		{ get; private set; }


	public Binary( SourceSpan s, BinaryOp op, Expression left, Expression right )
		:	base( s )
	{
		Op		= op;
		Left	= left;
		Right	= right;
	}


	public override void Accept( ExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}