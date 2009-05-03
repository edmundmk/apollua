// Value.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public abstract class Value
{

	// Metatable.

	public virtual Table Metatable
	{
		get { return null; }
		set { throw new InvalidOperationException(); }
	}


	// Conversion.

	public virtual bool TryToInteger( out int value )	{ value = 0; return false; }
	public virtual bool TryToNumber( out double value )	{ value = 0.0; return false; }



	// Binary arithmetic operators.

	public virtual Value Add( Value o )					{ return MetaOperation( this, o, "__add" ); }
	public virtual Value Subtract( Value o )			{ return MetaOperation( this, o, "__sub" ); }
	public virtual Value Multiply( Value o )			{ return MetaOperation( this, o, "__mul" ); }
	public virtual Value Divide( Value o )				{ return MetaOperation( this, o, "__div" ); }
	public virtual Value IntegerDivide( Value o )		{ return MetaOperation( this, o, "__idiv" ); }
	public virtual Value Modulus( Value o )				{ return MetaOperation( this, o, "__mod" ); }
	public virtual Value RaiseToPower( Value o )		{ return MetaOperation( this, o, "__pow" ); }
	public virtual Value Concatenate( Value o )			{ return MetaOperation( this, o, "__concat" ); }



	// Unary arithmetic operators.

	public virtual Value UnaryMinus()					{ return MetaOperation( this, "__unm" ); }
	public virtual Value Length()						{ return MetaOperation( this, "__len" ); }



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

		Value h = GetComparisonHandler( this, o, "__eq" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, o ).IsTrue();
		}

		return false;
	}

	public virtual bool LessThan( Value o )
	{
		Value h = GetComparisonHandler( this, o, "__lt" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, o ).IsTrue();
		}

		throw new InvalidOperationException();
	}

	public virtual bool LessThanOrEqual( Value o )
	{
		Value h = GetComparisonHandler( this, o, "__le" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, o ).IsTrue();
		}

		h = GetComparisonHandler( this, o, "__lt" );
		if ( h != Nil.Instance )
		{
			return ! h.InvokeS( o, this ).IsTrue();
		}
		
		throw new InvalidOperationException();
	}



	// Indexing.

	public virtual Value Index( Value key )
	{
		Value h = GetHandler( this, "__index" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, key );
		}

		throw new InvalidOperationException();
	}
	
	public virtual void NewIndex( Value key, Value value )
	{
		Value h = GetHandler( this, "__newindex" );
		if ( h != Nil.Instance )
		{
			h.InvokeS( this, key, value );
		}

		throw new InvalidOperationException();
	}



	// Function call.

	public virtual Value[] InvokeM()
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeM( this );
		}

		throw new InvalidOperationException();
	}
	
	public virtual Value[] InvokeM( Value arg )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeM( this, arg );
		}

		throw new InvalidOperationException();
	}

	public virtual Value[] InvokeM( Value arg1, Value arg2 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeM( this, arg1, arg2 );
		}

		throw new InvalidOperationException();
	}

	public virtual Value[] InvokeM( Value arg1, Value arg2, Value arg3 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeM( this, arg1, arg2, arg3 );
		}

		throw new InvalidOperationException();
	}

	public virtual Value[] InvokeM( Value arg1, Value arg2, Value arg3, Value arg4 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeM( new Value[]{ this, arg1, arg2, arg3, arg4 } );
		}

		throw new InvalidOperationException();
	}

	public virtual Value[] InvokeM( params Value[] arguments )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			Value[] harguments = new Value[ arguments.Length + 1 ];
			harguments[ 0 ] = this;
			arguments.CopyTo( harguments, 1 );

			return h.InvokeM( harguments );
		}

		throw new InvalidOperationException();
	}


	public virtual Value InvokeS()
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this );
		}

		throw new InvalidOperationException();
	}

	public virtual Value InvokeS( Value arg )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, arg );
		}

		throw new InvalidOperationException();
	}

	public virtual Value InvokeS( Value arg1, Value arg2 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, arg1, arg2 );
		}

		throw new InvalidOperationException();
	}

	public virtual Value InvokeS( Value arg1, Value arg2, Value arg3 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( this, arg1, arg2, arg3 );
		}

		throw new InvalidOperationException();
	}

	public virtual Value InvokeS( Value arg1, Value arg2, Value arg3, Value arg4 )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			return h.InvokeS( new Value[]{ this, arg1, arg2, arg3, arg4 } );
		}

		throw new InvalidOperationException();
	}

	public virtual Value InvokeS( Value[] arguments )
	{
		Value h = GetHandler( this, "__call" );
		if ( h != Nil.Instance )
		{
			Value[] harguments = new Value[ arguments.Length + 1 ];
			harguments[ 0 ] = this;
			arguments.CopyTo( harguments, 1 );

			return h.InvokeS( harguments );
		}

		throw new InvalidOperationException();
	}





	// Meta handlers.

	protected static Value GetHandler( Value o, string handler )
	{
		return null;
	}


	protected static Value MetaOperation( Value left, Value right, string handler )
	{
		throw new InvalidOperationException();
	}


	protected static Value MetaOperation( Value operand, string handler )
	{
		throw new InvalidOperationException();
	}


	protected static Value GetComparisonHandler( Value left, Value right, string handler )
	{
		return null;
	}

}


}