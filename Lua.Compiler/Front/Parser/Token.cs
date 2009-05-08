// Token.cs
//
// Lua 5.1 is copyright � 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright � 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright � 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Front.Parser
{


struct Token
{
	public SourceLocation	Location	{ get; private set; }
	public TokenKind		Kind		{ get; private set; }
	public object			Value		{ get; private set; }


	public Token( SourceLocation location, TokenKind kind, object value )
		:	this()
	{
		Location	= location;
		Kind		= kind;
		Value		= value;
	}
}


}

