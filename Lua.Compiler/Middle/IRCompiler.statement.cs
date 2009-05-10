// IRCompiler.statement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front;
using Lua.Compiler.Front.AST;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Middle.IR;


namespace Lua.Compiler.Middle
{


sealed partial class IRCompiler
	:	IParserActions
{

	public void Local( SourceLocation l, Scope scope, IList< string > namelist, IList< Expression > elist )
	{
		IList< IRExpression > expressionlist = CastExpressionList( elist );
		throw new NotImplementedException();
	}

	public void Assignment( SourceLocation l, Scope scope, IList< Expression > variablelist, IList< Expression > elist )
	{
		IList< IRExpression> expressionlist = CastExpressionList( elist );
		throw new NotImplementedException();
	}

	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		IRExpression callExpression = (IRExpression)call;
		Transform( callExpression );
		Statement( new Evaluate( l, callExpression ) );
	}

	public void Break( SourceLocation l, Scope loopScope )
	{
		IRScope scope = (IRScope)loopScope;
		scope.Break( l, code.Peek() );
	}

	public void Continue( SourceLocation l, Scope loopScope )
	{
		IRScope scope = (IRScope)loopScope;
		scope.Continue( l, code.Peek() );
	}

	public void Return( SourceLocation l, Scope functionScope, IList< Expression > elist )
	{
		IList< IRExpression> expressionlist = CastExpressionList( elist );
		
		if ( expressionlist.Count == 1 )
		{
			// Return a single result or tail call.

			Transform( expressionlist[ 0 ] );
			Statement( new Return( l, expressionlist[ 0 ] ) );
		}
		else
		{
			// Return multiple results.

			IRExpression	lastExpression = expressionlist[ expressionlist.Count - 1 ];
			ExtraArguments	extraArguments = ExtraArguments.None;
			MultipleResultsExpression.TransformExpressionList( code.Peek(), ref expressionlist, ref extraArguments );
			Statement( new ReturnMultipleResults( l, expressionlist, extraArguments ) );
		}
	}

}


}

