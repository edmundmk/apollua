// ReturnMultipleResults.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Text;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{




// return <results> [, valuelist | varargs ]

sealed class ReturnMultipleResults
	:	IRStatement
{

	public override bool			IsReturnStatement	{ get { return true; } }
	public IList< IRExpression >	Results				{ get; private set; }
	public ExtraArguments			ExtraArguments		{ get; private set; }


	public ReturnMultipleResults( SourceLocation l, IList< IRExpression > results, ExtraArguments extraArguments )
		:	base( l )
	{
		Results			= results;
		ExtraArguments	= extraArguments;
	}


	public override string ToString()
	{
		StringBuilder s = new StringBuilder();
		s.Append( "return " );
		
		bool isFirst = true;
		foreach ( IRExpression result in Results )
		{
			if ( ! isFirst )
				s.Append( ", " );
			isFirst = false;
			s.Append( result );
		}

		switch ( ExtraArguments )
		{
		case ExtraArguments.UseValueList:	s.Append( ", valuelist" );	break;
		case ExtraArguments.UseVararg:		s.Append( ", ..." );		break;
		}

		return s.ToString();
	}


}





}

