// Declare.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class Declare
	:	Statement
{
	public Variable		Variable		{ get; private set; }
	public Expression	Value			{ get; private set; }


	public Declare( SourceSpan s, Variable variable, Expression value )
		:	base( s )
	{
		Variable	= variable;
		Value		= value;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}