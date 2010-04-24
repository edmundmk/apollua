// ForBlock.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class ForBlock
	:	Block
{
	public Variable	Index				{ get; private set; }
	public Variable	Limit				{ get; private set; }
	public Variable	Step				{ get; private set; }
	public Variable	UserIndex			{ get; private set; }
	public LabelAST	BreakLabel			{ get; private set; }
	public LabelAST	ContinueLabel		{ get; private set; }
	

	public ForBlock( SourceSpan s, Block parent, string name,
				Variable index, Variable limit, Variable step, Variable userIndex,
				LabelAST breakLabel, LabelAST continueLabel )
		:	base( s, parent, name )
	{
		Index			= index;
		Limit			= limit;
		Step			= step;
		UserIndex		= userIndex;
		BreakLabel		= breakLabel;
		ContinueLabel	= continueLabel;
	}
	

	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}