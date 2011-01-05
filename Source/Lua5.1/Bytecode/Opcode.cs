// Opcode.cs
// 
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua.Bytecode
{


enum Opcode
{
	/*
		R( x )	means the xth register
		K( x )	means the xth constant
		RK( x )	means either R( x ) when x < K, or K( x - K ) when x >= K
		U( x )	means the xth upvalue
		P( x )	means the xth function prototype
		G		is the global table

		Test instructions are ALWAYS followed by a Jmp instruction.  The interpreter relies
		on this behaviour, as it decodes the instruction pair together.

		ForLoop and ForPrep act on a set of four consecutive registers:
			A + 0		Index
			A + 1		Limit
			A + 2		Step
			A + 3		Var

		TForLoop acts on a set of consecutive registers:
			A + 0		Generator
			A + 1		State
			A + 2		Control
			A + 3		Var_1
			...
			A + 3 + C	Var_C

		Where Var are the variables declared by the user as part of the for loop statement.

		Call (when C == 0) and Vararg (when B == 0), place all values returned onto the
		stack, and set TOP to the index of the last register modified.

		Call (when B == 0), Return (when B == 0) and SetList (when B == 0), use all values
		up to TOP.

		SetList (when C == 0) gets the value of C from the next instruction, interpreting
		the entire instruction word as an int.

		Test will jump when the truthiness of R( A ) matches C.  TestSet additionally
		updates R( A ) before jumping.
	*/


	Move,		// A B		R( A ) := R( B )
	LoadK,		// A Bx		R( A ) := K( Bx )
	LoadBool,	// A B C	R( A ) := (bool)B; if C skip next instruction
	LoadNil,	// A B		R( A ) ... R( B ) := nil
	
	GetUpVal,	// A B		R( A ) := U( B )
	GetGlobal,	// A Bx		R( A ) := G[ K( Bx ) ]
	GetTable,	// A B C	R( A ) := R( B )[ RK( C ) ]
	
	SetGlobal,	// A Bx		G[ K( Bx ) ] := R( A )
	SetUpVal,	// A B		U( B ) := R( A )
	SetTable,	// A B C	R( A )[ RK( B ) ] = RK( C )
	
	NewTable,	// A B C	R( A ) := {} ( B is array size hint, C is hash size hint )

	Self,		// A B C	R( A + 1 ) := R( B ); R( A ) = R( B )[ RK( C ) ]
	
	Add,		// A B C	R( A ) := RK( B ) + RK( C )
	Sub,		// A B C	R( A ) := RK( B ) - RK( C )
	Mul,		// A B C	R( A ) := RK( B ) * RK( C )
	Div,		// A B C	R( A ) := RK( B ) / RK( C )
	Mod,		// A B C	R( A ) := RK( B ) % RK( C )
	Pow,		// A B C	R( A ) := RK( B ) ^ RK( C )
	Unm,		// A B		R( A ) := -R( B )
	Not,		// A B		R( A ) := not R( B )
	Len,		// A B		R( A ) := length of R( B )
	Concat,		// A B C	R( A ) := R( B ) .. ... .. R( C ), concatenating whole list

	Jmp,		// sBx		relative jump sBx ( relative to next instruction )
	
	Eq,			// A B C	if ( RK( B ) == RK( C ) ) ~= A then skip associated jump
	Lt,			// A B C	if ( RK( B ) <  RK( C ) ) ~= A then skip associated jump
	Le,			// A B C	if ( RK( B ) <= RK( C ) ) ~= A then skip associated jump
	Test,		// A C		if not ( R( A ) <=> C ) then skip associated jump
	TestSet,	// A B C	if ( R( B ) <=> C ) then R( A ) := R( B ) else skip associated jump

	Call,		// A B C	R( A ), ... , R( A + C - 2 ) := R( A )( R( A + 1 ), ... , R( A + B - 1 ) )
	TailCall,	// A B		return R( A )( R( A + 1 ), ... , R( A + B - 1 ) )
	Return,		// A B		return R( A ), ... R( A + B - 2 )

	ForLoop,	// A sBx	Index += Step; if Index <= Limit then Var = Index, relative jump sBx
	ForPrep,	// A sBx	Index -= Step; relative jump sBx
	
	TForLoop,	// A C		Var_1, ..., Var_C := Generator( State, Control ); if Var_1 ~= nil then Control = Var_1 else skip associated jump
	
	SetList,	// A B C	R( A )[ ( C - 1 ) * 50 + i ] := R( A + i ), 1 <= i <= B

	Close,		// A		close all stack variables from R( A ) to the top
	Closure,	// A Bx		R( A ) := function closure from P( Bx ), followed by upval initialization with Move or GetUpVal

	Vararg,		// A B		R( A ), ..., R( A + B - 1 ) = vararg

	IntDiv,		// A B C	R( A ) := RK( B ) \ RK( C )

}


}


