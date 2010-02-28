// LuaDelegate.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public class LuaDelegateV
	:	LuaFunction
{
	Action function;

	public LuaDelegateV( Action function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()															{ function(); return null; }
	public override LuaValue InvokeS( LuaValue a1 )												{ function(); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )								{ function(); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )					{ function(); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function(); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )									{ function(); return null; }

	public override LuaValue[] InvokeM()														{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )											{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )								{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )					{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )									{ function(); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateV< T >
	:	LuaFunction
{
	Action< T > function;

	public LuaDelegateV( Action< T > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaDelegateV< T1, T2 >
	:	LuaFunction
{
	Action< T1, T2 > function;

	public LuaDelegateV( Action< T1, T2 > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaDelegateV< T1, T2, T3 >
	:	LuaFunction
{
	Action< T1, T2, T3 > function;

	public LuaDelegateV( Action< T1, T2, T3 > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateV< T1, T2, T3, T4 >
	:	LuaFunction
{
	Action< T1, T2, T3, T4 > function;

	public LuaDelegateV( Action< T1, T2, T3, T4 > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateVP< TParams >
	:	LuaFunction
{
	ActionP< TParams > function;

	public LuaDelegateVP( ActionP< TParams > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function(); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< TParams >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function(); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< TParams >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateVP< T, TParams >
	:	LuaFunction
{
	ActionP< T, TParams > function;

	public LuaDelegateVP( ActionP< T, TParams > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T >( a1 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T >( a1 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T >( arguments, 0 ), InteropHelpers.UnboxList< TParams >( arguments, 1 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaDelegateVP< T1, T2, TParams >
	:	LuaFunction
{
	ActionP< T1, T2, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, TParams > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ); return InteropHelpers.EmptyValues; }

}

	
public class LuaDelegateVP< T1, T2, T3, TParams >
	:	LuaFunction
{
	ActionP< T1, T2, T3, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, T3, TParams > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateVP< T1, T2, T3, T4, TParams >
	:	LuaFunction
{
	ActionP< T1, T2, T3, T4, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, T3, T4, TParams > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) ); return null; }

	public override LuaValue[] InvokeM()											{ function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) ); return InteropHelpers.EmptyValues; }

}


public class LuaDelegateS< TResult >
	:	LuaFunction
{
	Func< TResult > function;

	public LuaDelegateS( Func< TResult > function )
	{
		this.function = function;
	}


	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function() ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function() ) ); }

}

public class LuaDelegateS< T, TResult >
	:	LuaFunction
{
	Func< T, TResult > function;

	public LuaDelegateS( Func< T, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( default( T ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( InteropHelpers.Unbox< T >( a1 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( InteropHelpers.Unbox< T >( a1 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( InteropHelpers.Unbox< T >( a1 ) ) ) );  }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( InteropHelpers.Unbox< T >( a1 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS(( InteropHelpers.UnboxElement< T >( arguments, 0 ) ) ) ); }

}

	
public class LuaDelegateS< T1, T2, TResult >
	:	LuaFunction
{
	Func< T1, T2, TResult > function;

	public LuaDelegateS( Func< T1, T2, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( default( T1 ), default( T2 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ) ) ); }

}

	
public class LuaDelegateS< T1, T2, T3, TResult >
	:	LuaFunction
{
	Func< T1, T2, T3, TResult > function;

	public LuaDelegateS( Func< T1, T2, T3, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( default( T1 ), default( T2 ), default( T3 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ) ) ); }

}


public class LuaDelegateS< T1, T2, T3, T4, TResult >
	:	LuaFunction
{
	Func< T1, T2, T3, T4, TResult > function;

	public LuaDelegateS( Func< T1, T2, T3, T4, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ) ) ); }

}


public class LuaDelegateSP< TParams, TResult >
	:	LuaFunction
{
	FuncP< TParams, TResult > function;

	public LuaDelegateSP( FuncP< TParams, TResult > function )
	{
		this.function = function;
	}


	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function() ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< TParams >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< TParams >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ) ); }

}

public class LuaDelegateSP< T, TParams, TResult >
	:	LuaFunction
{
	FuncP< T, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T >( arguments, 0 ), InteropHelpers.UnboxList< TParams >( arguments, 1 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( function( default( T ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) );  }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( function( InteropHelpers.UnboxElement< T >( arguments, 0 ), InteropHelpers.UnboxList< TParams >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateSP< T1, T2, TParams, TResult >
	:	LuaFunction
{
	FuncP< T1, T2, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ) ); }

}

	
public class LuaDelegateSP< T1, T2, T3, TParams, TResult >
	:	LuaFunction
{
	FuncP< T1, T2, T3, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, T3, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ) ); }

}


public class LuaDelegateSP< T1, T2, T3, T4, TParams, TResult >
	:	LuaFunction
{
	FuncP< T1, T2, T3, T4, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, T3, T4, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) ) ); }

}


