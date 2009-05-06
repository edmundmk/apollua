// IRCompiler.constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Front;
using Lua.Compiler.Front.AST;
using Lua.Compiler.Front.Parser;
using Lua.Compiler.Middle.IR;


namespace Lua.Compiler.Middle
{


sealed partial class IRCompiler
	:	IParserActions
{

	public Scope Constructor( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, Expression key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, int key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void LastField( SourceLocation l, Scope constructorScope, int key, Expression valuelist )
	{
		throw new NotImplementedException();
	}

	public Expression EndConstructor( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

}


}

