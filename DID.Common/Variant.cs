using System;

namespace DID.Common
{
	// just a simplest temporary implementation to be replaced later with something better
	public class Variant
	{
		public enum EValType { Invalid=0, Int, Double };
		public EValType ValType;
		public Int64 IntValue;
		public double DoubleValue;
		public object AnyValue;	
	}
}
