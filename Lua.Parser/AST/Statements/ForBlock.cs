// ForBlock.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST.Statements
{


public class ForBlock
	:	Block
{
	public Expression	Index				{ get; private set; }
	public Expression	Limit				{ get; private set; }
	public Expression	Step				{ get; private set; }
	public Variable		UserIndex			{ get; private set; }
	public LabelAST		BreakLabel			{ get; private set; }
	public LabelAST		ContinueLabel		{ get; private set; }
	

	public ForBlock( SourceSpan s, Block parent, string name,
				Expression index, Expression limit, Expression step, Variable userIndex,
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
	

	public override void Accept( IStatementVisitor s )
	{
		s.Visit( this );
	}
}


}