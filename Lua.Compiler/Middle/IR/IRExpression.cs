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





// Operations.


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



// Values


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





// Assignable.
	

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




// <local>

sealed class LocalVariableExpression
	:	IRExpression
{

	public IRLocal		Local		{ get; private set; }


	public LocalVariableExpression( SourceLocation l, IRLocal local )
		:	base( l )
	{
		Local		= local;
	}

}




// <local>

sealed class UpValExpression
	:	IRExpression
{

	public IRLocal		Local;


	public UpValExpression( SourceLocation l, IRLocal local )
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




// Transformed temporaries.


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

sealed class ValueListElementExpression
	:	IRExpression
{

	public int			Index;


	public ValueListElementExpression( SourceLocation l, int index )
		:	base( l )
	{
		Index		= index;
	}

}




// Multiple Results.


enum ExtraArguments
{
	None,
	UseValueList,
	UseVararg,
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



// ...

sealed class VarargsExpression
	:	IRExpression
{

	public VarargsExpression( SourceLocation l )
		:	base( l )
	{
	}

}



// <function>( <arguments> [, valuelist |, varargs ] )

sealed class CallExpression
	:	IRExpression
{

	public IRExpression				Function		{ get; private set; }
	public IList< IRExpression >	Arguments		{ get; private set; }
	public ExtraArguments			ExtraArguments	{ get; private set; }

	
	public CallExpression( SourceLocation l, IRExpression function,
			IList< IRExpression > arguments, ExtraArguments extraArguments )
		:	base( l )
	{
		Function			= function;
		Arguments			= arguments;
		ExtraArguments		= extraArguments;
	}

}





// <object>.<methodname>( <object>, <arguments> [, valuelist |, varargs ] )

sealed class SelfCallExpression
	:	IRExpression
{
	public IRExpression				Object				{ get; private set; }
	public SourceLocation			MethodNameLocation	{ get; private set; }
	public string					MethodName			{ get; private set; }
	public IList< IRExpression >	Arguments			{ get; private set; }
	public ExtraArguments			ExtraArguments		{ get; private set; }


	public SelfCallExpression( SourceLocation l, IRExpression o, SourceLocation methodNameLocation,
					string methodName, IList< IRExpression > arguments, ExtraArguments extraArguments )
		:	base( l )
	{
		Object				= o;
		MethodNameLocation	= methodNameLocation;
		MethodName			= methodName;
		Arguments			= arguments;
		ExtraArguments		= extraArguments;
	}


}






}

