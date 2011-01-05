// Index.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class Index
	:	Expression
{
	public Expression	Table		{ get; private set; }
	public Expression	Key			{ get; private set; }


	public Index( SourceSpan s, Expression table, Expression key )
		:	base( s )
	{
		Table	= table;
		Key		= key;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}