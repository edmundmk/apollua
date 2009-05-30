// IRCompiler.expression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate.IR;
using Lua.Compiler.Intermediate.IR.Expression;


namespace Lua.Compiler.Intermediate
{


sealed partial class IRCompiler
	:	IParserActions
{

	// IParserActions.

	public Expression UnaryExpression( SourceLocation l, Expression operand, TokenKind op )
	{
		switch ( op )
		{
		case TokenKind.Not:
			return new NotExpression( l, (IRExpression)operand );

		default:
			return new UnaryExpression( l, (IRExpression)operand, op );
		}
	}

	public Expression BinaryExpression( SourceLocation l, Expression left, Expression right, TokenKind op )
	{
		switch ( op )
		{
		case TokenKind.LogicalEqual:
		case TokenKind.NotEqual:
		case TokenKind.LessThanSign:
		case TokenKind.GreaterThanOrEqual:
		case TokenKind.LessThanOrEqual:
		case TokenKind.GreaterThanSign:
			return new ComparisonExpression( l, (IRExpression)left, (IRExpression)right, op );

		case TokenKind.And:
			return new AndExpression( l, (IRExpression)left, (IRExpression)right );
		
		case TokenKind.Or:
			return new OrExpression( l, (IRExpression)left, (IRExpression)right );

		default:
			return new BinaryExpression( l, (IRExpression)left, (IRExpression)right, op );
		}
	}

	public Expression FunctionExpression( SourceLocation l, Code objectCode )
	{
		return new FunctionLiteralExpression( l, (IRCode)objectCode );
	}

	public Expression LiteralExpression( SourceLocation l, object value )
	{
		return new LiteralExpression( l, value );
	}

	public Expression VarargsExpression( SourceLocation l, Scope functionScope )
	{
		return new VarargExpression( l );
	}

	public Expression LookupExpression( SourceLocation l, Expression left, Expression key )
	{
		return new IndexExpression( l, (IRExpression)left, (IRExpression)key );
	}

	public Expression CallExpression( SourceLocation l, Expression left, IList< Expression > argumentlist )
	{
		return new CallExpression( l, (IRExpression)left, CastExpressionList( argumentlist ) );
	}

	public Expression SelfCallExpression( SourceLocation l, Expression left, SourceLocation keyl, string key, IList< Expression > argumentlist )
	{
		return new SelfCallExpression( l, (IRExpression)left, keyl, key, CastExpressionList( argumentlist ) );
	}

	public Expression NestedExpression( SourceLocation l, Expression expression )
	{
		IRExpression operand = (IRExpression)expression;
		operand.RestrictToSingleValue();
		return operand;
	}

	public Expression LocalVariableExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		return new LocalExpression( l, (IRLocal)local );
	}

	public Expression UpValExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		IRLocal upval = (IRLocal)local;
		upval.MarkUpVal();
		code.Peek().MarkUpVal( upval );
		return new UpValExpression( l, upval );
	}

	public Expression GlobalVariableExpression( SourceLocation l, string name )
	{
		return new GlobalExpression( l, name );
	}




}


}

