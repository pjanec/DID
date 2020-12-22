using System;
using System.Collections.Generic;
using System.Numerics;

namespace DID.Common
{
	public enum EDIType
	{
		Invalid = 0, // not yet initialized, just reseved item to be filled with info
		Primitive, // int, float, bool, string etc. stored in the embedded Variant
		Enum, // enumeration (stored in the Variant but having its own TypeName)
		Struct, // item having child members (has own TypeName, members listed as child-DumpItems); foe example class or structure; can have a value
		Array, // array variable; items as child-DumpItems
	};

	public enum EValueType
	{
		Invalid = 0,	
		Int,
		Double,
		String,
		Vector4,
		Quaternion
	};


	/// <summary>
	/// A generic tree node holding a key-value pair
	/// </summary>
	public interface IDumpItem : IDisposable
	{
		/// <summary>
		/// The "key" part of the key-value pair
		/// </summary>
		string Name { get { return string.Empty;} set {} }
		
		/// <summary>
		/// The "value" part of the key-value pair
		/// </summary>
		Variant Value { get { return new Variant();} set {} }
		
		/// <summary>
		/// Optional child elements of this item. Null = no children (a leaf element)
		/// </summary>
		IList<IDumpItem> Children { get { return null;} set {} }
		
		/// <summary>
		/// Set whenever the value changes
		/// </summary>
		bool Dirty { get { return false; } set {} }


		bool Expand { get { return false; } set {} }
	}
}
