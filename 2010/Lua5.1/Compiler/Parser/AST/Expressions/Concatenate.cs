// Concat.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST
{


class Concatenate
	:	Expression
{
	public IList< Expression > Operands { get; private set; }
	

	public Concatenate( SourceSpan s, IList< Expression > operands )
		:	base( s )
	{
		Operands = operands;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}
