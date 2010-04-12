// io.file.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Interop;


namespace Lua.Library
{



public static partial class io
{


	public class file
		:	IDisposable
	{
		Stream		stream;
		TextReader	reader;
		TextWriter	writer;


		internal file( Stream s )
		{
			stream = s;
		}

		internal file( TextReader r )
		{
			reader = r;
		}

		internal file( TextWriter w )
		{
			writer = w;
		}


		public void Dispose()
		{
			if ( reader != null )
			{
				reader.Dispose();
				reader = null;
			}
			if ( writer != null )
			{
				writer.Dispose();
				writer = null;
			}
			if ( stream != null )
			{
				stream.Dispose();
				stream = null;
			}
		}

		
		void EnsureReader()
		{
			if ( reader == null )
			{
				reader = new StreamReader( stream );
			}
		}

		void EnsureWriter()
		{
			if ( writer == null )
			{
				writer = new StreamWriter( stream );
			}
		}
		

		public void write( LuaInterop lua )
		{
			EnsureWriter();

			for ( int argument = 0; argument < lua.ArgumentCount; ++argument )
			{
				string type = lua.ArgumentType( argument );
				if ( type == "string" )
					writer.Write( lua.Argument< string >( argument ) );
				else if ( type == "number" )
					writer.Write( lua.Argument< double >( argument ).ToString( "G14" ) );
				else
					throw new ArgumentException( "write() only accepts strings or numbers as arguments." );
			}

			lua.Return();
		}
		
	}


}



}