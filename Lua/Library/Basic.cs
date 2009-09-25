// Basic.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using System.Globalization;
using Lua.Interop;


namespace Lua.Library
{


public class Basic
{
	public delegate LuaFunction Compile( TextReader source, string sourceName );
	public delegate LuaFunction StackLevel( int level );

	
	
	public Compile		CompileHandler		{ get; set; }
	public StackLevel	StackLevelHandler	{ get; set; }
	public TextReader	In					{ get; set; }
	public TextWriter	Out					{ get; set; }

	public LuaTable		Table				{ get; private set; }
	

	LuaFunction pairsGenerator;
	LuaFunction ipairsGenerator;


	public Basic()
	{
		CompileHandler				= delegate( TextReader source, string sourceName ) { throw new NotSupportedException(); };
		StackLevelHandler			= delegate( int level ) { throw new NotSupportedException(); };
		In							= Console.In;
		Out							= Console.Out;

		pairsGenerator				= new LuaDelegateM< LuaTable, LuaValue, LuaValue >( next );
		ipairsGenerator				= new LuaDelegateM< LuaTable, int, LuaValue >( inext );

		LuaTable basic = new LuaTable();

		basic[ "_G" ]				= Table;
		basic[ "_VERSION" ]			= "Lua 5.1";

		basic[ "assert" ]			= new LuaDelegateV< LuaValue, LuaValue >( assert );
		basic[ "collectgarbage" ]	= new LuaDelegateS< string, LuaValue, LuaValue >( collectgarbage );
		basic[ "dofile" ]			= new LuaDelegateM< string, LuaValue >( dofile );
		basic[ "error" ]			= new LuaDelegateV< LuaValue, int >( error );	
		basic[ "getfenv" ]			= new LuaDelegateS< LuaValue, LuaValue >( getfenv );
		basic[ "getmetatable" ]		= new LuaDelegateS< LuaValue, LuaValue >( getmetatable );
		basic[ "ipairs" ]			= new LuaDelegateM< LuaTable, LuaValue >( ipairs );
		basic[ "load" ]				= new LuaDelegateS< LuaFunction, string, LuaFunction >( load );
		basic[ "loadstring" ]		= new LuaDelegateS< string, string, LuaFunction >( loadstring );
		basic[ "next" ]				= new LuaDelegateM< LuaTable, LuaValue, LuaValue >( next );
		basic[ "pairs" ]			= new LuaDelegateM< LuaTable, LuaValue >( pairs );
		basic[ "pcall" ]			= new LuaDelegateMP< LuaFunction, LuaValue, LuaValue >( pcall );
		basic[ "print" ]			= new LuaDelegateVP< LuaValue >( print );
		basic[ "rawequal" ]			= new LuaDelegateS< LuaValue, LuaValue, bool >( rawequal );
		basic[ "rawget" ]			= new LuaDelegateS< LuaTable, LuaValue, LuaValue >( rawget );
		basic[ "rawset" ]			= new LuaDelegateS< LuaTable, LuaValue, LuaValue, LuaTable >( rawset );
		basic[ "select" ]			= new LuaDelegateMP< LuaValue, LuaValue, LuaValue >( select );
		basic[ "setfenv" ]			= new LuaDelegateS< LuaValue, LuaValue, LuaFunction >( setfenv );
		basic[ "setmetatable" ]		= new LuaDelegateS< LuaTable, LuaTable, LuaTable >( setmetatable );
		basic[ "tonumber" ]			= new LuaDelegateS< LuaValue, LuaValue, LuaValue >( tonumber );
		basic[ "tostring" ]			= new LuaDelegateS< LuaValue, string >( tostring );
		basic[ "type" ]				= new LuaDelegateS< LuaValue, string >( type );
		basic[ "unpack" ]			= new LuaDelegateM< LuaTable, LuaValue, LuaValue, LuaValue >( unpack );
		basic[ "xpcall" ]			= new LuaDelegateM< LuaFunction, LuaFunction, LuaValue >( xpcall );

		Table = basic;
	}



	// Constants.

	static readonly LuaValue handlerMetatable	= "__metatable";
	static readonly LuaValue handlerToString	= "__tostring";



	// Exceptions.

	class Error
		:	Exception
	{
		public LuaValue MessageObject	{ get; private set; }

		public Error( LuaValue messageObject )
			:	base( messageObject.ToString() )
		{
		}
	}



	// Functions.


	void assert( LuaValue v, LuaValue message )
	{
		if ( ! v.IsTrue() )
		{
			error( message != null ? message : "assertion failed!", 0 );
		}
	}


	LuaValue collectgarbage( string opt, LuaValue arg )
	{
		if ( opt == "stop" )
		{
			return null;
		}
		else if ( opt == "restart" )
		{
			return null;
		}
		else if ( opt == "collect" )
		{
			GC.Collect();
			return null;
		}
		else if ( opt == "count" )
		{
			return 0;
		}
		else if ( opt == "step" )
		{
			return null;
		}
		else if ( opt == "setpause" )
		{
			return 0;
		}
		else if ( opt == "setstepmul" )
		{
			return 0;
		}

		throw new ArgumentException();
	}


