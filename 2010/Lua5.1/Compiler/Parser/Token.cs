// Token.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;
using Lua.Compiler.Parser.AST;


namespace Lua.Compiler.Parser
{


struct Token
{
	public SourceSpan	SourceSpan	{ get; private set; }
	public TokenKind	Kind		{ get; private set; }
	public object		Value		{ get; private set; }


	public Token( SourceSpan sourceSpan, TokenKind kind, object value )
		:	this()
	{
		SourceSpan	= sourceSpan;
		Kind		= kind;
		Value		= value;
	}

}


}


