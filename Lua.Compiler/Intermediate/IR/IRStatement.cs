// IRStatement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR
{


/*	The IR for a function is composed of a list of statements.

	
	Statements in the IR are designed to be easy to turn into IL.  Temporary
	variables and the special valuelist variable (which holds an array of
	values) are used to ensure that:

	  o  The IL generator does not have to worry about the semantics of Lua
	     statements - multiple assignments are split, multiple results are
	     stored in the valuelist.
	  o  The IL evaluation stack is empty between each statement.
	  o  There are no function calls in expressions.  Instead call statements
	     are used for every individual call.


	Statements:

		declare <local>
		declare <local> = <expression>
		<target> = <expression>
		valuelist = <expression>
		evaluate <expression>
		return <expression>
		return <results> [ valuelist | varargs ]?
	

	There are three kinds of structural statements.
	
	  o  Blocks represent loops or branches.  They can either be broken (branch to
	     the end of the structure) or continued (branch to the top).  The innermost
	     block with the same name as the break or continue is the branch target.
	  o  Tests are the clauses in an if statement - they skip their contents if the
	     expression is false.
	  o  Scopes represent variable declaration scope, and are tracked for debugging.


	Structural statements:

		block <name>
		{
			break <name>
			continue <name>
		}
		
		test <expression>
		{
		}
		
		scope
		{
		}



	Constructor expressions are a special kind of temporary.  They are live
	beyond a single reference; they exist during table construction and for
	a single reference afterwards.

	constructor <constructor>
	{
	}

*/


abstract class IRStatement
{

	public virtual bool		IsReturnStatement	{ get { return false; } }
	public SourceLocation	Location			{ get; private set; }
	
	
	public IRStatement( SourceLocation l )
	{
		Location = l;
	}

}






}