	LuaValue[] dofile( string filename )
	{
		LuaFunction chunk = loadfile( filename );
		return chunk.InvokeM();
	}

	
	void error( LuaValue message, int level )
	{
		// Throw message as an exception (can be caught by pcall)
		// TODO: level should alter the generated stack trace.

		throw new Error( message );
	}


	LuaValue getfenv( LuaValue f )
	{
		// Get function from stack level.
		// TODO: level 0 is the 'thread' environment.

		int level;
		if ( f.TryToInt32( out level ) )
		{
			f = StackLevelHandler( level );
		}


		// Get environment.

		return ( (LuaFunction)f ).Environment;
	}


	LuaValue getmetatable( LuaValue v )
	{
		// nil doesn't have a metatable.

		if ( v == null )
		{
			return null;
		}


		// If the metatable has a "__metatable" handler, return the associated value.

		if ( v.Metatable != null )
		{
			LuaValue metatable = v.Metatable[ handlerMetatable ];
			if ( metatable != null )
			{
				return metatable;
			}
		}


		// Get metatable.

		return v.Metatable;
	}


	LuaValue[] inext( LuaTable table, int key )
	{
		// Get next key.

		key += 1;
		LuaValue value = table[ key ];


		// Return appropriately.

		if ( value != null )
		{
			return new LuaValue[] { key, value };
		}
		else
		{
			return new LuaValue[] { null, null };
		}
	}


	LuaValue[] ipairs( LuaTable table )
	{
		return new LuaValue[] { ipairsGenerator, table, 0 };
	}


	LuaFunction load( LuaFunction function, string chunkname )
	{
		if ( chunkname == null ) chunkname = "=(load)";


		// Accumulate source by calling function.

		string s = "";
		LuaValue part = function.InvokeS();
		while ( part is BoxedString && ( (BoxedString)part ).Value.Length > 0 )
		{
			s += ( (BoxedString)part ).Value;
			part = function.InvokeS();
		}
		if ( part != null && !( part is BoxedString ) )
		{
			throw new ArgumentException();
		}


		// Compile.

		return loadstring( s, chunkname );
	}


	LuaFunction loadfile( string filename )
	{
		if ( filename == null )
		{
			return CompileHandler( In, "<stdin>" );
		}
		else
		{
			using ( TextReader file = File.OpenText( filename ) )
			{
				return CompileHandler( file, filename );
			}
		}
	}


	LuaFunction loadstring( string s, string chunkname )
	{
		if ( chunkname == null ) chunkname = "=(load)";
		return CompileHandler( new StringReader( s ), chunkname );
	}


	LuaValue[] next( LuaTable table, LuaValue key )
	{
		LuaValue value;
		table.Next( ref key, out value );
		return new LuaValue[] { key, value };
	}


	LuaValue[] pairs( LuaTable table )
	{
		return new LuaValue[] { /* next */ null, table, null };
	}


	LuaValue[] pcall( LuaFunction f, params LuaValue[] arguments )
	{
		LuaValue[] results;

		try
		{
			// Attempt call.

			results = f.InvokeM( arguments );

		}
		catch ( Exception e )
		{
			// Return any message from the exception.

			LuaValue message = null;
			if ( e is Error )
			{
				message = ( (Error)e ).MessageObject;
			}
			else if ( e.InnerException is Error )
			{
				message = ( (Error)e.InnerException ).MessageObject;
			}
			else
			{
				message = e.Message;
			}
			return new LuaValue[]{ false, message };
		}


		// Success, repackage results with the success value.
		
		LuaValue[] newResults = new LuaValue[ results.Length + 1 ];
		newResults[ 0 ] = true;
		results.CopyTo( newResults, 1 );
		return newResults;
	}


	void print( params LuaValue[] arguments )
	{
		// Print for debugging.

		bool bFirst = true;
		for ( int i = 0; i < arguments.Length; ++i )
		{
			if ( ! bFirst )
			{
				Out.Write( " " );
			}
			bFirst = false;
			Out.Write( tostring( arguments[ i ] ) );
		}
		Out.WriteLine();
	}


	bool rawequal( object a, object b )
	{
		return a.Equals( b );
	}


	LuaValue rawget( LuaTable table, LuaValue key )
	{
		return table[ key ];
	}


	LuaTable rawset( LuaTable table, LuaValue key, LuaValue value )
	{
		table[ key ] = value;
		return table;
	}


	LuaValue[] select( LuaValue index, params LuaValue[] arguments )
	{
		int i;
		if ( index.TryToInt32( out i ) )
		{
			// Pick correct argument.

			if ( i < arguments.Length )
			{
				LuaValue[] results = new LuaValue[ arguments.Length - i ];
				Array.Copy( arguments, i, results, 0, results.Length );
				return results;
			}
			else
			{
				return new LuaValue[] {};
			}
		}
		else if ( index.EqualsValue( "#" ) )
		{
			// Return number of arguments.

			return new LuaValue[] { arguments.Length };
		}

		throw new ArgumentException();
	}


