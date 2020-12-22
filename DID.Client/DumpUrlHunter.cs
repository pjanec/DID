using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DID.Common;

namespace DID.Client
{
	/// <summary>
	/// Locates the Dump Item identified by DumpUrl.
	/// Provides the reference to the DI. Null if not found.
	/// Makes sure the DI is kept published by the server as long as the DumpUrlHunter instance exists.
	/// You need to call Tick() peridically to update the DI reference (will change to null if lost).
	/// </summary>

	public class DumpUrlHunter : Disposable
	{
		public NodeHunter NodeHunter { get; private set; }
		public TopicHunter TopicHunter { get; private set; }
		public DIChainHunter DIHunter { get; private set; }
	
		public DumpUrlHunter( NodeRegistry nreg, DumpUrl dumpUrl )
		{
			NodeHunter = new NodeHunter( nreg, dumpUrl.NodeIdent );
			TopicHunter = new TopicHunter( NodeHunter, dumpUrl.TopicIdent );
			DIHunter = new DIChainHunter( TopicHunter, dumpUrl.DIChainIdent );
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				DIHunter.Dispose();
				TopicHunter.Dispose();
				NodeHunter.Dispose();
			}
			base.Dispose( disposing );
		}

		public void Tick()
		{
			//noHu.Tick();
			TopicHunter.Tick();
			DIHunter.Tick();
		}

	}
}
