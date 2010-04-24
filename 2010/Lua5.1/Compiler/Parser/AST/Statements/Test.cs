// Test.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Bytecode;


namespace Lua.Compiler.Parser.AST.Statements
{


/*	This statement branches to target if condition is false.
*/

class Test
	:	Statement
{
	public Expression	Condition	{ get; private set; }
	public LabelAST		Target		{ get; private set; }

		
	public Test( SourceSpan s, Expression condition, LabelAST target )
		:	base( s )
	{
		Condition	= condition;
		Target		= target;
	}
	

	public override void Accept( IStatementVisitor v )
	{
		v.Visit( this );
	}
}


}