	LuaFunction setfenv( LuaValue f, LuaValue env )
	{
		// Find function from stack level.
		// TODO: level 0 is the 'thread' environment.

		int level;
		if ( f.TryToInt32( out level ) )
		{
			f = StackLevelHandler( level );
		}


		// Set environment.

		( (LuaFunction)f ).Environment = env;
		return ( (LuaFunction)f );
	}


	LuaTable setmetatable( LuaTable table, LuaTable metatable )
	{
		if ( table == null )
		{
			throw new ArgumentNullException();
		}


		// Can't set metatables where the metatable defines "__metatable".

		if ( table.Metatable != null && table.Metatable[ handlerMetatable ] != null )
		{
			throw new InvalidOperationException();
		}


		// Set metatable.

		table.Metatable = metatable;
		return table;
	}


	LuaValue tonumber( LuaValue v, LuaValue numberBase )
	{
		// If it's already a number return it.

		if ( v.TryToNumberValue( out v ) )
		{
			return v;
		}


		// Convert strings to numbers.

		if ( v is BoxedString )
		{
			string s = ( (BoxedString)v ).Value;


			// Find base.

			int b;
			if ( numberBase == null )
			{
				b = 10;
			}
			else if ( ! numberBase.TryToInt32( out b ) )
			{
				throw new ArgumentException();
			}


	
			if ( b == 10 )
			{
				// Base 10 numbers can potentially be floating-point.

				int integerValue;
				if ( Int32.TryParse( s, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out integerValue ) )
				{
					return integerValue;
				}

				double doubleValue;
				if ( Double.TryParse( s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue ) )
				{
					return doubleValue;
				}
			}
			else if ( b == 16 )
			{
				// Hexadecimal numbers can be prefixed with 0x.

				if ( s.StartsWith( "0x" ) )
				{
					s = s.Substring( 2 );
				}

				int hexValue;
				if ( Int32.TryParse( s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out hexValue ) )
				{
					return hexValue;
				}
			}
			else
			{
				// Otherwise convert using the given base.

				s = s.ToUpperInvariant();
				int integerValue = 0;
				for ( int digit = 0; digit < s.Length; ++digit )
				{
					char c = s[ digit ];
					int d = b;
					
					if ( c >= '0' && c <= '9' )
					{
						d = c - '0';
					}
					else if ( c >= 'a' && c <= 'z' )
					{
						d = 10 + c - 'a';
					}
					else if ( c >= 'A' && c <= 'Z' )
					{
						d = 10 + c - 'A';
					}

					if ( d >= b )
					{
						return null;
					}

					integerValue = integerValue * b + d;
				}

				return integerValue;
			}

		}

		return null;
	}


	string tostring( LuaValue v )
	{
		if ( v == null )
		{
			return "nil";
		}
		

		// Call meta handler.

		if ( v.Metatable != null )
		{
			LuaValue toString = v.Metatable[ handlerToString ];
			if ( toString != null )
			{
				toString.InvokeS( v );
			}
		}


		// Otherwise convert to string normally.

		return v.ToString();
	}


	string type( LuaValue v )
	{
		return v != null ? v.GetLuaType() : null;
	}


	LuaValue[] unpack( LuaTable table, LuaValue start, LuaValue end )
	{
		// Find start and end.

		if ( start == null ) start = 1;
		if ( end == null ) end = table.Length();

		int istart, iend;
		if ( ! start.TryToInt32( out istart ) || ! end.TryToInt32( out iend ) )
		{
			throw new ArgumentException();
		}


		// Pack table values into result array.

		LuaValue[] results = new LuaValue[ iend - istart + 1 ];
		for ( int i = istart; i <= iend; ++i )
		{
			results[ i - istart ] = table[ i ];
		}

		return results;
	}


	LuaValue[] xpcall( LuaFunction f, LuaFunction error )
	{
		LuaValue[] results;
		bool success;

		try
		{
			// Attempt call.

			results = f.InvokeM();
			success = true;

		}
		catch ( Exception e )
		{
			// Recover message.

			LuaValue message = null;
			if ( e is Error )
			{
				message = ( (Error)e ).MessageObject;
			}
			else if ( e.InnerException is Error )
			{
				message = ( (Error)e.InnerException ).MessageObject;
			}
			else
			{
				message = e.Message;
			}


			// Call error handler.

			results = error.InvokeM( message );
			success = false;
		}


		// Repackage results and indicate success.
		
		LuaValue[] newResults = new LuaValue[ results.Length + 1 ];
		newResults[ 0 ] = success;
		results.CopyTo( newResults, 1 );
		return newResults;
	}

	
}


}



