// IParserActions.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using Lua.Compiler.Frontend.AST;
using Lua.Compiler.Frontend.Parser;


namespace Lua.Compiler.Frontend
{


interface IParserActions
{

	// Blocks.
	
	Scope		Function( SourceLocation l, Scope scope, IList< string > parameternamelist, bool isVararg );
	Code		EndFunction( SourceLocation l, Scope end );
	Scope		Do( SourceLocation l, Scope scope );
	void		EndDo( SourceLocation l, Scope end );
	Scope		If( SourceLocation l, Scope scope, Expression condition );
	Scope		ElseIf( SourceLocation l, Scope scope, Expression condition );
	Scope		Else( SourceLocation l, Scope scope );
	void		EndIf( SourceLocation l, Scope end );
	Scope		While( SourceLocation l, Scope scope, Expression condition );
	void		EndWhile( SourceLocation l, Scope end );
	Scope		Repeat( SourceLocation l, Scope scope );
	void		Until( SourceLocation l, Scope scope, Expression condition );
	Scope		For( SourceLocation l, Scope scope, string varname, Expression start, Expression limit, Expression step );
	void		EndFor( SourceLocation l, Scope end );
	Scope		ForIn( SourceLocation l, Scope scope, IList< string > variablenamelist, IList< Expression > expressionlist );
	void		EndForIn( SourceLocation l, Scope end );



	// Statements.

	void		Local( SourceLocation l, Scope scope, IList< string > namelist, IList< Expression > expressionlist );
	void		Assignment( SourceLocation l, Scope scope, IList< Expression > variablelist, IList< Expression > expressionlist );
	void		CallStatement( SourceLocation l, Scope scope, Expression call );
	void		Break( SourceLocation l, Scope loopScope );
	void		Continue( SourceLocation l, Scope loopScope );
	void		Return( SourceLocation l, Scope functionScope, IList< Expression > expressionlist );



	// Expressions.

	Expression	UnaryExpression( SourceLocation l, Expression operand, TokenKind op );
	Expression	BinaryExpression( SourceLocation l, Expression left, Expression right, TokenKind op );
	Expression	FunctionExpression( SourceLocation l, Code objectCode );
	Expression	LiteralExpression( SourceLocation l, object value );
	Expression	VarargsExpression( SourceLocation l, Scope functionScope );
	Expression	LookupExpression( SourceLocation l, Expression left, Expression key );
	Expression	CallExpression( SourceLocation l, Expression left, IList< Expression > argumentlist );
	Expression	SelfCallExpression( SourceLocation l, Expression left, SourceLocation keyl, string key, IList< Expression > argumentlist );
	Expression	NestedExpression( SourceLocation l, Expression expression );
	Expression	LocalVariableExpression( SourceLocation l, Scope lookupScope, Local local );
	Expression	UpValExpression( SourceLocation l, Scope lookupScope, Local local );
	Expression	GlobalVariableExpression( SourceLocation l, string name );



	// Constructor expressions.

	Scope		Constructor( SourceLocation l, Scope scope );
	void		Field( SourceLocation l, Scope constructorScope, Expression key, Expression value );
	void		Field( SourceLocation l, Scope constructorScope, int key, Expression value );
	void		LastField( SourceLocation l, Scope constructorScope, int key, Expression valuelist );
	Expression	EndConstructor( SourceLocation l, Scope end );


}



}


