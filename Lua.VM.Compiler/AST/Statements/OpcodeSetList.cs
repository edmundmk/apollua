// OpcodeSetList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;


namespace Lua.VM.Compiler.AST.Statements
{


public class OpcodeSetList
	:	Statement
{
	public Temporary			Temporary		{ get; private set; }
	public int					Key				{ get; private set; }
	public IList< Expression >	Operands		{ get; private set; }
	public Expression			Values			{ get; private set; }


	public OpcodeSetList( SourceSpan s, Temporary temporary,
				int key, IList< Expression > operands, Expression values )
		:	base( s )
	{
		Temporary	= temporary;
		Key			= key;
		Operands	= operands;
		Values		= values;
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
