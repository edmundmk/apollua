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


	public override void Transform( IRCode code )
	{
		base.Transform( code );
		Table		= Table.TransformExpression( code );
		Key			= Key.TransformExpression( code );
	}


	public override void TransformAssign( IRCode code )
	{
		// Store operands in temporaries so that assignments can't trash them.

		IRExpression leftTemp	= new TemporaryExpression( Table.Location );
		IRExpression keyTemp	= new TemporaryExpression( Key.Location );

		leftTemp.TransformAssign( code );
		Table.Transform( code );
		code.Statement( new Assign( Location, leftTemp, Table ) );
		Table = leftTemp;

		keyTemp.TransformAssign( code );
		Key.Transform( code );
		code.Statement( new Assign( Location, keyTemp, Key ) );
		Key = keyTemp;
	}
	

	public override string ToString()
	{
		return String.Format( "{0}[ {1} ]", Table, Key );
	}

}





}

