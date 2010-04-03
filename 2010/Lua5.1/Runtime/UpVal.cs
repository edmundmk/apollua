// UpVal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Runtime
{


[DebuggerDisplay( "{Value}" )]
sealed class UpVal
{
	
	// Members.

	LuaThread	thread;
	int			stackIndex;
	LuaValue	value;

	public UpVal( LuaThread thread, int stackIndex )
	{
		this.thread		= thread;
		this.stackIndex	= stackIndex;
		this.value		= null;
	}


	// Value.

	public LuaValue Value
	{
		get { if ( thread != null ) return thread.Stack[ stackIndex ]; else return value; }
		set { if ( thread != null ) thread.Stack[ stackIndex ] = value; else this.value = value; }
	}

	public int StackIndex
	{
		get { return stackIndex; }
	}


	// Lifetime.

	public void Close()
	{
		if ( thread == null )
		{
			throw new InvalidOperationException();
		}

		value		= thread.Stack[ stackIndex ];
		thread		= null;
		stackIndex	= -1;
	}

}


}

