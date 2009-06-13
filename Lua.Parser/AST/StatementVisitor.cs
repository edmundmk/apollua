// StatementVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


public abstract class StatementVisitor
{
	public virtual void Visit( Assign s )				{}
	public virtual void Visit( Block s )				{}
	public virtual void Visit( Branch s )				{}
	public virtual void Visit( Declare s )				{}
	public virtual void Visit( Evaluate s )				{}
	public virtual void Visit( IndexMultipleValues s )	{}
	public virtual void Visit( MarkLabel s )			{}
	public virtual void Visit( Return s )				{}
	public virtual void Visit( ReturnMultipleValues s )	{}
	public virtual void Visit( Test s )					{}
}


}

