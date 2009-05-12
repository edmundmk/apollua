// RepeatScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;
using Lua.Compiler.Middle.IR.Statement;


namespace Lua.Compiler.Middle.IR.Scope
{



class RepeatScope
	:	IRScope
{
	public override bool	IsLoopScope				{ get { return true; } }
	public string			LoopBlockName			{ get; private set; }
	public string			LoopBodyBlockName		{ get; private set; }


	public RepeatScope( string loopBlockName, string loopBodyBlockName )
	{
		LoopBlockName		= loopBlockName;
		LoopBodyBlockName	= loopBodyBlockName;
	}


	public override void Break( SourceLocation l, IRCode code )
	{
		code.Statement( new Break( l, LoopBlockName ) );
	}

	public override void Continue( SourceLocation l, IRCode code )
	{
		code.Statement( new Break( l, LoopBodyBlockName ) );
	}

}




}

