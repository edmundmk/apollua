// BaseCallExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Intermediate.IR.Statement;


namespace Lua.Compiler.Intermediate.IR.Expression
{


	
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
	




}

