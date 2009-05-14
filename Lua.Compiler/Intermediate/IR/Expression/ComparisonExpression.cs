// ComparisonExpression.cs
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

sealed class ComparisonExpression
	:	IRExpression
{

	static readonly Dictionary< TokenKind, MethodInfo > operators = new Dictionary< TokenKind, MethodInfo >
	{
		{ TokenKind.LogicalEqual,			typeof( Value ).GetMethod( "Equals", new Type[] { typeof( Value ) } ) },
		{ TokenKind.NotEqual,				typeof( Value ).GetMethod( "Equals", new Type[] { typeof( Value ) } ) },
		{ TokenKind.LessThanSign,			typeof( Value ).GetMethod( "LessThan" )			},
		{ TokenKind.GreaterThanOrEqual,		typeof( Value ).GetMethod( "LessThan" )			},
		{ TokenKind.LessThanOrEqual,		typeof( Value ).GetMethod( "LessThanOrEqual" )	},
		{ TokenKind.GreaterThanSign,		typeof( Value ).GetMethod( "LessThanOrEqual" )	},
	};

	static readonly Dictionary< TokenKind, bool > invert = new Dictionary< TokenKind, bool >
	{
		{ TokenKind.LogicalEqual,			false	},
		{ TokenKind.NotEqual,				true	},
		{ TokenKind.LessThanSign,			false	},
		{ TokenKind.GreaterThanOrEqual,		true	},
		{ TokenKind.LessThanOrEqual,		false	},
		{ TokenKind.GreaterThanSign,		true	},
	};




	public MethodInfo	Operator			{ get; private set; }
	public bool			InvertComparison	{ get; private set; }
	public IRExpression	Left				{ get; private set; }
	public IRExpression	Right				{ get; private set; }


	public ComparisonExpression( SourceLocation l, IRExpression left, IRExpression right, TokenKind op )
		:	base( l )
	{
		Operator			= operators[ op ];
		InvertComparison	= invert[ op ];
		Left				= left;
		Right				= right;
	}


	public override IRExpression Transform( IRCode code )
	{
		Left	= Left.TransformSingleValue( code );
		Right	= Right.TransformSingleValue( code );
		return base.Transform( code );
	}

	
	public override string ToString()
	{
		return String.Format( "( {0} ) <{1}{2}> ( {3} )", Left, InvertComparison ? "! " : "", Operator.Name, Right );
	}

}







}

