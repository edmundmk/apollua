// NextPow2.cs
//
// © Edmund Kapusniak 2010


using System;


namespace Lua.Utility
{


/*	Finds the next highest power of two.
*/

static partial class MathEx
{

	public static int NextPow2( int x )
	{
		x -= 1;
		x |= ( x >> 1 );
		x |= ( x >> 2 );
		x |= ( x >> 4 );
		x |= ( x >> 8 );
		x |= ( x >> 16 );
		x += 1;
		return x;
	}

}


}


