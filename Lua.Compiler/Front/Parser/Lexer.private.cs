// Lexer.private.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using Lua.Compiler.Front.AST;


namespace Lua.Compiler.Front.Parser
{

	
partial class Lexer
{
	const char		EOF = '\uFFFF';

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



	// Errors.

	TextWriter		errorWriter;
	bool			hasError;


	// Location information.

	string			sourceName;
	int				tokenLine;
	int				tokenColumn;
	int				charLine;
	int				charColumn;


	// Reading state.

	TextReader		sourceReader;
	char[]			buffer;
	int				tokenIndex;
	int				charIndex;
	bool			wasNewline;



	void Construct( TextWriter errorWriter, string sourceName, TextReader sourceReader )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;
		
		this.sourceName		= sourceName;
		tokenLine			= 1;
		tokenColumn			= 1;
		charLine			= 1;
		charColumn			= 1;

		this.sourceReader	= sourceReader;
		buffer				= new char[ 0 ];
		tokenIndex			= 0;
		charIndex			= 0;
		wasNewline			= false;

		FillBuffer();
	}


	void FillBuffer()
	{
		if ( charIndex >= buffer.Length )
		{
			// Update buffer.
			char[] newBuffer = null;

			if ( ( charIndex - tokenIndex ) >= buffer.Length )
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
			int charsToCopy = buffer.Length - tokenIndex;
			int charsToRead = newBuffer.Length - charsToCopy;

			Array.Copy( buffer, tokenIndex, newBuffer, 0, charsToCopy );
			int charsRead = sourceReader.Read( newBuffer, charsToCopy, charsToRead );
			if ( charsRead < charsToRead )
			{
				newBuffer[ charsToCopy + charsRead ] = EOF;
			}


			// Replace buffer.
			buffer = newBuffer;


			// Update indices.
			charIndex	-= tokenIndex;
			tokenIndex	-= tokenIndex;
		}
	}


	char GetChar()
	{
		return buffer[ charIndex ];
	}


	void Shift()
	{
		// Move to next character.
		charColumn	+=1 ;
		charIndex	+= 1;


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
		charLine	+= 1;
		charColumn	= 1;
	}
	

	void ResetWasNewline()
	{
		wasNewline = false;
	}


	bool WasNewline()
	{
		return wasNewline;
	}



	void ElideText()
	{
		tokenIndex = charIndex;
	}

	string ReadText()
	{
		string s = new string( buffer, tokenIndex, charIndex - tokenIndex );
		tokenIndex = charIndex;
		return s;
	}


	
	void ElideToken()
	{
		// Update state for next token.
		tokenLine	= charLine;
		tokenColumn	= charColumn;
		tokenIndex	= charIndex;
	}


	Token AcceptToken( TokenKind kind )
	{
		return AcceptToken( kind, null );
	}


	Token AcceptToken( TokenKind kind, object value )
	{	
		// Create token.
		Token token = new Token(
				new SourceLocation( sourceName, tokenLine, tokenColumn ),
				kind,
				value
			);


		// Update state for next token.
		tokenLine	= charLine;
		tokenColumn	= charColumn;
		tokenIndex	= charIndex;
		
		return token;
	}
	

	void Error( string format, params object[] args )
	{
		Console.Error.WriteLine( "{0}({1},{2}): error: {3}",
			sourceName, tokenLine, tokenColumn,
			System.String.Format( format, args ) );
	}



}


}


