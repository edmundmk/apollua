// VMRuntime.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.VM
{


public static class VMRuntime
{
	// Global state.
	
	[ThreadStatic] static VirtualMachine virtualMachine;

	public static VirtualMachine VirtualMachine
	{
		get
		{
			if ( virtualMachine == null )
			{
				virtualMachine = new VirtualMachine();
			}
			return virtualMachine;
		}
	}

}



}

