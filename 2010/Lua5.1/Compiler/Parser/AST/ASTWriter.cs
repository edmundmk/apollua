// ASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Compiler.Parser.AST.Expressions;
using Lua.Compiler.Parser.AST.Statements;


namespace Lua.Compiler.Parser.AST
{


class ASTWriter
	:	IStatementVisitor
	,	IExpressionVisitor
{
	protected TextWriter	o;
	int						indent;


	public ASTWriter( TextWriter oWriter )
	{
		o		= oWriter;
		indent	= 0;
	}


	public void Write( FunctionAST function )
	{
		// Function signature.

		o.Write( "function " );
		if ( function.Name != null )
		{
			o.Write( function.Name );
		}
		else
		{
			o.Write( "x" );
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


		// Returns multiple values.

		if ( function.ReturnsMultipleValues )
		{
			o.WriteLine( "  -- multiple return" );
		}

		
		// Labels.
		
		if ( function.Labels.Count > 0 )
		{
			o.Write( "  -- label " );
			bool bFirst = true;
			foreach ( LabelAST label in function.Labels )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( label.Name );
			}
			o.WriteLine();
		}


		// Upvals.

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


		// Locals.

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
			o.WriteLine();
		}


		// Statements.

		WriteBlock( function.Block );


		// Final.

		o.WriteLine( "end" );
		o.WriteLine();
		

		// Other functions.
		
		foreach ( FunctionAST child in function.Functions )
		{
			Write( child );
		}
	}





	public void Visit( Assign s )
	{
		Indent();
		s.Target.Accept( this );
		o.Write( " = " );
		s.Value.Accept( this );
		o.WriteLine();
	}

