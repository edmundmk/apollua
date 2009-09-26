// VMFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua;
using Lua.Bytecode;


namespace Lua.VM
{


public sealed class VMFunction
	:	LuaFunction
{
	public UpVal[]		UpVals			{ get; private set; }
	public LuaBytecode	Prototype		{ get; private set; }


	public VMFunction( LuaBytecode prototype )
	{
		UpVals		= new UpVal[ prototype.UpValCount ];
		Prototype	= prototype;
	}



	// Invoke.

	public override LuaValue InvokeS()
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 0 );
		return vm.InvokeS( this );
	}

	public override LuaValue InvokeS( LuaValue a1 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 1 );
		vm.Argument( a1 );
		return vm.InvokeS( this );
	}

	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 2 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		return vm.InvokeS( this );
	}

	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 3 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		vm.Argument( a3 );
		return vm.InvokeS( this );
	}

	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 4 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		vm.Argument( a3 );
		vm.Argument( a4 );
		return vm.InvokeS( this );
	}

	public override LuaValue InvokeS( LuaValue[] arguments )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( arguments.Length );
		for ( int argument = 0; argument < arguments.Length; ++argument )
		{
			vm.Argument( arguments[ argument ] );
		}
		return vm.InvokeS( this );
	}

	public override LuaValue[] InvokeM()
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 0 );
		return vm.InvokeM( this );
	}

	public override LuaValue[] InvokeM( LuaValue a1 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 1 );
		vm.Argument( a1 );
		return vm.InvokeM( this );
	}

	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 2 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		return vm.InvokeM( this );
	}

	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 3 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		vm.Argument( a3 );
		return vm.InvokeM( this );
	}

	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( 4 );
		vm.Argument( a1 );
		vm.Argument( a2 );
		vm.Argument( a3 );
		vm.Argument( a4 );
		return vm.InvokeM( this );
	}

	public override LuaValue[] InvokeM( LuaValue[] arguments )
	{
		VirtualMachine vm = VMRuntime.VirtualMachine;
		vm.BeginInvoke( arguments.Length );
		for ( int argument = 0; argument < arguments.Length; ++argument )
		{
			vm.Argument( arguments[ argument ] );
		}
		return vm.InvokeM( this );
	}


}
	

}


