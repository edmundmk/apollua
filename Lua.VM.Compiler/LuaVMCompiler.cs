// LuaCLRCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using Lua;
using Lua.VM;
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.VM.Compiler
{


/*	Each function is compiled to a class deriving from Lua.Function.
*/


public class LuaVMCompiler
{
	// Errors.

	TextWriter				errorWriter;
	bool					hasError;


	// Parser.

	string					sourceName;
	LuaParser				parser;



	public LuaVMCompiler( TextWriter errorWriter, TextReader source, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceName		= sourceName;
		parser				= new LuaParser( errorWriter, source, sourceName );
	}

	public bool HasError
	{
		get { return hasError || parser.HasError; }
	}


	public Function Compile()
	{
		// Parse the function.

		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}
		

		return null;
	}


}


}
