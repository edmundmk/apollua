// ConstructorElement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class ConstructorElement
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


