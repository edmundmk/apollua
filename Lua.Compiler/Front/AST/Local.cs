// Local.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Compiler.Front.AST
{


/*	This class represents a local variable declaration.  It contains enough
	information to allow name lookup to be performed in the frontend.
*/


abstract class Local
{
	public string Name { get; private set; }

	public Local( string name )
	{
		Name = name;
	}

}


}

