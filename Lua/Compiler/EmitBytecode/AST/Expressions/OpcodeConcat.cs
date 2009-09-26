// OpcodeConcat.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Bytecode;
using Lua.Compiler.Parser.AST;


namespace Lua.VM.Compiler.AST.Expressions
{


public class OpcodeConcat
	:	Expression
{
	public IList< Expression > Operands { get; private set; }
	

	public OpcodeConcat( SourceSpan s, IList< Expression > operands )
		:	base( s )
	{
		Operands = operands;
	}


	public override void Accept( IExpressionVisitor v )
	{
		if ( v is IVMExpressionVisitor )
		{
			( (IVMExpressionVisitor)v ).Visit( this );
		}
	}

}


}
