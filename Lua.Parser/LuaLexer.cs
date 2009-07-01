// LuaLexer.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Lua.Parser.AST;


namespace Lua.Parser
{


public class LuaLexer
{
	// Constants.

	const char	EOF = '\uFFFF';


	// Errors.

	TextWriter	errorWriter;
	bool		hasError;


	// Character state.

	TextReader	sourceReader;
	char[]		buffer;
	int			startIndex;
	int			index;
	bool		wasNewline;

	
	// Location information.

	string		sourceName;
	int			startLine;
	int			startColumn;
	int			line;
	int			column;



	// Public.
	
	public LuaLexer( TextWriter errorWriter, TextReader sourceReader, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceReader	= sourceReader;
		buffer				= new char[ 0 ];
		startIndex			= 0;
		index				= 0;
		wasNewline			= false;

		this.sourceName		= sourceName;
		startLine			= 1;
		startColumn			= 1;
		line				= 1;
		column				= 1;

		FillBuffer();
	}


	public bool HasError
	{
		get { return hasError; }
	}


	public Token ReadToken()
	{
		return Lex();
	}


	static readonly Dictionary< TokenKind, string > tokenNames = new Dictionary< TokenKind, string >
	{
		{ TokenKind.Identifier,				"<identifier>"	},
		{ TokenKind.String,					"<string>"		},
		{ TokenKind.Number,					"<number>"		},

		{ TokenKind.And,					"and"			},
		{ TokenKind.Break,					"break"			},
		{ TokenKind.Continue,				"continue"		},
		{ TokenKind.Do,						"do"			},
		{ TokenKind.Else,					"else"			},
		{ TokenKind.Elseif,					"elseif"		},
		{ TokenKind.End,					"end"			},
		{ TokenKind.False,					"false"			},
		{ TokenKind.For,					"for"			},
		{ TokenKind.Function,				"function"		},
		{ TokenKind.If,						"if"			},
		{ TokenKind.In,						"in"			},
		{ TokenKind.Local,					"local"			},
		{ TokenKind.Nil,					"nil"			},
		{ TokenKind.Not,					"not"			},
		{ TokenKind.Or,						"or"			},
		{ TokenKind.Repeat,					"repeat"		},
		{ TokenKind.Return,					"return"		},
		{ TokenKind.Then,					"then"			},
		{ TokenKind.True,					"true"			},
		{ TokenKind.Until,					"until"			},
		{ TokenKind.While,					"while"			},
	
		{ TokenKind.NumberSign,				"#"				},
		{ TokenKind.PercentSign,			"%"				},
		{ TokenKind.LeftParenthesis,		"("				},
		{ TokenKind.RightParenthesis,		")"				},
		{ TokenKind.Asterisk,				"*"				},
		{ TokenKind.PlusSign,				"+"				},
		{ TokenKind.Comma,					","				},
		{ TokenKind.HyphenMinus,			"-"				},
		{ TokenKind.FullStop,				"."				},
		{ TokenKind.Solidus,				"/"				},
		{ TokenKind.Colon,					":"				},
		{ TokenKind.Semicolon,				";"				},
		{ TokenKind.LessThanSign,			"<"				},
		{ TokenKind.EqualSign,				"="				},
		{ TokenKind.GreaterThanSign,		">"				},
		{ TokenKind.LeftSquareBracket,		"["				},
		{ TokenKind.RightSquareBracket,		"]"				},
		{ TokenKind.CircumflexAccent,		"^"				},
		{ TokenKind.LeftCurlyBracket,		"{"				},
		{ TokenKind.RightCurlyBracket,		"}"				},

		{ TokenKind.LogicalEqual,			"=="			},
		{ TokenKind.NotEqual,				"~="			},
		{ TokenKind.LessThanOrEqual,		"<="			},
		{ TokenKind.GreaterThanOrEqual,		">="			},
		{ TokenKind.Concatenate,			".."			},
		{ TokenKind.Ellipsis,				"..."			},

		{ TokenKind.NewlineLeftParenthesis,	"("				},
	
		{ TokenKind.EOF,					"<eof>"			},
	};

