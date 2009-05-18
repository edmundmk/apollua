// Compiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate;
using Lua.Compiler.Intermediate.IR;
using Lua.Compiler.Backend;


namespace Lua.Compiler
{


public static class LuaCompiler
{


	public static Function Compile( string sourceFilePath )
	{
		StringWriter errors = new StringWriter();
		Parser parser = new Parser( errors, sourceFilePath );
		return Compile( errors, parser, Path.GetFileNameWithoutExtension( sourceFilePath ) );
	}


	public static Function Compile( string sourceName, TextReader sourceReader )
	{
		StringWriter errors	= new StringWriter();
		Parser parser = new Parser( errors, sourceName, sourceReader );
		return Compile( errors, parser, sourceName );
	}

	
	static Function Compile( StringWriter errors, Parser parser, string name )
	{
		IRCode intermediateRepresentation = (IRCode)parser.Parse( new IRCompiler() );
		intermediateRepresentation.Disassemble( Console.Out );

		AssemblyName dynamicAssemblyName = new AssemblyName( "LuaDynamic" ); 
		AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
			dynamicAssemblyName, AssemblyBuilderAccess.Save );
		ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule(
			dynamicAssemblyName.Name + ".dll" );

		CILCompiler compiler = new CILCompiler( dynamicModule );
		Type functionType = compiler.Compile( intermediateRepresentation, name );

		dynamicAssembly.Save( dynamicAssemblyName.Name + ".dll" );
		return null;
	}


}


}