// os.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2010 Edmund Kapusniak


using System;
using Lua.Interop;


namespace Lua.Library
{


public static partial class os
{

	public static LuaTable CreateTable()
	{
		LuaTable os = new LuaTable();
		os[ "getenv" ]	= new LuaInteropDelegateFunc< string, string >( getenv );
		return os;
	}



	public static string getenv( string varname )
	{
		return Environment.GetEnvironmentVariable( varname );
	}



}


}
