// Assign.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class Assign
	:	Statement
{
	public Expression	Target		{ get; private set; }
	public Expression	Value		{ get; private set; }


	public Assign( SourceSpan s, Expression target, Expression value )
		:	base( s )
	{
		Target	= target;
		Value	= value;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}