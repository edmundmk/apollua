// AssignList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


public class AssignList
	:	Statement
{
	public IList< Expression >	Targets		{ get; private set; }
	public IList< Expression >	Values		{ get; private set; }
	public Expression			ValueList	{ get; private set; }


	public AssignList( SourceSpan s, IList< Expression > targets, IList< Expression > values, Expression valueList )
		:	base( s )
	{
		Targets		= targets;
		Values		= values;
		ValueList	= valueList;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}