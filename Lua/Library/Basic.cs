﻿// Basic.cs
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


class Basic
{
	public delegate Function Compile( TextReader source, string sourceName );
	public delegate Function StackLevel( int level );

	
	
	public Compile		CompileHandler		{ get; set; }
	public StackLevel	StackLevelHandler	{ get; set; }
	public TextReader	In					{ get; set; }
	public TextWriter	Out					{ get; set; }

	public Table		Table				{ get; private set; }
	

	Function pairsGenerator;
	Function ipairsGenerator;


	public Basic()
	{
		CompileHandler				= delegate( TextReader source, string sourceName ) { throw new NotSupportedException(); };
		StackLevelHandler			= delegate( int level ) { throw new NotSupportedException(); };
		In							= Console.In;
		Out							= Console.Out;

		pairsGenerator				= new LuaFuncResultList< Table, Value, Value >( next );
		ipairsGenerator				= new LuaFuncResultList< Table, int, Value >( inext );

		Table basic = new Table();

		basic[ "_G" ]				= Table;
		basic[ "_VERSION" ]			= "Lua 5.1";

		basic[ "assert" ]			= new LuaAction< Value, Value >( assert );
		basic[ "collectgarbage" ]	= new LuaFunc< string, Value, Value >( collectgarbage );
		basic[ "dofile" ]			= new LuaFuncResultList< string, Value >( dofile );
		basic[ "error" ]			= new LuaAction< Value, int >( error );	
		basic[ "getfenv" ]			= new LuaFunc< Value, Value >( getfenv );
		basic[ "getmetatable" ]		= new LuaFunc< Value, Value >( getmetatable );
		basic[ "ipairs" ]			= new LuaFuncResultList< Table, Value >( ipairs );
		basic[ "load" ]				= new LuaFunc< Function, string, Function >( load );
		basic[ "loadstring" ]		= new LuaFunc< string, string, Function >( loadstring );
		basic[ "next" ]				= new LuaFuncResultList< Table, Value, Value >( next );
		basic[ "pairs" ]			= new LuaFuncResultList< Table, Value >( pairs );
		basic[ "pcall" ]			= new LuaFuncResultListParams< Function, Value, Value >( pcall );
		basic[ "print" ]			= new LuaActionParams< Value >( print );
		basic[ "rawequal" ]			= new LuaFunc< Value, Value, bool >( rawequal );
		basic[ "rawget" ]			= new LuaFunc< Table, Value, Value >( rawget );
		basic[ "rawset" ]			= new LuaFunc< Table, Value, Value, Table >( rawset );
		basic[ "select" ]			= new LuaFuncResultListParams< Value, Value, Value >( select );
		basic[ "setfenv" ]			= new LuaFunc< Value, Value, Function >( setfenv );
		basic[ "setmetatable" ]		= new LuaFunc< Table, Table, Table >( setmetatable );
		basic[ "tonumber" ]			= new LuaFunc< Value, Value, Value >( tonumber );
		basic[ "tostring" ]			= new LuaFunc< Value, string >( tostring );
		basic[ "type" ]				= new LuaFunc< Value, string >( type );
		basic[ "unpack" ]			= new LuaFuncResultList< Table, Value, Value, Value >( unpack );
		basic[ "xpcall" ]			= new LuaFuncResultList< Function, Function, Value >( xpcall );

		Table = basic;
	}



	// Constants.

	static readonly Value handlerMetatable	= "__metatable";
	static readonly Value handlerToString	= "__tostring";



	// Exceptions.

	class Error
		:	Exception
	{
		public Value MessageObject	{ get; private set; }

		public Error( Value messageObject )
			:	base( messageObject.ToString() )
		{
		}
	}



	// Functions.


	void assert( Value v, Value message )
	{
		if ( ! v.IsTrue() )
		{
			error( message != null ? message : "assertion failed!", 0 );
		}
	}


	Value collectgarbage( string opt, Value arg )
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
		else if ( opt == "step ")
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


	Value[] dofile( string filename )
	{
		Function chunk = loadfile( filename );
		return chunk.InvokeM();
	}

	
	void error( Value message, int level )
	{
		// Throw message as an exception (can be caught by pcall)
		// TODO: level should alter the generated stack trace.

		throw new Error( message );
	}


	Value getfenv( Value f )
	{
		// Get function from stack level.
		// TODO: level 0 is the 'thread' environment.

		int level;
		if ( f.TryToInteger( out level ) )
		{
			f = StackLevelHandler( level );
		}


		// Get environment.

		return ( (Function)f ).Environment;
	}


