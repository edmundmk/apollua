// BeginEndScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{



/*
	scope					// local variable declaration scope.
	{
	}
*/


sealed class BeginScope
	:	IRStatement
{

	public BeginScope( SourceLocation l )
		:	base( l )
	{
	}

}


sealed class EndScope
	:	IRStatement
{

	public EndScope( SourceLocation l )
		:	base( l )
	{
	}

}







}

