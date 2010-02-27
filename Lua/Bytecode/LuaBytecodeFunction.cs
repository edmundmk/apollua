// LuaBytecodeFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Lua;
using Lua.Runtime;
using Lua.Interop;


namespace Lua.Bytecode
{


public sealed class LuaBytecodeFunction
	:	LuaFunction
{
	UpVal[]		UpVals;
	LuaBytecode	Prototype;


	public LuaBytecodeFunction( LuaBytecode prototype )
	{
		UpVals		= new UpVal[ prototype.UpValCount ];
		Prototype	= prototype;
	}

	

	// Bytecode function interface.

	public override FrozenFrame Call( LuaThread t, int f, int a, int r )
	{
		int fp = CreateStackFrame( t, f, a );
		return Dispatch( t, f, r, fp, 0 );
	}

	public override FrozenFrame Resume( LuaThread t, FrozenFrame f )
	{
		FrozenFrame refrozenFrame = UnfreezeStackFrame( t, f );
		if ( refrozenFrame != null )
			return refrozenFrame;
		else
			return Dispatch( t, f.FrameBase, f.ResultCount, f.FramePointer, f.InstructionPointer );
	}


	// IL function interface.

	public override LuaValue InvokeS()															{ throw new NotImplementedException(); }
	public override LuaValue InvokeS( LuaValue a1 )												{ throw new NotImplementedException(); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )								{ throw new NotImplementedException(); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )					{ throw new NotImplementedException(); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ throw new NotImplementedException(); }
	public override LuaValue InvokeS( LuaValue[] arguments )									{ throw new NotImplementedException(); }

	public override LuaValue[] InvokeM()														{ throw new NotImplementedException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )											{ throw new NotImplementedException(); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )								{ throw new NotImplementedException(); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )					{ throw new NotImplementedException(); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ throw new NotImplementedException(); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )									{ throw new NotImplementedException(); }

	
	// Interop function interface.

	protected override Delegate MakeDelegate( Type delegateType )
	{
		MethodInfo delegateMethod = delegateType.GetMethod( "Invoke" );
		MethodInfo invokeMethod = InteropHelpers.BindInteropSignature( null, delegateMethod, interopBindTable );
		return Delegate.CreateDelegate( delegateType, this, invokeMethod );
	}

		

	// Constants.

	static readonly LuaValue zero = 0;


	// Bytecode interpreter.
		
	int CreateStackFrame( LuaThread thread, int frameBase, int argumentCount )
	{
		thread.BeginFrame( this );

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
			thread.StackWatermark( fp, fp + Prototype.StackSize );

			for ( int argument = 0; argument < argumentCount; ++argument )
			{
				thread.Values[ fp + argument ] = thread.Values[ frameBase + 1 + argument ];
				thread.Values[ frameBase + 1 + argument ] = null;
			}
		}
		else
		{
			fp = frameBase + 1;
			thread.StackWatermark( fp, fp + Prototype.StackSize );	
		}

		return fp;
	}


	FrozenFrame UnfreezeStackFrame( LuaThread thread, FrozenFrame frozenFrame )
	{
		IList< LuaValue > values = thread.Values;
		int fp = frozenFrame.FramePointer;
		int ip = frozenFrame.InstructionPointer;

		// Resume function that suspended us.
		Instruction i = Prototype.Instructions[ ip - 1 ];
		LuaValue function = values[ fp + i.A ];
		FrozenFrame refrozenFrame = function.Resume( thread, frozenFrame.NextFrame );
		if ( refrozenFrame != null )
		{
			// Refreeze.
			refrozenFrame = new FrozenFrame( refrozenFrame, frozenFrame.FrameBase, frozenFrame.ResultCount, fp, ip );
			return refrozenFrame;
		}

		// Function call completed normally, dispatch.
		return null;
	}


