// LuaCompilerCLR.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua;
using Lua.Parser;
using Lua.Parser.AST;


namespace Lua.Compiler.CLR
{

	
public static class LuaCompilerCLR
{


	public static Function Compile( StringWriter errors, TextReader source, string sourceName )
	{
		// Parse the function.

		LuaParser parser = new LuaParser( errors, source, sourceName );
		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}
		

		// A-normal transform.

		ANormalTransform.Transform( functionAST );
		ASTWriter.Write( Console.Out, functionAST );


		// Compile to CLR.

		

		return null;
	}


}


}
