// TokenKind.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser
{


enum TokenKind
{
	None = 0,


	// Tokens.
	
	Identifier,
	String,
	Number,

	
	// Keywords.
	
	And,
	Break,
	Continue,
	Do,
	Else,
	Elseif,
	End,
	False,
	For,
	Function,
	If,
	In,
	Local,
	Nil,
	Not,
	Or,
	Repeat,
	Return,
	Then,
	True,
	Until,
	While,
	

	// Operators.
	
	NumberSign,
	PercentSign,
	LeftParenthesis,
	RightParenthesis,
	Asterisk,
	PlusSign,
	Comma,
	HyphenMinus,
	FullStop,
	Solidus,
	Colon,
	Semicolon,
	LessThanSign,
	EqualSign,
	GreaterThanSign,
	LeftSquareBracket,
	ReverseSolidus,
	RightSquareBracket,
	CircumflexAccent,
	LeftCurlyBracket,
	RightCurlyBracket,

	LogicalEqual,			// ==
	NotEqual,				// ~=
	LessThanOrEqual,		// <=
	GreaterThanOrEqual,		// >=
	Concatenate,			// ..
	Ellipsis,				// ...


	// There is an ambiguity in the Lua grammar.  An expression followed by a left parenthesis could
	// be either the start of a function call or of the next expression.  This is resolved in favour
	// of the function call, except when there is a newline between the expression and the left
	// parenthesis, which is an error.

	NewlineLeftParenthesis,


	// End of file.
	
	EOF

}


}


