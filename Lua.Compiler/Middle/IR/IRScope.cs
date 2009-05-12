// IRScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


/*	Scopes are used to keep track of state during parsing.
*/

abstract class IRScope
	:	Scope
{
	public virtual bool IsIfScope { get { return false; } }


	public void Declare( IRLocal local )
	{
		Locals.Add( local );
	}


	public virtual void Break( SourceLocation l, IRCode code )
	{
		throw new InvalidOperationException();
	}

	public virtual void Continue( SourceLocation l, IRCode code )
	{
		throw new InvalidOperationException();
	}

}



}

