// basic.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Globalization;
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
		basic[ "tonumber" ]		= new LuaInteropDelegateFunc< LuaValue, LuaValue, LuaValue >( tonumber );
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
		LuaValue h = LuaValue.GetHandler( table, "__metatable" );
		if ( h != null )
		{
			throw new InvalidOperationException( "Metatable cannot be set because the existing metatable has a __metatable field." );
		}
		table.Metatable = metatable;
		return table;
	}


	public static LuaValue tonumber( LuaValue v, LuaValue parseBase )
	{
		// If it's already a number, return it.
		LuaValue result;
		if ( v.TryToNumberValue( out result ) )
			return result;

		// Parse it from a string.
		string s = (string)v;
		int b = parseBase != null ? (int)parseBase : 10;
		if ( b == 10 )
		{
			int iresult;
			if ( Int32.TryParse( s, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out iresult ) )
				return iresult;
			else
				return Double.Parse( s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat );
		}
		else if ( b == 16 )
		{
			return Int32.Parse( s, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat );
		}
		else
		{
			// Parse using base.
			s = s.Trim();

			int iresult = 0;
			for ( int i = 0; i < s.Length; ++i )
			{
				char c = s[ i ];
				int digit = b;
				if ( c >= '0' && c <= '9' )
					digit = c - '0';
				else if ( c >= 'a' && c <= 'z' )
					digit = c - 'a' + 10;
				else if ( c >= 'A' && c <= 'Z' )
					digit = c - 'A' + 10;
				
				if ( digit >= b )
					throw new FormatException( "Unable to parse number from string." );

				iresult = iresult * b + digit;
			}

			return iresult;
		}
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
