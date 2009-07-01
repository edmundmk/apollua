// PrototypeWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;


namespace Lua.VM
{


public class PrototypeWriter
{
	protected TextWriter	o;
	protected Prototype		prototype;
	protected int			ip;


	public PrototypeWriter( TextWriter oWriter )
	{
		o			= oWriter;
		prototype	= null;
		ip			= 0;
	}

	

	public void Write( Prototype p )
	{
		prototype = p;


		// Function signature.

		o.Write( "function " );
		if ( prototype.DebugName != null )
		{
			o.Write( prototype.DebugName );
		}
		else
		{
			o.Write( "x" );
			o.Write( prototype.GetHashCode().ToString( "X" ) );
		}
		o.Write( "(" );
		if ( prototype.ParameterCount > 0 || prototype.IsVararg )
		{
			o.Write( " " );
			bool bFirst = true;
			for ( int i = 0; i < prototype.ParameterCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugLocals[ i ].Name );
			}
			if ( prototype.IsVararg )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( "..." );
			}
			o.Write( " " );
		}
		o.WriteLine( ")" );


		// Information.

		o.WriteLine( " -- {0} registers, {1} constants",
			prototype.StackSize,
			prototype.Constants.Length );

		
		// Upvals.

		if ( prototype.UpValCount > 0 )
		{
			o.Write( "  -- upval " );
			bool bFirst = true;
			for ( int i = 0; i < prototype.UpValCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugUpValNames[ i ] );
			}
			o.WriteLine();
		}


		// Locals.

		if ( prototype.DebugLocals.Length > prototype.ParameterCount )
		{
			o.Write( "  -- local " );
			bool bFirst = true;
			for ( int i = prototype.ParameterCount; i < prototype.DebugLocals.Length; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugLocals[ i ].Name );
			}
			o.WriteLine();
		}


		// Instructions.

		for ( ip = 0; ip < prototype.Instructions.Length; ++ip )
		{
			WriteInstruction();
			if ( prototype.Instructions[ ip ].Opcode == Opcode.Closure )
			{
				Prototype closure = prototype.Prototypes[ prototype.Instructions[ ip ].Bx ];
				for ( int upval = 0; upval < closure.UpValCount; ++upval )
				{
					Instruction i = prototype.Instructions[ ip + 1 + upval ];
					if ( i.Opcode == Opcode.Move )
					{
						o.WriteLine( "              local   {0} {1}", upval,
							OperandString( Mode.R, i.B ) ); 
					}
					else if ( i.Opcode == Opcode.GetUpVal )
					{
						o.WriteLine( "              upval   {0} {1}", upval,
							OperandString( Mode.U, i.B ) );
					}

				}
				ip += closure.UpValCount;
			}
		}
		

		// Final.

		o.WriteLine( "end" );
		o.WriteLine();


		// Other functions.

		for ( int i = 0; i < p.Prototypes.Length; ++i )
		{
			Write( p.Prototypes[ i ] );
		}
	}


	// Information about instructions.

	enum Operands
	{
		ABC,
		ABx,
		Jump,
		SetListABC,
	}

	enum Mode
	{
		Unused,
		R,
		K,
		RK,
		U,
		P,
		Integer,
		Bool,
		Jump,
	}

	struct OpMetadata
	{
		public Operands	Operands	{ get; private set; }
		public Mode		A			{ get; private set; }
		public Mode		B			{ get; private set; }
		public Mode		C			{ get; private set; }


		public OpMetadata( Operands operands, Mode a, Mode b, Mode c )
			:	this()
		{
			Operands	= operands;
			A			= a;
			B			= b;
			C			= c;
		}
	}

	static readonly Dictionary< Opcode, OpMetadata > opcodeMetadata = new Dictionary< Opcode, OpMetadata >()
	{
		{ Opcode.Move,		new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Unused ) },
		{ Opcode.LoadK,		new OpMetadata( Operands.ABx, Mode.R, Mode.K, Mode.Unused ) },
		{ Opcode.LoadBool,	new OpMetadata( Operands.ABC, Mode.R, Mode.Bool, Mode.Unused ) },
		{ Opcode.LoadNil,	new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Unused ) },
		{ Opcode.GetUpVal,	new OpMetadata( Operands.ABC, Mode.R, Mode.U, Mode.Unused ) },
		{ Opcode.GetGlobal,	new OpMetadata( Operands.ABx, Mode.R, Mode.K, Mode.Unused ) },
		{ Opcode.GetTable,	new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.RK ) },
		{ Opcode.SetGlobal,	new OpMetadata( Operands.ABx, Mode.R, Mode.K, Mode.Unused ) },
		{ Opcode.SetUpVal,	new OpMetadata( Operands.ABC, Mode.R, Mode.U, Mode.Unused ) },
		{ Opcode.SetTable,	new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.NewTable,	new OpMetadata( Operands.ABC, Mode.R, Mode.Integer, Mode.Integer ) },
		{ Opcode.Self,		new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.RK ) },
		{ Opcode.Add,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Sub,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Mul,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Div,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Mod,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Pow,		new OpMetadata( Operands.ABC, Mode.R, Mode.RK, Mode.RK ) },
		{ Opcode.Unm,		new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Unused ) },
		{ Opcode.Not,		new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Unused ) },
		{ Opcode.Len,		new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Unused ) },
		{ Opcode.Concat,	new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.R ) },
		{ Opcode.Jmp,		new OpMetadata( Operands.Jump, Mode.Unused, Mode.Jump, Mode.Unused ) },
		{ Opcode.Eq,		new OpMetadata( Operands.ABC, Mode.Bool, Mode.RK, Mode.RK ) },
		{ Opcode.Lt,		new OpMetadata( Operands.ABC, Mode.Bool, Mode.RK, Mode.RK ) },
		{ Opcode.Le,		new OpMetadata( Operands.ABC, Mode.Bool, Mode.RK, Mode.RK ) },
		{ Opcode.Test,		new OpMetadata( Operands.ABC, Mode.R, Mode.Unused, Mode.Bool ) },
		{ Opcode.TestSet,	new OpMetadata( Operands.ABC, Mode.R, Mode.R, Mode.Bool ) },
		{ Opcode.Call,		new OpMetadata( Operands.ABC, Mode.R, Mode.Integer, Mode.Integer ) },
		{ Opcode.TailCall,	new OpMetadata( Operands.ABC, Mode.R, Mode.Integer, Mode.Unused ) },
		{ Opcode.Return,	new OpMetadata( Operands.ABC, Mode.R, Mode.Integer, Mode.Unused ) },
		{ Opcode.ForLoop,	new OpMetadata( Operands.Jump, Mode.R, Mode.Jump, Mode.Unused ) },
		{ Opcode.ForPrep,	new OpMetadata( Operands.Jump, Mode.R, Mode.Jump, Mode.Unused ) },
		{ Opcode.TForLoop,	new OpMetadata( Operands.ABC, Mode.R, Mode.Unused, Mode.Integer ) },
		{ Opcode.SetList,	new OpMetadata( Operands.SetListABC, Mode.R, Mode.Integer, Mode.Integer ) },
		{ Opcode.Close,		new OpMetadata( Operands.ABC, Mode.R, Mode.Unused, Mode.Unused ) },
		{ Opcode.Closure,	new OpMetadata( Operands.ABx, Mode.R, Mode.P, Mode.Unused ) },
		{ Opcode.Vararg,	new OpMetadata( Operands.ABC, Mode.R, Mode.Integer, Mode.Unused ) },
	};



	void WriteInstruction()
	{
		Instruction		instruction		= prototype.Instructions[ ip ];
		DebugSourceSpan instructionSpan	= prototype.DebugInstructionSourceSpans[ ip ];
		OpMetadata		metadata		= opcodeMetadata[ instruction.Opcode ];
		

		// Disassemble opcode.
		
		string assembler = String.Format( "{0,-10}  ",
			instruction.Opcode.ToString().ToLower() );

		switch ( metadata.Operands )
		{
		case Operands.ABC:
			assembler += String.Format( "{0}{1}{2}",
				OperandString( metadata.A, instruction.A ),
				OperandString( metadata.B, instruction.B ),
				OperandString( metadata.C, instruction.C ) );
			break;

		case Operands.ABx:
			assembler += String.Format( "{0}{1}",
				OperandString( metadata.A, instruction.A ),
				OperandString( metadata.B, instruction.Bx ) );
			break;

		case Operands.Jump:
			assembler += String.Format( "{0}{1}",
				OperandString( metadata.A, instruction.A ),
				OperandString( metadata.B, ip + 1 + instruction.sBx ) );
			break;

		case Operands.SetListABC:
			if ( instruction.C != 0 )
			{
				// C is encoded in instruction.
				assembler += String.Format( "{0}{1}{2}",
					OperandString( metadata.A, instruction.A ),
					OperandString( metadata.B, instruction.B ),
					OperandString( metadata.C, instruction.C ) );
			}
			else
			{
				// Recover C from next instruction.
				ip += 1;
				int C = prototype.Instructions[ ip ].Index;

				assembler += String.Format( "{0}{1}{2}",
					OperandString( metadata.A, instruction.A ),
					OperandString( metadata.B, instruction.B ),
					OperandString( metadata.C, C ) );
			}
			break;
		}
		assembler = assembler.TrimEnd();


		// Location.

		string span = String.Format( "({0},{1}) - ({2},{3})",
			instructionSpan.Start.Line, instructionSpan.Start.Column,
			instructionSpan.End.Line, instructionSpan.End.Column );


		// Get variable scopes.

		string newlocals = "";
		string oldlocals = "";
		
		for ( int local = 0; local < prototype.DebugLocals.Length; ++local )
		{
			DebugLocal debugLocal = prototype.DebugLocals[ local ];
			
			if ( debugLocal.StartInstruction == ip )
			{
				if ( newlocals.Length > 0 )
					newlocals += ", ";
				newlocals += debugLocal.Name;
			}

			if ( debugLocal.EndInstruction == ip + 1 )
			{
				if ( oldlocals.Length > 0 )
					oldlocals += ", ";
				oldlocals += debugLocal.Name;
			}
		}

		if ( newlocals.Length > 0 )
		{
			newlocals = "  --> " + newlocals;
		}

		if ( oldlocals.Length > 0 )
		{
			oldlocals = "  <-- " + oldlocals;
		}


		// Print.

		string output = String.Format( "  0x{0:X4}  {1,-40}  {2,-20}{3}{4}",
			ip, assembler, span, oldlocals, newlocals );
		o.WriteLine( output.TrimEnd() );
	}



	string OperandString( Mode mode, int operand )
	{
		switch ( mode )
		{
		case Mode.R:
			return String.Format( "{0} ", RegisterString( operand ) );

		case Mode.K:
			return String.Format( "{0} ", ConstantString( operand ) );

		case Mode.RK:
			if ( Instruction.IsConstant( operand ) )
			{
				return String.Format( "{0} ",
					ConstantString( Instruction.RKToConstant( operand ) ) );
			}
			else
			{
				return String.Format( "{0} ", RegisterString( operand ) );
			}

		case Mode.U:
			return String.Format( "upval {0}", prototype.DebugUpValNames[ operand ] );

		case Mode.P:
			return String.Format( "function x{0:X}", prototype.Prototypes[ operand ].GetHashCode() );

		case Mode.Integer:
			return String.Format( "{0} ", operand );

		case Mode.Bool:
			if ( operand != 0 )
			{
				return "true ";
			}
			else
			{
				return "false ";
			}

		case Mode.Jump:
			return String.Format( "0x{0:X4} ", operand );
		}

		return "";
	}


	string RegisterString( int operand )
	{
		int index = operand;
		for ( int debug = 0; debug < prototype.DebugLocals.Length; ++debug )
		{
			DebugLocal debugLocal = prototype.DebugLocals[ debug ];
			if ( ( ip >= debugLocal.StartInstruction ) && ( ip <= debugLocal.EndInstruction ) )
			{
				if ( index == 0 )
				{
					return String.Format( "{1} ", operand, debugLocal.Name );
				}
				else
				{
					index -= 1;
				}
			}
		}

		return String.Format( "r{0} ", operand );
	}



	string ConstantString( int index )
	{
		object constant = prototype.Constants[ index ];
		if ( constant is BoxedString )
		{
			string s = ( (BoxedString)constant ).Value;
			s = s.Replace( "\n", "\\n" );
			return String.Format( "\"{0}\"", s );
		}
		else
		{
			return String.Format( "#{0}", constant );
		}
	}


}


}