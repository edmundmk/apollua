// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public sealed class LuaThread
	:	LuaValue
{

	public static LuaTable TypeMetatable
	{
		get;
		set;
	}
	

	// LuaValue

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "thread";
	}

	
}


}

