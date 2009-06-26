// ICLRExpressionVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;
using Lua.CLR.Compiler.AST.Expressions;


namespace Lua.CLR.Compiler.AST
{


public interface ICLRExpressionVisitor
	:	IExpressionVisitor
{
	void Visit( TemporaryRef e );
	void Visit( ToNumber e );
	void Visit( ValueList e );
	void Visit( ValueListElement e );
	void Visit( VarargElement e );
}


}