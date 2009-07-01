// LuaActionParams.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public delegate void ActionParams< TParams >( params TParams[] arguments );
public delegate void ActionParams< T, TParams >( T a1, params TParams[] arguments );
public delegate void ActionParams< T1, T2, TParams >( T1 a1, T2 a2, params TParams[] arguments );
public delegate void ActionParams< T1, T2, T3, TParams >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate void ActionParams< T1, T2, T3, T4, TParams >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );




public class LuaActionParams< TParams >
	:	Function
{
	ActionParams< TParams > function;

	public LuaActionParams( ActionParams< TParams > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function(); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< TParams >( a1 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ); return null; }

	public override Value[] InvokeM()											{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< TParams >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaActionParams< T, TParams >
	:	Function
{
	ActionParams< T, TParams > function;

	public LuaActionParams( ActionParams< T, TParams > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T >( a1 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T >( a1 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T >( a1 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T >( a1 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T >( arguments, 0 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T >( arguments, 0 ), InteropHelpers.CastParams< TParams >( arguments, 1 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaActionParams< T1, T2, TParams >
	:	Function
{
	ActionParams< T1, T2, TParams > function;

	public LuaActionParams( ActionParams< T1, T2, TParams > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaActionParams< T1, T2, T3, TParams >
	:	Function
{
	ActionParams< T1, T2, T3, TParams > function;

	public LuaActionParams( ActionParams< T1, T2, T3, TParams > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaActionParams< T1, T2, T3, T4, TParams >
	:	Function
{
	ActionParams< T1, T2, T3, T4, TParams > function;

	public LuaActionParams( ActionParams< T1, T2, T3, T4, TParams > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 4 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 5 ) ); return InteropHelpers.EmptyValues; }

}



}