	public static string GetTokenName( TokenKind kind )
	{
		return tokenNames[ kind ];
	}





	// Lexing.

	Token Lex()
	{
		ResetWasNewline();

		while ( true )
		{
			switch ( GetChar() )
			{
			// end of file
			case EOF:
				return AcceptToken( TokenKind.EOF );

			case '#': Shift(); return AcceptToken( TokenKind.NumberSign );
			case '%': Shift(); return AcceptToken( TokenKind.PercentSign );
			case ')': Shift(); return AcceptToken( TokenKind.RightParenthesis );
			case '*': Shift(); return AcceptToken( TokenKind.Asterisk );
			case '+': Shift(); return AcceptToken( TokenKind.PlusSign );
			case ',': Shift(); return AcceptToken( TokenKind.Comma );
			case '/': Shift(); return AcceptToken( TokenKind.Solidus );
			case ':': Shift(); return AcceptToken( TokenKind.Colon );
			case ';': Shift(); return AcceptToken( TokenKind.Semicolon );
			case '\\': Shift(); return AcceptToken( TokenKind.ReverseSolidus );
			case ']': Shift(); return AcceptToken( TokenKind.RightSquareBracket );
			case '^': Shift(); return AcceptToken( TokenKind.CircumflexAccent );
			case '{': Shift(); return AcceptToken( TokenKind.LeftCurlyBracket );
			case '}': Shift(); return AcceptToken( TokenKind.RightCurlyBracket );

			// ambiguity resolution.
			case '(':
				Shift();
				if ( ! WasNewline() )
				{
					return AcceptToken( TokenKind.LeftParenthesis );
				}
				else
				{
					return AcceptToken( TokenKind.NewlineLeftParenthesis );
				}


			// = or ==
			case '=':
				Shift();
				if ( GetChar() != '=' )
				{
					return AcceptToken( TokenKind.EqualSign );
				}
				else
				{
					Shift();
					return AcceptToken( TokenKind.LogicalEqual );
				}

			// ~=
			case '~':
				Shift();
				if ( GetChar() == '=' )
				{
					Shift();
					return AcceptToken( TokenKind.NotEqual );
				}
				else
				{
					Error( "Expecting = to create not-equal sign ~=" );
					ElideToken();
					break;
				}

			// < or <=
			case '<':
				Shift();
				if ( GetChar() != '=' )
				{
					return AcceptToken( TokenKind.LessThanSign );
				}
				else
				{
					Shift();
					return AcceptToken( TokenKind.LessThanOrEqual );
				}
			
			// > or >=
			case '>':
				Shift();
				if ( GetChar() != '=' )
				{
					return AcceptToken( TokenKind.GreaterThanSign );
				}
				else
				{
					Shift();
					return AcceptToken( TokenKind.GreaterThanOrEqual );
				}

			// . or .. or ... or NUMBER
			case '.':
				Shift();
				if ( GetChar() == '.' )
				{
					Shift();
					if ( GetChar() == '.' )
					{
						Shift();
						return AcceptToken( TokenKind.Ellipsis );
					}
					return AcceptToken( TokenKind.Concatenate );
				}
				else if ( ! Char.IsDigit( GetChar() ) )
				{
					return AcceptToken( TokenKind.FullStop );
				}
				else
				{
					return LexNumber();
				}


			// minus sign or comment
			case '-':
				Shift();
				if ( GetChar() != '-' )
				{
					return AcceptToken( TokenKind.HyphenMinus );
				}
				else
				{
					Shift();
					LexComment();
					continue;
				}

			// string
			case '"':
			case '\'':
				return LexString();


			// left square bracket or long string
			case '[':
				int level = LexLongBracket();
				if ( level == NoLongBracket )
				{
					return AcceptToken( TokenKind.LeftSquareBracket );
				}
				else if ( level != MalformedLongBracket )
				{
					return LexLongString( level );
				}
				else
				{
					Error( "Malformed long bracket {0}", GetText() );
					ElideToken();
					continue;
				}
		
		
			default:
				// identifier or keyword
				if ( Char.IsLetter( GetChar() ) || GetChar() == '_' )
				{
					return LexIdentifierOrKeyword();
				}
				// number
				else if ( Char.IsDigit( GetChar() ) )
				{
					return LexNumber();
				}
				// newline
				else if ( IsNewline( GetChar() ) )
				{
					ShiftNewline();
					ElideToken();
					continue;
				}
				// whitespace
				else if ( Char.IsWhiteSpace( GetChar() ) )
				{
					Shift();
					ElideToken();
					continue;
				}
				// unexpected character
				else
				{
					Error( "Unexpected character {0}", GetChar() );
					Shift();
					ElideToken();
					continue;
				}
			}

		}
	}


