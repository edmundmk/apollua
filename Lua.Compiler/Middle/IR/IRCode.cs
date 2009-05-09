// IRCode.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


sealed class IRCode
	:	Code
{

	public IList< IRStatement >	Statements	{ get; private set; }


	public IRCode()
	{
		Statements = new List< IRStatement >();
	}


	public void Add( IRStatement statement )
	{
		Statements.Add( statement );
	}

	

}


}

