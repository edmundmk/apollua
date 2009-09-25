// BoxedDouble.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua.Values
{


[DebuggerDisplay( "{Value}" )]
public sealed class BoxedDouble
	:	LuaValue
{

	public static LuaTable TypeMetatable
	{
		get;
		set;
	}

	public double Value
	{
		get;
		private set;
	}

	public BoxedDouble( double value )
	{
		Value = value;
	}


	// Object.

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
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return Value.Equals( ( (BoxedDouble)o ).Value );
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


	// LuaValue

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "number";
	}

	public override bool IsPrimitiveValue()
	{
		return true;
	}

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

		value = 0;
		return false;
	}

	public override bool TryToDouble( out double value )
	{
		value = Value;
		return true;
	}

	public override bool TryToNumberValue( out LuaValue value )
	{
		value = this;
		return true;
	}


	// Binary operators

	public override LuaValue Add( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Value + (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Value + ( (BoxedDouble)o ).Value );
		}
		return base.Add( o );
	}

	public override LuaValue Subtract( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Value - (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Value - ( (BoxedDouble)o ).Value );
		}
		return base.Subtract( o );
	}

	public override LuaValue Multiply( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Value * (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Value * ( (BoxedDouble)o ).Value );
		}
		return base.Multiply( o );
	}

	public override LuaValue Divide( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Value / (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Value / ( (BoxedDouble)o ).Value );
		}
		return base.Divide( o );
	}

	public override LuaValue IntegerDivide( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Math.Floor( Value / (double)( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Math.Floor( Value / ( (BoxedDouble)o ).Value ) );
		}
		return base.IntegerDivide( o );
	}

	public override LuaValue Modulus( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Value % (double)( (BoxedInteger)o ).Value );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Value % ( (BoxedDouble)o ).Value );
		}
		return base.Modulus( o );
	}

	public override LuaValue RaiseToPower( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedDouble( Math.Pow( Value, (double)( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedDouble( Math.Pow( Value, ( (BoxedDouble)o ).Value ) );
		}
		return base.RaiseToPower( o );
	}

	public override LuaValue Concatenate( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedInteger)o ).Value ) );
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedDouble)o ).Value ) );
		}
		if ( o.GetType() == typeof( string ) )
		{
			return new BoxedString( System.String.Concat( Value, ( (BoxedString)o ).Value ) );
		}
		return base.Concatenate( o );
	}

	
	// Unary operators.

	public override LuaValue UnaryMinus()
	{
		return new BoxedDouble( -Value );
	}

	
	// Comparison operators.

	public override bool EqualsValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value == (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return Value == ( (BoxedDouble)o ).Value;
		}
		return base.EqualsValue( o );
	}

	public override bool LessThanValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value < (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return Value < ( (BoxedDouble)o ).Value;
		}
		return base.LessThanValue( o );
	}

	public override bool LessThanOrEqualsValue( LuaValue o )
	{
		if ( o.GetType() == typeof( BoxedInteger ) )
		{
			return Value <= (double)( (BoxedInteger)o ).Value;
		}
		if ( o.GetType() == typeof( BoxedDouble ) )
		{
			return Value <= ( (BoxedDouble)o ).Value;
		}
		return base.LessThanOrEqualsValue( o );
	}

}


}

