// VarargElement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class VarargElement
	:	Expression
{
	public int Index { get; private set; }


	public VarargElement( SourceSpan s, int index )
		:	base( s )
	{
		Index = index;
	}

}


}