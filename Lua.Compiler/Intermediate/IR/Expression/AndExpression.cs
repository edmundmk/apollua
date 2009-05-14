// AndExpression.cs
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



// <left> and <right>

sealed class AndExpression
	:	IRExpression
{

	public IRExpression	Left		{ get; private set; }
	public IRExpression	Right		{ get; private set; }


	public AndExpression( SourceLocation l, IRExpression left, IRExpression right )
		:	base( l )
	{
		Left	= left;
		Right	= right;
	}


	public override IRExpression Transform( IRCode code )
	{
		Left	= Left.TransformSingleValue( code );
		Right	= Right.TransformSingleValue( code );
		return base.Transform( code );
	}


	public override string ToString()
	{
		return String.Format( "( {0} ) and ( {1} )", Left, Right );
	}


}







}

