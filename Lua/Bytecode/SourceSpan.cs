// DebugSourceSpan.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.VM
{


public struct DebugSourceSpan
{
	public DebugSourceLocation	Start		{ get; private set; }
	public DebugSourceLocation	End			{ get; private set; }


	public DebugSourceSpan( DebugSourceLocation start, DebugSourceLocation end )
		:	this()
	{
		Start	= start;
		End		= end;
	}

}
	

}

