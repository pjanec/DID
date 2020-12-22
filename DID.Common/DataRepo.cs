using System;
using System.Collections.Generic;
using System.Text;

namespace DID.Common
{
	public class DataRepo
	{
		public void Flush() {}
		public void Poll() {}
		public event Action<ITopic> OnTopicAdded;
		public event Action<ITopic> OnTopicRemoved;
		public IEnumerable<ITopic> Topics { get; set; }
	}
}