	void LexComment()
	{
		// '--' has already been consumed.
		
		// Check for long comment.
		if ( GetChar() == '[' )
		{
			int level = LexLongBracket();
			if ( level >= 0 )
			{
				LexLongString( level );
				return;
			}
		}
		
		// Otherwise we're a short comment.
		while ( ! IsNewline( GetChar() ) && GetChar() != EOF )
		{
			Shift();
		}
		
		// Do not return it.
		ElideToken();
	}


	static readonly Dictionary< string, TokenKind > keywords = new Dictionary< string, TokenKind >()
	{
		{ "and",		TokenKind.And		},
		{ "break",		TokenKind.Break		},
		{ "continue",	TokenKind.Continue	},
		{ "do",			TokenKind.Do		},
		{ "else",		TokenKind.Else		},
		{ "elseif",		TokenKind.Elseif	},
		{ "end",		TokenKind.End		},
		{ "false",		TokenKind.False		},
		{ "for",		TokenKind.For		},
		{ "function",	TokenKind.Function	},
		{ "if",			TokenKind.If		},
		{ "in",			TokenKind.In		},
		{ "local",		TokenKind.Local		},
		{ "nil",		TokenKind.Nil		},
		{ "not",		TokenKind.Not		},
		{ "or",			TokenKind.Or		},
		{ "repeat",		TokenKind.Repeat	},
		{ "return",		TokenKind.Return	},
		{ "then",		TokenKind.Then		},
		{ "true",		TokenKind.True		},
		{ "until",		TokenKind.Until		},
		{ "while",		TokenKind.While		},
	};

	Token LexIdentifierOrKeyword()
	{
		Debug.Assert( Char.IsLetter( GetChar() ) || GetChar() == '_' );

		// First character.
		Shift();
		
		// Remaining characters.
		while ( Char.IsLetter( GetChar() ) || Char.IsNumber( GetChar() ) || GetChar() == '_' )
		{
			Shift();
		}

		// Check for keywords.
		string text = GetText();
		TokenKind keyword;
		if ( ! keywords.TryGetValue( text, out keyword ) )
		{
			return AcceptToken( TokenKind.Identifier, text );
		}
		else
		{
			return AcceptToken( keyword );
		}
	}


