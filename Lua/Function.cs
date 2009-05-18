// Function.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua
{


public abstract class Function
	:	Value
{
	// Metatable.

	public static Table TypeMetatable
	{
		get;
		set;
	}
	
	public override	Table Metatable
	{
		get { return TypeMetatable; }
		set { base.Metatable = value; }
	}



	// Environment.

	public Value Environment
	{
		get;
		set;
	}



	// Comparison operators.

	public override sealed bool Equals( Value o )
	{
		if ( o == this )
		{
			return true;
		}
		else
		{
			return base.Equals( o );
		}
	}



	// Individual functions cannot change these operations.
 
	public override sealed Value Add( Value o )						{ return base.Add( o ); }
	public override sealed Value Subtract( Value o )				{ return base.Subtract( o ); }
	public override sealed Value Multiply( Value o )				{ return base.Multiply( o ); }
	public override sealed Value Divide( Value o )					{ return base.Divide( o ); }
	public override sealed Value IntegerDivide( Value o )			{ return base.IntegerDivide( o ); }
	public override sealed Value Modulus( Value o )					{ return base.Modulus( o ); }
	public override sealed Value RaiseToPower( Value o )			{ return base.RaiseToPower( o ); }
	public override sealed Value Concatenate( Value o )				{ return base.Concatenate( o ); }
	public override sealed Value UnaryMinus()						{ return base.UnaryMinus(); }
	public override sealed Value Length()							{ return base.Length(); }
	public override sealed bool LessThan( Value o )					{ return base.LessThan( o ); }
	public override sealed bool LessThanOrEqual( Value o )			{ return base.LessThanOrEqual( o ); }
	public override sealed Value Index( Value key )					{ return base.Index( key ); }
	public override sealed void NewIndex( Value key, Value value )	{ base.NewIndex( key, value ); }



	// Individual functions must implement all call operations.
/*
	public override abstract Value InvokeS();
	public override abstract Value InvokeS( Value arg );
	public override abstract Value InvokeS( Value arg1, Value arg2 );
	public override abstract Value InvokeS( Value arg1, Value arg2, Value arg3 );
	public override abstract Value InvokeS( Value arg1, Value arg2, Value arg3, Value arg4 );
	public override abstract Value InvokeS( Value[] arguments );

	public override abstract Value[] InvokeM();
	public override abstract Value[] InvokeM( Value arg );
	public override abstract Value[] InvokeM( Value arg1, Value arg2 );
	public override abstract Value[] InvokeM( Value arg1, Value arg2, Value arg3 );
	public override abstract Value[] InvokeM( Value arg1, Value arg2, Value arg3, Value arg4 );
	public override abstract Value[] InvokeM( Value[] arguments );

//	public override abstract Value ResumeS( StackFrame frame );
//	public override abstract Value[] ResumeM( StackFrame frame );
*/
}


}

