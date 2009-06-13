// LabelAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST
{


public class LabelAST
{
	public string Name	{ get; private set; }


	public LabelAST( string name )
	{
		Name = name;
	}

}


}