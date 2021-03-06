// IExpressionVisitor.cs
//
// Lua 5.1 is copyright � 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file � 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Parser.AST.Expressions;


namespace Lua.Compiler.Parser.AST
{


interface IExpressionVisitor
{
	void Visit( Binary e );
	void Visit( Call e );
	void Visit( CallSelf e );
	void Visit( Comparison e );
	void Visit( Concatenate e );
	void Visit( Constructor e );
	void Visit( FunctionClosure e );
	void Visit( GlobalRef e );
	void Visit( Index e );
	void Visit( Literal e );
	void Visit( LocalRef e );
	void Visit( Logical e );
	void Visit( Not e );
	void Visit( Unary e );
	void Visit( UpValRef e );
	void Visit( Vararg e );
}


}



