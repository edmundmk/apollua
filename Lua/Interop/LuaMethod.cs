// InteropMethod.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Reflection;


namespace Lua.Interop
{


public class LuaMethodV< TObject >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodV( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodV< TObject, T >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodV( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ); return InteropHelpers.EmptyValues; }

}

	
public class LuaMethodV< TObject, T1, T2 >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodV( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ); return InteropHelpers.EmptyValues; }

}

	
public class LuaMethodV< TObject, T1, T2, T3 >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodV( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodV< TObject, T1, T2, T3, T4 >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodV( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodVP< TObject, TParams >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodVP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodVP< TObject, T, TParams >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodVP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ); return InteropHelpers.EmptyValues; }

}

	
public class LuaMethodVP< TObject, T1, T2, TParams >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodVP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ); return InteropHelpers.EmptyValues; }

}

	
public class LuaMethodVP< TObject, T1, T2, T3, TParams >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodVP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodVP< TObject, T1, T2, T3, T4, TParams >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodVP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ); return null; }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ); return null; }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ); return InteropHelpers.EmptyValues; }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ); return InteropHelpers.EmptyValues; }

}


public class LuaMethodS< TObject, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodS( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ) ); }

}


public class LuaMethodS< TObject, T, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodS( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ) ); }

}

	
public class LuaMethodS< TObject, T1, T2, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodS( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ) ); }

}

	
public class LuaMethodS< TObject, T1, T2, T3, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodS( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ) ); }

}


public class LuaMethodS< TObject, T1, T2, T3, T4, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodS( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ) ); }

}


public class LuaMethodSP< TObject, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodSP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ) ); }

}


public class LuaMethodSP< TObject, T, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodSP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ) ); }

}

	
public class LuaMethodSP< TObject, T1, T2, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodSP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ) ); }

}

	
public class LuaMethodSP< TObject, T1, T2, T3, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodSP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ) ); }

}


public class LuaMethodSP< TObject, T1, T2, T3, T4, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodSP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxS( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxM( (TResult)method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ) ); }

}


public class LuaMethodM< TObject, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodM( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), InteropHelpers.EmptyObjects ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), InteropHelpers.EmptyObjects ) ); }

}


public class LuaMethodM< TObject, T, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodM( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ) } ) ); }

}

	
public class LuaMethodM< TObject, T1, T2, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodM( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ) } ) ); }

}

	
public class LuaMethodM< TObject, T1, T2, T3, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodM( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ) } ) ); }

}


public class LuaMethodM< TObject, T1, T2, T3, T4, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodM( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ) } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ) } ) ); }

}


public class LuaMethodMP< TObject, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodMP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { new TParams[] { InteropHelpers.Unbox< TParams >( a2 ), InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxList< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxList< TParams >( arguments, 1 ) } ) ); }

}


public class LuaMethodMP< TObject, T, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodMP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T >( a2 ), new TParams[] { InteropHelpers.Unbox< TParams >( a3 ), InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T >( arguments, 1 ), InteropHelpers.UnboxList< TParams >( arguments, 2 ) } ) ); }

}

	
public class LuaMethodMP< TObject, T1, T2, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodMP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), new TParams[] { InteropHelpers.Unbox< TParams >( a4 ) } } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxList< TParams >( arguments, 3 ) } ) ); }

}

	
public class LuaMethodMP< TObject, T1, T2, T3, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodMP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxList< TParams >( arguments, 4 ) } ) ); }

}


public class LuaMethodMP< TObject, T1, T2, T3, T4, TParams, TResult >
	:	LuaFunction
{
	MethodBase method;

	public LuaMethodMP( MethodBase method )
	{
		this.method = method;
	}

	public override LuaValue InvokeS()												{ throw new NullReferenceException(); }
	public override LuaValue InvokeS( LuaValue a1 )									{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2 )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )		{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue InvokeS( LuaValue[] arguments )							{ return InteropHelpers.BoxListS( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ) ); }

	public override LuaValue[] InvokeM()											{ throw new NullReferenceException(); }
	public override LuaValue[] InvokeM( LuaValue a1 )									{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { default( T1 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2 )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), default( T2 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3 )				{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), default( T3 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue a1, LuaValue a2, LuaValue a3, LuaValue a4 )	{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.Unbox< TObject >( a1 ), new object[] { InteropHelpers.Unbox< T1 >( a2 ), InteropHelpers.Unbox< T2 >( a3 ), InteropHelpers.Unbox< T3 >( a4 ), default( T4 ), new TParams[] {} } ) ); }
	public override LuaValue[] InvokeM( LuaValue[] arguments )						{ return InteropHelpers.BoxListM( (TResult[])method.Invoke( InteropHelpers.UnboxElement< TObject >( arguments, 0 ), new object[] { InteropHelpers.UnboxElement< T1 >( arguments, 1 ), InteropHelpers.UnboxElement< T2 >( arguments, 2 ), InteropHelpers.UnboxElement< T3 >( arguments, 3 ), InteropHelpers.UnboxElement< T4 >( arguments, 4 ), InteropHelpers.UnboxList< TParams >( arguments, 5 ) } ) ); }

}


}

