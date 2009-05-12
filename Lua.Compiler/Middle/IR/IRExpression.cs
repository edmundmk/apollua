// IRExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


/*	Expressions are generated from the parse tree.  Then when constructing the IR
	for a function expressions are rewritten:

	  o  Temporaries and valuelist references are introduced to enable
	     representation of Lua semantics in the statement IR.
	  o  Function call expressions are converted into statements.

*/


abstract class IRExpression
	:	Expression
{

	public SourceLocation	Location		{ get; private set; }
	public virtual bool		IsSingleValue	{ get { return true; } }

	
	public IRExpression( SourceLocation l )
	{
		Location = l;
	}


	// Restricting multiple results to a single value.

	public virtual void RestrictToSingleValue()
	{
		// do nothing.
	}



	// Transforming so each function call is a separate statement.

	public virtual void Transform( IRCode code )
	{
		// do nothing.
	}


	public virtual void TransformAssign( IRCode code )
	{
		Transform( code );
	}


	public virtual IRExpression TransformExpression( IRCode code )
	{
		Transform( code );
		return this;
	}

}




}

