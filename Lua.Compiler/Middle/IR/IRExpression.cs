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

	public SourceLocation	Location		{ get; private set; }
	public virtual bool		IsSingleValue	{ get { return true; } }

	
	public IRExpression( SourceLocation l )
	{
		Location = l;
	}


	// Restricting multiple results to a single value.

	public virtual void RestrictToSingleValue()
	{
		// do nothing.
	}



	// Transforming so each function call is a separate statement.

	public virtual void Transform( IRCode code )
	{
		// do nothing.
	}


	public virtual void TransformAssign( IRCode code )
	{
		Transform( code );
	}


	public virtual IRExpression TransformExpression( IRCode code )
	{
		Transform( code );
		return this;
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


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Operand		= Operand.TransformExpression( code );
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


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Left		= Left.TransformExpression( code );
		Right		= Right.TransformExpression( code );
	}


}



// tonumber( operand )

sealed class ToNumberExpression
	:	IRExpression
{
	public IRExpression	Operand		{ get; private set; }


	public ToNumberExpression( SourceLocation l, IRExpression operand )
		:	base( l )
	{
		Operand		= operand;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Operand		= Operand.TransformExpression( code );
	}
}




// Values


// function() <ircode> end

sealed class FunctionLiteralExpression
	:	IRExpression
{

	public IRCode		IRCode		{ get; private set; }


	public FunctionLiteralExpression( SourceLocation l, IRCode code )
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

	public IRExpression	Table		{ get; private set; }
	public IRExpression Key			{ get; private set; }


	public IndexExpression( SourceLocation l, IRExpression table, IRExpression key )
		:	base( l )
	{
		Table		= table;
		Key			= key;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Table		= Table.TransformExpression( code );
		Key			= Key.TransformExpression( code );
	}


	public override void TransformAssign( IRCode code )
	{
		// Store operands in temporaries so that assignments can't trash them.

		IRExpression leftTemp	= new TemporaryExpression( Table.Location );
		IRExpression keyTemp	= new TemporaryExpression( Key.Location );

		leftTemp.TransformAssign( code );
		Table.Transform( code );
		code.Statement( new Assign( Location, leftTemp, Table ) );
		Table = leftTemp;

		keyTemp.TransformAssign( code );
		Key.Transform( code );
		code.Statement( new Assign( Location, keyTemp, Key ) );
		Key = keyTemp;
	}

}




// <local>

