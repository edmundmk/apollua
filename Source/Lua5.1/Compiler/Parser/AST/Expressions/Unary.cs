// Unary.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class Unary
	:	Expression
{
	public UnaryOp		Op			{ get; private set; }
	public Expression	Operand		{ get; private set; }


	public Unary( SourceSpan s, UnaryOp op, Expression operand )
		:	base( s )
	{
		Op		= op;
		Operand	= operand;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}