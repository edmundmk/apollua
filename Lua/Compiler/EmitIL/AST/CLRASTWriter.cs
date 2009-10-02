// CLRASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Parser.AST;
using Lua.CLR.Compiler.AST.Expressions;


namespace Lua.CLR.Compiler.AST
{


public class CLRASTWriter
	:	ASTWriter
	,	ICLRExpressionVisitor
{

	public CLRASTWriter( TextWriter oWriter )
		:	base( oWriter )
	{
	}


	public virtual void Visit( TemporaryRef e )
	{
		o.Write( "temporary x" );
		o.Write( e.Temporary.GetHashCode().ToString( "X" ) );
	}

	public virtual void Visit( ToNumber e )
	{
		o.Write( "__tonumber( " );
		e.Operand.Accept( this );
		o.Write( " )" );
	}

	public virtual void Visit( ValueList e )
	{
		o.Write( "__valuelist" );
	}

	public virtual void Visit( ValueListElement e )
	{
		o.Write( "__valuelist[ " );
		o.Write( e.Index );
		o.Write( " ]" );
	}

	public virtual void Visit( VarargElement e )
	{
		o.Write( "...[ " );
		o.Write( e.Index );
		o.Write( " ]" );
	}

}
	

}