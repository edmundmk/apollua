// FrozenFrame.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua.Runtime
{


/*	When the stack is unwound for coroutine suspension or for exception handling, function
	calls are represented by a stack frame, which is described by this structure.
*/

struct Frame
{
	
	public int FrameBase;
	public int ResultCount;
	public int FramePointer;
	public int InstructionPointer;


	public Frame( int frameBase, int resultCount, int fp, int ip )
	{
		FrameBase			= frameBase;
		ResultCount			= resultCount;
		FramePointer		= fp;
		InstructionPointer	= ip;
	}

}


}