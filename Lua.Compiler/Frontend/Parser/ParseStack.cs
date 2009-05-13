// ParseStack.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


namespace Lua.Compiler.Frontend.Parser
{


class ParseStack< T >
{
	List< T >	list;
	int			top;

	public ParseStack()
	{
		list	= new List< T >();
		top		= 0;
	}


	// Number of elements in the stack.

	public int Count
	{
		get { return top; }
	}



	// Stack manipulation.

	public void Push( T item )
	{
		list.RemoveRange( top, list.Count - top );
		list.Add( item );
		top = list.Count;
	}

	public T Pop()
	{
		top -= 1;
		return list[ top ];
	}

	public IList< T > Pop( int count )
	{
		top -= count;
		return new StackSlice( list, top, count );
	}

	public T Peek()
	{
		return list[ top - 1 ];
	}

	public T Peek( int count )
	{
		return list[ top - 1 - count ];
	}


	// Access to a subrange of the stack, without copying it.

	class StackSlice
		:	IList< T >
	{
		List< T >	list;
		int			start;
		int			count;

		public int	Count		{ get { return count; } }
		public bool	IsReadOnly	{ get { return true; } }

		public T this[ int index ]
		{
			get
			{
				return list[ start + index ];
			}
			set
			{
				list[ start + index ] = value;
			}
		}


		public StackSlice( List< T > list, int start, int count )
		{
			Debug.Assert( start >= 0 );
			Debug.Assert( start + count <= list.Count );

			this.list	= list;
			this.start	= start;
			this.count	= count;
		}

		public int IndexOf( T item )
		{
			return list.IndexOf( item, start, count );
		}

		public bool Contains( T item )
		{
			return list.Contains( item );
		}

		public void CopyTo( T[] array, int arrayIndex )
		{
			list.CopyTo( start, array, arrayIndex, count );
		}

		public IEnumerator<T> GetEnumerator()
		{
			for ( int i = 0; i < count; ++i )
			{
				yield return this[ i ];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Insert( int index, T item )
		{
			throw new NotSupportedException( "Collection is read-only" );
		}

		public void RemoveAt( int index )
		{
			throw new NotSupportedException( "Collection is read-only" );
		}

		public void Add( T item )
		{
			throw new NotSupportedException( "Collection is read-only" );
		}

		public void Clear()
		{
			throw new NotSupportedException( "Collection is read-only" );
		}

		public bool Remove( T item )
		{
			throw new NotSupportedException( "Collection is read-only" );
		}

	}

}


}


