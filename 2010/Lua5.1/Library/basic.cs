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
		basic[ "getfenv" ]		= new LuaInteropDelegateFunc< LuaValue, LuaTable >( getfenv );
		basic[ "print" ]		= new LuaInteropDelegate( print );
		basic[ "setmetatable" ]	= new LuaInteropDelegateFunc< LuaTable, LuaTable, LuaTable >( setmetatable );
		basic[ "tostring" ]		= new LuaInteropDelegateFunc< LuaValue, string >( tostring );
		basic[ "type" ]			= new LuaInteropDelegateFunc< LuaValue, string >( type );
		return basic;
	}


	public static LuaTable getfenv( LuaValue f )
	{
		LuaFunction function = f as LuaFunction;
		if ( function != null )
		{
			return function.Environment;
		}
		int level = 1;
		if ( f != null )
		{
			level = (int)f;
		}
		if ( level > 0 )
		{
			return LuaThread.CurrentThread.StackLevels[ LuaThread.CurrentThread.StackLevels.Count - level ].Environment;
		}
		else
		{
			return LuaThread.CurrentThread.Environment;
		}
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


	public static LuaTable setmetatable( LuaTable table, LuaTable metatable )
	{
		table.Metatable = metatable;
		return table;
	}


	public static string tostring( LuaValue v )
	{
		LuaValue handler = LuaValue.GetHandler( v, "__tostring" );
		if ( handler != null )
		{
			v = handler.Call( v );
		}
		if ( v != null )
			return v.ToString();
		else
			return "nil";
	}


	public static string type( LuaValue v )
	{
		if ( v != null )
			return v.LuaType;
		else
			return "nil";
	}


}


}
