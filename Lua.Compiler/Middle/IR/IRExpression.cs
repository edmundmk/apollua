// IRExpression.cs
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


/*	Expressions are generated from the parse tree.  Then when constructing the IR
	for a function expressions are rewritten:

	  o  Temporaries and valuelist references are introduced to enable
	     representation of Lua semantics in the statement IR.
	  o  Function call expressions are converted into statements.

*/


abstract class IRExpression
	:	Expression
{

	public SourceLocation	Location	{ get; private set; }

	
	public IRExpression( SourceLocation l )
	{
		Location = l;
	}

}



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


}



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


}



// function() <ircode> end

sealed class FunctionExpression
	:	IRExpression
{

	public IRCode		IRCode		{ get; private set; }


	public FunctionExpression( SourceLocation l, IRCode code )
		:	base( l )
	{
		IRCode	= code;
	}

}




// <value>

sealed class LiteralExpression
	:	IRExpression
{

	public object		Value		{ get; private set; }


	public LiteralExpression( SourceLocation l, object value )
		:	base( l )
	{
		Value		= value;
	}

}



// ...

sealed class VarargsExpression
	:	IRExpression
{

	public VarargsExpression( SourceLocation l )
		:	base( l )
	{
	}

}



// <left>[ <key> ]

sealed class IndexExpression
	:	IRExpression
{

	public IRExpression	Left		{ get; private set; }
	public IRExpression Key			{ get; private set; }


	public IndexExpression( SourceLocation l, IRExpression left, IRExpression key )
		:	base( l )
	{
		Left		= left;
		Key			= key;
	}

}



// <left>( <arguments> )

sealed class CallExpression
	:	IRExpression
{

	public IRExpression				Left		{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }

	
	public CallExpression( SourceLocation l, IRExpression left, IList< IRExpression > arguments )
		:	base( l )
	{
		Left		= left;
		Arguments	= arguments;
	}

}



// <left>[ <key> ]( <left>, <arguments> )

sealed class SelfCallExpression
	:	IRExpression
{
	public IRExpression				Left		{ get; private set; }
	public SourceLocation			KeyLocation	{ get; private set; }
	public string					Key			{ get; private set; }
	public IList< IRExpression >	Arguments	{ get; private set; }


	public SelfCallExpression( SourceLocation l, IRExpression left, SourceLocation keyl, string key, IList< IRExpression > arguments )
		:	base( l )
	{
		Left		= left;
		KeyLocation	= keyl;
		Key			= key;
		Arguments	= arguments;
	}


}



// ( f() ) or ( ... )

sealed class SingleValueExpression
	:	IRExpression
{

	public IRExpression	Expression;


	public SingleValueExpression( SourceLocation l, IRExpression expression )
		:	base( l )
	{
		Expression	= expression;
	}

}




// <local>

sealed class LocalVariableExpression
	:	IRExpression
{

	public Local		Local;


	public LocalVariableExpression( SourceLocation l, Local local )
		:	base( l )
	{
		Local		= local;
	}

}




// <local>

sealed class UpValExpression
	:	IRExpression
{

	public Local		Local;


	public UpValExpression( SourceLocation l, Local local )
		:	base( l )
	{
		Local		= local;
	}

}



// <name>

sealed class GlobalVariableExpression
	:	IRExpression
{

	public string		Name;


	public GlobalVariableExpression( SourceLocation l, string name )
		:	base( l )
	{
		Name		= name;
	}

}



// temporary#<index>

sealed class TemporaryExpression
	:	IRExpression
{

	public int			Index;


	public TemporaryExpression( SourceLocation l, int index )
		:	base( l )
	{
		Index		= index;
	}

}



// valuelist[ <index> ]

sealed class MultipleResultsElementExpression
	:	IRExpression
{

	public int			Index;


	public MultipleResultsElementExpression( SourceLocation l, int index )
		:	base( l )
	{
		Index		= index;
	}

}




}

