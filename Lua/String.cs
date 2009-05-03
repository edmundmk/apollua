// String.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public sealed class String
	:	Value
{
	// Value.

	public string Value { get; private set; }

	public String( string value )
	{
		Value = value;
	}



	public override bool Equals( object o )
	{
		if ( o == null )
		{
			return false;
		}
		if ( o.GetType() == typeof( String ) )
		{
			return Value.Equals( ( (String)o ).Value );
		}
		return base.Equals( o );
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
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



	// Binary arithmetic operators.

	public override Value Concatenate( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new String( System.String.Concat( Value, ( (Integer)o ).Value ) );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new String( System.String.Concat( Value, ( (Number)o ).Value ) );
		}
		if ( o.GetType() == typeof( string ) )
		{
			return new String( System.String.Concat( Value, ( (String)o ).Value ) );
		}
		return base.Concatenate( o );
	}



	// Unary arithmetic operators.

	public override Value Length()
	{
		return new Integer( Value.Length );
	}



	// Comparison operators.

	public override bool Equals( Value o )
	{
		if ( o.GetType() == typeof( String ) )
		{
			return Value == ( (String)o ).Value;
		}
		return base.Equals( o );
	}
	
	public override bool LessThan( Value o )
	{
		if ( o.GetType() == typeof( string ) )
		{
			return System.String.Compare( Value, ( (String)o ).Value ) < 0;
		}
		return base.LessThan( o );
	}
	
	public override bool LessThanOrEqual( Value o )
	{
		if ( o.GetType() == typeof( string ) )
		{
			return System.String.Compare( Value, ( (String)o ).Value ) <= 0;
		}
		return base.LessThanOrEqual( o );
	}




}


}

