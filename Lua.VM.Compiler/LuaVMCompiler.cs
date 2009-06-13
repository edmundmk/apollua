// LuaCLRCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using Lua;
using Lua.VM;
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.VM.Compiler
{


/*	Each function is compiled to a class deriving from Lua.Function.
*/


public class LuaVMCompiler
	:	IStatementVisitor
	,	IExpressionVisitor
{
	// Errors.

	TextWriter				errorWriter;
	bool					hasError;


	// Parser.

	string					sourceName;
	LuaParser				parser;


	// Prototype building.

	Builder					builder;



	public LuaVMCompiler( TextWriter errorWriter, TextReader source, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceName		= sourceName;
		parser				= new LuaParser( errorWriter, source, sourceName );
	}

	public bool HasError
	{
		get { return hasError || parser.HasError; }
	}


	public Function Compile()
	{
		// Parse the function.

		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}
		

		return null;
	}



	class Builder
	{
		public Builder Parent		{ get; private set; }


	}


	
	public void Visit( Assign s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Block s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Branch s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Constructor s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Declare s )
	{
		throw new NotImplementedException();
	}

	public void Visit( DeclareForIndex s )
	{
		throw new NotImplementedException();
	}
	
	public void Visit( Evaluate s )
	{
		throw new NotImplementedException();
	}

	public void Visit( IndexMultipleValues s )
	{
		throw new NotImplementedException();
	}

	public void Visit( MarkLabel s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeForLoop s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeForPrep s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeSetList s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeTForLoop s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Return s )
	{
		throw new NotImplementedException();
	}

	public void Visit( ReturnMultipleValues s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Test s )
	{
		throw new NotImplementedException();
	}




	public void Visit( Binary e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Call e )
	{
		throw new NotImplementedException();
	}

	public void Visit( CallSelf e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Comparison e )
	{
		throw new NotImplementedException();
	}

	public void Visit( FunctionClosure e )
	{
		throw new NotImplementedException();
	}

	public void Visit( GlobalRef e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Index e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Literal e )
	{
		throw new NotImplementedException();
	}

	public void Visit( LocalRef e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Logical e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Not e )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeConcat e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Temporary e )
	{
		throw new NotImplementedException();
	}

	public void Visit( ToNumber e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Unary e )
	{
		throw new NotImplementedException();
	}

	public void Visit( UpValRef e )
	{
		throw new NotImplementedException();
	}

	public void Visit( ValueList e )
	{
		throw new NotImplementedException();
	}

	public void Visit( ValueListElement e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Vararg e )
	{
		throw new NotImplementedException();
	}

	public void Visit( VarargElement e )
	{
		throw new NotImplementedException();
	}
}


}
