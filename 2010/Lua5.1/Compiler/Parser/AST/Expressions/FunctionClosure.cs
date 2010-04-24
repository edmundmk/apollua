// FunctionClosure.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Expressions
{


class FunctionClosure
	:	Expression
{
	public FunctionAST Function { get; private set; }


	public FunctionClosure( SourceSpan s, FunctionAST function )
		:	base( s )
	{
		Function = function;
	}


	public override void Accept( IExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}