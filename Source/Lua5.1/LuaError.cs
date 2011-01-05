// LuaError.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public sealed class LuaError
	:	Exception
{

	string luaStackTrace;


	internal LuaError( string luaStackTrace, Exception innerException )
		:	base( innerException.Message )
	{
		this.luaStackTrace = luaStackTrace;
	}


	public override string StackTrace
	{
		get { return luaStackTrace + base.StackTrace; }
	}

}


}


