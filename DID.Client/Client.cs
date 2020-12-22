using System;
using System.Collections.Generic;
using DID.Common;

namespace DID.Client
{
	/// <summary>
	/// Connects to a server, retrieving the published topics and thier content
	/// </summary>
	public class Client : Disposable
	{
		public bool Connect( string ipAddress, int port, double timeout )
		{
			return false;
		}

		public void Disconnect()
		{
		}

		public bool IsConnected()
		{
			return false;
		}
	
		public DataRepo GetDataRepo()
		{
			return null;
		}

		public List<ServerDesc> Scan( double timeout )
		{
			var list = new List<ServerDesc>();
			return list;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Disconnect();
			}
			base.Dispose(disposing);
		}
	}

}
