// ExpressionVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST.Expressions;


namespace Lua.Parser.AST
{


public abstract class ExpressionVisitor
{
	public virtual void Visit( Binary e )				{}
	public virtual void Visit( Call e )					{}
	public virtual void Visit( CallSelf e )				{}
	public virtual void Visit( Comparison e )			{}
	public virtual void Visit( Constructor e )			{}
	public virtual void Visit( FunctionClosure e )		{}
	public virtual void Visit( Global e )				{}
	public virtual void Visit( Index e )				{}
	public virtual void Visit( Literal e )				{}
	public virtual void Visit( Local e )				{}
	public virtual void Visit( Logical e )				{}
	public virtual void Visit( Not e )					{}
	public virtual void Visit( Temporary e )			{}
	public virtual void Visit( ToNumber e )				{}
	public virtual void Visit( Unary e )				{}
	public virtual void Visit( UpVal e )				{}
	public virtual void Visit( ValueList e )			{}
	public virtual void Visit( ValueListElement e )		{}
	public virtual void Visit( Vararg e )				{}
	public virtual void Visit( VarargElement e )		{}
}


}



