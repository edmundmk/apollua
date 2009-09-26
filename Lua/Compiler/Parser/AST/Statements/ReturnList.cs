// ReturnList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Compiler.Parser.AST.Statements
{


public class ReturnList
	:	Statement
{
	public IList< Expression >	Results			{ get; private set; }
	public Expression			ResultList		{ get; private set; }


	public ReturnList( SourceSpan s, IList< Expression > results, Expression resultList )
		:	base( s )
	{
		Results		= results;
		ResultList	= resultList;
	}


	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}

}


}
