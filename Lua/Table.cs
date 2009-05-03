// Table.cs
//
// Lua 5.1 is copyright © 1994-2008 Lua.org, PUC-Rio, released under the MIT license
// LuaCLR is copyright © 2007-2008 Fabio Mascarenhas, released under the MIT license
// Modifications copyright © 2009 Edmund Kapusniak


using System;
using System.Collections.Generic;


namespace Lua
{


public sealed class Table
	:	Value
	,	IDictionary< Value, Value >
{
	// Metatable.

	public override Table Metatable
	{
		get;
		set;
	}



	// Table implementation.

	class Node 
	{
		public static readonly Node DummyNode	= new Node( 0 );
		public static readonly Node NilNode		= new Node( 0 );

		public int		Position;
		public Value	Key;
		public Value	Value;
		public Node		Next;

		public Node( int position )
		{
			Position	= position;
			Key			= Nil.Instance;
			Value		= Nil.Instance;
			Next		= null;	
		}

		public void Copy( Node n ) 
		{
			Key			= n.Key;
			Value		= n.Value;
			Next		= n.Next;
		}
	}


	static readonly int maxBits = 32;


	static readonly byte[] log8 = new byte[]
	{
		0,
		1,1,
		2,2,2,2,
		3,3,3,3,3,3,3,3,
		4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
		6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
		6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
		7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
		7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
		7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
		7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7
	};


	Node[]	array;
	int[]	arrayOccupancyUpToLog;

	Node[]	hash;
	byte	logHashSize;
	int		firstFree;



	// Initialization.


	public Table()
		:	this( 0, 0 )
	{
	}

	
	public Table( int arraySize, int logHashSize )
	{
		arrayOccupancyUpToLog = new int[ maxBits + 1 ];
		SetArrayVector( array.Length );
		SetHashVector( logHashSize );
	}

	



	// Key positions.


	Node MainPosition( Value key )
	{
		int hashCode = key.GetHashCode() % ( hash.Length - 1 );
		return hash[ hashCode ];
	}


	bool TryArrayIndex( Value key, out int index )
	{
		// Try for an integer in the correct range.

		return key.TryToInteger( out index ) && 1 <= index && index <= array.Length;
	}
	

	Node Get( Value key ) 
	{
		// Nil key is never in the table.

		if ( key == Nil.Instance )
		{
			return Node.NilNode;
		}
	
		
		// Integer key.

		int index;
		if ( TryArrayIndex( key, out index ) )
		{
			return array[ index - 1 ];
		}


		// General value key.

		Node n = MainPosition( key );
		do 
		{
			// Check whether key is somewhere in the chain.
			
			if ( key.Equals( n.Key ) )
			{
				return n;
			}

			n = n.Next;
		}
		while ( n != null );
		
		return Node.NilNode;
	}
	

	Node NewKey( Value key )
	{
		// Find main position.

		Node mp = MainPosition( key );
		

		// Check if main position is free.

		if ( mp.Value != Nil.Instance ) 
		{
			Node othern = MainPosition( mp.Key );	// mp of colliding node.
			Node n = hash[ firstFree ];				// free place.


			// Is the colliding node in it's main position?

			if ( othern != mp ) 
			{
				// No, move colliding node into free position.

				while ( othern.Next != mp )
				{
					othern = othern.Next;			// find previous.
				}

				othern.Next = n;					// redo the chain with n in place of mp.
				n.Copy( mp );						// copy colliding node into free pos (mp->Next also goes).

				mp.Next	= null;						// now mp is free.
				mp.Value = Nil.Instance;
			}
			else
			{
				// Yes, colliding node is in its own main position, new node will be place in free position.

				n.Next = mp.Next;
				mp.Next = n;
				mp = n;
			}
		}


		// Place in position.

		mp.Key = key;


		// Update firstFree.

		while ( true )
		{
			if ( hash[ firstFree ].Key == Nil.Instance )
			{
				// Table still has a free place, return the node we found.

				return mp;
			}
			if ( firstFree == 0 )
			{
				break;
			}

			firstFree -= 1;
		}


		// No more free places, must create one.

		mp.Value = Boolean.False;	// avoid new key being removed.
		Rehash();
		mp = Get( key );			// find where it got shuffled to.
		mp.Value = Nil.Instance;	// reset value.

		return mp;
	}




	// Rehashing.


	void Rehash() 
	{
		// Compute new sizes for array and hash parts.

		int newArraySize, newHashSize;
		NumUse( out newArraySize, out newHashSize);


		// Tries to have at least 50% free space on hash for better performance profile.

		int logNewHashSize = FastLog2( (uint)newHashSize );
		logNewHashSize = ( 1 << logNewHashSize ) == newHashSize ? logNewHashSize + 1 : logNewHashSize + 2;
		Resize( newArraySize, logNewHashSize );
	}


	void NumUse( out int newArraySize, out int newHashSize ) 
	{
		// Initialize array.

		for( int k = 0; k < arrayOccupancyUpToLog.Length; k++ )
		{
			arrayOccupancyUpToLog[ k ] = 0;
		}


		// Count array elements.

		int totaluse = 0;
		


		// In array part.

		int i, log;
		for ( i = 0, log = 0; log <= maxBits; log++ ) 
		{ 
			// For each slice [2^(log-1) to 2^log)

			int sliceEnd = ( 1 << log );  // 2^log
			if ( sliceEnd > array.Length ) 
			{
				sliceEnd = array.Length;
				if ( i >= sliceEnd )
				{
					break;
				}
			}
			for ( ; i < sliceEnd; i++ ) 
			{
				if ( array[ i ].Value != Nil.Instance ) 
				{
					arrayOccupancyUpToLog[ log ] += 1;
					totaluse += 1;
				}
			}
		}

		newArraySize = totaluse;  // all previous uses were in array part.


		// In hash part.

		i = hash.Length;
		while ( i-- > 0 )
		{
			Node n = hash[ i ];
			if ( n.Value != Nil.Instance )
			{
				int k;
				if ( n.Key.TryToInteger( out k ) && 1 <= k )
				{
					// k is an integer key, count it.

					arrayOccupancyUpToLog[ FastLog2( (uint)k - 1 ) + 1 ] += 1;
					newArraySize += 1;
				}
				totaluse += 1;
			}
		}


		// Compute sizes.

		int arrayOccupancyTest	= arrayOccupancyUpToLog[ 0 ];
		int arrayOccupancy		= arrayOccupancyTest;
		int logOptimalArraySize	= ( arrayOccupancy == 0 ) ? -1 : 0;

		for ( log = 1; arrayOccupancyTest < newArraySize && newArraySize >= ( 1 << ( log - 1 ) ); log++ ) 
		{
			if ( arrayOccupancyUpToLog[ log ] == 0 ) 
				continue;


			// Accumulate total number of array elements up to this log.

			arrayOccupancyTest += arrayOccupancyUpToLog[ log ];


			// Are more than half the elements up to this log in use?

			if ( arrayOccupancyTest >= ( 1 << ( log - 1 ) ) ) 
			{
				logOptimalArraySize = i;
				arrayOccupancy = arrayOccupancyTest;
			}
		}

		newHashSize = totaluse - arrayOccupancy;
		newArraySize = ( logOptimalArraySize == -1 ) ? 0 : ( 1 << logOptimalArraySize );
	}


	int FastLog2(uint x)
	{
		if ( x >= 0x00010000 )
		{
			if ( x >= 0x01000000 )
			{
				return log8[ ( ( x >> 24 ) & 0xff ) - 1 ] + 24;
			}
			else
			{
				return log8[ ( ( x >> 16 ) & 0xff ) - 1 ] + 16;
			}
		}
		else 
		{
			if ( x >= 0x00000100 )
			{
				return log8[ ( ( x >> 8 ) & 0xff ) - 1 ] + 8;
			}
			else if( x != 0 )
			{
				return log8[ ( x & 0xff ) - 1 ];
			}
			return -1;  // special `log' for 0
		}
	}


	void Resize( int newArraySize, int logNewHashSize ) 
	{
		// Save old sizes.

		int oldArraySize	= array.Length;
		int logOldHashSize	= logHashSize;


		// Save old hash.

		Node[] oldHash;

		if ( logOldHashSize != 0 )
		{
			oldHash = this.hash;  /* save old hash ... */
		}
		else 
		{
			oldHash = new Node[] { new Node( 0 ) };
			oldHash[ 0 ].Copy( hash[ 0 ] );
		}


		// Grow array part if necessary.

		if ( newArraySize > oldArraySize )
		{
			SetArrayVector( newArraySize );
		}


		// Create new hash part with appropriate size.

		SetHashVector( logNewHashSize );


		// Reinsert elements into the array if it's shrinking.

		if ( newArraySize < oldArraySize ) 
		{
			// Create new array.

			Node[] oldArray = array;
			array = new Node[ newArraySize ];


			// Copy over elements that are not vanishing.

			for ( int i = 0; i < array.Length; i++ )
			{
				array[ i ] = oldArray[ i ];
			}


			// Reinsert elements from vanishing slice (into the hash).

			for ( int i = newArraySize; i < oldArraySize; i++ ) 
			{
				if ( array[ i ].Value != Nil.Instance )
				{
					NewIndex( new Integer( i + 1 ), array[ i ].Value );
				}
			}
		}


		// Reinsert elements into the hash.
		
		for ( int i = ( 1 << logOldHashSize ) - 1; i >= 0; i-- ) 
		{
			Node old = oldHash[ i ];
			if ( old.Value != Nil.Instance )
			{
				NewIndex( old.Key, old.Value );
			}
		}
	}




	// Resizing.

	void SetArrayVector( int newArraySize )
	{
		Node[] newArray = new Node[ newArraySize ];

		for( int i = 0; i < array.Length; i++ )
		{
			newArray[ i ] = array[ i ];
		}

		for( int i = array.Length; i < newArray.Length; i++ )
		{
			newArray[ i ] = new Node( i );
		}

		array = newArray;
	}


	void SetHashVector( int logNewHashSize )
	{
		int newHashSize = (1 << logNewHashSize);

		if ( logNewHashSize > maxBits )
		{
			throw new Exception( "Table overflow" );
		}

		if ( logNewHashSize == 0 ) 
		{
			// No elements to hash part?

			hash	= new Node[ 1 ];
			hash[0] = Node.DummyNode;
		}
		else 
		{
			// Resize hash part.

			hash = new Node[ newHashSize ];
			for ( int i = 0; i < newHashSize; i++ ) 
			{
				hash[ i ] = new Node( i );
			}
		}
		
		logHashSize = (byte)logNewHashSize;
		firstFree = newHashSize - 1;
	}




	// Unary arithmetic operators.

	public override Value Length()
	{
		/*	Try to find a boundary. A `boundary' is an integer index such that t[ i ]
			is non-nil and t[ i + 1 ] is nil (and 0 if t[ 1 ] is nil).
		*/
	
		int searchFirst, searchLast;


		// Check array part.

		searchLast = array.Length;
		if ( searchLast > 0 && array[ searchLast - 1 ].Value == Nil.Instance )
		{
			// There is a boundary in the array part: (binary) search for it.

			searchFirst = 0;
			while ( searchLast - searchFirst > 1 )
			{
				int searchPivot = ( searchFirst + searchLast ) / 2;
				if ( array[ searchPivot - 1 ].Value == Nil.Instance )
				{
					searchLast = searchPivot;
				}
				else
				{
					searchFirst = searchPivot;
				}
			}

			return new Integer( searchFirst );
		}



		// Check for empty hash part.

		if ( hash[ 0 ] == Node.DummyNode )
		{
			return new Integer( searchLast );
		}



		// Search hash part.

		searchFirst = searchLast;
		searchLast += 1;


		// Find first and last such that first is present and last is not.

		while ( true )
		{
			// Check for a non-present last.

			if ( Get( new Integer( (int)searchLast ) ).Value == Nil.Instance )
			{
				break;
			}


			// last is present.

			searchFirst = searchLast;
			checked { searchLast *= 2; }
		}

		

		// Perform binary search between first and last.

		while ( searchLast - searchFirst > 1 )
		{
			int searchPivot = ( searchFirst + searchLast ) / 2;
			if ( Get( new Integer( (int)searchPivot ) ).Value == Nil.Instance )
			{
				searchLast = searchPivot;
			}
			else
			{
				searchFirst = searchPivot;
			}
		}

		return new Integer( searchFirst );
	}






	// Indexing.

	public override Value Index( Value key )
	{
		// Find node for this key.

		return Get( key ).Value;
	}
	
	public override void NewIndex( Value key, Value value )
	{
		// Find node at which to set this key.

		Node p = Get( key );

		if ( p != Node.NilNode )
		{
			// Got a valid node, set directly.

			p.Value = value;
		} 
		else if ( value != Nil.Instance )
		{
			// Create a new node.

			if ( key == Nil.Instance )
			{
				throw new IndexOutOfRangeException( "Table index is nil." );
			}
			
			p = NewKey( key );
			p.Value = value;
		}
	}




	/*	Returns the index of a `key' for table traversals. First goes through
		all elements in the array part, then elements in the hash part. The
		beginning of a traversal is signalled by -1.
	*/

	public void Next( ref Value key, out Value value )
	{
		// Check for integer keys.

		int index = 0;
		if ( key == Nil.Instance || TryArrayIndex( key, out index ) )
		{
			// Find next key in array.

			for ( index += 1; ( index - 1 ) < array.Length; index++ )
			{
				if ( array[ index ].Value != Nil.Instance )
				{
					key		= new Integer( index );
					value	= array[ index - 1 ].Value;
					return;
				}
			}


			// No more array keys, find first hash key.

			for ( int i = 0; i < hash.Length; i++ )
			{
				Node firstHash = hash[ i ];
				if ( firstHash.Value != Nil.Instance )
				{
					key		= firstHash.Key;
					value	= firstHash.Value;
					return;
				}
			}


			// Table is empty.
			
			key		= Nil.Instance;
			value	= Nil.Instance;
			return;
		}


		// Recover position of the key.

		Node currentHash = MainPosition( key );
		do
		{
			if( key.Equals( currentHash.Key ) )
			{
				break;
			}
			currentHash = currentHash.Next;
		}
		while ( currentHash != null );

		if ( currentHash == null )
		{
			throw new ArgumentException( "Invalid key to next." );
		}
		

		// Find next key.

		for( int i = currentHash.Position + 1; i < hash.Length; i++ )
		{
			Node nextHash = hash[ i ];
			if ( nextHash.Value != Nil.Instance )
			{
				key		= nextHash.Key;
				value	= nextHash.Value;
				return;
			}
		}


		// Reached the end of the table.

		key		= Nil.Instance;
		value	= Nil.Instance;
	}




	// IDictionary< Value, Value >


	public int Count
	{
		get
		{
			int arraySize, hashSize;
			NumUse( out arraySize, out hashSize );
			return arraySize + hashSize;
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
		return Index( item.Key ) == item.Value;
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
		if ( value == item.Value )
		{
			NewIndex( item.Key, Nil.Instance );
			return true;
		}
		return false;
	}

	public void Clear()
	{
		SetArrayVector( 0 );
		SetHashVector( 0 );
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
		Value key	= Nil.Instance;
		Value value	= Nil.Instance;

		while ( true )
		{
			// Get next key.

			Next( ref key, out value );
			

			// Check for end of the table.
			
			if ( key == Nil.Instance )
			{
				break;
			}


			// Yield the appropriate value.

			yield return new KeyValuePair< Value, Value >( key, value );
		}
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return ( (IDictionary< Value, Value >)this ).GetEnumerator();
	}
}


}



