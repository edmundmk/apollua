// TemporaryExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR.Expression.Temporary
{



// Temporaries are assigned once and used once.

sealed class TemporaryExpression
	:	IRExpression
{

	public TemporaryExpression( SourceLocation l )
		:	base( l )
	{
	}

}





}

