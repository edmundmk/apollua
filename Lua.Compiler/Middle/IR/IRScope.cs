// IRScope.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{



abstract class IRScope
	:	Scope
{
	public virtual bool IsIfScope { get { return false; } }


	public void Declare( IRLocal local )
	{
		Locals.Add( local );
	}


	public virtual void Break( SourceLocation l, IRCode code )
	{
		throw new InvalidOperationException();
	}

	public virtual void Continue( SourceLocation l, IRCode code )
	{
		throw new InvalidOperationException();
	}

}



class DoScope
	:	IRScope
{
}



class IfScope
	:	IRScope
{
	public override bool IsIfScope { get { return true; } }
}



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



class LoopScope
	:	IRScope
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



class ConstructorScope
	:	IRScope
{

	public ConstructorExpression Constructor	{ get; private set; }


	public ConstructorScope( ConstructorExpression constructor )
	{
		Constructor	= constructor;
	}

}



}

