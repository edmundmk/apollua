// Statement.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;


namespace Lua.Parser.AST
{


/*	The AST for a function is composed of a list of statements.

	Statements are designed to be easy to manipulate.  Temporaries (which are
	created on assignment and are dead after the first reference to them) and
	valuelist (which represents an array of results) are used to ensure that
	consumers do not have to worry about the semantics of Lua's multiple-assignment
	and multiple-result expressions.

	There are three kinds of structural statements, which are present in begin/end
	pairs.  Blocks represent loops or branches, tests skip the contents if the
	expression is false, and scopes represent variable declaration scope.

	Constructor expressions are a special kind of temporary.  They are live
	beyond a single reference.  They exist during table construction and for
	a single reference afterwards.


	Statements:

		declare <local>
		declare <local> = <expression>
		<target> = <expression>
		evaluate <expression>
		return <expression>
		return <results> [ valuelist | varargs ]?
	
		block <name>
		{
			break <name>
			continue <name>
		}
		
		test <expression>
		{
		}
		
		scope
		{
		}
		
		constructor <constructor>
		{
		}

*/


public abstract class Statement
{
	public SourceSpan SourceSpan { get; private set; }

	
	public Statement( SourceSpan s )
	{
		SourceSpan = s;
	}

}


}

