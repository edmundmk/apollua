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

/*
[DebuggerDisplay( "{Value}" )]
sealed class UpVal
{
	// Members.

	IList< LuaValue >	stack;
	int					index;
	LuaValue			value;


	// Value.

	public LuaValue Value
	{
		get { return stack != null ? stack[ index ] : value; }
		set { if ( stack != null ) stack[ index ] = value; else this.value = value; }
	}

	public int Index
	{
		get { return stack != null ? index : -1; }
	}


	// Lifetime.

	public UpVal( IList< LuaValue > stack, int index )
	{
		this.stack	= stack;
		this.index	= index;
		value		= null;
	}


	public void Close()
	{
		if ( stack == null )
			throw new InvalidOperationException();

		value	= stack[ index ];
		stack	= null;
		index	= -1;
	}

}
*/

}

