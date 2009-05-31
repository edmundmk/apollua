// BeginEndConstructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Expressions;


namespace Lua.Parser.AST.Statements
{


public class BeginConstructor
	:	Statement
{
	public Constructor Constructor { get; private set; }


	public BeginConstructor( SourceSpan s, Constructor constructor )
		:	base( s )
	{
		Constructor = constructor;
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


public class EndConstructor
	:	Statement
{
	public EndConstructor( SourceSpan s )
		:	base( s )
	{
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


}

