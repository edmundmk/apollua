// Value.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


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



public abstract class Value
{

	// Metatable.

	public virtual Table Metatable
	{
		get { return null; }
		set { throw new InvalidOperationException(); }
	}


	// Meta handler names.

	static readonly Value handlerAdd		= "__add";
	static readonly Value handlerSub		= "__sub";
	static readonly Value handlerMul		= "__mul";
	static readonly Value handlerDiv		= "__div";
	static readonly Value handlerIDiv		= "__idiv";
	static readonly Value handlerMod		= "__mod";
	static readonly Value handlerPow		= "__pow";
	static readonly Value handlerConcat		= "__concat";
	static readonly Value handlerUnm		= "__unm";
	static readonly Value handlerLen		= "__len";
	static readonly Value handlerEq			= "__eq";
	static readonly Value handlerLt			= "__lt";
	static readonly Value handlerLe			= "__le";
	static readonly Value handlerIndex		= "__index";
	static readonly Value handlerNewIndex	= "__newindex";
	static readonly Value handlerCall		= "__call";



	// Conversion.

	public virtual bool  TryToInteger( out int value )	{ value = 0; return false; }
	public virtual bool  TryToNumber( out Value value )	{ value = null; return false; }
	public virtual bool  UsePrimitiveConcatenate()		{ return false; }


	// Binary arithmetic operators.

	public virtual Value Add( Value o )					{ return MetaOperation( this, o, handlerAdd ); }
	public virtual Value Subtract( Value o )			{ return MetaOperation( this, o, handlerSub ); }
	public virtual Value Multiply( Value o )			{ return MetaOperation( this, o, handlerMul ); }
	public virtual Value Divide( Value o )				{ return MetaOperation( this, o, handlerDiv ); }
	public virtual Value IntegerDivide( Value o )		{ return MetaOperation( this, o, handlerIDiv ); }
	public virtual Value Modulus( Value o )				{ return MetaOperation( this, o, handlerMod ); }
	public virtual Value RaiseToPower( Value o )		{ return MetaOperation( this, o, handlerPow ); }
	public virtual Value Concatenate( Value o )			{ return MetaOperation( this, o, handlerConcat ); }



	// Unary arithmetic operators.

	public virtual Value UnaryMinus()					{ return MetaOperation( this, handlerUnm ); }
	public virtual Value Length()						{ return MetaOperation( this, handlerLen ); }



	// Comparison operators.

	public virtual bool IsTrue()
	{
		return true;
	}

	public virtual bool Equals( Value o )
	{
		if ( o.GetType() != GetType() )
		{
			return false;
		}
		if ( o == this )
		{
			return true;
		}

		Value h = GetComparisonHandler( this, o, handlerEq );
		if ( h != null )
		{
			Value result = h.InvokeS( this, o );
			return result != null && result.IsTrue();
		}

		return false;
	}

	public virtual bool LessThan( Value o )
	{
		Value h = GetComparisonHandler( this, o, handlerLt );
		if ( h != null )
		{
			Value result = h.InvokeS( this, o );
			return result != null && result.IsTrue();
		}

		throw new InvalidOperationException();
	}

	public virtual bool LessThanOrEqual( Value o )
	{
		Value h = GetComparisonHandler( this, o, handlerLe );
		if ( h != null )
		{
			Value result = h.InvokeS( this, o );
			return result != null && result.IsTrue();
		}

		h = GetComparisonHandler( this, o, handlerLt );
		if ( h != null )
		{
			Value result = h.InvokeS( o, this );
			return result == null || ! result.IsTrue();
		}
		
		throw new InvalidOperationException();
	}



	// Indexing.

	public virtual Value Index( Value key )
	{
		Value h = GetHandler( this, handlerIndex );
		if ( h != null )
		{
			return h.InvokeS( this, key );
		}

		throw new InvalidOperationException();
	}
	
	public virtual void NewIndex( Value key, Value value )
	{
		Value h = GetHandler( this, handlerNewIndex );
		if ( h != null )
		{
			h.InvokeS( this, key, value );
		}

		throw new InvalidOperationException();
	}



	// Function call.

