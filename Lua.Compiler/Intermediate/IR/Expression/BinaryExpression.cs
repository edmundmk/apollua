// BinaryExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{



// <left> <operator> <right>

sealed class BinaryExpression
	:	IRExpression
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
	public IRExpression	Left		{ get; private set; }
	public IRExpression	Right		{ get; private set; }


	public BinaryExpression( SourceLocation l, IRExpression left, IRExpression right, TokenKind op )
		:	base( l )
	{
		Operator	= operators[ op ];
		Left		= left;
		Right		= right;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Left		= Left.TransformExpression( code );
		Right		= Right.TransformExpression( code );
	}


}







}

