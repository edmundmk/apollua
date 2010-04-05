// InteropHelpers.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Runtime;


namespace Lua.Interop
{


public static class InteropHelpers
{

	// Casting.

	public static T Unbox< T >( LuaValue v )
	{
		if ( v == null )								{ return default( T ); }
		else if ( typeof( T ) == typeof( LuaValue ) )	{ return (T)(object)v; }
		else if ( typeof( T ) == typeof( bool ) )		{ return (T)(object)(bool)v; }
		else if ( typeof( T ) == typeof( sbyte ) )		{ return (T)(object)(sbyte)v; }
		else if ( typeof( T ) == typeof( byte ) )		{ return (T)(object)(byte)v; }
		else if ( typeof( T ) == typeof( short ) )		{ return (T)(object)(short)v; }
		else if ( typeof( T ) == typeof( ushort ) )		{ return (T)(object)(ushort)v; }
		else if ( typeof( T ) == typeof( int ) )		{ return (T)(object)(int)v; }
		else if ( typeof( T ) == typeof( long ) )		{ return (T)(object)(long)v; }
		else if ( typeof( T ) == typeof( ulong ) )		{ return (T)(object)(ulong)v; }
		else if ( typeof( T ) == typeof( float ) )		{ return (T)(object)(float)v; }
		else if ( typeof( T ) == typeof( double ) )		{ return (T)(object)(double)v; }
		else if ( typeof( T ) == typeof( decimal ) )	{ return (T)(object)(decimal)v; }
		else if ( typeof( T ) == typeof( string ) )		{ return (T)(object)(string)v; }

		throw new InvalidCastException();
	}


	public static LuaValue Box< T >( T v )
	{
		if ( v == null )								{ return null; }
		else if ( typeof( T ) == typeof( LuaValue ) )	{ return (LuaValue)(object)v; }
		else if ( typeof( T ) == typeof( bool ) )		{ return (LuaValue)(bool)(object)v; }
		else if ( typeof( T ) == typeof( sbyte ) )		{ return (LuaValue)(sbyte)(object)v; }
		else if ( typeof( T ) == typeof( byte ) )		{ return (LuaValue)(byte)(object)v; }
		else if ( typeof( T ) == typeof( short ) )		{ return (LuaValue)(short)(object)v; }
		else if ( typeof( T ) == typeof( ushort ) )		{ return (LuaValue)(ushort)(object)v; }
		else if ( typeof( T ) == typeof( int ) )		{ return (LuaValue)(int)(object)v; }
		else if ( typeof( T ) == typeof( uint ) )		{ return (LuaValue)(uint)(object)v; }
		else if ( typeof( T ) == typeof( long ) )		{ return (LuaValue)(long)(object)v; }
		else if ( typeof( T ) == typeof( ulong ) )		{ return (LuaValue)(ulong)(object)v; }
		else if ( typeof( T ) == typeof( float ) )		{ return (LuaValue)(float)(object)v; }
		else if ( typeof( T ) == typeof( double ) )		{ return (LuaValue)(double)(object)v; }
		else if ( typeof( T ) == typeof( decimal ) )	{ return (LuaValue)(decimal)(object)v; }
		else if ( typeof( T ) == typeof( string ) )		{ return (LuaValue)(string)(object)v; }

		throw new InvalidCastException();
	}


	public static MethodInfo BindDelegateSignature( MethodInfo signature, MethodInfo[] actionTable, MethodInfo[] funcTable )
	{
		// Get information about signature.
		ParameterInfo[] parameters = signature.GetParameters();
		bool isFunc = signature.ReturnType != typeof( void );

		// Build type arguments array.
		int typeArgumentsCount = parameters.Length + ( isFunc ? 1 : 0 );
		Type[] typeArguments = new Type[ typeArgumentsCount ];
		for ( int parameter = 0; parameter < parameters.Length; ++parameter )
		{
			typeArguments[ parameter ] = parameters[ parameter ].ParameterType;
		}
		
		// Make correctly typed MethodInfo to match the signature.
		if ( isFunc )
		{
			typeArguments[ parameters.Length ] = signature.ReturnType;
			return funcTable[ parameters.Length ].MakeGenericMethod( typeArguments );
		}
		else
		{
			if ( parameters.Length == 0 )
				return actionTable[ 0 ];
			else
				return actionTable[ parameters.Length ].MakeGenericMethod( typeArguments );
		}
	}

}



}



