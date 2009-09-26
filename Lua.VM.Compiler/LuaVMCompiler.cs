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
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;
using Lua.VM.Compiler.AST;
using Lua.VM.Compiler.AST.Expressions;



namespace Lua.VM.Compiler
{


/*	Each function is compiled to a class deriving from Lua.Function.
*/


public class LuaVMCompiler
	:	IStatementVisitor
	,	IVMExpressionVisitor
{
	// Errors.

	TextWriter				errorWriter;
	bool					hasError;


	// Parser.

	string					sourceName;
	LuaParser				parser;


	// Prototype building.

	FunctionBuilder			function;
	BlockBuilder			block;
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


	public VMFunction Compile()
	{
		// Parse the function.

		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}


		// Transform.

		VMTransform transform = new VMTransform();
		functionAST = transform.Transform( functionAST );


		// Compile.

		Prototype prototype = BuildPrototype( functionAST );
		return new VMFunction( prototype );
	}


	Prototype BuildPrototype( FunctionAST functionAST )
	{
		// Enter block.
		block = new BlockBuilder( block, functionAST.Block, 0 );

		// Enter function.
		function = new FunctionBuilder( function, functionAST, block );

		for ( int parameter = 0; parameter < functionAST.Parameters.Count; ++parameter )
		{
			function.DeclareLocal( functionAST.Parameters[ parameter ] );
		}

		BuildBlock( functionAST.Block );

		for ( int parameter = functionAST.Parameters.Count - 1; parameter >= 0; --parameter )
		{
			function.RetireLocal( functionAST.Parameters[ parameter ] );
		}

		// End function.
		Prototype prototype = function.Build();
		function = function.Parent;

		// End of block.
		block = block.Parent;

		return prototype;
	}




	// Builder.

	struct Allocation
	{
		FunctionBuilder	function;
		int				value;
		int				count;
		
		public Allocation( int register )
		{
			function	= null;
			value		= register;
			count		= 1;
		}

		public Allocation( FunctionBuilder f, int top )
		{
			function	= f;
			value		= top;
			count		= 0;
		}

		public void Allocate()
		{
			Allocate( 1 );
		}

		public void Allocate( int allocate )
		{
			if ( function == null )
				throw new InvalidOperationException();

			count += allocate;
			function.Allocate( allocate );
		}

		public void SetTop()
		{
		}

		public void Release()
		{
			if ( function != null )
			{
				function.Release( count );
			}
		}

		public static implicit operator int ( Allocation a )
		{
			return a.value;
		}
	}

	class FunctionBuilder
	{
		public FunctionBuilder	Parent			{ get; private set; }
		public FunctionAST		Function		{ get; private set; }

		class Patch
		{
			public Patch	Previous		{ get; private set; }
			public int		Instruction		{ get; private set; }

			public Patch( Patch previous, int instruction )
			{
				Previous	= previous;
				Instruction	= instruction;
			}
		}

		List< object >					constants;
		List< Prototype >				prototypes;
		List< Instruction >				instructions;
		List< DebugSourceSpan >			instructionSourceSpans;

		Dictionary< LabelAST, int >		labels;
		Dictionary< LabelAST, Patch >	patchLabels;

		Dictionary< Variable, int >		locals;
		List< DebugLocal >				debugLocals;

		int								localTop;
		int								top;
		int								watermark;
		

		public FunctionBuilder( FunctionBuilder parent, FunctionAST function, BlockBuilder functionBlock )
		{
			Parent					= parent;
			Function				= function;

			constants				= new List< object >();
			prototypes				= new List< Prototype >();
			instructions			= new List< Instruction >();
			instructionSourceSpans	= new List< DebugSourceSpan >();

			labels					= new Dictionary< LabelAST, int >();
			patchLabels				= new Dictionary< LabelAST, Patch >();

			locals					= new Dictionary< Variable, int >();
			debugLocals				= new List< DebugLocal >();

			localTop				= 0;
			top						= 0;
			watermark				= 0;
		}
		

		// Locals.

		public bool DeclareLocal( Variable local )
		{
			if ( localTop != top )
				throw new InvalidOperationException();

			locals[ local ] = localTop;
			bool bInitialize = localTop < watermark;
			Allocate( 1 );
			localTop += 1;

			debugLocals.Add( new DebugLocal( local.Name, instructions.Count, -1 ) );
		
			return bInitialize;
		}

