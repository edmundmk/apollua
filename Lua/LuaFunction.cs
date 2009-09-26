// LuaFunction.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


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

	
	// LuaValue

	public override	LuaTable Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}

	public override string GetLuaType()
	{
		return "function";
	}


	// Binary operations.

	public override sealed LuaValue Add( LuaValue o )				{ return base.Add( o ); }
	public override sealed LuaValue Subtract( LuaValue o )			{ return base.Subtract( o ); }
	public override sealed LuaValue Multiply( LuaValue o )			{ return base.Multiply( o ); }
	public override sealed LuaValue Divide( LuaValue o )			{ return base.Divide( o ); }
	public override sealed LuaValue IntegerDivide( LuaValue o )		{ return base.IntegerDivide( o ); }
	public override sealed LuaValue Modulus( LuaValue o )			{ return base.Modulus( o ); }
	public override sealed LuaValue RaiseToPower( LuaValue o )		{ return base.RaiseToPower( o ); }
	public override sealed LuaValue Concatenate( LuaValue o )		{ return base.Concatenate( o ); }


	// Unary operations.

	public override sealed LuaValue UnaryMinus()					{ return base.UnaryMinus(); }
	public override sealed LuaValue Length()						{ return base.Length(); }


	// Comparisons.

	public override sealed bool EqualsValue( LuaValue o )			{ return base.EqualsValue( o ); }
	public override sealed bool LessThanValue( LuaValue o )			{ return base.LessThanValue( o ); }
	public override sealed bool LessThanOrEqualsValue( LuaValue o )	{ return base.LessThanOrEqualsValue( o ); }


	// Indexing.

	public override sealed LuaValue Index( LuaValue k )				{ return base.Index( k ); }
	public override sealed void NewIndex( LuaValue k, LuaValue v )	{ base.NewIndex( k, v ); }


	// Individual functions must implement all call operations.

	public override abstract LuaValue InvokeS();
	public override abstract LuaValue InvokeS( LuaValue a1 );
	public override abstract LuaValue InvokeS( LuaValue a1, LuaValue a2 );
	public override abstract LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 );
	public override abstract LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 );
	public override abstract LuaValue InvokeS( LuaValue[] arguments );

	public override abstract LuaValue[] InvokeM();
	public override abstract LuaValue[] InvokeM( LuaValue a1 );
	public override abstract LuaValue[] InvokeM( LuaValue a1, LuaValue a2 );
	public override abstract LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 );
	public override abstract LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 );
	public override abstract LuaValue[] InvokeM( LuaValue[] arguments );


}


}

