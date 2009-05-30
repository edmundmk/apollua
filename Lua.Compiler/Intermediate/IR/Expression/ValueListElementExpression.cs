// ValueListElementExpression.cs
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


// valuelist[ <index> ]

sealed class ValueListElementExpression
	:	IRExpression
{

	public int Index { get; private set; }


	public ValueListElementExpression( SourceLocation l, int index )
		:	base( l )
	{
		Index = index;
	}


	public override string ToString()
	{
		return String.Format( "valuelist[ {0} ]", Index );
	}

}


}

