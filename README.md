# DID
Remote dumping and introspection framework for C#.

Allows for an easy and low-effort diagnostics of a running application, remotely over the network and without the need for debug info.


Planned Features

* Dumping of application internals (structures, arrays...) in a fast way (no reflection)
* Publishing the dumps to a remote client to be watched in real time (as in a debugger)
* Watching selected data from multiple servers simultaneously
* Changing the app data on request from a remote client
* Invoking custom actions in the dumping app on request from a remote client
* Low performance impact
  * Just stuff being currently watched by some client is dumped.
  * No runtime reflection used while dumping
* Optimized data transfer
  * Server sends just the items being currently watched by some client
  * Just changes are sent
  * [FBE library]( https://github.com/chronoxor/FastBinaryEncoding)-based data serialization
* TCP based client-server architecture.
  * Server in a dumped app, multiple remote clients.
* Automatic scanning of available servers
* Simple ImGui.NET based client UI
* Kera-Lua based custom UI configuration
  * Predefining set of data items to be watched
  * Runtime watching for specific combination of servers or published data becoming available and displaying them in a custom format.

WARNING!

So far just a skeleton of classes with many missing parts.



# Example

```c#
using DID.Common;
using DID.Client;

namespace Test
{
	// some class that supports dumping
	public class MyClass : Dmp.IDumpable
	{
		int xInt = 7;
		double xDouble = -5.2;

		public void Dump( Dmp.DumpItem d )
		{
			// define the type of the dump item
			Dmp.Struct(d, "MyClass");

			// define subitems as key-value pairs
			Dmp.KV( d, "xInt", xInt );
			Dmp.KV( d, "xDouble", xDouble );
		}
	}

	// some class containing another class
	public class ContainerClass : Dmp.IDumpable
	{
		int xInt = 7;
		double xDouble = -5.2;
		MyClass myClass = new MyClass();
		List<int> xIntList = {1,2,3};

		public void Dump( Dmp.DumpItem d )
		{
			// define the type of the dump item
			Dmp.Struct(d, "ContainerClass");

			// define subitems as key-value pairs
			
			Dmp.KV( d, "my Int", xInt );
			
			Dmp.KV( d, "my Double", ref xDouble ) // marking by "ref" means the remote client can change the value
			   .WriteBackActionable( () => // optional callback on data changed by remote
				{
					Console.WriteLine("my Double field have been set from remote!") 
				}
			);

			Dmp.KV( d, "my Class", myClass )
			   .Actionable( () => // allow the client to invoke an action for this item
				{
					Console.WriteLine("Action has been fired on myClass field") 
				}
			);
			
			Dmp.KV( d, "my IntList", xIntList );
		}
	}
	
	class Program
	{
		// sample data
		static ContainerClass myCC = new ContainerClass();

		static void Dump( DumpItem d )
		{
			Dmp.Struct( d, "My Dump Tree" );
			Dmp.KV( d, "my class", myCC );
		}
		
		static void Main(string[] args)
		{
			var dumpMan = TopicManager.Instance;

			// define a topic (holding a dump tree)
			var t1 = new Topic(
				"topic-type-1", // name of the topic type
				"topic-instance-1", // name of the topic instance
				(x) => Dump(x), // dump function
				"topic-group-1", // topic group name (all topics in a group are dumped at the same time)
				0.5 // dumping interval
			);

			// register the topic for dumping
			// note: the topic is being dumped only as long as a remote client is watching that topic
			dumpMan.RegisterTopic( t1 );

			Console.WriteLine( "Press a key to stop..." );
			while( !Console.KeyAvailable )
			{
				// tick the dumping engine to invoke dump given topic group from the "right" thread
				dumpMan.Tick("topic-group-1");

				Thread.Sleep( 50 );
			}
		}
	}
}
```


# Concepts
The dumping framework is based on the application (dumping module) writing its data into a tree of key-value pairs (Dump Items).

There is no automatic reflection involved, all fields to be dumped need to be explicitly mentioned.

## Dump Item
 * A named node in a dump tree, can have child items.
 * Can represent a single value, a group of another items (structure), an array of values...
 * Does not provide any formatting/displaying capabilities, this needs to be done by an external code.
 * Holds TypeName which is an arbitrary human understandable string representing the stored value type.
   * Common names for primitive types (Byte, Int, String...)
   * Arbitrary custom names for structures and arrays...
 * Also keeps various flags/settings that can be used to control the displaying/formatting of the content.
 * Value is represented by something like a Variant.


## Dump Tree
Dump tree is a way how to describe whatever data structure in a human readable format separately from the structure data itself.

A dump tree consists of Dump Items, nodes representing either a primitive value or a sub-structure or array containing other nodes.

There can be many dump trees existing in parallel - each module can have its own for representing own data. A single module can provide even multiple independently generated/updated dump trees.

The dump tree can be created once as a one time snapshot and save to a file or something. Or it can be refreshed periodically and shown using some kind of browser UI in real time.

The browser UI (if any) not just reads & displays the dump tree  create by the owning module but can also write some requests back to the DumpItems. This can be used to:
 - Affect the way how the dump is made (for example what part of a large array needs to be dumped)
 - Affect the original values in the dumping modules (write-back)
 - Trigger an action (defined by the dumping module) associated with the dump item



## Refreshing the dump tree
The dumping module holds full responsibility of what DumpItems it creates and how they are filled with data.

The dump tree is constructed by the owning module during the very first dump pass. Once constructed, the tree is retained in memory. Later, during successive dump passes, it is updated to reflect the most recent state of the data.

Of course the updating of an already built item tree structure  is only possible if the structure of the data remains same throughout the time and just the values are changing. Should the data structure change, the dump tree needs to be re-built.

The structure is also considered changed (needs rebuild) if some of the tree nodes changes its DumpItem type (for example from a primitive value to a structure of subitems...)

The dumping operation should happen when it is safe to access the data being dumped. The dumping module is fully responsible for locking its data when performing the dump.


## First time intialization
A dump item is typically initialized just once when first time constructed. Some fields (that are considered constant during the lifetime of the dump item) are set just once during the first dump pass and left touched during successive dump passes.

These include:
 - Name of the item
 - DumpItem type (primitive, structure, array etc.)
 - Type Name

To save CPU cycles, the dumping code usually does not update these fields once the Dump Item has already been initialized.



## Dump Item Types

List of typical types of Dump Items

### Primitive
 * Used to represent the values of simple-typed variables, struc/class member fields etc.
 * Holds a primitive value (like int, string, vector etc.) in an embedded Variant variable.
 * TypeName string corresponds to the value type stored in the Variant field.
 * Usually having no subitems.

### Structure
 * Used to represent a struct-like data types
 * Does not hold a value, instead keeps one or more subitems
 * TypeName string is usually set to the struct/class type name
 * The number, type and order of subitems is expected to stay constant during the lifetime of the DumpItem

### Array
 * Represents an array of values
 * A root for various number of subitems
 * The subitems are usually of the same type (but do not need to be)
 * The number of items in the original array is stored in ArraySize field
 * Not necessarily as many subitems as there are items in the array.
   * Just a subset of array items (an index range specified by ShowIndexFrom, ShowIndexTo) can be dumped as sub-items.
   * The ShowIndexFrom and ShowIndexTo are set from outside (from the browser UI), telling the

