// LuaCLRCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;
using Lua.VM.Compiler.AST.Expressions;
using Lua.VM.Compiler.AST.Statements;



namespace Lua.VM.Compiler
{


/*	Each function is compiled to a class deriving from Lua.Function.
*/


public class LuaVMCompiler
	:	IStatementVisitor
	,	IExpressionVisitor
{
	// Errors.

	TextWriter				errorWriter;
	bool					hasError;


	// Parser.

	string					sourceName;
	LuaParser				parser;


	// Prototype building.

	Builder					builder;
	BlockBuilder			blockBuilder;
	Allocation				target;



	public LuaVMCompiler( TextWriter errorWriter, TextReader source, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceName		= sourceName;
		parser				= new LuaParser( errorWriter, source, sourceName );
	}

	public bool HasError
	{
		get { return hasError || parser.HasError; }
	}


	public Function Compile()
	{
		// Parse the function.

		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}
		

		return null;
	}


	Builder BuildPrototype( FunctionAST function )
	{
		return null;
	}




	// Builder.

	class Allocation
	{
		public void Allocate()
		{
			Allocate( 1 );
		}

		public void Allocate( int count )
		{
		}

		public void SetTop()
		{
		}

		public void Release()
		{
		}

		public static implicit operator int( Allocation a )
		{
			return 0;
		}
	}

	
	enum UpValSource
	{
		Local,
		UpVal
	}

	struct UpValLocator
	{
		public int			TargetIndex		{ get; private set; }
		public UpValSource	Source			{ get; private set; }
		public int			SourceIndex		{ get; private set; }
	}


	class Builder
	{
		public Builder			Parent			{ get; private set; }
		public FunctionAST		Function		{ get; private set; }


		public Builder( Builder parent, FunctionAST function, BlockBuilder functionBlock )
		{
			Parent			= parent;
			Function		= function;			
		}



		// Local and temporary lifetime.

		public bool DeclareLocal( Variable local )
		{
			return false;
		}

		public void DeclareLocalFromTemporary( Variable local, Temporary temporary )
		{
		}
		
		public void RetireLocal( Variable local )
		{
		}

		public void DeclareTemporary( Temporary temporary )
		{
		}

		public void LockTemporary( Temporary temporary )
		{
		}

		public void UnlockTemporary( Temporary temporary )
		{
		}

		public void RetireTemporary( Temporary temporary )
		{
		}



		// Registers.

		public Allocation Local( Variable local )
		{
			return new Allocation();
		}

		public Allocation Temporary( Temporary temporary )
		{
			return new Allocation();
		}

		public Allocation Top()
		{
			return new Allocation();
		}


		// RK values.

		public Allocation ConstantRK( object constant )
		{
			int k = Constant( constant );
			if ( Instruction.InRangeRK( k ) )
			{
			}
			return new Allocation();
		}

		
		// Values referenced by special instructions.

		public int UpVal( Variable upval )
		{
			return 0;
		}

		public int Constant( object constant )
		{
			return 0;
		}

		public int Prototype( Builder prototypeBuilder )
		{
			return 0;
		}
		

		// Opcodes.

		public void InstructionABC( Opcode opcode, int A, int B, int C )
		{
		}

		public void InstructionABx( Opcode opcode, int A, int Bx )
		{
		}

		public void InstructionIndex( int C )
		{
		}

		public void Label( LabelAST label )
		{
		}

		public void InstructionAsBx( Opcode opcode, int A, LabelAST label )
		{
		}

	}


	class BlockBuilder
	{
		public BlockBuilder	Parent		{ get; private set; }
		public Block		Block		{ get; private set; }
		public int			Base		{ get; private set; }
		public bool			NeedsClose	{ get; private set; }


		public BlockBuilder( BlockBuilder parent, Block block, int blockBase )
		{
			Parent		= parent;
			Block		= block;
			Base		= blockBase;
			NeedsClose	= false;
		}

		public void SetNeedsClose()
		{
			NeedsClose	= true;
		}
	}


	// Closing blocks.

	void Close( Block block )
	{
		bool needsClose = false;
		int	 closeBase  = 0;

		for ( BlockBuilder outer = blockBuilder; outer.Block != block; outer = outer.Parent )
		{
			if ( outer == null )
			{
				return;
			}

			needsClose |= outer.NeedsClose;
			closeBase = outer.Base;
		}

		if ( needsClose )
		{
			builder.InstructionABC( Opcode.Close, closeBase, 0, 0 );
		}
	}




