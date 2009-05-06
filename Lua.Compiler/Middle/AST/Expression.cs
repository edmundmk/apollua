// Expression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Front.Parser;


namespace Lua.Compiler.Middle.AST
{


abstract class Expression
	:	Front.AST.Expression
{
}



sealed class UnaryExpression
{
	static readonly Dictionary< TokenKind, MethodInfo > operators = new Dictionary< TokenKind, MethodInfo >
	{
		{ TokenKind.HyphenMinus,		typeof( Value ).GetMethod( "UnaryMinus" )		},
		{ TokenKind.NumberSign,			typeof( Value ).GetMethod( "Length" )			},
	};



	public MethodInfo	Operator	{ get; private set; }
	public Expression	Operand		{ get; private set; }


	public UnaryExpression( TokenKind op, Expression operand )
	{
		Operator	= operators[ op ];
		Operand		= operand;
	}


}



sealed class BinaryExpression
	:	Expression
{
	static readonly Dictionary< TokenKind, MethodInfo > operators = new Dictionary< TokenKind, MethodInfo >
	{
		{ TokenKind.PlusSign,			typeof( Value ).GetMethod( "Add" )				},
		{ TokenKind.HyphenMinus,		typeof( Value ).GetMethod( "Subtract" )			},
		{ TokenKind.Asterisk,			typeof( Value ).GetMethod( "Multiply" )			},
		{ TokenKind.Solidus,			typeof( Value ).GetMethod( "Divide" )			},
		{ TokenKind.None,				typeof( Value ).GetMethod( "IntegerDivide" )	},
		{ TokenKind.PercentSign,		typeof( Value ).GetMethod( "Modulus" )			},
		{ TokenKind.CircumflexAccent,	typeof( Value ).GetMethod( "RaiseToPower" )		},
		{ TokenKind.Concatenate,		typeof( Value ).GetMethod( "Concatenate" )		},
	};



	public MethodInfo	Operator	{ get; private set; }
	public Expression	Left		{ get; private set; }
	public Expression	Right		{ get; private set; }


	public BinaryExpression( Expression left, Expression right, TokenKind op )
	{
		Operator	= operators[ op ];
		Left		= left;
		right		= right;
	}


}

	
sealed class FunctionExpression
	:	Expression
{
}
	

sealed class LiteralExpression
	:	Expression
{
}
	

sealed class VarargsExpression
	:	Expression
{
}


sealed class IndexExpression
	:	Expression
{
}


sealed class CallExpression
	:	Expression
{
}



}

