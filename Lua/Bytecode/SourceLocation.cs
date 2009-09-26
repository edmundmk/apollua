// DebugSourceLocation.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.VM
{


public struct DebugSourceLocation
{
	public string	SourceName		{ get; private set; }
	public int		Line			{ get; private set; }
	public int		Column			{ get; private set; }


	public DebugSourceLocation( string sourceName, int line, int column )
		:	this()
	{
		SourceName	= sourceName;
		Line		= line;
		Column		= column;
	}

}
	

}

