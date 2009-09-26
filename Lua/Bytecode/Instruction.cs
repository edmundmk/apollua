// Instruction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Bytecode
{


public struct Instruction
{
	/*
		Each instruction is packed into a 32-bit uint using the
		following bit layout:
	
			big end  |   B   |   C   |  A  | Op |  little end
	*/

	uint i;


	// Bit pattern layout.

	const int sizeOpcode			= 6;
	const int sizeA					= 8;
	const int sizeC					= 9;
	const int sizeB					= 9;
	const int sizeBx				= sizeC + sizeB;
	
	const int posOpcode				= 0;
	const int posA					= posOpcode + sizeOpcode;
	const int posC					= posA + sizeA;
	const int posB					= posC + sizeC;
	const int posBx					= posC;

	const uint maskOpcode			= 0x0000003F;
	const uint maskA				= 0x00003FC0;
	const uint maskC				= 0x007FC000;
	const uint maskB				= 0xFF800000;
	const uint maskBx				= maskC | maskB;

	const int maxArgA				= ( 1 << sizeA ) - 1;
	const int maxArgC				= ( 1 << sizeC ) - 1;
	const int maxArgB				= ( 1 << sizeB ) - 1;
	const int maxArgBx				= ( 1 << sizeBx ) - 1;
	const int maxArgsBx				= maxArgBx >> 1;

	const int bitRK					= 1 << ( sizeB - 1 );


	// Operand information.

	public const int FieldsPerFlush	= 50;
	
	public static bool IsConstant( int rk )
	{
		return ( rk & bitRK ) != 0;
	}

	public static bool IsRK( int rk )
	{
		return ( rk & bitRK ) == 0;
	}

	public static int ConstantToRK( int k )
	{
		return k | bitRK;
	}

	public static int RKToConstant( int rk )
	{
		return rk & ~bitRK;
	}

	public static bool InRangeRK( int k )
	{
		return k < bitRK;
	}
	
	public static bool InRangeC( int C )
	{
		return C <= maxArgC;
	}

	
	// Argument accessors.

	public Opcode Opcode
	{
		get { return (Opcode)( ( i & maskOpcode ) >> posOpcode ); }
		private set { i &= ~maskOpcode; i |= (uint)value << posOpcode; }
	}

	public int A
	{
		get { return (int)( ( i & maskA ) >> posA ); }
		private set { i &= ~maskA; i |= (uint)value << posA; }
	}

	public int B
	{
		get { return (int)( ( i & maskB ) >> posB ); }
		private set { i &= ~maskB; i |= (uint)value << posB; }
	}

	public int C
	{
		get { return (int)( ( i & maskC ) >> posC ); }
		private set { i &= ~maskC; i |= (uint)value << posC; }
	}

	public int Bx
	{
		get { return (int)( ( i & maskBx ) >> posBx ); }
		private set { i &= ~maskBx; i |= (uint)value << posBx; }
	}

	public int sBx
	{
		get { return Bx - maxArgsBx; }
		private set { Bx = value + maxArgsBx; }
	}

	public int Index
	{
		get { return (int)i; }
		private set { i = (uint)value; }
	}



	// Factory methods.

	public static Instruction CreateABC( Opcode opcode, int A, int B, int C )
	{
		Instruction i = new Instruction();
		i.Opcode	= opcode;
		i.A			= A;
		i.B			= B;
		i.C			= C;
		return i;
	}

	public static Instruction CreateABx( Opcode opcode, int A, int Bx )
	{
		Instruction i = new Instruction();
		i.Opcode	= opcode;
		i.A			= A;
		i.Bx		= Bx;
		return i;
	}

	public static Instruction CreateAsBx( Opcode opcode, int A, int sBx )
	{
		Instruction i = new Instruction();
		i.Opcode	= opcode;
		i.A			= A;
		i.sBx		= sBx;
		return i;
	}

	public static Instruction CreateIndex( int C )
	{
		Instruction i = new Instruction();
		i.Index = C;
		return i;
	}

}


}

