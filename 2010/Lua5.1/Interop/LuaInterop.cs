﻿// LuaInterop.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public struct LuaInterop
{

	LuaThread	thread;
	int			frameBase;
	int			argumentCount;
	int			resultCount;


	internal LuaInterop( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		this.thread			= thread;
		this.frameBase		= frameBase;
		this.argumentCount	= argumentCount;
		this.resultCount	= resultCount;
	}


	public int ArgumentCount
	{
		get { return argumentCount; }
	}


	public string ArgumentType( int argument )
	{
		if ( argument < argumentCount )
			return thread.Stack[ frameBase + 1 + argument ].LuaType;
		else
			return "nil";
	}


	public T Argument< T >( int argument )
	{
		if ( argument < argumentCount )
			return InteropHelpers.Unbox< T >( thread.Stack[ frameBase + 1 + argument ] );
		else
			return default( T );
	}


	public void Return()
	{
		BeginReturn( 0 );
		EndReturn();
	}
	
	public void Return< T >( T result )
	{
		BeginReturn( 1 );
		ReturnResult( 0, result );
		EndReturn();
	}
	
	public void BeginReturn( int returnResultCount )
	{
		if ( resultCount == -1 )
		{
			thread.Top = frameBase + returnResultCount - 1;
			thread.StackWatermark( thread.Top + 1 );
		}
	}

	public void ReturnResult< T >( int result, T value )
	{
		if ( resultCount == -1 || result < resultCount )
		{
			thread.Stack[ frameBase + result ] = InteropHelpers.Box( value );
		}
	}

	public void EndReturn()
	{
	}


}


}