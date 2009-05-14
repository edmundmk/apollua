// MultipleResultsExpression.cs
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


}



}

