// SetList.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{



// <table>[ <startindex> ... ] = valuelist | varargs

sealed class SetList
	:	IRStatement
{

	public IRExpression				Table			{ get; private set; }
	public int						Index			{ get; private set; }
	public ExtraArguments			ExtraArguments	{ get; private set; }


	public SetList( SourceLocation l, IRExpression table, int index, ExtraArguments extraArguments )
		:	base( l )
	{
		Debug.Assert( extraArguments != ExtraArguments.None );

		Table			= table;
		Index			= index;
		ExtraArguments	= extraArguments;
	}


	public override string ToString()
	{
		return String.Format( "setlist {0}[ {1} ... ] = {2}",
			Table, Index, ExtraArguments == ExtraArguments.UseValueList ? "valuelist" : "..." );
	}

}




}

