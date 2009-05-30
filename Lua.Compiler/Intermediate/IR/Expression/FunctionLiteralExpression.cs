// FunctionLiteralExpression.cs
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



// function() <ircode> end

sealed class FunctionLiteralExpression
	:	IRExpression
{

	public IRCode		IRCode		{ get; private set; }


	public FunctionLiteralExpression( SourceLocation l, IRCode code )
		:	base( l )
	{
		IRCode	= code;
	}


	public override string ToString()
	{
		return String.Format( "function{0:X}", IRCode.GetHashCode() );
	}

}

	



}

