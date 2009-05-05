// Scope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Compiler.Front.AST
{


/*	This class represents a declaration scope, such as a function, a do block, a
	clause of an if statement, or a loop.  It contains enough informations to allow
	name lookup to be performed in the frontend, and to allow the usage of break,
	continue and ... to be checked.
*/


abstract class Scope
{
	public virtual bool		IsFunctionScope			{ get { return false; } }
	public virtual bool		IsVarargFunctionScope	{ get { return false; } }
	public virtual bool		IsLoopScope				{ get { return false; } }
	
	public IList< Local >	Locals					{ get; private set; }


	protected Scope()
	{
		Locals = new List< Local >();
	}


	
}


}

