// LocalRef.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class LocalRef
	:	Expression
{
	public Variable Variable { get; private set; }


	public LocalRef( SourceSpan s, Variable variable )
		:	base( s )
	{
		Variable = variable;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}