// Lexer.rules.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak	


using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;



namespace Lua.Compiler.Frontend.Parser
{

	
partial class Lexer
{
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



	private Token Lex()
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
					Error( "Malformed long bracket {0}", ReadText() );
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


	private void LexComment()
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


	private Token LexIdentifierOrKeyword()
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
		string text = ReadText();
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


	private Token LexString()
	{
		Debug.Assert( GetChar() == '"' || GetChar() == '\'' );

		
		// Opening quote.
		char quote = GetChar();
		Shift();
		ElideText();


		// String body.
		StringBuilder s = new StringBuilder();
		while ( GetChar() != quote )
		{
			// EOF or newline
			if ( GetChar() == EOF || IsNewline( GetChar() ) )
			{
				Error( "Newline in string constant" );
				s.Append( ReadText() );
				return AcceptToken( TokenKind.String, s.ToString() );
			}
			// escape sequence
			else if ( GetChar() == '\\' )
			{
				s.Append( ReadText() );
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
					ElideText();


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
				ElideText();
			}
			// normal character
			else
			{
				Shift();
			}
		}

		s.Append( ReadText() );
		Shift();

		return AcceptToken( TokenKind.String, s.ToString() );
	}


	const int NoLongBracket			= -1;
	const int MalformedLongBracket	= -2;

	private int LexLongBracket()
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


	private Token LexLongString( int level )
	{
		// Long bracket has already been consumed.
		ElideText();
		

		// String body.
		StringBuilder s = new StringBuilder();
		while ( true )
		{
			// closing bracket.
			if ( GetChar() == ']' )
			{
				s.Append( ReadText() );
				if ( LexLongBracket() == level )
				{
					return AcceptToken( TokenKind.String, s.ToString() );
				}
			}
			// end of file.
			else if ( GetChar() == EOF )
			{
				Error( "Unfinished long string" );
				s.Append( ReadText() );
				return AcceptToken( TokenKind.String, s.ToString() );
			}
			// newline.
			else if ( IsNewline( GetChar() ) )
			{
				s.Append( ReadText() );
				s.Append( '\n' );
				ShiftNewline();
				ElideText();
			}
			// normal character.
			else
			{
				Shift();
			}
		}
	}


	private Token LexNumber()
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
		string s = ReadText();

		if ( s.StartsWith( "0x" ) )
		{
			int hexValue;
			if ( Int32.TryParse( s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out hexValue ) )
			{
				return AcceptToken( TokenKind.Number, (double)hexValue );
			}
		}
		else
		{
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
}


}


