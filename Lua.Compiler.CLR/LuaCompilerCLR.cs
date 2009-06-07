// LuaCompilerCLR.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Lua;
using Lua.Parser;
using Lua.Parser.AST;
using Lua.Parser.AST.Expressions;
using Lua.Parser.AST.Statements;


namespace Lua.Compiler.CLR
{


/*	Each function is compiled to a class deriving from Lua.Function.
	
	Only one version of each function is compiled.  This is simpler than LuaCLR as
	described in http://portal.acm.org/citation.cfm?doid=1363686.1363743, which
	compiles both a multi-return and a single-return version of each function.
	Instead any function that can potentially return multiple values is compiled
	as a multi-return function.
	
	Coroutine implementation follows http://www.ccs.neu.edu/scheme/pubs/stackhack4.html.
	However, when yielding a coroutine, we do not use exceptions because they are slow
	and impose constraints on the generated code.  Instead yielding functions return
	StackFrame values, which are explicitly checked for (which slows down the normal,
	non-yield path, but is simpler and allows the stack frame saving code to be shared).
*/


public class LuaCompilerCLR
{
	// Errors.

	TextWriter				errorWriter;
	bool					hasError;


	// Parser.

	string					sourceName;
	LuaParser				parser;


	// Type building.

	static object			defaultModuleBuilderLock = new Object();
	static ModuleBuilder	defaultModuleBuilder;
	ModuleBuilder			moduleBuilder;


	public LuaCompilerCLR( TextWriter errorWriter, TextReader source, string sourceName )
		:	this( null, errorWriter, source, sourceName )
	{
	}

	public LuaCompilerCLR( ModuleBuilder moduleBuilder, TextWriter errorWriter, TextReader source, string sourceName )
	{
		this.errorWriter	= errorWriter;
		hasError			= false;

		this.sourceName		= sourceName;
		parser				= new LuaParser( errorWriter, source, sourceName );

		if ( moduleBuilder != null )
		{
			this.moduleBuilder	= moduleBuilder;
		}
		else
		{
			lock ( defaultModuleBuilderLock )
			{
				AssemblyBuilder	assemblyBuilder =
					AppDomain.CurrentDomain.DefineDynamicAssembly(
						new AssemblyName( "LuaCompilerCLR.Default" ), AssemblyBuilderAccess.Run );
				defaultModuleBuilder = assemblyBuilder.DefineDynamicModule( "Lua.CLR" );
				this.moduleBuilder = defaultModuleBuilder;
			}
		}
	}


	public bool HasError
	{
		get { return hasError || parser.HasError; }
	}


	public Function Compile()
	{
		// Parse the function.

		FunctionAST functionAST = parser.Parse();
		if ( functionAST == null )
		{
			return null;
		}
		

		// A-normal transform.

		functionAST = ANormalTransform.Transform( functionAST );
		ASTWriter.Write( Console.Out, functionAST );


		// Compile to CLR.

		string typeName = Path.GetFileNameWithoutExtension( sourceName );
		TypeBuilder typeBuilder = moduleBuilder.DefineType( typeName,
			TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class |
			TypeAttributes.AutoLayout | TypeAttributes.AutoClass, typeof( Function ) );
		IList< TypeBuilder > nestedTypes = new List< TypeBuilder >();
		CompileFunction( typeBuilder, functionAST, nestedTypes );


		// Create types in the correct order (parent types before nested types).

		Type type = typeBuilder.CreateType();
		foreach ( TypeBuilder nestedType in nestedTypes )
		{
			nestedType.CreateType();
		}
		


		// We have created a type for the function.

		return null;
	}


