// LiteralExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{


// <value>

sealed class LiteralExpression
	:	IRExpression
{

	public object		Value		{ get; private set; }


	public LiteralExpression( SourceLocation l, object value )
		:	base( l )
	{
		Value		= value;
	}

}



}