		public void RetireLocal( Variable local )
		{
			if ( localTop != top || locals[ local ] != localTop - 1 )
				throw new InvalidOperationException();

			locals.Remove( local );
			localTop -= 1;
			Release( 1 );

			for ( int debugLocal = debugLocals.Count - 1; debugLocal >= 0; --debugLocal )
			{
				DebugLocal debug = debugLocals[ debugLocal ];
				if ( debug.EndInstruction == -1 )
				{
					debugLocals[ debugLocal ] = new DebugLocal(
						debug.Name, debug.StartInstruction, instructions.Count );
					break;
				}
			}
		}

		public Allocation Local( Variable local )
		{
			return new Allocation( locals[ local ] );
		}


		// Allocations.

		public Allocation Top()
		{
			return new Allocation( this, top );
		}

		public void Allocate( int count )
		{
			top += count;
			if ( watermark < top )
				watermark = top;
		}

		public void Release( int count )
		{
			top -= count;
		}
		

		// RK values.

		public Allocation ConstantRK( SourceSpan s, object constant )
		{
			int k = Constant( constant );
			if ( Instruction.InRangeRK( k ) )
			{
				return new Allocation( Instruction.ConstantToRK( k ) );
			}
			else
			{
				Allocation r = Top();
				InstructionABx( s, Opcode.LoadK, r, k );
				r.Allocate();
				return r;
			}
		}

		
		// Values referenced by special instructions.

		public int UpVal( Variable upval )
		{
			return Function.UpVals.IndexOf( upval );
		}

		public int Constant( object constant )
		{
			int k = constants.IndexOf( constant );
			if ( k == -1 )
			{
				k = constants.Count;
				constants.Add( constant );
			}
			return k;
		}

		public int Prototype( Prototype prototype )
		{
			int p = prototypes.IndexOf( prototype );
			if ( p == -1 )
			{
				p = prototypes.Count;
				prototypes.Add( prototype );
			}
			return p;
		}
		

		// Opcodes.

		public void InstructionABC( SourceSpan s, Opcode opcode, int A, int B, int C )
		{
			instructions.Add( Instruction.CreateABC( opcode, A, B, C ) );
			instructionSourceSpans.Add( ConvertSourceSpan( s ) );
		}

		public void InstructionABx( SourceSpan s, Opcode opcode, int A, int Bx )
		{
			instructions.Add( Instruction.CreateABx( opcode, A, Bx ) );
			instructionSourceSpans.Add( ConvertSourceSpan( s ) );
		}

		public void InstructionSetList( SourceSpan s, int A, int B, int C )
		{
			if ( Instruction.InRangeC( C ) )
			{
				instructions.Add( Instruction.CreateABC( Opcode.SetList, A, B, C ) );
				instructionSourceSpans.Add( ConvertSourceSpan( s ) );
			}
			else
			{
				instructions.Add( Instruction.CreateABC( Opcode.SetList, A, B, 0 ) );
				instructionSourceSpans.Add( ConvertSourceSpan( s ) );
				instructions.Add( Instruction.CreateIndex( C ) );
				instructionSourceSpans.Add( ConvertSourceSpan( s ) );
			}
		}

		public void Label( LabelAST label )
		{
			if ( labels.ContainsKey( label ) )
				throw new InvalidOperationException();

			labels[ label ] = instructions.Count;
			Patch patch;
			patchLabels.TryGetValue( label, out patch );
			while ( patch != null )
			{
				Instruction instruction = instructions[ patch.Instruction ];
				instructions[ patch.Instruction ] =
					Instruction.CreateAsBx( instruction.Opcode, instruction.A, instructions.Count - ( patch.Instruction + 1 ) );
				patch = patch.Previous;
			}
			patchLabels.Remove( label );
		}

		public void InstructionAsBx( SourceSpan s, Opcode opcode, int A, LabelAST label )
		{
			if ( labels.ContainsKey( label ) )
			{
				int offset = labels[ label ] - ( instructions.Count + 1 );
				instructions.Add( Instruction.CreateAsBx( opcode, A, offset ) );
				instructionSourceSpans.Add( ConvertSourceSpan( s ) );
			}
			else
			{
				Patch patch;
				patchLabels.TryGetValue( label, out patch );
				patchLabels[ label ] = new Patch( patch, instructions.Count );
				instructions.Add( Instruction.CreateAsBx( opcode, A, -1 ) );
				instructionSourceSpans.Add( ConvertSourceSpan( s ) );
			}
		}
		

