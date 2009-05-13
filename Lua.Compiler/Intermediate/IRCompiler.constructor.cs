// IRCompiler.constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lua.Compiler.Frontend;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;
using Lua.Compiler.Intermediate.CompilerScope;
using Lua.Compiler.Intermediate.IR;
using Lua.Compiler.Intermediate.IR.Expression;
using Lua.Compiler.Intermediate.IR.Statement;


namespace Lua.Compiler.Intermediate
{


sealed partial class IRCompiler
	:	IParserActions
{

	public Scope Constructor( SourceLocation l, Scope scope )
	{
		ConstructorExpression constructor = new ConstructorExpression( l );
		Statement( new BeginConstructor( l, constructor ) );
		return new ConstructorScope( constructor );
	}

	public void Field( SourceLocation l, Scope constructorScope, Expression key, Expression v )
	{
		ConstructorScope scope = (ConstructorScope)constructorScope;
		scope.Constructor.IncrementHashCount();

		IRExpression index = new IndexExpression( l, scope.Constructor, (IRExpression)key );
		IRExpression value = (IRExpression)v;
		
		Transform( index );
		Transform( value );
		Statement( new Assign( l, index, value ) );
	}

	public void Field( SourceLocation l, Scope constructorScope, int key, Expression v )
	{
		ConstructorScope scope = (ConstructorScope)constructorScope;
		scope.Constructor.IncrementArrayCount();

		IRExpression index = new IndexExpression( l, scope.Constructor, new LiteralExpression( l, (double)key ) );
		IRExpression value = (IRExpression)v;
		
		Transform( index );
		Transform( value );
		Statement( new Assign( l, index, value ) );
	}

	public void LastField( SourceLocation l, Scope constructorScope, int key, Expression v )
	{
		ConstructorScope scope = (ConstructorScope)constructorScope;

		MultipleResultsExpression multipleResults = v as MultipleResultsExpression;
		if ( multipleResults != null && ! multipleResults.IsSingleValue )
		{
			// Set list.

			multipleResults = (MultipleResultsExpression)multipleResults.TransformExpression( code.Peek() );
			ExtraArguments extraArguments = multipleResults.TransformToExtraArguments();
			Debug.Assert( extraArguments != ExtraArguments.None );
			Statement( new SetList( l, scope.Constructor, key, extraArguments ) );
			return;
		}

		
		// Fall back to single value.

		Field( l, constructorScope, key, v );
	}

	public Expression EndConstructor( SourceLocation l, Scope end )
	{
		ConstructorScope scope = (ConstructorScope)end;
		Statement( new EndConstructor( l ) );
		return scope.Constructor;
	}

}


}

