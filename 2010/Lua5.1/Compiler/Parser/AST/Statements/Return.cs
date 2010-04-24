// Return.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class Return
	:	Statement
{
	public Expression Result { get; private set; }


	public Return( SourceSpan s, Expression result )
		:	base( s )
	{
		Result = result;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}
