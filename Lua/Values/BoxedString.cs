// BoxedString.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua.Values
{


[DebuggerDisplay( "{Value}" )]
public sealed class BoxedString
	:	LuaValue
{

	public static LuaTable TypeMetatable
	{
		get;
		set;
	}
	
	public string Value
	{
		get;
		private set;
	}

	public BoxedString( string value )
	{
		Value = value;
	}


	// Object

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


	// LuaValue.

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "string";
	}

	public override bool IsPrimitiveValue()
	{
		return true;
	}


	// Binary operators.

	public override LuaValue Concatenate( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedDouble)o ).Value ) );
		}
		if ( o.GetType() == typeof( string ) )
		{
			return new BoxedString( String.Concat( Value, ( (BoxedString)o ).Value ) );
		}
		return base.Concatenate( o );
	}

	
	// Unary operators.

	public override LuaValue Length()
	{
		return new BoxedInteger( Value.Length );
	}
	

	// Comparison operators.

	public override bool EqualsValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedString ) )
		{
			return Value == ( (BoxedString)o ).Value;
		}
		return base.EqualsValue( o );
	}
	
	public override bool LessThanValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedString ) )
		{
			return String.Compare( Value, ( (BoxedString)o ).Value ) < 0;
		}
		return base.LessThanValue( o );
	}
	
	public override bool LessThanOrEqualsValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedString ) )
		{
			return String.Compare( Value, ( (BoxedString)o ).Value ) <= 0;
		}
		return base.LessThanOrEqualsValue( o );
	}




}


}

