﻿// FunctionTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Compiler.Parser.AST;
using Lua.Compiler.Parser.AST.Expressions;
using Lua.Compiler.Parser.AST.Statements;


namespace Lua.Compiler.Parser.AST
{


abstract class FunctionTransform
	:	IStatementVisitor
	,	IExpressionVisitor
{
	protected LuaAST	function;
	protected Block		block;
	protected object	result;


	public FunctionTransform()
	{
		function	= null;
		block		= null;
	}


	public virtual LuaAST Transform( LuaAST f )
	{
		function = new LuaAST( f.Name, function );

		// Upvals.
		foreach ( Variable upval in f.UpVals )
		{
			function.UpVal( upval );
		}

		// Parameters.
		foreach ( Variable parameter in f.Parameters )
		{
			function.Parameter( parameter );
		}
		if ( f.IsVararg )
		{
			function.SetVararg();
		}

		// Locals.
		foreach ( Variable local in f.Locals )
		{
			function.Local( local );
		}

		// Labels.
		foreach ( Label label in f.Labels )
		{
			function.Label( label );
		}

		// Block.
		function.SetBlock( (Block)Transform( f.Block ) );	

		// Multiple value return.
		if ( f.ReturnsMultipleValues )
		{
			function.SetReturnsMultipleValues();
		}


		// Return.
		f = function;
		function = function.Parent;
		return f;
	}


	public virtual Statement Transform( Statement s )
	{
		s.Accept( this );
		s = (Statement)result;
		result = null;
		return s;
	}

	public virtual Expression Transform( Expression e )
	{
		e.Accept( this );
		e = (Expression)result;
		result = null;
		return e;
	}


	public virtual void Visit( Assign s )
	{
		result = new Assign( s.SourceSpan, Transform( s.Target ), Transform( s.Value ) );
	}

	public virtual void Visit( AssignList s )
	{
		Expression[] targets = new Expression[ s.Targets.Count ];
		for ( int i = 0; i < s.Targets.Count; ++i )
		{
			targets[ i ] = Transform( s.Targets[ i ] );
		}
		Expression[] values = new Expression[ s.Values.Count ];
		for ( int i = 0; i < s.Values.Count; ++i )
		{
			values[ i ] = Transform( s.Values[ i ] );
		}
		Expression valueList = s.ValueList != null ? Transform( s.ValueList ) : null;
		result = new AssignList( s.SourceSpan, Array.AsReadOnly( targets ), Array.AsReadOnly( values ), valueList );
	}

	public virtual void Visit( Block s )
	{
		block = new Block( s.SourceSpan, block, s.Name );
		TransformBlock( s );
		result = block;
		block = block.Parent;
	}

	protected void TransformBlock( Block s )
	{
		// Locals
		foreach ( Variable local in s.Locals )
		{
			block.Local( local );
		}

		// Statements.
		foreach ( Statement statement in s.Statements )
		{
			Statement transformed = Transform( statement );
			if ( transformed != null )
			{
				block.Statement( transformed );
			}
		}
	}

	public virtual void Visit( Branch s )
	{
		result = s;
	}

	public virtual void Visit( Declare s )
	{
		result = new Declare( s.SourceSpan, s.Variable, Transform( s.Value ) );
	}

	public virtual void Visit( DeclareList s )
	{
		Expression valueList = s.ValueList != null ? Transform( s.ValueList ) : null;
		result = new DeclareList( s.SourceSpan, s.Variables, valueList );
	}

	public virtual void Visit( Evaluate s )
	{
		result = new Evaluate( s.SourceSpan, Transform( s.Expression ) );
	}

	public virtual void Visit( ForBlock s )
	{
		block = new ForBlock( s.SourceSpan, block, s.Name,
			s.Index, s.Limit, s.Step, s.UserIndex, s.BreakLabel, s.ContinueLabel );
		TransformBlock( s );
		result = block;
		block = block.Parent;
	}

	public virtual void Visit( ForListBlock s )
	{
		block = new ForListBlock( s.SourceSpan, block, s.Name,
			s.Generator, s.State, s.Control, s.UserVariables,
			s.BreakLabel, s.ContinueLabel );
		TransformBlock( s );
		result = block;
		block = block.Parent;
	}

	public virtual void Visit( MarkLabel s )
	{
		result = s;
	}

	public virtual void Visit( Return s )
	{
		result = new Return( s.SourceSpan, Transform( s.Result ) );
	}

	public virtual void Visit( ReturnList s )
	{
		Expression[] results = new Expression[ s.Results.Count ];
		for ( int i = 0; i < s.Results.Count; ++i )
		{
			results[ i ] = Transform( s.Results[ i ] );
		}
		Expression resultValues = s.ResultList != null ? Transform( s.ResultList ) : null;
		result = new ReturnList( s.SourceSpan, Array.AsReadOnly( results ), resultValues );
	}

	public virtual void Visit( Test s )
	{
		result = new Test( s.SourceSpan, Transform( s.Condition ), s.Target );
	}





	public virtual void Visit( Binary e )
	{
		result = new Binary( e.SourceSpan, e.Op, Transform( e.Left ), Transform( e.Right ) );
	}

	public virtual void Visit( Call e )
	{
		Expression function = Transform( e.Function );
		Expression[] arguments = new Expression[ e.Arguments.Count ];
		for ( int i = 0; i < e.Arguments.Count; ++i )
		{
			arguments[ i ] = Transform( e.Arguments[ i ] );
		}
		Expression argumentValues = e.ArgumentValues != null ? Transform( e.ArgumentValues ) : null;
		result = new Call( e.SourceSpan, function, Array.AsReadOnly( arguments ), argumentValues );
	}

	public virtual void Visit( CallSelf e )
	{
		Expression o = Transform( e.Object );
		Expression[] arguments = new Expression[ e.Arguments.Count ];
		for ( int i = 0; i < e.Arguments.Count; ++i )
		{
			arguments[ i ] = Transform( e.Arguments[ i ] );
		}
		Expression argumentValues = e.ArgumentValues != null ? Transform( e.ArgumentValues ) : null;
		result = new CallSelf( e.SourceSpan, o, e.MethodName, Array.AsReadOnly( arguments ), argumentValues );
	}

	public virtual void Visit( Comparison e )
	{
		result = new Comparison( e.SourceSpan, e.Op, Transform( e.Left ), Transform( e.Right ) );
	}

	public virtual void Visit( Concatenate e )
	{
		Expression[] operands = new Expression[ e.Operands.Count ];
		for ( int i = 0; i < e.Operands.Count; ++i )
		{
			operands[ i ] = Transform( e.Operands[ i ] );
		}
		result = new Concatenate( e.SourceSpan, operands );
	}

	public virtual void Visit( Constructor e )
	{
		ConstructorElement[] elements = new ConstructorElement[ e.Elements.Count ];
		for ( int i = 0; i < e.Elements.Count; ++i )
		{
			if ( e.Elements[ i ].HashKey == null )
			{
				elements[ i ] = new ConstructorElement(
					e.Elements[ i ].SourceSpan, Transform( e.Elements[ i ].Value ) );
			}
			else
			{
				elements[ i ] = new ConstructorElement( e.Elements[ i ].SourceSpan,
					Transform( e.Elements[ i ].HashKey ), Transform( e.Elements[ i ].Value ) );
			}
		}
		Expression elementList = e.ElementList != null ? Transform( e.ElementList ) : null;
		result = new Constructor( e.SourceSpan, e.ArrayCount, e.HashCount, Array.AsReadOnly( elements ), elementList );
	}

	public virtual void Visit( FunctionClosure e )
	{
		LuaAST f = Transform( e.Function );
		function.ChildFunction( f );
		result = new FunctionClosure( e.SourceSpan, f );
	}

	public virtual void Visit( GlobalRef e )
	{
		result = e;
	}

	public virtual void Visit( Index e )
	{
		result = new Index( e.SourceSpan, Transform( e.Table ), Transform( e.Key ) );
	}

	public virtual void Visit( Literal e )
	{
		result = e;
	}

	public virtual void Visit( LocalRef e )
	{
		result = e;
	}

	public virtual void Visit( Logical e )
	{
		result = new Logical( e.SourceSpan, e.Op, Transform( e.Left ), Transform( e.Right ) );
	}

	public virtual void Visit( Not e )
	{
		result = new Not( e.SourceSpan, Transform( e.Operand ) );
	}

	public virtual void Visit( Unary e )
	{
		result = new Unary( e.SourceSpan, e.Op, Transform( e.Operand ) );
	}

	public virtual void Visit( UpValRef e )
	{
		result = e;
	}

	public virtual void Visit( Vararg e )
	{
		result = e;
	}

}


}