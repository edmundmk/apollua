// Declare.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{

	

// declare <local>

sealed class Declare
	:	IRStatement
{

	public IRLocal					Local			{ get; private set; }


	public Declare( SourceLocation l, IRLocal local )
		:	base( l )
	{
		Local			= local;
	}


	public override string ToString()
	{
		return String.Format( "declare {0}", Local.Name );
	}

}

	

}

