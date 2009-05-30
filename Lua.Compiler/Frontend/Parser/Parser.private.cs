// Parser.private.cs
//
// Lua is copyright © 1994-2008 Lua.org, PUC-Rio
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using System.Diagnostics;
using Lua.Compiler.Frontend.AST;


namespace Lua.Compiler.Frontend.Parser
{


partial class Parser
{
	// Errors.

	TextWriter	errorWriter;
	bool		hasError;


	// Lexer.
	
	Lexer		lexer;
	Token		lookahead;
	Token		token;


	

	// Token reading.

	TokenKind Get()
	{
		return token.Kind;
	}

	Token GetToken()
	{
		return token;
	}

	TokenKind Lookahead()
	{
		if ( lookahead.Kind == TokenKind.None )
		{
			lookahead = lexer.ReadToken();
		}
		return lookahead.Kind;
	}
	
	Token Next()
	{
		Token current = token;

		if ( lookahead.Kind == TokenKind.None )
		{
			token = lexer.ReadToken();
		}
		else
		{
			token		= lookahead;
			lookahead	= new Token();
		}
		
		return current;
	}

	bool Test( TokenKind kind )
	{
		if ( Get() == kind )
		{
			Next();
			return true;
		}
		return false;
	}

	Token Check( TokenKind kind )
	{
		if ( Get() != kind )
		{
			Error( "{0} expected", Lexer.GetTokenName( kind ) );
			return GetToken();
		}

		return Next();
	}

	Token Check( TokenKind kind, Token matchTo )
	{
		if ( Get() != kind )
		{
			Error( "{0} expected to close {1} at line {2}", Lexer.GetTokenName( kind ),
				Lexer.GetTokenName( matchTo.Kind ), matchTo.Location.Line );
			return GetToken();
		}

		return Next();
	}



	// Scopes and expressions.

	Expression Lookup( Token name )
	{
		bool isUpVal = false;
		for ( int scopecount = 0; scopecount < scope.Count; ++scopecount )
		{
			Scope lookupScope = scope.Peek( scopecount );

			// Search this scope.

			for ( int localcount = lookupScope.Locals.Count - 1; localcount >= 0; --localcount )
			{
				Local local = lookupScope.Locals[ localcount ];
				if ( local.Name == (string)name.Value )
				{
					if ( ! isUpVal )
					{
						// Normal local variable.

						return actions.LocalVariableExpression( name.Location, lookupScope, local );
					}
					else
					{
						// UpVals require special handling.

						return actions.UpValExpression( name.Location, lookupScope, local );
					}
				}
			}


			// If we're leaving a function scope then this is an upvalue.

			if ( lookupScope.IsFunctionScope )
			{
				isUpVal = true;
			}
		}


		// Didn't find it, it's a global variable.

		return actions.GlobalVariableExpression( name.Location, (string)name.Value );
	}



	// Error reporting.

	void Error( string format, params object[] args )
	{
		Console.Error.WriteLine( "{0}({1},{2}): error: {3}",
			token.Location.SourceName, token.Location.Line, token.Location.Column,
			System.String.Format( format, args ) );
	}

}



}


