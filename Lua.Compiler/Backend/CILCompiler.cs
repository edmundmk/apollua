﻿// CILCompiler.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Lua.Compiler.Intermediate.IR;


namespace Lua.Compiler.Backend
{


/*	Each function is compiled to a class deriving from Lua.Function.
	
	Only one version of each function is compiled.  This is simpler than LuaCLR as
	described in http://portal.acm.org/citation.cfm?doid=1363686.1363743, which
	compiles both a multi-return and a single-return version of each function.
	Instead any function that can potentially return multiple values is compiled
	as a multi-return function.
	
	Coroutine implementation follows http://www.ccs.neu.edu/scheme/pubs/stackhack4.html,
	however when yielding a coroutine, we do not use exceptions because they are slow
	and impose constraints on the generated code.  Instead yielding functions return
	StackFrame values, which are explicitly checked for (which slows down the normal,
	non-yield path, but is simpler and allows the stack frame saving code to be shared).
*/


sealed class CILCompiler
{
	const TypeAttributes typeAttributes =
		  TypeAttributes.AutoLayout
		| TypeAttributes.Class
		| TypeAttributes.Sealed
		| TypeAttributes.AutoClass;


	ModuleBuilder moduleBuilder;


	public CILCompiler( ModuleBuilder moduleBuilder )
	{
		this.moduleBuilder = moduleBuilder;
	}


	public Type Compile( IRCode ir, string name )
	{
		// Create type.

		TypeBuilder builder = moduleBuilder.DefineType( name, typeAttributes | TypeAttributes.Public, typeof( Function ) );
		BuildType( builder, ir );
		return builder.CreateType();
	}


	void BuildType( TypeBuilder builder, IRCode ir )
	{
		/*	Compile contained functions.

			class <name>
				...
		*/

		Dictionary< IRCode, Type > functions = new Dictionary< IRCode, Type >();
		foreach ( IRCode function in ir.Functions )
		{
			TypeBuilder nestedBuilder = builder.DefineNestedType(
				String.Format( "x{0:X}", function.GetHashCode() ), TypeAttributes.NestedPrivate, typeof( Function ) );
			BuildType( nestedBuilder, function );
			functions[ function ] = nestedBuilder.CreateType();
		}



		/*	Declare upvals.

			[ UpVal upval; ]*
		*/

		Dictionary< IRLocal, FieldBuilder > upvals = new Dictionary< IRLocal, FieldBuilder >();
		foreach ( IRLocal upval in ir.UpVals )
		{
			upvals[ upval ] = builder.DefineField( upval.Name, typeof( UpVal ), FieldAttributes.Private );
		}



		/*	Create constructor.  Upval references are set up when the function
			value is created, and are inherited from the enclosing function.

			public .ctor( [ UpVal upval, ]* )
			{
				[ this.upval = upval; ]*
			}
		*/

		Type[] constructorParameters = new Type[ ir.UpVals.Count ];
		for ( int i = 0; i < constructorParameters.Length; ++i )
		{
			constructorParameters[ i ] = typeof( UpVal );
		}

		ConstructorBuilder constructor = builder.DefineConstructor(
			MethodAttributes.Public, CallingConventions.Standard, constructorParameters );

		for ( int i = 0; i < ir.UpVals.Count; ++i )
		{
			constructor.DefineParameter( i + 1, ParameterAttributes.None, ir.UpVals[ i ].Name );
		}

		ILGenerator constructorAssembler = constructor.GetILGenerator();

		constructorAssembler.Emit( OpCodes.Ldarg_0 );
		constructorAssembler.Emit( OpCodes.Call, typeof( Object ).GetConstructor(
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			Type.DefaultBinder, Type.EmptyTypes, null ) );
		
		foreach ( IRLocal upval in ir.UpVals )
		{
			constructorAssembler.Emit( OpCodes.Ldarg_0 );
			constructorAssembler.Emit( OpCodes.Ldarg_1 );
			constructorAssembler.Emit( OpCodes.Stfld, upvals[ upval ] );
		}
		
		constructorAssembler.Emit( OpCodes.Ret );
		


		/*	Function parameters are declared exactly as in the Lua declaration,
			though each function must declare at least one parameter so that we
			can check for coroutine StackFrames.  A function returns multiple
			results if it contains any ReturnMultipleResults statements.

			[ Value | Value[] ] Invoke( Value argument0, [ Value argument, ]* [ params Value[] vararg ]? )
			{
		*/





	
	}




}


/*

	The general structure of a compiled function class is:


	class <name>
		:	Lua.Function
	{
		// All constants are initialized only once, when the function is loaded.

		[ const Value constant = <constant>; ]*
	





		// Function parameters are declared exactly as in the Lua declaration, though each
		// function must declare at least one parameter so that we can check for StackFrames.

		[ Value | Value[] ] Invoke( Value argument0, [ Value argument, ]* [ params Value[] vararg ]? )
		{
			// If the first argument is a StackFrame, we are resuming a suspended continuation.

			if ( argument0 != null && argument0.GetType() == typeof( StackFrame ) )
			{
				goto resume;
			}


			[ Compiled lua code goes here. ]

	
			// Values that are used as upvals must be declared as upvals everywhere they are used.

			Value local0;
			UpVal local1;
	

			// Operations are compiled:

			Value result = [ left ].[ Op ]( right );
	

			// Functions are compiled:

			Value result = [ function ].InvokeS( [ argument, ]* );
		continuation0:
			if ( result != null && result.GetType() == typeof( StackFrame ) )
			{
				goto yield; ( result, 0 )
			}


			// Or for multi-return:

			Value[] results = [ function ].InvokeM( [ argument, ]* )
		continuation1:
			if ( results.Length > 0 && results[ 0 ].GetType() == typeof( StackFrame ) )
			{
				goto yield; ( results[ 0 ], 1 )
			}
	
	
			// Saves the stack frame and indicates that the calling function should also yield.

		yield: ( stackFrame, continuation )
			stackFrame = new StackFrame( stackFrame, continuation );
			[ All arguments and locals are stored into stackFrame. ]
			[ return stackFrame; | return new Value[]{ stackFrame }; ]
	
				
			// Restores the stack frame and continues from where we left off.

		resume: ( stackFrame )
			int continuation = stackFrame.Continuation;
			[ Restore all arguments and locals. ]
			stackFrame = stackFrame.Next;

			switch ( continuation )
			{
			case 0: result = [ function ].ResumeS( stackFrame ); goto continuation0;
			case 1: results = [ function ].ResumeM( stackFrame ); goto continuation1;
			}

			throw new InvalidContinuationException();
			
		}



		// All the various overloads of Invoke are emitted to forward the request
		// directly to the generated function.

		public override Value InvokeS( Value argument )
		{
			[ Marshal parameters and/or return values while calling Invoke ].
		}


	}

	
*/


}