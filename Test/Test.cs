// Test
//
// Copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Test
{


static class EntryPoint
{

	static void Main( string[] args )
	{
		// Excercise the table.
		
		Lua.Table table = new Lua.Table();

		for ( int i = 5; i >= 1; --i )
		{
			table.NewIndex( new Lua.Integer( i ), Lua.Boolean.True );
		}

		for ( int i = -2; i <= 8; ++i )
		{
			Console.WriteLine( "{0} {1}", i, table[ new Lua.Integer( i ) ] );
		}

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Lua.Value, Lua.Value > item in table )
		{
			Console.WriteLine( "{0} {1}", item.Key, item.Value );
		}


		Lua.Value key	= null;
		Lua.Value value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "{0} {1}", key, value );

			table.Next( ref key, out value );
		}


		table[ new Lua.Integer( 3 ) ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );


		table[ new Lua.String( "Hello" ) ] = new Lua.String( "World" );


		table[ new Lua.Integer( 3 ) ] = Lua.Boolean.True;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ new Lua.Integer( 3 ) ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ new Lua.Integer( 4 ) ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ new Lua.Integer( 3 ) ] = Lua.Boolean.True;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ new Lua.Integer( 4 ) ] = Lua.Boolean.True;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ new Lua.Integer( 0 ) ] = Lua.Boolean.True;

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Lua.Value, Lua.Value > item in table )
		{
			Console.WriteLine( "{0} {1}", item.Key, item.Value );
		}


		key		= null;
		value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "{0} {1}", key, value );
			table[ key ] = null;

			table.Next( ref key, out value );
		}


		foreach ( KeyValuePair< Lua.Value, Lua.Value > item in table )
		{
			Console.WriteLine( "ELEMENT: {0} {1}", item.Key, item.Value );
		}

		key		= null;
		value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "NEXT: {0} {1}", key, value );
			table[ key ] = null;

			table.Next( ref key, out value );
		}

	}

}


}
