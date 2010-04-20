// LuaFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Text;
using Lua.Runtime;
using Lua.Bytecode;


namespace Lua
{


public sealed class LuaFunction
	:	LuaValue
{

	// Function state.

	UpVal[]			upVals;
	LuaPrototype	prototype;


	public LuaFunction( LuaPrototype p )
		:	this( p, null )
	{
	}

	public LuaFunction( LuaPrototype p, LuaTable environment )
	{
		upVals = new UpVal[ p.UpValCount ];
		prototype = p;

		if ( environment != null )
			Environment = environment;
		else
			Environment = LuaThread.CurrentThread.Environment;
	}

	
	public LuaPrototype Prototype
	{
		get { return prototype; }
	}

	
	
	// Lua.

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal LuaTable Environment
	{
		get; set;
	}


	// Function interface.

	static readonly LuaValue zero = 0;


	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		/*	frameBase		-->	Function
				^				argument
			argumentCount		argument
				v				argument
		*/

		int fp = frameBase;

		if ( Prototype.IsVararg && argumentCount > Prototype.ParameterCount )
		{
			/*	frameBase		-->	Function
					^				null
				argumentCount		null
					v				argument (vararg)
				framePointer	--> argument
									argument
			*/

			fp = frameBase + 1 + argumentCount;
			thread.StackWatermark( fp + prototype.StackSize );

			for ( int argument = 0; argument < argumentCount; ++argument )
			{
				thread.Stack[ fp + argument ] = thread.Stack[ frameBase + 1 + argument ];
				thread.Stack[ frameBase + 1 + argument ] = null;
			}
		}
		else
		{
			fp = frameBase + 1;
			thread.StackWatermark( fp + prototype.StackSize );
		}

		thread.StackLevels.Add( this );
		Dispatch( thread, frameBase, resultCount, fp, 0 );
	}

	
	internal override void Resume( LuaThread thread )
	{
		// Pop frame off the stack.
		Frame frame = thread.UnwoundFrames[ thread.UnwoundFrames.Count - 1 ];
		thread.UnwoundFrames.RemoveAt( thread.UnwoundFrames.Count - 1 );


		// Recover frame information.
		int frameBase = frame.FrameBase;
		int resultCount = frame.ResultCount;
		int fp = frame.FramePointer;
		int ip = frame.InstructionPointer;


		// Resume the next suspended frame.
		try
		{
	
		Instruction i = prototype.Instructions[ ip - 1 ];
		
		switch ( i.Opcode )
		{
			case Opcode.Call:
			{
				// Resume function.
				LuaValue function = thread.Stack[ fp + i.A ];
				function.Resume( thread );

				if ( thread.UnwoundFrames.Count > 0 )
				{
					thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
					return;
				}

				if ( i.C != 0 )
				{
					thread.StackWatermark( fp + prototype.StackSize );
				}
				else
				{
					thread.StackWatermark( Math.Max( fp + prototype.StackSize, thread.Top + 1 ) );
				}

				break;
			}

			default:
				throw new InvalidOperationException();
		}

		}
		catch ( Exception e )
		{
			thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
			throw e;
		}

		// Returned normally, dispatch the rest of the function.
		Dispatch( thread, frameBase, resultCount, fp, ip );
	}
	
	
	void Dispatch( LuaThread thread, int frameBase, int resultCount, int fp, int ip )
	{
		try
		{

		while ( true )
		{
			// Suspend coroutine (support for suspending at arbitrary instructions).
/*
			if ( thread.UnwoundFrames.Count > 0 )
			{
				thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
				return;
			}
*/

			// Dispatch instructions.
	
			Instruction i = prototype.Instructions[ ip++ ];

			switch ( i.Opcode )
			{

			case Opcode.Move:
			{
				// R( A ) := R( B )
				thread.Stack[ fp + i.A ] = thread.Stack[ fp + i.B ];
				continue;
			}

			case Opcode.LoadK:
			{
				// R( A ) := K( Bx )
				thread.Stack[ fp + i.A ] = K( i.Bx );
				continue;
			}

			case Opcode.LoadBool:
			{
				// R( A ) := (bool)B
				if ( i.B != 0 )
				{
					thread.Stack[ fp + i.A ] = true;
				}
				else
				{
					thread.Stack[ fp + i.A ] = false;
				}
			
				// if C skip next instruction
				if ( i.C != 0 )
				{
					ip += 1;
				}
				
				continue;
			}

			case Opcode.LoadNil:
			{
				// R( A ) ... R( B ) := nil
				for ( int r = i.A; r < i.B; ++r )
				{
					thread.Stack[ fp + r ] = null;
				}
				continue;
			}
	
			case Opcode.GetUpVal:
			{
				// R( A ) := U( B )
				thread.Stack[ fp + i.A ] = upVals[ i.B ].Value;
				continue;
			}

			case Opcode.GetGlobal:
			{
				// R( A ) := G[ K( Bx ) ]
				thread.Stack[ fp + i.A ] = Environment.Index( K( i.Bx ) );
				continue;
			}

			case Opcode.GetTable:
			{
				// R( A ) := R( B )[ RK( C ) ]
				thread.Stack[ fp + i.A ] = thread.Stack[ fp + i.B ].Index( RK( thread.Stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.SetGlobal:
			{
				// G[ K( Bx ) ] := R( A )
				Environment.NewIndex( K( i.Bx ), thread.Stack[ fp + i.A ] );
				continue;
			}

			case Opcode.SetUpVal:
			{
				// U( B ) := R( A )
				upVals[ i.B ].Value = thread.Stack[ fp + i.A ];
				continue;
			}

			case Opcode.SetTable:
			{
				// R( A )[ RK( B ) ] = RK( C )
				thread.Stack[ fp + i.A ].NewIndex( RK( thread.Stack, fp, i.B ), RK( thread.Stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.NewTable:
			{
				// R( A ) := {} ( B is array size hint, C is hash size hint )
				thread.Stack[ fp + i.A ] = new LuaTable( i.B, i.C );
				continue;
			}

			case Opcode.Self:
			{
				LuaValue self = thread.Stack[ fp + i.B ];

				// R( A + 1 ) := R( B ) 
				thread.Stack[ fp + i.A + 1 ] = self;

				// R( A ) = R( B )[ RK( C ) ]
				thread.Stack[ fp + i.A ] = self.Index( RK( thread.Stack, fp, i.C ) );

				continue;
			}
	
			case Opcode.Add:
			{
				// R( A ) := RK( B ) + RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).Add( RK( thread.Stack, fp, i.C ) );
				continue;
			}

			case Opcode.Sub:
			{
				// R( A ) := RK( B ) - RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).Subtract( RK( thread.Stack, fp, i.C ) );
				continue;
			}

			case Opcode.Mul:
			{
				// R( A ) := RK( B ) * RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).Multiply( RK( thread.Stack, fp, i.C ) );
				continue;
			}

			case Opcode.Div:
			{
				// R( A ) := RK( B ) / RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).Divide( RK( thread.Stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.Mod:
			{
				// R( A ) := RK( B ) % RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).Modulus( RK( thread.Stack, fp, i.C ) );
				continue;
			}

			case Opcode.Pow:
			{
				// R( A ) := RK( B ) ^ RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).RaiseToPower( RK( thread.Stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.Unm:
			{
				// R( A ) := -R( B )
				thread.Stack[ fp + i.A ] = thread.Stack[ fp + i.B ].UnaryMinus();
				continue;
			}

			case Opcode.Not:
			{
				// R( A ) := not R( B )
				LuaValue B = thread.Stack[ fp + i.B ];
				thread.Stack[ fp + i.A ] = !( B != null && B.IsTrue() );
				continue;
			}

			case Opcode.Len:
			{
				// R( A ) := length of R( B )
				thread.Stack[ fp + i.A ] = thread.Stack[ fp + i.B ].Length();
				continue;
			}

			case Opcode.Concat:
			{
				// R( A ) := R( B ) .. ... .. R( C ), concatenating whole list
				int listTop	= fp + i.C;
				int count	= i.C - i.B + 1;

				while ( count > 1 )
				{
					LuaValue left	= thread.Stack[ listTop - 1 ];
					LuaValue right	= thread.Stack[ listTop - 0 ];

					if ( left.SupportsSimpleConcatenation() && right.SupportsSimpleConcatenation() )
					{
						// Count how many we can concatenate in this pass.
						int concatCount = 2;
						for ( concatCount = 2; concatCount < count; ++concatCount )
						{
							LuaValue operand = thread.Stack[ listTop - concatCount ];
							if ( ! operand.SupportsSimpleConcatenation() )
							{
								break;
							}
						}

						// Concatenate them.
						StringBuilder s = new StringBuilder();

						for ( int r = listTop - ( concatCount - 1 ); r <= listTop; ++r )
						{
							s.Append( thread.Stack[ r ].ToString() );
						}

						// Modify the thread.Stack top and continue.
						thread.Stack[ listTop - ( concatCount - 1 ) ] = s.ToString();
						listTop	-= concatCount - 1;
						count	-= concatCount - 1;
					}
					else
					{
						// Perform meta concatenation.
						thread.Stack[ listTop - 1 ] = left.Concatenate( right );
						listTop	-= 1;
						count	-= 1;
					}
				}

				thread.Stack[ fp + i.A ] = thread.Stack[ listTop ];

				continue;
			}

			case Opcode.Jmp:
			{
				// relative jump sBx ( relative to next instruction )
				ip += i.sBx;
				continue;
			}
	
			case Opcode.Eq:
			{
				// if ( RK( B ) == RK( C ) ) ~= A then skip associated jump
				if ( RK( thread.Stack, fp, i.B ).Equals( RK( thread.Stack, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}

			case Opcode.Lt:
			{
				// if ( RK( B ) <  RK( C ) ) ~= A then skip associated jump
				if ( RK( thread.Stack, fp, i.B ).LessThan( RK( thread.Stack, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}

			case Opcode.Le:
			{
				// if ( RK( B ) <= RK( C ) ) ~= A then skip associated jump
				if ( RK( thread.Stack, fp, i.B ).LessThanOrEquals( RK( thread.Stack, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}

			case Opcode.Test:
			{
				// if not ( R( A ) <=> C ) then skip associated jump
				LuaValue A = thread.Stack[ fp + i.A ];
				if ( ( A != null && A.IsTrue() ) == ( i.C != 0 ) )
				{
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}

			case Opcode.TestSet:
			{
				// if ( R( B ) <=> C ) then R( A ) := R( B ) else skip associated jump
				LuaValue B = thread.Stack[ fp + i.B ];
				if ( ( B != null && B.IsTrue() ) == ( i.C != 0 ) )
				{
					// Set.
					thread.Stack[ fp + i.A ] = thread.Stack[ fp + i.B ];

					// Perform associated jump.
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}

			case Opcode.Call:
			{
				// R( A ), ... , R( A + C - 2 ) := R( A )( R( A + 1 ), ... , R( A + B - 1 ) )
				
				// Count arguments.
				int callArgumentCount;
				if ( i.B != 0 )
				{
					callArgumentCount = i.B - 1;
				}
				else
				{
					callArgumentCount = thread.Top - fp - i.A;
					thread.Top = -1;
				}

				LuaValue function = thread.Stack[ fp + i.A ];
				function.Call( thread, fp + i.A, callArgumentCount, i.C - 1 );

				if ( thread.UnwoundFrames.Count > 0 )
				{
					thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
					return;
				}

				if ( i.C != 0 )
				{
					thread.StackWatermark( fp + prototype.StackSize );
				}
				else
				{
					thread.StackWatermark( Math.Max( fp + prototype.StackSize, thread.Top + 1 ) );
				}

				continue;
			}

			case Opcode.TailCall:
			{
				// return R( A )( R( A + 1 ), ... , R( A + B - 1 ) )

				thread.CloseUpVals( fp );
				thread.StackLevels.RemoveAt( thread.StackLevels.Count - 1 );

				int callArgumentCount;
				if ( i.B != 0 )
				{
					callArgumentCount = i.B - 1;
				}
				else
				{
					callArgumentCount = thread.Top - fp - i.A;
					thread.Top = -1;
				}

				LuaValue function = thread.Stack[ fp + i.A ];

				thread.Stack[ frameBase ] = function;
				for ( int argument = 0; argument < callArgumentCount; ++argument )
				{
					thread.Stack[ frameBase + 1 + argument ] = thread.Stack[ fp + i.A + 1 + argument ];
				}

				function.Call( thread, frameBase, callArgumentCount, resultCount );
				
				return;
			}

			case Opcode.Return:
			{
				// return R( A ), ... R( A + B - 2 )

				thread.CloseUpVals( fp );
				thread.StackLevels.RemoveAt( thread.StackLevels.Count - 1 );

				// Find number of results we have.
				int returnResultCount;
				if ( i.B != 0 )
				{
					returnResultCount = i.B - 1;
				}
				else
				{
					returnResultCount = thread.Top + 1 - fp - i.A;
					thread.Top = -1;
				}
				
				// Calculate number of results we want.
				int copyCount;
				if ( resultCount == -1 )
				{
					copyCount = returnResultCount;
					thread.Top = frameBase + returnResultCount - 1;
				}
				else
				{
					copyCount = Math.Min( resultCount, returnResultCount );
				}

				// Copy results.
				for ( int result = 0; result < copyCount; ++result )
				{
					thread.Stack[ frameBase + result ] = thread.Stack[ fp + i.A + result ];
				}
				for ( int result = copyCount; result < resultCount; ++result )
				{
					thread.Stack[ frameBase + result ] = null;
				}

				return;
			}


			/*	A + 0		Index
				A + 1		Limit
				A + 2		Step
				A + 3		Var
			*/

			case Opcode.ForLoop:
			{
				LuaValue index	= thread.Stack[ fp + i.A + 0 ];
				LuaValue limit	= thread.Stack[ fp + i.A + 1 ];
				LuaValue step	= thread.Stack[ fp + i.A + 2 ];

				// Index += Step
				index = index.Add( step );
				thread.Stack[ fp + i.A + 0 ] = index;
				
				// if ( Step > 0 and Index <= Limit ) or ( Step < 0 and Index >= Limit ) then Var = Index, relative jump sBx
				if (    ( ! step.LessThanOrEquals( zero ) && index.LessThanOrEquals( limit ) )
					 || ( step.LessThan( zero ) && ! index.LessThan( limit ) ) )
				{
					thread.Stack[ fp + i.A + 3 ] = index;
					ip += i.sBx;
				}

				continue;
			}

			case Opcode.ForPrep:
			{
				LuaValue index	= thread.Stack[ fp + i.A + 0 ];
				LuaValue limit	= thread.Stack[ fp + i.A + 1 ];
				LuaValue step	= thread.Stack[ fp + i.A + 2 ];
				
				// Convert for control variables to numbers.
				if ( ! index.TryToNumberValue( out index ) )
				{
					throw new InvalidOperationException( "'for' initial value must be a number." );
				}
				if ( ! limit.TryToNumberValue( out limit ) )
				{
					throw new InvalidOperationException( "'for' limit must be a number." );
				}
				if ( ! step.TryToNumberValue( out step ) )
				{
					throw new InvalidOperationException( "'for' step must be a number." );
				}

				// Index -= Step
				index = index.Subtract( step );

				// Update thread.Stack.
				thread.Stack[ fp + i.A + 0 ] = index;
				thread.Stack[ fp + i.A + 1 ] = limit;
				thread.Stack[ fp + i.A + 2 ] = step;
				
				// relative jump sBx
				ip += i.sBx;
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
				LuaValue generator	= thread.Stack[ fp + i.A + 0 ];
				LuaValue state		= thread.Stack[ fp + i.A + 1 ];
				LuaValue control	= thread.Stack[ fp + i.A + 2 ];

				// Var_1, ..., Var_C := Generator( State, Control )
				thread.Stack[ fp + i.A + 3 ] = generator;
				thread.Stack[ fp + i.A + 4 ] = state;
				thread.Stack[ fp + i.A + 5 ] = control;
				generator.Call( thread, fp + i.A + 3, 2, i.C );

				// if Var_1 ~= nil then Control = Var_1 else skip associated jump
				LuaValue var_1 = thread.Stack[ fp + i.A + 3 ];
				if ( var_1 != null && var_1.IsTrue() )
				{
					// Set control.
					thread.Stack[ fp + i.A + 2 ] = var_1;

					// Perform associated jump.
					i = prototype.Instructions[ ip++ ];
					ip += i.sBx;
				}
				else
				{
					ip += 1;
				}
				continue;
			}
	

			case Opcode.SetList:
			{
				// Special decoding.
				int keyBase;
				if ( i.C != 0 )
				{
					keyBase = i.C;
				}
				else
				{
					keyBase = prototype.Instructions[ ip++ ].Index;
				}

				int lastKey;
				if ( i.B != 0 )
				{
					lastKey = i.B;
				}
				else
				{
					lastKey = thread.Top - fp - i.A;
					thread.Top = -1;
				}
	
				// R( A )[ ( C - 1 ) * 50 + i ] := R( A + i ), 1 <= i <= B
				for ( int key = 1; key <= lastKey; ++key )
				{
					thread.Stack[ fp + i.A ].NewIndex( ( keyBase - 1 ) * Instruction.FieldsPerFlush + key, thread.Stack[ fp + i.A + key ] );
				}

				if ( i.B == 0 )
				{
					thread.StackWatermark( fp + prototype.StackSize );
				}

				continue;
			}

			case Opcode.Close:
			{
				// close all thread.Stack variables from R( A ) to the top
				thread.CloseUpVals( fp + i.A );
				continue;
			}

			case Opcode.Closure:
			{
				// R( A ) := function closure from P( Bx )
				LuaFunction function = new LuaFunction( prototype.Prototypes[ i.Bx ], Environment );
				thread.Stack[ fp + i.A ] = function;
				
				// followed by upval initialization with Move or GetUpVal
				for ( int upval = 0; upval < function.prototype.UpValCount; ++upval )
				{
					Instruction u = prototype.Instructions[ ip++ ];

					if ( u.Opcode == Opcode.Move )
					{
						function.upVals[ upval ] = thread.MakeUpVal( fp + u.B );
					}
					else if ( u.Opcode == Opcode.GetUpVal )
					{
						function.upVals[ upval ] = upVals[ u.B ];
					}
					else
					{
						throw new InvalidProgramException( "Malformed upval initialization bytecode." );
					}
				}

				continue;
			}

			case Opcode.Vararg:
			{
				// R( A ), ..., R( A + B - 1 ) = vararg

				/*	frameBase		-->	Function
						^				null
					argumentCount		null
						v				argument (vararg)
					framePointer	--> argument
										argument
				*/
				
				// Find varargs.
				int varargBase	= frameBase + 1 + prototype.ParameterCount;
				int varargCount	= Math.Max( fp - varargBase, 0 );

				// Find how many we want.
				int copyCount;
				if ( i.B != 0 )
				{
					copyCount = Math.Min( i.B, varargCount );
				}
				else
				{
					copyCount = varargCount;
					thread.Top = fp + i.A + copyCount - 1;
					thread.StackWatermark( Math.Max( fp + prototype.StackSize, thread.Top + 1 ) );
				}

				// Copy into correct position.
				for ( int vararg = 0; vararg < copyCount; ++vararg )
				{
					thread.Stack[ fp + i.A + vararg ] = thread.Stack[ varargBase + vararg ];
				}

				for ( int vararg = copyCount; vararg < i.B; ++vararg )
				{
					thread.Stack[ fp + i.A + vararg ] = null;
				}

				continue;
			}

			case Opcode.IntDiv:
			{
				// R( A ) := RK( B ) \ RK( C )
				thread.Stack[ fp + i.A ] = RK( thread.Stack, fp, i.B ).IntegerDivide( RK( thread.Stack, fp, i.C ) );
				continue;
			}

			}

		}

		}
		catch ( Exception e )
		{
			thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
			throw e;
		}

	}


	LuaValue K( int operand )
	{
		return prototype.Constants[ operand ];
	}
	

	LuaValue RK( LuaValue[] stack, int fp, int operand )
	{
		if ( Instruction.IsConstant( operand ) )
		{
			return Prototype.Constants[ Instruction.RKToConstant( operand ) ];
		}
		else
		{
			return stack[ fp + operand ];
		}
	}


}


}

