// VarargExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Expression
{



// ...

sealed class VarargExpression
	:	MultipleResultsExpression
{

	public VarargExpression( SourceLocation l )
		:	base( l )
	{
	}



	public override IRExpression Transform( IRCode code )
	{
		// All contexts other than multiple values, use the first variable argument.

		return new VarargElementExpression( Location, 0 );
	}
	
	public override IRExpression TransformMultipleValues( IRCode code, out ExtraArguments extraArguments )
	{
		// Single values return the first vararg.

		if ( IsSingleValue )
		{
			extraArguments = ExtraArguments.None;
			return this;
		}


		// Otherwise use the vararg array.

		extraArguments = ExtraArguments.UseVararg;
		return null;
	}




	public override ExtraArguments TransformToExtraArguments()
	{
		if ( ! IsSingleValue )
		{
			return ExtraArguments.UseVararg;
		}
		return base.TransformToExtraArguments();
	}


	public override string ToString()
	{
		return "...";
	}

}




}

