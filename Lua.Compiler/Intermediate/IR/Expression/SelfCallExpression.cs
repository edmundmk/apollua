// SelfCallExpression.cs
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


namespace Lua.Compiler.Intermediate.IR.Expression
{



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


	public override string ToString()
	{
		StringBuilder s = new StringBuilder();
		s.Append( Object );
		s.Append( ":" );
		s.Append( MethodName );
		s.Append( "(" );

		if ( Arguments.Count > 0 )
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
			case ExtraArguments.UseValueList:	s.Append( ", valuelist" );	break;
			case ExtraArguments.UseVararg:		s.Append( ", ..." );		break;
			}

			s.Append( " " );
		}

		s.Append( ")" );
		return s.ToString();
	}


}



}

