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

	public static readonly object[]		EmptyObjects	= new object[] {};
	public static readonly LuaValue[]	EmptyValues		= new LuaValue[] {};



	// Casting.

	public static T Unbox< T >( LuaValue v )
	{
		if ( v == null )								{ return default( T ); }
		else if ( typeof( T ) == typeof( LuaValue ) )	{ return (T)(object)v; }
		else if ( typeof( T ) == typeof( bool ) )		{ return (T)(object)(bool)v; }
		else if ( typeof( T ) == typeof( sbyte ) )		{ return (T)(object)(sbyte)v; }
		else if ( typeof( T ) == typeof( byte ) )		{ return (T)(object)(byte)v; }
		else if ( typeof( T ) == typeof( short ) )		{ return (T)(object)(short)v; }
		else if ( typeof( T ) == typeof( ushort ) )		{ return (T)(object)(ushort)v; }
		else if ( typeof( T ) == typeof( int ) )		{ return (T)(object)(int)v; }
		else if ( typeof( T ) == typeof( long ) )		{ return (T)(object)(long)v; }
		else if ( typeof( T ) == typeof( ulong ) )		{ return (T)(object)(ulong)v; }
		else if ( typeof( T ) == typeof( float ) )		{ return (T)(object)(float)v; }
		else if ( typeof( T ) == typeof( double ) )		{ return (T)(object)(double)v; }
		else if ( typeof( T ) == typeof( decimal ) )	{ return (T)(object)(decimal)v; }
		else if ( typeof( T ) == typeof( string ) )		{ return (T)(object)(string)v; }

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


	public static LuaValue Box< T >( T v )
	{
		if ( v == null )								{ return null; }
		else if ( typeof( T ) == typeof( LuaValue ) )	{ return (LuaValue)(object)v; }
		else if ( typeof( T ) == typeof( bool ) )		{ return (LuaValue)(bool)(object)v; }
		else if ( typeof( T ) == typeof( sbyte ) )		{ return (LuaValue)(sbyte)(object)v; }
		else if ( typeof( T ) == typeof( byte ) )		{ return (LuaValue)(byte)(object)v; }
		else if ( typeof( T ) == typeof( short ) )		{ return (LuaValue)(short)(object)v; }
		else if ( typeof( T ) == typeof( ushort ) )		{ return (LuaValue)(ushort)(object)v; }
		else if ( typeof( T ) == typeof( int ) )		{ return (LuaValue)(int)(object)v; }
		else if ( typeof( T ) == typeof( uint ) )		{ return (LuaValue)(uint)(object)v; }
		else if ( typeof( T ) == typeof( long ) )		{ return (LuaValue)(long)(object)v; }
		else if ( typeof( T ) == typeof( ulong ) )		{ return (LuaValue)(ulong)(object)v; }
		else if ( typeof( T ) == typeof( float ) )		{ return (LuaValue)(float)(object)v; }
		else if ( typeof( T ) == typeof( double ) )		{ return (LuaValue)(double)(object)v; }
		else if ( typeof( T ) == typeof( decimal ) )	{ return (LuaValue)(decimal)(object)v; }
		else if ( typeof( T ) == typeof( string ) )		{ return (LuaValue)(string)(object)v; }

		else
		{
			// Box the object generically (as a userdata).
			return new BoxedObject< T >( v );
		}

		throw new InvalidCastException();
	}



	// Interop casting helpers.

	public static T UnboxElement< T >( LuaValue[] values, int index )
	{
		if ( index < values.Length )
		{
			return Unbox< T >( values[ index ] );
		}
		else
		{
			return Unbox< T >( null );
		}
	}


	public static T[] UnboxList< T >( LuaValue[] values, int index )
	{
		if ( index < values.Length )
		{
			T[] arguments = new T[ values.Length - index ];
			for ( int argument = 0; argument < arguments.Length; ++argument )
			{
				arguments[ argument ] = Unbox< T >( values[ index + argument ] );
			}
			return arguments;
		}
		else
		{
			return new T[] {};
		}
	}

	public static LuaValue BoxS< T >( T v )
	{
		return Box( v );
	}


	public static LuaValue[] BoxM< T >( T v )
	{
		return new LuaValue[] { Box( v ) };
	}


	public static LuaValue BoxListS< T >( T[] values )
	{
		if ( values.Length > 0 )
		{
			return Box( values[ 0 ] );
		}
		else
		{
			return null;
		}
	}


	public static LuaValue[] BoxListM< T >( T[] values )
	{
		LuaValue[] results = new LuaValue[ values.Length ];
		for ( int result = 0; result < values.Length; ++result )
		{
			results[ result ] = Box( values[ result ] );
		}
		return results;
	}



	// Wrapping object members.

	static readonly Type[][] luaMethodBindTable = new Type[][]
	{
		new Type[]
		{
			null,
			typeof( LuaMethodV<> ),
			typeof( LuaMethodV<,> ),
			typeof( LuaMethodV<,,> ),
			typeof( LuaMethodV<,,,> ),
			typeof( LuaMethodV<,,,,> ),
		},
		new Type[]
		{
			null,
			null,
			typeof( LuaMethodVP<,> ),
			typeof( LuaMethodVP<,,> ),
			typeof( LuaMethodVP<,,,> ),
			typeof( LuaMethodVP<,,,,> ),
			typeof( LuaMethodVP<,,,,,> ),
		},
		new Type[]
		{
			null,
			null,
			typeof( LuaMethodS<,> ),
			typeof( LuaMethodS<,,> ),
			typeof( LuaMethodS<,,,> ),
			typeof( LuaMethodS<,,,,> ),
			typeof( LuaMethodS<,,,,,> ),
		},
		new Type[]
		{
			null,
			null,
			null,
			typeof( LuaMethodSP<,,> ),
			typeof( LuaMethodSP<,,,> ),
			typeof( LuaMethodSP<,,,,> ),
			typeof( LuaMethodSP<,,,,,> ),
			typeof( LuaMethodSP<,,,,,,> ),
		},
		new Type[]
		{
			null,
			null,
			typeof( LuaMethodM<,> ),
			typeof( LuaMethodM<,,> ),
			typeof( LuaMethodM<,,,> ),
			typeof( LuaMethodM<,,,,> ),
			typeof( LuaMethodM<,,,,,> ),
		},
		new Type[]
		{
			null,
			null,
			null,
			typeof( LuaMethodMP<,,> ),
			typeof( LuaMethodMP<,,,> ),
			typeof( LuaMethodMP<,,,,> ),
			typeof( LuaMethodMP<,,,,,> ),
			typeof( LuaMethodMP<,,,,,,> ),
		}
	};


	public static LuaFunction WrapMethod( Type thisType, MethodInfo method )
	{
		Type luaMethodType = BindInteropSignature( thisType, method, luaMethodBindTable );
		ConstructorInfo constructor = luaMethodType.GetConstructor( new Type[] { typeof( MethodInfo ) } );
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



	// Binding to interop signatures.

	public static MethodInfo BindInteropSignature( Type thisType, MethodInfo method, MethodInfo[][] bindTable )
	{
		// Parse signature.
		InteropSignature signature; Type[] typeArguments;
		ParseInteropSignature( thisType, method, out signature, out typeArguments );

		// Instantiate correct method from the bind table.
		MethodInfo bind = bindTable[ (int)signature ][ typeArguments.Length ];
		if ( typeArguments.Length > 0 )
			return bind.MakeGenericMethod( typeArguments );
		else
			return bind;
	}

	public static Type BindInteropSignature( Type thisType, MethodInfo method, Type[][] bindTable )
	{
		// Parse signature.
		InteropSignature signature; Type[] typeArguments;
		ParseInteropSignature( thisType, method, out signature, out typeArguments );

		// Instantiate correct type from the bind table.
		Type bind = bindTable[ (int)signature ][ typeArguments.Length ];
		if ( typeArguments.Length > 0 )
			return bind.MakeGenericType( typeArguments );
		else
			return bind;
	}


	static void ParseInteropSignature( Type thisType, MethodInfo method, out InteropSignature signature, out Type[] typeArguments )
	{
		// Get parameters.
		ParameterInfo[] parameters = method.GetParameters();
		
		// Check if we need to specialize on the type of this.
		bool hasThis = thisType != null;
		
		// Check if the method is void.
		bool hasReturn = method.ReturnType != typeof( void );

		// Check if it's a multiple return function.
		bool hasMultiReturn = method.ReturnType.IsArray;

		// Check if the method has variable arguments.
		bool hasParams = parameters.Length > 0
			&& parameters[ parameters.Length - 1 ].ParameterType.IsArray
			&& parameters[ parameters.Length - 1 ].GetCustomAttributes( typeof( ParamArrayAttribute ), false ).Length > 0;
		
		// Calculate number of generic arguments.
		int typeArgumentsLength = parameters.Length;
		if ( hasThis )
			typeArgumentsLength += 1;
		if ( hasReturn )
			typeArgumentsLength += 1;

		// Create arguments.
		typeArguments = new Type[ typeArgumentsLength ];
		int argument = 0;

		// Fill in this type argument.
		if ( hasThis )
		{
			typeArguments[ argument++ ] = thisType;
		}

		// Fill in parameter arguments.
		for ( int parameter = 0; parameter < parameters.Length; ++parameter )
		{
			typeArguments[ argument++ ] = parameters[ parameter ].ParameterType;
		}

		if ( hasParams )
		{
			typeArguments[ argument - 1 ] = typeArguments[ argument - 1 ].GetElementType();
		}

		// Find interop signature type and fill in return type argument.
		if ( hasMultiReturn )
		{
			typeArguments[ argument++ ] = method.ReturnType.GetElementType();
			signature = hasParams ? InteropSignature.MP : InteropSignature.M;
		}
		else if ( hasReturn )
		{
			typeArguments[ argument++ ] = method.ReturnType;
			signature = hasParams ? InteropSignature.SP : InteropSignature.S;
		}
		else
		{
			signature = hasParams ? InteropSignature.VP : InteropSignature.V;
		}
	}


}



}



