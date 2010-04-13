// basic.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Interop;


namespace Lua.Library
{


public static partial class basic
{

	public static LuaTable CreateTable()
	{
		LuaTable basic = new LuaTable();
		basic[ "print" ]	= new LuaInteropDelegate( print );
		basic[ "tostring" ]	= new LuaInteropDelegateFunc< LuaValue, string >( tostring );
		return basic;
	}



	public static void print( LuaInterop lua )
	{
		for ( int argument = 0; argument < lua.ArgumentCount; ++argument )
		{
			if ( argument > 0 )
			{
				Console.Out.Write( "\t" );
			}
			Console.Out.Write( tostring( lua.Argument( argument ) ) );
		}
		Console.Out.Write( "\n" );
	}


	public static string tostring( LuaValue v )
	{
		LuaValue handler = LuaValue.GetHandler( v, "__tostring" );
		if ( handler != null )
		{
			v = handler.Call( v );
		}
		if ( v != null )
		{
			return v.ToString();
		}
		else
		{
			return "nil";
		}
	}


}


}
