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

		Dispatch( thread, frameBase, resultCount, fp, 0 );
	}

	
	internal override void Resume( LuaThread t )
	{
		throw new NotImplementedException();
	}
	
	
	void Dispatch( LuaThread thread, int frameBase, int resultCount, int fp, int ip )
	{
		LuaValue[] stack = thread.Stack;
		
		try
		{

		while ( true )
		{
			// Suspend coroutine.

			if ( thread.UnwoundFrames.Count > 0 )
			{
				thread.UnwoundFrames.Add( new Frame( frameBase, resultCount, fp, ip ) );
				return;
			}


			// Dispatch instructions.
	
			Instruction i = prototype.Instructions[ ip++ ];

			switch ( i.Opcode )
			{

			case Opcode.Move:
			{
				// R( A ) := R( B )
				stack[ fp + i.A ] = stack[ fp + i.B ];
				continue;
			}

			case Opcode.LoadK:
			{
				// R( A ) := K( Bx )
				stack[ fp + i.A ] = K( i.Bx );
				continue;
			}

			case Opcode.LoadBool:
			{
				// R( A ) := (bool)B
				if ( i.B != 0 )
				{
					stack[ fp + i.A ] = true;
				}
				else
				{
					stack[ fp + i.A ] = false;
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
					stack[ fp + r ] = null;
				}
				continue;
			}
	
			case Opcode.GetUpVal:
			{
				// R( A ) := U( B )
				stack[ fp + i.A ] = upVals[ i.B ].Value;
				continue;
			}

			case Opcode.GetGlobal:
			{
				// R( A ) := G[ K( Bx ) ]
				stack[ fp + i.A ] = Environment.Index( K( i.Bx ) );
				continue;
			}

			case Opcode.GetTable:
			{
				// R( A ) := R( B )[ RK( C ) ]
				stack[ fp + i.A ] = stack[ fp + i.B ].Index( RK( stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.SetGlobal:
			{
				// G[ K( Bx ) ] := R( A )
				Environment.NewIndex( K( i.Bx ), stack[ fp + i.A ] );
				continue;
			}

			case Opcode.SetUpVal:
			{
				// U( B ) := R( A )
				upVals[ i.B ].Value = stack[ fp + i.A ];
				continue;
			}

			case Opcode.SetTable:
			{
				// R( A )[ RK( B ) ] = RK( C )
				stack[ fp + i.A ].NewIndex( RK( stack, fp, i.B ), RK( stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.NewTable:
			{
				// R( A ) := {} ( B is array size hint, C is hash size hint )
				stack[ fp + i.A ] = new LuaTable( i.B, i.C );
				continue;
			}

			case Opcode.Self:
			{
				LuaValue self = stack[ fp + i.B ];

				// R( A + 1 ) := R( B ) 
				stack[ fp + i.A + 1 ] = self;

				// R( A ) = R( B )[ RK( C ) ]
				stack[ fp + i.A ] = self.Index( RK( stack, fp, i.C ) );

				continue;
			}
	
			case Opcode.Add:
			{
				// R( A ) := RK( B ) + RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).Add( RK( stack, fp, i.C ) );
				continue;
			}

			case Opcode.Sub:
			{
				// R( A ) := RK( B ) - RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).Subtract( RK( stack, fp, i.C ) );
				continue;
			}

			case Opcode.Mul:
			{
				// R( A ) := RK( B ) * RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).Multiply( RK( stack, fp, i.C ) );
				continue;
			}

			case Opcode.Div:
			{
				// R( A ) := RK( B ) / RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).Divide( RK( stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.Mod:
			{
				// R( A ) := RK( B ) % RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).Modulus( RK( stack, fp, i.C ) );
				continue;
			}

			case Opcode.Pow:
			{
				// R( A ) := RK( B ) ^ RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).RaiseToPower( RK( stack, fp, i.C ) );
				continue;
			}
	
			case Opcode.Unm:
			{
				// R( A ) := -R( B )
				stack[ fp + i.A ] = stack[ fp + i.B ].UnaryMinus();
				continue;
			}

			case Opcode.Not:
			{
				// R( A ) := not R( B )
				stack[ fp + i.A ] = ! stack[ fp + i.B ].IsTrue();
				continue;
			}

			case Opcode.Len:
			{
				// R( A ) := length of R( B )
				stack[ fp + i.A ] = stack[ fp + i.B ].Length();
				continue;
			}

			case Opcode.Concat:
			{
				// R( A ) := R( B ) .. ... .. R( C ), concatenating whole list
				int listTop	= fp + i.C;
				int count	= i.C - i.B + 1;

				while ( count > 1 )
				{
					LuaValue left	= stack[ listTop - 1 ];
					LuaValue right	= stack[ listTop - 0 ];

					if ( left.SupportsSimpleConcatenation() && right.SupportsSimpleConcatenation() )
					{
						// Count how many we can concatenate in this pass.
						int concatCount = 2;
						for ( concatCount = 2; concatCount < count; ++concatCount )
						{
							LuaValue operand = stack[ listTop - concatCount ];
							if ( ! operand.SupportsSimpleConcatenation() )
							{
								break;
							}
						}

						// Concatenate them.
						StringBuilder s = new StringBuilder();

						for ( int r = listTop - ( concatCount - 1 ); r <= listTop; ++r )
						{
							s.Append( stack[ r ].ToString() );
						}

						// Modify the stack top and continue.
						stack[ listTop - ( concatCount - 1 ) ] = s.ToString();
						listTop	-= concatCount - 1;
						count	-= concatCount - 1;
					}
					else
					{
						// Perform meta concatenation.
						stack[ listTop - 1 ] = left.Concatenate( right );
						listTop	-= 1;
						count	-= 1;
					}
				}

				stack[ fp + i.A ] = stack[ listTop ];

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
				if ( RK( stack, fp, i.B ).Equals( RK( stack, fp, i.C ) ) == ( i.A != 0 ) )
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
				if ( RK( stack, fp, i.B ).LessThan( RK( stack, fp, i.C ) ) == ( i.A != 0 ) )
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
				if ( RK( stack, fp, i.B ).LessThanOrEquals( RK( stack, fp, i.C ) ) == ( i.A != 0 ) )
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
				if ( stack[ fp + i.A ].IsTrue() != ( i.C != 0 ) )
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
				if ( stack[ fp + i.B ].IsTrue() != ( i.C != 0 ) )
				{
					// Set.
					stack[ fp + i.A ] = stack[ fp + i.B ];

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

				LuaValue function = stack[ fp + i.A ];
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

				LuaValue function = stack[ fp + i.A ];
				
				stack[ frameBase ] = function;
				for ( int argument = 0; argument < callArgumentCount; ++argument )
				{
					stack[ frameBase + 1 + argument ] = stack[ fp + i.A + 1 + argument ];
				}

				function.Call( thread, frameBase, callArgumentCount, resultCount );

				return;
			}

			case Opcode.Return:
			{
				// return R( A ), ... R( A + B - 2 )

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
					stack[ frameBase + result ] = stack[ fp + i.A + result ];
				}
				for ( int result = copyCount; result < resultCount; ++result )
				{
					stack[ frameBase + result ] = null;
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
				LuaValue index	= stack[ fp + i.A + 0 ];
				LuaValue limit	= stack[ fp + i.A + 1 ];
				LuaValue step	= stack[ fp + i.A + 2 ];

				// Index += Step
				index = index.Add( step );
				stack[ fp + i.A + 0 ] = index;
				
				// if ( Step > 0 and Index <= Limit ) or ( Step < 0 and Index >= Limit ) then Var = Index, relative jump sBx
				if (    ( ! step.LessThanOrEquals( zero ) && index.LessThanOrEquals( limit ) )
					 || ( step.LessThan( zero ) && ! index.LessThan( limit ) ) )
				{
					stack[ fp + i.A + 3 ] = index;
					ip += i.sBx;
				}

				continue;
			}

			case Opcode.ForPrep:
			{
				LuaValue index	= stack[ fp + i.A + 0 ];
				LuaValue limit	= stack[ fp + i.A + 1 ];
				LuaValue step	= stack[ fp + i.A + 2 ];
				
				// Convert for control variables to numbers.
				if ( ! index.TryToNumberValue( out index ) )
				{
					throw new InvalidOperationException( "'for' initial value must be a number." );
				}
				if ( ! index.TryToNumberValue( out limit ) )
				{
					throw new InvalidOperationException( "'for' limit must be a number." );
				}
				if ( ! index.TryToNumberValue( out step ) )
				{
					throw new InvalidOperationException( "'for' step must be a number." );
				}

				// Index -= Step
				index = index.Subtract( step );

				// Update stack.
				stack[ fp + i.A + 0 ] = index;
				stack[ fp + i.A + 1 ] = limit;
				stack[ fp + i.A + 2 ] = step;
				
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
				LuaValue generator	= stack[ fp + i.A + 0 ];
				LuaValue state		= stack[ fp + i.A + 1 ];
				LuaValue control	= stack[ fp + i.A + 2 ];

				// Var_1, ..., Var_C := Generator( State, Control )
				stack[ fp + i.A + 3 ] = generator;
				stack[ fp + i.A + 4 ] = state;
				stack[ fp + i.A + 5 ] = control;
				generator.Call( thread, fp + i.A + 3, 2, i.C );

				// if Var_1 ~= nil then Control = Var_1 else skip associated jump
				LuaValue var_1 = stack[ fp + i.A + 3 ];
				if ( var_1.IsTrue() )
				{
					// Set control.
					stack[ fp + i.A + 2 ] = var_1;

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
					stack[ fp + i.A ].NewIndex( ( keyBase - 1 ) * Instruction.FieldsPerFlush + key, stack[ fp + i.A + key ] );
				}

				if ( i.B == 0 )
				{
					thread.StackWatermark( fp + prototype.StackSize );
				}

				continue;
			}

			case Opcode.Close:
			{
				// close all stack variables from R( A ) to the top
				thread.CloseUpVals( fp + i.A );
				continue;
			}

			case Opcode.Closure:
			{
				// R( A ) := function closure from P( Bx )
				LuaFunction function = new LuaFunction( prototype.Prototypes[ i.Bx ], Environment );
				stack[ fp + i.A ] = function;
				
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
					stack = thread.Stack;
				}

				// Copy into correct position.
				for ( int vararg = 0; vararg < copyCount; ++vararg )
				{
					stack[ fp + i.A + vararg ] = stack[ varargBase + vararg ];
				}

				for ( int vararg = copyCount; vararg < i.B; ++vararg )
				{
					stack[ fp + i.A + vararg ] = null;
				}

				continue;
			}

			case Opcode.IntDiv:
			{
				// R( A ) := RK( B ) \ RK( C )
				stack[ fp + i.A ] = RK( stack, fp, i.B ).IntegerDivide( RK( stack, fp, i.C ) );
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

