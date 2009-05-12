// FunctionScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR.Scope
{



class FunctionScope
	:	IRScope
{
	public override bool	IsFunctionScope			{ get { return true; } }
	public override bool	IsVarargFunctionScope	{ get { return isVararg; } }

	bool isVararg;


	public FunctionScope( bool isVararg )
	{
		this.isVararg = isVararg;
	}

}



}

