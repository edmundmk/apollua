// VirtualMachine.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Text;
using System.Collections.Generic;


namespace Lua.VM
{


public class VirtualMachine
{
	// Constants

	const int stackLimit		= 204;
	const int setTop			= -1;
	const int invokeS			= -2;
	const int invokeM			= -3;
	static readonly Value zero	= 0;

	
	// VM State.

	struct Frame
	{
		public VMFunction	Function		{ get; private set; }
		public int			FunctionBase	{ get; private set; }
		public int			FrameBase		{ get; private set; }
		public int			ResultCount		{ get; private set; }
		public int			TailCallCount	{ get; set; }
		public int			IP				{ get; set; }


		public Frame( VMFunction function, int functionBase, int frameBase, int resultCount )
			:	this()
		{
			Function		= function;
			FunctionBase	= functionBase;
			FrameBase		= frameBase;
			ResultCount		= resultCount;
			TailCallCount	= 0;
			IP				= 0;
		}
	}

	List< Frame >	frames;
	Frame			frame;
	List< Value >	stack;
	List< UpVal >	openUpVals;
	int				top;
	
	int				invokeBase;
	int				invokeCount;


	// Interface.

	public VirtualMachine()
	{
		frames		= new List< Frame >();
		stack		= new List< Value >();
		openUpVals	= new List< UpVal >();
		top			= -1;

		invokeBase	= -1;
		invokeCount	= 0;
	}


	public void BeginInvoke( int argumentCount )
	{
		if ( invokeBase != -1 )
			throw new InvalidOperationException();

		// Allocate space at the top of the stack for the call frame.
		invokeBase = frame.FrameBase + ( ( frame.Function != null ) ? frame.Function.Prototype.StackSize : 0 );
		CloseStack( invokeBase );
		EnsureStack( invokeBase + 1 + argumentCount );
		invokeCount = 0;
	}

	public void Argument( Value argument )
	{
		if ( invokeBase == -1 )
			throw new InvalidOperationException();

		// Add parameter.
		stack[ invokeBase + 1 + invokeCount ] = argument;
		invokeCount += 1;
	}
	
	public Value InvokeS( VMFunction function )
	{
		// Add function in correct position in the call frame.
		stack[ invokeBase ] = function;

		// Push existing frame.
		PushFrame();

		// Build a frame to call this function.
		NewFrame( function, invokeBase, invokeCount, invokeS );

		// Dispatch.
		invokeBase	= -1;
		invokeCount	= 0;

		DispatchLoop();

		invokeBase = frame.FunctionBase;
		Value result = stack[ invokeBase ];

		// Finished.
		invokeBase	= -1;
		invokeCount	= 0;

		return result;
	}

	public Value[] InvokeM( VMFunction function )
	{
		// Add function in correct position in the call frame.
		stack[ invokeBase ] = function;

		// Push existing frame.
		PushFrame();

		// Build a frame to call this function.
		NewFrame( function, invokeBase, invokeCount, invokeS );

		// Dispatch.
		invokeBase	= -1;
		invokeCount	= 0;

		DispatchLoop();

		invokeBase	= frame.FunctionBase;
		invokeCount	= GetTop() - invokeBase;

		// Move results into an array.
		Value[] results = new Value[ invokeCount ];
		for ( int result = 0; result < invokeCount; ++result )
		{
			results[ result ] = stack[ invokeBase + result ];
		}

		// Finished.
		invokeBase	= -1;
		invokeCount	= 0;

		return results;
	}
	


	// Dispatch.