	void CompileFunction( TypeBuilder typeBuilder, FunctionAST functionAST, IList< TypeBuilder > outNestedTypes )
	{
		/*
			class <name>
				:	Lua.Function
			{
		*/


		/*
			class <nestedfunction> : Lua.Function { ... }
			class <nestedfunction> : Lua.Function { ... }
		*/

		IDictionary< FunctionAST, TypeBuilder > functions = new Dictionary< FunctionAST, TypeBuilder >();
		foreach ( FunctionAST nestedFunctionAST in functionAST.Functions )
		{			
			// Find appropriate name.

			string typeName = nestedFunctionAST.Name;
			if ( typeName == null )
			{
				typeName = String.Format( "x{0:X}", nestedFunctionAST.GetHashCode() );
			}


			// Declare nested type and compile function.

			TypeBuilder nestedTypeBuilder = typeBuilder.DefineNestedType( typeName,
				TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Class |
				TypeAttributes.AutoLayout | TypeAttributes.AutoClass, typeof( Function ) );

			functions[ nestedFunctionAST ] = nestedTypeBuilder;
			outNestedTypes.Add( nestedTypeBuilder );
			
			CompileFunction( nestedTypeBuilder, nestedFunctionAST, outNestedTypes );
		}



		/*
			UpVal <upvalname>;
			UpVal <upvalname>;
		*/

		IDictionary< Variable, FieldBuilder > upvals = new Dictionary< Variable, FieldBuilder >();
		foreach ( Variable upval in functionAST.UpVals )
		{
			FieldBuilder fieldBuilder = typeBuilder.DefineField( upval.Name,
				typeof( UpVal ), FieldAttributes.Private );
			upvals[ upval ] = fieldBuilder;
		}



		/*
			public <name>( UpVal <upvalname>, UpVal <upvalname> )
			{
				this.<upvalname> = <upvalname>;
				this.<upvalname> = <upvalname>;
			}
		*/

		Type[] constructorParamTypes = new Type[ functionAST.UpVals.Count ];
		for ( int i = 0; i < constructorParamTypes.Length; ++i )
		{
			constructorParamTypes[ i ] = typeof( UpVal );
		}

		ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
			MethodAttributes.Public | MethodAttributes.HideBySig,
			CallingConventions.Standard, constructorParamTypes );

		for ( int i = 0; i < constructorParamTypes.Length; ++i )
		{
			constructorBuilder.DefineParameter( i + 1, ParameterAttributes.None,
				functionAST.UpVals[ i ].Name );
		}


		ConstructorInfo functionConstructor = typeof( Function ).GetConstructor(
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			Type.DefaultBinder, Type.EmptyTypes, null );

		ILGenerator constructorIL = constructorBuilder.GetILGenerator();
		constructorIL.Emit( OpCodes.Ldarg_0 );
		constructorIL.Emit( OpCodes.Call, functionConstructor );
		constructorIL.Emit( OpCodes.Ret );
		


		/*
			Value[] Invoke( Value <argumentname>, Value <argumentname> ( , params Value[] vararg )? )
			{
		*/


		// Return type.

		Type returnType;
		if ( ! functionAST.ReturnsMultipleValues )
		{
			returnType = typeof( Value );
		}
		else
		{
			returnType = typeof( Value[] );
		}


		// Parameter types.

		int parameterCount = functionAST.Parameters.Count;
		if ( functionAST.IsVararg )
		{
			parameterCount += 1;
		}
		
		Type[] parameterTypes = new Type[ parameterCount ];
		for ( int i = 0; i < functionAST.Parameters.Count; ++i )
		{
			parameterTypes[ i ] = typeof( Value );
		}
		if ( functionAST.IsVararg )
		{
			parameterTypes[ functionAST.Parameters.Count ] = typeof( Value[] );
		}


		// Define method.

		MethodBuilder methodBuilder = typeBuilder.DefineMethod( "Invoke",
			MethodAttributes.Private | MethodAttributes.HideBySig,
			CallingConventions.HasThis, returnType, parameterTypes );


		// Declare parameters

		for ( int i = 0; i < functionAST.Parameters.Count; ++i )
		{
			methodBuilder.DefineParameter( i + 1, ParameterAttributes.None,
				functionAST.Parameters[ i ].Name );
		}
		
		if ( functionAST.IsVararg )
		{
			ParameterBuilder varargParameter = methodBuilder.DefineParameter(
				functionAST.Parameters.Count + 1, ParameterAttributes.None, "..." );
			varargParameter.SetCustomAttribute( new CustomAttributeBuilder(
				typeof( ParamArrayAttribute ).GetConstructor( Type.EmptyTypes ), new Object[] {} ) );
		}

		ILGenerator methodIL = methodBuilder.GetILGenerator();
		

				
		/*
			// Check if we are resuming a suspended continuation.

			if ( StackFrame != null )
			{
				goto resume;
			}
		*/