	public void Visit( AssignList s )
	{
		Indent();
		bool bFirst = true;
		foreach ( Expression target in s.Targets )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			target.Accept( this );
		}
		o.Write( " = " );
		bFirst = true;
		foreach ( Expression value in s.Values )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			value.Accept( this );
		}
		if ( s.ValueList != null )
		{
			if ( ! bFirst )
				o.Write( ", " );
			o.Write( "values " );
			s.ValueList.Accept( this );
		}
		o.WriteLine();
	}

	public void Visit( Block s )
	{
		Indent();
		o.Write( "do -- " );
		o.Write( s.Name );
		o.WriteLine();

		WriteBlock( s );

		Indent();
		o.WriteLine( "end" );
	}

	public void WriteBlock( Block s )
	{
		indent += 1;

		if ( s.Locals.Count > 0 )
		{
			Indent();
			o.Write( "-- local " );
			bool bFirst = true;
			foreach ( Variable local in s.Locals )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( local.Name );
			}
			o.WriteLine();
		}
		
		foreach ( Statement statement in s.Statements )
		{
			statement.Accept( this );
		}

		indent -= 1;
	}

	public void Visit( Branch s )
	{
		Indent();
		o.Write( "b " );
		o.WriteLine( s.Target.Name );
	}

	public void Visit( Declare s )
	{
		Indent();
		o.Write( "local " );
		o.Write( s.Variable.Name );
		o.Write( " = " );
		s.Value.Accept( this );
		o.WriteLine();
	}

	public void Visit( DeclareList s )
	{
		Indent();
		o.Write( "local " );
		bool bFirst = true;
		foreach ( Variable variable in s.Variables )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			o.Write( variable.Name );
		}
		o.Write( " = " );
		o.Write( "values " );
		s.ValueList.Accept( this );
		o.WriteLine();
	}

	public void Visit( Evaluate s )
	{
		Indent();
		s.Expression.Accept( this );
		o.WriteLine();
	}

	public void Visit( ForBlock s )
	{
		Indent();
		o.Write( "for " );
		o.Write( s.UserIndex.Name );
		o.Write( " : " );
		o.Write( s.Index.Name );
		o.Write( ", " );
		o.Write( s.Limit.Name );
		o.Write( ", " );
		o.Write( s.Step.Name );
		o.Write( " : " );
		o.Write( s.BreakLabel.Name );
		o.Write( ", " );
		o.Write( s.ContinueLabel.Name );
		o.WriteLine();

		WriteBlock( s );

		Indent();
		o.WriteLine( "end" );
	}

	public void Visit( ForListBlock s )
	{
		Indent();
		o.Write( "forlist " );
		bool bFirst = true;
		foreach ( Variable variable in s.UserVariables )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			o.Write( variable.Name );
		}
		o.Write( " : " );
		o.Write( s.Generator.Name );
		o.Write( ", " );
		o.Write( s.State.Name );
		o.Write( ", " );
		o.Write( s.Control.Name );
		o.Write( " : " );
		o.Write( s.BreakLabel.Name );
		o.Write( ", " );
		o.Write( s.ContinueLabel.Name );
		o.WriteLine();

		WriteBlock( s );

		Indent();
		o.WriteLine( "end" );
	}

	public void Visit( MarkLabel s )
	{
		o.Write( s.Label.Name );
		o.WriteLine( ":" );
	}

	public void Visit( Return s )
	{
		Indent();
		o.Write( "return " );
		s.Result.Accept( this );
		o.WriteLine();
	}

	public void Visit( ReturnList s )
	{
		Indent();
		o.Write( "return " );
		bool bFirst = true;
		foreach ( Expression result in s.Results )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			result.Accept( this );
		}
		if ( s.ResultList != null )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = false;
			o.Write( "values " );
			s.ResultList.Accept( this );
		}
		o.WriteLine();
	}

	public void Visit( Test s )
	{
		Indent();
		o.Write( "bfalse " );
		s.Condition.Accept( this );
		o.Write( " " );
		o.WriteLine( s.Target.Name );
	}




	public void Visit( Binary e )
	{
		o.Write( "( " );
		e.Left.Accept( this );
		o.Write( " " );
		switch ( e.Op )
		{
		case BinaryOp.Add:				o.Write( "+" );		break;
		case BinaryOp.Subtract:			o.Write( "-" );		break;
		case BinaryOp.Multiply:			o.Write( "*" );		break;
		case BinaryOp.Divide:			o.Write( "/" );		break;
		case BinaryOp.IntegerDivide:	o.Write( "\\" );	break;
		case BinaryOp.Modulus:			o.Write( "%" );		break;
		case BinaryOp.RaiseToPower:		o.Write( "^" );		break;
		case BinaryOp.Concatenate:		o.Write( ".." );	break;
		}
		o.Write( " " );
		e.Right.Accept( this );
		o.Write( " )" );
	}

	public void Visit( Call e )
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

	public void Visit( CallSelf e )
	{
		e.Object.Accept( this );
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

	public void Visit( Comparison e )
	{
		o.Write( "( " );
		e.Left.Accept( this );
		o.Write( " " );
		switch ( e.Op )
		{
			case ComparisonOp.Equal:				o.Write( "==" );	break;
			case ComparisonOp.NotEqual:				o.Write( "!=" );	break;
			case ComparisonOp.LessThan:				o.Write( "<" );		break;
			case ComparisonOp.GreaterThan:			o.Write( ">" );		break;
			case ComparisonOp.LessThanOrEqual:		o.Write( "<=" );	break;
			case ComparisonOp.GreaterThanOrEqual:	o.Write( ">=" );	break;
		}
		o.Write( " " );
		e.Right.Accept( this );
		o.Write( " )" );
	}

	public virtual void Visit( Concat e )
	{
		// TODO.
	}

	public void Visit( Constructor e )
	{
		o.Write( "{" );
		o.WriteLine();
		indent += 1;

		foreach ( ConstructorElement element in e.Elements )
		{
			Indent();
			if ( element.HashKey != null )
			{
				o.Write( "[ " );
				element.HashKey.Accept( this );
				o.Write( " ] = " );
			}
			element.Value.Accept( this );
			o.Write( ";" );
			o.WriteLine();
		}
		if ( e.ElementList != null )
		{
			Indent();
			o.Write( "values " );
			e.ElementList.Accept( this );
			o.Write( ";" );
			o.WriteLine();
		}

		indent -= 1;
		Indent();
		o.Write( "}" );
	}

	public void Visit( FunctionClosure e )
	{
		o.Write( "function " );
		if ( e.Function.Name != null )
		{
			o.Write( e.Function.Name );
		}
		else
		{
			o.Write( "x" );
			o.Write( e.Function.GetHashCode().ToString( "X" ) );
		}
	}

	public void Visit( GlobalRef e )
	{
		o.Write( e.Name );
	}

	public void Visit( Index e )
	{
		e.Table.Accept( this );
		o.Write( "[ " );
		e.Key.Accept( this );
		o.Write( " ]" );
	}

	public void Visit( Literal e )
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

	public void Visit( LocalRef e )
	{
		o.Write( e.Variable.Name );
	}

	public void Visit( Logical e )
	{
		o.Write( "( " );
		e.Left.Accept( this );
		o.Write( " " );
		switch ( e.Op )
		{
		case LogicalOp.And:		o.Write( "and" );	break;
		case LogicalOp.Or:		o.Write( "or" );	break;
		}
		o.Write( " " );
		e.Right.Accept( this );
		o.Write( " )" );
	}

	public void Visit( Not e )
	{
		o.Write( "not " );
		e.Operand.Accept( this );
	}

	public void Visit( Unary e )
	{
		switch ( e.Op )
		{
		case UnaryOp.Minus:		o.Write( "-" );		break;
		case UnaryOp.Length:	o.Write( "#" );		break;
		}
		e.Operand.Accept( this );
	}

	public void Visit( UpValRef e )
	{
		o.Write( e.Variable.Name );
	}

	public void Visit( Vararg e )
	{
		o.Write( "..." );
	}



	protected void Indent()
	{
		for ( int i = 0; i < indent * 2; ++i )
		{
			o.Write( " " );
		}
	}

}


}

