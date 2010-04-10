﻿// Lua
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

		
		try
		{
			LuaPrototype prototype;
			using ( BinaryReader r = new BinaryReader( File.OpenRead( arguments[ 0 ] ) ) )
			{
				prototype = LuaPrototype.Load( r );
			}
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