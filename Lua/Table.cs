// Table.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// This version copyright © 2009 Edmund Kapusniak


using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Lua
{


/*	Lua tables have a dictionary part and an array part.  From
	http://www.tecgraf.puc-rio.br/~lhf/ftp/doc/jucs05.pdf: "the array
	part is always the largest n such that at least half the slots
	between 1 and n are in use ... and there is at least one used slot
	between n/2 + 1 and n".  This is acheived in this implementation
	by keeping track of the number of non-nil slots between 0 and n/2
	(arrayOccupancy), and between n/2 and n (nextOccupancy), and
	'rehashing' when the total occupancy meets the condition for n.

	The C version of Lua calcuates the length of the array part by
	performing a search for a 'boundary'.  This implementation tracks
	the lowest possible 'boundary' explicitly.

	Removing values sets them to null; this is to allow removing of
	values while iterating using lua's semantics.  From what I can
	tell there is no way to shrink the allocated space for a table.
*/


[DebuggerDisplay( "Count = {Count}" ), DebuggerTypeProxy( typeof( TableDebugView ) )]
public sealed class Table
	:	Value
	,	IDictionary< Value, Value >
{

	struct Element
	{
		public Value	Key;
		public Value	Value;

		int next;
		public int Next
		{
			get { return next - 1; }
			set { next = value + 1; }
		}
	}

	Element[]	hash;
	Value[]		array;
	int			hashOccupancy;
	int			arrayLengthNext;
	int			arrayOccupancy;
	int			arrayOccupancyNext;
	int			boundary;
	

	public Table()
		:	this( 0, 0 )
	{
	}

	public Table( int arrayCount, int hashCount )
	{
		Reset( arrayCount, hashCount );
	}

	void Reset( int arrayCount, int hashCount )
	{
		hash				= new Element[ NextPow2( hashCount ) ];
		array				= new Value[ arrayCount ];
		hashOccupancy		= 0;
		arrayLengthNext		= 2;
		arrayOccupancy		= 0;
		arrayOccupancyNext	= 0;
		boundary			= 0;
	}




	// Metatable.

	public override Table Metatable
	{
		get;
		set;
	}


	// Conversion.

	public override string LuaType
	{
		get { return "table"; }
	}



	// Unary arithmetic operators.

	public override Value Length()
	{
		return new BoxedInteger( boundary );
	}




	// Indexing.

	public override Value Index( Value key )
	{
		Value value = Get( key );
		if ( value != null )
		{
			return value;
		}

		Value h = GetHandler( this, handlerIndex );
		if ( h is Function )
		{
			return h.InvokeS( this, key );
		}
		else if ( h != null )
		{
			return h.Index( key );
		}

		return null;
	}

	public override void NewIndex( Value key, Value value )
	{
		Value existingValue = Get( key );
		if ( existingValue != null )
		{
			Set( key, value );
			return;
		}

		Value h = GetHandler( this, handlerIndex );
		if ( h is Function )
		{
			h.InvokeS( this, key, value );
			return;
		}
		else if ( h != null )
		{
			h.NewIndex( key, value );
			return;
		}

		Set( key, value );
	}


	
	// Next operation to iterate over all entries.

	public void Next( ref Value key, out Value value )
	{
		// Try integer index.

		int index = -1;
		if ( key == null || TryArrayIndex( key, out index ) )
		{
			// Try next index.

			for ( index += 1; index < array.Length; ++index )
			{
				if ( array[ index ] != null )
				{
					key		= new BoxedInteger( index + 1 );
					value	= array[ index ];
					return;
				}
			}


			// Start from the start of the hash.
			
			key = null;
		}



		// Otherwise search through hash.

		int i = 0;

		if ( hash.Length > 0 && key != null )
		{
			// Find key.

			i = key.GetHashCode() & ( hash.Length - 1 );
			if ( hash[ i ].Key != null )
			{
				while ( i != -1 && ! hash[ i ].Key.Equals( key ) )
				{
					i = hash[ i ].Next;
				}
			}
			else
			{
				throw new KeyNotFoundException();
			}

			if ( i == -1 )
			{
				throw new KeyNotFoundException();
			}


			// Start search from next key.

			i += 1;

		}


		// Find next key.
		
		while ( i < hash.Length && hash[ i ].Value == null )
		{
			i += 1;
		}


		// Return key.

		if ( i < hash.Length )
		{
			key = hash[ i ].Key;
			value = hash[ i ].Value;
			return;
		}


		// End of the table.

		key		= null;
		value	= null;
	}







	// Table access.


	bool TryArrayIndex( Value key, out int index )
	{
		if ( key.TryToInteger( out index ) && 1 <= index )
		{
			index -= 1;
			return true;
		}

		return false;
	}


	Value Get( Value key )
	{
		// Try integer index.

		int index;
		if ( TryArrayIndex( key, out index ) && index < array.Length )
		{
			return array[ index ];
		}
		

		// Try hash.

		return GetHash( key );
	}


	void Set( Value key, Value value )
	{
		// Try integer index.

		int index;
		if ( TryArrayIndex( key, out index ) )
		{
			// Update occupancy.

			if ( index < array.Length )
			{
				Value existingValue = array[ index ];
				if ( existingValue != null && value == null )
				{
					arrayOccupancy -= 1;
				}
				else if ( existingValue == null && value != null )
				{
					arrayOccupancy += 1;
				}
			}
			else if ( index < arrayLengthNext )
			{
				Value existingValue = Get( key );
				if ( existingValue != null && value == null )
				{
					arrayOccupancyNext -= 1;
				}
				else if ( existingValue == null && value != null )
				{
					arrayOccupancyNext += 1;
				}
			}

			
			// If nextOccupancy is more than 0, and total occupancy is more than
			// half the proposed new length, then rehash.

			if ( arrayOccupancyNext > 0 && ( arrayOccupancy + arrayOccupancyNext ) > array.Length )
			{
				UpdateOccupancy();
			}

			
			// The key may well fit in the array part.

			if ( index < array.Length )
			{
				// Add it.

				array[ index ] = value;


				// Update boundary.

				if ( value != null )
				{
					if ( boundary == index )
					{
						UpdateBoundary();
					}
				}
				else
				{
					boundary = Math.Min( boundary, index );
				}

				return;
			}

		}


		// Try hash.

		if ( key == null )
		{
			throw new IndexOutOfRangeException( "Table indices cannot be nil." );
		}

		SetHash( key, value );
	}





	// Table updating.

	
	void UpdateBoundary()
	{
		// The boundary is the first array index containing a nil value, where
		// all the previous indices contain non-nil values.
		
		// The boundary always lies in the array part, or one past it:
		//	-> When nextOccupancy is zero, the maximum value for the boundary is one
		//	   past the last array entry, since the next set of values are all nil.
		//	-> When nextOccupancy is nonzero, there is at least one nil value
		//	   in the array or it would be rehashed.

		// This function is called when the previous boundary index becomes non-nil.

		Debug.Assert( boundary < array.Length );
		Debug.Assert( array[ boundary ] != null );

		for ( boundary += 1; boundary < array.Length; ++boundary )
		{
			if ( array[ boundary ] == null )
			{
				break;
			}
		}
	}


	void UpdateOccupancy()
	{
		// Extend array until the array is less than half full, or it is half full and
		// there are no integer keys in the second half.

		while ( arrayOccupancyNext > 0 && ( arrayOccupancy + arrayOccupancyNext ) > array.Length )
		{
			// Resize array.
			
			int oldLength = array.Length;
			Array.Resize( ref array, arrayLengthNext );
			for ( int i = oldLength; i < array.Length; ++i )
			{
				array[ i ] = null;
			}
			arrayLengthNext *= 2;


			// Array will contain all keys in nextOccupancy.

			int oldArrayOccupancyNext = arrayOccupancyNext;
			arrayOccupancy = arrayOccupancy + arrayOccupancyNext;
			arrayOccupancyNext = 0;


			// Move hash keys to integer keys and recalcuate occupancy.

			if ( hashOccupancy > 0 )
			{
				Element[] oldHash = hash;
				hashOccupancy -= oldArrayOccupancyNext;
				hash = new Element[ NextPow2( hashOccupancy ) ];
				
				for ( int i = 0; i < oldHash.Length; ++i )
				{
					Element element = oldHash[ i ];
					
					if ( element.Value == null )
					{
						continue;
					}

					int index;
					if ( TryArrayIndex( element.Key, out index ) )
					{
						if ( index < array.Length )
						{
							array[ index ] = element.Value;
							continue;
						}
						else if ( index < arrayLengthNext )
						{
							arrayOccupancyNext += 1;
						}
					}

					SetHash( element.Key, element.Value );
				}
			}

		}

	}




	// Hash operations.


	static int NextPow2( int x )
	{
		x -= 1;
		x |= ( x >> 1 );
		x |= ( x >> 2 );
		x |= ( x >> 4 );
		x |= ( x >> 8 );
		x |= ( x >> 16 );
		x += 1;
		return x;
	}


	Value GetHash( Value key )
	{
		if ( hash.Length == 0 )
			return null;

		
		// Find key.

		int i = key.GetHashCode() & ( hash.Length - 1 );
		if ( hash[ i ].Key != null )
		{
			while ( i != -1 && ! hash[ i ].Key.Equals( key ) )
			{
				i = hash[ i ].Next;
			}

			if ( i != -1 )
			{
				return hash[ i ].Value;
			}
		}

		return null;
	}


	void SetHash( Value key, Value value )
	{
		int hashCode = key.GetHashCode();
		int i;


		// Try to overwrite old element.

		if ( hash.Length > 0 )
		{
			i = hashCode & ( hash.Length - 1 );
			if ( hash[ i ].Key != null )
			{
				while ( i != -1 && ! hash[ i ].Key.Equals( key ) )
				{
					i = hash[ i ].Next;
				}

				if ( i != -1 )
				{
					if ( hash[ i ].Value != null && value == null )
					{
						hashOccupancy -= 1;
					}
					else if ( hash[ i ].Value == null && value != null )
					{
						hashOccupancy += 1;
					}
					hash[ i ].Value = value;
					return;
				}
			}
		}


		// Need a new element, check if we need to grow.

		if ( hashOccupancy >= hash.Length )
		{
			GrowHash();
		}
		hashOccupancy += 1;


		// New entry.

		i = hashCode & ( hash.Length - 1 );
		if ( hash[ i ].Value == null )
		{
			hash[ i ].Key	= key;
			hash[ i ].Value	= value;
			return;
		}


		// Collision!

		int collision = hash[ i ].Key.GetHashCode() & ( hash.Length - 1 );
		int overflow = collision;
		while ( hash[ overflow ].Value != null )
		{
			overflow += 1;
			if ( overflow >= hash.Length ) overflow = 0;
		}


		if ( collision == i )
		{
			// Colliding element is in the correct position.
			// Put the new element in an overflow position.

			while ( hash[ collision ].Next != -1 )
			{
				collision = hash[ collision ].Next;
			}
			
			hash[ collision ].Next = overflow;
			hash[ overflow ].Key	= key;
			hash[ overflow ].Value	= value;
		}
		else
		{
			// Colliding element is already in the wrong place.
			// Put the new element in the correct position.
			// Put the old element in an overflow position.

			while ( hash[ collision ].Next != i )
			{
				collision = hash[ collision ].Next;
			}

			hash[ collision ].Next = overflow;
			hash[ overflow ].Key	= hash[ i ].Key;
			hash[ overflow ].Value	= hash[ i ].Value;
			hash[ overflow ].Next	= hash[ i ].Next;
			hash[ i ].Key	= key;
			hash[ i ].Value	= value;
		}

	}


	void GrowHash()
	{
		Element[] oldHash = hash;
		hash = new Element[ oldHash.Length > 0 ? oldHash.Length * 2 : 1 ];
		hashOccupancy = 0;

		for ( int i = 0; i < oldHash.Length; ++i )
		{
			Element element = oldHash[ i ];
					
			if ( element.Value == null )
			{
				continue;
			}

			SetHash( element.Key, element.Value );
		}
	}





	// IDictionary< Value, Value >


	public int Count
	{
		get
		{
			return arrayOccupancy + hashOccupancy;
		}
	}

	public Value this[ Value key ]
	{
		get
		{
			return Get( key );
		}
		set
		{
			Set( key, value );
		}
	}
	
	public ICollection< Value > Keys
	{
		get
		{
			List< Value > keys = new List< Value >( Count );
			foreach ( KeyValuePair< Value, Value > item in this )
			{
				keys.Add( item.Key );
			}
			return keys;
		}
	}

	public ICollection< Value > Values
	{
		get
		{
			List< Value > values = new List< Value >( Count );
			foreach ( KeyValuePair< Value, Value > item in this )
			{
				values.Add( item.Value );
			}
			return values;
		}
	}
		
	public bool IsReadOnly
	{
		get { return false; }
	}
	
	public void Add( Value key, Value value )
	{
		Set( key, value );
	}
		
	public void Add( KeyValuePair< Value, Value > item )
	{
		Add( item.Key, item.Value );
	}

	public bool ContainsKey( Value key )
	{
		return Get( key ) != null;
	}

	public bool Contains( KeyValuePair< Value, Value > item )
	{
		return Get( item.Key ).Equals( item.Value );
	}

	public bool TryGetValue( Value key, out Value value )
	{
		value = Get( key );
		return value != null;
	}

	public bool Remove( Value key )
	{
		Value value = Get( key );
		if ( value != null )
		{
			Set( key, null );
			return true;
		}
		return false;
	}

	public bool Remove( KeyValuePair< Value, Value > item )
	{
		Value value = Get( item.Key );
		if ( value.Equals( item.Value ) )
		{
			Set( item.Key, null );
			return true;
		}
		return false;
	}

	public void Clear()
	{
		Reset( 0, 0 );
	}

	public void CopyTo( KeyValuePair< Value, Value >[] array, int arrayIndex )
	{
		foreach ( KeyValuePair< Value, Value > item in this )
		{
			if ( arrayIndex >= array.Length )
			{
				break;
			}
			array[ arrayIndex ] = item;
			arrayIndex += 1;
		}
	}

	public IEnumerator< KeyValuePair< Value, Value > > GetEnumerator()
	{
		for ( int index = 0; index < array.Length; ++index )
		{
			Value value = array[ index ];
			if ( value != null )
			{
				yield return new KeyValuePair< Value, Value >( new BoxedInteger( index + 1 ), value );
			}
		}

		for ( int index = 0; index < hash.Length; ++index )
		{
			Element element = hash[ index ];
			if ( element.Value != null )
			{
				yield return new KeyValuePair< Value, Value >( element.Key, element.Value );
			}
		}
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return ( (IDictionary< Value, Value >)this ).GetEnumerator();
	}



	// Debug view.

	class TableDebugView
	{
		Table table;


		public TableDebugView( Table table )
		{
			this.table = table;
		}


		[DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
		public KeyValuePair< Value, Value >[] Elements
		{
			get
			{
				KeyValuePair< Value, Value >[] elements = new KeyValuePair< Value, Value >[ table.Count ];
				int i = 0;
				foreach ( KeyValuePair< Value, Value > item in table )
				{
					elements[ i ] = item;
					i += 1;
				}
				return elements;
			}
		}
	}


}


}



