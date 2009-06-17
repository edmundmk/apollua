// LuaCLRCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lua;
using Lua.VM;
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


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
	int						target;



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
		public Builder	Builder		{ get; private set; }
		public int		Value		{ get; private set; }
		public int		Count		{ get; private set; }


		public Allocation( int value )
		{
			Builder	= null;
			Value	= value;
		}

		public Allocation( Builder builder )
		{
			Builder	= builder;
			Value	= builder.Top;
			Count	= 0;
		}

		public Allocation( Builder builder, int value )
		{
			Builder	= builder;
			Value	= value;
			Count	= 1;
		}

		public void Push()
		{
			Debug.Assert( Builder != null );
			Debug.Assert( Value + Count == Builder.Top );
			Builder.Allocate();
			Count += 1;
		}

		public void Release()
		{
			if ( Builder != null )
			{
				Builder.Release( Count );
				Count = 0;
			}
		}

		public static implicit operator int( Allocation a )
		{
			return a.Value;
		}
	}

	
	enum UpValSource
	{
		Local,
		UpVal
	}

	struct UpValBuilder
	{
		public int			TargetIndex		{ get; private set; }
		public UpValSource	Source			{ get; private set; }
		public int			SourceIndex		{ get; private set; }
	}


	class LabelBuilder
	{
		public Builder		Builder			{ get; private set; }
		public IList< int >	PatchOffsets	{ get; private set; }
		public int			LabelOffset		{ get; private set; }


		public LabelBuilder( Builder builder )
		{
		}
		
	}

	
	class Builder
	{
		public Builder							Parent			{ get; private set; }
		public FunctionAST						FunctionAST		{ get; private set; }
		public UpValBuilder[]					UpValLocators	{ get; private set; }
		public IList< Variable >				Locals			{ get; private set; }
		public IDictionary< Temporary, int >	Temporaries		{ get; private set; }
		public int								Top				{ get; private set; }


		// Register allocation.

		public Allocation Allocate()
		{
			return new Allocation( this );
		}

		public Allocation LocalRef( Variable variable )
		{
			return new Allocation( this );
		}

		public Allocation TemporaryRef( Temporary temporary )
		{
			return new Allocation( this );
		}

		public void Release( int count )
		{
		}


		// Upvals.

		public int UpVal( Variable upval )
		{
			return 0;
		}


		// Constants.

		public int Constant( object constant )
		{
			return 0;
		}
		

		// Prototypes.

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

		public void Label( LabelBuilder label )
		{
		}

		public void InstructionAsBx( Opcode opcode, int A, LabelBuilder label )
		{
		}



	}




	// Expression evaluation.

	void Move( int r, Expression e )
	{
		int oldtarget = target;
		target = r;
		e.Accept( this );
		target = oldtarget;
	}

	void Push( Allocation allocation, Expression e )
	{
		Move( builder.Top, e );
		allocation.Push();
	}

	Allocation R( Expression e )
	{
		if ( e is LocalRef )
		{
			return builder.LocalRef( ( (LocalRef)e ).Variable );
		}
		else if ( e is Temporary )
		{
			return builder.TemporaryRef( (Temporary)e );
		}

		Allocation a = new Allocation( builder );
		Push( a, e );
		return a;
	}

	Allocation RK( Expression e )
	{
		if ( e is Literal )
		{
			int k = builder.Constant( ( (Literal)e ).Value );
			if ( Instruction.InRangeRK( k ) )
			{
				return new Allocation( Instruction.ConstantToRK( k ) );
			}
		}

		return R( e );
	}

	void Branch( Expression e, bool ifTrue, LabelBuilder label )
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
					LabelBuilder noBranch = new LabelBuilder( builder );
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
					LabelBuilder noBranch = new LabelBuilder( builder );
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
		throw new NotImplementedException();
	}

	public void Visit( Block s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Branch s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Constructor s )
	{
		throw new NotImplementedException();
	}

	public void Visit( Declare s )
	{
		throw new NotImplementedException();
	}

	public void Visit( DeclareForIndex s )
	{
		throw new NotImplementedException();
	}
	
	public void Visit( Evaluate s )
	{
		throw new NotImplementedException();
	}

	public void Visit( IndexMultipleValues s )
	{
		throw new NotImplementedException();
	}

	public void Visit( MarkLabel s )
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	public void Visit( ReturnMultipleValues s )
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	public void Visit( CallSelf e )
	{
		throw new NotImplementedException();
	}

	public void Visit( Comparison e )
	{
		// Convert a branch into a value.
		LabelBuilder returnTrue = new LabelBuilder( builder );
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
		foreach ( UpValBuilder locator in builder.UpValLocators )
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
		builder.InstructionABC( Opcode.Move, target, builder.LocalRef( e.Variable ), 0 );
	}

	public void Visit( Logical e )
	{
		// Perform shortcut evaluation.
		LabelBuilder shortcutEvaluation = new LabelBuilder( builder );
		Allocation left = R( e.Left );
		builder.InstructionABC( Opcode.TestSet, target, left, e.Op == LogicalOp.Or ? 1 : 0 );
		builder.InstructionAsBx( Opcode.Jmp, 0, shortcutEvaluation );
		left.Release();
		Move( target, e.Right );
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
		Allocation operands = new Allocation( builder );
		for ( int operand = 0; operand < e.Operands.Count; ++operand )
		{
			Push( operands, e.Operands[ operand ] );
		}

		// Instruction.
		builder.InstructionABC( Opcode.Concat, target, operands, operands.Value + operands.Count - 1 );
		operands.Release();
	}

	public void Visit( Temporary e )
	{
		builder.InstructionABC( Opcode.Move, target, builder.TemporaryRef( e ), 0 );
	}

	public void Visit( ToNumber e )
	{
		throw new NotImplementedException();
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
		builder.InstructionABC( Opcode.GetUpVal, target, builder.UpVal( e.Variable ), 0 );
	}

	public void Visit( ValueList e )
	{
		throw new NotImplementedException();
	}

	public void Visit( ValueListElement e )
	{
	}

	public void Visit( Vararg e )
	{
		builder.InstructionABC( Opcode.Vararg, target, 1, 0 );
	}

}


}
