// Test
//
// Copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lua;
using Lua.Bytecode;
using Lua.Compiler.EmitBytecode;


namespace Test
{


static class EntryPoint
{

	static int Main( string[] arguments )
	{
		if ( arguments.Length != 1 )
		{
			Console.Error.WriteLine( "Usage: {0} <script-file>", Environment.GetCommandLineArgs()[ 0 ] );
			return 1;
		}


		// Compile script.
		LuaBytecode scriptBytecode;
		using ( TextReader sourceReader = File.OpenText( arguments[ 0 ] ) )
		{
			scriptBytecode = BytecodeCompiler.Compile( Console.Error, sourceReader, arguments[ 0 ] );
		}


		// Invoke script.
		LuaFunction scriptFunction = new LuaBytecodeFunction( scriptBytecode );
		Action script = scriptFunction.MakeDelegate< Action >();
		Func< int, string > test = scriptFunction.MakeDelegate< Func< int, string > >();
		script();
		
		
		return 0;
	}


}


}
