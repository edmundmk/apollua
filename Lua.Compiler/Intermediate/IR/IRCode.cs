// IRCode.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR
{


sealed class IRCode
	:	Code
{
	public IRCode				Parent		{ get; private set; }
	public IList< IRCode >		Functions	{ get; private set; }

	public IList< IRLocal >		UpVals		{ get; private set; }
	public IList< IRLocal >		Parameters	{ get; private set; }
	public bool					IsVararg	{ get; private set; }
	public IList< IRLocal >		Locals		{ get; private set; }
	
	public IList< IRStatement >	Statements	{ get; private set; }


	public IRCode( IRCode parent )
	{
		Parent		= parent;
		Functions	= new List< IRCode >();

		UpVals		= new List< IRLocal >();
		Parameters	= new List< IRLocal >();
		IsVararg	= false;
		Locals		= new List< IRLocal >();
	
		Statements	= new List< IRStatement >();
	}




	// Locals.

	public void MarkUpVal( IRLocal upval )
	{
		IRCode code = this;
		while (    ! code.Parameters.Contains( upval )
			    && ! code.Locals.Contains( upval )
				&& ! code.UpVals.Contains( upval ) )
		{
			code.UpVals.Add( upval );
			code = code.Parent;
		}
	}

	public void DeclareParameter( IRLocal parameter )
	{
		Parameters.Add( parameter );
	}
		
	public void MarkVararg()
	{
		IsVararg	= true;
	}

	public void DeclareLocal( IRLocal local )
	{
		Locals.Add( local );
	}




	// Functions.

	public void ChildFunction( IRCode code )
	{
		Functions.Add( code );
	}



	// Statements.

	public void Statement( IRStatement statement )
	{
		Statements.Add( statement );
	}

	public bool EndsWithReturnStatement()
	{
		if ( Statements.Count == 0 )
		{
			return false;
		}
		else
		{
			return Statements[ Statements.Count - 1 ].IsReturnStatement;
		}
	}

	

	// Disassembly.

	public void Disassemble( TextWriter w )
	{
		w.WriteLine( "function{0:X}", GetHashCode() );

		w.WriteLine( "UpVals" );
		foreach ( IRLocal upval in UpVals )
		{
			w.WriteLine( "\t{0:X} {1}", upval.GetHashCode(), upval.Name );
		}

		w.WriteLine( "Parameters" );
		foreach ( IRLocal parameter in Parameters )
		{
			w.WriteLine( "\t{0:X} {1}", parameter.GetHashCode(), parameter.Name );
		}
		if ( IsVararg )
		{
			w.WriteLine( "\t..." );
		}

		w.WriteLine( "Locals" );
		foreach ( IRLocal local in Locals )
		{
			w.WriteLine( "\t{0:X} {1}", local.GetHashCode(), local.Name );
		}

		w.WriteLine( "Statements" );
		foreach( IRStatement statement in Statements )
		{
			w.WriteLine( "\t{0}", statement.ToString() );
		}

		w.WriteLine();

		foreach( IRCode code in Functions )
		{
			code.Disassemble( w );
		}
	}


}


}

