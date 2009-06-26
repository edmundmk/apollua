// VMASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.VM.Compiler.AST.Expressions;


namespace Lua.VM.Compiler.AST
{


public class VMASTWriter
	:	ASTWriter
	,	IVMExpressionVisitor
{

	public VMASTWriter( TextWriter oWriter )
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
