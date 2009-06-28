// VMFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using Lua;


namespace Lua.VM
{


public sealed class VMFunction
	:	Function
{
	public UpVal[]		UpVals			{ get; private set; }
	public Prototype	Prototype		{ get; private set; }


	public VMFunction( Prototype prototype )
	{
		UpVals		= new UpVal[ prototype.UpValCount ];
		Prototype	= prototype;
	}



	// Invoke.

	public override Value InvokeS()
	{
		return null;
	}

	public override Value InvokeS( Value a1 )
	{
		return null;
	}

	public override Value InvokeS( Value a1, Value a2 )
	{
		return null;
	}

	public override Value InvokeS( Value a1, Value a2, Value a3 )
	{
		return null;
	}

	public override Value InvokeS( Value a1, Value a2, Value a3, Value a4 )
	{
		return null;
	}

	public override Value InvokeS( Value[] arguments )
	{
		return null;
	}

	public override Value[] InvokeM()
	{
		return null;
	}

	public override Value[] InvokeM( Value a1 )
	{
		return null;
	}

	public override Value[] InvokeM( Value a1, Value a2 )
	{
		return null;
	}

	public override Value[] InvokeM( Value a1, Value a2, Value a3 )
	{
		return null;
	}

	public override Value[] InvokeM( Value a1, Value a2, Value a3, Value a4 )
	{
		return null;
	}

	public override Value[] InvokeM( Value[] arguments )
	{
		return null;
	}


}
	

}