		// Construct prototype.

		DebugSourceSpan ConvertSourceSpan( SourceSpan s )
		{
			return new DebugSourceSpan(
				new DebugSourceLocation( s.Start.SourceName, s.Start.Line, s.Start.Column ),
				new DebugSourceLocation( s.End.SourceName, s.End.Line, s.End.Column ) );
		}

		LuaValue ObjectToValue( object o )
		{
			if ( o == null )
			{
				return null;
			}
			else if ( o is bool )
			{
				return (bool)o;
			}
			else if ( o is int )
			{
				return (int)o;
			}
			else if ( o is double )
			{
				return (double)o;
			}
			else if ( o is string )
			{
				return (string)o;
			}

			throw new ArgumentException();
		}

		public Prototype Build()
		{
			Prototype prototype = new Prototype();
			
			prototype.UpValCount					= Function.UpVals.Count;
			prototype.ParameterCount				= Function.Parameters.Count;
			prototype.IsVararg						= Function.IsVararg;

			prototype.Constants						= new LuaValue[ constants.Count ];
			prototype.Prototypes					= prototypes.ToArray();

			prototype.StackSize						= watermark;
			prototype.Instructions					= instructions.ToArray();

			prototype.DebugName						= Function.Name;
			prototype.DebugSourceSpan				= ConvertSourceSpan( Function.Block.SourceSpan );
			prototype.DebugInstructionSourceSpans	= instructionSourceSpans.ToArray();
			prototype.DebugUpValNames				= new string[ Function.UpVals.Count ];
			prototype.DebugLocals					= debugLocals.ToArray();

			for ( int constant = 0; constant < constants.Count; ++constant )
			{
				prototype.Constants[ constant ] = ObjectToValue( constants[ constant ] );
			}

			for ( int upval = 0; upval < Function.UpVals.Count; ++upval )
			{
				prototype.DebugUpValNames[ upval ] = Function.UpVals[ upval ].Name;
			}

			return prototype;
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

	void Close( SourceSpan s, Block closeBlock )
	{
		bool needsClose = false;
		int	 closeBase  = 0;

		for ( BlockBuilder outer = block; outer != null && outer.Block != closeBlock; outer = outer.Parent )
		{
			needsClose |= outer.NeedsClose;
			closeBase = outer.Base;
		}

		if ( needsClose )
		{
			function.InstructionABC( s, Opcode.Close, closeBase, 0, 0 );
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
		Allocation allocation = function.Top();
		Move( allocation, e );
		allocation.Allocate();
		return allocation;
	}

	Allocation R( Expression e )
	{
		if ( e is LocalRef )
		{
			return function.Local( ( (LocalRef)e ).Variable );
		}

		return Push( e );
	}

	Allocation RK( Expression e )
	{
		if ( e is Literal )
		{
			return function.ConstantRK( e.SourceSpan, ( (Literal)e ).Value ); 
		}

		return R( e );
	}

	Allocation PushList( Expression e, int count )
	{
		Allocation results;

		if ( e is Call )
		{
			Call call = (Call)e;
			results = BuildCall( count + 1, call.Function, null, call.Arguments, call.ArgumentValues );
		}
		else if ( e is CallSelf )
		{
			CallSelf call = (CallSelf)e;
			results = BuildCall( count + 1, call.Object, call.MethodName, call.Arguments, call.ArgumentValues );
		}
		else if ( e is Vararg )
		{
			results = function.Top();
			function.InstructionABC( e.SourceSpan, Opcode.Vararg, results, count, 0 );
			results.Allocate( count );
		}
		else
		{
			throw new InvalidOperationException();
		}

		return results;
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
			results = function.Top();
			function.InstructionABC( e.SourceSpan, Opcode.Vararg, results, 0, 0 );
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
				A = A != 0 ? 0 : 1;
			}

			Allocation left		= RK( comparison.Left );
			Allocation right	= RK( comparison.Right );
			function.InstructionABC( e.SourceSpan, op, A, left, right );
			function.InstructionAsBx( e.SourceSpan, Opcode.Jmp, 0, label );
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
					function.Label( noBranch );
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
					function.Label( noBranch );
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
			function.InstructionABC( e.SourceSpan, Opcode.Test, expression, 0, ifTrue ? 1 : 0 );
			function.InstructionAsBx( e.SourceSpan, Opcode.Jmp, 0, label );
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
			int constant		= function.Constant( global.Name );
			Allocation value	= R( s.Value );
			function.InstructionABx( s.SourceSpan, Opcode.SetGlobal, value, constant );
			value.Release();
		}
		else if ( s.Target is Index )
		{
			// Assign to table index.
			Index index = (Index)s.Target;
			Allocation table	= R( index.Table );
			Allocation key		= RK( index.Key );
			Allocation value	= RK( s.Value );
			function.InstructionABC( s.SourceSpan, Opcode.SetTable, table, key, value );
			value.Release();
			key.Release();
			table.Release();
		}
		else if ( s.Target is LocalRef )
		{
			// Assign to local.
			LocalRef local = (LocalRef)s.Target;
			Allocation target = function.Local( local.Variable );
			Move( target, s.Value );
			target.Release();
		}
		else if ( s.Target is UpValRef )
		{
			// Assign to upval.
			UpValRef upval = (UpValRef)s.Target;
			int index = function.UpVal( upval.Variable );
			Allocation value = R( s.Value );
			function.InstructionABC( s.SourceSpan, Opcode.SetUpVal, value, index, 0 );
			value.Release();
		}
		else 
		{
			throw new InvalidOperationException();
		}
	}

