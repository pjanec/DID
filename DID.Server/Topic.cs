using System;
using DID.Common;

namespace DID.Server
{
	/// <summary>
	/// Defines the dump parameters	and keeps the full implementation of a root dump item
	/// </summary>
	public class Topic : ITopic
	{
		public string DumpGroupName;
		public double DumpPeriod;
		public Action DumpFunc;
		// ...
	}

}
