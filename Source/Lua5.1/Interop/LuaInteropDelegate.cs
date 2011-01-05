// InteropDelegate.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2010 Edmund Kapusniak


using System;


namespace Lua.Interop
{


public sealed class LuaInteropDelegate
	:	LuaValue
{
	LuaInteropSignature interopFunction;

	public LuaInteropDelegate( LuaInteropSignature f )
	{
		interopFunction = f;
	}
	
	protected internal override string LuaType
	{
		get { return "function"; }
	}
	
	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		interopFunction( new LuaInterop( thread, frameBase, argumentCount, resultCount ) );
	}
}


public sealed class LuaInteropDelegateAction
	:	LuaValue
{
	Action action;

	public LuaInteropDelegateAction( Action f )
	{
		action = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		action();
		lua.Return();		
	}
}

public sealed class LuaInteropDelegateAction< T >
	:	LuaValue
{
	Action< T > action;

	public LuaInteropDelegateAction( Action< T > f )
	{
		action = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		action( lua.Argument< T >( 0 ) );
		lua.Return();
	}
}

public sealed class LuaInteropDelegateAction< T1, T2 >
	:	LuaValue
{
	Action< T1, T2 > action;

	public LuaInteropDelegateAction( Action< T1, T2 > f )
	{
		action = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		action( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ) );
		lua.Return();
	}
}

public sealed class LuaInteropDelegateAction< T1, T2, T3 >
	:	LuaValue
{
	Action< T1, T2, T3 > action;

	public LuaInteropDelegateAction( Action< T1, T2, T3 > f )
	{
		action = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		action( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ), lua.Argument< T3 >( 2 ) );
		lua.Return();
	}
}

public sealed class LuaInteropDelegateAction< T1, T2, T3, T4 >
	:	LuaValue
{
	Action< T1, T2, T3, T4 > action;

	public LuaInteropDelegateAction( Action< T1, T2, T3, T4 > f )
	{
		action = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		action( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ), lua.Argument< T3 >( 2 ), lua.Argument< T4 >( 3 ) );
		lua.Return();
	}
}


public sealed class LuaInteropDelegateFunc< TResult >
	:	LuaValue
{
	Func< TResult > func;

	public LuaInteropDelegateFunc( Func< TResult > f )
	{
		func = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		lua.Return( func() );
	}
}

public sealed class LuaInteropDelegateFunc< T, TResult >
	:	LuaValue
{
	Func< T, TResult > func;

	public LuaInteropDelegateFunc( Func< T, TResult > f )
	{
		func = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		lua.Return( func( lua.Argument< T >( 0 ) ) );
	}
}

public sealed class LuaInteropDelegateFunc< T1, T2, TResult >
	:	LuaValue
{
	Func< T1, T2, TResult > func;

	public LuaInteropDelegateFunc( Func< T1, T2, TResult > f )
	{
		func = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		lua.Return( func( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ) ) );
	}
}

public sealed class LuaInteropDelegateFunc< T1, T2, T3, TResult >
	:	LuaValue
{
	Func< T1, T2, T3, TResult > func;

	public LuaInteropDelegateFunc( Func< T1, T2, T3, TResult > f )
	{
		func = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		lua.Return( func( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ), lua.Argument< T3 >( 2 ) ) );
	}
}

public sealed class LuaInteropDelegateFunc< T1, T2, T3, T4, TResult >
	:	LuaValue
{
	Func< T1, T2, T3, T4, TResult > func;

	public LuaInteropDelegateFunc( Func< T1, T2, T3, T4, TResult > f )
	{
		func = f;
	}

	protected internal override string LuaType
	{
		get { return "function"; }
	}

	internal override void Call( LuaThread thread, int frameBase, int argumentCount, int resultCount )
	{
		LuaInterop lua = new LuaInterop( thread, frameBase, argumentCount, resultCount );
		lua.Return( func( lua.Argument< T1 >( 0 ), lua.Argument< T2 >( 1 ), lua.Argument< T3 >( 2 ), lua.Argument< T4 >( 3 ) ) );
	}
}



}