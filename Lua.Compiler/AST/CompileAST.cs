// CompileAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;


namespace Lua.Compiler.AST
{


/*	We implement continuations on the CLR using the technique described in
	http://www.ccs.neu.edu/scheme/pubs/stackhack4.html.  This requires
	functions to be converted to a-normal form (so that we can handle the
	yield exception at every call site), and for liveness information for
	all local variables to be available after every function call (so that
	we know what to save and restore into the stack snapshot).
*/

sealed class CompileAST
	:	Frontend.IParserActions
{
	public Scope Function( SourceLocation l, Scope scope, IList< string > parameternamelist, bool isVararg )
	{
		throw new NotImplementedException();
	}

	public ObjectCode EndFunction( SourceLocation l, Scope end )
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

	public Scope ForIn( SourceLocation l, Scope scope, IList<string> variablenamelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void EndForIn( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public void Local( SourceLocation l, Scope scope, IList<string> namelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void Assignment( SourceLocation l, Scope scope, IList<Expression> variablelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		throw new NotImplementedException();
	}

	public void Break( SourceLocation l, Scope loopScope )
	{
		throw new NotImplementedException();
	}

	public void Return( SourceLocation l, Scope functionScope, IList<Expression> expressionlist )
	{
		throw new NotImplementedException();
	}

	public Expression UnaryExpression( SourceLocation l, Expression operand, TokenKind op )
	{
		throw new NotImplementedException();
	}

	public Expression BinaryExpression( SourceLocation l, Expression left, Expression right, TokenKind op )
	{
		throw new NotImplementedException();
	}

	public Expression FunctionExpression( SourceLocation l, ObjectCode objectCode )
	{
		throw new NotImplementedException();
	}

	public Expression LiteralExpression( SourceLocation l, object value )
	{
		throw new NotImplementedException();
	}

	public Expression VarargsExpression( SourceLocation l, Scope functionScope )
	{
		throw new NotImplementedException();
	}

	public Expression LookupExpression( SourceLocation l, Expression left, Expression key )
	{
		throw new NotImplementedException();
	}

	public Expression CallExpression( SourceLocation l, Expression left, IList< Expression > argumentlist )
	{
		throw new NotImplementedException();
	}

	public Expression SelfCallExpression( SourceLocation l, Expression left, SourceLocation keyl, string key, IList< Expression > argumentlist )
	{
		throw new NotImplementedException();
	}

	public Expression NestedExpression( SourceLocation l, Expression expression )
	{
		throw new NotImplementedException();
	}

	public Expression LocalVariableExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		throw new NotImplementedException();
	}

	public Expression UpValExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		throw new NotImplementedException();
	}

	public Expression GlobalVariableExpression( SourceLocation l, string name )
	{
		throw new NotImplementedException();
	}

	public Scope Constructor( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, Expression key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, int key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void LastField( SourceLocation l, Scope constructorScope, int key, Expression valuelist )
	{
		throw new NotImplementedException();
	}

	public Expression EndConstructor( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}
}


}

