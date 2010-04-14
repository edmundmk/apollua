// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Runtime;
using Lua.Utility;
using Lua.Library;


namespace Lua
{


/*	A LuaThread holds all the information needed to implement Lua's stack-based modification
	of functions, error handling, and coroutines.  This means the stack of current function
	calls, the VM parameter and local stack (including upvals), and data that allows resumption
	of coroutines after suspension.
*/


public sealed class LuaThread
	:	LuaValue
{

	// Current thread.

	[ThreadStatic] static LuaThread currentThread;
	public static LuaThread CurrentThread
	{
		get { if ( currentThread == null ) currentThread = new LuaThread(); return currentThread; }
		set { currentThread = value; }
	}

	
	// Lua.

	protected internal override string LuaType
	{
		get { return "thread"; }
	}
	
	public LuaTable Environment
	{
		get; set;
	}


	// Thread state.

	internal LuaValue[]				Stack;
	internal int					Top;
	internal List< Frame >			UnwoundFrames;
	internal List< LuaFunction >	StackLevels;
	List< UpVal >					openUpVals;
	int								watermark;
	
	public LuaThread()
	{
		// Thread state.
		Stack			= new LuaValue[ 64 ];
		Top				= -1;
		UnwoundFrames	= new List< Frame >();
		StackLevels		= new List< LuaFunction >();
		openUpVals		= new List< UpVal >();
		watermark		= 0;
		
		// Environment.
		Environment				= basic.CreateTable();
		Environment[ "io" ]		= io.CreateTable();
		Environment[ "math" ]	= math.CreateTable();
		Environment[ "os" ]		= os.CreateTable();
		Environment[ "string" ]	= @string.CreateTable();
	}
		


	// UpVals.

	internal UpVal MakeUpVal( int stackIndex )
	{
		UpVal upval;

		// Find existing UpVal.
		int upvalIndex = 0;
		while ( upvalIndex < openUpVals.Count )
		{
			upval = openUpVals[ upvalIndex ];

			if ( upval.StackIndex == stackIndex )
			{
				return upval;
			}

			if ( upval.StackIndex > stackIndex )
			{
				break;
			}

			upvalIndex += 1;
		}

		// Create new one.
		upval = new UpVal( this, stackIndex );
		openUpVals.Insert( upvalIndex, upval );
		return upval;
	}

	internal void CloseUpVals( int stackIndex )
	{
		UpVal upval;

		// Keep upvals below index.
		int upvalIndex = 0;
		while ( upvalIndex < openUpVals.Count )
		{
			upval = openUpVals[ upvalIndex ];
			
			if ( upval.StackIndex >= stackIndex )
			{
				break;
			}

			upvalIndex += 1;
		}

		// Close all upvals after this.
		int removeIndex = upvalIndex;
		while ( upvalIndex < openUpVals.Count )
		{
			upval = openUpVals[ upvalIndex ];
			upval.Close();
			upvalIndex += 1;
		}

		openUpVals.RemoveRange( upvalIndex, openUpVals.Count - upvalIndex );
	}


	// Stack management.

	internal void StackWatermark( int newWatermark )
	{
		if ( newWatermark < watermark )
		{
			// Clear the stack.
			Array.Clear( Stack, newWatermark, watermark - newWatermark );
		}
		else if ( newWatermark > Stack.Length )
		{
			// Grow the stack.
			Array.Resize( ref Stack, MathEx.NextPow2( newWatermark ) );
		}

		watermark = newWatermark;
	}	



	// Interop.

	internal int BeginCall( LuaValue function, int argumentCount )
	{
		int frameBase = watermark;
		StackWatermark( frameBase + 1 + argumentCount );
		Stack[ frameBase ] = function;
		return frameBase;
	}

	internal void CallArgument( int frameBase, int argument, LuaValue value )
	{
		Stack[ frameBase + 1 + argument ] = value;
	}

	internal void Call( int frameBase, int resultCount )
	{
		Stack[ frameBase ].Call( this, frameBase, watermark - frameBase - 1, resultCount );
	}

	internal LuaValue CallResult( int frameBase, int result )
	{
		return Stack[ frameBase + result ];
	}

	internal void EndCall( int frameBase )
	{
		StackWatermark( frameBase );
	}

}


}

