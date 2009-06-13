// Branch.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Statements
{


public class Branch
	:	Statement
{
	public LabelAST Target { get; private set; }

		
	public Branch( SourceSpan s, LabelAST target )
		:	base( s )
	{
		Target = target;
	}
	

	public override void Accept( IStatementVisitor s )
	{
		s.Visit( this );
	}
}


}