// BoxedBoolean.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua.Runtime
{


[DebuggerDisplay( "{IsTrue()}" )]
internal sealed class BoxedBoolean
	:	LuaValue
{

	internal static readonly BoxedBoolean True	= new BoxedBoolean();
	internal static readonly BoxedBoolean False	= new BoxedBoolean();


	private BoxedBoolean()	{ /* cannot create instances */ }


	// Object.

	public override bool Equals( object o )
	{
		if ( o is bool )
		{
			return (bool)o == (bool)this;
		}
		return base.Equals( o );
	}

	public override int GetHashCode()
	{
		return ( (bool)this ).GetHashCode();
	}
	
	public override string ToString()
	{
		return ( (bool)this ).ToString();
	}
	
	
	// Lua.

	protected internal override string LuaType
	{
		get { return "boolean"; }
	}
	
	protected internal override bool SupportsSimpleConcatenation()
	{
		return true;
	}
	

	// Operations.

	protected internal override bool IsTrue()
	{
		return this == True;
	}

	protected internal override bool RawEquals( LuaValue o )
	{
		return (bool)this == (bool)o;
	}
	

}


}

