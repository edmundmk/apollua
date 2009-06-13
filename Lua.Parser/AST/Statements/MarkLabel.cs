// MarkLabel.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Statements
{


public class MarkLabel
	:	Statement
{
	public LabelAST Label { get; private set; }

		
	public MarkLabel( SourceSpan s, LabelAST label )
		:	base( s )
	{
		Label = label;
	}
	

	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}
}


}