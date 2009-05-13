// Compiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate;
using Lua.Compiler.Intermediate.IR;


namespace Lua.Compiler
{


public static class LuaCompiler
{


	public static Function Compile( string sourceFilePath )
	{
		StringWriter errors = new StringWriter();
		Parser parser = new Parser( errors, sourceFilePath );
		return Compile( errors, parser );
	}


	public static Function Compile( string sourceName, TextReader sourceReader )
	{
		StringWriter errors	= new StringWriter();
		Parser parser = new Parser( errors, sourceName, sourceReader );
		return Compile( errors, parser );
	}

	
	static Function Compile( StringWriter errors, Parser parser )
	{
		IRCode intermediateRepresentation = (IRCode)parser.Parse( new IRCompiler() );
		intermediateRepresentation.Disassemble( Console.Out );
		return null;
	}


}


}