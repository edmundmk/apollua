// BeginEndBlock.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Statements
{


public class BeginBlock
	:	Statement
{
	public string Name { get; private set; }


	public BeginBlock( SourceSpan s, string name )
		:	base( s )
	{
		Name = name;
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


public class Break
	:	Statement
{
	public string BlockName { get; private set; }


	public Break( SourceSpan s, string blockName )
		:	base( s )
	{
		BlockName = blockName;
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


public class Continue
	:	Statement
{
	public string BlockName { get; private set; }


	public Continue( SourceSpan s, string blockName )
		:	base( s )
	{
		BlockName	= blockName;
	}


	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


public class EndBlock
	:	Statement
{
	public EndBlock( SourceSpan s )
		:	base( s )
	{
	}
	

	public override void Accept( StatementVisitor s )
	{
		s.Visit( this );
	}

}


}

