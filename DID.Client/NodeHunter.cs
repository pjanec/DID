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
	/// Locates the node identified by NodeIdent in the NodeRegistry.
	/// Provides the Node reference.
	/// Makes sure the node is kept connected etc.
	/// Keeps subscription to (expanded) topics and dump items to keep them published by the server.
	/// 
	///  - Získává Nodu z NodeRegistry podle NodeIdent, tím ji vytváří zvyšuje její RefCount (Alloc)
	///  - Unrefne Nodu pro svém Dispose (Dealloc)
	///  - Informuje, zda je nakonektěná (online/offline).
	///  - Spravuje subscription na DumpItemy a na Topicy
	///    - Aspoň jedna žádost => řekni nodě, že DI/Topic chceme
	///    - Žádná žádost => řekni nodě, že DI/Topic nechceme
	///    - Dispose proxy => zruš všechny žádosti za tuto proxy
	/// </summary>

	public class NodeHunter : Disposable
	{
		NodeRegistry nreg;
		Node node;
		public NodeIdent nodeIdent { get { return node.nodeIdent; } }
		
		// DumpItem => refCount
		class DISubsInfo
		{
			public int RefCount;
		};
		Dictionary<IDumpItem, DISubsInfo> diSubs = new Dictionary<IDumpItem, DISubsInfo>();

		// Topic => refCount
		class TopicSubsInfo
		{
			public int RefCount;
		};
		Dictionary<ITopic, TopicSubsInfo> topicSubs = new Dictionary<ITopic, TopicSubsInfo>();
		
		public NodeHunter( NodeRegistry nreg, NodeIdent ni )
		{
			this.nreg = nreg;
			// tell the node we need it (this should connect if not yet connected)
			node = nreg.Alloc( ni );
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( node != null )
				{
					// tell the node we don't need it
					node.Unref();
					node = null;
				}
			}
			base.Dispose( disposing );
		}

		public Node Node { get { return node; } }


		// DumpItem subscriptions

		public void Publish( IDumpItem di )
		{
			DISubsInfo si;
			if( !diSubs.TryGetValue(di, out si) )
			{
				si = new DISubsInfo();
				diSubs[di] = si;

				// let the node know we need this DI to be published
				node.Publish( di );
			}

			si.RefCount++;
		}

		public void Unpublish( IDumpItem di )
		{
			DISubsInfo si;
			if( diSubs.TryGetValue(di, out si) )
			{
				si.RefCount--;
				if( si.RefCount <= 0 )
				{
					diSubs.Remove(di);

					// let the node forget about this DI to be published
					node.Unpublish( di );
				}
			}
		}

		// Topic subscriptions

		public void Publish( ITopic topic )
		{
			TopicSubsInfo si;
			if( !topicSubs.TryGetValue(topic, out si) )
			{
				si = new TopicSubsInfo();
				topicSubs[topic] = si;

				// let the node know we need this DI to be published
				node.Publish( topic );
			}

			si.RefCount++;
		}

		public void Unpublish( ITopic topic )
		{
			TopicSubsInfo si;
			if( topicSubs.TryGetValue(topic, out si) )
			{
				si.RefCount--;
				if( si.RefCount <= 0 )
				{
					topicSubs.Remove(topic);

					// let the node forget about this DI to be published
					node.Unpublish( topic );
				}
			}
		}


	}
}
