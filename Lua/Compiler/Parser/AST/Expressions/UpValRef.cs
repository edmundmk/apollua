// UpValRef.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Compiler.Parser.AST.Expressions
{


public class UpValRef
	:	Expression
{
	public Variable Variable { get; private set; }

	
	public UpValRef( SourceSpan s, Variable variable )
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
