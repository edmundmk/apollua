// IRCode.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Middle.IR
{


sealed class IRCode
	:	Code
{
	public IRCode				Parent		{ get; private set; }
	public IList< IRCode >		Children	{ get; private set; }

	public IList< IRLocal >		UpVals		{ get; private set; }
	public IList< IRLocal >		Parameters	{ get; private set; }
	public bool					IsVararg	{ get; private set; }
	public IList< IRLocal >		Locals		{ get; private set; }
	
	public IList< IRStatement >	Statements	{ get; private set; }


	public IRCode( IRCode parent )
	{
		Parent		= parent;
		Children	= new List< IRCode >();

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
		while ( ! code.Parameters.Contains( upval ) && ! code.Locals.Contains( upval ) )
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
		Children.Add( code );
	}



	// Statements.

	public void Statement( IRStatement statement )
	{
		Statements.Add( statement );
	}

	

	// Disassembly.

	public void Disassemble( TextWriter w )
	{
	}


}


}

