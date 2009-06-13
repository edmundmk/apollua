// Constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST.Expressions;


namespace Lua.Parser.AST
{


public class Constructor
	:	Statement
{
	public Temporary			Temporary		{ get; private set; }
	public int					ArrayCount		{ get; private set; }
	public int					HashCount		{ get; private set; }
	public IList< Statement >	Statements		{ get; private set; }

	List< Statement > statements;


	public Constructor( SourceSpan s, Temporary temporary )
		:	this( s, temporary, 0, 0 )
	{
	}

	public Constructor( SourceSpan s, Temporary temporary, int arrayCount, int hashCount )
		:	base( s )
	{
		statements	= new List< Statement >();

		Temporary	= temporary;
		ArrayCount	= arrayCount;
		HashCount	= hashCount;
		Statements	= statements.AsReadOnly();
	}


	public void IncrementArrayCount()
	{
		ArrayCount += 1;
	}

	public void IncrementHashCount()
	{
		HashCount += 1;
	}

	public void Statement( Statement statement )
	{
		statements.Add( statement );
	}


	public override void Accept( IStatementVisitor s )
	{
		s.Visit( this );
	}

}


}