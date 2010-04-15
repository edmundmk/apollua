// os.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2010 Edmund Kapusniak


using System;
using System.Diagnostics;
using Lua.Interop;


namespace Lua.Library
{


public static partial class os
{

	public static LuaTable CreateTable()
	{
		LuaTable os = new LuaTable();
		os[ "clock" ]	= new LuaInteropDelegateFunc< double >( clock );
		os[ "getenv" ]	= new LuaInteropDelegateFunc< string, string >( getenv );
		return os;
	}


	static Stopwatch stopwatch = Stopwatch.StartNew();
	

	public static double clock()
	{
		return stopwatch.Elapsed.TotalSeconds;
	}


	public static string getenv( string varname )
	{
		return Environment.GetEnvironmentVariable( varname );
	}



}


}
