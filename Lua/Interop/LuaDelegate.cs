// LuaDelegate.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public delegate void ActionP< TParams >( params TParams[] arguments );
public delegate void ActionP< T, TParams >( T a1, params TParams[] arguments );
public delegate void ActionP< T1, T2, TParams >( T1 a1, T2 a2, params TParams[] arguments );
public delegate void ActionP< T1, T2, T3, TParams >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate void ActionP< T1, T2, T3, T4, TParams >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );

public delegate TResult FuncP< TParams, TResult >( params TParams[] arguments );
public delegate TResult FuncP< T, TParams, TResult >( T a1, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, TParams, TResult >( T1 a1, T2 a2, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );

public delegate TResult[] FuncM< TResult >();
public delegate TResult[] FuncM< T, TResult >( T a1 );
public delegate TResult[] FuncM< T1, T2, TResult >( T1 a1, T2 a2 );
public delegate TResult[] FuncM< T1, T2, T3, TResult >( T1 a1, T2 a2, T3 a3 );
public delegate TResult[] FuncM< T1, T2, T3, T4, TResult >( T1 a1, T2 a2, T3 a3, T4 a4 );

public delegate TResult[] FuncMP< TParams, TResult >( params TParams[] arguments );
public delegate TResult[] FuncMP< T, TParams, TResult >( T a1, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, TParams, TResult >( T1 a1, T2 a2, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );



public class LuaDelegateV
	:	Function
{
	Action function;

	public LuaDelegateV( Action function )
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


public class LuaDelegateV< T >
	:	Function
{
	Action< T > function;

	public LuaDelegateV( Action< T > function )
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

	
public class LuaDelegateV< T1, T2 >
	:	Function
{
	Action< T1, T2 > function;

	public LuaDelegateV( Action< T1, T2 > function )
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

	
public class LuaDelegateV< T1, T2, T3 >
	:	Function
{
	Action< T1, T2, T3 > function;

	public LuaDelegateV( Action< T1, T2, T3 > function )
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


public class LuaDelegateV< T1, T2, T3, T4 >
	:	Function
{
	Action< T1, T2, T3, T4 > function;

	public LuaDelegateV( Action< T1, T2, T3, T4 > function )
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


public class LuaDelegateVP< TParams >
	:	Function
{
	ActionP< TParams > function;

	public LuaDelegateVP( ActionP< TParams > function )
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


public class LuaDelegateVP< T, TParams >
	:	Function
{
	ActionP< T, TParams > function;

	public LuaDelegateVP( ActionP< T, TParams > function )
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

	
public class LuaDelegateVP< T1, T2, TParams >
	:	Function
{
	ActionP< T1, T2, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, TParams > function )
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

	
public class LuaDelegateVP< T1, T2, T3, TParams >
	:	Function
{
	ActionP< T1, T2, T3, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, T3, TParams > function )
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


public class LuaDelegateVP< T1, T2, T3, T4, TParams >
	:	Function
{
	ActionP< T1, T2, T3, T4, TParams > function;

	public LuaDelegateVP( ActionP< T1, T2, T3, T4, TParams > function )
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


public class LuaDelegateS< TResult >
	:	Function
{
	Func< TResult > function;

	public LuaDelegateS( Func< TResult > function )
	{
		this.function = function;
	}


	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function() ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function() ) ); }

}

public class LuaDelegateS< T, TResult >
	:	Function
{
	Func< T, TResult > function;

	public LuaDelegateS( Func< T, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( arguments, 0 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( default( T ) ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( InteropHelpers.Cast< T >( a1 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( InteropHelpers.Cast< T >( a1 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( InteropHelpers.Cast< T >( a1 ) ) ) );  }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( InteropHelpers.Cast< T >( a1 ) ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS(( InteropHelpers.Cast< T >( arguments, 0 ) ) ) ); }

}

	
public class LuaDelegateS< T1, T2, TResult >
	:	Function
{
	Func< T1, T2, TResult > function;

	public LuaDelegateS( Func< T1, T2, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( default( T1 ), default( T2 ) ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ) ) ); }

}

	
public class LuaDelegateS< T1, T2, T3, TResult >
	:	Function
{
	Func< T1, T2, T3, TResult > function;

	public LuaDelegateS( Func< T1, T2, T3, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( default( T1 ), default( T2 ), default( T3 ) ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ) ) ); }

}


public class LuaDelegateS< T1, T2, T3, T4, TResult >
	:	Function
{
	Func< T1, T2, T3, T4, TResult > function;

	public LuaDelegateS( Func< T1, T2, T3, T4, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ) ) ); }

}


public class LuaDelegateSP< TParams, TResult >
	:	Function
{
	FuncP< TParams, TResult > function;

	public LuaDelegateSP( FuncP< TParams, TResult > function )
	{
		this.function = function;
	}


	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function() ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< TParams >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( function() ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< TParams >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ) ); }

}

public class LuaDelegateSP< T, TParams, TResult >
	:	Function
{
	FuncP< T, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T >( arguments, 0 ), InteropHelpers.CastParams< TParams >( arguments, 1 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( function( default( T ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) );  }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T >( arguments, 0 ), InteropHelpers.CastParams< TParams >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateSP< T1, T2, TParams, TResult >
	:	Function
{
	FuncP< T1, T2, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( function( default( T1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ) ); }

}

	
public class LuaDelegateSP< T1, T2, T3, TParams, TResult >
	:	Function
{
	FuncP< T1, T2, T3, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, T3, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ) ); }

}


