// Function.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua.Parser.AST
{


public class Function
{
	// Properties.

	public string				Name			{ get; private set; }
	public Function				Parent			{ get; private set; }
	public IList< Function >	Functions		{ get; private set; }
	public IList< Variable >	UpVals			{ get; private set; }
	public IList< Variable >	Parameters		{ get; private set; }
	public bool					IsVararg		{ get; private set; }
	public IList< Variable >	Locals			{ get; private set; }
	public IList< Statement >	Statements		{ get; private set; }
	

	// Mutable collections.

	List< Function >	functions;
	List< Variable >	upvals;
	List< Variable >	parameters;
	List< Variable >	locals;
	List< Statement >	statements;


	// Construtor.

	public Function( string name, Function parent )
	{
		functions	= new List< Function >();
		upvals		= new List< Variable >();
		parameters	= new List< Variable >();
		locals		= new List< Variable >();
		statements	= new List< Statement >();
		
		Name		= name;
		Parent		= parent;
		Functions	= functions.AsReadOnly();
		UpVals		= upvals.AsReadOnly();
		Parameters	= parameters.AsReadOnly();
		IsVararg	= false;
		Locals		= locals.AsReadOnly();
		Statements	= statements.AsReadOnly();
	}


	// Building interface.

	public void ChildFunction( Function function )
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
		for ( Function f = this; ! f.ContainsUpVal( upval ); f = f.Parent )
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


	public void Statement( Statement statement )
	{
		statements.Add( statement );
	}
	
}


}

