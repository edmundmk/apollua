// PrototypeWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;


namespace Lua.VM
{


public static class PrototypeWriter
{

	public static void Write( TextWriter o, Prototype prototype )
	{
		// Function signature.

		o.Write( "function" );
		if ( prototype.DebugName != null )
		{
			o.Write( prototype.DebugName );
		}
		else
		{
			o.Write( "x" );
			o.Write( prototype.GetHashCode().ToString( "X" ) );
		}
		o.Write( "(" );
		if ( prototype.ParameterCount > 0 || prototype.IsVararg )
		{
			o.Write( " " );
			bool bFirst = true;
			for ( int i = 0; i < prototype.ParameterCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugLocalScopes[ i ].Name );
			}
			if ( prototype.IsVararg )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( "..." );
			}
			o.Write( " " );
		}
		o.WriteLine( ")" );


		// Upvals.

		if ( prototype.UpValCount > 0 )
		{
			o.Write( "  -- upval " );
			bool bFirst = true;
			for ( int i = 0; i < prototype.UpValCount; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugUpValNames[ i ] );
			}
			o.WriteLine();
		}


		// Locals.

		if ( prototype.DebugLocalScopes.Length > prototype.ParameterCount )
		{
			o.Write( "  -- local " );
			bool bFirst = true;
			for ( int i = prototype.ParameterCount; i < prototype.DebugLocalScopes.Length; ++i )
			{
				if ( ! bFirst )
					o.Write( ", " );
				bFirst = false;
				o.Write( prototype.DebugLocalScopes[ i ].Name );
			}
			o.WriteLine();
		}


		// Instructions.

		// TOOD.



		// Final.

		o.WriteLine( "end" );
		o.WriteLine();


		// Other functions.

		for ( int i = 0; i < prototype.Prototypes.Length; ++i )
		{
			Write( o, prototype.Prototypes[ i ] );
		}
	}

}


}