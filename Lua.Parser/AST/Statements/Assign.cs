// Assign.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Statements
{


public class Assign
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


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


}