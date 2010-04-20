// coroutine.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Interop;
using Lua.Runtime;


namespace Lua.Library
{


public static partial class coroutine
{
	
	public static LuaTable CreateTable()
	{
		LuaTable coroutine = new LuaTable();
		coroutine[ "create" ]	= new LuaInteropDelegateFunc< LuaFunction, LuaThread >( create );
		coroutine[ "resume" ]	= new LuaInteropDelegate( resume );
		coroutine[ "running" ]	= new LuaInteropDelegateFunc< LuaThread >( running );
		coroutine[ "status" ]	= new LuaInteropDelegateFunc< LuaThread, string >( status );
		coroutine[ "wrap" ]		= new LuaInteropDelegateFunc< LuaFunction, LuaValue >( wrap );
		coroutine[ "yield" ]	= new Yield();
		return coroutine;
	}



	// Functions.

	public static LuaThread create( LuaFunction f )
	{
		return new LuaThread( f );
	}


	public static void resume( LuaInterop lua )
	{
		LuaThread co = lua.Argument< LuaThread >( 0 );

		co.BeginResume( lua.ArgumentCount - 1 );
		for ( int argument = 0; argument < lua.ArgumentCount - 1; ++argument )
		{
			co.ResumeArgument( argument, lua.Argument( argument ) );
		}

		int resultCount;
		try
		{
			resultCount = co.Resume( LuaThread.VarResult );
		}
		catch ( Exception e )
		{
			lua.BeginReturn( 2 );
			lua.ReturnResult( 0, false );
			lua.ReturnResult( 1, e.Message );
			lua.EndReturn();
			return;
		}

		lua.BeginReturn( resultCount + 1 );
		lua.ReturnResult( 0, true );
		for ( int result = 0; result < resultCount; ++result )
		{
			lua.ReturnResult( result + 1, co.ResumeResult( result ) );
		}
		co.EndResume();
		lua.EndReturn();
	}

	public static LuaThread running()
	{
		// Should actually return 'nil' if called from the main thread.
		return LuaThread.CurrentThread;
	}

	public static string status( LuaThread co )
	{
		switch ( co.Status )
		{
			case LuaThreadStatus.Running:	return "running";
			case LuaThreadStatus.Suspended:	return "suspended";
			case LuaThreadStatus.Normal:	return "normal";
			case LuaThreadStatus.Dead:		return "dead";
		}

		throw new ArgumentException();
	}

	public static LuaValue wrap( LuaFunction f )
	{
		return new WrappedCoroutine( new LuaThread( f ) );
	}

	


	// Special functions.


	class Yield
		:	LuaValue
	{

		protected internal override string LuaType
		{
			get { return "function"; }
		}

		internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
		{
			thread.Top = frameBase + argumentCount;
			thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, frameBase + 1, 0 ) );
			// Callers should also unwind until the whole stack is unwound.
		}
	
		internal override void Resume( LuaThread thread )
		{
			// We should be the last function in the stack (at index 0).
			Frame frame = thread.UnwoundFrames[ thread.UnwoundFrames.Count - 1 ];
			thread.UnwoundFrames.RemoveAt( thread.UnwoundFrames.Count - 1 );

			// The resume code will have altered our arguments to correspond to the
			// arguments of the resume call (always the correct number of results).

			// Find number of arguments/results.
			int resultCount;
			if ( frame.ResultCount != -1 )
			{
				resultCount = frame.ResultCount;
			}
			else
			{
				resultCount = thread.Top - frame.FrameBase;
				thread.Top = -1;
				thread.Top = frame.FrameBase + resultCount - 1;
			}

			// Copy down.
			for ( int result = 0; result < resultCount; ++result )
			{
				thread.Stack[ frame.FrameBase + result ] = thread.Stack[ frame.FrameBase + 1 + result ];
			}
		}
	
	}

	
	class WrappedCoroutine
		:	LuaValue
	{

		LuaThread coroutine;

		public WrappedCoroutine( LuaThread coroutine )
		{
			this.coroutine = coroutine;
		}
	
	
		protected internal override string LuaType
		{
			get { return "function"; }
		}

		internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
		{
			coroutine.BeginResume( argumentCount );

			// Copy arguments.
			for ( int argument = 0; argument < argumentCount; ++argument )
			{
				coroutine.ResumeArgument( argument, thread.Stack[ frameBase + 1 + argument ] );
			}

			// Resume.
			int actualResultCount = coroutine.Resume( resultCount );

			// Calculate number of results we want.
			int copyCount;
			if ( resultCount == -1 )
			{
				copyCount = actualResultCount;
				thread.Top = frameBase + actualResultCount - 1;
			}
			else
			{
				copyCount = Math.Min( resultCount, actualResultCount );
			}

			// Copy results.
			for ( int result = 0; result < copyCount; ++result )
			{
				thread.Stack[ frameBase + result ] = coroutine.ResumeResult( result );
			}
			for ( int result = copyCount; result < resultCount; ++result )
			{
				thread.Stack[ frameBase + result ] = null;
			}

			coroutine.EndResume();
		}
	
		internal override void Resume( LuaThread thread )
		{
			throw new NotSupportedException();
		}

	}

}


}



