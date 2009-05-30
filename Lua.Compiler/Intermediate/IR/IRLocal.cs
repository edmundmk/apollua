﻿// IRLocal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR
{


sealed class IRLocal
	:	Local
{

	public bool IsUpVal		{ get; private set; }


	public IRLocal( string name )
		:	base( name )
	{
		IsUpVal = false;
	}


	public void MarkUpVal()
	{
		IsUpVal = true;
	}

}


}

