// LuaFuncResultsParams.cs
// 
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public delegate TResult[] FuncResultListParams< TParams, TResult >( params TParams[] arguments );
public delegate TResult[] FuncResultListParams< T, TParams, TResult >( T a1, params TParams[] arguments );
public delegate TResult[] FuncResultListParams< T1, T2, TParams, TResult >( T1 a1, T2 a2, params TParams[] arguments );
public delegate TResult[] FuncResultListParams< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate TResult[] FuncResultListParams< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );




public class LuaFuncResultListParams< TParams, TResult >
	:	Function
{
	FuncResultListParams< TParams, TResult > function;

	public LuaFuncResultListParams( FuncResultListParams< TParams, TResult > function )
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

public class LuaFuncResultListParams< T, TParams, TResult >
	:	Function
{
	FuncResultListParams< T, TParams, TResult > function;

	public LuaFuncResultListParams( FuncResultListParams< T, TParams, TResult > function )
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

	
public class LuaFuncResultListParams< T1, T2, TParams, TResult >
	:	Function
{
	FuncResultListParams< T1, T2, TParams, TResult > function;

	public LuaFuncResultListParams( FuncResultListParams< T1, T2, TParams, TResult > function )
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

	
public class LuaFuncResultListParams< T1, T2, T3, TParams, TResult >
	:	Function
{
	FuncResultListParams< T1, T2, T3, TParams, TResult > function;

	public LuaFuncResultListParams( FuncResultListParams< T1, T2, T3, TParams, TResult > function )
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


public class LuaFuncResultListParams< T1, T2, T3, T4, TParams, TResult >
	:	Function
{
	FuncResultListParams< T1, T2, T3, T4, TParams, TResult > function;

	public LuaFuncResultListParams( FuncResultListParams< T1, T2, T3, T4, TParams, TResult > function )
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



