// VMException.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.VM
{


public class VMException
	:	Exception
{
	string luaStackTrace;


	public VMException( Exception innerException, string luaStackTrace )
		:	base( innerException.Message, innerException )
	{
		this.luaStackTrace = luaStackTrace;
	}


	public override string StackTrace
	{
		get { return luaStackTrace + "\n" + base.StackTrace; }
	}

	public virtual string LuaStackTrace
	{
		get { return luaStackTrace; }
	}

	public override string ToString()
	{
		return Message + "\n" + InnerException.StackTrace + "\n" + StackTrace;
	}
}


}