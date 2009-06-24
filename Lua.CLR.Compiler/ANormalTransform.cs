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


/*
		// Construct AST.

		SourceSpan s			= new SourceSpan( matchFor.SourceSpan.Start, doToken.SourceSpan.End );
		LabelAST fornum			= new LabelAST( "fornum" );			function.Label( fornum );
		LabelAST fornumBreak	= new LabelAST( "fornumBreak" );	function.Label( fornumBreak );
		LabelAST fornumContinue	= new LabelAST( "fornumContinue" );	function.Label( fornumContinue );


		block = new Block( new SourceSpan(), block, "fornum" );
		block.Parent.Statement( block );
	

		// Delcare internal variables.

		Variable forIndex = new Variable( "(for index)" );
		function.Local( forIndex ); block.Local( forIndex );
		Variable forLimit = new Variable( "(for limit)" );
		function.Local( forLimit ); block.Local( forLimit );
		Variable forStep  = new Variable( "(for step)" );
		function.Local( forStep ); block.Local( forStep );

		Expression startExpression = new ToNumber( start.SourceSpan, start );
		Expression limitExpression = new ToNumber( limit.SourceSpan, limit );
		Expression stepExpression  = new ToNumber( step.SourceSpan, step );

		block.Statement( new Declare( start.SourceSpan, forIndex, startExpression ) );
		block.Statement( new Declare( limit.SourceSpan, forLimit, limitExpression ) );
		block.Statement( new Declare( step.SourceSpan, forStep, stepExpression ) );


		// Test expression.

		Expression condition =
			new Logical( s, LogicalOp.Or,
				new Logical( s, LogicalOp.And, 
					new Comparison( s, ComparisonOp.GreaterThan,
						new LocalRef( s, forStep ),
						new Literal( s, 0.0 ) ),
					new Comparison( s, ComparisonOp.LessThanOrEqual,
						new LocalRef( s, forIndex ),
						new LocalRef( s, forLimit ) ) ),
				new Logical( s, LogicalOp.And,
					new Comparison( s, ComparisonOp.LessThan,
						new LocalRef( s, forStep ),
						new Literal( s, 0.0 ) ),
					new Comparison( s, ComparisonOp.GreaterThanOrEqual,
						new LocalRef( s, forIndex ),
						new LocalRef( s, forLimit ) ) ) );


		// Loop body.

		block.Statement( new MarkLabel( s, fornum ) );
		block.Statement( new Test( s, condition, fornumBreak ) );

		block = new Block( new SourceSpan(), block, "fornumBody" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, fornumBreak, fornumContinue );
		

		// Declare index variable.

		Variable userIndex = new Variable( (string)name.Value );
		function.Local( userIndex ); block.Local( userIndex );
		Expression indexExpression = new LocalRef( s, forIndex );
		block.Statement( new Declare( s, userIndex, indexExpression ) );


		// Loop body.

		blockstat();

		Token endFor = Check( TokenKind.End, matchFor );


		// Close AST.

		s = endFor.SourceSpan;

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( doToken.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;

		block.Statement( new MarkLabel( s, fornumContinue ) );

		
		// Increment index.

		Expression indexVariable = new LocalRef( s, forIndex );
		Expression incrementExpression  =
			new Binary( s, BinaryOp.Add,
				new LocalRef( s, forIndex ),
				new LocalRef( s, forStep ) );

		block.Statement( new Assign( s, indexVariable, incrementExpression ) );


		// Finish loop.

		block.Statement( new Branch( s, fornum ) );
		block.Statement( new MarkLabel( s, fornumBreak ) );

		block.SetSourceSpan( new SourceSpan( matchFor.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;






		// Build AST

		SourceSpan s				= new SourceSpan( matchFor.SourceSpan.Start, doToken.SourceSpan.End );
		LabelAST forlistBreak		= new LabelAST( "forlistBreak" );		function.Label( forlistBreak );
		LabelAST forlistContinue	= new LabelAST( "forlistContinue" );	function.Label( forlistContinue );

		block = new Block( new SourceSpan(), block, "forlist" );
		block.Parent.Statement( block );


		// Declare internal variables.

		Token[] internalNameList = new Token[]
		{
			new Token( s, TokenKind.Identifier, "(for generator)" ),
			new Token( s, TokenKind.Identifier, "(for state)" ),
			new Token( s, TokenKind.Identifier, "(for control)" ),
		};
		LocalStatementAST( s, internalNameList, expressioncount );

		Debug.Assert( block.Locals.Count == 3 );
		Variable forGenerator	= block.Locals[ 0 ];
		Variable forState		= block.Locals[ 1 ];
		Variable forControl		= block.Locals[ 2 ];
		Debug.Assert( forGenerator.Name	== "(for generator)" );
		Debug.Assert( forState.Name		== "(for state)" );
		Debug.Assert( forControl.Name	== "(for control)" );


		// Loop block

		block.Statement( new MarkLabel( s, forlistContinue ) );

		block = new Block( new SourceSpan(), block, "forlistBody" );
		block.Parent.Statement( block );
		loopScope = new LoopScope( function, loopScope, forlistBreak, forlistContinue );


		// Generator expression.

		Expression generator =
			new Call( s,
				new LocalRef( s, forGenerator ),
				new Expression[] {
					new LocalRef( s, forState ),
					new LocalRef( s, forControl ) },
				null );

		PushExpression( generator );
		LocalStatementAST( s, namelist, 1 );

		Debug.Assert( block.Locals.Count == namelist.Count );
		Variable userControl = block.Locals[ 0 ];


		// Update control and test.

		Expression condition =
			new Comparison( s, ComparisonOp.NotEqual,
				new LocalRef( s, forControl ),
				new Literal( s, null ) );

		Expression controlVariable	= new LocalRef( s, forControl );
		Expression updateExpression	= new LocalRef( s, userControl );

		block.Statement( new Assign( s, controlVariable, updateExpression ) );
		block.Statement( new Test( s, condition, forlistBreak ) );


		// Loop body.

		blockstat();

		Token endFor = Check( TokenKind.End, matchFor );


		// Close AST.

		s = endFor.SourceSpan;

		loopScope = loopScope.Parent;
		block.SetSourceSpan( new SourceSpan( doToken.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;

		block.Statement( new Branch( s, forlistContinue ) );
		block.Statement( new MarkLabel( s, forlistBreak ) );

		block.SetSourceSpan( new SourceSpan( matchFor.SourceSpan.Start, endFor.SourceSpan.End ) );
		block = block.Parent;



		void LocalStatementAST( SourceSpan s, IList< Token > namelist, int expressioncount )
	{
		
		// Declare locals.

		IList< Variable > locallist = new Variable[ namelist.Count ];
		for ( int variable = 0; variable < namelist.Count; ++variable )
		{
			Variable local = new Variable( (string)namelist[ variable ].Value );
			function.Local( local ); block.Local( local );
			locallist[ variable ] = local;
		}
		

		// Assign.

		if ( expressioncount == 0 )
		{

			// Just declare all the variables.

			for ( int local = 0; local < locallist.Count; ++local )
			{
				block.Statement( new Declare( s, locallist[ local ],
					new Literal( s, null ) ) );
			}

		}
		else if ( namelist.Count > expressioncount )
		{

			// Check for multiple results on the last expression.
						
			Expression multipleValues = PopMultipleValues();
			if ( multipleValues != null )
			{
				expressioncount -= 1;
			}


			// Assign expressions.

			IList< Expression > expressionlist = PopValues( expressioncount );	
			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Expression value = expressionlist[ expression ];
				block.Statement( new Declare( s, locallist[ expression ], value ) );
			}


			// Deal with remaining variables.

			if ( multipleValues != null )
			{
				// Evaluate multiple values.

				block.Statement( new Assign( s,
					new ValueList( s, locallist.Count - expressionlist.Count ), multipleValues ) );


				// Assign from value list.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					block.Statement( new Declare( s, locallist[ local ],
						new ValueListElement( s, local - expressionlist.Count ) ) );
				}
			}
			else
			{
				// No extra values, just declare.

				for ( int local = expressionlist.Count; local < locallist.Count; ++local )
				{
					block.Statement( new Declare( s, locallist[ local ],
						new Literal( s, null ) ) );
				}
			}

		}
		else
		{
			
			// Assign locals.

			IList< Expression > expressionlist = PopValues( expressioncount );
			for ( int local = 0; local < locallist.Count; ++local )
			{
				Expression value = expressionlist[ local ];
				block.Statement( new Declare( s, locallist[ local ], value ) );
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = locallist.Count; expression < expressionlist.Count; ++expression )
			{
				Expression value = expressionlist[ expression ];
				block.Statement( new Evaluate( s, value ) );
			}

		}

	}



	void AssignStatementAST( SourceSpan s, int variablecount, int expressioncount )
	{
		Debug.Assert( variablecount > 0 && expressioncount > 0 );


		// Simpler code when there are no dependencies between expressions
		
		if ( variablecount == 1 && expressioncount == 1 )
		{
			Expression expression	= PopValue();
			Expression variable		= PopValue();			
			block.Statement( new Assign( s, variable, expression ) );	
			return;
		}


		// Pop expressions and check for multiple values.

		Expression multipleValues = null;
		if ( variablecount > expressioncount )
		{
			multipleValues = PopMultipleValues();
			if ( multipleValues != null )
			{
				expressioncount -= 1;
			}
		}

		IList< Expression > expressionlist = PopValues( expressioncount );

		
		// Transform assignment expressions.

		IList< Expression > vlist = PopValues( variablecount );
		IList< Expression > variablelist = new Expression[ variablecount ];
		for ( int variable = 0; variable < variablelist.Count; ++variable )
		{
			Index index = vlist[ variable ] as Index;
			if ( index != null )
			{
				Temporary temporaryTable	= new Temporary( index.Table.SourceSpan );
				Temporary temporaryKey		= new Temporary( index.Key.SourceSpan );

				block.Statement( new Assign( index.SourceSpan, temporaryTable, index.Table ) );
				block.Statement( new Assign( index.SourceSpan, temporaryKey, index.Key ) );

				variablelist[ variable ] = new Index( index.SourceSpan, temporaryTable, temporaryKey );
			}
			else
			{
				variablelist[ variable ] = vlist[ variable ];
			}
		}
		

		// Assign.

		if ( variablelist.Count > expressionlist.Count )
		{
			// Evaluate expression list and assign to temporaries.

			IList< Expression > temporarylist = new Expression[ expressionlist.Count ];
			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				Expression e			= expressionlist[ expression ];
				Expression temporary	= new Temporary( e.SourceSpan );
			
				block.Statement( new Assign( s, temporary, e ) );
				
				temporarylist[ expression ] = temporary;
			}


			// Evaluate last (multiple value) expression.

			if ( multipleValues != null )
			{
				block.Statement( new Assign( s,
					new ValueList( s, variablelist.Count - expressionlist.Count ), multipleValues ) );
			}


			// Perform assignments.

			for ( int expression = 0; expression < expressionlist.Count; ++expression )
			{
				block.Statement( new Assign( s,
					variablelist[ expression ], temporarylist[ expression ] ) );
			}
			
			if ( multipleValues != null )
			{
				// Assign from value list.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					block.Statement( new Assign( s, variablelist[ variable ],
						new ValueListElement( s, variable - expressionlist.Count ) ) );
				}
			}
			else
			{
				// No extra values, assign null.

				for ( int variable = expressionlist.Count; variable < variablelist.Count; ++variable )
				{
					block.Statement( new Assign( s, variablelist[ variable ],
						new Literal( s, null ) ) );
				}
			}
		}
		else
		{

			// Evaluate expression list and assign to temporaries.

			IList< Expression > temporarylist = new Expression[ variablelist.Count ];
			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				Expression e			= expressionlist[ variable ];
				Expression temporary	= new Temporary( e.SourceSpan );

				block.Statement( new Assign( s, temporary, e ) );
				
				temporarylist[ variable ] = temporary;
			}


			// Evaluate and throw away extra expressions.

			for ( int expression = variablelist.Count; expression < expressionlist.Count; ++expression )
			{
				Expression e = expressionlist[ expression ];
				block.Statement( new Evaluate( e.SourceSpan, e ) );
			}


			// Preform assignments.

			for ( int variable = 0; variable < variablelist.Count; ++variable )
			{
				block.Statement( new Assign( s, variablelist[ variable ], temporarylist[ variable ] ) );
			}

		}

	}



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

	public override void Visit( ConstructList s )
	{
		// Hoist any multiple values out.
		result = new ConstructList( s.SourceSpan, s.Temporary, s.Key, TransformMultipleValues( s.ValueList ) );
	}

	public override void Visit( ReturnList s )
	{
		if ( s.Results.Count > 0 )
		{
			// Not a tail call, transform.
			Expression[] results = new Expression[ s.Results.Count ];
			for ( int i = 0; i < s.Results.Count; ++i )
			{
				results[ i ] = TransformSingleValue( s.Results[ i ] );
			}
			Expression resultValues = s.ResultList != null ? TransformMultipleValues( s.ResultList ) : null;
			result = new ReturnList( s.SourceSpan, Array.AsReadOnly( results ), resultValues );
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
