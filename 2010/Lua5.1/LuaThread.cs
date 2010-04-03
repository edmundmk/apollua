// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Runtime;
using Lua.Utility;


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
	internal static LuaThread CurrentThread
	{
		get { if ( currentThread == null ) currentThread = new LuaThread(); return currentThread; }
		set { currentThread = value; }
	}

	
	// Lua.

	protected internal override string LuaType
	{
		get { return "thread"; }
	}
	
	internal LuaTable Environment
	{
		get; set;
	}


	// Thread state.

	internal LuaValue[]		Stack;
	internal int			Top;
	internal List< Frame >	SuspendedFrames;
	List< UpVal >			openUpVals;
	int						watermark;
	
	public LuaThread()
	{
		Stack			= new LuaValue[ 64 ];
		Top				= -1;
		SuspendedFrames	= new List< Frame >();
		openUpVals		= new List< UpVal >();
		watermark		= 0;
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
		Array.Clear( Stack, stackIndex, watermark - stackIndex );
	}


	// Stack management.

	internal void StackWatermark( int newWatermark )
	{
		if ( newWatermark < watermark )
		{
			// Clear the stack.
			Array.Clear( Stack, newWatermark, watermark - newWatermark );
		}
		else if ( watermark > Stack.Length )
		{
			// Grow the stack.
			Array.Resize( ref Stack, MathEx.NextPow2( newWatermark ) );
		}

		watermark = newWatermark;
	}	

}


}