	public virtual Value InvokeS()												{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this ); throw new InvalidOperationException(); }
	public virtual Value InvokeS( Value a1 )									{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1 ); throw new InvalidOperationException(); }
	public virtual Value InvokeS( Value a1, Value a2 )							{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1, a2 ); throw new InvalidOperationException(); }
	public virtual Value InvokeS( Value a1, Value a2, Value a3 )				{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( this, a1, a2, a3 ); throw new InvalidOperationException(); }
	public virtual Value InvokeS( Value a1, Value a2, Value a3, Value a4 )		{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( new Value[]{ this, a1, a2, a3, a4 } ); throw new InvalidOperationException(); }
	public virtual Value InvokeS( Value[] arguments )							{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeS( ForwardArguments( this, arguments ) ); throw new InvalidOperationException(); }

	public virtual Value[] InvokeM()											{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this ); throw new InvalidOperationException(); }
	public virtual Value[] InvokeM( Value a1 )									{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1 ); throw new InvalidOperationException(); }
	public virtual Value[] InvokeM( Value a1, Value a2 )						{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1, a2 ); throw new InvalidOperationException(); }
	public virtual Value[] InvokeM( Value a1, Value a2, Value a3 )				{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( this, a1, a2, a3 ); throw new InvalidOperationException(); }
	public virtual Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )	{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( new Value[]{ this, a1, a2, a3, a4 } ); throw new InvalidOperationException(); }
	public virtual Value[] InvokeM( Value[] arguments )							{ Value h = GetHandler( this, handlerCall ); if ( h != null ) return h.InvokeM( ForwardArguments( this, arguments ) ); throw new InvalidOperationException(); }




	// Meta handlers.

	protected static Value[] ForwardArguments( Value function, Value[] arguments )
	{
		Value[] newArguments = new Value[ arguments.Length + 1 ];
		newArguments[ 0 ] = function;
		arguments.CopyTo( newArguments, 1 );
		return newArguments;
	}


	protected static Value GetHandler( Value o, Value handler )
	{
		return o != null && o.Metatable != null ? o.Metatable[ handler ] : null;
	}


	protected static Value MetaOperation( Value left, Value right, Value handler )
	{
		Value h = GetHandler( left, handler );
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


	protected static Value MetaOperation( Value operand, Value handler )
	{
		Value h = GetHandler( operand, handler );
		if ( h != null )
		{
			return h.InvokeS( operand );
		}
		throw new InvalidOperationException();
	}


	protected static Value GetComparisonHandler( Value left, Value right, Value handler )
	{
		if ( left != null && right != null && left.GetType() == right.GetType() )
		{
			Value hLeft = GetHandler( left, handler );
			Value hRight = GetHandler( right, handler );
			if ( hLeft == hRight )
			{
				return hLeft;
			}
		}
		return null;
	}




	// Convenience conversions.

	public static implicit operator Value ( bool b )
	{
		return b ? BoxedBoolean.True : BoxedBoolean.False;
	}

	public static implicit operator Value ( sbyte i )
	{
		return new BoxedInteger( i );
	}

	public static implicit operator Value ( byte i )
	{
		return new BoxedInteger( i );
	}

	public static implicit operator Value ( short i )
	{
		return new BoxedInteger( i );
	}

	public static implicit operator Value ( ushort i )
	{
		return new BoxedInteger( i );
	}

	public static implicit operator Value ( int i )
	{
		return new BoxedInteger( i );
	}

	public static explicit operator Value ( uint i )
	{
		checked { return new BoxedInteger( (int)i ); }
	}

	public static explicit operator Value ( long i )
	{
		checked { return new BoxedInteger( (int)i ); }
	}

	public static explicit operator Value ( ulong i )
	{
		checked { return new BoxedInteger( (int)i ); }
	}

	public static implicit operator Value ( float n )
	{
		return new BoxedNumber( n );
	}

	public static implicit operator Value ( double n )
	{
		return new BoxedNumber( n );
	}

	public static explicit operator Value ( decimal n )
	{
		checked { return new BoxedNumber( (double)n ); }
	}

	public static implicit operator Value ( string s )
	{
		return new BoxedString( s );
	}



}


}