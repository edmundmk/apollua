// ReturnMultipleResults.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR.Statement.Instruction
{




// return <results> [, valuelist | varargs ]

sealed class ReturnMultipleResults
	:	IRStatement
{

	public IList< IRExpression >	Results			{ get; private set; }
	public ExtraArguments			ExtraArguments	{ get; private set; }


	public ReturnMultipleResults( SourceLocation l, IList< IRExpression > results, ExtraArguments extraArguments )
		:	base( l )
	{
		Results			= results;
		ExtraArguments	= extraArguments;
	}

}





}

