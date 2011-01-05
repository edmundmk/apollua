// LuaInterop.cs
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

	public LuaValue Argument( int argument )
	{
		if ( argument < argumentCount )
			return thread.Stack[ frameBase + 1 + argument ];
		else
			return null;
	}

	public T Argument< T >( int argument )
	{
		return InteropHelpers.Unbox< T >( Argument( argument ) );
	}


	public void Return()
	{
		BeginReturn( 0 );
		EndReturn();
	}
	
	public void Return( LuaValue result )
	{
		BeginReturn( 1 );
		ReturnResult( 0, result );
		EndReturn();
	}

	public void Return< T >( T result )
	{
		Return( InteropHelpers.Box( result ) );
	}
	
	public void BeginReturn( int returnResultCount )
	{
		if ( resultCount == -1 )
		{
			thread.Top = frameBase + returnResultCount - 1;
			thread.StackWatermark( thread.Top + 1 );
		}
	}

	public void ReturnResult( int result, LuaValue value )
	{
		if ( resultCount == -1 || result < resultCount )
		{
			thread.Stack[ frameBase + result ] = value;
		}
	}

	public void ReturnResult< T >( int result, T value )
	{
		ReturnResult( result, InteropHelpers.Box( value ) );
	}

	public void EndReturn()
	{
	}


}


}