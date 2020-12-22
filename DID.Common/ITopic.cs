using System;
using System.Collections.Generic;
using System.Text;

namespace DID.Common
{
	public interface ITopic
	{
		public string TypeName  { get { return string.Empty; } set {} }
		public string InstName  { get { return string.Empty; } set {} }
		public IDumpItem RootDI { get { return null; } set {} }
	}
}
