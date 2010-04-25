// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Text;
using System.Collections.Generic;
using Lua.Bytecode;
using Lua.Interop;
using Lua.Library;
using Lua.Runtime;
using Lua.Utility;


namespace Lua
{


/*	A LuaThread holds all the information needed to implement Lua's stack-based modification
	of functions, error handling, and coroutines.  This means the stack of current function
	calls, the VM parameter and local stack (including upvals), and data that allows resumption
	of coroutines after suspension.
*/


public enum LuaThreadStatus
{
	Running,
	Suspended,
	Normal,
	Dead
}


public sealed class LuaThread
	:	LuaValue
{
	public static int VarResult = -1;



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

	public LuaThreadStatus Status
	{
		get { return GetStatus(); }
	}
	
	internal LuaValue[]				Stack;
	internal int					Top;
	internal List< Frame >			UnwoundFrames;
	internal List< LuaFunction >	StackLevels;
	List< UpVal >					openUpVals;
	int								watermark;



	// Constructors

	public LuaThread()
		:	this( null )
	{
	}


	public LuaThread( LuaFunction function )
	{
		// Thread state.
		Stack			= new LuaValue[ 16 ];
		Top				= -1;
		UnwoundFrames	= new List< Frame >();
		StackLevels		= new List< LuaFunction >();
		openUpVals		= new List< UpVal >();
		watermark		= 0;
		
		// Environment.
		if ( function != null )
		{
			Environment	= CurrentThread.Environment;

			// Set up as suspended frame.
			Stack[ 0 ] = function;
			UnwoundFrames.Add( new Frame( 0, -1, 1, 0 ) );
			UnwoundFrames.Add( new Frame( 0, -1, 1, 0 ) );
		}
		else
		{
			Environment	= CreateDefaultEnvironment();
		}
	}



	// State.

	static LuaTable CreateDefaultEnvironment()
	{
		LuaTable environment		= basic.CreateTable();
		environment[ "coroutine" ]	= coroutine.CreateTable();
		environment[ "io" ]			= io.CreateTable();
		environment[ "math" ]		= math.CreateTable();
		environment[ "os" ]			= os.CreateTable();
		environment[ "string" ]		= @string.CreateTable();
		return environment;
	}


	LuaThreadStatus GetStatus()
	{
		if ( CurrentThread == this )
			return LuaThreadStatus.Running;
		else if ( UnwoundFrames.Count > 0 )
			return LuaThreadStatus.Suspended;
		else if ( watermark != 0 )
			return LuaThreadStatus.Normal;
		else
			return LuaThreadStatus.Dead;
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

		openUpVals.RemoveRange( removeIndex, openUpVals.Count - removeIndex );
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



	// Call interop.

	public int BeginCall( LuaValue function, int argumentCount )
	{
		int frameBase = watermark;
		StackWatermark( frameBase + 1 + argumentCount );
		Stack[ frameBase ] = function;
		return frameBase;
	}

	public void CallArgument( int frameBase, int argument, LuaValue value )
	{
		Stack[ frameBase + 1 + argument ] = value;
	}

	public void CallArgument< T >( int frameBase, int argument, T value )
	{
		CallArgument( frameBase, argument, InteropHelpers.Box( value ) );
	}

	public int Call( int frameBase, int resultCount )
	{
		try
		{
			Stack[ frameBase ].Call( this, frameBase, watermark - frameBase - 1, resultCount );
			if ( resultCount != VarResult )
				return resultCount;
			else
				return Top + 1 - frameBase;
		}
		catch ( Exception e )
		{
			throw new LuaError( UnwindStackTrace(), e );
		}
	}

	public LuaValue CallResult( int frameBase, int result )
	{
		return Stack[ frameBase + result ];
	}

	public T CallResult< T >( int frameBase, int result )
	{
		return InteropHelpers.Unbox< T >( CallResult( frameBase, result ) );
	}

	public void EndCall( int frameBase )
	{
		StackWatermark( frameBase );
	}




	// Resume interop.

	public void BeginResume( int argumentCount )
	{
		Frame yield = UnwoundFrames[ 0 ];
		if ( yield.ResultCount != -1 )
		{
			StackWatermark( yield.FrameBase + 1 + yield.ResultCount );
		}
		else
		{
			Top = yield.FrameBase + 1 + argumentCount - 1;
			StackWatermark( Top + 1 );
		}
	}

	public void ResumeArgument( int argument, LuaValue value )
	{
		Frame yield = UnwoundFrames[ 0 ];
		if ( argument < yield.ResultCount || yield.ResultCount == -1 )
		{
			Stack[ yield.FrameBase + 1 + argument ] = value;
		}
	}

	public void ResumeArgument< T >( int argument, T value )
	{
		ResumeArgument( argument, InteropHelpers.Box( value ) );
	}

	public int Resume( int resultCount )
	{
		try
		{

			LuaThread coroutine = CurrentThread;
			CurrentThread = this;
			
			Frame resume = UnwoundFrames[ UnwoundFrames.Count - 1 ];
			if ( resume.InstructionPointer > 0 )
			{
				// Resume from a yield.
				Stack[ resume.FrameBase ].Resume( this );
			}
			else
			{
				// First time this function has been called.
				UnwoundFrames.Clear();
				int callArgumentCount = Top - resume.FrameBase;
				Top = -1;
				Stack[ resume.FrameBase ].Call( this, resume.FrameBase, callArgumentCount, -1 );
			}

			CurrentThread = coroutine;


			int resultBase;
			int actualResultCount;
			
			if ( UnwoundFrames.Count > 0 )
			{
				// Yielded.
				Frame yield = UnwoundFrames[ 0 ];
				resultBase = yield.FrameBase + 1;
				actualResultCount = Top - yield.FrameBase;
				Top = -1;
			}
			else
			{
				// Normal return.
				resultBase = resume.FrameBase;
				actualResultCount = Top + 1 - resume.FrameBase;
				Top = -1;
			}
						
			if ( resultCount != VarResult )
			{
				StackWatermark( resultBase + resultCount );
				if ( resultCount > actualResultCount )
				{
					Array.Clear( Stack, resultBase, resultCount - actualResultCount );
				}
				return resultCount;
			}
			else
			{
				StackWatermark( resultBase + actualResultCount );
				return actualResultCount;
			}

		}
		catch ( Exception e )
		{
			throw new LuaError( UnwindStackTrace(), e );
		}
	}

	public LuaValue ResumeResult( int result )
	{
		if ( UnwoundFrames.Count > 0 )
		{
			Frame yield = UnwoundFrames[ 0 ];
			return Stack[ yield.FrameBase + 1 + result ];
		}
		else
		{
			return Stack[ result ];
		}
	}

	public T ResumeResult< T >( int result )
	{
		return InteropHelpers.Unbox< T >( ResumeResult( result ) );
	}

	public void EndResume()
	{
		if ( UnwoundFrames.Count > 0 )
		{
			Frame yield = UnwoundFrames[ 0 ];
			StackWatermark( yield.FrameBase + 1 );
		}
		else
		{
			StackWatermark( 0 );
		}

	}




	// Stack unwinding due to error.

	internal string UnwindStackTrace()
	{
		StringBuilder s = new StringBuilder();

		foreach ( Frame frame in UnwoundFrames )
		{
			LuaValue function = Stack[ frame.FrameBase ];
			if ( function is LuaFunction )
			{
				LuaPrototype prototype = ( (LuaFunction)function ).Prototype;
				SourceSpan location = prototype.DebugInstructionSourceSpans[ frame.InstructionPointer - 1 ];
				s.AppendFormat( "   at <unknown> in {0}:line {1}\n", location.Start.SourceName, location.Start.Line );
			}
			else
			{
				s.AppendFormat( "   Frame: {0} {1} {2} {3}\n", frame.FrameBase, frame.ResultCount, frame.FramePointer, frame.InstructionPointer );
			}

			StackWatermark( frame.FrameBase );
		}

		UnwoundFrames.Clear();

		return s.ToString();
	}


}


}

