// string.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2010 Edmund Kapusniak


using System;
using System.Text;
using Lua.Interop;


namespace Lua.Library
{


public static partial class @string
{

	public static LuaTable CreateTable()
	{
		LuaTable @string = new LuaTable();
		@string[ "format" ] = new LuaInteropDelegate( format );
		return @string;
	}


	public static void format( LuaInterop lua )
	{
		string format = lua.Argument< string >( 0 );

		StringBuilder s = new StringBuilder();
		s.Append( format );

		lua.Return( s.ToString() );
	}


}



}

