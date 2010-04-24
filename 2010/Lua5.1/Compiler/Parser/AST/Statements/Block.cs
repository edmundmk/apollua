// Block.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


class Block
	:	Statement
{
	public Block				Parent			{ get; private set; }
	public string				Name			{ get; private set; }
	public IList< Variable >	Locals			{ get; private set; }
	public IList< Statement >	Statements		{ get; private set; }

	List< Variable >	locals;
	List< Statement >	statements;
	
		
	public Block( SourceSpan s, Block parent, string name )
		:	base( s )
	{
		locals		= new List< Variable >();
		statements	= new List< Statement >();

		Parent		= parent;
		Name		= name;
		Locals		= locals.AsReadOnly();
		Statements	= statements.AsReadOnly();
	}
	

	public void Local( Variable local )
	{
		locals.Add( local );
		local.SetBlock( this );
	}

	public void Statement( Statement statement )
	{
		statements.Add( statement );
		if ( statement is MarkLabel )
		{
			( (MarkLabel)statement ).SetBlock( this );
		}
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}