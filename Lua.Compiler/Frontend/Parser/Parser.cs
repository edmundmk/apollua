// Parser.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Frontend.Parser
{


partial class Parser
	:	IDisposable
{

	public Parser( TextWriter errorWriter, string sourceFilePath )
	{
		lexer = new Lexer( errorWriter, sourceFilePath );
		this.errorWriter = errorWriter;
		hasError = false;
	}


	public Parser( TextWriter errorWriter, string sourceName, TextReader sourceReader )
	{
		lexer = new Lexer( errorWriter, sourceName, sourceReader );
		this.errorWriter = errorWriter;
		hasError = false;
	}


	public bool HasError
	{
		get { return hasError || lexer.HasError; }
	}


	public ObjectCode Parse( IParserActions actions )
	{
		ObjectCode objectCode = chunk( actions );
		if ( ! HasError )
		{
			return objectCode;
		}
		else
		{
			return null;
		}
	}


	public void Dispose()
	{
		if ( lexer != null )
		{
			lexer.Dispose();
			lexer = null;
		}
	}
}



}


