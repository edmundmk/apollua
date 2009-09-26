// LuaBytecode.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua;


namespace Lua.Bytecode
{
	

public class LuaBytecode
{

	// Parameters.

	public int				UpValCount					{ get; set; }
	public int				ParameterCount				{ get; set; }
	public bool				IsVararg					{ get; set; }


	// Constants.

	public LuaValue[]		Constants					{ get; set; }
	public LuaBytecode[]	Prototypes					{ get; set; }


	// VM opcodes.

	public int				StackSize					{ get; set; }
	public Instruction[]	Instructions				{ get; set; }


	// Debug information.

	public string			DebugName					{ get; set; }
	public SourceSpan		DebugSourceSpan				{ get; set; }
	public SourceSpan[]		DebugInstructionSourceSpans	{ get; set; }
	public string[]			DebugUpValNames				{ get; set; }
	public Symbol[]			DebugLocals					{ get; set; }

}


}

