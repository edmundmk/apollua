// Number.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public sealed class Number
	:	Value
{
	// Value.

	public double Value { get; private set; }

	public Number( double value )
	{
		Value = value;
	}



	// Hashing.

	public override bool Equals( object o )
	{
		if ( o == null )
		{
			return false;
		}
		if ( o.GetType() == typeof( Integer ) )
		{
			return Value.Equals( (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return Value.Equals( ( (Number)o ).Value );
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



	// Conversion.

	public override bool TryToInteger( out int value )
	{
		if ( Value >= (double)int.MinValue && Value <= (double)int.MaxValue )
		{
			int integer = (int)Value;
			if ( (double)integer == Value )
			{
				value = integer;
				return true;
			}
		}
		return base.TryToInteger( out value );
	}
	
	public override bool TryToNumber( out double value )
	{
		value = Value;
		return true;
	}



	// Binary arithmetic operators.

	public override Value Add( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Value + (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Value + ( (Number)o ).Value );
		}
		return base.Add( o );
	}

	public override Value Subtract( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Value - (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Value - ( (Number)o ).Value );
		}
		return base.Subtract( o );
	}

	public override Value Multiply( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Value * (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Value * ( (Number)o ).Value );
		}
		return base.Multiply( o );
	}

	public override Value Divide( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Value / (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Value / ( (Number)o ).Value );
		}
		return base.Divide( o );
	}

	public override Value IntegerDivide( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Math.Floor( Value / (double)( (Integer)o ).Value ) );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Math.Floor( Value / ( (Number)o ).Value ) );
		}
		return base.IntegerDivide( o );
	}

	public override Value Modulus( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Value % (double)( (Integer)o ).Value );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Value % ( (Number)o ).Value );
		}
		return base.Modulus( o );
	}

	public override Value RaiseToPower( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return new Number( Math.Pow( Value, (double)( (Integer)o ).Value ) );
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return new Number( Math.Pow( Value, ( (Number)o ).Value ) );
		}
		return base.RaiseToPower( o );
	}

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

	public override Value UnaryMinus()
	{
		return new Number( -Value );
	}



	// Comparison operators.

	public override bool Equals( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return Value == (double)( (Integer)o ).Value;
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return Value == ( (Number)o ).Value;
		}
		return base.Equals( o );
	}

	public override bool LessThan( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return Value < (double)( (Integer)o ).Value;
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return Value < ( (Number)o ).Value;
		}
		return base.LessThan( o );
	}

	public override bool LessThanOrEqual( Value o )
	{
		if ( o.GetType() == typeof( Integer ) )
		{
			return Value <= (double)( (Integer)o ).Value;
		}
		if ( o.GetType() == typeof( Number ) )
		{
			return Value <= ( (Number)o ).Value;
		}
		return base.LessThanOrEqual( o );
	}

}


}

