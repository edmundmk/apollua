// IStatementVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


public interface IStatementVisitor
{
	void Visit( Assign s );
	void Visit( Block s );
	void Visit( Branch s );
	void Visit( Constructor s );
	void Visit( Declare s );
	void Visit( Evaluate s );
	void Visit( IndexMultipleValues s );
	void Visit( MarkLabel s );
	void Visit( Return s );
	void Visit( ReturnMultipleValues s );
	void Visit( Test s );
}


}

