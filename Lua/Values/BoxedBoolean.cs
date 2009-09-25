// BoxedBoolean.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


[DebuggerDisplay( "{IsTrue()}" )]
public sealed class BoxedBoolean
	:	LuaValue
{
	
	public static LuaTable TypeMetatable
	{
		get;
		set;
	}

	[DebuggerHidden] public static BoxedBoolean True	{ get; private set; }
	[DebuggerHidden] public static BoxedBoolean False	{ get; private set; }

	static BoxedBoolean()
	{
		True	= new BoxedBoolean();
		False	= new BoxedBoolean();
	}

	private BoxedBoolean()
	{
	}


	// Object

	public override int GetHashCode()
	{
		if ( this == False )
		{
			return false.GetHashCode();
		}
		return true.GetHashCode();
	}

	public override string ToString()
	{
		if ( this == False )
		{
			return false.ToString();
		}
		return true.ToString();
	}


	// LuaValue

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "boolean";
	}

	public override bool IsTrue()
	{
		return this != False;
	}

}


}

