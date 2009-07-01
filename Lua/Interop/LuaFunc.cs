// LuaFunc.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public class LuaFunc< TResult >
	:	Function
{
	Func< TResult > function;

	public LuaFunc( Func< TResult > function )
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

public class LuaFunc< T, TResult >
	:	Function
{
	Func< T, TResult > function;

	public LuaFunc( Func< T, TResult > function )
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

	
public class LuaFunc< T1, T2, TResult >
	:	Function
{
	Func< T1, T2, TResult > function;

	public LuaFunc( Func< T1, T2, TResult > function )
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

	
public class LuaFunc< T1, T2, T3, TResult >
	:	Function
{
	Func< T1, T2, T3, TResult > function;

	public LuaFunc( Func< T1, T2, T3, TResult > function )
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


public class LuaFunc< T1, T2, T3, T4, TResult >
	:	Function
{
	Func< T1, T2, T3, T4, TResult > function;

	public LuaFunc( Func< T1, T2, T3, T4, TResult > function )
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



}



