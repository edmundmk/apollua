// Table.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


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
*/


public sealed class Table
	:	Value
	,	IDictionary< Value, Value >
{

	Dictionary< Value, Value >	hash;
	Value[]						array;
	int							nextLength;
	int							arrayOccupancy;
	int							nextOccupancy;
	int							boundary;
	

	public Table()
		:	this( 0, 0 )
	{
	}

	public Table( int arrayCount, int logHashCount )
	{
		Reset( arrayCount, logHashCount );
	}

	void Reset( int arrayCount, int logHashCount )
	{
		hash			= new Dictionary< Value, Value >( 1 << logHashCount );
		array			= new Value[ arrayCount ];
		nextLength		= 2;
		arrayOccupancy	= 0;
		nextOccupancy	= 0;
		boundary		= 0;
	}




	// Metatable.

	public override Table Metatable
	{
		get;
		set;
	}

	


	// Unary arithmetic operators.

	public override Value Length()
	{
		return new Integer( boundary );
	}




	// Indexing.


	bool TryArrayIndex( Value key, out int index )
	{
		if ( key.TryToInteger( out index ) && 1 <= index )
		{
			index -= 1;
			return true;
		}

		return false;
	}


	public override Value Index( Value key )
	{
		// Try integer index.

		int index;
		if ( TryArrayIndex( key, out index ) && index < array.Length )
		{
			return array[ index ];
		}
		

		// Try hash.

		Value value;
		if ( hash.TryGetValue( key, out value ) )
		{
			return value;
		}


		return Nil.Instance;
	}


	public override void NewIndex( Value key, Value value )
	{
		// Try integer index.

		int index;
		if ( TryArrayIndex( key, out index ) )
		{
			// Update occupancy.

			if ( index < array.Length )
			{
				Value existingValue = array[ index ];
				if ( existingValue != Nil.Instance && value == Nil.Instance )
				{
					arrayOccupancy -= 1;
				}
				else if ( existingValue == Nil.Instance && value != Nil.Instance )
				{
					arrayOccupancy += 1;
				}
			}
			else if ( index < nextLength )
			{
				Value existingValue = Index( key );
				if ( existingValue != Nil.Instance && value == Nil.Instance )
				{
					nextOccupancy -= 1;
				}
				else if ( existingValue == Nil.Instance && value != Nil.Instance )
				{
					nextOccupancy += 1;
				}
			}

			
			// If nextOccupancy is more than 0, and total occupancy is more than
			// half the proposed new length, then rehash.

			if ( nextOccupancy > 0 && ( arrayOccupancy + nextOccupancy ) > array.Length )
			{
				Rehash();
			}

			
			// The key may well fit in the array part.

			if ( index < array.Length )
			{
				// Add it.

				array[ index ] = value;


				// Update boundary.

				if ( value != Nil.Instance )
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

		if ( key == Nil.Instance )
		{
			throw new IndexOutOfRangeException( "Table indices cannot be nil." );
		}

		hash[ key ] = value;
	}



	// Next operation to iterate over all entries.

	public void Next( ref Value key, out Value value )
	{
		// Try integer index.

		int index = -1;
		if ( key == Nil.Instance || TryArrayIndex( key, out index ) )
		{
			// Try next index.

			for ( index += 1; index < array.Length; ++index )
			{
				if ( array[ index ] != Nil.Instance )
				{
					key		= new Integer( index + 1 );
					value	= array[ index ];
					return;
				}
			}


			// Start from the start of the hash.

			key = Nil.Instance;
		}



		// Otherwise search through hash.

		IEnumerator< KeyValuePair< Value, Value > > enumerator = hash.GetEnumerator();

		if ( key != Nil.Instance )
		{
			// Search for the key.

			while ( enumerator.MoveNext() )
			{
				if ( enumerator.Current.Key.Equals( key ) )
				{
					// Return next entry.

					if ( enumerator.MoveNext() )
					{
						key		= enumerator.Current.Key;
						value	= enumerator.Current.Value;
						return;
					}
				}
			}
		}
		else
		{

			// First hash index.

			if ( enumerator.MoveNext() )
			{
				key		= enumerator.Current.Key;
				value	= enumerator.Current.Value;
				return;
			}
		}



		// Finished.

		key		= Nil.Instance;
		value	= Nil.Instance;
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
		Debug.Assert( array[ boundary ] != Nil.Instance );

		for ( boundary += 1; boundary < array.Length; ++boundary )
		{
			if ( array[ boundary ] == Nil.Instance )
			{
				break;
			}
		}
	}


	void Rehash()
	{
		// Extend array until the array is less than half full, or it is half full and
		// there are no integer keys in the second half.

		while ( nextOccupancy > 0 && ( arrayOccupancy + nextOccupancy ) > array.Length )
		{
			// Resize array.
			
			int oldLength = array.Length;
			Array.Resize( ref array, nextLength );
			for ( int i = oldLength; i < array.Length; ++i )
			{
				array[ i ] = Nil.Instance;
			}
			nextLength *= 2;


			// Array will contain all keys in nextOccupancy.

			arrayOccupancy = arrayOccupancy + nextOccupancy;
			nextOccupancy = 0;


			// Move hash keys to integer keys and recalcuate occupancy.

			if ( hash.Count > 0 )
			{
				Dictionary< Value, Value > newHash = new Dictionary< Value, Value >();


				// Update array and new hash.
				
				foreach ( KeyValuePair< Value, Value > entry in hash )
				{
					if ( entry.Value == Nil.Instance )
					{
						continue;
					}

					int index;
					if ( TryArrayIndex( entry.Key, out index ) )
					{
						if ( index < array.Length )
						{
							array[ index ] = entry.Value;
							continue;
						}
						else if ( index < nextLength )
						{
							nextOccupancy += 1;
						}
					}
					
					newHash.Add( entry.Key, entry.Value );
				}


				// Replace hash.
				
				hash = newHash;
			}
		}

	}










	// IDictionary< Value, Value >


	public int Count
	{
		get
		{
			return arrayOccupancy + hash.Count;
		}
	}

	public Value this[ Value key ]
	{
		get
		{
			return Index( key );
		}
		set
		{
			NewIndex( key, value );
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
		NewIndex( key, value );
	}
		
	public void Add( KeyValuePair< Value, Value > item )
	{
		Add( item.Key, item.Value );
	}

	public bool ContainsKey( Value key )
	{
		return Index( key ) != Nil.Instance;
	}

	public bool Contains( KeyValuePair< Value, Value > item )
	{
		return Index( item.Key ).Equals( item.Value );
	}

	public bool TryGetValue( Value key, out Value value )
	{
		value = Index( key );
		return value != Nil.Instance;
	}

	public bool Remove( Value key )
	{
		Value value = Index( key );
		if ( value != Nil.Instance )
		{
			NewIndex( key, Nil.Instance );
			return true;
		}
		return false;
	}

	public bool Remove( KeyValuePair< Value, Value > item )
	{
		Value value = Index( item.Key );
		if ( value.Equals( item.Value ) )
		{
			NewIndex( item.Key, Nil.Instance );
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
			if ( value != Nil.Instance )
			{
				yield return new KeyValuePair< Value, Value >( new Integer( index + 1 ), value );
			}
		}

		foreach ( KeyValuePair< Value, Value > item in hash )
		{
			yield return item;
		}
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return ( (IDictionary< Value, Value >)this ).GetEnumerator();
	}
}


}



