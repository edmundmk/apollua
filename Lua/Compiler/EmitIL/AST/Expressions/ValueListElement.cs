// ValueListElement.cs
//
// Lua 5.1 is copyright � 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright � 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright � 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;
using Lua.Compiler.Parser.AST;


namespace Lua.Compiler.EmitIL.AST.Expressions
{


public class ValueListElement
	:	Expression
{
	public int Index { get; private set; }


	public ValueListElement( SourceSpan s, int index )
		:	base( s )
	{
		Index = index;
	}


	public override void Accept( IExpressionVisitor v )
	{
		if ( v is IILExpressionVisitor )
		{
			( (IILExpressionVisitor)v ).Visit( this );
		}
	}

}

}