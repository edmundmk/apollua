// ASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.Parser.AST
{


public static class ASTWriter
{

	public static void Write( TextWriter o, Function function )
	{
		ExpressionWriter	ew = new ExpressionWriter( o );
		StatementWriter		sw = new StatementWriter( o, ew );

		o.Write( "function " );
		if ( function.Name != null )
		{
			o.Write( function.Name );
		}
		else
		{
			o.Write( function.GetHashCode().ToString( "X" ) );
		}
		o.Write( "(" );
		if ( function.Parameters.Count > 0 || function.IsVararg )
		{
			o.Write( " " );
			bool bFirst = true;
			foreach ( Variable parameter in function.Parameters )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( parameter.Name );
			}
			if ( function.IsVararg )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( "..." );
			}
			o.Write( " " );
		}
		o.WriteLine( ")" );

		if ( function.UpVals.Count > 0 )
		{
			o.Write( "  -- upval " );
			bool bFirst = true;
			foreach ( Variable upval in function.UpVals )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( upval.Name );
			}
			o.WriteLine();
		}

		if ( function.Locals.Count > 0 )
		{
			o.Write( "  -- local " );
			bool bFirst = true;
			foreach ( Variable local in function.Locals )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( local.Name );
			}
			o.WriteLine( "" );
		}
		
		foreach ( Statement statement in function.Statements )
		{
			statement.Accept( sw );
		}

		o.WriteLine( "end" );
		o.WriteLine();
		
		
		foreach ( Function child in function.Functions )
		{
			Write( o, child );
		}
	}


	class ExpressionWriter
		:	ExpressionVisitor
	{
		TextWriter o;

		public ExpressionWriter( TextWriter o )
		{
			this.o = o;
		}


		public override void Visit( Binary e )
		{
			o.Write( "( " );
			e.Left.Accept( this );
			o.Write( " " );
			o.Write( e.Op );
			o.Write( " " );
			e.Right.Accept( this );
			o.Write( " )" );
		}

		public override void Visit( Call e )
		{
			e.Function.Accept( this );
			o.Write( "(" );
			if ( e.Arguments.Count > 0 || e.ArgumentValues != null )
			{
				o.Write( " " );
				bool bFirst = true;
				foreach ( Expression argument in e.Arguments )
				{
					if ( ! bFirst )
						o.Write( ", " );
					bFirst = false;
					argument.Accept( this );
				}
				if ( e.ArgumentValues != null )
				{
					if ( ! bFirst )
						o.Write( ", " );
					bFirst = false;
					o.Write( "values " );
					e.ArgumentValues.Accept( this );
				}
				o.Write( " " );
			}
			o.Write( ")" );
		}

		public override void Visit( CallSelf e )
		{
			e.Function.Accept( this );
			o.Write( ":" );
			o.Write( e.MethodName );
			o.Write( "(" );
			if ( e.Arguments.Count > 0 || e.ArgumentValues != null )
			{
				o.Write( " " );
				bool bFirst = true;
				foreach ( Expression argument in e.Arguments )
				{
					if ( ! bFirst )
						o.Write( ", " );
					bFirst = false;
					argument.Accept( this );
				}
				if ( e.ArgumentValues != null )
				{
					if ( ! bFirst )
						o.Write( ", " );
					bFirst = false;
					o.Write( "values " );
					e.ArgumentValues.Accept( this );
				}
				o.Write( " " );
			}
			o.Write( ")" );
		}

		public override void Visit( Comparison e )
		{
			o.Write( "( " );
			e.Left.Accept( this );
			o.Write( " " );
			o.Write( e.Op );
			o.Write( " " );
			e.Right.Accept( this );
			o.Write( " )" );
		}

		public override void Visit( Constructor e )
		{
			o.Write( "constructor " );
			o.Write( e.GetHashCode().ToString( "X" ) );
		}

		public override void Visit( FunctionClosure e )
		{
			o.Write( "function " );
			if ( e.Function.Name != null )
			{
				o.Write( e.Function.Name );
			}
			else
			{
				o.Write( e.Function.GetHashCode().ToString( "X" ) );
			}
		}

		public override void Visit( Global e )
		{
			o.Write( e.Name );
		}

		public override void Visit( Index e )
		{
			e.Table.Accept( this );
			o.Write( "[ " );
			e.Key.Accept( this );
			o.Write( " ]" );
		}

		public override void Visit( Literal e )
		{
			if ( e.Value == null )
			{
				o.Write( "nil" );
			}
			else if ( e.Value is string )
			{
				o.Write( "\"" );
				o.Write( ( (string)e.Value ).Replace( "\n", "\\n" ).Replace( "\"", "\\\"" ) );
				o.Write( "\"" );
			}
			else
			{
				o.Write( e.Value.ToString() );
			}
		}

		public override void Visit( Local e )
		{
			o.Write( e.Variable.Name );
		}

		public override void Visit( Logical e )
		{
			o.Write( "( " );
			e.Left.Accept( this );
			o.Write( " " );
			o.Write( e.Op );
			o.Write( " " );
			e.Right.Accept( this );
			o.Write( " )" );
		}

		public override void Visit( Not e )
		{
			o.Write( "not " );
			e.Operand.Accept( this );
		}

		public override void Visit( Temporary e )
		{
			o.Write( "temporary " );
			o.Write( e.GetHashCode().ToString( "X" ) );
		}

		public override void Visit( ToNumber e )
		{
			o.Write( "tonumber " );
			e.Operand.Accept( this );
		}

		public override void Visit( Unary e )
		{
			o.Write( e.Op );
			o.Write( " " );
			e.Operand.Accept( this );
		}

		public override void Visit( UpVal e )
		{
			o.Write( e.Variable.Name );
		}

		public override void Visit( ValueList e )
		{
			o.Write( "valuelist" );
		}

		public override void Visit( ValueListElement e )
		{
			o.Write( "valuelist[ " );
			o.Write( e.Index );
			o.Write( " ]" );
		}

		public override void Visit( Vararg e )
		{
			o.Write( "..." );
		}

		public override void Visit( VarargElement e )
		{
			o.Write( "...[ " );
			o.Write( e.Index );
			o.Write( " ]" );
		}
	}


	class StatementWriter
		:	StatementVisitor
	{
		TextWriter			o;
		ExpressionWriter	x;
		int					indent;
		
		public StatementWriter( TextWriter o, ExpressionWriter x )
		{
			this.o	= o;
			this.x	= x;
			indent	= 1;
		}


		public override void Visit( BeginBlock s )
		{
			Indent();
			o.Write( "block " );
			o.WriteLine( s.Name );
			indent += 1;
		}

		public override void Visit( EndBlock s )
		{
			indent -= 1;
			Indent();
			o.WriteLine( "end" );
		}

		public override void Visit( BeginConstructor s )
		{
			Indent();
			o.Write( "constructor " );
			o.WriteLine( s.Constructor.GetHashCode().ToString( "X" ) );
			indent += 1;
		}

		public override void Visit( EndConstructor s )
		{
			indent -= 1;
			Indent();
			o.WriteLine( "end" );
		}

		public override void Visit( BeginScope s )
		{
			Indent();
			o.WriteLine( "scope" );
			indent += 1;
		}

		public override void Visit( EndScope s )
		{
			indent -= 1;
			Indent();
			o.WriteLine( "end" );
		}

		public override void Visit( BeginTest s )
		{
			Indent();
			o.Write( "test " );
			s.Condition.Accept( x );
			o.WriteLine();
			indent += 1;
		}

		public override void Visit( EndTest s )
		{
			indent -= 1;
			Indent();
			o.WriteLine( "end" );
		}


		public override void Visit( Assign s )
		{
			Indent();
			s.Target.Accept( x );
			o.Write( " = " );
			if ( s.Target is ValueList )
			{
				o.Write( "values " );
			}
			s.Value.Accept( x );
			o.WriteLine();
		}

		public override void Visit( Break s )
		{
			Indent();
			o.Write( "break " );
			o.WriteLine( s.BlockName );
		}

		public override void Visit( Continue s )
		{
			Indent();
			o.Write( "continue " );
			o.WriteLine( s.BlockName );
		}

		public override void Visit( Declare s )
		{
			Indent();
			o.Write( "local " );
			o.Write( s.Variable.Name );
			o.Write( " = " );
			s.Value.Accept( x );
			o.WriteLine();
		}

		public override void Visit( Evaluate s )
		{
			Indent();
			s.Expression.Accept( x );
			o.WriteLine();
		}

		public override void Visit( IndexMultipleValues s )
		{
			Indent();
			s.Table.Accept( x );
			o.Write( "[ " );
			o.Write( s.Key );
			o.Write( " ... ] = values " );
			s.Values.Accept( x );
			o.WriteLine();
		}

		public override void Visit( Return s )
		{
			Indent();
			o.Write( "return " );
			s.Result.Accept( x );
			o.WriteLine();
		}

		public override void Visit( ReturnMultipleValues s )
		{
			Indent();
			o.Write( "return multiple " );
			bool bFirst = true;
			foreach ( Expression result in s.Results )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				result.Accept( x );
			}
			if ( s.ResultValues != null )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( "values " );
				s.ResultValues.Accept( x );
			}
			o.WriteLine();
		}


		void Indent()
		{
			for ( int i = 0; i < indent * 2; ++i )
			{
				o.Write( " " );
			}
		}
	}

}


}

