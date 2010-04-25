// FunctionAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lua.Compiler.Parser.AST.Statements;


namespace Lua.Compiler.Parser.AST
{


/*	An intermediate representation is built from the results of parsing.  Each
	function is represented as a list of statements, referencing a tree-based
	representation of expressions.
*/



class LuaAST
{
	// Properties.

	public string				Name					{ get; private set; }
	public LuaAST				Parent					{ get; private set; }
	public IList< LuaAST >		Functions				{ get; private set; }
	public IList< Variable >	UpVals					{ get; private set; }
	public IList< Variable >	Parameters				{ get; private set; }
	public bool					IsVararg				{ get; private set; }
	public IList< Variable >	Locals					{ get; private set; }
	public IList< Label >		Labels					{ get; private set; }
	public Block				Block					{ get; private set; }
	public bool					ReturnsMultipleValues	{ get; private set; }
	

	// Mutable collections.

	List< LuaAST >		functions;
	List< Variable >	upvals;
	List< Variable >	parameters;
	List< Variable >	locals;
	List< Label >		labels;


	// Construtor.

	public LuaAST( string name, LuaAST parent )
	{
		functions				= new List< LuaAST >();
		upvals					= new List< Variable >();
		parameters				= new List< Variable >();
		locals					= new List< Variable >();
		labels					= new List< Label >();
		
		Name					= name;
		Parent					= parent;
		Functions				= functions.AsReadOnly();
		UpVals					= upvals.AsReadOnly();
		Parameters				= parameters.AsReadOnly();
		IsVararg				= false;
		Locals					= locals.AsReadOnly();
		Labels					= labels.AsReadOnly();
		Block					= null;
		ReturnsMultipleValues	= false;
	}


	// Building interface.

	public void ChildFunction( LuaAST function )
	{
		functions.Add( function );
	}

	public void UpVal( Variable upval )
	{
		for ( LuaAST f = this; ! f.ContainsUpVal( upval ); f = f.Parent )
		{
			f.upvals.Add( upval );
		}
	}

	public void Parameter( Variable parameter )
	{
		parameters.Add( parameter );
	}

	public void SetVararg()
	{
		Debug.Assert( IsVararg == false );
		IsVararg = true;
	}

	public void Local( Variable local )
	{
		locals.Add( local );
	}

	public void Label( Label label )
	{
		labels.Add( label );
	}

	public void SetBlock( Block block )
	{
		Debug.Assert( Block == null && block != null );
		Block = block;
	}

	public void SetReturnsMultipleValues()
	{
		Debug.Assert( ReturnsMultipleValues == false );
		ReturnsMultipleValues = true;
	}

	

	// Helpers.

	bool ContainsUpVal( Variable upval )
	{
		return upvals.Contains( upval )
			|| parameters.Contains( upval )
			|| locals.Contains( upval );
	}

}


}

