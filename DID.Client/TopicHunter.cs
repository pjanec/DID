using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DID.Common;

namespace DID.Client
{
	/// <summary>
	/// Locates the topic identified by TopicName string.
	/// Provides the reference to topic's RootDI. Null if not found.
	/// Makes sure the topic is kept published by the server as long as the TopicHunter instance exists.
	/// You need to call Tick() peridically to update the RootDI (will change to null if lost).
	/// </summary>

	public class TopicHunter : Disposable
	{
		TopicIdent topicIdent;
		NodeHunter noHu;

		public NodeHunter NodeHunter { get { return noHu; } }
		public ITopic Topic { get; private set; }
		public IDumpItem RootDI { get; private set; }

		public TopicHunter( NodeHunter noHu, TopicIdent topicIdent )
		{
			this.noHu = noHu;
			this.topicIdent = topicIdent;
			// try to look up
			Tick();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				// unpublish rootDI
				if( RootDI != null )
				{
					noHu.Unpublish( RootDI );
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Looks for the topic or checks if it is still available.
		/// </summary>
		bool Look( out ITopic topic, out IDumpItem rootDI )
		{
			// topics having same type name
			var topics = (from t in noHu.Node.Topics where string.Compare(t.TypeName, topicIdent.TypeName, StringComparison.OrdinalIgnoreCase)==0 select t);
			foreach( var t in topics )
			{
				var rdi = t.RootDI;
				if( rdi != null && string.Compare(rdi.Name, topicIdent.InstName, StringComparison.OrdinalIgnoreCase )==0 )
				{
					topic = t;
					rootDI = rdi;
					return true;
				}
			}
			// topic or RootDi not found..
			topic = null;
			rootDI = null;
			return false;
		}

		public void Tick()
		{
			ITopic topic;
			IDumpItem rootDI;
				
			if( RootDI == null ) // try to look up the topic & its root item
			{
				if( Look( out topic, out rootDI ) )
				{
					noHu.Publish( rootDI );
					Topic = topic;
					RootDI = rootDI;
				}
			}
			else // just checking if topic still exists
			{
				if( !Look( out topic, out rootDI ) )
				{
					noHu.Unpublish( RootDI );
					RootDI = null;
					Topic = null;
				}
			}
		}

	}
}
