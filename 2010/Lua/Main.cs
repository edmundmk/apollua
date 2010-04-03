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
		if ( arguments.Length != 1 )
		{
			Console.Out.WriteLine( "Usage: Lua <chunk>.luac" );
			return 1;
		}


		LuaBytecode bytecode = new LuaBytecode();
		using ( BinaryReader r = new BinaryReader( File.OpenRead( arguments[ 0 ] ) ) )
		{
			bytecode.Load( r );
		}
		bytecode.Disassemble( Console.Out );
		

		return 0;
	}

}