	FrozenFrame Dispatch( LuaThread thread, int frameBase, int resultCount, int fp, int ip )
	{
		IList< LuaValue > values = thread.Values;
		
		while ( true )
		{
			Instruction i = Prototype.Instructions[ ip++ ];

			switch ( i.Opcode )
			{

			case Opcode.Move:
			{
				// R( A ) := R( B )
				values[ fp + i.A ] = values[ fp + i.B ];
				continue;
			}

			case Opcode.LoadK:
			{
				// R( A ) := K( Bx )
				values[ fp + i.A ] = K( i.Bx );
				continue;
			}

			case Opcode.LoadBool:
			{
				// R( A ) := (bool)B
				if ( i.B != 0 )
				{
					values[ fp + i.A ] = true;
				}
				else
				{
					values[ fp + i.A ] = false;
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
					values[ fp + r ] = null;
				}
				continue;
			}
	
			case Opcode.GetUpVal:
			{
				// R( A ) := U( B )
				values[ fp + i.A ] = UpVals[ i.B ].Value;
				continue;
			}

			case Opcode.GetGlobal:
			{
				// R( A ) := G[ K( Bx ) ]
				values[ fp + i.A ] = Environment.Index( K( i.Bx ) );
				continue;
			}

			case Opcode.GetTable:
			{
				// R( A ) := R( B )[ RK( C ) ]
				values[ fp + i.A ] = values[ fp + i.B ].Index( RK( values, fp, i.C ) );
				continue;
			}
	
			case Opcode.SetGlobal:
			{
				// G[ K( Bx ) ] := R( A )
				Environment.NewIndex( K( i.Bx ), values[ fp + i.A ] );
				continue;
			}

			case Opcode.SetUpVal:
			{
				// U( B ) := R( A )
				UpVals[ i.B ].Value = values[ fp + i.A ];
				continue;
			}

			case Opcode.SetTable:
			{
				// R( A )[ RK( B ) ] = RK( C )
				values[ fp + i.A ].NewIndex( RK( values, fp, i.B ), RK( values, fp, i.C ) );
				continue;
			}
	
			case Opcode.NewTable:
			{
				// R( A ) := {} ( B is array size hint, C is hash size hint )
				values[ fp + i.A ] = new LuaTable( i.B, i.C );
				continue;
			}

			case Opcode.Self:
			{
				LuaValue self = values[ fp + i.B ];

				// R( A + 1 ) := R( B ) 
				values[ fp + i.A + 1 ] = self;

				// R( A ) = R( B )[ RK( C ) ]
				values[ fp + i.A ] = self.Index( RK( values, fp, i.C ) );

				continue;
			}
	
			case Opcode.Add:
			{
				// R( A ) := RK( B ) + RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).Add( RK( values, fp, i.C ) );
				continue;
			}

			case Opcode.Sub:
			{
				// R( A ) := RK( B ) - RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).Subtract( RK( values, fp, i.C ) );
				continue;
			}

			case Opcode.Mul:
			{
				// R( A ) := RK( B ) * RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).Multiply( RK( values, fp, i.C ) );
				continue;
			}

			case Opcode.Div:
			{
				// R( A ) := RK( B ) / RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).Divide( RK( values, fp, i.C ) );
				continue;
			}
	
			case Opcode.Mod:
			{
				// R( A ) := RK( B ) % RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).Modulus( RK( values, fp, i.C ) );
				continue;
			}

			case Opcode.Pow:
			{
				// R( A ) := RK( B ) ^ RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).RaiseToPower( RK( values, fp, i.C ) );
				continue;
			}
	
			case Opcode.Unm:
			{
				// R( A ) := -R( B )
				values[ fp + i.A ] = values[ fp + i.B ].UnaryMinus();
				continue;
			}

			case Opcode.Not:
			{
				// R( A ) := not R( B )
				values[ fp + i.A ] = ! values[ fp + i.B ].IsTrue();
				continue;
			}

			case Opcode.Len:
			{
				// R( A ) := length of R( B )
				values[ fp + i.A ] = values[ fp + i.B ].Length();
				continue;
			}

			case Opcode.Concat:
			{
				// R( A ) := R( B ) .. ... .. R( C ), concatenating whole list
				int listTop	= fp + i.C;
				int count	= i.C - i.B + 1;

				while ( count > 1 )
				{
					LuaValue left	= values[ listTop - 1 ];
					LuaValue right	= values[ listTop - 0 ];

					if ( left.IsPrimitiveValue() && right.IsPrimitiveValue() )
					{
						// Count how many we can concatenate in this pass.
						int concatCount = 2;
						for ( concatCount = 2; concatCount < count; ++concatCount )
						{
							LuaValue operand = values[ listTop - concatCount ];
							if ( ! operand.IsPrimitiveValue() )
							{
								break;
							}
						}

						// Concatenate them.
						StringBuilder s = new StringBuilder();

						for ( int r = listTop - ( concatCount - 1 ); r <= listTop; ++r )
						{
							s.Append( values[ r ].ToString() );
						}

						// Modify the stack top and continue.
						values[ listTop - ( concatCount - 1 ) ] = s.ToString();
						listTop	-= concatCount - 1;
						count	-= concatCount - 1;
					}
					else
					{
						// Perform meta concatenation.
						values[ listTop - 1 ] = left.Concatenate( right );
						listTop	-= 1;
						count	-= 1;
					}
				}

				values[ fp + i.A ] = values[ listTop ];

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
				if ( RK( values, fp, i.B ).EqualsValue( RK( values, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = Prototype.Instructions[ ip++ ];
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
				if ( RK( values, fp, i.B ).LessThanValue( RK( values, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = Prototype.Instructions[ ip++ ];
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
				if ( RK( values, fp, i.B ).LessThanOrEqualsValue( RK( values, fp, i.C ) ) == ( i.A != 0 ) )
				{
					i = Prototype.Instructions[ ip++ ];
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
				if ( values[ fp + i.A ].IsTrue() != ( i.C != 0 ) )
				{
					i = Prototype.Instructions[ ip++ ];
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
				if ( values[ fp + i.B ].IsTrue() != ( i.C != 0 ) )
				{
					// Set.
					values[ fp + i.A ] = values[ fp + i.B ];

					// Perform associated jump.
					i = Prototype.Instructions[ ip++ ];
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
				if ( i.B == 0 )
				{
					callArgumentCount = thread.Top - i.A - 1;
				}
				else
				{
					callArgumentCount = i.B - 1;
				}


				// Call function 
				LuaValue function = values[ fp + i.A ];
				
				FrozenFrame frozenFrame = function.Call( thread, fp + i.A, callArgumentCount, i.C - 1 );
				if ( frozenFrame != null )
				{
					// Freeze this function call.

					frozenFrame = new FrozenFrame( frozenFrame, frameBase, resultCount, fp, ip );
					return frozenFrame;
				}
				

				// Trim stack frame used by function.
				int stackWatermark = fp + Prototype.StackSize;
				if ( i.C == 0 )
				{
					stackWatermark = Math.Max( stackWatermark, thread.Top );
				}
				thread.StackWatermark( stackWatermark, stackWatermark );

				continue;
			}

			case Opcode.TailCall:
			{
				// return R( A )( R( A + 1 ), ... , R( A + B - 1 ) )

				int callArgumentCount;
				if ( i.B == 0 )
				{
					callArgumentCount = thread.Top - i.A - 1;
				}
				else
				{
					callArgumentCount = i.B - 1;
				}

				LuaValue function = values[ fp + i.A ];
				
				values[ frameBase ] = function;
				for ( int argument = 0; argument < callArgumentCount; ++argument )
				{
					values[ frameBase + 1 + argument ] = values[ fp + i.A + 1 + argument ];
				}

				thread.EndFrame( this );
				return function.Call( thread, frameBase, callArgumentCount, resultCount );
			}

			case Opcode.Return:
			{
				// return R( A ), ... R( A + B - 2 )

				// Find number of results we have.
				int returnResultCount;
				if ( i.B == 0 )
				{
					returnResultCount = thread.Top - i.A;
				}
				else
				{
					returnResultCount = i.B - 1;
				}
				
				// Calculate number of results we want.
				int copyCount;
				if ( resultCount == -1 )
				{
					copyCount = returnResultCount;
					thread.Top = frameBase + returnResultCount;
				}
				else
				{
					copyCount = Math.Min( resultCount, returnResultCount );
				}

				// Copy results.
				for ( int result = 0; result < copyCount; ++result )
				{
					values[ frameBase + result ] = values[ fp + i.A + result ];
				}
				for ( int result = copyCount; result < resultCount; ++result )
				{
					values[ frameBase + result ] = null;
				}

				// Function complete.
				return null;
			}


			/*	A + 0		Index
				A + 1		Limit
				A + 2		Step
				A + 3		Var
			*/

			case Opcode.ForLoop:
			{
				LuaValue index	= values[ fp + i.A + 0 ];
				LuaValue limit	= values[ fp + i.A + 1 ];
				LuaValue step	= values[ fp + i.A + 2 ];

				// Index += Step
				index = index.Add( step );
				values[ fp + i.A + 0 ] = index;
				
				// if ( Step > 0 and Index <= Limit ) or ( Step < 0 and Index >= Limit ) then Var = Index, relative jump sBx
				if (    ( ! step.LessThanOrEqualsValue( zero ) && index.LessThanOrEqualsValue( limit ) )
					 || ( step.LessThanValue( zero ) && ! index.LessThanValue( limit ) ) )
				{
					values[ fp + i.A + 3 ] = index;
					ip += i.sBx;
				}
				continue;
			}

			case Opcode.ForPrep:
			{
				LuaValue index	= values[ fp + i.A + 0 ];
				LuaValue limit	= values[ fp + i.A + 1 ];
				LuaValue step	= values[ fp + i.A + 2 ];
				
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

				// Update values.
				values[ fp + i.A + 0 ] = index;
				values[ fp + i.A + 1 ] = limit;
				values[ fp + i.A + 2 ] = step;
				
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
				LuaValue generator	= values[ fp + i.A + 0 ];
				LuaValue state		= values[ fp + i.A + 1 ];
				LuaValue control	= values[ fp + i.A + 2 ];

				// Var_1, ..., Var_C := Generator( State, Control )
				values[ fp + i.A + 3 ] = generator;
				values[ fp + i.A + 4 ] = state;
				values[ fp + i.A + 5 ] = control;
				generator.Call( thread, fp + i.A + 3, 2, i.C );

				// if Var_1 ~= nil then Control = Var_1 else skip associated jump
				LuaValue var_1 = values[ fp + i.A + 3 ];
				if ( var_1.IsTrue() )
				{
					// Set control.
					values[ fp + i.A + 2 ] = var_1;

					// Perform associated jump.
					i = Prototype.Instructions[ ip++ ];
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
				int C = i.C;
				if ( C == 0 )
				{
					Instruction index = Prototype.Instructions[ ip++ ];
					C = index.Index;
				}
	
				// R( A )[ ( C - 1 ) * 50 + i ] := R( A + i ), 1 <= i <= B
				for ( int key = 1; key < i.B; ++key )
				{
					values[ fp + i.A ].NewIndex( ( C - 1 ) * Instruction.FieldsPerFlush + key, values[ fp + i.A + key ] );
				}

				if ( i.C == 0 )
				{
					thread.StackWatermark( fp + Prototype.StackSize, fp + Prototype.StackSize );
				}

				continue;
			}

			case Opcode.Close:
			{
				// close all stack variables from R( A ) to the top
				thread.CloseUpVals( fp + i.A );
				thread.StackWatermark( fp + i.A, fp + Prototype.StackSize );
				continue;
			}

			case Opcode.Closure:
			{
				// R( A ) := function closure from P( Bx )
				LuaBytecodeFunction function = new LuaBytecodeFunction( Prototype.Prototypes[ i.Bx ] );
				function.Environment = Environment;
				values[ fp + i.A ] = function;
				
				// followed by upval initialization with Move or GetUpVal
				for ( int upval = 0; upval < function.Prototype.UpValCount; ++upval )
				{
					Instruction u = Prototype.Instructions[ ip++ ];

					if ( u.Opcode == Opcode.Move )
					{
						function.UpVals[ upval ] = thread.MakeUpVal( fp + u.B );
					}
					else if ( u.Opcode == Opcode.GetUpVal )
					{
						function.UpVals[ upval ] = UpVals[ u.B ];
					}
					else
					{
						throw new InvalidOperationException( "Malformed upval initialization bytecode." );
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
				int varargBase	= frameBase + 1 + Prototype.ParameterCount;
				int varargCount	= Math.Max( fp - varargBase, 0 );

				// Find how many we want.
				int copyCount;
				if ( i.B == 0 )
				{
					copyCount = varargCount;
					thread.Top = fp + i.A + i.B;
				}
				else
				{
					copyCount = Math.Min( i.B, varargCount );
				}

				// Copy into correct position.
				for ( int vararg = 0; vararg < copyCount; ++vararg )
				{
					values[ fp + i.A + vararg ] = values[ varargBase + vararg ];
				}

				for ( int vararg = copyCount; vararg < i.B; ++vararg )
				{
					values[ fp + i.A + vararg ] = null;
				}

				continue;
			}

			case Opcode.IntDiv:
			{
				// R( A ) := RK( B ) \ RK( C )
				values[ fp + i.A ] = RK( values, fp, i.B ).IntegerDivide( RK( values, fp, i.C ) );
				continue;
			}

			}

		}
	}


	LuaValue K( int operand )
	{
		return Prototype.Constants[ operand ];
	}
	

	LuaValue RK( IList< LuaValue > values, int fp, int operand )
	{
		if ( Instruction.IsConstant( operand ) )
		{
			return Prototype.Constants[ Instruction.RKToConstant( operand ) ];
		}
		else
		{
			return values[ fp + operand ];
		}
	}



	


	// Interop bind table.

	static readonly MethodInfo[][] interopBindTable;

	static LuaBytecodeFunction()
	{
		// Create bind table.
		interopBindTable = new MethodInfo[][]
		{
			new MethodInfo[ 5 ],
			new MethodInfo[ 6 ],
			new MethodInfo[ 6 ],
			new MethodInfo[ 7 ],
			new MethodInfo[ 6 ],
			new MethodInfo[ 7 ]
		};

		// Fill it with the appropriate MethodInfo objects.
		foreach ( MethodInfo methodInfo in typeof( LuaBytecodeFunction ).GetMethods(
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic ) )
		{
			int typeParametersCount =
				methodInfo.IsGenericMethodDefinition ? methodInfo.GetGenericArguments().Length : 0;

			Console.WriteLine( methodInfo.Name );

			if ( methodInfo.Name == "InteropV" )
				interopBindTable[ 0 ][ typeParametersCount ] = methodInfo;
			else if ( methodInfo.Name == "InteropVP" )
				interopBindTable[ 1 ][ typeParametersCount ] = methodInfo;
			else if ( methodInfo.Name == "InteropS" )
				interopBindTable[ 2 ][ typeParametersCount ] = methodInfo;
			else if ( methodInfo.Name == "InteropSP" )
				interopBindTable[ 3 ][ typeParametersCount ] = methodInfo;
			else if ( methodInfo.Name == "InteropM" )
				interopBindTable[ 4 ][ typeParametersCount ] = methodInfo;
			else if ( methodInfo.Name == "InteropMP" )
				interopBindTable[ 5 ][ typeParametersCount ] = methodInfo;
		}
	}




	// Interop delegate functions.

	void InteropV()
	{
		LuaThread thread = LuaThread.GetCurrent();
		int frameBase = thread.BeginInterop( this, 0 );
		Call( thread, frameBase, 0, 0 );
		thread.EndInterop( frameBase );
	}

	void InteropV< T >( T a1 ) {}
	void InteropV< T1, T2 >( T1 a1, T2 a2 ) {}
	void InteropV< T1, T2, T3 >( T1 a1, T2 a2, T3 a3 ) {}
	void InteropV< T1, T2, T3, T4 >( T1 a1, T2 a2, T3 a3, T4 a4 ) {}

	void InteropVP< TParams >( params TParams[] arguments ) {}
	void InteropVP< T, TParams >( T a1, params TParams[] arguments ) {}
	void InteropVP< T1, T2, TParams >( T1 a1, T2 a2, params TParams[] arguments ) {}
	void InteropVP< T1, T2, T3, TParams >( T1 a1, T2 a2, T3 a3, params TParams[] arguments ) {}
	void InteropVP< T1, T2, T3, T4, TParams >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments ) {}

	TResult InteropS< TResult >() { return default( TResult ); }
	TResult InteropS< T, TResult >( T a1 ) { return default( TResult ); }
	TResult InteropS< T1, T2, TResult >( T1 a1, T2 a2 ) { return default( TResult ); }
	TResult InteropS< T1, T2, T3, TResult >( T1 a1, T2 a2, T3 a3 ) { return default( TResult ); }
	TResult InteropS< T1, T2, T3, T4, TResult >( T1 a1, T2 a2, T3 a3, T4 a4 ) { return default( TResult ); }

	TResult InteropSP< TParams, TResult >( TParams[] arguments ) { return default( TResult ); }
	TResult InteropSP< T, TParams, TResult >( T a1, TParams[] arguments ) { return default( TResult ); }
	TResult InteropSP< T1, T2, TParams, TResult >( T1 a1, T2 a2, TParams[] arguments ) { return default( TResult ); }
	TResult InteropSP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, TParams[] arguments ) { return default( TResult ); }
	TResult InteropSP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, TParams[] arguments ) { return default( TResult ); }

	TResult[] InteropM< TResult >() { return new TResult[] {}; }
	TResult[] InteropM< T, TResult >( T a1 ) { return new TResult[] {}; }
	TResult[] InteropM< T1, T2, TResult >( T1 a1, T2 a2 ) { return new TResult[] {}; }
	TResult[] InteropM< T1, T2, T3, TResult >( T1 a1, T2 a2, T3 a3 ) { return new TResult[] {}; }
	TResult[] InteropM< T1, T2, T3, T4, TResult >( T1 a1, T2 a2, T3 a3, T4 a4 ) { return new TResult[] {}; }

	TResult[] InteropMP< TParams, TResult >( TParams[] arguments ) { return new TResult[] {}; }
	TResult[] InteropMP< T, TParams, TResult >( T a1, TParams[] arguments ) { return new TResult[] {}; }
	TResult[] InteropMP< T1, T2, TParams, TResult >( T1 a1, T2 a2, TParams[] arguments ) { return new TResult[] {}; }
	TResult[] InteropMP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, TParams[] arguments ) { return new TResult[] {}; }
	TResult[] InteropMP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, TParams[] arguments ) { return new TResult[] {}; }






}
	

}


