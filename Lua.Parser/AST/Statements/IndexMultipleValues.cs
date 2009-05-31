// IndexMultipleValues.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Expressions;


namespace Lua.Parser.AST.Statements
{


public class IndexMultipleValues
	:	Statement
{
	public Constructor	Constructor	{ get; private set; }
	public int			Key			{ get; private set; }
	public Expression	Values		{ get; private set; }


	public IndexMultipleValues( SourceSpan s, Constructor constructor, int key, Expression values )
		:	base( s )
	{
		Constructor	= constructor;
		Key			= key;
		Values		= values;
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


}
