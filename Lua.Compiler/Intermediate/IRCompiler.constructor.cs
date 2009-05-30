// IRCompiler.constructor.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


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
		
		TransformIndependentAssignment( ref index );
		TransformAssignmentValue( index, ref value );
		Statement( new Assign( l, index, value ) );
	}

	public void Field( SourceLocation l, Scope constructorScope, int key, Expression v )
	{
		ConstructorScope scope = (ConstructorScope)constructorScope;
		scope.Constructor.IncrementArrayCount();

		IRExpression index = new IndexExpression( l, scope.Constructor, new LiteralExpression( l, (double)key ) );
		IRExpression value = (IRExpression)v;
		
		TransformIndependentAssignment( ref index );
		TransformAssignmentValue( index, ref value );
		Statement( new Assign( l, index, value ) );
	}

	public void LastField( SourceLocation l, Scope constructorScope, int key, Expression v )
	{
		ConstructorScope scope = (ConstructorScope)constructorScope;


		// Try multiple values.

		IRExpression value = (IRExpression)v;
		ExtraArguments extraArguments;
		TransformMultipleValues( ref value, out extraArguments );


		if ( extraArguments == ExtraArguments.None )
		{
			// Fall back to single value.

			Field( l, constructorScope, key, v );
		}
		else
		{
			// Set list.

			Statement( new SetList( l, scope.Constructor, key, extraArguments ) );
		}

	}

	public Expression EndConstructor( SourceLocation l, Scope end )
	{
		ConstructorScope scope = (ConstructorScope)end;
		Statement( new EndConstructor( l ) );
		return scope.Constructor;
	}

}


}

