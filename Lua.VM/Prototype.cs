// Prototype.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua;


namespace Lua.VM
{
	

public class Prototype
{

	// Parameters.

	public int					UpValCount				{ get; set; }
	public int					ParameterCount			{ get; set; }
	public bool					IsVararg				{ get; set; }


	// Constants.

	public Value[]				Constants				{ get; set; }
	public Prototype[]			Prototypes				{ get; set; }


	// VM opcodes.

	public int					StackSize				{ get; set; }
	public Instruction[]		Instructions			{ get; set; }


	// Debug information.

	public string				DebugName				{ get; set; }
	public DebugSourceSpan		DebugSourceSpan			{ get; set; }
	public DebugSourceSpan[]	DebugOpcodeSourceSpans	{ get; set; }
	public string				DebugUpValNames			{ get; set; }
	public DebugLocalScope[]	DebugLocalScopes		{ get; set; }

}


}

