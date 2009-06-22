// OpcodeTForLoop.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST;


namespace Lua.VM.Compiler.AST.Statements
{


public class OpcodeTForLoop
	:	Statement
{
	public Variable				Generator		{ get; private set; }
	public Variable				State			{ get; private set; }
	public Variable				Control			{ get; private set; }  
	public IList< Variable >	Variables		{ get; private set; }
	public LabelAST				Target			{ get; private set; }


	public OpcodeTForLoop( SourceSpan s, Variable generator, Variable state,
				Variable control, IList< Variable > variables, LabelAST target )
		:	base( s )
	{
		Generator	= generator;
		State		= state;
		Control		= control;
		Variables	= variables;
		Target		= target;
	}
	

	public override void Accept( IStatementVisitor s )
	{
		VMASTWriter w = s as VMASTWriter;
		if ( w != null )
		{
			w.Visit( this );
		}
		LuaVMCompiler c = s as LuaVMCompiler;
		if ( c != null )
		{
			c.Visit( this );
		}
	}

}


}