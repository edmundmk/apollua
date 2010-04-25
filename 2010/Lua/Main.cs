// Lua
//
// © Edmund Kapusniak 2010


using System;
using System.IO;
using Lua;


static class EntryPoint
{

	public static int Main( string[] arguments )
	{
		if ( arguments.Length < 1 )
		{
			Console.Out.WriteLine( "Usage: Lua <chunk>.luac" );
			return 1;
		}

		
		try
		{
			// Test parsing.
			using ( TextReader r = File.OpenText( arguments[ 0 ] ) )
			{
				Lua.Compiler.Parser.TestParser.Parse( Console.Error, r, arguments[ 0 ] );
			}

/*

			// Load .luac
			LuaPrototype prototype;
			using ( BinaryReader r = new BinaryReader( File.OpenRead( arguments[ 0 ] ) ) )
			{
				prototype = LuaPrototype.Load( r );
			}
			//prototype.Disassemble( Console.Out );


			// Build arg table.
			LuaTable arg = new LuaTable();
			for ( int argument = 0; argument < arguments.Length; ++argument )
			{
				arg[ argument ] = arguments[ argument ];
			}


			// Modify environment.
			LuaThread.CurrentThread.Environment[ "arg" ] = arg;


			// Invoke script.
			LuaFunction function = new LuaFunction( prototype );
			Action action = function.MakeDelegate< Action >();
			action();

*/		}
		catch ( Exception e )
		{
			Console.Error.WriteLine( e.ToString() );
			return 1;
		}

		return 0;
	}

}