// RaiseToPower.cs
//
// © Edmund Kapusniak 2010


using System;


namespace Lua.Utility
{


/*	Integer raise-to-power operation.
*/

static partial class MathEx
{

	public static int Pow( int x, int y )
	{
		if ( y < 0 )
			throw new ArgumentOutOfRangeException();

		int result = 1;
		int xpowerbit = x;
		int bit = 1;

		while ( y != 0 )
		{
			if ( ( y & bit ) != 0)
			{
				result *= xpowerbit;
				y &= ~bit;
			}
			bit <<= 1;
			xpowerbit *= xpowerbit;
		}

		return result;
	}

}



}

