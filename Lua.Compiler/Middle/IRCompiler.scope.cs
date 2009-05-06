// IRCompiler.scope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front;
using Lua.Compiler.Front.AST;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Middle.IR;


namespace Lua.Compiler.Middle
{


sealed partial class IRCompiler
	:	IParserActions
{

	public Scope Function( SourceLocation l, Scope scope, IList< string > parameternamelist, bool isVararg )
	{
		throw new NotImplementedException();
	}

	public Code EndFunction( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope Do( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void EndDo( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope If( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope ElseIf( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope Else( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void EndIf( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope While( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public void EndWhile( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope Repeat( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void Until( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope For( SourceLocation l, Scope scope, string varname, Expression start, Expression limit, Expression step )
	{
		throw new NotImplementedException();
	}

	public void EndFor( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope ForIn( SourceLocation l, Scope scope, IList< string > variablenamelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void EndForIn( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

}


}