	class IndexRef
		:	Expression
	{
		public int Table	{ get; private set; }
		public int Key		{ get; private set; }

		public IndexRef( SourceSpan s, int table, int key )
			:	base( s )
		{
			Table	= table;
			Key		= key;
		}

		public override void Accept( IExpressionVisitor v )
		{
			throw new NotSupportedException( "IndexRefs are internal compiler objects." );
		}
	}

	public void Visit( AssignList s )
	{
		// Evaluate subexpressions for index targets.
		Allocation allocTargets = function.Top();
		Expression[] targets = new Expression[ s.Targets.Count ];
		for ( int target = 0; target < s.Targets.Count; ++target )
		{
			Expression e = s.Targets[ target ];
			Index index = e as Index;
			if ( index != null )
			{
				Allocation table = R( index.Table );
				Allocation key = RK( index.Key );

				targets[ target ] = new IndexRef( index.SourceSpan, table, key );

				key.Release();
				table.Release();
				allocTargets.Allocate( 2 );
			}
			else
			{
				targets[ target ] = e;
			}
		}

		// Evaluate all values.
		Allocation allocValues = function.Top();
		for ( int value = 0; value < s.Values.Count; ++value )
		{
			Allocation allocValue = Push( s.Values[ value ] );
			allocValue.Release();
			allocValues.Allocate();
		}
		if ( s.ValueList != null )
		{
			if ( s.Targets.Count <= s.Values.Count )
				throw new ArgumentException();

			Allocation allocValueList = PushList( s.ValueList, s.Targets.Count - s.Values.Count );
			allocValueList.Release();
			allocValues.Allocate( s.Targets.Count - s.Values.Count );
		}

		// Assign each value.
		for ( int target = 0; target < targets.Length; ++target )
		{
			if ( targets[ target ] is GlobalRef )
			{
				GlobalRef global = (GlobalRef)targets[ target ];
				function.InstructionABx( s.SourceSpan, Opcode.SetGlobal,
					allocValues + target, function.Constant( global.Name ) );
			}
			else if ( targets[ target ] is IndexRef )
			{
				IndexRef index = (IndexRef)targets[ target ];
				function.InstructionABC( s.SourceSpan, Opcode.SetTable,
					index.Table, index.Key, allocValues + target );
			}
			else if ( targets[ target ] is LocalRef )
			{
				LocalRef local = (LocalRef)targets[ target ];
				function.InstructionABC( s.SourceSpan, Opcode.Move,
					function.Local( local.Variable ), allocValues + target, 0 );
			}
			else if ( targets[ target ] is UpValRef )
			{
				UpValRef upval = (UpValRef)targets[ target ];
				function.InstructionABC( s.SourceSpan, Opcode.SetUpVal,
					allocValues + target, function.UpVal( upval.Variable ), 0 );
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		// Release.
		allocValues.Release();
		allocTargets.Release();
	}

	public void Visit( Block s )
	{
		// Enter block.
		Allocation blockBase = function.Top();
		block = new BlockBuilder( block, s, blockBase );
		blockBase.Release();

		BuildBlock( s );

		// End of block.
		block = block.Parent;
	}

	void BuildBlock( Block s )
	{
		// Build statements.
		foreach ( Statement statement in s.Statements )
		{
			statement.Accept( this );
		}

		// Close block if necessary.
		if ( block.NeedsClose && ( block.Block != function.Function.Block ) )
		{
			function.InstructionABC( s.SourceSpan, Opcode.Close, block.Base, 0, 0 );
		}
		
		// Retire locals.
		for ( int local = s.Locals.Count - 1; local >= 0; --local )
		{
			function.RetireLocal( s.Locals[ local ] );
		}
	}

	public void Visit( Branch s )
	{
		// If we are jumping out of the block, close upvals.
		Close( s.SourceSpan, s.Target.Block );

		// Jump.
		function.InstructionAsBx( s.SourceSpan, Opcode.Jmp, 0, s.Target );
	}

	public void Visit( Declare s )
	{
		bool bInitialize = function.DeclareLocal( s.Variable );
		Allocation local = function.Local( s.Variable );
		Literal literal = s.Value as Literal;
		if ( bInitialize || literal == null || literal.Value != null )
		{
			Move( local, s.Value );
		}
		local.Release();
	}

	public void Visit( DeclareList s )
	{
		if ( s.ValueList == null )
			throw new ArgumentException();

		Allocation values = PushList( s.ValueList, s.Variables.Count );
		values.Release();

		// Declaring these variables on the top of the stack should give them all the
		// correct values.
		for ( int local = 0; local < s.Variables.Count; ++local )
		{
			function.DeclareLocal( s.Variables[ local ] );
		}
	}

	public void Visit( Evaluate s )
	{
		// Evaluate and throw away result.
		Allocation allocation = R( s.Expression );
		allocation.Release();
	}

	public void Visit( ForBlock s )
	{
		/*		forprep (for index), (for limit), (for step) forContinue
			forLoop:
				block
					local <index>
					...
				end
			forContinue:
				forloop (for index), (for limit), (for step), <index> forLoop
			forBreak:
		*/

		// For index variables should have been allocated sequentially.
		Allocation allocation = function.Local( s.Index );
		int A = allocation;
		allocation.Release();

		// Prologue.
		function.InstructionAsBx( s.SourceSpan, Opcode.ForPrep, A, s.ContinueLabel );
		LabelAST forLoop = new LabelAST( "forLoop" );
		function.Label( forLoop );
	
		// Enter block.
		Allocation blockBase = function.Top();
		block = new BlockBuilder( block, s, blockBase );
		blockBase.Release();
		
		function.DeclareLocal( s.UserIndex );
		BuildBlock( s );

		// End of block.
		block = block.Parent;

		// Epilogue.
		function.Label( s.ContinueLabel );
		function.InstructionAsBx( s.SourceSpan, Opcode.ForLoop, A, forLoop );
		function.Label( s.BreakLabel );
	}

	public void Visit( ForListBlock s )
	{
		/*		b forlistContinue
			forlistLoop:
				block
					local <variablelist>
					...
				end
			forlistContinue:
				tforloop (for generator), (for state), (for control), <variablelist> forlistLoop
			forlistBreak:
		*/

		// For index variables should have been allocated sequentially.
		Allocation allocation = function.Local( s.Generator );
		int A = allocation;
		allocation.Release();

		// Prologue.
		function.InstructionAsBx( s.SourceSpan, Opcode.Jmp, 0, s.ContinueLabel );
		LabelAST forlistLoop = new LabelAST( "forlistLoop" );
		function.Label( forlistLoop );
	
		// Enter block.
		Allocation blockBase = function.Top();
		block = new BlockBuilder( block, s, blockBase );
		blockBase.Release();

		for ( int variable = 0; variable < s.UserVariables.Count; ++variable )
		{
			function.DeclareLocal( s.UserVariables[ variable ] );
		}

		BuildBlock( s );

		// Epilogue.
		function.Label( s.ContinueLabel );
		function.InstructionAsBx( s.SourceSpan, Opcode.TForLoop, A, forlistLoop );
		function.Label( s.BreakLabel );

		// End of block.
		block = block.Parent;
	}

	public void Visit( MarkLabel s )
	{
		function.Label( s.Label );
	}

	public void Visit( Return s )
	{
		// Close upvals.
		Close( s.SourceSpan, function.Function.Block.Parent );

		// Return.
		if ( ( s.Result is Literal ) && ( ( (Literal)s.Result ).Value == null ) )
		{
			function.InstructionABC( s.SourceSpan, Opcode.Return, 0, 1, 0 );
		}
		else
		{
			Allocation result = R( s.Result );
			function.InstructionABC( s.SourceSpan, Opcode.Return, result, 2, 0 );
			result.Release();
		}
	}

	public void Visit( ReturnList s )
	{
		// Close upvals.
		Close( s.SourceSpan, function.Function.Block.Parent );

		// Push results onto the stack.
		Allocation allocation = function.Top();
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
		function.InstructionABC( s.SourceSpan, Opcode.Return, allocation, B, 0 );
		allocation.Release();
	}

	public void Visit( Test s )
	{
		Branch( s.Condition, false, s.Target );
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
		function.InstructionABC( e.SourceSpan, op, target, left, right );
		right.Release();
		left.Release();
	}

	public void Visit( Call e )
	{
		Allocation result = BuildCall( 2, e.Function, null, e.Arguments, e.ArgumentValues );
		if ( (int)target != (int)result )
		{
			function.InstructionABC( e.SourceSpan, Opcode.Move, target, result, 0 );
		}
		result.Release();
	}

	public void Visit( CallSelf e )
	{
		Allocation result = BuildCall( 2, e.Object, e.MethodName, e.Arguments, e.ArgumentValues );
		if ( (int)target != (int)result )
		{
			function.InstructionABC( e.SourceSpan, Opcode.Move, target, result, 0 );
		}
		result.Release();
	}

	Allocation BuildCall( int C,
			Expression functionOrObject, string methodName,
			IList< Expression > arguments, Expression argumentValues )
	{
		// Push function (or method) onto the stack.
		Allocation allocation = function.Top();
		if ( methodName == null )
		{
			Allocation allocFunction = Push( functionOrObject );
			allocFunction.Release();
			allocation.Allocate();
		}
		else
		{
			Allocation allocObject = R( functionOrObject );
			Allocation allocMethod = function.ConstantRK( functionOrObject.SourceSpan, methodName );
			function.InstructionABC( functionOrObject.SourceSpan, Opcode.Self, allocation, allocObject, allocMethod );
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
		function.InstructionABC( functionOrObject.SourceSpan, Opcode.Call, allocation, B, C );
		allocation.Release();

		// Return appropriate number of values.
		Allocation results = function.Top();
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
		function.InstructionABC( e.SourceSpan, Opcode.LoadBool, target, 0, 1 );
		function.Label( returnTrue );
		function.InstructionABC( e.SourceSpan, Opcode.LoadBool, target, 1, 0 );
	}

	public void Visit( Constructor e )
	{
		// Construct.
		Allocation table = function.Top();
		function.InstructionABC( e.SourceSpan, Opcode.NewTable, table, e.ArrayCount, e.HashCount );
		table.Allocate();

		// Initialize.
		Allocation pending = function.Top();
		int pendingCount	= 0;
		int batchCount		= 0;

		foreach ( ConstructorElement element in e.Elements )
		{
			if ( element.HashKey == null )
			{
				Move( pending, element.Value );
				pending.Allocate();
				pendingCount += 1;

				if ( pendingCount == Instruction.FieldsPerFlush )
				{
					function.InstructionSetList( e.SourceSpan, table, pendingCount, batchCount + 1 );
					pending.Release();
					pending = function.Top();
					pendingCount = 0;
					batchCount += 1;
				}
			}
			else
			{
				Allocation key		= RK( element.HashKey );
				Allocation value	= RK( element.Value );
				function.InstructionABC( e.SourceSpan, Opcode.SetTable, table, key, value );
				value.Release();
				key.Release();
			}
		}

		if ( e.ElementList != null )
		{
			Allocation allocList = SetTop( e.ElementList );
			allocList.Release();
			pending.SetTop();

			function.InstructionSetList( e.SourceSpan, table, 0, batchCount + 1 );
			pending.Release();
			pendingCount = 0;
		}

		if ( pendingCount > 0 )
		{
			function.InstructionSetList( e.SourceSpan, table, pendingCount, batchCount + 1 );
			pending.Release();
			pendingCount = 0;
		}

		// Move table.
		if ( (int)target != (int)table )
		{
			function.InstructionABC( e.SourceSpan, Opcode.Move, target, table, 0 );
		}
		table.Release();
	}

	public void Visit( FunctionClosure e )
	{
		// Compile function and reference it.
		Prototype prototype = BuildPrototype( e.Function );
		function.InstructionABx( e.SourceSpan, Opcode.Closure, target, function.Prototype( prototype ) );

		// Initialize upvals.
		for ( int upval = 0; upval < e.Function.UpVals.Count; ++upval )
		{
			Variable variable = e.Function.UpVals[ upval ];
			if ( function.Function.UpVals.Contains( variable ) )
			{
				function.InstructionABC( e.SourceSpan, Opcode.GetUpVal, upval, function.UpVal( variable ), 0 );
			}
			else
			{
				function.InstructionABC( e.SourceSpan, Opcode.Move, upval, function.Local( variable ), 0 );
			}
		}
	}

	public void Visit( GlobalRef e )
	{
		function.InstructionABx( e.SourceSpan, Opcode.GetGlobal, target, function.Constant( e.Name ) );
	}

	public void Visit( Index e )
	{
		Allocation table	= R( e.Table );
		Allocation key		= RK( e.Key );
		function.InstructionABC( e.SourceSpan, Opcode.GetTable, target, table, key );
		key.Release();
		table.Release();
	}

	public void Visit( Literal e )
	{
		function.InstructionABx( e.SourceSpan, Opcode.LoadK, target, function.Constant( e.Value ) );
	}

	public void Visit( LocalRef e )
	{
		Allocation local = function.Local( e.Variable );
		if ( (int)target != (int)local )
		{
			function.InstructionABC( e.SourceSpan, Opcode.Move, target, local, 0 );
		}
		local.Release();
	}

	public void Visit( Logical e )
	{
		// Perform shortcut evaluation.
		LabelAST shortcutEvaluation = new LabelAST( "shortcutEvaluation" );
		Allocation left = R( e.Left );
		function.InstructionABC( e.SourceSpan, Opcode.TestSet, target, left, e.Op == LogicalOp.Or ? 1 : 0 );
		function.InstructionAsBx( e.SourceSpan, Opcode.Jmp, 0, shortcutEvaluation );
		left.Release();
		e.Right.Accept( this );
		function.Label( shortcutEvaluation );
	}

	public void Visit( Not e )
	{
		Allocation operand = R( e.Operand );
		function.InstructionABC( e.SourceSpan, Opcode.Not, target, operand, 0 );
		operand.Release();
	}

	public void Visit( OpcodeConcat e )
	{
		// Get operand list.
		Allocation allocation = function.Top();
		foreach ( Expression operand in e.Operands )
		{
			Allocation allocOperand = Push( operand );
			allocOperand.Release();
			allocation.Allocate();
		}

		// Instruction.
		function.InstructionABC( e.SourceSpan, Opcode.Concat, target, allocation, allocation + e.Operands.Count - 1 );
		allocation.Release();
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
		function.InstructionABC( e.SourceSpan, op, target, operand, 0 );
		operand.Release();
	}

	public void Visit( UpValRef e )
	{
		// The block where the upval was declared needs to be closed.
		for ( BlockBuilder outer = block; outer != null; outer = outer.Parent )
		{
			if ( outer.Block == e.Variable.Block )
			{
				outer.SetNeedsClose();
				break;
			}
		}

		// Get upval.
		function.InstructionABC( e.SourceSpan, Opcode.GetUpVal, target, function.UpVal( e.Variable ), 0 );
	}

	public void Visit( Vararg e )
	{
		function.InstructionABC( e.SourceSpan, Opcode.Vararg, target, 1, 0 );
	}

}


}
