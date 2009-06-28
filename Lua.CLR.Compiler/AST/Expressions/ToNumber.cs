// ToNumber.cs
//
// Lua 5.1 is copyright � 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright � 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright � 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;


namespace Lua.CLR.Compiler.AST.Expressions
{


public class ToNumber
	:	Expression
{
	public Expression Operand { get; private set; }


	public ToNumber( SourceSpan s, Expression operand )
		:	base( s )
	{
		Operand = operand;
	}


	public override void Accept( IExpressionVisitor v )
	{
		if ( v is ICLRExpressionVisitor )
		{
			( (ICLRExpressionVisitor)v ).Visit( this );
		}
	}

}


}