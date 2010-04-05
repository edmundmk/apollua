// LuaError.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Text;
using Lua.Runtime;


namespace Lua
{


public sealed class LuaError
	:	Exception
{

	string luaStackTrace;


	public LuaError( LuaThread thread, Exception innerException )
		:	base( innerException.Message )
	{
		luaStackTrace = UnwindStackTrace( thread );
	}


	public override string StackTrace
	{
		get { return luaStackTrace + base.StackTrace; }
	}


	string UnwindStackTrace( LuaThread thread )
	{
		StringBuilder s = new StringBuilder();

		foreach ( Frame frame in thread.UnwoundFrames )
		{
			s.AppendFormat( "   Frame: {0} {1} {2} {3}\n", frame.FrameBase, frame.ResultCount, frame.FramePointer, frame.InstructionPointer );
			thread.StackWatermark( frame.FrameBase );
		}

		thread.UnwoundFrames.Clear();

		return s.ToString();
	}


}


}


