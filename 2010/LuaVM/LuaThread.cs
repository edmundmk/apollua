// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Runtime;


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
/*	public static LuaTable TypeMetatable
	{
		get;
		set;
	}
	
	public LuaValue Environment
	{
		get;
		set;
	}


	// Thread state.

	public List< LuaFunction >	Frames				{ get; private set; }
	public List< LuaValue >		Values				{ get; private set; }
	public List< UpVal >		OpenUpVals			{ get; private set; }
	public int					Top					{ get; set; }

	public FrozenFrame			FrozenFrames		{ get; private set; }
	

	// Constructor.

	public LuaThread()
	{
		Frames			= new List< LuaFunction >();
		Values			= new List< LuaValue >();
		OpenUpVals		= new List< UpVal >();
		Top				= -1;
		FrozenFrames	= null;
	}



	// Current thread.

	[ThreadStatic] static LuaThread currentThread;

	public void MakeCurrent()
	{
		currentThread = this;
	}

	public static LuaThread GetCurrent()
	{
		if ( currentThread == null )
		{
			currentThread = new LuaThread();
		}
		return currentThread;
	}




	// LuaValue

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "thread";
	}


	
	// Methods.

	public int BeginInterop( LuaFunction function, int argumentCount )
	{
		int frameBase = Values.Count;
		StackWatermark( frameBase, frameBase + 1 + argumentCount );
		Values[ frameBase ] = function;
		return frameBase;
	}

	public void InteropArgument( int frameBase, int argument, LuaValue value )
	{
		Values[ frameBase + 1 + argument ] = value;
	}

	public LuaValue InteropResult( int frameBase, int result )
	{
		return Values[ frameBase + result ];
	}

	public void EndInterop( int frameBase )
	{
		StackWatermark( frameBase, frameBase );
	}


	public void BeginFrame( LuaFunction function )
	{
		Frames.Add( function );
	}


	public void EndFrame( LuaFunction function )
	{
		Frames.RemoveAt( Frames.Count - 1 );
	}


	public void StackWatermark( int valueTop, int frameTop )
	{
		// Make sure we have enough stack space for the function.
		frameTop = Math.Max( valueTop, frameTop );
		
		while ( frameTop > Values.Count )
		{
			Values.Add( null );
		}

		if ( frameTop < Values.Count )
		{
			Values.RemoveRange( frameTop, Values.Count - frameTop );
		}


		// Clear all values that have been retired.
		for ( int index = valueTop; index < Values.Count; ++index )
		{
			Values[ index ] = null;
		}
	}


	public UpVal MakeUpVal( int index )
	{
		UpVal upval;

		// Find existing UpVal.
		int upvalIndex = 0;
		while ( upvalIndex < OpenUpVals.Count )
		{
			upval = OpenUpVals[ upvalIndex ];

			if ( upval.Index == index )
			{
				return upval;
			}

			if ( upval.Index > index )
			{
				break;
			}

			upvalIndex += 1;
		}

		// Create new one.
		upval = new UpVal( Values, index );
		OpenUpVals.Insert( upvalIndex, upval );
		return upval;
	}


	public void CloseUpVals( int index )
	{
		UpVal upval;

		// Keep upvals below index.
		int upvalIndex = 0;
		while ( upvalIndex < OpenUpVals.Count )
		{
			upval = OpenUpVals[ upvalIndex ];
			
			if ( upval.Index >= index )
			{
				break;
			}

			upvalIndex += 1;
		}

		// Close all upvals after this.
		int removeIndex = upvalIndex;
		while ( upvalIndex < OpenUpVals.Count )
		{
			upval = OpenUpVals[ upvalIndex ];
			upval.Close();
			upvalIndex += 1;
		}

		OpenUpVals.RemoveRange( upvalIndex, OpenUpVals.Count - upvalIndex );
	}
*/
}


}

