// ConstructorExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{



// Constructors exist between BeginConstructor and EndConstructor statements,
// after which they are referenced once.

sealed class ConstructorExpression
	:	IRExpression
{

	public int			ArrayCount	{ get; private set; }
	public int			HashCount	{ get; private set; }


	public ConstructorExpression( SourceLocation l )
		:	base( l )
	{
		ArrayCount	= 0;
		HashCount	= 0;
	}


	public void IncrementArrayCount()
	{
		ArrayCount += 1;
	}

	public void IncrementHashCount()
	{
		HashCount += 1;
	}


	public override string ToString()
	{
		return String.Format( "constructor{0:X}", GetHashCode() );
	}

}



}

