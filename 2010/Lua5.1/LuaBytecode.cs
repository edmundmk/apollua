// LuaBytecode.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lua.Bytecode;


namespace Lua
{
	

public class LuaBytecode
{

	// Bytecode.

	internal int			UpValCount;
	internal int			ParameterCount;
	internal bool			IsVararg;

	internal LuaValue[]		Constants;
	internal LuaBytecode[]	Prototypes;

	internal int			StackSize;
	internal Instruction[]	Instructions;


	// Debug info.

	internal string			DebugName;
	internal SourceSpan		DebugSourceSpan;
	internal SourceSpan[]	DebugInstructionSourceSpans;
	internal string[]		DebugUpValNames;
	internal Symbol[]		DebugLocals;




	// Serialization.

	const sbyte LUA_TNIL		= 0;
	const sbyte LUA_TBOOLEAN	= 1;
	const sbyte LUA_TNUMBER		= 3;
	const sbyte LUA_TSTRING		= 4;


	public void Load( BinaryReader r )
	{
		// Header.
		CheckSByte( r, 0x1B );	// <esc>
		CheckSByte( r, 0x4C );	// 'L'
		CheckSByte( r, 0x75 );	// 'u'
		CheckSByte( r, 0x61 );	// 'a'
		CheckSByte( r, 0x51 );	// Version 5.1
		CheckSByte( r, 0x00 );	// Format 0
		CheckSByte( r, 0x01 );	// Little-endian
		CheckSByte( r, 0x04 );	// sizeof( int )
		CheckSByte( r, 0x04 );	// sizeof( size_t )
		CheckSByte( r, 0x04 );	// sizeof( Instruction )
		CheckSByte( r, 0x08 );	// sizeof( lua_Number )
		CheckSByte( r, 0x00 );	// Floating-point


		// Function.
		LoadFunction( r );
	}


	void LoadFunction( BinaryReader r )
	{
		// Function information.
		DebugName		= ReadString( r );
		DebugSourceSpan	= new SourceSpan( DebugName, r.ReadInt32(), 0, r.ReadInt32(), 0 );
		UpValCount		= r.ReadByte();
		ParameterCount	= r.ReadByte();
		IsVararg		= r.ReadByte() != 0;
		StackSize		= r.ReadByte();
		

		// Instructions.
		int instructionsLength = r.ReadInt32();
		Instructions = new Instruction[ instructionsLength ];
		for ( int instruction = 0; instruction < instructionsLength; ++instruction )
		{
			Instructions[ instruction ] = Instruction.FromUInt32( r.ReadUInt32() );
		}


		// Constants.
		int constantsLength = r.ReadInt32();
		Constants = new LuaValue[ constantsLength ];
		for ( int constant = 0; constant < constantsLength; ++constant )
		{
			switch ( r.ReadSByte() )
			{
				case LUA_TNIL:
					Constants[ constant ] = null;
					break;

				case LUA_TBOOLEAN:
					Constants[ constant ] = r.ReadSByte() != 0;
					break;

				case LUA_TNUMBER:
					double d = r.ReadDouble();
					int i = (int)d;
					if ( (double)i == d )
					{
						Constants[ constant ] = i;
					}
					else
					{
						Constants[ constant ] = d;
					}
					break;

				case LUA_TSTRING:
					Constants[ constant ] = ReadString( r );
					break;

				default:
					throw new FileLoadException();
			}
		}



		// Prototypes.
		int prototypesLength = r.ReadInt32();
		Prototypes = new LuaBytecode[ prototypesLength ];
		for ( int prototype = 0; prototype < prototypesLength; ++prototype )
		{
			Prototypes[ prototype ] = new LuaBytecode();
			Prototypes[ prototype ].LoadFunction( r );
		}


		// Debug information.
		int debugInstructionSourceSpansLength = r.ReadInt32();
		DebugInstructionSourceSpans = new SourceSpan[ debugInstructionSourceSpansLength ];
		for ( int debugInstructionSourceSpan = 0; debugInstructionSourceSpan < debugInstructionSourceSpansLength; ++debugInstructionSourceSpan )
		{
			int line = r.ReadInt32();
			DebugInstructionSourceSpans[ debugInstructionSourceSpan ] = new SourceSpan( DebugName, line, 0, line, 0 );
		}

		int debugLocalsLength = r.ReadInt32();
		DebugLocals = new Symbol[ debugLocalsLength ];
		for ( int debugLocal = 0; debugLocal < debugLocalsLength; ++debugLocal )
		{
			DebugLocals[ debugLocal ] = new Symbol( ReadString( r ), r.ReadInt32(), r.ReadInt32() );
		}

		int debugUpValNamesLength = r.ReadInt32();
		DebugUpValNames = new string[ debugUpValNamesLength ];
		for ( int debugUpValName = 0; debugUpValName < debugUpValNamesLength; ++debugUpValName )
		{
			DebugUpValNames[ debugUpValName ] = ReadString( r );
		}
	}


