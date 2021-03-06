﻿// MarkLabel.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class MarkLabel
	:	Statement
{
	public Label Label	{ get; private set; }
	public Block	Block	{ get; private set; }

		
	public MarkLabel( SourceSpan s, Label label )
		:	base( s )
	{
		Label = label;
		Block = null;
	}
	

	public void SetBlock( Block block )
	{
		Block = block;
		Label.SetBlock( Block );
	}
	

	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}