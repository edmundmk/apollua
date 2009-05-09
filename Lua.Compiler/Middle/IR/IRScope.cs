// IRScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


enum ScopeKind
{
	Normal,
	Function,
	Loop,
}


sealed class IRScope
	:	Scope
{
	public ScopeKind		ScopeKind				{ get; private set; }
	public override bool	IsFunctionScope			{ get { return ScopeKind == ScopeKind.Function; } }
	public override bool	IsVarargFunctionScope	{ get { return isVararg; } }
	public override bool	IsLoopScope				{ get { return ScopeKind == ScopeKind.Loop; } }
	public string			BreakBlockName			{ get; private set; }
	public string			ContinueBlockName		{ get; private set; }

	bool isVararg;


	public IRScope()
		:	this( ScopeKind.Normal, false, null, null )
	{
	}

	public IRScope( ScopeKind kind, bool isVararg )
		:	this( kind, isVararg, null, null )
	{
	}

	public IRScope( ScopeKind kind, string breakName, string continueName )
		:	this( kind, false, breakName, continueName )
	{
	}

	public IRScope( ScopeKind kind, bool isVararg, string breakName, string continueName )
	{
		ScopeKind				= kind;
		this.isVararg			= isVararg;
		BreakBlockName			= breakName;
		ContinueBlockName		= continueName;
	}
}


}

