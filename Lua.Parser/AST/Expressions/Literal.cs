// Literal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class Literal
	:	Expression
{
	public object Value { get; private set; }
	

	public Literal( SourceSpan s, object value )
		:	base( s )
	{
		Value = value;
	}


	public override void Accept( ExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}