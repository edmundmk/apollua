// BoxedNumber.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


[DebuggerDisplay( "{Value}" )]
public sealed class BoxedNumber
	:	Value
{
	// Value.

	public double Value { get; private set; }

	public BoxedNumber( double value )
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
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value.Equals( (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return Value.Equals( ( (BoxedNumber)o ).Value );
		}
		return base.Equals( o );
	}

	public override int GetHashCode()
	{
		int integer;
		if ( TryToInteger( out integer ) )
		{
			return integer.GetHashCode();
		}
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
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Value + (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Value + ( (BoxedNumber)o ).Value );
		}
		return base.Add( o );
	}

	public override Value Subtract( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Value - (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Value - ( (BoxedNumber)o ).Value );
		}
		return base.Subtract( o );
	}

	public override Value Multiply( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Value * (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Value * ( (BoxedNumber)o ).Value );
		}
		return base.Multiply( o );
	}

	public override Value Divide( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Value / (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Value / ( (BoxedNumber)o ).Value );
		}
		return base.Divide( o );
	}

	public override Value IntegerDivide( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Math.Floor( Value / (double)( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Math.Floor( Value / ( (BoxedNumber)o ).Value ) );
		}
		return base.IntegerDivide( o );
	}

	public override Value Modulus( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Value % (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Value % ( (BoxedNumber)o ).Value );
		}
		return base.Modulus( o );
	}

	public override Value RaiseToPower( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedNumber( Math.Pow( Value, (double)( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Math.Pow( Value, ( (BoxedNumber)o ).Value ) );
		}
		return base.RaiseToPower( o );
	}

	public override Value Concatenate( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedNumber)o ).Value ) );
		}
		if ( o.GetType() == typeof( string ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedString)o ).Value ) );
		}
		return base.Concatenate( o );
	}




	// Unary arithmetic operators.

	public override Value UnaryMinus()
	{
		return new BoxedNumber( -Value );
	}



	// Comparison operators.

	public override bool Equals( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value == (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return Value == ( (BoxedNumber)o ).Value;
		}
		return base.Equals( o );
	}

	public override bool LessThan( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value < (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return Value < ( (BoxedNumber)o ).Value;
		}
		return base.LessThan( o );
	}

	public override bool LessThanOrEqual( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value <= (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return Value <= ( (BoxedNumber)o ).Value;
		}
		return base.LessThanOrEqual( o );
	}

}


}

