// IndexExpression.cs
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


	
// <left>[ <key> ]

sealed class IndexExpression
	:	IRExpression
{

	public IRExpression	Table		{ get; private set; }
	public IRExpression Key			{ get; private set; }


	public IndexExpression( SourceLocation l, IRExpression table, IRExpression key )
		:	base( l )
	{
		Table		= table;
		Key			= key;
	}


	public override IRExpression Transform( IRCode code )
	{
		Table	= Table.TransformSingleValue( code );
		Key		= Key.TransformSingleValue( code );
		return base.Transform( code );
	}


	public override IRExpression TransformDependentAssignment( IRCode code )
	{
		// Transform operands.

		Transform( code );


		// Store operands in temporaries so that assignments can't trash them.

		if ( !( Table is TemporaryExpression ) )
		{
			IRExpression tableTemp = new TemporaryExpression( Table.Location );
			code.Statement( new Assign( Location, tableTemp, Table ) );
			Table = tableTemp;
		}

		if ( !( Key is TemporaryExpression ) )
		{
			IRExpression keyTemp = new TemporaryExpression( Key.Location );
			code.Statement( new Assign( Location, keyTemp, Key ) );
			Key = keyTemp;
		}

		return this;
	}



	public override string ToString()
	{
		return String.Format( "{0}[ {1} ]", Table, Key );
	}

}





}

