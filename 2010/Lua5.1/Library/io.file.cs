// io.file.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
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

		char GetChar()
		{
			int c = reader.Peek();
			if ( c != -1 )
				return (char)c;
			else
				return '\uFFFF';
		}

		char Shift()
		{
			return (char)reader.Read();
		}

		

		public void read( LuaInterop lua )
		{
			EnsureReader();

			if ( lua.ArgumentCount > 0 )
			{
				List< LuaValue > results = new List< LuaValue >();

				for ( int argument = 0; argument < lua.ArgumentCount; ++argument )
				{
					string type = lua.ArgumentType( argument );
					if ( type == "number" )
					{
						int number = lua.Argument< int >( argument );
						if ( number > 0 )
						{
							// Read actual number of characters.
							char[] s = new char[ number ];
							int count = reader.ReadBlock( s, 0, number );
							if ( count > 0 )
								results.Add( new String( s, 0, count ) );
							else
								results.Add( null );
						}
						else
						{
							// Check for EOF.
							if ( reader.Peek() != -1 )
								results.Add( "" );
							else
								results.Add( null );
						}
					}
					else
					{
						string format = lua.Argument< string >( argument );
						if ( format == "*n" )
						{
							StringBuilder s = new StringBuilder();

							// Read a number token from the file.
							while ( Char.IsWhiteSpace( GetChar() ) )
								Shift();
							if ( GetChar() == '+' || GetChar() == '-' )
								s.Append( Shift() );
							while ( Char.IsDigit( GetChar() ) || GetChar() == '.' )
								s.Append( Shift() );
							if ( GetChar() == 'e' || GetChar() == 'E' )
							{
								s.Append( Shift() );
								if ( GetChar() == '+' || GetChar() == '-' )
									s.Append( Shift() );
							}
							while ( Char.IsLetter( GetChar() ) || Char.IsNumber( GetChar() ) || GetChar() == '_' )
								s.Append( Shift() );

							// Parse the number.
							int integerValue;
							double doubleValue;
							string v = s.ToString();
							if ( v.StartsWith( "0x" ) && Int32.TryParse( v.Substring( 2 ), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out integerValue ) )
								results.Add( integerValue );
							else if ( Int32.TryParse( v, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out integerValue ) )
								results.Add( integerValue );
							else if ( Double.TryParse( v, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue ) )
								results.Add( doubleValue );
							else
								results.Add( null );
						}
						else if ( format == "*a" )
						{
							results.Add( reader.ReadToEnd() ?? "" );
						}
						else if ( format == "*l" )
						{
							results.Add( reader.ReadLine() );
						}
					}
				}

				lua.BeginReturn( results.Count );
				for ( int result = 0; result < results.Count; ++result )
				{
					lua.ReturnResult( result, results[ result ] );
				}
				lua.EndReturn();
			}
			else
			{
				lua.Return( reader.ReadLine() );
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