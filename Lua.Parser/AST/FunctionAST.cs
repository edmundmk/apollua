// FunctionAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


/*	An intermediate representation is built from the results of parsing.  Each
	function is represented as a list of statements, referencing a tree-based
	representation of expressions.

	All control structures map to configurations of two structural statements:
	blocks (which be continued to emulate loops or broken to skip unnecessary
	clauses) and tests (which are like a single if statement).
	
	Temporaries are inserted into expressions in order that the details of
	multiple assignments and multiple returns are transparent to the consuming
	code.
*/



public class FunctionAST
{
	// Properties.

	public string				Name					{ get; private set; }
	public FunctionAST			Parent					{ get; private set; }
	public IList< FunctionAST >	Functions				{ get; private set; }
	public IList< Variable >	UpVals					{ get; private set; }
	public IList< Variable >	Parameters				{ get; private set; }
	public bool					IsVararg				{ get; private set; }
	public IList< Variable >	Locals					{ get; private set; }
	public IList< Constructor >	Constructors			{ get; private set; }
	public IList< LabelAST >	Labels					{ get; private set; }
	public Block				Block					{ get; private set; }
	public bool					ReturnsMultipleValues	{ get; private set; }
	

	// Mutable collections.

	List< FunctionAST >	functions;
	List< Variable >	upvals;
	List< Variable >	parameters;
	List< Variable >	locals;
	List< LabelAST >	labels;


	// Construtor.

	public FunctionAST( string name, FunctionAST parent, Block block )
	{
		functions				= new List< FunctionAST >();
		upvals					= new List< Variable >();
		parameters				= new List< Variable >();
		locals					= new List< Variable >();
		labels					= new List< LabelAST >();
		
		Name					= name;
		Parent					= parent;
		Functions				= functions.AsReadOnly();
		UpVals					= upvals.AsReadOnly();
		Parameters				= parameters.AsReadOnly();
		IsVararg				= false;
		Locals					= locals.AsReadOnly();
		Labels					= labels.AsReadOnly();
		Block					= block;
		ReturnsMultipleValues	= false;
	}


	// Building interface.

	public void ChildFunction( FunctionAST function )
	{
		functions.Add( function );
	}

	bool ContainsUpVal( Variable upval )
	{
		return upvals.Contains( upval )
			|| parameters.Contains( upval )
			|| locals.Contains( upval );
	}

	public void UpVal( Variable upval )
	{
		for ( FunctionAST f = this; ! f.ContainsUpVal( upval ); f = f.Parent )
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
		IsVararg = true;
	}

	public void Local( Variable local )
	{
		locals.Add( local );
	}

	public void Label( LabelAST label )
	{
		labels.Add( label );
	}

	public void SetReturnsMultipleValues()
	{
		ReturnsMultipleValues = true;
	}
	
}


}

