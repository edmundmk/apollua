// ANormalTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.CLR.Compiler
{


/*	In order to implement continuations using the technique described in
	http://www.ccs.neu.edu/scheme/pubs/stackhack4.html, we hoist function calls out of
	expressions and complex assignments so that after every function call the only thing
	on the evaluation stack is the result of the call.
*/


public class ANormalTransform
	:	FunctionTransform
{

	Expression TransformSingleValue( Expression e )
	{
		e = Transform( e );
		if ( ( e is Call ) || ( e is CallSelf ) )
		{
			// Hoist out of line.
			
			Temporary t = new Temporary( e.SourceSpan );
			block.Statement( new Assign( e.SourceSpan, t, e ) );
			return t;
		}
		return e;
	}

	Expression TransformMultipleValues( Expression e )
	{
		e = Transform( e );
		if ( ( e is Call ) || ( e is CallSelf ) )
		{
			// Hoist out of line.

			ValueList v = new ValueList( e.SourceSpan, 0 );
			block.Statement( new Assign( e.SourceSpan, v, e ) );
			return v;
		}
		return e;
	}



	public override void Visit( Assign s )
	{
		if ( ( s.Target is GlobalRef ) || ( s.Target is Index ) )
		{
			// Assigning to these targets requires pushing things onto the stack
			// before the value, so if the value is a function call, it needs to
			// be hoisted into its own statement.
			result = new Assign( s.SourceSpan, Transform( s.Target ), TransformSingleValue( s.Value ) );
		}
		else
		{
			// Otherwise just hoist any nested function calls, as usual.
			base.Visit( s );
		}
	}

	public override void Visit( IndexMultipleValues s )
	{
		// Hoist any multiple values out.
		result = new IndexMultipleValues( s.SourceSpan, s.Temporary, s.Key, TransformMultipleValues( s.Values ) );
	}

	public override void Visit( ReturnMultipleValues s )
	{
		if ( s.Results.Count > 0 )
		{
			// Not a tail call, transform.
			Expression[] results = new Expression[ s.Results.Count ];
			for ( int i = 0; i < s.Results.Count; ++i )
			{
				results[ i ] = TransformSingleValue( s.Results[ i ] );
			}
			Expression resultValues = s.ResultValues != null ? TransformMultipleValues( s.ResultValues ) : null;
			result = new ReturnMultipleValues( s.SourceSpan, Array.AsReadOnly( results ), resultValues );
		}
		else
		{
			// If the multiple values are a function call, it will be a tail call and so
			// shouldn't be hoisted.  If it's ..., it doesn't need to be transformed anyway.
			base.Visit( s );
		}
	}


	public override void Visit( Binary e )
	{
		result = new Binary( e.SourceSpan, e.Op,
			TransformSingleValue( e.Left ), TransformSingleValue( e.Right ) );
	}

	public override void Visit( Call e )
	{
		Expression function = Transform( e.Function );
		Expression[] arguments = new Expression[ e.Arguments.Count ];
		for ( int i = 0; i < e.Arguments.Count; ++i )
		{
			arguments[ i ] = TransformSingleValue( e.Arguments[ i ] );
		}
		Expression argumentValues = e.ArgumentValues != null ? TransformMultipleValues( e.ArgumentValues ) : null;
		result = new Call( e.SourceSpan, function, Array.AsReadOnly( arguments ), argumentValues );
	}

	public override void Visit( CallSelf e )
	{
		Expression function = Transform( e.Object );
		Expression[] arguments = new Expression[ e.Arguments.Count ];
		for ( int i = 0; i < e.Arguments.Count; ++i )
		{
			arguments[ i ] = TransformSingleValue( e.Arguments[ i ] );
		}
		Expression argumentValues = e.ArgumentValues != null ? TransformMultipleValues( e.ArgumentValues ) : null;
		result = new CallSelf( e.SourceSpan, function, e.MethodName, Array.AsReadOnly( arguments ), argumentValues );
	}

	public override void Visit( Comparison e )
	{
		result = new Comparison( e.SourceSpan, e.Op,
			TransformSingleValue( e.Left ), TransformSingleValue( e.Right ) );
	}

	public override void Visit( Index e )
	{
		result = new Index( e.SourceSpan,
			TransformSingleValue( e.Table ), TransformSingleValue( e.Key ) );
	}

	public override void Visit( Logical e )
	{
		result = new Logical( e.SourceSpan, e.Op,
			TransformSingleValue( e.Left ), TransformSingleValue( e.Right ) );
	}

	public override void Visit( Not e )
	{
		result = new Not( e.SourceSpan, TransformSingleValue( e.Operand ) );
	}

	public override void Visit( ToNumber e )
	{
		result = new ToNumber( e.SourceSpan, TransformSingleValue( e.Operand ) );
	}

	public override void Visit( Unary e )
	{
		result = new Unary( e.SourceSpan, e.Op, TransformSingleValue( e.Operand ) );
	}

}



}
