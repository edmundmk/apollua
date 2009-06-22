// TemporaryList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;


namespace Lua.VM.Compiler.AST.Expressions
{


public class TemporaryList
	:	Expression
{
	public IList< Temporary > Temporaries { get; private set; }


	public TemporaryList( SourceSpan s, IList< Temporary > temporaries )
		:	base( s )
	{
		Temporaries = temporaries;
	}


	public override void Accept( IExpressionVisitor v )
	{
		VMASTWriter w = v as VMASTWriter;
		if ( w != null )
		{
			w.Visit( this );
		}
		LuaVMCompiler c = v as LuaVMCompiler;
		if ( c != null )
		{
			c.Visit( this );
		}
	}

}


}