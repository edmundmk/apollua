// Branch.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class Branch
	:	Statement
{
	public LabelAST Target { get; private set; }

		
	public Branch( SourceSpan s, LabelAST target )
		:	base( s )
	{
		Target = target;
	}
	

	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}