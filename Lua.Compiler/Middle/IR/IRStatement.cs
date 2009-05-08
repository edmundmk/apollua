// IRStatement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


/*	The IR for a function is composed of a list of statements.
*/

abstract class IRStatement
{

	public SourceLocation Location { get; private set; }

	
	public IRStatement( SourceLocation l )
	{
		Location = l;
	}

}




/*	In the IR, there are three kinds of structural statements.
	
	  o  Blocks represent loops or branches.  They can either be broken (branch to
	     the end of the structure) or continued (branch to the top).  The innermost
	     block with the same name as the break or continue is the branch target.
	  o  Tests are the clauses in an if statement - they skip their contents if the
	     expression is false.
	  o  Scopes represent variable declaration scope, and are tracked for debugging.

*/


/*
	block <name>
	{
		break <name>		// branches to end.
		continue <name>		// branches to top.
	}
*/


sealed class BeginBlock
	:	IRStatement
{

	public string Name { get; private set; }

}


sealed class Break
	:	IRStatement
{

	public string BlockName { get; private set; }

}


sealed class Continue
	:	IRStatement
{

	public string BlockName { get; private set; }

}


sealed class EndBlock
	:	IRStatement
{
}



/*
	test <expression>		// if expression is false, branch to end.
	{
	}
*/


sealed class BeginTest
	:	IRStatement
{

	public IRExpression	Expression { get; private set; }

}


sealed class EndTest
	:	IRStatement
{
}



/*
	scope					// local variable declaration scope.
	{
	}
*/


sealed class BeginScope
	:	IRStatement
{
}


sealed class EndScope
	:	IRStatement
{
}






// Statements.


/*	Statements in the IR are designed to be easy to turn into IL.  Temporary
	variables and the special valuelist variable (which holds an array of
	values) are used to ensure that:

	  o  The IL generator does not have to worry about the semantics of Lua
	     statements - multiple assignments are split, multiple results are
	     stored in the valuelist.
	  o  The IL evaluation stack is empty between each instruction.
	  o  There are no function calls in expressions.  Instead call statements
	     are used for every individual call.

*/




// local <local>

sealed class Declare
	:	IRStatement
{

	public Local					Local		{ get; private set; }

}



// local <local> = <expression>

sealed class DeclareAssign
	:	IRStatement
{

	public Local					Local		{ get; private set; }
	public IRExpression				Expression	{ get; private set; }

}



// <target> = <expression>

sealed class Assign
	:	IRStatement
{

	public IRExpression				Target		{ get; private set; }
	public IRExpression				Expression	{ get; private set; }

}



// <target> = <function>( <arguments> [, valuelist ] )

sealed class Call
	:	IRStatement
{

	public IRExpression				Target		{ get; private set; }
	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// <target> = <object>.<methodname>( <object>, <arguments> [, valuelist ] )

sealed class CallSelf
	:	IRStatement
{

	public IRExpression				Target		{ get; private set; }
	public IRExpression				Object		{ get; private set; }
	public string					MethodName	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// valuelist = <function>( <arguments> [, valuelist ] )

sealed class MultipleResultsCall
	:	IRStatement
{

	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// valuelist = <object>.<methodname>( <object>, <arguments> [, valuelist ] )

sealed class MultipleResultsCallSelf
	:	IRStatement
{

	public IRExpression				Object		{ get; private set; }
	public string					MethodName	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// <table>.InsertRange( <index>, valuelist )

sealed class SetList
	:	IRStatement
{

	public IRExpression				Table		{ get; private set; }
	public int						Index		{ get; private set; }

}



// return <expression>

sealed class Return
	:	IRStatement
{

	public IRExpression				Expression	{ get; private set; }

}



// return <results> [, valuelist ]

sealed class ReturnMultipleResults
	:	IRStatement
{

	public IList< IRExpression >	Results		{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// return <function>( <arguments> [, valuelist] )

sealed class TailCallReturn
	:	IRStatement
{

	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }

}



// return <object>.<methodname>( <object>, <arguments> [, valuelist ] )

sealed class TailCallReturnMultipleResults
	:	IRStatement
{

	public IRExpression				Object		{ get; private set; }
	public string					MethodName	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


}



}

