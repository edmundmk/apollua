// Constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST.Expressions
{


public class Constructor
	:	Expression
{
	public int ArrayCount	{ get; private set; }
	public int HashCount	{ get; private set; }


	public Constructor( SourceSpan s )
		:	base( s )
	{
		ArrayCount	= 0;
		HashCount	= 0;
	}


	public void IncrementArrayCount()
	{
		ArrayCount += 1;
	}

	public void IncrementHashCount()
	{
		HashCount += 1;
	}


	public override void Accept( ExpressionVisitor v )
	{
		v.Visit( this );
	}

}


}