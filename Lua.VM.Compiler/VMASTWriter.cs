// VMASTWriter.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.IO;
using Lua.Parser.AST;


namespace Lua.VM.Compiler
{


public class VMASTWriter
	:	ASTWriter
{

	public VMASTWriter( TextWriter oWriter )
		:	base( oWriter )
	{
	}

	
	public virtual void Visit( DeclareForIndex s )
	{
		Indent();
		o.Write( "local forindex " );
		o.WriteLine( s.Variable.Name );
	}

	public virtual void Visit( OpcodeForLoop s )
	{
		Indent();
		o.Write( "forloop " );
		o.Write( s.Index.Name );
		o.Write( ", " );
		o.Write( s.Limit.Name );
		o.Write( ", " );
		o.Write( s.Step.Name );
		o.Write( ", " );
		o.Write( s.UserIndex.Name );
		o.Write( " b " );
		o.WriteLine( s.Target.Name );
	}

	public virtual void Visit( OpcodeForPrep s )
	{
		Indent();
		o.Write( "forprep " );
		o.Write( s.Index.Name );
		o.Write( ", " );
		o.Write( s.Limit.Name );
		o.Write( ", " );
		o.Write( s.Step.Name );
		o.Write( " b " );
		o.WriteLine( s.Target.Name );
	}

	public virtual void Visit( OpcodeTForLoop s )
	{
		Indent();
		o.Write( "tforloop " );
		bool bFirst = true;
		foreach ( Variable variable in s.Variables )
		{
			if ( ! bFirst )
				o.Write( ", " );
			bFirst = true;
			o.Write( variable.Name );
		}
		o.Write( " = " );
		o.Write( s.Generator.Name );
		o.Write( "( " );
		o.Write( s.State.Name );
		o.Write( ", " );
		o.Write( s.Control.Name );
		o.Write( " ) b " );
		o.WriteLine( s.Target.Name );
	}

}


}
