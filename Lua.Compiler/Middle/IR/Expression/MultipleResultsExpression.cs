// MultipleResultsExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR.Expression
{



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



}

