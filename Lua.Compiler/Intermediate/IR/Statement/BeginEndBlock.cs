// BeginEndBlock.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Intermediate.IR.Statement
{



/*
	block <name>
	{
		break <name>		// branches to end.
		continue <name>		// branches to top.
	}
*/


sealed class BeginBlock
	:	IRStatement
{

	public string Name { get; private set; }


	public BeginBlock( SourceLocation l, string name )
		:	base( l )
	{
		Name = name;
	}


	public override string ToString()
	{
		return String.Format( "block {0} {{", Name );
	}

}


sealed class Break
	:	IRStatement
{

	public string BlockName { get; private set; }


	public Break( SourceLocation l, string blockName )
		:	base( l )
	{
		BlockName = blockName;
	}


	public override string ToString()
	{
		return String.Format( "break {0}", BlockName );
	}

}


sealed class Continue
	:	IRStatement
{

	public string BlockName { get; private set; }


	public Continue( SourceLocation l, string blockName )
		:	base( l )
	{
		BlockName	= blockName;
	}


	public override string ToString()
	{
		return String.Format( "continue {0}", BlockName );
	}

}


sealed class EndBlock
	:	IRStatement
{

	public EndBlock( SourceLocation l )
		:	base( l )
	{
	}


	public override string ToString()
	{
		return "}";
	}
	
}


}

