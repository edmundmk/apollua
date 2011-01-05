// math.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Interop;


namespace Lua.Library
{


public static partial class math
{

	public static LuaTable CreateTable()
	{
		LuaTable math = new LuaTable();
		math[ "abs" ] = new LuaInteropDelegateFunc< double, double >( Math.Abs );
		return math;
	}


	




}


}