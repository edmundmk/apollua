// Lexer.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;


namespace Lua.Compiler.Front.Parser
{

	
partial class Lexer
	:	IDisposable
{

	public Lexer( TextWriter errorWriter, string sourceFilePath )
		:	this( errorWriter, sourceFilePath, File.OpenText( sourceFilePath ) )
	{
	}


	public Lexer( TextWriter errorWriter, string sourceName, TextReader sourceReader )
	{
		Construct( errorWriter, sourceName, sourceReader );
	}


	public string SourceName
	{
		get { return sourceName; }
	}

	public bool HasError
	{
		get { return hasError; }
	}


	public Token ReadToken()
	{
		return Lex();
	}

	
	public void Dispose()
	{
		if ( sourceReader != null )
		{
			sourceReader.Dispose();
			sourceReader = null;
		}
	}
	

	public static string GetTokenName( TokenKind kind )
	{
		return tokenNames[ kind ];
	}
}


}


