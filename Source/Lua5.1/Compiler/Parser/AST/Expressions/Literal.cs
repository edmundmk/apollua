// Literal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class Literal
	:	Expression
{
	public object Value { get; private set; }
	

	public Literal( SourceSpan s, object value )
		:	base( s )
	{
		Value = value;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}