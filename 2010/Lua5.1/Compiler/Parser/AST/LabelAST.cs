// LabelAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Parser.AST.Statements;


namespace Lua.Compiler.Parser.AST
{


class LabelAST
{
	public string	Name	{ get; private set; }
	public Block	Block	{ get; private set; }


	public LabelAST( string name )
	{
		Name = name;
	}


	public void SetBlock( Block block )
	{
		Block = block;
	}

}


}