	// Expression evaluation.

	void Move( Allocation allocation, Expression e )
	{
		Allocation oldtarget = target;
		target = allocation;
		e.Accept( this );
		target = oldtarget;		
	}

	Allocation Push( Expression e )
	{
		Allocation allocation = builder.Top();
		Move( allocation, e );
		allocation.Allocate();
		return allocation;
	}

	Allocation R( Expression e )
	{
		if ( e is LocalRef )
		{
			return builder.Local( ( (LocalRef)e ).Variable );
		}
		else if ( e is Temporary )
		{
			return builder.Temporary( (Temporary)e );
		}

		return Push( e );
	}

	Allocation RK( Expression e )
	{
		if ( e is Literal )
		{
			return builder.ConstantRK( ( (Literal)e ).Value ); 
		}

		return R( e );
	}

	Allocation SetTop( Expression e )
	{
		Allocation results;

		if ( e is Call )
		{
			Call call = (Call)e;
			results = BuildCall( 0, call.Function, null, call.Arguments, call.ArgumentValues );
		}
		else if ( e is CallSelf )
		{
			CallSelf call = (CallSelf)e;
			results = BuildCall( 0, call.Object, call.MethodName, call.Arguments, call.ArgumentValues );
		}
		else if ( e is Vararg )
		{
			results = builder.Top();
			builder.InstructionABC( Opcode.Vararg, results, 0, 0 );
			results.SetTop();
		}
		else
		{
			throw new InvalidOperationException();
		}

		return results;
	}

	void Branch( Expression e, bool ifTrue, LabelAST label )
	{
		if ( e is Comparison )
		{
			// Perform comparison.
			Comparison comparison = (Comparison)e;

			Opcode op; int A;
			switch ( comparison.Op )
			{
			case ComparisonOp.Equal:				op = Opcode.Eq; A = 1;	break;
			case ComparisonOp.NotEqual:				op = Opcode.Eq; A = 0;	break;
			case ComparisonOp.LessThan:				op = Opcode.Lt; A = 1;	break;
			case ComparisonOp.GreaterThan:			op = Opcode.Le; A = 0;	break;
			case ComparisonOp.LessThanOrEqual:		op = Opcode.Le; A = 1;	break;
			case ComparisonOp.GreaterThanOrEqual:	op = Opcode.Lt; A = 0;	break;
			default: throw new ArgumentException();
			}

			if ( ! ifTrue )
			{
				A = ~A;
			}

			Allocation left		= RK( comparison.Left );
			Allocation right	= RK( comparison.Right );
			builder.InstructionABC( op, A, left, right );
			builder.InstructionAsBx( Opcode.Jmp, 0, label );
			right.Release();
			left.Release();

		}
		else if ( e is Logical )
		{
			// Perform shortcut evaluation.
			Logical logical = (Logical)e;

			if ( logical.Op == LogicalOp.And )
			{
				if ( ifTrue )
				{
					// left and right
					LabelAST noBranch = new LabelAST( "noBranch" );
					Branch( logical.Left, false, noBranch );
					Branch( logical.Right, true, label );
					builder.Label( noBranch );
				}
				else
				{
					// not( left and right ) == not( left ) or not( right )
					Branch( logical.Left, false, label );
					Branch( logical.Right, false, label );
				}
			}
			else if ( logical.Op == LogicalOp.Or )
			{
				if ( ifTrue )
				{
					// left or right
					Branch( logical.Left, true, label );
					Branch( logical.Right, true, label );
				}
				else
				{
					// not( left or right ) == not( left ) and not( right )
					LabelAST noBranch = new LabelAST( "noBranch" );
					Branch( logical.Left, true, noBranch );
					Branch( logical.Right, false, label );
					builder.Label( noBranch );
				}
			}
			else
			{
				throw new ArgumentException();
			}
		}
		else if ( e is Not )
		{
			// Branch in the opposite sense.
			Branch( ( (Not)e ).Operand, ! ifTrue, label );
		}
		else
		{
			// Test an actual value.
			Allocation expression = R( e );
			builder.InstructionABC( Opcode.Test, expression, 0, ifTrue ? 1 : 0 );
			builder.InstructionAsBx( Opcode.Jmp, 0, label );
			expression.Release();
		}
	}




	// Statement visitors.
	
