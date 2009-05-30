// ToNumberExpression.cs
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



// tonumber( operand )

sealed class ToNumberExpression
	:	IRExpression
{
	public IRExpression	Operand		{ get; private set; }


	public ToNumberExpression( SourceLocation l, IRExpression operand )
		:	base( l )
	{
		Operand	 = operand;
	}


	public override IRExpression Transform( IRCode code )
	{
		Operand = Operand.TransformSingleValue( code );
		return base.Transform( code );
	}


	public override string ToString()
	{
		return String.Format( "tonumber( {0} )", Operand );
	}

}




}

