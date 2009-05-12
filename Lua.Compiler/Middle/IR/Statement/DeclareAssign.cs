// DeclareAssign.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR.Statement
{


// declare <local> = <expression>

sealed class DeclareAssign
	:	IRStatement
{

	public IRLocal					Local			{ get; private set; }
	public IRExpression				Expression		{ get; private set; }


	public DeclareAssign( SourceLocation l, IRLocal local, IRExpression expression )
		:	base( l )
	{
		Local			= local;
		Expression		= expression;
	}

}




}

