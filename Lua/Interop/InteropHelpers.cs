// InteropHelpers.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Runtime;


namespace Lua.Interop
{


public static class InteropHelpers
{
	// Constants.

	public static readonly object[]	EmptyObjects	= new object[] {};
	public static readonly LuaValue[]	EmptyValues		= new LuaValue[] {};



	// Casting.

	public static T Cast< T >( LuaValue v )
	{
		if ( v == null )
		{
			return default( T );
		}
		else if ( typeof( T ) == typeof( LuaValue ) )
		{
			return (T)(object)v;
		}
		else if ( typeof( T ) == typeof( bool ) )
		{
			return (T)(object)v.IsTrue();
		}
		else if ( typeof( T ) == typeof( sbyte ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				sbyte i;
				checked { i = (sbyte)integer; }
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( byte ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				byte i;
				checked { i = (byte)integer; }
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( short ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				short i;
				checked { i = (short)integer; }
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( ushort ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				ushort i;
				checked { i = (ushort)integer; }
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( int ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				return (T)(object)integer;
			}
		}
		else if ( typeof( T ) == typeof( long ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				long i = integer;
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( ulong ) )
		{
			int integer;
			if ( v.TryToInteger( out integer ) )
			{
				ulong i;
				checked { i = (ulong)integer; }
				return (T)(object)i;
			}
		}
		else if ( typeof( T ) == typeof( float ) )
		{
			if ( v.GetType() == typeof( BoxedInteger ) )
			{
				float real = ( (BoxedInteger)v ).Value;
				return (T)(object)real;
			}
			else if ( v.GetType() == typeof( BoxedDouble ) )
			{
				float real;
				checked { real = (float)( (BoxedDouble)v ).Value; }
				return (T)(object)real;
			}
		}
		else if ( typeof( T ) == typeof( double ) )
		{
			if ( v.GetType() == typeof( BoxedInteger ) )
			{
				double real = ( (BoxedInteger)v ).Value;
				return (T)(object)real;
			}
			else if ( v.GetType() == typeof( BoxedDouble ) )
			{
				return (T)(object)( (BoxedDouble)v ).Value;
			}
		}
		else if ( typeof( T ) == typeof( decimal ) )
		{
			if ( v.GetType() == typeof( BoxedInteger ) )
			{
				decimal real = ( (BoxedInteger)v ).Value;
				return (T)(object)real;
			}
			else if ( v.GetType() == typeof( BoxedDouble ) )
			{
				decimal real;
				checked { real = (decimal)( (BoxedDouble)v ).Value; }
				return (T)(object)real;
			}
		}
		else if ( typeof( T ) == typeof( string ) )
		{
			if ( v.GetType() == typeof( BoxedString ) )
			{
				return (T)(object)( (BoxedString)v ).Value;
			}
		}
		else if ( v.GetType() == typeof( BoxedObject< T > ) )
		{
			// Cast boxed objects.
			return (T)(object)( (BoxedObject< T >)v ).Value;
		}
		else if ( v.GetType().IsSubclassOf( typeof( BoxedObject ) ) )
		{
			// Cast boxed objects (not exactly the same type).
			return (T)(object)( (BoxedObject)v ).GetBoxedValue();
		}

		throw new InvalidCastException();
	}


	public static T Cast< T >( LuaValue[] values, int index )
	{
		if ( index < values.Length )
		{
			return Cast< T >( values[ index ] );
		}
		else
		{
			return Cast< T >( null );
		}
	}


	public static T[] CastParams< T >( LuaValue[] values, int index )
	{
		if ( index < values.Length )
		{
			T[] arguments = new T[ values.Length - index ];
			for ( int argument = 0; argument < arguments.Length; ++argument )
			{
				arguments[ argument ] = Cast< T >( values[ index + argument ] );
			}
			return arguments;
		}
		else
		{
			return new T[] {};
		}
	}


	public static LuaValue CastResultS< T >( T v )
	{
		if ( v == null )
		{
			return null;
		}
		else if ( typeof( T ) == typeof( LuaValue ) )
		{
			return (LuaValue)(object)v;
		}
		else if ( typeof( T ) == typeof( bool ) )
		{
			return (bool)(object)v ? BoxedBoolean.True : BoxedBoolean.False;
		}
		else if ( typeof( T ) == typeof( sbyte ) )
		{
			return new BoxedInteger( (sbyte)(object)v );
		}
		else if ( typeof( T ) == typeof( byte ) )
		{
			return new BoxedInteger( (byte)(object)v );
		}
		else if ( typeof( T ) == typeof( short ) )
		{
			return new BoxedInteger( (short)(object)v );
		}
		else if ( typeof( T ) == typeof( ushort ) )
		{
			return new BoxedInteger( (ushort)(object)v );
		}
		else if ( typeof( T ) == typeof( int ) )
		{
			return new BoxedInteger( (int)(object)v );
		}
		else if ( typeof( T ) == typeof( uint ) )
		{
			uint integer = (uint)(object)v;
			checked { return new BoxedInteger( (int)integer ); }
		}
		else if ( typeof( T ) == typeof( long ) )
		{
			long integer = (long)(object)v;
			checked { return new BoxedInteger( (int)integer ); }
		}
		else if ( typeof( T ) == typeof( ulong ) )
		{
			ulong integer = (ulong)(object)v;
			checked { return new BoxedInteger( (int)integer ); }
		}
		else if ( typeof( T ) == typeof( float ) )
		{
			return new BoxedDouble( (float)(object)v );
		}
		else if ( typeof( T ) == typeof( double ) )
		{
			return new BoxedDouble( (double)(object)v );
		}
		else if ( typeof( T ) == typeof( decimal ) )
		{
			decimal real = (decimal)(object)v;
			checked { return new BoxedDouble( (double)real ); }
		}
		else if ( typeof( T ) == typeof( string ) )
		{
			return new BoxedString( (string)(object)v );
		}
		else
		{
			// Box the object generically (as a userdata).
			return new BoxedObject< T >( v );
		}

		throw new NotSupportedException();
	}


	public static LuaValue[] CastResultM< T >( T v )
	{
		return new LuaValue[] { CastResultS( v ) };
	}


	public static LuaValue CastResultListS< T >( T[] values )
	{
		if ( values.Length > 0 )
		{
			return CastResultS( values[ 0 ] );
		}
		else
		{
			return null;
		}
	}


	public static LuaValue[] CastResultListM< T >( T[] values )
	{
		LuaValue[] results = new LuaValue[ values.Length ];
		for ( int result = 0; result < values.Length; ++result )
		{
			results[ result ] = CastResultS( values[ result ] );
		}
		return results;
	}




	// Wrapping object members.

	static readonly Type[] LuaMethodV = new Type[]
	{
		null,
		typeof( LuaMethodV<> ),
		typeof( LuaMethodV<,> ),
		typeof( LuaMethodV<,,> ),
		typeof( LuaMethodV<,,,> ),
		typeof( LuaMethodV<,,,,> ),
	};

	static readonly Type[] LuaMethodVP = new Type[]
	{
		null,
		null,
		typeof( LuaMethodVP<,> ),
		typeof( LuaMethodVP<,,> ),
		typeof( LuaMethodVP<,,,> ),
		typeof( LuaMethodVP<,,,,> ),
		typeof( LuaMethodVP<,,,,,> ),
	};

	static readonly Type[] LuaMethodS = new Type[]
	{
		null,
		null,
		typeof( LuaMethodS<,> ),
		typeof( LuaMethodS<,,> ),
		typeof( LuaMethodS<,,,> ),
		typeof( LuaMethodS<,,,,> ),
		typeof( LuaMethodS<,,,,,> ),
	};

	static readonly Type[] LuaMethodSP = new Type[]
	{
		null,
		null,
		null,
		typeof( LuaMethodSP<,,> ),
		typeof( LuaMethodSP<,,,> ),
		typeof( LuaMethodSP<,,,,> ),
		typeof( LuaMethodSP<,,,,,> ),
		typeof( LuaMethodSP<,,,,,,> ),
	};

	static readonly Type[] LuaMethodM = new Type[]
	{
		null,
		null,
		typeof( LuaMethodM<,> ),
		typeof( LuaMethodM<,,> ),
		typeof( LuaMethodM<,,,> ),
		typeof( LuaMethodM<,,,,> ),
		typeof( LuaMethodM<,,,,,> ),
	};

	static readonly Type[] LuaMethodMP = new Type[]
	{
		null,
		null,
		null,
		typeof( LuaMethodMP<,,> ),
		typeof( LuaMethodMP<,,,> ),
		typeof( LuaMethodMP<,,,,> ),
		typeof( LuaMethodMP<,,,,,> ),
		typeof( LuaMethodMP<,,,,,,> ),
	};

	public static LuaFunction WrapMethod( Type type, MethodInfo method )
	{
		// Get parameters.

		ParameterInfo[] parameters = method.GetParameters();
		

		// Construct generic type parameters.

		List< Type > typeArguments = new List< Type >();
		typeArguments.Add( type );
		foreach ( ParameterInfo parameter in parameters )
		{
			typeArguments.Add( parameter.ParameterType );
		}

		
		// Check if the method has variable arguments.
		
		bool hasParams = false;
		if ( parameters.Length > 0 )
		{
			ParameterInfo lastParameter = parameters[ parameters.Length - 1 ];
			if (    lastParameter.ParameterType.IsArray
			     && lastParameter.GetCustomAttributes( typeof( ParamArrayAttribute ), false ).Length > 0 )
			{
				typeArguments[ typeArguments.Count - 1 ] = lastParameter.ParameterType.GetElementType();
				hasParams = true;
			}
		}

		
		// Find appropriate generic type for the method function.

		Type[] genericMethodTypeList = null;
		if ( method.ReturnType == typeof( void ) )
		{
			genericMethodTypeList = hasParams ? LuaMethodVP : LuaMethodV;
		}
		else if ( ! method.ReturnType.IsArray )
		{
			typeArguments.Add( method.ReturnType );
			genericMethodTypeList = hasParams ? LuaMethodSP : LuaMethodS;
		}
		else
		{
			typeArguments.Add( method.ReturnType.GetElementType() );
			genericMethodTypeList = hasParams ? LuaMethodMP : LuaMethodM;
		}


		// Construct appropriate method type to marshal the method's parameters.

		Type genericMethodType = null;
		if ( typeArguments.Count < genericMethodTypeList.Length )
		{
			genericMethodType = genericMethodTypeList[ typeArguments.Count ];
		}

		if ( genericMethodType == null )
		{
			throw new TargetParameterCountException();
		}

		Type methodType = genericMethodType.MakeGenericType( typeArguments.ToArray() );


		// Construct function from type.

		ConstructorInfo constructor = methodType.GetConstructor( new Type[] { typeof( MethodInfo ) } );
		return (LuaFunction)constructor.Invoke( new object[] { method } );

	}


	public static LuaProperty WrapProperty( Type type, PropertyInfo property )
	{
		Type propertyType = typeof( LuaProperty<> ).MakeGenericType( new Type[] { property.PropertyType } );
		ConstructorInfo constructor = propertyType.GetConstructor( new Type[] { typeof( PropertyInfo ) } );
		return (LuaProperty)constructor.Invoke( new object[] { property } );
	}


	public static LuaField WrapField( Type type, FieldInfo field )
	{
		Type propertyType = typeof( LuaField<> ).MakeGenericType( new Type[] { field.FieldType } );
		ConstructorInfo constructor = propertyType.GetConstructor( new Type[] { typeof( FieldInfo ) } );
		return (LuaField)constructor.Invoke( new object[] { field } );
	}


}



}



