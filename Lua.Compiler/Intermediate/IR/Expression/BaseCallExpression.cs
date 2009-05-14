// BaseCallExpression.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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


	public override IRExpression Transform( IRCode code )
	{
		// Transform arguments.

		for ( int argument = 0; argument < Arguments.Count - 1; ++argument )
		{
			Arguments[ argument ] = Arguments[ argument ].TransformSingleValue( code );
		}


		// Last argument can return multiple results.

		if ( Arguments.Count > 0 )
		{
			int last = Arguments.Count - 1;
			ExtraArguments extraArguments;
			Arguments[ last ] = Arguments[ last ].TransformMultipleValues( code, out extraArguments );
			if ( extraArguments != ExtraArguments.None )
			{
				ExtraArguments = extraArguments;
				Arguments.RemoveAt( last );
			}
		}


		// Leave function.

		return base.Transform( code );
	}


	public override IRExpression TransformSingleValue( IRCode code )
	{
		// Transform arguments.

		Transform( code );


		// Function results must be the only thing on the stack.

		TemporaryExpression temporary = new TemporaryExpression( Location );
		code.Statement( new Assign( Location, temporary, this ) );
		return temporary;
	}


	public override IRExpression TransformMultipleValues( IRCode code, out ExtraArguments extraArguments )
	{
		// If we have been restricted to a single result, transform it.

		if ( IsSingleValue )
		{
			extraArguments = ExtraArguments.None;
			return this;
		}

		
		// Transform arguments.

		Transform( code );

		
		// Evaluate and return value list.

		code.Statement( new AssignValueList( Location, this ) );
		extraArguments = ExtraArguments.UseValueList;
		return null;
	}




	protected string ArgumentsToString()
	{
		StringBuilder s = new StringBuilder();
		s.Append( "(" );

		if ( Arguments.Count > 0 || ExtraArguments != ExtraArguments.None )
		{
			s.Append( " " );

			bool isFirst = true;
			foreach ( IRExpression argument in Arguments )
			{
				if ( ! isFirst )
					s.Append( ", " );
				isFirst = false;
				s.Append( argument );
			}

			switch ( ExtraArguments )
			{
			case ExtraArguments.UseValueList:
				if ( ! isFirst )
					s.Append( ", " );
				s.Append( "valuelist" );
				break;

			case ExtraArguments.UseVararg:
				if ( ! isFirst )
					s.Append( ", " );
				s.Append( "..." );
				break;
			}

			s.Append( " " );
		}

		s.Append( ")" );
		return s.ToString();
	}

}
	




}

