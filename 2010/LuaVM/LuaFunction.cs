// LuaFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using Lua.Runtime;


namespace Lua
{

/*
public abstract class LuaFunction
	:	LuaValue
{

	// Type metatable and function environment.

	public static LuaTable TypeMetatable
	{
		get;
		set;
	}
	
	public LuaValue Environment
	{
		get;
		set;
	}

	
	// Metatable.

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}


	// Type.
	
	public override sealed string	GetLuaType()						{ return "function"; }
	public override sealed bool		IsPrimitiveValue()					{ return base.IsPrimitiveValue(); }
	public override sealed bool		TryToInteger( out int v )			{ return base.TryToInteger( out v ); }
	public override sealed bool		TryToDouble( out double v )			{ return base.TryToDouble( out v ); }
	public override sealed bool		TryToString( out string v )			{ return base.TryToString( out v ); }
	public override sealed bool		TryToNumberValue( out LuaValue v )	{ return base.TryToNumberValue( out v ); }


	// Operations

	public override sealed LuaValue Add( LuaValue o )					{ return base.Add( o ); }
	public override sealed LuaValue Subtract( LuaValue o )				{ return base.Subtract( o ); }
	public override sealed LuaValue Multiply( LuaValue o )				{ return base.Multiply( o ); }
	public override sealed LuaValue Divide( LuaValue o )				{ return base.Divide( o ); }
	public override sealed LuaValue IntegerDivide( LuaValue o )			{ return base.IntegerDivide( o ); }
	public override sealed LuaValue Modulus( LuaValue o )				{ return base.Modulus( o ); }
	public override sealed LuaValue RaiseToPower( LuaValue o )			{ return base.RaiseToPower( o ); }
	public override sealed LuaValue Concatenate( LuaValue o )			{ return base.Concatenate( o ); }

	public override sealed LuaValue UnaryMinus()						{ return base.UnaryMinus(); }
	public override sealed LuaValue Length()							{ return base.Length(); }

	public sealed override bool IsTrue()								{ return base.IsTrue(); }
	public override sealed bool EqualsValue( LuaValue o )				{ return base.EqualsValue( o ); }
	public override sealed bool LessThanValue( LuaValue o )				{ return base.LessThanValue( o ); }
	public override sealed bool LessThanOrEqualsValue( LuaValue o )		{ return base.LessThanOrEqualsValue( o ); }

	public override sealed LuaValue Index( LuaValue k )					{ return base.Index( k ); }
	public override sealed void NewIndex( LuaValue k, LuaValue v )		{ base.NewIndex( k, v ); }

/*	
	// Bytecode function interface.

	public override abstract FrozenFrame Call( LuaThread t, int f, int a, int r );
	public override abstract FrozenFrame Resume( LuaThread t, FrozenFrame f );


	// Interop function interface.

	protected override abstract Delegate MakeDelegate( Type delegateType );
*//*

}
*/

}

