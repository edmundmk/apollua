// LuaValue.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Values;


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
	
	// Metatable.

	public virtual LuaTable Metatable
	{
		get { return null; }
		set { throw new InvalidOperationException(); }
	}


	// Conversions.

	public static implicit operator LuaValue ( bool b )			{ return b ? BoxedBoolean.True : BoxedBoolean.False; }
	public static implicit operator LuaValue ( sbyte i )		{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( byte i )			{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( short i )		{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( ushort i )		{ return new BoxedInteger( i ); }
	public static implicit operator LuaValue ( int i )			{ return new BoxedInteger( i ); }
	public static explicit operator LuaValue ( uint i )			{ checked { return new BoxedInteger( (int)i ); } }
	public static explicit operator LuaValue ( long i )			{ checked { return new BoxedInteger( (int)i ); } }
	public static explicit operator LuaValue ( ulong i )		{ checked { return new BoxedInteger( (int)i ); } }
	public static implicit operator LuaValue ( float n )		{ return new BoxedDouble( n ); }
	public static implicit operator LuaValue ( double n )		{ return new BoxedDouble( n ); }
	public static explicit operator LuaValue ( decimal n )		{ checked { return new BoxedDouble( (double)n ); } }
	public static implicit operator LuaValue ( string s )		{ return new BoxedString( s ); }

	public static explicit operator bool ( LuaValue v )			{ return v != null && v.IsTrue(); }
	public static explicit operator sbyte ( LuaValue v )		{ int value; if ( v.TryToInteger( out value ) ) checked { return (sbyte)value; } else throw new InvalidCastException(); }
	public static explicit operator byte ( LuaValue v )			{ int value; if ( v.TryToInteger( out value ) ) checked { return (byte)value; } else throw new InvalidCastException(); }
	public static explicit operator short ( LuaValue v )		{ int value; if ( v.TryToInteger( out value ) ) checked { return (short)value; } else throw new InvalidCastException(); }
	public static explicit operator ushort ( LuaValue v )		{ int value; if ( v.TryToInteger( out value ) ) checked { return (ushort)value; } else throw new InvalidCastException(); }
	public static explicit operator int ( LuaValue v )			{ int value; if ( v.TryToInteger( out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator uint ( LuaValue v )			{ int value; if ( v.TryToInteger( out value ) ) checked { return (uint)value; } else throw new InvalidCastException(); }
	public static explicit operator long ( LuaValue v )			{ int value; if ( v.TryToInteger( out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator ulong ( LuaValue v )		{ int value; if ( v.TryToInteger( out value ) ) checked { return (ulong)value; } else throw new InvalidCastException(); }
	public static explicit operator float ( LuaValue v )		{ double value; if ( v.TryToDouble( out value ) ) checked { return (float)value; } else throw new InvalidCastException(); }
	public static explicit operator double ( LuaValue v )		{ double value; if ( v.TryToDouble( out value ) ) return value; else throw new InvalidCastException(); }
	public static explicit operator decimal ( LuaValue v )		{ double value; if ( v.TryToDouble( out value ) ) checked { return (decimal)value; } else throw new InvalidCastException(); }
	public static explicit operator string ( LuaValue v )		{ string value; if ( v.TryToString( out value ) ) return value; else throw new InvalidCastException(); }


	// Type.

	public virtual string	GetLuaType()						{ return "userdata"; }
	public virtual bool		IsPrimitiveValue()					{ return false; }
	public virtual bool		TryToInteger( out int v )				{ v = 0; return false; }
	public virtual bool		TryToDouble( out double v )			{ v = 0.0; return false; }
	public virtual bool		TryToString( out string v )			{ v = String.Empty; return false; }
	public virtual bool		TryToNumberValue( out LuaValue v )	{ v = null; return false; }


	// Operations.

	public virtual LuaValue	Add( LuaValue o )					{ return MetaBinaryOp( this, o, handlerAdd ); }
	public virtual LuaValue	Subtract( LuaValue o )				{ return MetaBinaryOp( this, o, handlerSub ); }
	public virtual LuaValue	Multiply( LuaValue o )				{ return MetaBinaryOp( this, o, handlerMul ); }
	public virtual LuaValue	Divide( LuaValue o )				{ return MetaBinaryOp( this, o, handlerDiv ); }
	public virtual LuaValue	IntegerDivide( LuaValue o )			{ return MetaBinaryOp( this, o, handlerIDiv ); }
	public virtual LuaValue	Modulus( LuaValue o )				{ return MetaBinaryOp( this, o, handlerMod ); }
	public virtual LuaValue	RaiseToPower( LuaValue o )			{ return MetaBinaryOp( this, o, handlerPow ); }
	public virtual LuaValue	Concatenate( LuaValue o )			{ return MetaBinaryOp( this, o, handlerConcat ); }

	public virtual LuaValue	UnaryMinus()						{ return MetaUnaryOp( this, handlerUnm ); }
	public virtual LuaValue	Length()							{ return MetaUnaryOp( this, handlerLen ); }

	public virtual bool		IsTrue()							{ return true; }
	public virtual bool		EqualsValue( LuaValue o )			{ return MetaEquals( this, o ); }
	public virtual bool		LessThanValue( LuaValue o )			{ return MetaLessThan( this, o ); }
	public virtual bool		LessThanOrEqualsValue( LuaValue o )	{ return MetaLessThanOrEqual( this, o ); }
		
	public virtual LuaValue	Index( LuaValue k )					{ return MetaIndex( this, k ); }
	public virtual void		NewIndex( LuaValue k, LuaValue v )	{ MetaNewIndex( this, k, v ); }


	// Function invocation.

	public virtual LuaValue InvokeS()														{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this ); else throw new InvalidOperationException(); }
	public virtual LuaValue InvokeS( LuaValue a1 )											{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1 ); else throw new InvalidOperationException(); }
	public virtual LuaValue InvokeS( LuaValue a1, LuaValue a2 )								{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1, a2 ); else throw new InvalidOperationException(); }
	public virtual LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1, a2, a3 ); else throw new InvalidOperationException(); }
	public virtual LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( new LuaValue[]{ this, a1, a2, a3, a4 } ); else throw new InvalidOperationException(); }
	public virtual LuaValue InvokeS( LuaValue[] arguments )									{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( ForwardArguments( this, arguments ) ); else throw new InvalidOperationException(); }

	public virtual LuaValue[] InvokeM()														{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this ); else throw new InvalidOperationException(); }
	public virtual LuaValue[] InvokeM( LuaValue a1 )										{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1 ); else throw new InvalidOperationException(); }
	public virtual LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )							{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1, a2 ); else throw new InvalidOperationException(); }
	public virtual LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1, a2, a3 ); else throw new InvalidOperationException(); }
	public virtual LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( new LuaValue[]{ this, a1, a2, a3, a4 } ); else throw new InvalidOperationException(); }
	public virtual LuaValue[] InvokeM( LuaValue[] arguments )								{ LuaValue h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( ForwardArguments( this, arguments ) ); else throw new InvalidOperationException(); }


	// Meta handlers.

	protected static readonly LuaValue handlerAdd		= "__add";
	protected static readonly LuaValue handlerSub		= "__sub";
	protected static readonly LuaValue handlerMul		= "__mul";
	protected static readonly LuaValue handlerDiv		= "__div";
	protected static readonly LuaValue handlerIDiv		= "__idiv";
	protected static readonly LuaValue handlerMod		= "__mod";
	protected static readonly LuaValue handlerPow		= "__pow";
	protected static readonly LuaValue handlerConcat	= "__concat";
	protected static readonly LuaValue handlerUnm		= "__unm";
	protected static readonly LuaValue handlerLen		= "__len";
	protected static readonly LuaValue handlerEq		= "__eq";
	protected static readonly LuaValue handlerLt		= "__lt";
	protected static readonly LuaValue handlerLe		= "__le";
	protected static readonly LuaValue handlerIndex		= "__index";
	protected static readonly LuaValue handlerNewIndex	= "__newindex";
	protected static readonly LuaValue handlerCall		= "__call";


	protected static LuaValue GetHandler( LuaValue o, LuaValue handler )
	{
		return o != null && o.Metatable != null ? o.Metatable[ handler ] : null;
	}


	protected static LuaValue MetaBinaryOp( LuaValue left, LuaValue right, LuaValue handler )
	{
		LuaValue h = GetHandler( left, handler );
		if ( h != null )
		{
			return h.InvokeS( left, right );
		}
		h = GetHandler( right, handler );
		if ( h != null )
		{
			return h.InvokeS( left, right );
		}
		throw new InvalidOperationException();
	}


	protected static LuaValue MetaUnaryOp( LuaValue operand, LuaValue handler )
	{
		LuaValue h = GetHandler( operand, handler );
		if ( h != null )
		{
			return h.InvokeS( operand );
		}
		throw new InvalidOperationException();
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
		if ( left.GetLuaType() != right.GetLuaType() )
		{
			return false;
		}
		if ( Object.Equals( left, right ) )
		{
			return true;
		}

		LuaValue h = GetComparisonHandler( left, right, handlerEq );
		if ( h != null )
		{
			LuaValue result = h.InvokeS( left, right );
			return (bool)result;
		}

		return false;
	}


	protected static bool MetaLessThan( LuaValue left, LuaValue right )
	{
		LuaValue h = GetComparisonHandler( left, right, handlerLt );
		if ( h != null )
		{
			LuaValue result = h.InvokeS( left, right );
			return (bool)result;
		}

		throw new InvalidOperationException();
	}


	protected static bool MetaLessThanOrEqual( LuaValue left, LuaValue right )
	{
		LuaValue h = GetComparisonHandler( left, right, handlerLe );
		if ( h != null )
		{
			LuaValue result = h.InvokeS( left, right );
			return (bool)result;
		}

		h = GetComparisonHandler( left, right, handlerLt );
		if ( h != null )
		{
			LuaValue result = h.InvokeS( right, left );
			return (bool)result;
		}
		
		throw new InvalidOperationException();
	}


	protected static LuaValue MetaIndex( LuaValue table, LuaValue key )
	{
		LuaValue h = GetHandler( table, handlerIndex );
		if ( h is LuaFunction )
		{
			return h.InvokeS( table, key );
		}
		else if ( h != null )
		{
			return h.Index( key );
		}

		throw new InvalidOperationException();
	}
	

	protected static void MetaNewIndex( LuaValue table, LuaValue key, LuaValue value )
	{
		LuaValue h = GetHandler( table, handlerNewIndex );
		if ( h is LuaFunction )
		{
			h.InvokeS( table, key, value );
			return;
		}
		else if ( h != null )
		{
			h.NewIndex( key, value );
			return;
		}

		throw new InvalidOperationException();
	}


	protected static LuaValue[] ForwardArguments( LuaValue function, LuaValue[] arguments )
	{
		LuaValue[] newArguments = new LuaValue[ arguments.Length + 1 ];
		newArguments[ 0 ] = function;
		arguments.CopyTo( newArguments, 1 );
		return newArguments;
	}


}


}