// ConstructorElement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class ConstructorElement
{
	public SourceSpan	SourceSpan	{ get; private set; }
	public Expression	HashKey		{ get; private set; }
	public Expression	Value		{ get; private set; }


	public ConstructorElement( SourceSpan s, Expression value )
		:	this( s, null, value )
	{
	}

	public ConstructorElement( SourceSpan s, Expression hashKey, Expression value )
	{
		SourceSpan	= s;
		HashKey		= hashKey;
		Value		= value;
	}

}



}


