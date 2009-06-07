// UpVal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Lua
{


public class UpVal
{
	// Members.

	IList< Value >	stack;
	int				stackindex;
	Value			value;


	// Value.

	public Value Value
	{
		get { return stack != null ? stack[ stackindex ] : value; }
		set { if ( stack != null ) stack[ stackindex ] = value; else this.value = value; }
	}


	// Lifetime.

	public UpVal( IList< Value > stack, int stackindex )
	{
		this.stack		= stack;
		this.stackindex	= stackindex;
		value			= null;
	}


	public void Close()
	{
		Debug.Assert( stack != null );
		value			= stack[ stackindex ];
		stack			= null;
		stackindex		= -1;
	}

}


}

