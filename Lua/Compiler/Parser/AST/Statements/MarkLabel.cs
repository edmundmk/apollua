// MarkLabel.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


public class MarkLabel
	:	Statement
{
	public LabelAST Label	{ get; private set; }
	public Block	Block	{ get; private set; }

		
	public MarkLabel( SourceSpan s, LabelAST label )
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