	void DispatchLoop()
	{
		try
		{

		while ( true )
		{
			Instruction i = NextInstruction();

			switch ( i.Opcode )
			{

			case Opcode.Move:
			{
				// R( A ) := R( B )
				SetR( i.A, R( i.B ) );
				continue;
			}

			case Opcode.LoadK:
			{
				// R( A ) := K( Bx )
				SetR( i.A, K( i.Bx ) );
				continue;
			}

			case Opcode.LoadBool:
			{
				// R( A ) := (bool)B
				if ( i.B != 0 )
				{
					SetR( i.A, true );
				}
				else
				{
					SetR( i.A, false );
				}
			
				// if C skip next instruction
				if ( i.C != 0 )
				{
					NextInstruction();
				}
				
				continue;
			}

			case Opcode.LoadNil:
			{
				// R( A ) ... R( B ) := nil
				for ( int r = i.A; r < i.B; ++r )
				{
					SetR( r, null );
				}
				continue;
			}
	
			case Opcode.GetUpVal:
			{
				// R( A ) := U( B )
				SetR( i.A, U( i.B ) );
				continue;
			}

			case Opcode.GetGlobal:
			{
				// R( A ) := G[ K( Bx ) ]
				SetR( i.A, GetEnvironment().Index( K( i.Bx ) ) );
				continue;
			}

			case Opcode.GetTable:
			{
				// R( A ) := R( B )[ RK( C ) ]
				SetR( i.A, R( i.B ).Index( RK( i.C ) ) );
				continue;
			}
	
			case Opcode.SetGlobal:
			{
				// G[ K( Bx ) ] := R( A )
				GetEnvironment().NewIndex( K( i.Bx ), R( i.A ) );
				continue;
			}

			case Opcode.SetUpVal:
			{
				// U( B ) := R( A )
				SetU( i.B, R( i.A ) );
				continue;
			}

			case Opcode.SetTable:
			{
				// R( A )[ RK( B ) ] = RK( C )
				R( i.A ).NewIndex( RK( i.B ), RK( i.C ) );
				continue;
			}
	
			case Opcode.NewTable:
			{
				// R( A ) := {} ( B is array size hint, C is hash size hint )
				SetR( i.A, new Table( i.B, i.C ) ); // hash size hint should be log2
				continue;
			}

			case Opcode.Self:
			{
				Value self = R( i.B );

				// R( A + 1 ) := R( B ) 
				SetR( i.A + 1, self );

				// R( A ) = R( B )[ RK( C ) ]
				SetR( i.A, R( i.B ).Index( RK( i.C ) ) );

				continue;
			}
	
			case Opcode.Add:
			{
				// R( A ) := RK( B ) + RK( C )
				SetR( i.A, RK( i.B ).Add( RK( i.C ) ) );
				continue;
			}

			case Opcode.Sub:
			{
				// R( A ) := RK( B ) - RK( C )
				SetR( i.A, RK( i.B ).Subtract( RK( i.C ) ) );
				continue;
			}

			case Opcode.Mul:
			{
				// R( A ) := RK( B ) * RK( C )
				SetR( i.A, RK( i.B ).Multiply( RK( i.C ) ) );
				continue;
			}

			case Opcode.Div:
			{
				// R( A ) := RK( B ) / RK( C )
				SetR( i.A, RK( i.B ).Divide( RK( i.C ) ) );
				continue;
			}
	
			case Opcode.Mod:
			{
				// R( A ) := RK( B ) % RK( C )
				SetR( i.A, RK( i.B ).Modulus( RK( i.C ) ) );
				continue;
			}

			case Opcode.Pow:
			{
				// R( A ) := RK( B ) ^ RK( C )
				SetR( i.A, RK( i.B ).RaiseToPower( RK( i.C ) ) );
				continue;
			}
	
			case Opcode.Unm:
			{
				// R( A ) := -R( B )
				SetR( i.A, R( i.B ).UnaryMinus() );
				continue;
			}

			case Opcode.Not:
			{
				// R( A ) := not R( B )
				SetR( i.A, R( i.B ).IsTrue() );
				continue;
			}

			case Opcode.Len:
			{
				// R( A ) := length of R( B )
				SetR( i.A, R( i.B ).Length() );
				continue;
			}

			case Opcode.Concat:
			{
				// R( A ) := R( B ) .. ... .. R( C ), concatenating whole list
				int top = i.C;
				int count = i.C - i.B + 1;

				while ( count > 1 )
				{
					Value left	= R( top - 1 );
					Value right	= R( top - 0 );

					if ( left.UsePrimitiveConcatenate() && right.UsePrimitiveConcatenate() )
					{
						// Count how many we can concatenate in this pass.
						int concatcount = 2;
						for ( concatcount = 2; concatcount < count; ++concatcount )
						{
							Value operand = R( top - concatcount );
							if ( ! operand.UsePrimitiveConcatenate() )
							{
								break;
							}
						}

						// Concatenate them.
						StringBuilder s = new StringBuilder();

						for ( int r = top - ( concatcount - 1 ); r <= top; ++r )
						{
							Value operand = R( r );
							s.Append( operand.ToString() );
						}

						// Modify the stack top and continue.
						SetR( top - ( concatcount - 1 ), s.ToString() );
						top		-= concatcount - 1;
						count	-= concatcount - 1;
					}
					else
					{
						// Perform meta concatenation.
						SetR( top - 1, left.Concatenate( right ) );
						top		-= 1;
						count	-= 1;
					}
				}

				SetR( i.A, R( top ) );

				continue;
			}

			case Opcode.Jmp:
			{
				// relative jump sBx ( relative to next instruction )
				Jump( i.sBx );
				continue;
			}
	
			case Opcode.Eq:
			{
				// if ( RK( B ) == RK( C ) ) ~= A then skip associated jump
				if ( RK( i.B ).Equals( RK( i.C ) ) == ( i.A != 0 ) )
				{
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}

			case Opcode.Lt:
			{
				// if ( RK( B ) <  RK( C ) ) ~= A then skip associated jump
				if ( RK( i.B ).LessThan( RK( i.C ) ) == ( i.A != 0 ) )
				{
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}

			case Opcode.Le:
			{
				// if ( RK( B ) <= RK( C ) ) ~= A then skip associated jump
				if ( RK( i.B ).LessThanOrEqual( RK( i.C ) ) == ( i.A != 0 ) )
				{
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}

			case Opcode.Test:
			{
				// if not ( R( A ) <=> C ) then skip associated jump
				if ( R( i.A ).IsTrue() != ( i.C != 0 ) )
				{
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}

			case Opcode.TestSet:
			{
				// if ( R( B ) <=> C ) then R( A ) := R( B ) else skip associated jump
				if ( R( i.B ).IsTrue() != ( i.C != 0 ) )
				{
					// Set.
					SetR( i.A, R( i.B ) );

					// Perform associated jump.
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}

			case Opcode.Call:
			{
				// R( A ), ... , R( A + C - 2 ) := R( A )( R( A + 1 ), ... , R( A + B - 1 ) )
				
				int argumentCount;
				if ( i.B == 0 )
				{
					argumentCount = GetTop() - i.A - 1;
				}
				else
				{
					argumentCount = i.B - 1;
				}

				Value function = R( i.A );
				if ( function is VMFunction )
				{
					CallFunction( (VMFunction)function, i.A, argumentCount, i.C - 1 );
				}
				else
				{
					CallFunction( function, i.A, argumentCount, i.C - 1 );
				}

				continue;
			}

			case Opcode.TailCall:
			{
				// return R( A )( R( A + 1 ), ... , R( A + B - 1 ) )

				int argumentCount;
				if ( i.B == 0 )
				{
					argumentCount = GetTop() - i.A;
				}
				else
				{
					argumentCount = i.B - 1;
				}

				Value function = R( i.A );
				if ( function is VMFunction )
				{
					TailCallFunction( (VMFunction)function, i.A, argumentCount );
				}
				else
				{
					CallFunction( function, i.A, argumentCount, i.C - 1 );
				}

				continue;
			}

			case Opcode.Return:
			{
				// return R( A ), ... R( A + B - 2 )
				
				int resultcount;
				if ( i.B == 0 )
				{
					resultcount = GetTop() - i.A;
				}
				else
				{
					resultcount = i.B - 1;
				}

				if ( Return( i.A, resultcount ) )
				{
					// Invoked function, return back to the caller.
					return;
				}
				else
				{
					// Called function, continue with next stack frame.
					continue;
				}
			}


			/*	A + 0		Index
				A + 1		Limit
				A + 2		Step
				A + 3		Var
			*/

			case Opcode.ForLoop:
			{
				Value index = R( i.A + 0 );
				Value limit = R( i.A + 1 );
				Value step	= R( i.A + 2 );

				// Index += Step
				index = index.Add( step );
				SetR( i.A + 0, index );
				
				// if ( Step > 0 and Index <= Limit ) or ( Step < 0 and Index >= Limit ) then Var = Index, relative jump sBx
				if (    ( ! step.LessThanOrEqual( zero ) && index.LessThanOrEqual( limit ) )
					 || ( step.LessThan( zero ) && ! index.LessThan( limit ) ) )
				{
					SetR( i.A + 3, index );
					Jump( i.sBx );
				}
				continue;
			}

			case Opcode.ForPrep:
			{
				Value index = R( i.A + 0 );
				Value limit = R( i.A + 1 );
				Value step	= R( i.A + 2 );
				
				// Convert for control variables to numbers.
				if ( ! index.TryToNumber( out index ) )
				{
					throw new InvalidOperationException( "'for' initial value must be a number" );
				}
				if ( ! index.TryToNumber( out limit ) )
				{
					throw new InvalidOperationException( "'for' limit must be a number" );
				}
				if ( ! index.TryToNumber( out step ) )
				{
					throw new InvalidOperationException( "'for' step must be a number" );
				}

				// Index -= Step
				index = index.Subtract( step );

				// Update values.
				SetR( i.A + 0, index );
				SetR( i.A + 1, limit );
				SetR( i.A + 2, step );
				
				// relative jump sBx
				Jump( i.sBx );
				continue;
			}

	
			/*	A + 0		Generator
				A + 1		State
				A + 2		Control
				A + 3		Var_1
				...
				A + 3 + C	Var_C
			*/

			case Opcode.TForLoop:
			{
				Value generator	= R( i.A + 0 );
				Value state		= R( i.A + 1 );
				Value control	= R( i.A + 2 );

				// Var_1, ..., Var_C := Generator( State, Control )
				Value[] results = generator.InvokeM( state, control );
				for ( int result = 0; result < i.C; ++result )
				{
					SetR( i.A + 3 + result, result < results.Length ? results[ result ] : null );
				}
				
				// if Var_1 ~= nil then Control = Var_1 else skip associated jump
				Value var_1 = results[ 0 ];
				if ( var_1.IsTrue() )
				{
					// Set.
					SetR( i.A + 2, var_1 );

					// Perform associated jump.
					i = NextInstruction(); Jump( i.sBx );
				}
				else
				{
					NextInstruction();
				}
				continue;
			}
	

			case Opcode.SetList:
			{
				// Special decoding.
				int C = i.C;
				if ( C == 0 )
				{
					Instruction index = NextInstruction();
					C = index.Index;
				}
	
				// R( A )[ ( C - 1 ) * 50 + i ] := R( A + i ), 1 <= i <= B
				for ( int key = 1; key < i.B; ++key )
				{
					R( i.A ).NewIndex( ( C - 1 ) * Instruction.FieldsPerFlush + key, R( i.A + key ) );
				}
				continue;
			}

			case Opcode.Close:
			{
				// close all stack variables from R( A ) to the top
				CloseUpVals( i.A );
				continue;
			}

			case Opcode.Closure:
			{
				// R( A ) := function closure from P( Bx )
				VMFunction function = new VMFunction( P( i.Bx ) );
				function.Environment = GetEnvironment();
				SetR( i.A, function );
				
				// followed by upval initialization with Move or GetUpVal
				for ( int upval = 0; upval < function.Prototype.UpValCount; ++upval )
				{
					Instruction u = NextInstruction();

					if ( u.Opcode == Opcode.Move )
					{
						function.UpVals[ upval ] = FindUpVal( u.B );
					}
					else if ( u.Opcode == Opcode.GetUpVal )
					{
						function.UpVals[ upval ] = GetUpVal( u.B );
					}
					else
					{
						throw new InvalidOperationException( "malformed upval initialization bytecode" );
					}
				}

				continue;
			}

			case Opcode.Vararg:
			{
				// R( A ), ..., R( A + B - 1 ) = vararg
				Vararg( i.A, i.B - 1 );
				continue;
			}

			case Opcode.IntDiv:
			{
				// R( A ) := RK( B ) \ RK( C )
				SetR( i.A, RK( i.B ).IntegerDivide( RK( i.C ) ) );
				continue;
			}

			}

		}
		
		}
		catch ( Exception e )
		{
			// Unwind this crap and re-throw exception including Lua callstack.
			throw UnwindError( e );
		}

	}

	

	// Operands.

	Instruction NextInstruction()
	{
		Instruction i = frame.Function.Prototype.Instructions[ frame.IP ];
		frame.IP += 1;
		return i;

	}

	void Jump( int offset )
	{
		frame.IP += offset;
	}

	Value GetEnvironment()
	{
		return frame.Function.Environment;
	}

	int GetTop()
	{
		return top - frame.FrameBase;
	}

	Value R( int operand )
	{
		return stack[ frame.FrameBase + operand ];
	}

	void SetR( int operand, Value value )
	{
		stack[ frame.FrameBase + operand ] = value;
	}

	Value U( int operand )
	{
		return frame.Function.UpVals[ operand ].Value;
	}

	void SetU( int operand, Value value )
	{
		frame.Function.UpVals[ operand ].Value = value;
	}

	Prototype P( int operand )
	{
		return frame.Function.Prototype.Prototypes[ operand ];
	}

	Value K( int operand )
	{
		return frame.Function.Prototype.Constants[ operand ];
	}

	Value RK( int operand )
	{
		if ( Instruction.IsConstant( operand ) )
		{
			return frame.Function.Prototype.Constants[ Instruction.RKToConstant( operand ) ];
		}
		else
		{
			return R( operand );
		}
	}



	// Upvals.

	UpVal GetUpVal( int upval )
	{
		return frame.Function.UpVals[ upval ];
	}

	UpVal FindUpVal( int r )
	{
		UpVal upval;

		// r is relative to FrameBase.
		r += frame.FrameBase;

		// Find existing UpVal.
		int upvalindex = 0;
		while ( upvalindex < openUpVals.Count )
		{
			upval = openUpVals[ upvalindex ];

			if ( upval.Index == r )
			{
				return upval;
			}

			if ( upval.Index > r )
			{
				break;
			}
		}

		// Create new one.
		upval = new UpVal( stack, r );
		openUpVals.Insert( upvalindex, upval );
		return upval;
	}

	void CloseUpVals( int r )
	{
		UpVal upval;

		// r is relative to FrameBase.
		r += frame.FrameBase;

		// Close upvals.
		for ( int upvalindex = 0; upvalindex < openUpVals.Count; ++upvalindex )
		{
			upval = openUpVals[ upvalindex ];

			// Keep upvals below r.
			if ( upval.Index < r )
			{
				continue;
			}

			// Found an upval after r, close all upvals after this one.
			for ( int i = upvalindex; i < openUpVals.Count; ++i )
			{
				upval = openUpVals[ upvalindex ];
				upval.Close();
			}

			// Remove them from the list.
			openUpVals.RemoveRange( upvalindex, openUpVals.Count - upvalindex );

			// Done.
			break;
		}

		// Close stack variables.
		CloseStack( r );
	}



	// Stack.

	void EnsureStack( int top )
	{
		// Make sure we have enough stack elements to contain top.
		while ( stack.Count < top )
		{
			stack.Add( null );
		}
	}

	void CloseStack( int top )
	{
		// Clear stack elements to enable GC.
		int frameTop = frame.FrameBase + ( ( frame.Function != null ) ? frame.Function.Prototype.StackSize : 0 );
		for ( int index = top; index < frameTop; ++index )
		{
			stack[ index ] = null;
		}

		// Remove elements above both top and frametop.
		int removeBase = Math.Max( top, frameTop );
		stack.RemoveRange( removeBase, stack.Count - removeBase );
	}



	// Calls.

	void CallFunction( Value function, int functionBase, int argumentCount, int resultCount )
	{
		// Get stack value for the function base.
		functionBase += frame.FrameBase;
	
		// Package arguments.
		Value[] arguments = new Value[ argumentCount ];
		for ( int argument = 0; argument < argumentCount; ++argument )
		{
			arguments[ argument ] = stack[ functionBase + 1 + argument ];
		}

		// Invoke.
		if ( resultCount == 1 )
		{
			stack[ functionBase ] = function.InvokeS( arguments );
		}
		else
		{
			Value[] results = function.InvokeM( arguments );

			// Unpack results.
			for ( int result = 0; result < resultCount; ++result )
			{
				stack[ functionBase + result ] = result < results.Length ? results[ result ] : null;
			}
		}

	}

	void CallFunction( VMFunction function, int functionBase, int argumentCount, int resultCount )
	{
		// Get stack value for the function base.
		functionBase += frame.FrameBase;

		// Function calls are always at the top of the stack.
		CloseStack( functionBase + 1 + argumentCount );

		// Create new frame.
		PushFrame();
		NewFrame( function, functionBase, argumentCount, resultCount );
	}


	void TailCallFunction( VMFunction function, int r, int argumentCount )
	{
		// Get stack value for r.
		r += frame.FrameBase;

		// Replace frame with this one.
		int functionBase	= frame.FunctionBase;
		int resultCount		= frame.ResultCount;
		int tailCallCount	= frame.TailCallCount;
		PopFrame();
		
		EnsureStack( functionBase + 1 + argumentCount );
		stack[ functionBase ] = function;
		for( int argument = 0; argument < argumentCount; ++argument )
		{
			stack[ functionBase + 1 + argument ] = stack[ r + 1 + argument ];
		}

		// Call frame.
		CallFunction( function, functionBase - frame.FrameBase, argumentCount, resultCount );
		frame.TailCallCount = tailCallCount + 1;
	}


	void NewFrame( VMFunction function, int functionBase, int argumentCount, int resultCount )
	{
		/*						argument
								argument
			FunctionBase	--> Function
		*/
	
		// Adjust parameters to find the frame base.
		int frameBase;

		if ( argumentCount < function.Prototype.ParameterCount )
		{
			/*						null
									argument
				FrameBase		-->	argument
				FunctionBase	--> Function
			*/

			frameBase = functionBase + 1;
			EnsureStack( frameBase + function.Prototype.StackSize );
		}
		else if ( function.Prototype.IsVararg )
		{
			/*	FrameBase		--> argument (*)
									argument (vararg)
									null (*)
				FunctionBase	--> Function
			*/

			frameBase = functionBase + 1 + argumentCount;
			EnsureStack( frameBase + function.Prototype.StackSize );

			// Copy non-vararg parameters to FrameBase.
			for ( int argument = 0; argument < function.Prototype.ParameterCount; ++argument )
			{
				int rargument = functionBase + 1 + argument;
				stack[ frameBase + argument ] = stack[ rargument ];
				stack[ rargument ] = null;
			}
		}
		else
		{
			/*						null
				FrameBase		-->	argument
				FunctionBase	--> Function
			*/

			frameBase = functionBase + 1;
			EnsureStack( frameBase + function.Prototype.StackSize );
		}

		// Create new frame.
		frame = new Frame( function, functionBase, frameBase, resultCount );
	}


	void PushFrame()
	{
		// Don't push nonexistent frame.
		if ( frame.Function == null && frames.Count == 0 )
		{
			return;
		}

		// Check stack.
		if ( frames.Count >= stackLimit )
		{
			throw new StackOverflowException( "stack overflow" );
		}

		// Push frame.
		frames.Add( frame );
	}


	void PopFrame()
	{
		// Pop frame
		if ( frames.Count > 0 )
		{
			frame = frames[ frames.Count - 1 ];
			frames.RemoveAt( frames.Count - 1 );
		}
		else
		{
			frame = new Frame();
		}
	}

	
	void Vararg( int r, int resultCount )
	{
		// r is relative to this frame.
		r += frame.FrameBase;

		// Find varargs.
		int varargBase	= frame.FunctionBase + 1 + frame.Function.Prototype.ParameterCount;
		int varargCount = Math.Max( frame.FrameBase - varargBase, 0 );

		// Set up correct number.
		int copyCount;
		if ( resultCount == setTop )
		{
			copyCount = varargCount;
			top = r + varargCount;
		}
		else
		{
			copyCount = Math.Min( varargCount, resultCount );
		}

		// Copy into correct position.

		for ( int vararg = 0; vararg < copyCount; ++vararg )
		{
			stack[ r + vararg ] = stack[ varargBase + vararg ];
		}

		for ( int vararg = copyCount; vararg < resultCount; ++vararg )
		{
			stack[ r + vararg ] = null;
		}
	}


	bool Return( int r, int resultCount )
	{
		// Set up the correct number of results.
		int resultTop;
		int copyCount;
		if (    frame.ResultCount == invokeM
			 || frame.ResultCount == setTop )
		{
			copyCount	= resultCount;
			resultTop	= frame.FunctionBase + resultCount;
			top			= resultTop;
		}
		else
		{
			copyCount	= Math.Min( resultCount, frame.ResultCount );
			resultTop	= frame.FunctionBase + frame.ResultCount;
		}

		// Copy results into the correct position.

		for ( int result = 0; result < copyCount; ++result )
		{
			stack[ frame.FunctionBase + result ] = stack[ frame.FrameBase + r + result ];
		}

		for ( int result = copyCount; result < frame.ResultCount; ++result )
		{
			stack[ frame.FunctionBase + result ] = null;
		}

		// Return true to return from an invocation.
		bool wasInvoke = frame.ResultCount == invokeS || frame.ResultCount == invokeM;

		// Pop frame.
		PopFrame();
		CloseStack( resultTop );

		// Return true to return from an invocation.
		return wasInvoke;
	}



	// Stack trace.

	Exception UnwindError( Exception e )
	{
		// Unwind to last invoke.
		int unwindLevel;
		for ( unwindLevel = 1; unwindLevel < frames.Count; ++unwindLevel )
		{
			Frame unwind = StackFrame( unwindLevel );
			if ( unwind.ResultCount == invokeS || unwind.ResultCount == invokeM )
			{
				break;
			}
		}

		// Get stack trace and unwind the stack for real.
		string stackTrace = StackTrace( 0, unwindLevel );
		Unwind( unwindLevel );

		// Throw new error.
		if ( e is VMException )
		{
			return new VMException( e, e.StackTrace + "\n" + stackTrace );
		}
		else
		{
			return new VMException( e, stackTrace );
		}
	}

	string SourceLine( int level )
	{
		Frame stackFrame = StackFrame( level );
		
		if ( stackFrame.Function != null )
		{
			Prototype prototype = stackFrame.Function.Prototype;
			DebugSourceSpan line = prototype.DebugInstructionSourceSpans[ stackFrame.IP - 1 ];
			return String.Format( "{0}:{1}", line.Start.SourceName, line.Start.Line );
		}
		else
		{
			return "[C]";
		}
	}

	string StackTrace( int startLevel, int endLevel )
	{
		StringBuilder s = new StringBuilder();

		for ( int level = startLevel; level <= endLevel; ++level )
		{
			if ( level > startLevel )
			{
				s.Append( "\n" );
			}

			Frame stackFrame = StackFrame( level );
			
			if ( stackFrame.Function != null )
			{
				Prototype prototype = stackFrame.Function.Prototype;
				DebugSourceSpan span = prototype.DebugInstructionSourceSpans[ stackFrame.IP - 1 ];
				s.AppendFormat( "   {0}:{1}:", span.Start.SourceName, span.Start.Line );
				if ( prototype.DebugName != null )
				{
					s.Append( " " );
					s.Append( prototype.DebugName );
				}
			}
			else
			{
				s.AppendFormat( "   [C]" );
			}
		}

		return s.ToString();
	}

	Frame StackFrame( int level )
	{
		if ( level > 0 )
		{
			return frames[ frames.Count - level ];
		}
		else
		{
			return frame;
		}
	}

	void Unwind( int level )
	{
		if ( level > 1 )
		{
			frames.RemoveRange( frames.Count - ( level - 1 ), level - 1 );
		}

		PopFrame();
	}





}


}