// Basic.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;


namespace Lua.Library
{


class Basic
{
	public delegate Function Compile( TextReader source, string sourceName );
	public delegate Function StackLevel( int level );

	
	
	public Compile		CompileHandler		{ get; set; }
	public StackLevel	StackLevelHandler	{ get; set; }

	public Table		Table				{ get; private set; }
	


	public Basic()
	{
		CompileHandler		= delegate( TextReader source, string sourceName ) { throw new InvalidOperationException(); };
		StackLevelHandler	= delegate( int level ) { throw new InvalidOperationException(); };

		Table				= new Table();

	}



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
		else
		{
			throw new ArgumentException();
		}
	}


	void error( Value message, int level )
	{
		throw new Error( message );
	}


	Value[] dofile( string filename )
	{
		Function chunk = loadfile( filename );
		return chunk.InvokeM();
	}


	Value getfenv( Value f )
	{
		int level;
		if ( f.TryToInteger( out level ) )
		{
			f = StackLevelHandler( level );
		}

		return ( (Function)f ).Environment;
	}


	Table getmetatable( Value o )
	{
		return o != null ? o.Metatable : null;
	}


	Value[] ipairs( Table table )
	{
		return new Value[] { /* iterator */ null, table, 0 };
	}


	Function load( Function function, string chunkname )
	{
		if ( chunkname == null ) chunkname = "=(load)";
		// TODO.
		return null;
	}


	Function loadfile( string filename )
	{
		if ( filename == null )
		{
			return CompileHandler( Console.In, "<stdin>" );
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
		try
		{
			Value[] results = f.InvokeM( arguments );
			Value[] newResults = new Value[ results.Length + 1 ];
			newResults[ 0 ] = true;
			results.CopyTo( newResults, 1 );
			return newResults;
		}
		catch ( Exception e )
		{
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
	}


	void print( params Value[] arguments )
	{
		// TODO.
	}


	bool rawequal( object a, object b )
	{
		return a.Equals( b );
	}


	Value rawget( Table table, Value key )
	{
		return table[ key ];
	}


	Value rawset( Table table, Value key, Value value )
	{
		table[ key ] = value;
		return table;
	}


	Value[] select( Value index, params Value[] arguments )
	{
		int i;
		if ( index.TryToInteger( out i ) )
		{
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
			return new Value[] { arguments.Length };
		}
		else
		{
			throw new ArgumentException();
		}
	}










	






}


}