// ForListBlock.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST.Statements
{


public class ForListBlock
	:	Block
{
	public IList< Variable >	UserVariables		{ get; private set; }
	public IList< Expression >	Expressions			{ get; private set; }
	public Expression			ExpressionList		{ get; private set; }
	public LabelAST				BreakLabel			{ get; private set; }
	public LabelAST				ContinueLabel		{ get; private set; }
	

	public ForListBlock( SourceSpan s, Block parent, string name,
				IList< Variable > userVariables, IList< Expression > expressions, Expression expressionList, 
				LabelAST breakLabel, LabelAST continueLabel )
		:	base( s, parent, name )
	{
		UserVariables	= userVariables;
		Expressions		= expressions;
		ExpressionList	= expressionList;
		BreakLabel		= breakLabel;
		ContinueLabel	= continueLabel;
	}
	

	public override void Accept( IStatementVisitor s )
	{
		s.Visit( this );
	}
}


}