	public void Visit( Assign s )
	{
		if ( s.Target is GlobalRef )
		{
			// Assign to global.
			GlobalRef global = (GlobalRef)s.Target;
			int constant		= builder.Constant( ( (GlobalRef)s.Target ).Name );
			Allocation value	= R( s.Value );
			builder.InstructionABx( Opcode.SetGlobal, value, constant );
			value.Release();
		}
		else if ( s.Target is Index )
		{
			// Assign to table index.
			Index index = (Index)s.Target;
			Allocation table	= R( index.Table );
			Allocation key		= RK( index.Key );
			Allocation value	= RK( s.Value );
			builder.InstructionABC( Opcode.SetTable, table, key, value );
			value.Release();
			key.Release();
			table.Release();
		}
		else if ( s.Target is LocalRef )
		{
			// Assign to local.
			LocalRef local = (LocalRef)s.Target;
			Allocation target = builder.Local( local.Variable );
			Move( target, s.Value );
			target.Release();
		}
		else if ( s.Target is Temporary )
		{
			// Assign to temporary.
			Temporary temporary = (Temporary)s.Target;
			builder.DeclareTemporary( temporary );
			Allocation target = builder.Temporary( temporary );
			Move( target, s.Value );
			target.Release();
		}
		else if ( s.Target is TemporaryList )
		{
			// Assign to temporary list.
			TemporaryList temporaryList = (TemporaryList)s.Target;

			// Evaluate results.
			Allocation results;
			if ( s.Value is Call )
			{
				Call call = (Call)s.Value;
				results = BuildCall( temporaryList.Temporaries.Count + 1,
					call.Function, null, call.Arguments, call.ArgumentValues );
			}
			else if ( s.Value is CallSelf )
			{
				CallSelf call = (CallSelf)s.Value;
				results = BuildCall( temporaryList.Temporaries.Count + 1,
					call.Object, call.MethodName, call.Arguments, call.ArgumentValues );
			}
			else if ( s.Value is Vararg )
			{
				results = builder.Top();
				builder.InstructionABC( Opcode.Vararg, results, temporaryList.Temporaries.Count, 0 );
				results.Allocate( temporaryList.Temporaries.Count );
			}
			else
			{
				throw new InvalidOperationException();
			}

			
			// Replace results with temporaries.
			results.Release();
			foreach ( Temporary temporary in temporaryList.Temporaries )
			{
				builder.DeclareTemporary( temporary );
			}
		}
		else 
		{
			throw new InvalidOperationException();
		}
	}

	public void Visit( Block s )
	{
		// Enter block.
		Allocation blockBase = builder.Top();
		blockBuilder = new BlockBuilder( blockBuilder, s, blockBase );
		blockBase.Release();

		// Build statements.
		foreach ( Statement statement in s.Statements )
		{
			statement.Accept( this );
		}

		// Close block if necessary.
		if ( blockBuilder.NeedsClose )
		{
			builder.InstructionABC( Opcode.Close, blockBuilder.Base, 0, 0 );
		}
		
		// Retire locals.
		for ( int local = s.Locals.Count - 1; local >= 0; --local )
		{
			builder.RetireLocal( s.Locals[ local ] );
		}

		// End of block.
		blockBuilder = blockBuilder.Parent;
	}

	public void Visit( Branch s )
	{
		// If we are jumping out of the block, close upvals.
		Close( s.Target.Block );

		// Jump.
		builder.InstructionAsBx( Opcode.Jmp, 0, s.Target );
	}

	public void Visit( Constructor s )
	{
		// Create constructor.
		builder.DeclareTemporary( s.Temporary );
		builder.LockTemporary( s.Temporary );
		Allocation constructor = builder.Temporary( s.Temporary );
		builder.InstructionABC( Opcode.NewTable, constructor, s.ArrayCount, s.HashCount );
		constructor.Release();

		// Initialization statements.
		foreach ( Statement statement in s.Statements )
		{
			statement.Accept( this );
		}

		// Next use of the constructor temporary retires it.
		builder.UnlockTemporary( s.Temporary );
	}

	public void Visit( Declare s )
	{
	}

	public void Visit( DeclareForIndex s )
	{
		builder.DeclareLocal( s.Variable );
	}
	
	public void Visit( Evaluate s )
	{
		// Evaluate and throw away result.
		Allocation allocation = R( s.Expression );
		allocation.Release();
	}

	public void Visit( ConstructList s )
	{
		throw new NotImplementedException();
	}

	public void Visit( MarkLabel s )
	{
		builder.Label( s.Label );
	}

	public void Visit( OpcodeForLoop s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeForPrep s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeSetList s )
	{
		throw new NotImplementedException();
	}

