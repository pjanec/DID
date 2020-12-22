using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DID.Common;

namespace DID.Client
{
	/// <summary>
	/// Represents the server node;
	/// Keeps the connection to a server node, reconnects when disconnected.
	/// Provides the topics
	/// </summary>
	public class Node : Disposable
	{
		NodeRegistry nreg;
		public NodeIdent nodeIdent { get; private set; }
		//RemoteDiag.ServerDesc sd;
		Client rdClient;
		DataRepo drepo;
		int refCount; // 0=no one needs a node
		

		class TopicRec
		{
			public ITopic topic;
			//public TopicRenderer renderer;
		}

		List<TopicRec> topicRecs = new List<TopicRec>();

		public event Action<ITopic> TopicAdded;
		public event Action<ITopic> TopicRemoved;
		
		// DumpItem => refCount
		class DISubsInfo
		{
			public int RefCount;
			public bool Expanded;
		};
		Dictionary<IDumpItem, DISubsInfo> diSubs = new Dictionary<IDumpItem, DISubsInfo>();

		// Topic => refCount
		class TopicSubsInfo
		{
			public int RefCount;
		};
		Dictionary<ITopic, TopicSubsInfo> topicSubs = new Dictionary<ITopic, TopicSubsInfo>();
		

		public Node( NodeRegistry nreg, NodeIdent ni )
		{
			this.rdClient = new Client();
			this.nreg = nreg;
			this.nodeIdent = ni;
			// FIXME!
		}

		//public Node( NodeRegistry nreg, RemoteDiag.ServerDesc sd )
		//	: this( nreg, NodeIdent.FromServerDesc( sd ) )
		//{
		//	this.sd = sd;
		//}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				Disconnect();
				rdClient.Dispose();
			}
			base.Dispose( disposing );
		}

		public void Tick()
		{
			TickConnect();
			TickDataRepo();
			TickSubscr();
		}

		void TickDataRepo()
		{
			if( !IsConnected )
				return;

			// rdClient does not tick data repo  on its own - we do
			if( drepo != null )
			{
				drepo.Flush();
				drepo.Poll();
			}
		}

		bool wasConnected = false;

		void TickConnect()
		{
			// check for connection/disconnection
			if( wasConnected && !IsConnected )
			{
				OnDisconnected();
				wasConnected = false;
			}
			else
			if( !wasConnected && IsConnected )
			{
				OnConnected();
				wasConnected = true;
			}

			// if not connected, try to re-connect
			if( !IsConnected && !IsConnecting)
			{
				Reconnect();
			}
		}

		void OnConnected()
		{
			drepo = rdClient.GetDataRepo();

			drepo.OnTopicAdded += OnTopicAdded;
			drepo.OnTopicRemoved += OnTopicRemoved;

			// add already existing topics
			foreach( var t in drepo.Topics )
			{
				OnTopicAdded( t );
			}
			
		}

		void OnDisconnected()
		{
			if( drepo != null )
			{
				drepo.OnTopicAdded -= OnTopicAdded;
				drepo.OnTopicRemoved -= OnTopicRemoved;
				drepo = null;
			}

			topicRecs.Clear();
		}

		void OnTopicAdded( ITopic t )
		{
			var topicRec = new TopicRec() { topic = t };
			topicRecs.Add( topicRec );
			TopicAdded?.Invoke( t );
		}

		void OnTopicRemoved( ITopic t )
		{
			TopicRemoved?.Invoke( t );
			var tr = topicRecs.FirstOrDefault( x => x.topic == t );
			topicRecs.Remove( tr );
		}

		public IEnumerable<ITopic> Topics
		{
			get
			{
				return from x in topicRecs select x.topic;	
			}
		}

		volatile Task _connectTask; // once not null, check IsComplete to find if still connecting

		public bool IsConnecting
		{
			get{ return (_connectTask != null && !_connectTask.IsCompleted ); }
		}

		public bool IsConnected
		{
			get
			{
				if( IsConnecting )
					return false;

				return rdClient.IsConnected();
			}
		}

		void StartConnecting( ServerDesc sd )
		{
			_connectTask = Task.Run( () => rdClient.Connect( sd.IpAddress, sd.Port, 2.0 ) );
		}

		void Disconnect()
		{
			// wait for potential connect to finish
			if(	_connectTask != null )
				_connectTask.Wait();

			// disconnect
			rdClient.Disconnect();

			// clear topics
			foreach( var tr in topicRecs )
			{
				TopicRemoved?.Invoke( tr.topic );
			}

			topicRecs.Clear();

			TopicAdded = null;
			TopicRemoved = null;
		}

		void Reconnect()
		{
			var sd = nreg.FindInScanned( nodeIdent );
			if( sd != null )
			{
				// start connecting
				StartConnecting(sd); // async, non-blocking
			}
		}

		public DataRepo DataRepo	{ get { return drepo; } }

		public bool Wanted { get { return refCount > 0; }} 

		public int AddRef()
		{
			refCount++;

			// Start connecting asynchronously
			//  - first check if in scan result
			//  - if so, try to connect
			//  - else just wait for next scan

			if( !IsConnected && !IsConnecting )
			{
				Reconnect();
			}

			return refCount;
		}

		public int Unref()
		{
			refCount--;

			// no one needs us, disconnect!
			if( refCount == 0 )
			{
				Disconnect();
			}

			return refCount;
		}

		// DumpItem subscriptions
		// The di.Expand field controls whether thr server publishes the item or not
		// Set it to true only if at least one subscriber is subscribed.

		public void Publish( IDumpItem di )
		{
			DISubsInfo si;
			if( !diSubs.TryGetValue(di, out si) )
			{
				si = new DISubsInfo();
				diSubs[di] = si;


				//// make the server start publishing the DI
				//di.Expand = true;
			}

			si.Expanded = true;
			di.Expand = si.Expanded;

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
					//diSubs.Remove(di);
					si.Expanded = false;
					di.Expand = si.Expanded;

					//// make the server stop publishing this DI
					//di.Expand = false;
				}
			}
		}

		// Topic subscriptions
		// The topic.RootItem.Expand field controls whether thr server publishes the item or not
		// Set it to true only if at least one subscriber is subscribed.

		public void Publish( ITopic topic )
		{
			TopicSubsInfo si;
			if( !topicSubs.TryGetValue(topic, out si) )
			{
				si = new TopicSubsInfo();
				topicSubs[topic] = si;

				// make the server start publishing the topic
				topic.RootDI.Expand = true;
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

					// make the server stop publishing this DI
					topic.RootDI.Expand = false;
				}
			}
		}

		void TickSubscr()
		{
			// set di.Expand for all registered DumpItems
			// this is necessary to finally set correct value as sometimes
			//  our request made when clicking the expand triangle is overwritten
			//  by the DataRepo update or something
			// FIXME: remove the unexpanded item after some time when no new overwrite
			// is likely to happen
			foreach( var kv in diSubs )
			{
				var di = kv.Key;
				var si = kv.Value;
				
				di.Expand = si.Expanded;
			}

		}
	}
}
