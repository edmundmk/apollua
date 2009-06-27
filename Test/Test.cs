// Test
//
// Copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lua;
using Lua.Parser;
using Lua.Parser.AST;
//using Lua.CLR.Compiler;
//using Lua.CLR.Compiler.AST;
using Lua.VM;
using Lua.VM.Compiler;
//using Lua.VM.Compiler.AST;


namespace Test
{


static class EntryPoint
{

	static int Main( string[] args )
	{
		Console.Out.WriteLine( args[ 0 ] );
		Console.Error.WriteLine( args[ 0 ] );

		
		LuaVMCompiler compiler = new LuaVMCompiler( Console.Error, File.OpenText( args[ 0 ] ), args[ 0 ] );
		VMFunction function = compiler.Compile();
		PrototypeWriter.Write( Console.Out, function.Prototype );



/*
		LuaParser parser = new LuaParser( Console.Error, File.OpenText( args[ 0 ] ), args[ 0 ] );
		FunctionAST function = parser.Parse();
//		ASTWriter writer = new ASTWriter( Console.Out );
//		writer.Write( function );


		CLRTransform transform = new CLRTransform();
		function = transform.Transform( function );
//		CLRASTWriter clrWriter = new CLRASTWriter( Console.Out );
//		clrWriter.Write( function );

		ANormalTransform anormal = new ANormalTransform();
		function = anormal.Transform( function );
//		clrWriter.Write( function );

		}

		timing.Stop();
		Console.Error.WriteLine( timing.ElapsedMilliseconds / 100 );

//		BytecodeTransform transform = new BytecodeTransform();
//		function = transform.Transform( function );
//		writer.Write( function );


	
/*		
		AssemblyBuilder	a = AppDomain.CurrentDomain.DefineDynamicAssembly( new AssemblyName( "TestCompiled" ), AssemblyBuilderAccess.Save );
		ModuleBuilder	m = a.DefineDynamicModule( "TestCompiled", "TestCompiled.dll", true );
		
		StringWriter	errors		= new StringWriter();
		LuaCLRCompiler	compiler	= new LuaCLRCompiler( m, errors, File.OpenText( args[ 0 ] ), args[ 0 ] );
		Function		function	= compiler.Compile();

		a.Save( "TestCompiled.dll" );
*/
		return 0;
	}


	static void TestTable()
	{
		// Excercise the table.
		
		Table table = new Table();

		for ( int i = 5; i >= 1; --i )
		{
			table.NewIndex( i, true );
		}

		for ( int i = -2; i <= 8; ++i )
		{
			Console.WriteLine( "{0} {1}", i, table[ i ] );
		}

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Value, Value > item in table )
		{
			Console.WriteLine( "{0} {1}", item.Key, item.Value );
		}


		Value key	= null;
		Value value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "{0} {1}", key, value );

			table.Next( ref key, out value );
		}


		table[ 3 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );


		table[ "Hello" ] = "World";


		table[ 3 ] = 42;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 3 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 4 ] = null;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 3 ] = 67.432;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 4 ] = false;

		Console.WriteLine( "#table = {0}", table.Length() );

		table[ 0 ] = "AHAHAAH";

		Console.WriteLine( "#table = {0}", table.Length() );

		foreach ( KeyValuePair< Value, Value > item in table )
		{
			Console.WriteLine( "{0} {1}", item.Key, item.Value );
		}


		key		= null;
		value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "{0} {1}", key, value );
			table[ key ] = null;

			table.Next( ref key, out value );
		}


		foreach ( KeyValuePair< Value, Value > item in table )
		{
			Console.WriteLine( "ELEMENT: {0} {1}", item.Key, item.Value );
		}

		key		= null;
		value	= null;

		table.Next( ref key, out value );
		while ( key != null )
		{
			Console.WriteLine( "NEXT: {0} {1}", key, value );
			table[ key ] = null;

			table.Next( ref key, out value );
		}

	}

}


}
