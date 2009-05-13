// NotExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{


// not <operand>

sealed class NotExpression
	:	IRExpression
{

	public IRExpression	Operand		{ get; private set; }


	public NotExpression( SourceLocation l, IRExpression operand )
		:	base( l )
	{
		Operand		= operand;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Operand		= Operand.TransformExpression( code );
	}


	public override string ToString()
	{
		return String.Format( "not {0}", Operand );
	}

}


	
}

