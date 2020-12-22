using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DID.Common
{
public static partial class Dmp
{
	public interface IDumpable
	{
		void Dump( DumpItem d );
	};


	public static DumpItem KV<T>( DumpItem d, string name, T x ) where T:IDumpable
	{
		var subDI = d.GetOrCreateSubitem( name );
		x.Dump( subDI );
		return subDI;
	}

	public static DumpItem KV<T>( DumpItem d, string name, ref T x ) where T:IDumpable
	{
		var subDI = d.GetOrCreateSubitem( name );
		x.Dump( subDI );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Int64 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Int64 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var wb = subDI.DumpInt( ref x );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, bool x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = x ? 1 : 0;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref bool x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = x ? 1 : 0;
		var wb = subDI.DumpInt( ref tmp );
		x = tmp!=0;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, byte x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64) x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref byte x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = x;
		var wb = subDI.DumpInt( ref tmp );
		x = (byte) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, char x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64) x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref char x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = x;
		var wb = subDI.DumpInt( ref tmp );
		x = (char) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Int16 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = (Int64)x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Int16 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64)x;
		var wb = subDI.DumpInt( ref tmp );
		x = (Int16) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, UInt16 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = (Int64)x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref UInt16 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64)x;
		var wb = subDI.DumpInt( ref tmp );
		x = (UInt16) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Int32 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = (Int64)x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Int32 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64)x;
		var wb = subDI.DumpInt( ref tmp );
		x = (Int32) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, UInt32 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = (Int64)x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref UInt32 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64)x;
		var wb = subDI.DumpInt( ref tmp );
		x = (UInt32) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, UInt64 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		Int64 tmp = (Int64)x;
		subDI.DumpInt( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref UInt64 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (Int64)x;
		var wb = subDI.DumpInt( ref tmp );
		x = (UInt64) tmp;
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, double x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = x;
		subDI.DumpDouble( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref double x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var wb = subDI.DumpDouble( ref x );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, float x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (double)x;
		subDI.DumpDouble( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref float x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = (double)x;
		var wb = subDI.DumpDouble( ref tmp );
		x = (float) tmp;
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, string x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		subDI.DumpString( x );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref string x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var wb = subDI.DumpString( ref x );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Vector4 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		subDI.DumpVector4( x );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Vector4 x )
	{
		var subDI = d.GetOrCreateSubitem( name );

		var wb = subDI.DumpVector4( ref x );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Vector3 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Vector4(x.X, x.Y, x.Z, 0);
		subDI.DumpVector4( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Vector3 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Vector4(x.X, x.Y, x.Z, 0);
		var wb = subDI.DumpVector4( ref tmp );
		x = new Vector3( tmp.X, tmp.Y, tmp.Z );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, Vector2 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Vector4(x.X, x.Y, 0, 0);
		subDI.DumpVector4( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Vector2 x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Vector4(x.X, x.Y, 0, 0);
		var wb = subDI.DumpVector4( ref tmp );
		x = new Vector2( tmp.X, tmp.Y );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}


	public static DumpItem KV( DumpItem d, string name, Quaternion x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Quaternion(x.X, x.Y, x.Z, x.W);
		subDI.DumpQuaternion( tmp );
		return subDI;
	}

	public static DumpItem KV( DumpItem d, string name, ref Quaternion x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		var tmp = new Quaternion(x.X, x.Y, x.Z, x.W);
		var wb = subDI.DumpQuaternion( ref tmp );
		x = new Quaternion( tmp.X, tmp.Y, tmp.Z, tmp.W );
		if(wb) subDI.OnWriteBackAction?.Invoke();
		return subDI;
	}

	public static void Struct( DumpItem d, string name )
	{
		d.SetupHeader( EDIType.Struct, name );
	}

	public static void Array( DumpItem d, int count, out int a_from, out int a_to, string typeName="Array" )
	{
		d.SetupHeader( EDIType.Array, typeName );

		d.ArraySize = count;

		int a_size = ( int ) count;
		a_from = System.Math.Min( d.ArrayShowFrom, a_size-1 );
		if( a_from < 0 ) a_from = 0;
		a_to = d.ArrayShowTo < 0 ? a_size-1 : System.Math.Min( d.ArrayShowTo, a_size-1 );
	}

	public static void DumpList<T>( DumpItem d, IList<T> x ) where T:IDumpable
	{
		int a_from, a_to;	
		Array(d, x.Count, out a_from, out a_to, "Array of "+typeof(T).Name );

		for( int i=a_from; i <= a_to; i++ )
		{
			KV( d, String.Empty, x[i] ).ArrayIndex = i;
		}

		d.RemoveExtraSubitems();
	}

	public static DumpItem KV<T>( DumpItem d, string name, IList<T> x ) where T:IDumpable
	{
		var subDI = d.GetOrCreateSubitem( name );
		DumpList( subDI, x );
		return subDI;
	}


	#region list dumpers for various basic types
	public static void DumpList( DumpItem d, IList<int> x )
	{
		int a_from, a_to;	
		Array(d, x.Count, out a_from, out a_to );

		for( int i=a_from; i <= a_to; i++ )
		{
			KV( d, String.Empty, x[i] ).ArrayIndex = i;
		}

		d.RemoveExtraSubitems();
	}

	public static DumpItem KV( DumpItem d, string name, IList<int> x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		DumpList( subDI, x );
		return subDI;
	}

	public static void DumpList( DumpItem d, IList<string> x )
	{
		int a_from, a_to;	
		Array(d, x.Count, out a_from, out a_to );

		for( int i=a_from; i <= a_to; i++ )
		{
			KV( d, String.Empty, x[i] ).ArrayIndex = i;
		}

		d.RemoveExtraSubitems();
	}

	public static DumpItem KV( DumpItem d, string name, IList<string> x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		DumpList( subDI, x );
		return subDI;
	}

	public static void DumpList( DumpItem d, IList<Vector3> x )
	{
		int a_from, a_to;	
		Array(d, x.Count, out a_from, out a_to );

		for( int i=a_from; i <= a_to; i++ )
		{
			KV( d, String.Empty, x[i] ).ArrayIndex = i;
		}

		d.RemoveExtraSubitems();
	}

	public static DumpItem KV( DumpItem d, string name, IList<Vector3> x )
	{
		var subDI = d.GetOrCreateSubitem( name );
		DumpList( subDI, x );
		return subDI;
	}

	#endregion

	}

	/*
		public class MyClass : Dmp.IDumpable
		{
			int xInt = 7;
			double xDouble = -5.2;

			public void Dump( Dmp.DumpItem d )
			{
				Dmp.Struct(d, "MyClass");
				Dmp.KV( d, "xInt", xInt );
				Dmp.KV( d, "xDouble", xDouble );
			}
		}

		public class AnotherClass : Dmp.IDumpable
		{
			int xInt = 7;
			double xDouble = -5.2;
			MyClass myClass = new MyClass();

			public void Dump( Dmp.DumpItem d )
			{
				Dmp.Struct(d, "AnotherClass");
				Dmp.KV( d, "xInt", xInt );
				Dmp.KV( d, "xDouble", xDouble );
				Dmp.KV( d, "myClass", myClass );
			}
		}



		class Program
		{
			static void Main(string[] args)
			{
				AnotherClass myAC = new AnotherClass();
				var d = new Dmp.DumpItem();
				//MyClass myC = new MyClass();
				//myC.Dump(d);
				Dmp.Struct( d, "Test1" );
				Dmp.KV( d, "myAC", myAC );


				Console.WriteLine("Hello World!");
			}
		}

	*/
}