sealed class LocalExpression
	:	IRExpression
{

	public IRLocal		Local		{ get; private set; }


	public LocalExpression( SourceLocation l, IRLocal local )
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

sealed class GlobalExpression
	:	IRExpression
{

	public string		Name;


	public GlobalExpression( SourceLocation l, string name )
		:	base( l )
	{
		Name		= name;
	}

}




// Temporaries are assigned once and used once.

sealed class TemporaryExpression
	:	IRExpression
{

	public TemporaryExpression( SourceLocation l )
		:	base( l )
	{
	}

}



// Constructors exist between BeginConstructor and EndConstructor statements,
// after which they are referenced once.

sealed class ConstructorExpression
	:	IRExpression
{

	public int			ArrayCount	{ get; private set; }
	public int			HashCount	{ get; private set; }


	public ConstructorExpression( SourceLocation l )
		:	base( l )
	{
		ArrayCount	= 0;
		HashCount	= 0;
	}


	public void IncrementArrayCount()
	{
		ArrayCount += 1;
	}

	public void IncrementHashCount()
	{
		HashCount += 1;
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



// { ... }[ <index> ]


sealed class VarargElementExpression
	:	IRExpression
{

	public int			Index;

	
	public VarargElementExpression( SourceLocation l, int index )
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



abstract class MultipleResultsExpression
	:	IRExpression
{

	public override bool IsSingleValue { get { return isSingleValue; } }

	bool isSingleValue;


	public MultipleResultsExpression( SourceLocation l )
		:	base( l )
	{
		isSingleValue = false;
	}


	public override void RestrictToSingleValue()
	{
		base.RestrictToSingleValue();
		isSingleValue = true;
	}


	public virtual ExtraArguments TransformToExtraArguments()
	{
		return ExtraArguments.None;
	}


	public static void TransformExpressionList( IRCode code, IList< IRExpression> list )
	{
		// Transform all expressions in list.

		for ( int i = 0; i < list.Count; ++i )
		{
			list[ i ] = list[ i ].TransformExpression( code );
		}
	}


	public static ExtraArguments TransformLastExpression( IRCode code, IList< IRExpression > list )
	{
		// If the final expression is a multiple-result expression, use the correct
		// kind of extra arguments when compiling.

		if ( list.Count > 0 )
		{
			MultipleResultsExpression lastExpression =
				list[ list.Count - 1 ] as MultipleResultsExpression;
			if ( lastExpression != null )
			{
				return lastExpression.TransformToExtraArguments();
			}
		}

		return ExtraArguments.None;
	}

}





// ...

sealed class VarargExpression
	:	MultipleResultsExpression
{

	public VarargExpression( SourceLocation l )
		:	base( l )
	{
	}


	public override ExtraArguments TransformToExtraArguments()
	{
		if ( ! IsSingleValue )
		{
			return ExtraArguments.UseVararg;
		}
		return base.TransformToExtraArguments();
	}


}






// Function calls.


abstract class BaseCallExpression
	:	MultipleResultsExpression
{
	
	public IList< IRExpression >	Arguments			{ get; private set; }
	public ExtraArguments			ExtraArguments		{ get; private set; }


	public BaseCallExpression( SourceLocation l, IList< IRExpression > arguments )
		:	base( l )
	{
		Arguments		= arguments;
		ExtraArguments	= ExtraArguments.None;
	}


	public override void Transform( IRCode code )
	{
		base.Transform( code );

		// Transform arguments and convert final argument to extra arguments
		// if required.

		TransformExpressionList( code, Arguments );
		ExtraArguments = TransformLastExpression( code, Arguments );
		if ( ExtraArguments != ExtraArguments.None )
		{
			Arguments.RemoveAt( Arguments.Count - 1 );
		}
	}


	public override IRExpression TransformExpression( IRCode code )
	{
		base.TransformExpression( code );

		if ( IsSingleValue )
		{
			// Ensure each function call has its own statement.

			TemporaryExpression temporary = new TemporaryExpression( Location );
			code.Statement( new Assign( Location, temporary, this ) );
			return temporary;
		}
		else
		{
			// Can return multiple results, 

			code.Statement( new AssignValueList( Location, this ) );
			return this;
		}
	}


	public override ExtraArguments TransformToExtraArguments()
	{
		if ( ! IsSingleValue )
		{
			return ExtraArguments.UseValueList;
		}
		return base.TransformToExtraArguments();
	}

}
	


// <function>( <arguments> [, valuelist |, varargs ] )

sealed class CallExpression
	:	BaseCallExpression
{

	public IRExpression	Function	{ get; private set; }

	
	public CallExpression( SourceLocation l, IRExpression function, IList< IRExpression > arguments )
		:	base( l, arguments )
	{
		Function = function;
	}


	public override void Transform( IRCode code )
	{
		Function = Function.TransformExpression( code );
		base.Transform( code );
	}

}

	


// <object>.<methodname>( <object>, <arguments> [, valuelist |, varargs ] )

sealed class SelfCallExpression
	:	BaseCallExpression
{

	public IRExpression				Object				{ get; private set; }
	public SourceLocation			MethodNameLocation	{ get; private set; }
	public string					MethodName			{ get; private set; }


	public SelfCallExpression( SourceLocation l, IRExpression o,
		SourceLocation methodNameLocation, string methodName, IList< IRExpression > arguments )
		:	base( l, arguments )
	{
		Object				= o;
		MethodNameLocation	= methodNameLocation;
		MethodName			= methodName;
	}


	public override void Transform( IRCode code )
	{
		Object = Object.TransformExpression( code );
		base.Transform( code );
	}

}






}

