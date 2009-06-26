// CLRTransform.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Parser.AST;
using Lua.Parser.AST.Statements;
using Lua.Parser.AST.Expressions;
using Lua.CLR.Compiler.AST;
using Lua.CLR.Compiler.AST.Expressions;


namespace Lua.CLR.Compiler
{


/*	The AST from the parser is transformed to remove Lua-specific constructions:

		->	ForBlocks and ForListBlocks are transformed into general statements.
		->	The order of operations in DeclareList and AssignList is made explicit
			and dependencies represented by temporaries.
*/

public class CLRTransform
	:	FunctionTransform
	,	ICLRExpressionVisitor
{

	// Make order of evaluation and dependencies explicit using temporaries.

	public override void Visit( AssignList s )
	{
		// Evaluate target expressions if necessary and store the resulting expressions.

		IList< Expression > targets = new Expression[ s.Targets.Count ];
		for ( int target = 0; target < s.Targets.Count; ++target )
		{
			Expression e = Transform( s.Targets[ target ] );
			Index index = e as Index;
			if ( index != null )
			{
				Temporary temporaryTable	= new Temporary();
				Temporary temporaryKey		= new Temporary();

				block.Statement( new Assign( index.SourceSpan,
					new TemporaryRef( index.Table.SourceSpan, temporaryTable ),
					Transform( index.Table ) ) );
				block.Statement( new Assign( index.SourceSpan,
					new TemporaryRef( index.Key.SourceSpan, temporaryKey ),
					Transform( index.Key ) ) );

				targets[ target ] = new Index( index.SourceSpan,
					new TemporaryRef( index.Table.SourceSpan, temporaryTable ),
					new TemporaryRef( index.Key.SourceSpan, temporaryKey ) );
			}
			else
			{
				targets[ target ] = e;
			}
		}
		

		// Assign.

		int assigncount = Math.Min( s.Targets.Count, s.Values.Count );
		IList< Expression > values = new Expression[ assigncount ];
		for ( int value = 0; value < assigncount; ++value )
		{
			Expression e = Transform( s.Values[ value ] );
			Temporary temporary = new Temporary();
			
			block.Statement( new Assign( e.SourceSpan,
				new TemporaryRef( e.SourceSpan, temporary ), e ) );

			values[ value ] = new TemporaryRef( e.SourceSpan, temporary );
		}

		if ( s.Targets.Count > s.Values.Count )
		{
			// Evaluate last (multiple value) expression.

			Expression valueList = s.ValueList != null ? Transform( s.ValueList ) : null;
			if ( ( valueList != null ) && !( valueList is Vararg ) )
			{
				block.Statement( new Assign( valueList.SourceSpan, new ValueList( valueList.SourceSpan ), valueList ) );
			}


			// Perform assignments.

			for ( int value = 0; value < values.Count; ++value )
			{
				block.Statement( new Assign( s.SourceSpan, targets[ value ], values[ value ] ) );
			}
			
			if ( valueList is Vararg )
			{
				// Assign from vararg.

				for ( int target = values.Count; target < targets.Count; ++target )
				{
					block.Statement( new Assign( s.SourceSpan, targets[ target ],
						new VarargElement( valueList.SourceSpan, target - values.Count ) ) );
				}
			}
			else if ( valueList != null )
			{
				// Assign from value list.

				for ( int target = values.Count; target < targets.Count; ++target )
				{
					block.Statement( new Assign( s.SourceSpan, targets[ target ],
						new ValueListElement( valueList.SourceSpan, target - values.Count ) ) );
				}
			}
			else
			{
				// No extra values, assign null.

				for ( int target = values.Count; target < targets.Count; ++target )
				{
					block.Statement( new Assign( s.SourceSpan, targets[ target ],
						new Literal( s.SourceSpan, null ) ) );
				}
			}
		}
		else
		{
			if ( s.ValueList != null )
				throw new ArgumentException();


			// Evaluate and throw away extra expressions.

			for ( int value = values.Count; value < s.Values.Count; ++value )
			{
				block.Statement( new Evaluate( s.Values[ value ].SourceSpan, Transform( s.Values[ value ] ) ) );
			}


			// Preform assignments.

			for ( int value = 0; value < values.Count; ++value )
			{
				block.Statement( new Assign( s.SourceSpan, targets[ value ], values[ value ] ) );
			}

		}
	}


	// Uses ValueList to make multiple-value assignments explicit.

	public override void Visit( DeclareList s )
	{
		Expression valueList = Transform( s.ValueList );

		if ( valueList is Vararg )
		{
			// Assign from vararg.

			for ( int variable = 0; variable < s.Variables.Count; ++variable )
			{
				block.Statement( new Declare( s.SourceSpan,
					s.Variables[ variable ], new VarargElement( valueList.SourceSpan, variable ) ) );
			}
		}
		else if ( valueList != null )
		{
			// Evaluate value list.

			block.Statement( new Assign( s.SourceSpan, new ValueList( s.SourceSpan ), valueList ) );


			// Assign from value list.

			for ( int variable = 0; variable < s.Variables.Count; ++variable )
			{
				block.Statement( new Declare( s.SourceSpan,
					s.Variables[ variable ], new ValueListElement( valueList.SourceSpan, variable ) ) );
			}
		}
	}

	public override void Visit( ForBlock s )
	{
		/*		do -- for
					local (for index), (for limit), (for step) = <start>, <limit>, <step>
					(for index) = __tonumber( (for index) )
					(for limit) = __tonumber( (for limit) )
					(for step) = __tonumber( (for step) )
			forLoop:
					bfalse (    ( (for step) > 0 and (for index) <= (for limit) )
							 or ( (for step) < 0 and (for index) >= (for limit) ) ) fornumBreak
					do -- forBody
						local <index> = (for index)
						...
					end
			forContinue:
					(for index) = (for index) + (for step)
					b fornum
			forBreak:
				end
		*/


		// ToNumber.

		block.Statement( new Assign( s.SourceSpan, new LocalRef( s.SourceSpan, s.Index ),
			new ToNumber( s.SourceSpan, new LocalRef( s.SourceSpan, s.Index ) ) ) );
		block.Statement( new Assign( s.SourceSpan, new LocalRef( s.SourceSpan, s.Limit ),
			new ToNumber( s.SourceSpan, new LocalRef( s.SourceSpan, s.Limit ) ) ) );
		block.Statement( new Assign( s.SourceSpan, new LocalRef( s.SourceSpan, s.Step ),
			new ToNumber( s.SourceSpan, new LocalRef( s.SourceSpan, s.Step ) ) ) );


		// Loop prologue.

		LabelAST forLoop = new LabelAST( "forloop" );
		function.Label( forLoop );

		block.Statement( new MarkLabel( s.SourceSpan, forLoop ) );

		Expression condition =
			new Logical( s.SourceSpan, LogicalOp.Or,
				new Logical( s.SourceSpan, LogicalOp.And, 
					new Comparison( s.SourceSpan, ComparisonOp.GreaterThan,
						new LocalRef( s.SourceSpan, s.Step ),
						new Literal( s.SourceSpan, 0 ) ),
					new Comparison( s.SourceSpan, ComparisonOp.LessThanOrEqual,
						new LocalRef( s.SourceSpan, s.Index ),
						new LocalRef( s.SourceSpan, s.Limit ) ) ),
				new Logical( s.SourceSpan, LogicalOp.And,
					new Comparison( s.SourceSpan, ComparisonOp.LessThan,
						new LocalRef( s.SourceSpan, s.Step ),
						new Literal( s.SourceSpan, 0 ) ),
					new Comparison( s.SourceSpan, ComparisonOp.GreaterThanOrEqual,
						new LocalRef( s.SourceSpan, s.Index ),
						new LocalRef( s.SourceSpan, s.Limit ) ) ) );

		block.Statement( new Test( s.SourceSpan, condition, s.BreakLabel ) );


		// Loop body.

		block = new Block( s.SourceSpan, block, "forBody" );
		block.Parent.Statement( block );

		block.Statement( new Declare( s.SourceSpan,
			s.UserIndex, new LocalRef( s.SourceSpan, s.Index ) ) );

		TransformBlock( s );

		block = block.Parent;


		// Loop epilogue.

		block.Statement( new MarkLabel( s.SourceSpan, s.ContinueLabel ) );

		Expression increment  =
			new Binary( s.SourceSpan, BinaryOp.Add,
				new LocalRef( s.SourceSpan, s.Index ),
				new LocalRef( s.SourceSpan, s.Step ) );

		block.Statement( new Assign( s.SourceSpan,
			new LocalRef( s.SourceSpan, s.Index ), increment ) );

		block.Statement( new MarkLabel( s.SourceSpan, s.BreakLabel ) );
	}

	public override void Visit( ForListBlock s )
	{
		/*		do -- forlist
					local (for generator), (for state), (for control) = <expressionlist>
			forlistContinue:
					do -- forlistBody
						local <variablelist> = (for generator) ( (for state), (for control) )
						(for control) = <variablelist>[ 0 ]
						bfalse ( (for control) != nil ) forlistBreak
						...
					end
					b forlistContinue
			forlistBreak:
				end
		*/


		// Loop prologue.

		block.Statement( new MarkLabel( s.SourceSpan, s.ContinueLabel ) );


		// Loop body.

		block = new Block( s.SourceSpan, block, "forlistBody" );
		block.Parent.Statement( block );


		// Execute generator.

		Expression generator =
			new Call( s.SourceSpan,
				new LocalRef( s.SourceSpan, s.Generator ),
				new Expression[] {
					new LocalRef( s.SourceSpan, s.State ),
					new LocalRef( s.SourceSpan, s.Control ) },
				null );

		if ( s.UserVariables.Count == 1 )
		{
			block.Statement( new Declare( s.SourceSpan, s.UserVariables[ 0 ], generator ) );
		}
		else
		{
			block.Statement( new Assign( s.SourceSpan, new ValueList( s.SourceSpan ), generator ) );
		
			for ( int variable = 0; variable < s.UserVariables.Count; ++variable )
			{
				block.Statement( new Declare( s.SourceSpan, s.UserVariables[ variable ],
					new ValueListElement( s.SourceSpan, variable ) ) );
			}
		}

		block.Statement( new Assign( s.SourceSpan,
			new LocalRef( s.SourceSpan, s.Control ),
			new LocalRef( s.SourceSpan, s.UserVariables[ 0 ] ) ) );


		// Test.

		Expression condition =
			new Comparison( s.SourceSpan, ComparisonOp.NotEqual,
				new LocalRef( s.SourceSpan, s.Control ),
				new Literal( s.SourceSpan, null ) );

		block.Statement( new Test( s.SourceSpan, condition, s.BreakLabel ) );


		// Statements.

		TransformBlock( s );
		
		block = block.Parent;


		// Loop epliogue.

		block.Statement( new Branch( s.SourceSpan, s.ContinueLabel ) );
		block.Statement( new MarkLabel( s.SourceSpan, s.BreakLabel ) );

	}



	public virtual void Visit( TemporaryRef e )
	{
		result = e;
	}

	public virtual void Visit( ToNumber e )
	{
		result = new ToNumber( e.SourceSpan, Transform( e.Operand ) );
	}

	public virtual void Visit( ValueList e )
	{
		result = e;
	}

	public virtual void Visit( ValueListElement e )
	{
		result = e;
	}

	public virtual void Visit( VarargElement e )
	{
		result = e;
	}

}


}

