// ReturnMultipleValues.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST.Statements
{


public class ReturnMultipleValues
	:	Statement
{
	public IList< Expression >	Results			{ get; private set; }
	public Expression			ResultValues	{ get; private set; }


	public ReturnMultipleValues( SourceSpan s, IList< Expression > results, Expression resultValues )
		:	base( s )
	{
		Results			= results;
		ResultValues	= resultValues;
	}


	public override void Accept( IStatementVisitor s )
	{
		s.Visit( this );
	}

}


}
