// StatementVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


public abstract class StatementVisitor
{
	public virtual void Visit( BeginBlock s )			{}
	public virtual void Visit( EndBlock s )				{}
	public virtual void Visit( BeginConstructor s )		{}
	public virtual void Visit( EndConstructor s )		{}
	public virtual void Visit( BeginScope s )			{}
	public virtual void Visit( EndScope s )				{}
	public virtual void Visit( BeginTest s )			{}
	public virtual void Visit( EndTest s )				{}

	public virtual void Visit( Assign s )				{}
	public virtual void Visit( Break s )				{}
	public virtual void Visit( Continue s )				{}
	public virtual void Visit( Declare s )				{}
	public virtual void Visit( Evaluate s )				{}
	public virtual void Visit( IndexMultipleValues s )	{}
	public virtual void Visit( Return s )				{}
	public virtual void Visit( ReturnMultipleValues s )	{}
}


}

