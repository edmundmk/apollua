// Test
//
// Copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua;


namespace Test
{


static class EntryPoint
{

	static void Main( string[] args )
	{
		// Excercise the table.
		
		Table table = new Table();

		for ( int i = 5; i >= 1; --i )
		{
			table.NewIndex( i, true );
		}

		for ( int i = -2; i <= 8; ++i )
		{
			Console.WriteLine( "{0} {1}", i, table[ i ] );
		}

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Value, Value > item in table )
		{
			Console.WriteLine( "{0} {1}", item.Key, item.Value );
		}


		Value key	= null;
		Value value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "{0} {1}", key, value );

			table.Next( ref key, out value );
		}


		table[ 3 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );


		table[ "Hello" ] = "World";


		table[ 3 ] = 42;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 3 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 4 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 3 ] = 67.432;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 4 ] = false;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 0 ] = "AHAHAAH";

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Value, Value > item in table )
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


		foreach ( KeyValuePair< Value, Value > item in table )
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
