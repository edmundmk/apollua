// ExpressionVisitor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


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
	public virtual void Visit( ConstructorRef e )		{}
	public virtual void Visit( FunctionClosure e )		{}
	public virtual void Visit( GlobalRef e )			{}
	public virtual void Visit( Index e )				{}
	public virtual void Visit( Literal e )				{}
	public virtual void Visit( LocalRef e )				{}
	public virtual void Visit( Logical e )				{}
	public virtual void Visit( Not e )					{}
	public virtual void Visit( Temporary e )			{}
	public virtual void Visit( ToNumber e )				{}
	public virtual void Visit( Unary e )				{}
	public virtual void Visit( UpValRef e )				{}
	public virtual void Visit( ValueList e )			{}
	public virtual void Visit( ValueListElement e )		{}
	public virtual void Visit( Vararg e )				{}
	public virtual void Visit( VarargElement e )		{}
}


}



