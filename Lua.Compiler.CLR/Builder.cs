// Builder.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Reflection;
using System.Reflection.Emit;
using Lua.Parser.AST;


namespace Lua.Compiler.CLR
{


public class Builder
{
	public FunctionAST			Function		{ get; private set; }
	public ModuleBuilder	ModuleBuilder	{ get; private set; }
	public TypeBuilder		TypeBuilder		{ get; private set; }
	public ILGenerator		ILGenerator		{ get; private set; }

	



	

}


}