// Signatures.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public enum InteropSignature
{
	V,		// Action	void F( object )
	VP,		// ActionP	void F( object, params object[] )
	S,		// Func		object F( object )
	SP,		// FuncP	object F( object, params object[] )
	M,		// FuncM	object[] F( object )
	MP,		// FuncMP	object[] F( object, params object[] )
}


public delegate void ActionP< TParams >( params TParams[] arguments );
public delegate void ActionP< T, TParams >( T a1, params TParams[] arguments );
public delegate void ActionP< T1, T2, TParams >( T1 a1, T2 a2, params TParams[] arguments );
public delegate void ActionP< T1, T2, T3, TParams >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate void ActionP< T1, T2, T3, T4, TParams >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );

public delegate TResult FuncP< TParams, TResult >( params TParams[] arguments );
public delegate TResult FuncP< T, TParams, TResult >( T a1, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, TParams, TResult >( T1 a1, T2 a2, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate TResult FuncP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );

public delegate TResult[] FuncM< TResult >();
public delegate TResult[] FuncM< T, TResult >( T a1 );
public delegate TResult[] FuncM< T1, T2, TResult >( T1 a1, T2 a2 );
public delegate TResult[] FuncM< T1, T2, T3, TResult >( T1 a1, T2 a2, T3 a3 );
public delegate TResult[] FuncM< T1, T2, T3, T4, TResult >( T1 a1, T2 a2, T3 a3, T4 a4 );

public delegate TResult[] FuncMP< TParams, TResult >( params TParams[] arguments );
public delegate TResult[] FuncMP< T, TParams, TResult >( T a1, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, TParams, TResult >( T1 a1, T2 a2, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, T3, TParams, TResult >( T1 a1, T2 a2, T3 a3, params TParams[] arguments );
public delegate TResult[] FuncMP< T1, T2, T3, T4, TParams, TResult >( T1 a1, T2 a2, T3 a3, T4 a4, params TParams[] arguments );


}