// BoxedObject.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;


namespace Lua.Interop
{


/*	For casting, we have to be able to recover a boxed value in
	a general way.
*/

public abstract class BoxedObject
	:	LuaValue
{

	public abstract object GetBoxedValue();

}



/*	Exposes a CLR object as a lua value.  Allows method calls and
	property accesses.
*/

public class BoxedObject< TObject >
	:	BoxedObject
{
	// Static type reflection.

	static Dictionary< string, LuaFunction >	methods;
	static Dictionary< string, LuaProperty >	properties;
	static Dictionary< string, LuaField >		fields;


	static BoxedObject()
	{
		Type type = typeof( TObject );
		

		// Construct function objects for all methods.

		methods = new Dictionary< string, LuaFunction >();
		MethodInfo[] methodInfos = type.GetMethods();
		foreach ( MethodInfo method in methodInfos )
		{
			methods.Add( method.Name, InteropHelpers.WrapMethod( type, method ) );
		}
		

		// Cache property info objects.

		properties = new Dictionary< string, LuaProperty >();
		PropertyInfo[] propertyInfos = type.GetProperties();
		foreach ( PropertyInfo property in propertyInfos )
		{
			properties.Add( property.Name, InteropHelpers.WrapProperty( type, property ) );
		}


		// Cache field info objects.

		fields = new Dictionary< string, LuaField >();
		FieldInfo[] fieldInfos = type.GetFields();
		foreach ( FieldInfo field in fieldInfos )
		{
			fields.Add( field.Name, InteropHelpers.WrapField( type, field ) );
		}
	}



	// Value.

	public TObject Value { get; set; }


	public BoxedObject( TObject value )
	{
		Value = value;
	}


	public override object GetBoxedValue()
	{
		return Value;
	}



	// Hashing.

	public override bool Equals( object o )
	{
		if ( o == null )
		{
			return false;
		}
		if ( o.GetType() == typeof( BoxedObject< TObject > ) )
		{
			return Value.Equals( ( (BoxedObject< TObject >)o ).Value );
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



	// Conversion.

	public override string GetLuaType()
	{
		return "userdata";
	}



	// Indexing.

	public override LuaValue Index( LuaValue key )
	{
		string s = key.ToString();
		
		LuaFunction method;
		if ( methods.TryGetValue( s, out method ) )
		{
			return method;
		}

		LuaProperty property;
		if ( properties.TryGetValue( s, out property ) )
		{
			return property.GetValue( Value );
		}

		LuaField field;
		if ( fields.TryGetValue( s, out field ) )
		{
			return field.GetValue( Value );
		}

		throw new KeyNotFoundException();
	}
	
	public override void NewIndex( LuaValue key, LuaValue value )
	{
		string s = key.ToString();

		LuaProperty property;
		if ( properties.TryGetValue( s, out property ) )
		{
			property.SetValue( Value, value );
			return;
		}

		LuaField field;
		if ( fields.TryGetValue( s, out field ) )
		{
			field.SetValue( Value, value );
			return;
		}

		throw new NotSupportedException();
	}



	// Convenience conversions.

	public static implicit operator TObject ( BoxedObject< TObject > o )
	{
		return o.Value;
	}

	public static implicit operator BoxedObject< TObject >( TObject o )
	{
		return new BoxedObject< TObject >( o );
	}

}



}