public class LuaDelegateSP< T1, T2, T3, T4, TParams, TResult >
	:	Function
{
	FuncP< T1, T2, T3, T4, TParams, TResult > function;

	public LuaDelegateSP( FuncP< T1, T2, T3, T4, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 4 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 4 ) ) ); }

}


public class LuaDelegateM< TResult >
	:	Function
{
	FuncM< TResult > function;

	public LuaDelegateM( FuncM< TResult > function )
	{
		this.function = function;
	}


	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function() ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function() ); }

}

public class LuaDelegateM< T, TResult >
	:	Function
{
	FuncM< T, TResult > function;

	public LuaDelegateM( FuncM< T, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( arguments, 0 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ) ) );  }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( arguments, 0 ) ) ); }

}

	
public class LuaDelegateM< T1, T2, TResult >
	:	Function
{
	FuncM< T1, T2, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateM< T1, T2, T3, TResult >
	:	Function
{
	FuncM< T1, T2, T3, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, T3, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ) ) ); }

}


public class LuaDelegateM< T1, T2, T3, T4, TResult >
	:	Function
{
	FuncM< T1, T2, T3, T4, TResult > function;

	public LuaDelegateM( FuncM< T1, T2, T3, T4, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ) ) ); }

}


public class LuaDelegateMP< TParams, TResult >
	:	Function
{
	FuncMP< TParams, TResult > function;

	public LuaDelegateMP( FuncMP< TParams, TResult > function )
	{
		this.function = function;
	}


	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function() ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< TParams >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function() ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< TParams >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< TParams >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.CastParams< TParams >( arguments, 0 ) ) ); }

}

public class LuaDelegateMP< T, TParams, TResult >
	:	Function
{
	FuncMP< T, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T >( arguments, 0 ), InteropHelpers.CastParams< TParams >( arguments, 1 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) );  }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( a1 ), InteropHelpers.Cast< TParams >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T >( arguments, 0 ), InteropHelpers.CastParams< TParams >( arguments, 1 ) ) ); }

}

	
public class LuaDelegateMP< T1, T2, TParams, TResult >
	:	Function
{
	FuncMP< T1, T2, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< TParams >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.CastParams< TParams >( arguments, 2 ) ) ); }

}

	
public class LuaDelegateMP< T1, T2, T3, TParams, TResult >
	:	Function
{
	FuncMP< T1, T2, T3, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, T3, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function ( default( T1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< TParams >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function ( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.CastParams< TParams >( arguments, 3 ) ) ); }

}


public class LuaDelegateMP< T1, T2, T3, T4, TParams, TResult >
	:	Function
{
	FuncMP< T1, T2, T3, T4, TParams, TResult > function;

	public LuaDelegateMP( FuncMP< T1, T2, T3, T4, TParams, TResult > function )
	{
		this.function = function;
	}

	public override Value InvokeS()												{ return InteropHelpers.CastResultListS( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1 )									{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2 )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value InvokeS( Value[] arguments )							{ return InteropHelpers.CastResultListS( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 4 ) ) ); }

	public override Value[] InvokeM()											{ return InteropHelpers.CastResultListM( function( default( T1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1 )									{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), default( T2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2 )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), default( T3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3 )				{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), default( T4 ) ) ); }
	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( a1 ), InteropHelpers.Cast< T2 >( a2 ), InteropHelpers.Cast< T3 >( a3 ), InteropHelpers.Cast< T4 >( a4 ) ) ); }
	public override Value[] InvokeM( Value[] arguments )						{ return InteropHelpers.CastResultListM( function( InteropHelpers.Cast< T1 >( arguments, 0 ), InteropHelpers.Cast< T2 >( arguments, 1 ), InteropHelpers.Cast< T3 >( arguments, 2 ), InteropHelpers.Cast< T4 >( arguments, 3 ), InteropHelpers.CastParams< TParams >( arguments, 4 ) ) ); }

}



}

