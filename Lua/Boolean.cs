// Boolean.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public sealed class Boolean
	:	Value
{

	// Singleton instances.

	public static Boolean True	{ get; private set; }
	public static Boolean False	{ get; private set; }

	static Boolean()
	{
		True	= new Boolean();
		False	= new Boolean();
	}

	private Boolean()
	{
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

