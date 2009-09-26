// LuaThread.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


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

	public static LuaTable TypeMetatable
	{
		get;
		set;
	}
	
	public List< LuaFunction >		Frames					{ get; private set; }
	public List< LuaValue >			Values					{ get; private set; }
//	public List< UpVal >			OpenUpVals				{ get; private set; }
//	public SuspendedFrame			SuspendedFrameRoot		{ get; private set; }
	


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

	
}


}

