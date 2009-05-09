// IRCompiler.statement.cs
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

	public void Local( SourceLocation l, Scope scope, IList< string > namelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void Assignment( SourceLocation l, Scope scope, IList< Expression > variablelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		throw new NotImplementedException();
	}

	public void Break( SourceLocation l, Scope loopScope )
	{
		throw new NotImplementedException();
	}

	public void Continue( SourceLocation l, Scope loopScope )
	{
	}

	public void Return( SourceLocation l, Scope functionScope, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

}


}

