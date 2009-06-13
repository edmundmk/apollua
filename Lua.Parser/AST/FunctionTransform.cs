// FunctionTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


public class FunctionTransform
	:	IStatementVisitor
	,	IExpressionVisitor
{
	protected FunctionAST	function;
	protected Block			block;
	protected object		result;


	public FunctionTransform()
	{
		function	= null;
		block		= null;
	}


	public virtual FunctionAST Transform( FunctionAST f )
	{
		function = new FunctionAST( f.Name, function );

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
		foreach ( LabelAST label in f.Labels )
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

	public virtual void Visit( Block s )
	{
		block = new Block( s.SourceSpan, block, s.Name );

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

		result = block;
		block = block.Parent;
	}

	public virtual void Visit( Branch s )
	{
		result = s;
	}

	public virtual void Visit( Constructor s )
	{
		Constructor constructor = new Constructor( s.SourceSpan, s.Temporary, s.ArrayCount, s.HashCount );

		// Statements.
		foreach ( Statement statement in s.Statements )
		{
			Statement transformed = Transform( statement );
			if ( transformed != null )
			{
				constructor.Statement( transformed );
			}
		}

		result = constructor;
	}

	public virtual void Visit( Declare s )
	{
		result = new Declare( s.SourceSpan, s.Variable, Transform( s.Value ) );
	}

	public virtual void Visit( Evaluate s )
	{
		result = new Evaluate( s.SourceSpan, Transform( s.Expression ) );
	}

	public virtual void Visit( IndexMultipleValues s )
	{
		result = new IndexMultipleValues( s.SourceSpan, s.Temporary, s.Key, Transform( s.Values ) );
	}

	public virtual void Visit( MarkLabel s )
	{
		result = s;
	}

	public virtual void Visit( Return s )
	{
		result = new Return( s.SourceSpan, Transform( s.Result ) );
	}

	public virtual void Visit( ReturnMultipleValues s )
	{
		Expression[] results = new Expression[ s.Results.Count ];
		for ( int i = 0; i < s.Results.Count; ++i )
		{
			results[ i ] = Transform( s.Results[ i ] );
		}
		Expression resultValues = s.ResultValues != null ? Transform( s.ResultValues ) : null;
		result = new ReturnMultipleValues( s.SourceSpan, Array.AsReadOnly( results ), resultValues );
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
		Expression function = Transform( e.Function );
		Expression[] arguments = new Expression[ e.Arguments.Count ];
		for ( int i = 0; i < e.Arguments.Count; ++i )
		{
			arguments[ i ] = Transform( e.Arguments[ i ] );
		}
		Expression argumentValues = e.ArgumentValues != null ? Transform( e.ArgumentValues ) : null;
		result = new CallSelf( e.SourceSpan, function, e.MethodName, Array.AsReadOnly( arguments ), argumentValues );
	}

	public virtual void Visit( Comparison e )
	{
		result = new Comparison( e.SourceSpan, e.Op, Transform( e.Left ), Transform( e.Right ) );
	}

	public virtual void Visit( FunctionClosure e )
	{
		FunctionAST f = Transform( e.Function );
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

	public virtual void Visit( Temporary e )
	{
		result = e;
	}

	public virtual void Visit( ToNumber e )
	{
		result = new ToNumber( e.SourceSpan, Transform( e.Operand ) );
	}

	public virtual void Visit( Unary e )
	{
		result = new Unary( e.SourceSpan, e.Op, Transform( e.Operand ) );
	}

	public virtual void Visit( UpValRef e )
	{
		result = e;
	}

	public virtual void Visit( ValueList e )
	{
		result = e;
	}

	public virtual void Visit( ValueListElement e )
	{
		result = e;
	}

	public virtual void Visit( Vararg e )
	{
		result = e;
	}

	public virtual void Visit( VarargElement e )
	{
		result = e;
	}
}


}