public class LuaDelegateM< TResult >
	:	LuaFunction
{
	FuncM< TResult > function;

	public LuaDelegateM( FuncM< TResult > function )
	{
		this.function = function;
	}


	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function() ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function() ); }

}

public class LuaDelegateM< T, TResult >
	:	LuaFunction
{
	FuncM< T, TResult > function;

	public LuaDelegateM( FuncM< T, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ) ) );  }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T >( arguments, 0 ) ) ); }

}

	
public class LuaDelegateM< T1, T2, TResult >
	:	LuaFunction
{
	FuncM< T1, T2, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateM< T1, T2, T3, TResult >
	:	LuaFunction
{
	FuncM< T1, T2, T3, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, T3, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ) ) ); }

}


public class LuaDelegateM< T1, T2, T3, T4, TResult >
	:	LuaFunction
{
	FuncM< T1, T2, T3, T4, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, T3, T4, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ) ) ); }

}


public class LuaDelegateMP< TParams, TResult >
	:	LuaFunction
{
	FuncMP< TParams, TResult > function;

	public LuaDelegateMP( FuncMP< TParams, TResult > function )
	{
		this.function = function;
	}


	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function() ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< TParams >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function() ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< TParams >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< TParams >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxList< TParams >( arguments, 0 ) ) ); }

}

public class LuaDelegateMP< T, TParams, TResult >
	:	LuaFunction
{
	FuncMP< T, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T >( arguments, 0 ), InteropHelpers.UnboxList< TParams >( arguments, 1 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) );  }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T >( a1 ), InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T >( arguments, 0 ), InteropHelpers.UnboxList< TParams >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateMP< T1, T2, TParams, TResult >
	:	LuaFunction
{
	FuncMP< T1, T2, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) ) ); }

}

	
public class LuaDelegateMP< T1, T2, T3, TParams, TResult >
	:	LuaFunction
{
	FuncMP< T1, T2, T3, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, T3, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function ( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function ( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) ) ); }

}


public class LuaDelegateMP< T1, T2, T3, T4, TParams, TResult >
	:	LuaFunction
{
	FuncMP< T1, T2, T3, T4, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, T3, T4, TParams, TResult > function )
	{
		this.function = function;
	}

	public override LuaValue InvokeS()												{ return InteropHelpers.BoxListS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) ) ); }

	public override LuaValue[] InvokeM()											{ return InteropHelpers.BoxListM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), default( T4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( function( InteropHelpers.Unbox< T1 >( a1 ), InteropHelpers.Unbox< T2 >( a2 ), InteropHelpers.Unbox< T3 >( a3 ), InteropHelpers.Unbox< T4 >( a4 ) ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( function( InteropHelpers.UnboxElement< T1 >( arguments, 0 ), InteropHelpers.UnboxElement< T2 >( arguments, 1 ), InteropHelpers.UnboxElement< T3 >( arguments, 2 ), InteropHelpers.UnboxElement< T4 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) ) ); }

}



}

