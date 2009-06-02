// ANormalTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.Compiler.CLR
{


/*	In order to implement continuations using the technique described in
	http://www.ccs.neu.edu/scheme/pubs/stackhack4.html, we hoist function calls out of
	expressions and complex assignments so that after every function call the only thing
	on the evaluation stack is the result of the call.
*/

	
static class ANormalTransform
{

	public static FunctionAST Transform( FunctionAST function )
	{
		return Transform( function, null );
	}

	public static FunctionAST Transform( FunctionAST function, FunctionAST parent )
	{
		// Copy function.

		FunctionAST f = new FunctionAST( function.Name, parent );
		
		foreach ( Variable upval in function.UpVals )
		{
			f.UpVal( upval );
		}

		foreach ( Variable parameter in function.Parameters )
		{
			f.Parameter( parameter );
		}

		if ( function.IsVararg )
		{
			f.SetVararg();
		}

		foreach ( Variable local in function.Locals )
		{
			f.Local( local );
		}


		// Transform the statements into a-normal form.

		TransformExpression	t = new TransformExpression( f );
		TransformStatement	s = new TransformStatement( f, t );

		foreach ( Statement statement in function.Statements )
		{
			statement.Accept( s );
		}


		// Return transformed expression.

		return f;
	}


	sealed class TransformStatement
		:	StatementVisitor
	{
		FunctionAST			f;
		TransformExpression	t;
		

		public TransformStatement( FunctionAST f, TransformExpression t )
		{
			this.f	= f;
			this.t	= t;
		}


		public override void Visit( BeginBlock s )			{ f.Statement( s ); }
		public override void Visit( Break s )				{ f.Statement( s ); }
		public override void Visit( Continue s )			{ f.Statement( s ); }
		public override void Visit( EndBlock s )			{ f.Statement( s ); }
		public override void Visit( BeginConstructor s )	{ f.Statement( s ); }
		public override void Visit( EndConstructor s )		{ f.Statement( s ); }
		public override void Visit( BeginScope s )			{ f.Statement( s ); }
		public override void Visit( EndScope s )			{ f.Statement( s ); }
		

		public override void Visit( BeginTest s )
		{
			f.Statement( new BeginTest( s.SourceSpan, t.Transform( s.Condition ) ) );
		}

		public override void Visit( EndTest s )
		{
			f.Statement( s );
		}

		public override void Visit( Assign s )
		{
			if ( ( s.Target is GlobalRef ) || ( s.Target is Index ) )
			{
				// Assigning to these targets requires pushing things onto the stack
				// before the value, so if the value is a function call, it needs to
				// be hoisted into its own statement.

				f.Statement( new Assign( s.SourceSpan,
					t.Transform( s.Target ), t.TransformSingleValue( s.Value ) ) );
			}
			else
			{
				// Otherwise just hoist any nested function calls, as usual.

				f.Statement( new Assign( s.SourceSpan,
					t.Transform( s.Target ), t.Transform( s.Value ) ) );
			}
		}

		public override void Visit( Declare s )
		{
			f.Statement( new Declare( s.SourceSpan, s.Variable, t.Transform( s.Value ) ) );
		}

		public override void Visit( Evaluate s )
		{
			f.Statement( new Evaluate( s.SourceSpan, t.Transform( s.Expression ) ) );
		}

		public override void Visit( IndexMultipleValues s )
		{
			f.Statement( new IndexMultipleValues( s.SourceSpan, s.Constructor, s.Key,
				t.TransformMultipleValues( s.Values ) ) );
		}

		public override void Visit( Return s )
		{
			f.Statement( new Return( s.SourceSpan, t.Transform( s.Result ) ) );
		}

		public override void Visit( ReturnMultipleValues s )
		{
			if ( s.Results.Count > 0 )
			{
				// Not a tail call, transform.

				Expression[] results = new Expression[ s.Results.Count ];
				for ( int i = 0; i < s.Results.Count; ++i )
				{
					results[ i ] = t.Transform( s.Results[ i ] );
				}

				f.Statement( new ReturnMultipleValues( s.SourceSpan,
					Array.AsReadOnly( results ), t.TransformMultipleValues( s.ResultValues ) ) );
			}
			else
			{
				// If the multiple values are a function call, it will be a tail call and so
				// shouldn't be hoisted.  If it's ..., it doesn't need to be transformed anyway.

				f.Statement( s );
			}
		}

	}


	sealed class TransformExpression
		:	ExpressionVisitor
	{
		FunctionAST	f;
		Expression	result;


		public TransformExpression( FunctionAST f )
		{
			this.f	= f;
			result	= null;
		}


		public Expression Transform( Expression e )
		{
			if ( e == null)
			{
				return null;
			}

			e.Accept( this );
			Expression r = result;
			result = null;
			return r;
		}
		
		public Expression TransformSingleValue( Expression e )
		{
			e = Transform( e );
			if ( ( e is Call ) || ( e is CallSelf ) )
			{
				// Hoist out of line.

				Temporary t = new Temporary( e.SourceSpan );
				f.Statement( new Assign( e.SourceSpan, t, e ) );
				return t;
			}
			return e;
		}

		public Expression TransformMultipleValues( Expression e )
		{
			e = Transform( e );
			if ( ( e is Call ) || ( e is CallSelf ) )
			{
				// Hoist out of line.

				ValueList v = new ValueList( e.SourceSpan );
				f.Statement( new Assign( e.SourceSpan, v, e ) );
				return v;
			}
			return e;
		}


		
		public override void Visit( Binary e )
		{
			result = new Binary( e.SourceSpan, e.Op,
				TransformSingleValue( e.Left ), TransformSingleValue( e.Right ) );
		}

		public override void Visit( Call e )
		{
			Expression[] arguments = new Expression[ e.Arguments.Count ];
			for ( int i = 0; i < e.Arguments.Count; ++i )
			{
				arguments[ i ] = TransformSingleValue( e.Arguments[ i ] );
			}
			result = new Call( e.SourceSpan, TransformSingleValue( e.Function ),
				Array.AsReadOnly( arguments ), TransformMultipleValues( e.ArgumentValues ) );
		}

		public override void Visit( CallSelf e )
		{
			Expression[] arguments = new Expression[ e.Arguments.Count ];
			for ( int i = 0; i < e.Arguments.Count; ++i )
			{
				arguments[ i ] = TransformSingleValue( e.Arguments[ i ] );
			}
			result = new CallSelf( e.SourceSpan, TransformSingleValue( e.Function ), e.MethodName,
				Array.AsReadOnly( arguments ), TransformMultipleValues( e.ArgumentValues ) );
		}

		public override void Visit( Comparison e )
		{
			result = new Comparison( e.SourceSpan, e.Op,
				TransformSingleValue( e.Left ), TransformSingleValue( e.Right ) );
		}

		public override void Visit( Constructor e )
		{
			result = e;
		}

		public override void Visit( FunctionClosure e )
		{
			// Transform nested function.

			FunctionAST childFunction = ANormalTransform.Transform( e.Function, f );
			f.ChildFunction( childFunction );
			result = new FunctionClosure( e.SourceSpan, childFunction );
		}

		public override void Visit( GlobalRef e )
		{
			result = e;
		}

		public override void Visit( Index e )
		{
			result = new Index( e.SourceSpan,
				TransformSingleValue( e.Table ), TransformSingleValue( e.Key ) );
		}

		public override void Visit( Literal e )
		{
			result = e;
		}

		public override void Visit( LocalRef e )
		{
			result = e;
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

		public override void Visit( Temporary e )
		{
			result = e;
		}

		public override void Visit( ToNumber e )
		{
			result = new ToNumber( e.SourceSpan, TransformSingleValue( e.Operand ) );
		}

		public override void Visit( Unary e )
		{
			result = new Unary( e.SourceSpan, e.Op, TransformSingleValue( e.Operand ) );
		}

		public override void Visit( UpValRef e )
		{
			result = e;
		}

		public override void Visit( ValueList e )
		{
			result = e;
		}

		public override void Visit( ValueListElement e )
		{
			result = e;
		}

		public override void Visit( Vararg e )
		{
			result = e;
		}

		public override void Visit( VarargElement e )
		{
			result = e;
		}

	}


}


}
