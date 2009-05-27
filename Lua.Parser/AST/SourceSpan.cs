// SourceSpan.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST
{


public struct SourceSpan
{
	public SourceLocation	Start		{ get; private set; }
	public SourceLocation	End			{ get; private set; }


	public SourceSpan( SourceLocation start, SourceLocation end )
		:	this()
	{
		Start	= start;
		End		= end;
	}

}
	

}

