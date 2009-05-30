// LoopScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Intermediate.IR;
using Lua.Compiler.Intermediate.IR.Statement;


namespace Lua.Compiler.Intermediate.CompilerScope
{


class LoopScope
	:	IRCompilerScope
{
	public override bool	IsLoopScope				{ get { return true; } }
	public string			LoopBlockName			{ get; private set; }


	public LoopScope( string loopBlockName )
	{
		LoopBlockName = loopBlockName;
	}


	public override void Break( SourceLocation l, IRCode code )
	{
		code.Statement( new Break( l, LoopBlockName ) );
	}

	public override void Continue( SourceLocation l, IRCode code )
	{
		code.Statement( new Continue( l, LoopBlockName ) );
	}


}


}

