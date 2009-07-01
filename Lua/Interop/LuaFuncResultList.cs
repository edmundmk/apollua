// LuaFuncResultList.cs
// 
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public delegate TResult[] FuncResults< TResult >();
public delegate TResult[] FuncResults< T, TResult >( T a1 );
public delegate TResult[] FuncResults< T1, T2, TResult >( T1 a1, T2 a2 );
public delegate TResult[] FuncResults< T1, T2, T3, TResult >( T1 a1, T2 a2, T3 a3 );
public delegate TResult[] FuncResults< T1, T2, T3, T4, TResult >( T1 a1, T2 a2, T3 a3, T4 a4 );


public class LuaFuncResultList< TResult >
	:	Function
{
	FuncResults< TResult > function;

	public LuaFuncResultList( FuncResults< TResult > function )
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

public class LuaFuncResultList< T, TResult >
	:	Function
{
	FuncResults< T, TResult > function;

	public LuaFuncResultList( FuncResults< T, TResult > function )
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

	
public class LuaFuncResultList< T1, T2, TResult >
	:	Function
{
	FuncResults< T1, T2, TResult > function;

	public LuaFuncResultList( FuncResults< T1, T2, TResult > function )
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

	
public class LuaFuncResultList< T1, T2, T3, TResult >
	:	Function
{
	FuncResults< T1, T2, T3, TResult > function;

	public LuaFuncResultList( FuncResults< T1, T2, T3, TResult > function )
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


public class LuaFuncResultList< T1, T2, T3, T4, TResult >
	:	Function
{
	FuncResults< T1, T2, T3, T4, TResult > function;

	public LuaFuncResultList( FuncResults< T1, T2, T3, T4, TResult > function )
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






}
