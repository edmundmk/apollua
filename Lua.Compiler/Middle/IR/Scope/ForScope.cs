// ForScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


class ForScope
	:	RepeatScope
{
	public IRLocal			ForIndex				{ get; private set; }
	public IRLocal			ForLimit				{ get; private set; }
	public IRLocal			ForStep					{ get; private set; }


	public ForScope( string loopBlockName, string loopBodyBlockName,
						IRLocal forIndex, IRLocal forLimit, IRLocal forStep )
		:	base( loopBlockName, loopBodyBlockName )
	{
		ForIndex	= forIndex;
		ForLimit	= forLimit;
		ForStep		= forStep;
	}


}



}

