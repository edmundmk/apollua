// UnaryExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


// <operator> <operand>

sealed class UnaryExpression
	:	IRExpression
{
	static readonly Dictionary< TokenKind, MethodInfo > operators = new Dictionary< TokenKind, MethodInfo >
	{
		{ TokenKind.HyphenMinus,		typeof( Value ).GetMethod( "UnaryMinus" )		},
		{ TokenKind.NumberSign,			typeof( Value ).GetMethod( "Length" )			},
	};



	public MethodInfo	Operator	{ get; private set; }
	public IRExpression	Operand		{ get; private set; }


	public UnaryExpression( SourceLocation l, TokenKind op, IRExpression operand )
		:	base( l )
	{
		Operator	= operators[ op ];
		Operand		= operand;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Operand		= Operand.TransformExpression( code );
	}


}


	
}

