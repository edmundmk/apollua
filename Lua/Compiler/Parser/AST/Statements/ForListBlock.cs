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
	
	public Variable				Generator			{ get; private set; }
	public Variable				State				{ get; private set; }
	public Variable				Control				{ get; private set; }
	public IList< Variable >	UserVariables		{ get; private set; }
	public LabelAST				BreakLabel			{ get; private set; }
	public LabelAST				ContinueLabel		{ get; private set; }
	

	public ForListBlock( SourceSpan s, Block parent, string name,
				Variable generator, Variable state, Variable control, IList< Variable > userVariables,
				LabelAST breakLabel, LabelAST continueLabel )
		:	base( s, parent, name )
	{
		Generator		= generator;
		State			= state;
		Control			= control;
		UserVariables	= userVariables;
		BreakLabel		= breakLabel;
		ContinueLabel	= continueLabel;
	}
	

	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}