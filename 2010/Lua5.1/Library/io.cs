// io.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Interop;


namespace Lua.Library
{


public static partial class io
{

	public static LuaTable CreateTable()
	{
		LuaTable io = new LuaTable();
		io[ "write" ] = new LuaInteropDelegate( write );
		return io;
	}
	
	
	static readonly file stdin	= new file( Console.In );
	static readonly file stdout	= new file( Console.Out );
	static file defaultInput	= stdin;
	static file defaultOutput	= stdout;


	public static void write( LuaInterop lua )
	{
		defaultOutput.write( lua );
	}


}
	

}