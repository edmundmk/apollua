// Variable.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST
{


public class Variable
{
	public string	Name		{ get; private set; }
	public bool		IsUpVal		{ get; private set; }


	public Variable( string name )
	{
		Name	= name;
		IsUpVal	= false;
	}


	internal void SetUpVal()
	{
		IsUpVal	= true;
	}

}


}