	public void Visit( OpcodeTForLoop s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Return s )
	{
		// Close upvals.
		Close( builder.Function.Block.Parent );

		// Return.
		if ( ( s.Result is Literal ) && ( ( (Literal)s.Result ).Value == null ) )
		{
			builder.InstructionABC( Opcode.Return, 0, 1, 0 );
		}
		else
		{
			Allocation result = R( s.Result );
			builder.InstructionABC( Opcode.Return, result, 2, 0 );
			result.Release();
		}
	}

	public void Visit( ReturnList s )
	{
		// Close upvals.
		Close( builder.Function.Block.Parent );

		// Push results onto the stack.
		Allocation allocation = builder.Top();
		foreach ( Expression result in s.Results )
		{
			Allocation allocResult = Push( result );
			allocResult.Release();
			allocation.Allocate();
		}

		// Push result values onto the stack.
		int B;
		if ( s.ResultList != null )
		{
			Allocation allocValues = SetTop( s.ResultList );
			allocValues.Release();
			allocation.SetTop();
			B = 0;
		}
		else
		{
			B = s.Results.Count + 1;
		}

		// Return.
		builder.InstructionABC( Opcode.Return, allocation, B, 0 );
		allocation.Release();
	}

	public void Visit( Test s )
	{
		throw new NotImplementedException();
	}




	// Expression visitors.

	public void Visit( Binary e )
	{
		Opcode op;
		switch ( e.Op )
		{
		case BinaryOp.Add:				op = Opcode.Add;	break;
		case BinaryOp.Subtract:			op = Opcode.Sub;	break;
		case BinaryOp.Multiply:			op = Opcode.Mul;	break;
		case BinaryOp.Divide:			op = Opcode.Div;	break;
		case BinaryOp.IntegerDivide:	op = Opcode.IntDiv;	break;
		case BinaryOp.Modulus:			op = Opcode.Mod;	break;
		case BinaryOp.RaiseToPower:		op = Opcode.Pow;	break;
		default: throw new ArgumentException();
		}

		Allocation left		= RK( e.Left );
		Allocation right	= RK( e.Right );
		builder.InstructionABC( op, target, left, right );
		right.Release();
		left.Release();
	}

	public void Visit( Call e )
	{
		Allocation result = BuildCall( 2, e.Function, null, e.Arguments, e.ArgumentValues );
		if ( (int)target != (int)result )
		{
			builder.InstructionABC( Opcode.Move, target, result, 0 );
		}
		result.Release();
	}

	public void Visit( CallSelf e )
	{
		Allocation result = BuildCall( 2, e.Object, e.MethodName, e.Arguments, e.ArgumentValues );
		if ( (int)target != (int)result )
		{
			builder.InstructionABC( Opcode.Move, target, result, 0 );
		}
		result.Release();
	}

	Allocation BuildCall( int C,
			Expression functionOrObject, string methodName,
			IList< Expression > arguments, Expression argumentValues )
	{
		// Push function (or method) onto the stack.
		Allocation allocation = builder.Top();
		if ( methodName == null )
		{
			Allocation allocFunction = R( functionOrObject );
			allocFunction.Release();
			allocation.Allocate();
		}
		else
		{
			Allocation allocObject = R( functionOrObject );
			Allocation allocMethod = builder.ConstantRK( methodName );
			builder.InstructionABC( Opcode.Self, allocation, allocObject, allocMethod );
			allocMethod.Release();
			allocObject.Release();
			allocation.Allocate();
		}

		// Push arguments onto the stack.
		foreach ( Expression argument in arguments )
		{
			Allocation allocArgument = Push( argument );
			allocArgument.Release();
			allocation.Allocate();
		}

		// Push variable arguments onto the stack.
		int B;
		if ( argumentValues != null )
		{
			Allocation allocValues = SetTop( argumentValues );
			allocValues.Release();
			allocation.SetTop();
			B = 0;
		}
		else
		{
			B = arguments.Count + 1;
		}

		// Call.
		builder.InstructionABC( Opcode.Call, allocation, B, C );
		allocation.Release();

		// Return appropriate number of values.
		Allocation results = builder.Top();
		if ( C > 0 )
		{
			results.Allocate( C - 1 );
		}
		else
		{
			results.SetTop();
		}
		return results;
	}

	public void Visit( Comparison e )
	{
		// Convert a branch into a value.
		LabelAST returnTrue = new LabelAST( "returnTrue" );
		Branch( e, true, returnTrue );
		builder.InstructionABC( Opcode.LoadBool, target, 0, 1 );
		builder.Label( returnTrue );
		builder.InstructionABC( Opcode.LoadBool, target, 1, 0 );
	}

