// BinaryOp.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// This file © 2009 Edmund Kapusniak


using System;


namespace Lua.Compiler.Parser.AST.Expressions
{


enum BinaryOp
{
	Add,
	Subtract,
	Multiply,
	Divide,
	IntegerDivide,
	Modulus,
	RaiseToPower,
	Concatenate
}


}


