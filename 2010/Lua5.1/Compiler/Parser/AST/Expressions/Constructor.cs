// Constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class Constructor
	:	Expression
{
	public int							ArrayCount		{ get; private set; }
	public int							HashCount		{ get; private set; }
	public IList< ConstructorElement >	Elements		{ get; private set; }
	public Expression					ElementList		{ get; private set; }


	public Constructor( SourceSpan s, int arrayCount, int hashCount,
				IList< ConstructorElement > elements, Expression elementList )
		:	base( s )
	{
		ArrayCount	= arrayCount;
		HashCount	= hashCount;
		Elements	= elements;
		ElementList	= elementList;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}



}