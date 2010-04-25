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
			// Get function.
			LuaPrototype prototype;
			string sourceName = arguments[ 0 ];
			if ( String.Equals( Path.GetExtension( sourceName ), ".luac", StringComparison.InvariantCultureIgnoreCase ) )
			{
				// Load script.
				using ( BinaryReader r = new BinaryReader( File.OpenRead( sourceName ) ) )
				{
					prototype = LuaPrototype.Load( r );
				}
			}
			else
			{
				using ( TextReader r = File.OpenText( sourceName ) )
				{
					Lua.Compiler.Parser.TestParser.Parse( Console.Error, r, sourceName );
				}


				// Compile script.
				using ( TextReader r = File.OpenText( sourceName ) )
				{
					prototype = LuaPrototype.Compile( Console.Error, r, sourceName );
				}
			}

			if ( prototype == null )
			{
				return 1;
			}

			prototype.Disassemble( Console.Out );


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

		}
		catch ( Exception e )
		{
			Console.Error.WriteLine( e.ToString() );
			return 1;
		}

		return 0;
	}

}