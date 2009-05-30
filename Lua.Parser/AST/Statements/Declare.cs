// Declare.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Statements
{


public class Declare
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


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


}