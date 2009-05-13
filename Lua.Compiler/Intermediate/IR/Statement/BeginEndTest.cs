// BeginEndTest.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{



/*
	test <expression>		// if expression is false, branch to end.
	{
	}
*/


sealed class BeginTest
	:	IRStatement
{

	public IRExpression	Expression { get; private set; }


	public BeginTest( SourceLocation l, IRExpression expression )
		:	base( l )
	{
		Expression	= expression;
	}


	public override string ToString()
	{
		return String.Format( "test {0} {{", Expression );
	}


}


sealed class EndTest
	:	IRStatement
{
	
	public EndTest( SourceLocation l )
		:	base( l )
	{
	}


	public override string ToString()
	{
		return "}";
	}

}





}

