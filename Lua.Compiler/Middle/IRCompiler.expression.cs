// IRCompiler.expression.cs
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

	// IParserActions.

	public Expression UnaryExpression( SourceLocation l, Expression operand, TokenKind op )
	{
		return new UnaryExpression( l, op, (IRExpression)operand );
	}

	public Expression BinaryExpression( SourceLocation l, Expression left, Expression right, TokenKind op )
	{
		return new BinaryExpression( l, (IRExpression)left, (IRExpression)right, op );
	}

	public Expression FunctionExpression( SourceLocation l, Code objectCode )
	{
		return new FunctionExpression( l, (IRCode)objectCode );
	}

	public Expression LiteralExpression( SourceLocation l, object value )
	{
		return new LiteralExpression( l, value );
	}

	public Expression VarargsExpression( SourceLocation l, Scope functionScope )
	{
		return new VarargsExpression( l );
	}

	public Expression LookupExpression( SourceLocation l, Expression left, Expression key )
	{
		return new IndexExpression( l, (IRExpression)left, (IRExpression)key );
	}

	public Expression CallExpression( SourceLocation l, Expression left, IList< Expression > argumentlist )
	{
		return new CallExpression( l, (IRExpression)left, CopyExpressionList( argumentlist ), ExtraArguments.None );
	}

	public Expression SelfCallExpression( SourceLocation l, Expression left, SourceLocation keyl, string key, IList< Expression > argumentlist )
	{
		return new SelfCallExpression( l, (IRExpression)left, keyl, key, CopyExpressionList( argumentlist ), ExtraArguments.None );
	}

	public Expression NestedExpression( SourceLocation l, Expression expression )
	{
		if (    expression.GetType() == typeof( CallExpression )
			 || expression.GetType() == typeof( SelfCallExpression )
		     || expression.GetType() == typeof( VarargsExpression ) )
		{
			return new SingleValueExpression( l, (IRExpression)expression );
		}
		else
		{
			return expression;
		}
	}

	public Expression LocalVariableExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		return new LocalVariableExpression( l, (IRLocal)local );
	}

	public Expression UpValExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		return new UpValExpression( l, (IRLocal)local );
	}

	public Expression GlobalVariableExpression( SourceLocation l, string name )
	{
		return new GlobalVariableExpression( l, name );
	}




	// Helpers.

	IList< IRExpression > CopyExpressionList( IList< Expression > list )
	{
		List< IRExpression > copy = new List< IRExpression >( list.Count );
		foreach ( Expression expression in list )
		{
			copy.Add( (IRExpression)expression );
		}
		return copy.AsReadOnly();
	}

}


}

