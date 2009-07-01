// BoxedString.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


[DebuggerDisplay( "{Value}" )]
public sealed class BoxedString
	:	Value
{
	// Value.

	public string Value { get; private set; }

	public BoxedString( string value )
	{
		Value = value;
	}



	public override bool Equals( object o )
	{
		if ( o == null )
		{
			return false;
		}
		if ( o.GetType() == typeof( BoxedString ) )
		{
			return Value.Equals( ( (BoxedString)o ).Value );
		}
		return base.Equals( o );
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return Value.ToString();
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



	// Conversion.

	public override string LuaType
	{
		get { return "string"; }
	}

	public override bool UsePrimitiveConcatenate()
	{
		return true;
	}



	// Binary arithmetic operators.

	public override Value Concatenate( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedNumber)o ).Value ) );
		}
		if ( o.GetType() == typeof( string ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedString)o ).Value ) );
		}
		return base.Concatenate( o );
	}



	// Unary arithmetic operators.

	public override Value Length()
	{
		return new BoxedInteger( Value.Length );
	}



	// Comparison operators.

	public override bool Equals( Value o )
	{
		if ( o.GetType() == typeof( BoxedString ) )
		{
			return Value == ( (BoxedString)o ).Value;
		}
		return base.Equals( o );
	}
	
	public override bool LessThan( Value o )
	{
		if ( o.GetType() == typeof( string ) )
		{
			return String.Compare( Value, ( (BoxedString)o ).Value ) < 0;
		}
		return base.LessThan( o );
	}
	
	public override bool LessThanOrEqual( Value o )
	{
		if ( o.GetType() == typeof( string ) )
		{
			return String.Compare( Value, ( (BoxedString)o ).Value ) <= 0;
		}
		return base.LessThanOrEqual( o );
	}




}


}

