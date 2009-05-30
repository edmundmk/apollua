// CallExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{



// <function>( <arguments> [, valuelist |, varargs ] )

sealed class CallExpression
	:	BaseCallExpression
{

	public IRExpression	Function	{ get; private set; }

	
	public CallExpression( SourceLocation l, IRExpression function, IList< IRExpression > arguments )
		:	base( l, arguments )
	{
		Function = function;
	}


	public override IRExpression Transform( IRCode code )
	{
		Function = Function.TransformSingleValue( code );
		return base.Transform( code );
	}


	public override string ToString()
	{
		return String.Format( "{0}{1}", Function, ArgumentsToString() );
	}

}




}

