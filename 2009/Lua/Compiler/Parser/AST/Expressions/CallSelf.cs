// CallSelf.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


public class CallSelf
	:	Expression
{
	public Expression			Object				{ get; private set; }
	public string				MethodName			{ get; private set; }
	public IList< Expression >	Arguments			{ get; private set; }
	public Expression			ArgumentValues		{ get; private set; }


	public CallSelf( SourceSpan s, Expression o, string methodName, IList< Expression > arguments, Expression argumentValues )
		:	base( s )
	{
		Object			= o;
		MethodName		= methodName;
		Arguments		= arguments;
		ArgumentValues	= argumentValues;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}