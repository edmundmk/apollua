// BoxedInteger.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua
{


[DebuggerDisplay( "{Value}" )]
public sealed class BoxedInteger
	:	Value
{
	// Value.

	public int Value { get; private set; }

	public BoxedInteger( int value )
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
			return Value.Equals( ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return ( (double)Value ).Equals( ( (BoxedNumber)o ).Value );
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

	public override bool TryToInteger( out int value )
	{
		value = Value;
		return true;
	}
	
	public override bool TryToNumber( out Value value )
	{
		value = this;
		return true;
	}

	public override bool UsePrimitiveConcatenate()
	{
		return true;
	}
	


	// Binary arithmetic operators.

	public override Value Add( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( Value + ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( (double)Value + ( (BoxedNumber)o ).Value );
		}
		return base.Add( o );
	}

	public override Value Subtract( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( Value - ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( (double)Value - ( (BoxedNumber)o ).Value );
		}
		return base.Subtract( o );
	}

	public override Value Multiply( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( Value * ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( (double)Value * ( (BoxedNumber)o ).Value );
		}
		return base.Multiply( o );
	}

	public override Value Divide( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			int oValue = ( (BoxedInteger)o ).Value;
			if ( Value % oValue == 0 )
			{
				return new BoxedInteger( Value / oValue );
			}
			else
			{
				return new BoxedNumber( (double)Value / (double)oValue );
			}
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( (double)Value / ( (BoxedNumber)o ).Value );
		}
		return base.Divide( o );
	}

	public override Value IntegerDivide( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( Value / ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Math.Floor( (double)Value / ( (BoxedNumber)o ).Value ) );
		}
		return base.IntegerDivide( o );
	}

	public override Value Modulus( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( Value % ( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( (double)Value % ( (BoxedNumber)o ).Value );
		}
		return base.Modulus( o );
	}

	public override Value RaiseToPower( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedInteger( (int)Math.Pow( (double)Value, (double)( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return new BoxedNumber( Math.Pow( (double)Value, ( (BoxedNumber)o ).Value ) );
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
		return new BoxedInteger( -Value );
	}



	// Comparison operators.

	public override bool Equals( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value == ( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return (double)Value == ( (BoxedNumber)o ).Value;
		}
		return base.Equals( o );
	}

	public override bool LessThan( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value < ( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return (double)Value < ( (BoxedNumber)o ).Value;
		}
		return base.LessThan( o );
	}

	public override bool LessThanOrEqual( Value o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value <= ( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedNumber ) )
		{
			return (double)Value <= ( (BoxedNumber)o ).Value;
		}
		return base.LessThanOrEqual( o );
	}


}


}