	public void Visit( FunctionClosure e )
	{
		// Compile function and reference it.
		Builder prototypeBuilder = BuildPrototype( e.Function );
		builder.InstructionABx( Opcode.Closure, target, builder.Prototype( prototypeBuilder ) );

		// Initialize upvals.
/*		foreach ( UpValLocator locator in builder.BuildUpValLocators() )
		{
			if ( locator.Source == UpValSource.Local )
			{
				builder.InstructionABC( Opcode.Move, locator.TargetIndex, locator.SourceIndex, 0 );
			}
			else if ( locator.Source == UpValSource.UpVal )
			{
				builder.InstructionABC( Opcode.GetUpVal, locator.TargetIndex, locator.SourceIndex, 0 );
			}
		}
*/
	}

	public void Visit( GlobalRef e )
	{
		builder.InstructionABx( Opcode.GetGlobal, target, builder.Constant( e.Name ) );
	}

	public void Visit( Index e )
	{
		Allocation table	= R( e.Table );
		Allocation key		= RK( e.Key );
		builder.InstructionABC( Opcode.GetTable, target, table, key );
		key.Release();
		table.Release();
	}

	public void Visit( Literal e )
	{
		builder.InstructionABx( Opcode.LoadK, target, builder.Constant( e.Value ) );
	}

	public void Visit( LocalRef e )
	{
		Allocation local = builder.Local( e.Variable );
		if ( target != local )
		{
			builder.InstructionABC( Opcode.Move, target, local, 0 );
		}
		local.Release();
	}

	public void Visit( Logical e )
	{
		// Perform shortcut evaluation.
		LabelAST shortcutEvaluation = new LabelAST( "shortcutEvaluation" );
		Allocation left = R( e.Left );
		builder.InstructionABC( Opcode.TestSet, target, left, e.Op == LogicalOp.Or ? 1 : 0 );
		builder.InstructionAsBx( Opcode.Jmp, 0, shortcutEvaluation );
		left.Release();
		e.Right.Accept( this );
		builder.Label( shortcutEvaluation );
	}

	public void Visit( Not e )
	{
		Allocation operand = R( e.Operand );
		builder.InstructionABC( Opcode.Not, target, operand, 0 );
		operand.Release();
	}

	public void Visit( OpcodeConcat e )
	{
		// Get operand list.
		Allocation allocation = builder.Top();
		foreach ( Expression operand in e.Operands )
		{
			Allocation allocOperand = Push( operand );
			allocOperand.Release();
			allocation.Allocate();
		}

		// Instruction.
		builder.InstructionABC( Opcode.Concat, target, allocation, allocation + e.Operands.Count - 1 );
		allocation.Release();
	}

	public void Visit( Temporary e )
	{
		Allocation temporary = builder.Temporary( e );
		if ( target != temporary )
		{
			builder.InstructionABC( Opcode.Move, target, temporary, 0 );
		}
		temporary.Release();
		builder.RetireTemporary( e );
	}

	public void Visit( TemporaryList e )
	{
		throw new InvalidOperationException();
	}

	public void Visit( ToNumber e )
	{
		throw new InvalidOperationException();
	}

	public void Visit( Unary e )
	{
		Opcode op;
		switch ( e.Op )
		{
		case UnaryOp.Minus:		op = Opcode.Unm;	break;
		case UnaryOp.Length:	op = Opcode.Len;	break;
		default: throw new ArgumentException();
		}

		Allocation operand = R( e.Operand );
		builder.InstructionABC( op, target, operand, 0 );
		operand.Release();
	}

	public void Visit( UpValRef e )
	{
		// The block where the upval was declared needs to be closed.
		for ( BlockBuilder outer = blockBuilder; outer != null; outer = outer.Parent )
		{
			if ( outer.Block == e.Variable.Block )
			{
				outer.SetNeedsClose();
				break;
			}
		}

		// Get upval.
		builder.InstructionABC( Opcode.GetUpVal, target, builder.UpVal( e.Variable ), 0 );
	}

	public void Visit( ValueList e )
	{
		throw new InvalidOperationException();
	}

	public void Visit( ValueListElement e )
	{
		throw new InvalidOperationException();
	}

	public void Visit( Vararg e )
	{
		builder.InstructionABC( Opcode.Vararg, target, 1, 0 );
	}

}


}
