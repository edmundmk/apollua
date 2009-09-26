// BytecodeASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Parser.AST;
using Lua.Compiler.Parser.AST.Expressions;
using Lua.Compiler.EmitBytecode.AST.Expressions;


namespace Lua.Compiler.EmitBytecode.AST
{


public class BytecodeASTWriter
	:	ASTWriter
	,	IBytecodeExpressionVisitor
{

	public BytecodeASTWriter( TextWriter oWriter )
		:	base( oWriter )
	{
	}

	
	public virtual void Visit( OpcodeConcat e )
	{
		o.Write( "concat " );
		bool bFirst = true;
		foreach ( Expression operand in e.Operands )
		{
			if ( ! bFirst )
				o.Write( " .. " );
			bFirst = false;
			operand.Accept( this );
		}
	}

}


}
