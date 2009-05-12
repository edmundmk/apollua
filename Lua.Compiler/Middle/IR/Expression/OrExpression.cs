// OrExpression.cs
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



// <left> and <right>

sealed class OrExpression
	:	IRExpression
{

	public IRExpression	Left		{ get; private set; }
	public IRExpression	Right		{ get; private set; }


	public OrExpression( SourceLocation l, IRExpression left, IRExpression right )
		:	base( l )
	{
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

