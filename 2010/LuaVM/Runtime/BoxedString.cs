// BoxedString.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua.Runtime
{


[DebuggerDisplay( "{value}" )]
internal sealed class BoxedString
	:	LuaValue
{

	string value;

	internal BoxedString( string v )
	{
		value = v;
	}



	// Object.

	public override bool Equals( object o )
	{
		if ( o is string )
		{
			return value == (string)o;
		}
		return base.Equals( o );
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}

	public override string ToString()
	{
		return value.ToString();
	}
	
	
	// Lua.

	protected internal override string LuaType
	{
		get { return "string"; }
	}
	
	protected internal override bool SupportsSimpleConcatenation()
	{
		return true;
	}
	
	protected internal override bool TryToString( out string v )
	{
		v = value;
		return true;
	}


	// Operations.

	protected internal override LuaValue Length()
	{
		return value.Length;
	}

	protected internal override bool RawEquals( LuaValue o )
	{
		string s;
		if ( o.TryToString( out s ) )
		{
			return String.Equals( value, s, StringComparison.Ordinal );
		}
		return base.RawEquals( o );
	}

	protected internal override bool LessThan( LuaValue o )
	{
		string s;
		if ( o.TryToString( out s ) )
		{
			return String.Compare( value, s, StringComparison.Ordinal ) < 0;
		}
		return base.LessThan( o );
	}

	protected internal override bool LessThanOrEquals( LuaValue o )
	{
		string s;
		if ( o.TryToString( out s ) )
		{
			return String.Compare( value, s, StringComparison.Ordinal ) <= 0;
		}
		return base.LessThan( o );
	}
		




}


}

