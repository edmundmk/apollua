// Token.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;


namespace Lua.Parser
{


public struct Token
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