	void CheckSByte( BinaryReader r, sbyte b )
	{
		if ( r.ReadSByte() != b )
		{
			throw new FileLoadException();
		}
	}

	string ReadString( BinaryReader r )
	{
		int length = r.ReadInt32();
		if ( length > 0 )
		{
			byte[] characters = r.ReadBytes( length );
			return Encoding.UTF8.GetString( characters, 0, length - 1 );
		}
		else
		{
			return String.Empty;
		}
	}


	





	// Disassembly.

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


	public void Disassemble( TextWriter o )
	{
		// Function signature.

		o.Write( "function " );
		if ( DebugName != null )
		{
			o.Write( DebugName );
		}
		else
		{
			o.Write( "x" );
			o.Write( GetHashCode().ToString( "X" ) );
		}
		o.Write( "(" );
		if ( ParameterCount > 0 || IsVararg )
		{
			o.Write( " " );
			bool bFirst = true;
			for ( int i = 0; i < ParameterCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( DebugLocals[ i ].Name );
			}
			if ( IsVararg )
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

		o.WriteLine( " -- {0} registers, {1} constants", StackSize, Constants.Length );

		
		// Upvals.

		if ( UpValCount > 0 )
		{
			o.Write( "  -- upval " );
			bool bFirst = true;
			for ( int i = 0; i < UpValCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( DebugUpValNames[ i ] );
			}
			o.WriteLine();
		}


		// Locals.

		if ( DebugLocals.Length > ParameterCount )
		{
			o.Write( "  -- local " );
			bool bFirst = true;
			for ( int i = ParameterCount; i < DebugLocals.Length; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( DebugLocals[ i ].Name );
			}
			o.WriteLine();
		}


		// Instructions.

		for ( int ip = 0; ip < Instructions.Length; ++ip )
		{
			ip = WriteInstruction( o, ip );
			if ( Instructions[ ip ].Opcode == Opcode.Closure )
			{
				LuaBytecode closure = Prototypes[ Instructions[ ip ].Bx ];
				for ( int upval = 0; upval < closure.UpValCount; ++upval )
				{
					Instruction i = Instructions[ ip + 1 + upval ];
					if ( i.Opcode == Opcode.Move )
					{
						o.WriteLine( "              local   {0} {1}", upval,
							OperandString( Mode.R, i.B, ip ) ); 
					}
					else if ( i.Opcode == Opcode.GetUpVal )
					{
						o.WriteLine( "              upval   {0} {1}", upval,
							OperandString( Mode.U, i.B, ip ) );
					}

				}
				ip += closure.UpValCount;
			}
		}
		

		// Final.

		o.WriteLine( "end" );
		o.WriteLine();


		// Other functions.

		for ( int i = 0; i < Prototypes.Length; ++i )
		{
			Prototypes[ i ].Disassemble( o );
		}
	}



	int WriteInstruction( TextWriter o, int ip )
	{
		Instruction	instruction		= Instructions[ ip ];
		SourceSpan	instructionSpan	= DebugInstructionSourceSpans[ ip ];
		OpMetadata	metadata		= opcodeMetadata[ instruction.Opcode ];
		

		// Disassemble opcode.
		
		string assembler = String.Format( "{0,-10}  ",
			instruction.Opcode.ToString().ToLower() );

		switch ( metadata.Operands )
		{
		case Operands.ABC:
			assembler += String.Format( "{0}{1}{2}",
				OperandString( metadata.A, instruction.A, ip ),
				OperandString( metadata.B, instruction.B, ip ),
				OperandString( metadata.C, instruction.C, ip ) );
			break;

		case Operands.ABx:
			assembler += String.Format( "{0}{1}",
				OperandString( metadata.A, instruction.A, ip ),
				OperandString( metadata.B, instruction.Bx, ip ) );
			break;

		case Operands.Jump:
			assembler += String.Format( "{0}{1}",
				OperandString( metadata.A, instruction.A, ip ),
				OperandString( metadata.B, ip + 1 + instruction.sBx, ip ) );
			break;

		case Operands.SetListABC:
			if ( instruction.C != 0 )
			{
				// C is encoded in instruction.
				assembler += String.Format( "{0}{1}{2}",
					OperandString( metadata.A, instruction.A, ip ),
					OperandString( metadata.B, instruction.B, ip ),
					OperandString( metadata.C, instruction.C, ip ) );
			}
			else
			{
				// Recover C from next instruction.
				ip += 1;
				int C = Instructions[ ip ].Index;

				assembler += String.Format( "{0}{1}{2}",
					OperandString( metadata.A, instruction.A, ip ),
					OperandString( metadata.B, instruction.B, ip ),
					OperandString( metadata.C, C, ip ) );
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
		
		for ( int local = 0; local < DebugLocals.Length; ++local )
		{
			Symbol debugLocal = DebugLocals[ local ];
			
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


		// Return updated ip.

		return ip;
	}



	string OperandString( Mode mode, int operand, int ip )
	{
		switch ( mode )
		{
		case Mode.R:
			return String.Format( "{0} ", RegisterString( operand, ip ) );

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
				return String.Format( "{0} ", RegisterString( operand, ip ) );
			}

		case Mode.U:
			return String.Format( "upval {0}", DebugUpValNames[ operand ] );

		case Mode.P:
			return String.Format( "function x{0:X}", Prototypes[ operand ].GetHashCode() );

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


	string RegisterString( int operand, int ip )
	{
		int index = operand;
		for ( int debug = 0; debug < DebugLocals.Length; ++debug )
		{
			Symbol debugLocal = DebugLocals[ debug ];
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
		LuaValue constant = Constants[ index ];
		string s;
		if ( constant == null )
		{
			return "nil";
		}
		else if ( constant.TryToString( out s ) )
		{
			StringBuilder escapedString = new StringBuilder();
			escapedString.Append( "\"" );
			int start = 0;
			for ( int c = 0; c < s.Length; ++c )
			{
				if ( s[ c ] == '"' || s[ c ] == '\\' || Char.IsControl( s[ c ] ) )
				{
					escapedString.Append( s, start, c - start );
					start = c + 1;
					switch ( s[ c ] )
					{
						case '"':	escapedString.Append( "\\\"" );	break;
						case '\0':	escapedString.Append( "\\0" );	break;
						case '\a':	escapedString.Append( "\\a" );	break;
						case '\b':	escapedString.Append( "\\b" );	break;
						case '\f':	escapedString.Append( "\\f" );	break;
						case '\n':	escapedString.Append( "\\n" );	break;
						case '\r':	escapedString.Append( "\\r" );	break;
						case '\t':	escapedString.Append( "\\t" );	break;
						case '\v':	escapedString.Append( "\\v" );	break;
						
						default:
							byte[] utf8 = Encoding.UTF8.GetBytes( s[ c ].ToString() );
							foreach ( byte b in utf8 )
							{
								escapedString.AppendFormat( "\\x{0:X2}", b );
							}
							break;
					}
				}
			}
			escapedString.Append( s, start, s.Length - start );
			escapedString.Append( "\"" );
			return escapedString.ToString();
		}
		else
		{
			return String.Format( "#{0}", constant );
		}
	}


}


}

