// DeclareForIndex.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST;


namespace Lua.VM.Compiler
{


public class DeclareForIndex
	:	Statement
{
	public Variable Variable { get; private set; }


	public DeclareForIndex( SourceSpan s, Variable variable )
		:	base( s )
	{
		Variable = variable;
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