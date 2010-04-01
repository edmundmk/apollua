// BoxedDouble.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;


namespace Lua.Runtime
{


[DebuggerDisplay( "{value}" )]
internal sealed class BoxedDouble
	:	LuaValue
{

	double value;

	internal BoxedDouble( double v )
	{
		value = v;
	}


	// Object.

	public override bool Equals( object o )
	{
		if ( o is double )
		{
			return value == (double)o;
		}
		if ( o is int )
		{
			return value == (double)(int)o;
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
		 get { return "number"; }
	}

	protected internal override bool SupportsSimpleConcatenation()
	{
		return true;
	}
	
	protected internal override bool TryToDouble( out double v )
	{
		v = value;
		return true;
	}
	
	protected internal override bool TryToNumberValue( out LuaValue v )
	{
		v = this;
		return true;
	}


	// Operations.

	protected internal override LuaValue Add( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value + v;
		}
		return base.Add( o );
	}

	protected internal override LuaValue Subtract( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value - v;
		}
		return base.Subtract( o );
	}

	protected internal override LuaValue Multiply( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value * v;
		}
		return base.Multiply( o );
	}

	protected internal override LuaValue Divide( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value / v;
		}
		return base.Divide( o );
	}

	protected internal override LuaValue IntegerDivide( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return Math.Floor( value / v );
		}
		return base.IntegerDivide( o );
	}

	protected internal override LuaValue Modulus( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value % v;
		}
		return base.Modulus( o );
	}

	protected internal override LuaValue RaiseToPower( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return Math.Pow( value, v );
		}
		return base.RaiseToPower( o );
	}

	protected internal override LuaValue UnaryMinus()
	{
		return -value;
	}


	protected internal override bool RawEquals( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value == v;
		}
		return base.RawEquals( o );
	}

	protected internal override bool LessThan( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value < v;
		}
		return base.LessThan( o );
	}

	protected internal override bool LessThanOrEquals( LuaValue o )
	{
		double v;
		if ( o.TryToDouble( out v ) )
		{
			return value <= v;
		}
		return base.LessThanOrEquals( o );
	}



}


}

