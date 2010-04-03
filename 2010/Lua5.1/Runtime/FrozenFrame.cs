// FrozenFrame.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua.Runtime
{


/*	Coroutines are suspended by returning FrozenFrame objects from function calls,
	and resumed by invoking the function objects again with the frozen frames.
*/

sealed class FrozenFrame
{
/*	public FrozenFrame	NextFrame			{ get; private set; }
	public int			FrameBase			{ get; private set; }
	public int			ResultCount			{ get; private set; }
	public int			FramePointer		{ get; private set; }
	public int			InstructionPointer	{ get; private set; }


	public FrozenFrame( FrozenFrame next, int frameBase, int resultCount, int fp, int ip )
	{
		NextFrame			= next;
		FrameBase			= frameBase;
		ResultCount			= resultCount;
		FramePointer		= fp;
		InstructionPointer	= ip;
	}
*/
}


}