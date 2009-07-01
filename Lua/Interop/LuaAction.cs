// LuaAction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public class LuaAction
	:	Function
{
	Action function;

	public LuaAction( Action function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function(); return null; }
	public override Value InvokeS( Value a1 )									{ function(); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function(); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function(); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function(); return null; }
	public override Value InvokeS( Value[] arguments )							{ function(); return null; }

	public override Value[] InvokeM()											{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function(); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function(); return InteropHelpers.EmptyValues; }

}


public class LuaAction< T >
	:	Function
{
	Action< T > function;

	public LuaAction( Action< T > function )
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
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T >( arguments, 0 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaAction< T1, T2 >
	:	Function
{
	Action< T1, T2 > function;

	public LuaAction( Action< T1, T2 > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaAction< T1, T2, T3 >
	:	Function
{
	Action< T1, T2, T3 > function;

	public LuaAction( Action< T1, T2, T3 > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaAction< T1, T2, T3, T4 >
	:	Function
{
	Action< T1, T2, T3, T4 > function;

	public LuaAction( Action< T1, T2, T3, T4 > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2 )							{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ); return null; }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ); return null; }
	public override Value InvokeS( Value[] arguments )							{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ); return null; }

	public override Value[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1 )									{ function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2 )						{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override Value[] InvokeM( Value[] arguments )						{ function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ); return InteropHelpers.EmptyValues; }

}


}



