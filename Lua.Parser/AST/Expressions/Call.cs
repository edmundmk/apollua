// Call.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST.Expressions
{


public class Call
	:	Expression
{
	public Expression			Function		{ get; private set; }
	public IList< Expression >	Arguments		{ get; private set; }
	public Expression			ArgumentValues	{ get; private set; }


	public Call( SourceSpan s, Expression function, IList< Expression > arguments, Expression argumentValues )
		:	base( s )
	{
		Function		= function;
		Arguments		= arguments;
		ArgumentValues	= argumentValues;
	}


	public override void Accept( ExpressionVisitor v )
	{
		v.Visit( this );
	}

}



}