	Token LexString()
	{
		Debug.Assert( GetChar() == '"' || GetChar() == '\'' );
				
		// Opening quote.
		char quote = GetChar();
		Shift();
		GetText();

		// String body.
		StringBuilder s = new StringBuilder();
		while ( GetChar() != quote )
		{
			// EOF or newline
			if ( GetChar() == EOF || IsNewline( GetChar() ) )
			{
				Error( "Newline in string constant" );
				s.Append( GetText() );
				return AcceptToken( TokenKind.String, s.ToString() );
			}
			// escape sequence
			else if ( GetChar() == '\\' )
			{
				s.Append( GetText() );
				Shift();

				// newline.
				if ( IsNewline( GetChar() ) )
				{
					s.Append( '\n' );
					ShiftNewline();
				}
				// numerical escape sequence.
				else if ( Char.IsDigit( GetChar() ) )
				{
					// Parse three-digit decimal character code.
					int character = 0;
					for ( int i = 0; i < 3; ++i )
					{
						if ( ! Char.IsDigit( GetChar() ) )
							break;

						int digit = GetChar() - '0';
						character = 10 * character + digit;
						Shift();
					}
					
					// Do not add the escape sequence digits.
					GetText();
					
					// Add escaped character.
					if ( character <= 0x7F )
					{
						s.Append( (char)character );
					}
					else
					{
						Error( "Character escape sequence \\{0} is too large", character );
					}

				}
				else switch ( GetChar() )
				{
				// end of file.
				case EOF:
					// Loop round to the error, above.
					break;

				// actual escape sequence.
				case 'a': Shift(); s.Append( '\a' ); break;
				case 'b': Shift(); s.Append( '\b' ); break;
				case 'f': Shift(); s.Append( '\f' ); break;
				case 'n': Shift(); s.Append( '\n' ); break;
				case 'r': Shift(); s.Append( '\r' ); break;
				case 't': Shift(); s.Append( '\t' ); break;
				case 'v': Shift(); s.Append( '\v' ); break;

				// character 'escape' (including quote and backslash).
				default:
					Shift();
					break;
				}

				// Do not add text of escape sequence to the string.
				GetText();
			}
			// normal character
			else
			{
				Shift();
			}
		}

		s.Append( GetText() );
		Shift();

		return AcceptToken( TokenKind.String, s.ToString() );
	}


	const int NoLongBracket			= -1;
	const int MalformedLongBracket	= -2;

	int LexLongBracket()
	{
		Debug.Assert( GetChar() == '[' || GetChar() == ']' );
		
		// Opening bracket.
		char bracket = GetChar();
		Shift();
		
		// Count number of = characters.
		int level = 0;
		while ( GetChar() == '=' )
		{
			level += 1;
			Shift();
		}
		
		// Check for closing bracket.
		if ( GetChar() == bracket )
		{
			Shift();
			return level;
		}
		else if ( level > 0 )
		{
			return MalformedLongBracket;
		}
		else
		{
			return NoLongBracket;
		}
	}


	Token LexLongString( int level )
	{
		// Long bracket has already been consumed.
		GetText();
		
		// String body.
		StringBuilder s = new StringBuilder();
		while ( true )
		{
			// closing bracket.
			if ( GetChar() == ']' )
			{
				s.Append( GetText() );
				if ( LexLongBracket() == level )
				{
					return AcceptToken( TokenKind.String, s.ToString() );
				}
			}
			// end of file.
			else if ( GetChar() == EOF )
			{
				Error( "Unfinished long string" );
				s.Append( GetText() );
				return AcceptToken( TokenKind.String, s.ToString() );
			}
			// newline.
			else if ( IsNewline( GetChar() ) )
			{
				s.Append( GetText() );
				s.Append( '\n' );
				ShiftNewline();
				GetText();
			}
			// normal character.
			else
			{
				Shift();
			}
		}
	}


	Token LexNumber()
	{
		Debug.Assert( Char.IsDigit( GetChar() ) );

		// Number part.
		while ( Char.IsDigit( GetChar() ) || GetChar() == '.' )
		{
			Shift();
		}

		// Exponent marker and sign.
		if ( GetChar() == 'E' || GetChar() == 'e' )
		{
			Shift();
			if ( GetChar() == '+' || GetChar() == '-' )
			{
				Shift();
			}
		}

		// Exponent and trailing characters.
		while ( Char.IsLetter( GetChar() ) || Char.IsNumber( GetChar() ) || GetChar() == '_' )
		{
			Shift();
		}

		// Parse number.
		string s = GetText();

		if ( s.StartsWith( "0x" ) )
		{
			int hexValue;
			if ( Int32.TryParse( s.Substring( 2 ), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out hexValue ) )
			{
				return AcceptToken( TokenKind.Number, hexValue );
			}
		}
		else
		{
			int integerValue;
			if ( Int32.TryParse( s, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out integerValue ) )
			{
				return AcceptToken( TokenKind.Number, integerValue );
			}

			double doubleValue;
			if ( Double.TryParse( s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue ) )
			{
				return AcceptToken( TokenKind.Number, doubleValue );
			}
		}
		
		// Error.
		Error( "Malformed number {0}", s );
		return AcceptToken( TokenKind.Number, 0.0d );
	}


	

