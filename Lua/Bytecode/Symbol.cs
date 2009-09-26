// Symbol.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Bytecode
{


public struct Symbol
{
	public string	Name				{ get; private set; }
	public int		StartInstruction	{ get; private set; }
	public int		EndInstruction		{ get; private set; }


	public Symbol( string name, int startInstruction, int endInstruction )
		:	this()
	{
		Name				= name;
		StartInstruction	= startInstruction;
		EndInstruction		= endInstruction;
	}
}


}

