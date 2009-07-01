// InteropHelpers.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public static class InteropHelpers
{
	// Constants.

	public static readonly Value[] EmptyValues = new Value[] {};



	// Casting.

	public static T Cast< T >( Value v )
	{
		return default( T );
	}

	public static T Cast< T >( Value[] values, int index )
	{
		return default( T );
	}

	public static Value CastResultS< T >( T value )
	{
		return null;
	}

	public static Value[] CastResultM< T >( T value )
	{
		return EmptyValues;
	}

	public static Value CastResultListS< T >( T[] values )
	{
		return null;
	}

	public static Value[] CastResultListM< T >( T[] values )
	{
		return EmptyValues;
	}



}



}



