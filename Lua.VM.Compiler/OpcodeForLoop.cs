// OpcodeForLoop.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;


namespace Lua.VM.Compiler
{


public class OpcodeForLoop
	:	Statement
{
	public Variable Index		{ get; private set; }
	public Variable Limit		{ get; private set; }
	public Variable Step		{ get; private set; }
	public Variable UserIndex	{ get; private set; }
	public LabelAST Target		{ get; private set; }



	public OpcodeForLoop( SourceSpan s, Variable index,
				Variable limit, Variable step, Variable userIndex, LabelAST target )
		:	base( s )
	{
		Index		= index;
		Limit		= limit;
		Step		= step;
		UserIndex	= userIndex;
		Target		= target;
	}



	public override void Accept( IStatementVisitor s )
	{
		VMASTWriter w = s as VMASTWriter;
		if ( w != null )
		{
			w.Visit( this );
		}
	}

}


}