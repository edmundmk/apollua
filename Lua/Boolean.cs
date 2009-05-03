// Boolean.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


[DebuggerDisplay( "{IsTrue()}" )]
public sealed class Boolean
	:	Value
{

	// Singleton instances.

	[DebuggerHidden] public static Boolean True		{ get; private set; }
	[DebuggerHidden] public static Boolean False	{ get; private set; }

	static Boolean()
	{
		True	= new Boolean();
		False	= new Boolean();
	}

	private Boolean()
	{
	}



	// Hashing.

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



	// Metatable.

	public static Table TypeMetatable
	{
		get;
		set;
	}
	
	public override	Table Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}



	// Comparison operators.

	public override bool IsTrue()
	{
		return this != False;
	}

}


}

