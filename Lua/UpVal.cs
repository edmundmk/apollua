// UpVal.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


/*	UpVals must be accessed through an extra layer of indirection.
*/

[DebuggerDisplay( "{Value}" )]
public sealed class UpVal
{
	// Value.

	public Value Value { get; set; }


	// Constructors.

	public UpVal()
		:	this( null )
	{
	}

	public UpVal( Value value )
	{
		Value = value;
	}

}


}

