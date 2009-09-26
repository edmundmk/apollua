// DeclareList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Compiler.Parser.AST.Statements
{


public class DeclareList
	:	Statement
{
	public IList< Variable >	Variables		{ get; private set; }
	public Expression			ValueList		{ get; private set; }


	public DeclareList( SourceSpan s, IList< Variable > variables, Expression valueList )
		:	base( s )
	{
		Variables	= variables;
		ValueList	= valueList;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}