	// Accepting tokens.


	void ElideToken()
	{
		// Update state for next token.
		startLine	= line;
		startColumn	= column;
		startIndex	= index;
	}


	Token AcceptToken( TokenKind kind )
	{
		return AcceptToken( kind, null );
	}


	Token AcceptToken( TokenKind kind, object value )
	{	
		// Create token.
		Token token = new Token(
				new SourceSpan(
					new SourceLocation( sourceName, startLine, startColumn ),
					new SourceLocation( sourceName, line, column ) ),
				kind,
				value
			);


		// Update state for next token.
		startLine	= line;
		startColumn	= column;
		startIndex	= index;
		
		return token;
	}



	// Character reading.

	void FillBuffer()
	{
		if ( index >= buffer.Length )
		{
			// Update buffer.
			char[] newBuffer = null;

			if ( ( index - startIndex ) >= buffer.Length )
			{
				// Create new buffer.
				newBuffer = new char[ Math.Max( buffer.Length * 2, 32 ) ];
			}
			else
			{
				// Use existing buffer.
				newBuffer = buffer;
			}


			// Copy and fill.
			int charsToCopy = buffer.Length - startIndex;
			int charsToRead = newBuffer.Length - charsToCopy;

			Array.Copy( buffer, startIndex, newBuffer, 0, charsToCopy );
			int charsRead = sourceReader.Read( newBuffer, charsToCopy, charsToRead );
			if ( charsRead < charsToRead )
			{
				newBuffer[ charsToCopy + charsRead ] = EOF;
			}


			// Replace buffer.
			buffer = newBuffer;


			// Update indices.
			index		-= startIndex;
			startIndex	-= startIndex;
		}
	}

	char GetChar()
	{
		return buffer[ index ];
	}

	void Shift()
	{
		// Move to next character.
		column	+= 1;
		index	+= 1;


		// Refill buffer.
		FillBuffer();
	}
		
	bool IsNewline( char c )
	{
		return c == '\u000A'	// LINE FEED
			|| c == '\u000C'	// FORM FEED
			|| c == '\u000D'	// CARRIAGE RETURN
			|| c == '\u0085'	// NEXT LINE
			|| c == '\u2028'	// LINE SEPARATOR
			|| c == '\u2029';	// PARAGRAPH SEPARATOR
	}
	
	void ShiftNewline()
	{
		// We shifted a newline between the last token and this one.
		wasNewline = true;


		// Check for CR+LF pair.
		if ( GetChar() == '\u000D' )
		{
			Shift();
			if ( GetChar() == '\u000A' )
			{
				Shift();
			}
		}
		else
		{
			Debug.Assert( IsNewline( GetChar() ) );
			Shift();
		}
	

		// Update line information.
		line	+= 1;
		column	= 1;
	}
	
	void ResetWasNewline()
	{
		wasNewline = false;
	}

	bool WasNewline()
	{
		return wasNewline;
	}

	string GetText()
	{
		string s = new string( buffer, startIndex, index - startIndex );
		startIndex = index;
		return s;
	}



	// Errors.

	void Error( string format, params object[] args )
	{
		Error( new SourceLocation( sourceName, startLine, startColumn ), format, args );
	}

	void Error( SourceLocation l, string format, params object[] args )
	{
		Console.Error.WriteLine( "{0}({1},{2}): error: {3}",
			l.SourceName, l.Line, l.Column, System.String.Format( format, args ) );
		hasError = true;
	}


}


}

