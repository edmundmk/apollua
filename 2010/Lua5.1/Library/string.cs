// string.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2010 Edmund Kapusniak


using System;
using System.Globalization;
using System.Text;
using Lua.Interop;


namespace Lua.Library
{


public static partial class @string
{

	public static LuaTable CreateTable()
	{
		LuaTable @string = new LuaTable();
		@string[ "format" ] = new LuaInteropDelegate( format );
		return @string;
	}


	public static void format( LuaInterop lua )
	{
		string format = lua.Argument< string >( 0 );

		StringBuilder s = new StringBuilder();

		// Iterate through conversion specifications in format.
		int argument = 1;
		int start = 0;
		while ( true )
		{
			// Find next format specification.
			int p = format.IndexOf( '%', start );
			if ( p == -1 )
			{
				s.Append( format.Substring( start ) );
				break;
			}

			// Add chunk of format string.
			s.Append( format.Substring( start, p - start ) );

			// Parse conversion specification.
			char c = format[ p++ ];

			// Flags.
			bool bLeftJustify	= false;
			bool bSignPlus		= false;
			bool bSignSpace		= false;
			bool bAlternateForm	= false;
			bool bPadWithZeroes	= false;

			while ( true )
			{
				c = format[ p++ ];
				switch ( c )
				{
					case '-':	bLeftJustify	= true;	continue;
					case '+':	bSignPlus		= true;	continue;
					case ' ':	bSignSpace		= true; continue;
					case '#':	bAlternateForm	= true; continue;
					case '0':	bPadWithZeroes	= true; continue;
				}

				break;
			}

			if ( bSignSpace && bSignPlus )
				bSignSpace = false;
			if ( bPadWithZeroes && bLeftJustify )
				bPadWithZeroes = false;
			

			// Field width.
			int fieldWidth = 0;

			while ( c >= '0' && c <= '9' )
			{
				int digit = c - '0';
				fieldWidth = fieldWidth * 10 + digit;

				c = format[ p++ ];
			}


			// Precision.
			int precision = -1;

			if ( c == '.' )
			{
				precision = 0;
				c = format[ p++ ];
				
				bool bNegativePrecision = false;
				if ( c == '-' )
				{
					bNegativePrecision = true;
					c = format[ p++ ];
				}
				
				while ( c >= '0' && c <= '9' )
				{
					int digit = c - '0';
					precision = precision * 10 + digit;

					c = format[ p++ ];
				}

				if ( bNegativePrecision )
				{
					precision = -1;
				}
			}


			// Conversion specifier cheracter.
			switch ( c )
			{

			// Integer conversions.

			case 'd': case 'i':
			{
				string value = IntegerFormat( lua.Argument< int >( argument++ ), "D", precision, ref bPadWithZeroes );
				value = AddSign( value, bSignPlus, bSignSpace );
				if ( bPadWithZeroes )
					AppendPadded( s, value, fieldWidth );
				else
					AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			case 'o':
			{
				throw new NotImplementedException( "%o format conversion specifier not implemented." );
			}

			case 'u':
			{
				string value = IntegerFormat( lua.Argument< uint >( argument++ ), "D", precision, ref bPadWithZeroes );
				if ( bPadWithZeroes )
					AppendPadded( s, value, fieldWidth );
				else
					AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			case 'x': case 'X':
			{
				string f = c.ToString();
				string value = IntegerFormat( lua.Argument< int >( argument++ ), f, precision, ref bPadWithZeroes );
				if ( bAlternateForm )
					value = "0" + f + value;
				if ( bPadWithZeroes )
					AppendPadded( s, value, fieldWidth );
				else
					AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}


			// Floating-point conversions.

			case 'f': case 'F':
			{
				double v = lua.Argument< double >( argument++ );

				if ( precision == -1 )
					precision = 6;
				string value = v.ToString( "F" + precision.ToString() );

				if ( ! Double.IsInfinity( v ) && ! ! Double.IsNaN( v ) )
				{
					// Add decimal point if forced.
					if ( bAlternateForm && precision == 0 )
						value += ".";

					// Print.
					value = AddSign( value, bSignPlus, bSignSpace );
					if ( bPadWithZeroes )
					{
						AppendPadded( s, value, fieldWidth );
						break;
					}
				}

				AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

		
			case 'e': case 'E':
			{
				string f = c.ToString();
				double v = lua.Argument< double >( argument++ );

				if ( precision == -1 )
					precision = 6;
				string value = v.ToString( f + precision.ToString() );

				if ( ! Double.IsInfinity( v ) && ! Double.IsNaN( v ) )
				{
					// Add decimal point if forced.
					if ( bAlternateForm )
					{
						int e = value.IndexOf( Char.IsUpper( c ) ? 'E' : 'e' );
						if ( e == -1 )
							e = value.Length;
						if ( value.IndexOf( NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ) == -1 )
							value = value.Insert( e, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator );
					}

					// Print.
					value = AddSign( value, bSignPlus, bSignSpace );
					if ( bPadWithZeroes )
					{
						AppendPadded( s, value, fieldWidth );
						break;
					}
				}

				AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			
			case 'g': case 'G':
			{
				string f = c.ToString();
				double v = lua.Argument< double >( argument++ );

				if ( precision == -1 )
					precision = 6;
				if ( precision == 0 )
					precision = 1;
				string value = v.ToString( f + precision.ToString() );
				
				if ( ! Double.IsInfinity( v ) && ! Double.IsNaN( v ) )
				{
					// Add in decimal point and trailing zeroes if alternate form.
					if ( bAlternateForm )
					{
						int e = value.IndexOf( Char.IsUpper( c ) ? 'E' : 'e' );
						if ( e == -1 )
							e = value.Length;
						if ( value.IndexOf( NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ) == -1 )
						{
							value = value.Insert( e, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ); 
							e += 1;
						}
						int length = precision + 1;
						if ( value.StartsWith( NumberFormatInfo.CurrentInfo.NegativeSign ) )
							length += NumberFormatInfo.CurrentInfo.NegativeSign.Length;
						while ( e < length )
						{
							value = value.Insert( e, "0" );
							e += 1;
						}
					}

					// Print.
					value = AddSign( value, bSignPlus, bSignSpace );
					if ( bPadWithZeroes )
					{
						AppendPadded( s, value, fieldWidth );
						break;
					}
				}

				AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			case 'a': case 'A':
			{
				throw new NotImplementedException( "%" + c.ToString() + " format conversion specifier not supported." );
			}


			// String conversions.

			case 'c':
			{
				int value = lua.Argument< int >( argument++ );
				AppendJustified( s, Char.ConvertFromUtf32( value ), fieldWidth, bLeftJustify );
				break;
			}

			case 's':
			{
				string value = lua.Argument< string >( argument++ );
				if ( precision != -1 && value.Length > precision )
					value = value.Remove( precision );
				AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			case 'q':
			{
				string value = lua.Argument< string >( argument++ );
				
				StringBuilder q = new StringBuilder();
				q.Append( '"' );
				start = 0;
				for ( int i = 0; i < value.Length; ++i )
				{
					string escape = null;
					
					switch ( value[ i ] )
					{
					case '"':	escape = "\\\"";	break;
					case '\n':	escape = "\\n";		break;
					case '\0':	escape = "\\0";		break;
					case '\\':	escape = "\\\\";	break;
					}

					if ( escape != null )
					{
						q.Append( value, start, i - start );
						q.Append( escape );
						start = i + 1;
					}
				}
				q.Append( value, start, value.Length - start ); 
				q.Append( '"' );
				
				value = q.ToString();

				if ( precision != -1 && value.Length > precision )
					value = value.Remove( precision );
				AppendJustified( s, value, fieldWidth, bLeftJustify );
				break;
			}

			case '%':
			{
				s.Append( '%' );
				break;
			}


			// Invalid conversion.

			default:
				throw new FormatException( "Invalid format conversion specifier '%" + c.ToString() + "'." );
			}

			
			start = p;
		}
		

		lua.Return( s.ToString() );
	}


	static string IntegerFormat( long value, string format, int precision, ref bool bPadWithZeroes )
	{
		if ( precision >= 0 )
		{
			bPadWithZeroes = false;
			format += precision.ToString();
		}
		return value.ToString( format );
	}


	static string AddSign( string value, bool bSignPlus, bool bSignSpace )
	{
		if ( ! value.StartsWith( NumberFormatInfo.CurrentInfo.NegativeSign ) )
		{
			if ( bSignPlus )	value = NumberFormatInfo.CurrentInfo.PositiveSign + value;
			if ( bSignSpace )	value = ' ' + value;
		}
		return value;
	}

	
	static void AppendPadded( StringBuilder s, string value, int fieldWidth )
	{
		// Don't pad values that are larger than their fields.
		if ( value.Length >= fieldWidth )
		{
			s.Append( value );
			return;
		}

		// Add sign/hexadecimal specifier.
		int start = 0;
		if ( value.StartsWith( NumberFormatInfo.CurrentInfo.PositiveSign ) )
		{
			s.Append( NumberFormatInfo.CurrentInfo.PositiveSign );
			start = NumberFormatInfo.CurrentInfo.PositiveSign.Length;
		}
		else if ( value.StartsWith( NumberFormatInfo.CurrentInfo.NegativeSign ) )
		{
			s.Append( NumberFormatInfo.CurrentInfo.NegativeSign );
			start = NumberFormatInfo.CurrentInfo.NegativeSign.Length;
		}
		else if ( value.StartsWith( "0x" ) || value.StartsWith( "0X" ) )
		{
			s.Append( value, 0, 2 );
			start = 2;
		}

		// Add padding.
		for ( int i = value.Length; i < fieldWidth; ++i )
			s.Append( '0' );

		// Add value.
		s.Append( value, start, value.Length - start );
	}

	
	static void AppendJustified( StringBuilder s, string value, int fieldWidth, bool bLeftJustify )
	{
		// Don't pad values that are larger than their fields.
		if ( value.Length >= fieldWidth )
		{
			s.Append( value );
			return;
		}
		
		// Pad with spaces.
		if ( bLeftJustify )
		{
			s.Append( value );
			for ( int i = value.Length; i < fieldWidth; ++i )
				s.Append( ' ' );
		}
		else
		{
			for ( int i = value.Length; i < fieldWidth; ++i )
				s.Append( ' ' );
			s.Append( value );
		}
	}





}



}

