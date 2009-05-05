// CompileAST.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;


namespace Lua.Compiler.AST
{


/*	Each function is compiled to a class deriving from Lua.Function.
	
	We implement continuations using the technique described in
	http://www.ccs.neu.edu/scheme/pubs/stackhack4.html.  When yielding a coroutine,
	we do not use exceptions because they are slow and impose constraints on the
	generated code.  Instead yielding functions return StackFrame values, which are
	explicitly checked for.

	The general structure of a compiled function class is:


	class <name>
		:	Lua.Function
	{
		// All constants are initialized only once, when the function is loaded.

		[ const Value constant = <constant>; ]*
	


		// Upval references are set up when the function value is created, and are
		// inherited from the enclosing function.
		
		[ UpVal upval; ]*

		public .ctor( [ UpVal upval, ]* )
		{
			[ this.upval = upval; ]*
		}



		// Only one version of each function is compiled.  This is simpler than LuaCLR as
		// described in http://portal.acm.org/citation.cfm?doid=1363686.1363743, which
		// compiles both a multi-return and a single-return version of each function.
		// Instead any function that can potentially return multiple values is compiled
		// as a multi-return function.

		// Function parameters are declared exactly as in the Lua declaration, though each
		// function must declare at least one parameter so that we can check for StackFrames.

		[ Value | Value[] ] Invoke( Value argument0, [ Value argument, ]* [ params Value[] vararg ]? )
		{
			// If the first argument is a StackFrame, we are resuming a suspended continuation.

			if ( argument0 != null && argument0.GetType() == typeof( StackFrame ) )
			{
				goto resume;
			}


			[ Compiled lua code goes here. ]

	
			// Values that are used as upvals must be declared as upvals everywhere they are used.

			Value local0;
			UpVal local1;
	

			// Operations are compiled:

			Value result = [ left ].[ Op ]( right );
	

			// Functions are compiled:

			Value result = [ function ].InvokeS( [ argument, ]* );
		continuation0:
			if ( result != null && result.GetType() == typeof( StackFrame ) )
			{
				goto yield; ( result, 0 )
			}


			// Or for multi-return:

			Value[] results = [ function ].InvokeM( [ argument, ]* )
		continuation1:
			if ( results.Length > 0 && results[ 0 ].GetType() == typeof( StackFrame ) )
			{
				goto yield; ( results[ 0 ], 1 )
			}
	
	
			// Saves the stack frame and indicates that the calling function should also yield.

		yield: ( stackFrame, continuation )
			stackFrame = new StackFrame( stackFrame, continuation );
			[ All arguments and locals are stored into stackFrame. ]
			[ return stackFrame; | return new Value[]{ stackFrame }; ]
	
				
			// Restores the stack frame and continues from where we left off.

		resume: ( stackFrame )
			int continuation = stackFrame.Continuation;
			[ Restore all arguments and locals. ]
			stackFrame = stackFrame.Next;

			switch ( continuation )
			{
			case 0: result = [ function ].ResumeS( stackFrame ); goto continuation0;
			case 1: results = [ function ].ResumeM( stackFrame ); goto continuation1;
			}

			throw new InvalidContinuationException();
			
		}



		// All the various overloads of Invoke are emitted to forward the request
		// directly to the generated function.

		public override Value InvokeS( Value argument )
		{
			[ Marshal parameters and/or return values while calling Invoke ].
		}


	}

	
*/




sealed class CompileAST
	:	Frontend.IParserActions
{
	public Scope Function( SourceLocation l, Scope scope, IList< string > parameternamelist, bool isVararg )
	{
		throw new NotImplementedException();
	}

	public Code EndFunction( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope Do( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void EndDo( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope If( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope ElseIf( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope Else( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void EndIf( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope While( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public void EndWhile( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope Repeat( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void Until( SourceLocation l, Scope scope, Expression condition )
	{
		throw new NotImplementedException();
	}

	public Scope For( SourceLocation l, Scope scope, string varname, Expression start, Expression limit, Expression step )
	{
		throw new NotImplementedException();
	}

	public void EndFor( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public Scope ForIn( SourceLocation l, Scope scope, IList< string > variablenamelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void EndForIn( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}

	public void Local( SourceLocation l, Scope scope, IList< string > namelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void Assignment( SourceLocation l, Scope scope, IList< Expression > variablelist, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public void CallStatement( SourceLocation l, Scope scope, Expression call )
	{
		throw new NotImplementedException();
	}

	public void Break( SourceLocation l, Scope loopScope )
	{
		throw new NotImplementedException();
	}

	public void Return( SourceLocation l, Scope functionScope, IList< Expression > expressionlist )
	{
		throw new NotImplementedException();
	}

	public Expression UnaryExpression( SourceLocation l, Expression operand, TokenKind op )
	{
		throw new NotImplementedException();
	}

	public Expression BinaryExpression( SourceLocation l, Expression left, Expression right, TokenKind op )
	{
		throw new NotImplementedException();
	}

	public Expression FunctionExpression( SourceLocation l, Code objectCode )
	{
		throw new NotImplementedException();
	}

	public Expression LiteralExpression( SourceLocation l, object value )
	{
		throw new NotImplementedException();
	}

	public Expression VarargsExpression( SourceLocation l, Scope functionScope )
	{
		throw new NotImplementedException();
	}

	public Expression LookupExpression( SourceLocation l, Expression left, Expression key )
	{
		throw new NotImplementedException();
	}

	public Expression CallExpression( SourceLocation l, Expression left, IList< Expression > argumentlist )
	{
		throw new NotImplementedException();
	}

	public Expression SelfCallExpression( SourceLocation l, Expression left, SourceLocation keyl, string key, IList< Expression > argumentlist )
	{
		throw new NotImplementedException();
	}

	public Expression NestedExpression( SourceLocation l, Expression expression )
	{
		throw new NotImplementedException();
	}

	public Expression LocalVariableExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		throw new NotImplementedException();
	}

	public Expression UpValExpression( SourceLocation l, Scope lookupScope, Local local )
	{
		throw new NotImplementedException();
	}

	public Expression GlobalVariableExpression( SourceLocation l, string name )
	{
		throw new NotImplementedException();
	}

	public Scope Constructor( SourceLocation l, Scope scope )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, Expression key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void Field( SourceLocation l, Scope constructorScope, int key, Expression value )
	{
		throw new NotImplementedException();
	}

	public void LastField( SourceLocation l, Scope constructorScope, int key, Expression valuelist )
	{
		throw new NotImplementedException();
	}

	public Expression EndConstructor( SourceLocation l, Scope end )
	{
		throw new NotImplementedException();
	}
}


}

