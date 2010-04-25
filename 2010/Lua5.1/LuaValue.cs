// LuaValue.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Reflection;
using Lua.Runtime;
using Lua.Interop;


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

	internal virtual void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		MetaCall( this, thread, frameBase, argumentCount, resultCount );
	}
	
	internal virtual void Resume( LuaThread thread )
	{
		throw new NotSupportedException();
	}
		
	internal virtual LuaValue Call( LuaValue a )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 1 );
		thread.CallArgument( frameBase, 0, a );
		thread.Call( frameBase, 1 );
		LuaValue result = thread.CallResult( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	internal virtual LuaValue Call( LuaValue a, LuaValue b )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 2 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.Call( frameBase, 1 );
		LuaValue result = thread.CallResult( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	internal virtual LuaValue Call( LuaValue a, LuaValue b, LuaValue c )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 3 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, a );
		thread.CallArgument( frameBase, 2, a );
		thread.Call( frameBase, 1 );
		LuaValue result = thread.CallResult( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	internal virtual Delegate MakeDelegate( Type delegateType )
	{
		MethodInfo signature = delegateType.GetMethod( "Invoke" );
		MethodInfo callMethod = InteropHelpers.BindDelegateSignature( signature, callActionTable, callFuncTable );
		return Delegate.CreateDelegate( delegateType, this, callMethod );
	}


	void CallAction()
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 0 );
		thread.Call( frameBase, 0 );
		thread.EndCall( frameBase );
	}

	void CallAction< T >( T a )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 1 );
		thread.CallArgument( frameBase, 0, a );
		thread.Call( frameBase, 0 );
		thread.EndCall( frameBase );
	}

	void CallAction< T1, T2 >( T1 a, T2 b )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 2 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.Call( frameBase, 0 );
		thread.EndCall( frameBase );
	}

	void CallAction< T1, T2, T3 >( T1 a, T2 b, T3 c )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 3 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.CallArgument( frameBase, 2, c );
		thread.Call( frameBase, 0 );
		thread.EndCall( frameBase );
	}

	void CallAction< T1, T2, T3, T4 >( T1 a, T2 b, T3 c, T4 d )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 4 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.CallArgument( frameBase, 2, c );
		thread.CallArgument( frameBase, 3, d );
		thread.Call( frameBase, 0 );
		thread.EndCall( frameBase );
	}

	TResult CallFunc< TResult >()
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 0 );
		thread.Call( frameBase, 1 );
		TResult result = thread.CallResult< TResult >( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	TResult CallFunc< T, TResult >( T a )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 1 );
		thread.CallArgument( frameBase, 0, a );
		thread.Call( frameBase, 1 );
		TResult result = thread.CallResult< TResult >( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	TResult CallFunc< T1, T2, TResult >( T1 a, T2 b )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 2 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.Call( frameBase, 1 );
		TResult result = thread.CallResult< TResult >( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	TResult CallFunc< T1, T2, T3, TResult >( T1 a, T2 b, T3 c )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 3 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.CallArgument( frameBase, 2, c );
		thread.Call( frameBase, 1 );
		TResult result = thread.CallResult< TResult >( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}

	TResult CallFunc< T1, T2, T3, T4, TResult >( T1 a, T2 b, T3 c, T4 d )
	{
		LuaThread thread = LuaThread.CurrentThread;
		int frameBase = thread.BeginCall( this, 4 );
		thread.CallArgument( frameBase, 0, a );
		thread.CallArgument( frameBase, 1, b );
		thread.CallArgument( frameBase, 2, c );
		thread.CallArgument( frameBase, 3, d );
		thread.Call( frameBase, 1 );
		TResult result = thread.CallResult< TResult >( frameBase, 0 );
		thread.EndCall( frameBase );
		return result;
	}


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
	
	internal static LuaValue GetHandler( LuaValue o, LuaValue handler )
	{
		return o != null && o.Metatable != null ? o.Metatable[ handler ] : null;
	}


	internal static LuaValue MetaBinaryOp( LuaValue left, LuaValue right, LuaValue handler )
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


	internal static LuaValue MetaConcatenate( LuaValue left, LuaValue right )
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


	internal static LuaValue MetaUnaryOp( LuaValue operand, LuaValue handler )
	{
		LuaValue h = GetHandler( operand, handler );
		if ( h != null )
		{
			return h.Call( operand );
		}

		throw new NotSupportedException();
	}


	internal static LuaValue GetComparisonHandler( LuaValue left, LuaValue right, LuaValue handler )
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


	internal static bool MetaEquals( LuaValue left, LuaValue right )
	{
		if ( left == null || right == null )
		{
			return left == right;
		}
		if ( left.LuaType != right.LuaType )
		{
			return false;
		}
		if ( left.RawEquals( right ) )
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


	internal static bool MetaLessThan( LuaValue left, LuaValue right )
	{
		LuaValue h = GetComparisonHandler( left, right, "__lt" );
		if ( h != null )
		{
			LuaValue result = h.Call( left, right );
			return (bool)result;
		}

		throw new NotSupportedException();
	}


	internal static bool MetaLessThanOrEquals( LuaValue left, LuaValue right )
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


	internal static LuaValue MetaIndex( LuaValue table, LuaValue key )
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
	

	internal static void MetaNewIndex( LuaValue table, LuaValue key, LuaValue value )
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


	internal static void MetaCall( LuaValue function, LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaValue h = GetHandler( function, "__call" );
		if ( h != null )
		{
			thread.StackWatermark( frameBase + 2 + argumentCount );
			
			for ( int p = argumentCount; p >= 0; --p )
			{
				thread.Stack[ frameBase + p + 1 ] = thread.Stack[ frameBase + p ];
			}
			thread.Stack[ frameBase ] = h;

			h.Call( thread, frameBase, argumentCount + 1, resultCount );	
			return;
		}

		throw new NotSupportedException();
	}



	// Interop bind tables.

	static readonly MethodInfo[] callActionTable;
	static readonly MethodInfo[] callFuncTable;

	static LuaValue()
	{
		// Create bind tables.
		callActionTable	= new MethodInfo[ 5 ];
		callFuncTable	= new MethodInfo[ 5 ];

		// Fill them with MethodInfos for our generic call entry points.
		foreach ( MethodInfo methodInfo in typeof( LuaValue ).GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic ) )
		{
			int typeParametersCount = methodInfo.IsGenericMethodDefinition ? methodInfo.GetGenericArguments().Length : 0;
			if ( methodInfo.Name == "CallAction" )
			{
				callActionTable[ typeParametersCount ] = methodInfo;
			}
			else if ( methodInfo.Name == "CallFunc" )
			{
				callFuncTable[ typeParametersCount - 1 ] = methodInfo;
			}
		}
	}


}


}