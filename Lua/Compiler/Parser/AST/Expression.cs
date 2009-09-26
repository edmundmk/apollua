// Expression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST
{


/*	Expressions represent the parse tree for expressions.
*/

public abstract class Expression
{
	public SourceSpan SourceSpan { get; private set; }


	public Expression( SourceSpan s )
	{
		SourceSpan = s;
	}


	public void SetSourceSpan( SourceSpan s )
	{
		SourceSpan = s;
	}


	public abstract void Accept( IExpressionVisitor v );
}


}
