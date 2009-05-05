// Compiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler
{


static class Compiler
{


	static Function Compile( string sourceFilePath )
	{
		StringWriter	errors	= new StringWriter();
		Parser			parser	= new Parser( errors, sourceFilePath );
		return Compile( errors, parser );
	}


	static Function Compile( string sourceName, TextReader sourceReader )
	{
		StringWriter	errors	= new StringWriter();
		Parser			parser	= new Parser( errors, sourceName, sourceReader );
		return Compile( errors, parser );
	}

	
	static Function Compile( StringWriter errors, Parser parser )
	{

		return null;
	}


}


}