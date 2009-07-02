// LuaField.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Reflection;


namespace Lua.Interop
{

	
/*	Abstracts getting and setting fields.
*/

public abstract class LuaField
{
	public abstract Value GetValue( object o );
	public abstract void SetValue( object o, Value v );
}


public class LuaField< T >
	:	LuaField
{
	FieldInfo field;

	public LuaField( FieldInfo field )
	{
		this.field = field;
	}

	public override Value GetValue( object o )
	{
		return InteropHelpers.CastResultS( (T)field.GetValue( o ) );
	}

	public override void SetValue( object o, Value v )
	{
		field.SetValue( o, InteropHelpers.Cast< T >( v ) );
	}
}


}





