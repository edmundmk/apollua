// LuaValue.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Runtime;


namespace Lua
{


/*	All values manipulated by the Lua runtime are instances of this class.  This
	allows simple compilation of Lua operations into virtual method calls on this
	type.  The compiled code does not switch on the type of values and does not
	need to implement the semantics of metatables.

	Even primitive values (numbers, booleans) are 'boxed' into Value instances.
	To manipulate these values as Object references would require boxing anyway,
	and this way has the advantages described above.

	Unlike LuaCLR, nil is represented directly as a null reference.  This is more
	obvious than having a special Value instance to represent nil.  Any operations
	on null values will cause the runtime to throw a NullReferenceException.  This
	is acceptable because none of Lua 5.1's primitive operations are valid on nil
	values and nil cannot have a custom metatable.  However, as a consequence,
	the expression ( nil <op> right ) always raises an error, rather than allowing
	a call to right's meta handler.
*/


public abstract class LuaValue
{

	// Conversions.

	public static implicit operator LuaValue ( bool b )								{ return b ? BoxedBoolean.True : BoxedBoolean.False; }
	public static implicit operator LuaValue ( sbyte i )							{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( byte i )								{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( short i )							{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( ushort i )							{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( int i )								{ return new BoxedInteger( i ); }
	public static explicit operator LuaValue ( uint i )								{ checked { return new BoxedInteger( (int)i ); } }
	public static explicit operator LuaValue ( long i )								{ checked { return new BoxedInteger( (int)i ); } }
	public static explicit operator LuaValue ( ulong i )							{ checked { return new BoxedInteger( (int)i ); } }
	public static implicit operator LuaValue ( float n )							{ return new BoxedDouble( n ); }
	public static implicit operator LuaValue ( double n )							{ return new BoxedDouble( n ); }
	public static explicit operator LuaValue ( decimal n )							{ checked { return new BoxedDouble( (double)n ); } }
	public static implicit operator LuaValue ( string s )							{ return new BoxedString( s ); }

	public static explicit operator bool ( LuaValue v )								{ return v != null && v.IsTrue(); }
	public static explicit operator sbyte ( LuaValue v )							{ int value; if ( CastToInteger( v, out value ) ) checked { return (sbyte)value; } else throw new InvalidCastException(); }
	public static explicit operator byte ( LuaValue v )								{ int value; if ( CastToInteger( v, out value ) ) checked { return (byte)value; } else throw new InvalidCastException(); }
	public static explicit operator short ( LuaValue v )							{ int value; if ( CastToInteger( v, out value ) ) checked { return (short)value; } else throw new InvalidCastException(); }
	public static explicit operator ushort ( LuaValue v )							{ int value; if ( CastToInteger( v, out value ) ) checked { return (ushort)value; } else throw new InvalidCastException(); }
	public static explicit operator int ( LuaValue v )								{ int value; if ( CastToInteger( v, out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator uint ( LuaValue v )								{ int value; if ( CastToInteger( v, out value ) ) checked { return (uint)value; } else throw new InvalidCastException(); }
	public static explicit operator long ( LuaValue v )								{ int value; if ( CastToInteger( v, out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator ulong ( LuaValue v )							{ int value; if ( CastToInteger( v, out value ) ) checked { return (ulong)value; } else throw new InvalidCastException(); }
	public static explicit operator float ( LuaValue v )							{ double value; if ( CastToDouble( v, out value ) ) checked { return (float)value; } else throw new InvalidCastException(); }
	public static explicit operator double ( LuaValue v )							{ double value; if ( CastToDouble( v, out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator decimal ( LuaValue v )							{ double value; if ( CastToDouble( v, out value ) ) checked { return (decimal)value; } else throw new InvalidCastException(); }
	public static explicit operator string ( LuaValue v )							{ string value; if ( CastToString( v, out value ) ) return value; else throw new InvalidCastException(); }

	public T MakeDelegate< T >()													{ return (T)(object)MakeDelegate( typeof( T ) ); }


	// Object.

	public override bool				Equals( object o )							{ return o is LuaValue && this.Equals( (LuaValue)o ); }
	public override int					GetHashCode()								{ return base.GetHashCode(); }
	public override string				ToString()									{ return base.ToString(); }
	
	
	// Lua.

	protected internal virtual string	LuaType										{ get { throw new NotImplementedException(); } }
	protected internal virtual bool		SupportsSimpleConcatenation()				{ return false; }
	protected internal virtual bool		TryToInteger( out int v )					{ v = 0; return false; }
	protected internal virtual bool		TryToDouble( out double v )					{ v = 0.0; return false; }
	protected internal virtual bool		TryToString( out string v )					{ v = String.Empty; return false; }
	protected internal virtual bool		TryToNumberValue( out LuaValue v )			{ v = null; return false; }


	// Operations.

	protected internal virtual LuaTable Metatable									{ get { return null; } set { throw new NotSupportedException(); } }
	protected internal virtual LuaValue Add( LuaValue o )							{ return MetaBinaryOp( this, o, "__add" ); }
	protected internal virtual LuaValue	Subtract( LuaValue o )						{ return MetaBinaryOp( this, o, "__sub" ); }
	protected internal virtual LuaValue	Multiply( LuaValue o )						{ return MetaBinaryOp( this, o, "__mul" ); }
	protected internal virtual LuaValue	Divide( LuaValue o )						{ return MetaBinaryOp( this, o, "__div" ); }
	protected internal virtual LuaValue	IntegerDivide( LuaValue o )					{ return MetaBinaryOp( this, o, "__idiv" ); }
	protected internal virtual LuaValue	Modulus( LuaValue o )						{ return MetaBinaryOp( this, o, "__mod" ); }
	protected internal virtual LuaValue	RaiseToPower( LuaValue o )					{ return MetaBinaryOp( this, o, "__pow" ); }
	protected internal virtual LuaValue	Concatenate( LuaValue o )					{ return MetaConcatenate( this, o ); }

	protected internal virtual LuaValue	UnaryMinus()								{ return MetaUnaryOp( this, "__unm" ); }
	protected internal virtual LuaValue	Length()									{ return MetaUnaryOp( this, "__len" ); }

	protected internal virtual bool		IsTrue()									{ return true; }
	protected internal virtual bool		RawEquals( LuaValue o )						{ return Object.ReferenceEquals( this, o ); }
	protected internal virtual bool		Equals( LuaValue o )						{ return MetaEquals( this, o ); }
	protected internal virtual bool		LessThan( LuaValue o )						{ return MetaLessThan( this, o ); }
	protected internal virtual bool		LessThanOrEquals( LuaValue o )				{ return MetaLessThanOrEquals( this, o ); }
		
	protected internal virtual LuaValue	Index( LuaValue k )							{ return MetaIndex( this, k ); }
	protected internal virtual void		NewIndex( LuaValue k, LuaValue v )			{ MetaNewIndex( this, k, v ); }


	// Function interface.

	protected internal virtual void		Call( LuaThread t, int f, int a, int r )	{ LuaValue h = GetHandler( this, "__call" ); if ( h != null ) h.Call( t, f, a, r ); else throw new NotSupportedException(); }
	protected internal virtual void		Resume( LuaThread t )						{ throw new NotSupportedException(); }
	protected internal virtual Delegate	MakeDelegate( Type delegateType )			{ LuaValue h = GetHandler( this, "__call" ); if ( h != null ) return h.MakeDelegate( delegateType ); else throw new NotSupportedException(); }
	
	protected internal virtual LuaValue	Call( LuaValue a )							{ throw new NotSupportedException(); }
	protected internal virtual LuaValue Call( LuaValue a, LuaValue b )				{ throw new NotSupportedException(); }
	protected internal virtual LuaValue Call( LuaValue a, LuaValue b, LuaValue c )	{ throw new NotSupportedException(); }


	// Casts.

	protected static bool CastToInteger( LuaValue v, out int value )
	{
		if ( v == null )
		{
			value = 0;
			return false;
		}

		if ( v.TryToInteger( out value ) )
		{
			return true;
		}
		
		double d;
		if ( v.TryToDouble( out d ) )
		{
			value = (int)d;
			return (double)value == d;
		}

		return false;
	}

	protected static bool CastToDouble( LuaValue v, out double value )
	{
		if ( v == null )
		{
			value = 0.0;
			return false;
		}

		return v.TryToDouble( out value );
	}

	protected static bool CastToString( LuaValue v, out string value )
	{
		if ( v == null )
		{
			value = String.Empty;
			return false;
		}

		return v.TryToString( out value );
	}


	// Meta handlers.
	
	protected static LuaValue GetHandler( LuaValue o, LuaValue handler )
	{
		return o != null && o.Metatable != null ? o.Metatable[ handler ] : null;
	}


	protected static LuaValue MetaBinaryOp( LuaValue left, LuaValue right, LuaValue handler )
	{
		LuaValue h = GetHandler( left, handler );
		if ( h != null )
		{
			return h.Call( left, right );
		}
		h = GetHandler( right, handler );
		if ( h != null )
		{
			return h.Call( left, right );
		}
		
		throw new NotSupportedException();
	}


	protected static LuaValue MetaConcatenate( LuaValue left, LuaValue right )
	{
		if ( left.SupportsSimpleConcatenation() && right.SupportsSimpleConcatenation() )
		{
			return new BoxedString( left.ToString() + right.ToString() );
		}
		else
		{
			return MetaBinaryOp( left, right, "__concat" );
		}
	}


	protected static LuaValue MetaUnaryOp( LuaValue operand, LuaValue handler )
	{
		LuaValue h = GetHandler( operand, handler );
		if ( h != null )
		{
			return h.Call( operand );
		}

		throw new NotSupportedException();
	}


	protected static LuaValue GetComparisonHandler( LuaValue left, LuaValue right, LuaValue handler )
	{
		if ( left != null && right != null && left.GetType() == right.GetType() )
		{
			LuaValue hLeft = GetHandler( left, handler );
			LuaValue hRight = GetHandler( right, handler );
			if ( hLeft == hRight )
			{
				return hLeft;
			}
		}
		return null;
	}


	protected static bool MetaEquals( LuaValue left, LuaValue right )
	{
		if ( left.LuaType != right.LuaType )
		{
			return false;
		}
		if ( left.Equals( right ) )
		{
			return true;
		}

		LuaValue h = GetComparisonHandler( left, right, "__eq" );
		if ( h != null )
		{
			LuaValue result = h.Call( left, right );
			return (bool)result;
		}

		return false;
	}


	protected static bool MetaLessThan( LuaValue left, LuaValue right )
	{
		LuaValue h = GetComparisonHandler( left, right, "__lt" );
		if ( h != null )
		{
			LuaValue result = h.Call( left, right );
			return (bool)result;
		}

		throw new NotSupportedException();
	}


	protected static bool MetaLessThanOrEquals( LuaValue left, LuaValue right )
	{
		LuaValue h = GetComparisonHandler( left, right, "__le" );
		if ( h != null )
		{
			LuaValue result = h.Call( left, right );
			return (bool)result;
		}

		h = GetComparisonHandler( left, right, "__lt" );
		if ( h != null )
		{
			LuaValue result = h.Call( right, left );
			return ! (bool)result;
		}

		throw new NotSupportedException();
	}


	protected static LuaValue MetaIndex( LuaValue table, LuaValue key )
	{
		LuaValue h = GetHandler( table, "__index" );
		if ( h != null )
		{	
			if ( h.LuaType == "function" )
			{
				return h.Call( table, key );
			}
			else
			{
				return h.Index( key );
			}
		}

		throw new NotSupportedException();
	}
	

	protected static void MetaNewIndex( LuaValue table, LuaValue key, LuaValue value )
	{
		LuaValue h = GetHandler( table, "__newindex" );
		if ( h != null )
		{
			if ( h.LuaType == "function" )
			{
				h.Call( table, key, value );
				return;
			}
			else
			{
				h.NewIndex( key, value );
				return;
			}
		}

		throw new NotSupportedException();
	}



}


}