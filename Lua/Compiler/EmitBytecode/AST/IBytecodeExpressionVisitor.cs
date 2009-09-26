// IBytecodeExpresisonVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Parser.AST;
using Lua.Compiler.EmitBytecode.AST.Expressions;


namespace Lua.Compiler.EmitBytecode.AST
{


public interface IBytecodeExpressionVisitor
	:	IExpressionVisitor
{
	void Visit( OpcodeConcat e );
}


}