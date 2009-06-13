// IExpressionVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Expressions;


namespace Lua.Parser.AST
{


public interface IExpressionVisitor
{
	void Visit( Binary e );
	void Visit( Call e );
	void Visit( CallSelf e );
	void Visit( Comparison e );
	void Visit( FunctionClosure e );
	void Visit( GlobalRef e );
	void Visit( Index e );
	void Visit( Literal e );
	void Visit( LocalRef e );
	void Visit( Logical e );
	void Visit( Not e );
	void Visit( Temporary e );
	void Visit( ToNumber e );
	void Visit( Unary e );
	void Visit( UpValRef e );
	void Visit( ValueList e );
	void Visit( ValueListElement e );
	void Visit( Vararg e );
	void Visit( VarargElement e );
}


}



