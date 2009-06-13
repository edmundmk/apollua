// Block.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST.Statements
{


public class Block
	:	Statement
{
	public Block				Parent			{ get; private set; }
	public IList< Variable >	Locals			{ get; private set; }
	public IList< Statement >	Statements		{ get; private set; }

	List< Variable >	locals;
	List< Statement >	statements;
	
		
	public Block( SourceSpan s, Block parent )
		:	base( s )
	{
		locals		= new List< Variable >();
		statements	= new List< Statement >();

		Parent		= parent;
		Locals		= locals.AsReadOnly();
		Statements	= statements.AsReadOnly();
	}
	

	public void Local( Variable local )
	{
		locals.Add( local );
	}

	public void Statement( Statement statement )
	{
		statements.Add( statement );
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}
}


}