	Value getmetatable( Value v )
	{
		// nil doesn't have a metatable.

		if ( v == null )
		{
			return null;
		}


		// If the metatable has a "__metatable" handler, return the associated value.

		if ( v.Metatable != null )
		{
			Value metatable = v.Metatable[ handlerMetatable ];
			if ( metatable != null )
			{
				return metatable;
			}
		}


		// Get metatable.

		return v.Metatable;
	}


	Value[] inext( Table table, int key )
	{
		// Get next key.

		key += 1;
		Value value = table[ key ];


		// Return appropriately.

		if ( value != null )
		{
			return new Value[] { key, value };
		}
		else
		{
			return new Value[] { null, null };
		}
	}


	Value[] ipairs( Table table )
	{
		return new Value[] { ipairsGenerator, table, 0 };
	}


	Function load( Function function, string chunkname )
	{
		if ( chunkname == null ) chunkname = "=(load)";


		// Accumulate source by calling function.

		string s = "";
		Value part = function.InvokeS();
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


	Function loadfile( string filename )
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


	Function loadstring( string s, string chunkname )
	{
		if ( chunkname == null ) chunkname = "=(load)";
		return CompileHandler( new StringReader( s ), chunkname );
	}


	Value[] next( Table table, Value key )
	{
		Value value;
		table.Next( ref key, out value );
		return new Value[] { key, value };
	}


	Value[] pairs( Table table )
	{
		return new Value[] { /* next */ null, table, null };
	}


	Value[] pcall( Function f, params Value[] arguments )
	{
		Value[] results;

		try
		{
			// Attempt call.

			results = f.InvokeM( arguments );

		}
		catch ( Exception e )
		{
			// Return any message from the exception.

			Value message = null;
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
			return new Value[]{ false, message };
		}


		// Success, repackage results with the success value.
		
		Value[] newResults = new Value[ results.Length + 1 ];
		newResults[ 0 ] = true;
		results.CopyTo( newResults, 1 );
		return newResults;
	}


	void print( params Value[] arguments )
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


	Value rawget( Table table, Value key )
	{
		return table[ key ];
	}


	Table rawset( Table table, Value key, Value value )
	{
		table[ key ] = value;
		return table;
	}


	Value[] select( Value index, params Value[] arguments )
	{
		int i;
		if ( index.TryToInteger( out i ) )
		{
			// Pick correct argument.

			if ( i < arguments.Length )
			{
				Value[] results = new Value[ arguments.Length - i ];
				Array.Copy( arguments, i, results, 0, results.Length );
				return results;
			}
			else
			{
				return new Value[] {};
			}
		}
		else if ( index.Equals( "#" ) )
		{
			// Return number of arguments.

			return new Value[] { arguments.Length };
		}

		throw new ArgumentException();
	}


	Function setfenv( Value f, Value env )
	{
		// Find function from stack level.
		// TODO: level 0 is the 'thread' environment.

		int level;
		if ( f.TryToInteger( out level ) )
		{
			f = StackLevelHandler( level );
		}


		// Set environment.

		( (Function)f ).Environment = env;
		return ( (Function)f );
	}


	Table setmetatable( Table table, Table metatable )
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


	Value tonumber( Value v, Value numberBase )
	{
		// If it's already a number return it.

		if ( v.TryToNumber( out v ) )
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
			else if ( ! numberBase.TryToInteger( out b ) )
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


	string tostring( Value v )
	{
		if ( v == null )
		{
			return "nil";
		}
		

		// Call meta handler.

		if ( v.Metatable != null )
		{
			Value toString = v.Metatable[ handlerToString ];
			if ( toString != null )
			{
				toString.InvokeS( v );
			}
		}


		// Otherwise convert to string normally.

		return v.ToString();
	}


	string type( Value v )
	{
		return v != null ? v.LuaType : null;
	}


	Value[] unpack( Table table, Value start, Value end )
	{
		// Find start and end.

		if ( start == null ) start = 1;
		if ( end == null ) end = table.Length();

		int istart, iend;
		if ( ! start.TryToInteger( out istart ) || ! end.TryToInteger( out iend ) )
		{
			throw new ArgumentException();
		}


		// Pack table values into result array.

		Value[] results = new Value[ iend - istart + 1 ];
		for ( int i = istart; i <= iend; ++i )
		{
			results[ i - istart ] = table[ i ];
		}

		return results;
	}


	Value[] xpcall( Function f, Function error )
	{
		Value[] results;
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

			Value message = null;
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
		
		Value[] newResults = new Value[ results.Length + 1 ];
		newResults[ 0 ] = success;
		results.CopyTo( newResults, 1 );
		return newResults;
	}

	
}


}



