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


	public BeginBlock( SourceLocation l, string name )
		:	base( l )
	{
		Name		= name;
	}

}


sealed class Break
	:	IRStatement
{

	public string BlockName { get; private set; }


	public Break( SourceLocation l, string blockName )
		:	base( l )
	{
		BlockName	= blockName;
	}

}


sealed class Continue
	:	IRStatement
{

	public string BlockName { get; private set; }


	public Continue( SourceLocation l, string blockName )
		:	base( l )
	{
		BlockName	= blockName;
	}
}


sealed class EndBlock
	:	IRStatement
{

	public EndBlock( SourceLocation l )
		:	base( l )
	{
	}

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


	public BeginTest( SourceLocation l, IRExpression expression )
		:	base( l )
	{
		Expression	= expression;
	}

}


sealed class EndTest
	:	IRStatement
{
	
	public EndTest( SourceLocation l )
		:	base( l )
	{
	}

}



/*
	scope					// local variable declaration scope.
	{
	}
*/


sealed class BeginScope
	:	IRStatement
{

	public BeginScope( SourceLocation l )
		:	base( l )
	{
	}

}


sealed class EndScope
	:	IRStatement
{

	public EndScope( SourceLocation l )
		:	base( l )
	{
	}

}






// Statements.


/*	Statements in the IR are designed to be easy to turn into IL.  Temporary
	variables and the special valuelist variable (which holds an array of
	values) are used to ensure that:

	  o  The IL generator does not have to worry about the semantics of Lua
	     statements - multiple assignments are split, multiple results are
	     stored in the valuelist.
	  o  The IL evaluation stack is empty between each statement.
	  o  There are no function calls in expressions.  Instead call statements
	     are used for every individual call.

*/




// local <local>

sealed class Declare
	:	IRStatement
{

	public Local					Local		{ get; private set; }


	public Declare( SourceLocation l, Local local )
		:	base( l )
	{
		Local		= local;
	}

}



// local <local> = <expression>

sealed class DeclareAssign
	:	IRStatement
{

	public Local					Local		{ get; private set; }
	public IRExpression				Expression	{ get; private set; }


	public DeclareAssign( SourceLocation l, Local local, IRExpression expression )
		:	base( l )
	{
		Local		= local;
		Expression	= expression;
	}

}



// <target> = <expression>

sealed class Assign
	:	IRStatement
{

	public IRExpression				Target		{ get; private set; }
	public IRExpression				Expression	{ get; private set; }


	public Assign( SourceLocation l, IRExpression target, IRExpression expression )
		:	base( l )
	{
		Target		= target;
		Expression	= expression;
	}

}



// <target> = <function>( <arguments> [, valuelist ] )

sealed class Call
	:	IRStatement
{

	public IRExpression				Target		{ get; private set; }
	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public Call( SourceLocation l, IRExpression target, IRExpression function,
					IList< IRExpression > arguments, bool useResults )
		:	base( l )
	{
		Target		= target;
		Function	= function;
		Arguments	= arguments;
		UseResults	= useResults;
	}

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


	public CallSelf( SourceLocation l, IRExpression target, IRExpression o,
					string methodName, IList< IRExpression > arguments, bool useResults )
		:	base( l )
	{
		Target		= target;
		Object		= o;
		MethodName	= methodName;
		Arguments	= arguments;
		UseResults	= useResults;
	}


}



// valuelist = <function>( <arguments> [, valuelist ] )

sealed class MultipleResultsCall
	:	IRStatement
{

	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public MultipleResultsCall( SourceLocation l, IRExpression function,
					IList< IRExpression > arguments, bool useResults )
		:	base( l )
	{
		Function	= function;
		Arguments	= arguments;
		UseResults	= useResults;
	}

}



// valuelist = <object>.<methodname>( <object>, <arguments> [, valuelist ] )

sealed class MultipleResultsCallSelf
	:	IRStatement
{

	public IRExpression				Object		{ get; private set; }
	public string					MethodName	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public MultipleResultsCallSelf( SourceLocation l, IRExpression o,
					string methodName, IList< IRExpression > arguments, bool useResults )
		:	base( l )
	{
		Object		= o;
		MethodName	= methodName;
		Arguments	= arguments;
		UseResults	= useResults;
	}

}



// <table>.InsertRange( <index>, valuelist )

sealed class SetList
	:	IRStatement
{

	public IRExpression				Table		{ get; private set; }
	public int						Index		{ get; private set; }


	public SetList( SourceLocation l, IRExpression table, int index )
		:	base( l )
	{
		Table		= table;
		Index		= index;
	}

}



// return <expression>

sealed class Return
	:	IRStatement
{

	public IRExpression				Result		{ get; private set; }


	public Return( SourceLocation l, IRExpression result )
		:	base( l )
	{
		Result		= result;
	}

}



// return <results> [, valuelist ]

sealed class ReturnMultipleResults
	:	IRStatement
{

	public IList< IRExpression >	Results		{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public ReturnMultipleResults( SourceLocation l, IList< IRExpression > results, bool useResults )
		:	base( l )
	{
		Results		= results;
		UseResults	= useResults;
	}

}



// return <function>( <arguments> [, valuelist] )

sealed class TailCallReturn
	:	IRStatement
{

	public IRExpression				Function	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public TailCallReturn( SourceLocation l, IRExpression function,
					IList< IRExpression > arguments, bool useResults )
		:	base( l )
	{
		Function	= function;
		Arguments	= arguments;
		UseResults	= useResults;
	}

}



// return <object>.<methodname>( <object>, <arguments> [, valuelist ] )

sealed class TailCallReturnMultipleResults
	:	IRStatement
{

	public IRExpression				Object		{ get; private set; }
	public string					MethodName	{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }
	public bool						UseResults	{ get; private set; }


	public TailCallReturnMultipleResults( SourceLocation l, IRExpression o,
				string methodName, IList< IRExpression > arguments, bool useResults )
		: base( l )
	{
		Object		= o;
		MethodName	= methodName;
		Arguments	= arguments;
		UseResults	= useResults;
	}

}



}

