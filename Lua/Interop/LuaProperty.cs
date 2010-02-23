// LuaProperty.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Reflection;


namespace Lua.Interop
{

	
/*	Abstracts getting and setting properties.
*/

public abstract class LuaProperty
{
	public abstract LuaValue GetValue( object o );
	public abstract void SetValue( object o, LuaValue v );
}


public class LuaProperty< T >
	:	LuaProperty
{
	PropertyInfo property;

	public LuaProperty( PropertyInfo property )
	{
		this.property = property;
	}

	public override LuaValue GetValue( object o )
	{
		return InteropHelpers.BoxS( (T)property.GetValue( o, null ) );
	}

	public override void SetValue( object o, LuaValue v )
	{
		property.SetValue( o, InteropHelpers.Unbox< T >( v ), null );
	}
}




}