		/*
			// Compiled code.
		*/


		Builder builder = new Builder( functionAST, functions, upvals );
		ExpressionCompiler	ec = new ExpressionCompiler( methodIL, builder );
		StatementCompiler	sc = new StatementCompiler( methodIL, builder );

		foreach ( Statement statement in functionAST.Statements )
		{
			statement.Accept( sc );
		}


		methodIL.Emit( OpCodes.Ldnull );
		methodIL.Emit( OpCodes.Ret );

		

		/*
	
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

		resume:
			int continuation = StackFrame.Continuation;
			[ Restore all arguments and locals. ]
			StackFrame = StackFrame.Next;

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


	class Builder
	{
		public FunctionAST								FunctionAST		{ get; private set; }
		public IDictionary< FunctionAST, TypeBuilder >	Functions		{ get; private set; }
		public IDictionary< Variable, FieldBuilder >	UpVals			{ get; private set; }


		public Builder( FunctionAST functionAST, IDictionary< FunctionAST, TypeBuilder > functions,
			IDictionary< Variable, FieldBuilder > upvals )
		{
			FunctionAST	= functionAST;
			Functions	= functions;
			UpVals		= upvals;
		}

	}



	class Block
	{
		public Label Break			{ get; private set; }
		public Label Continue		{ get; private set; }


		public Block( Label breakLabel, Label continueLabel )
		{
			Break		= breakLabel;
			Continue	= continueLabel;
		}
	}



	sealed class StatementCompiler
		:	StatementVisitor
	{
		ILGenerator								il;
		Builder									b;
		IDictionary< string, Stack< Block > >	block;

		public StatementCompiler( ILGenerator ilGenerator, Builder builder )
		{
			il		= ilGenerator;
			b		= builder;

			block	= new Dictionary< string, Stack< Block > >();
		}


		public override void Visit( BeginBlock s )
		{
			// Track this block.

			if ( ! block.ContainsKey( s.Name ) )
			{
				block[ s.Name ] = new Stack< Block >();
			}

			block[ s.Name ].Push( new Block( il.DefineLabel(), il.DefineLabel() ) );
			

			// Mark the continue label.

			il.MarkLabel( block[ s.Name ].Peek().Continue );
		}
		
		public override void Visit( EndBlock s )
		{
			il.MarkLabel( block[ s.Name ].Peek().Break );
			block[ s.Name ].Pop();
		}

		public override void Visit( BeginConstructor s )		{}
		public override void Visit( EndConstructor s )			{}
		public override void Visit( BeginScope s )				{}
		public override void Visit( EndScope s )				{}
		public override void Visit( BeginTest s )				{}
		public override void Visit( EndTest s )					{}

		public override void Visit( Assign s )					{}
		public override void Visit( Break s )					{}
		public override void Visit( Continue s )				{}
		public override void Visit( Declare s )					{}
		public override void Visit( Evaluate s )				{}
		public override void Visit( IndexMultipleValues s )		{}
		public override void Visit( Return s )					{}
		public override void Visit( ReturnMultipleValues s )	{}
	}


	sealed class ExpressionCompiler
		:	ExpressionVisitor
	{
		ILGenerator	il;
		Builder		b;

		public ExpressionCompiler( ILGenerator ilGenerator, Builder builder )
		{
			il	= ilGenerator;
			b	= builder;
		}


		public override void Visit( Binary e )					{}
		public override void Visit( Call e )					{}
		public override void Visit( CallSelf e )				{}
		public override void Visit( Comparison e )				{}
		public override void Visit( Constructor e )				{}
		public override void Visit( FunctionClosure e )			{}
		public override void Visit( GlobalRef e )				{}
		public override void Visit( Index e )					{}
		public override void Visit( Literal e )					{}
		public override void Visit( LocalRef e )				{}
		public override void Visit( Logical e )					{}
		public override void Visit( Not e )						{}
		public override void Visit( Temporary e )				{}
		public override void Visit( ToNumber e )				{}
		public override void Visit( Unary e )					{}
		public override void Visit( UpValRef e )				{}
		public override void Visit( ValueList e )				{}
		public override void Visit( ValueListElement e )		{}
		public override void Visit( Vararg e )					{}
		public override void Visit( VarargElement e )			{}
	}
	


}


}
