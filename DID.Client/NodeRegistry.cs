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
	/// Stores all nodes that has been found by scanning or no longer found but still connected.
	/// Together with Node completely hides the RdClient
	/// Nodes that are not referenced by anyone are disconnected and removed.
	///  - Automaticky skenuje síť.
	///  - Poskytuje výsedek posledního skenu
	///  - Udržuje seznam Nodes, o které byl v poslední době zájem (indexovaný podle nodeIdent)
	///  - Udržuje se připojená na nody, o které je zájem.
	///    - Když je zájem, vytvoří objekt nody a započne s pokusy o připojení.
	///    - Když není zájem, odpojí se a smaže.
	///  - Spravuje publishování DumpItemů/Topiců (Expand atribut) podle subscriptions od Proxy
	///    - Nikdo nechce => Expand=false
	///    - Aspoň někdo chce => Expand=true
	/// </summary>

	public class NodeRegistry : Disposable
	{
		/// <summary>
		/// Nodes that are wanted by someone
		/// </summary>
		Dictionary<NodeIdent, Node> _nodes = new Dictionary<NodeIdent, Node>();
	
		List<ServerDesc> lastScanResult = new List<ServerDesc>();
		
		public List<ServerDesc> ScanResults	{ get {	return lastScanResult; }}


		volatile Task<List<ServerDesc>> _scanTask; // once not null, check IsComplete to find if still scanning

		const double scanPeriod = 2.0; // how often to scan (must be greater than scanDuration!)
		const double scanDuration = 1.0; // how long to wait for incoming responses
		Stopwatch scanTimer = new Stopwatch();
		
		public NodeRegistry()
		{
			scanTimer.Start();	
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				// wait for scan to finish (is this necessary?)
				if( _scanTask != null && !_scanTask.IsCompleted )
					_scanTask.Wait();
			
			}
			base.Dispose( disposing );
		}

		bool scanFinished = true;	// blocks starting new scan

		// Scan the network for new nodes.
		void StartScan()
		{
			scanFinished = false;

			_scanTask = Task.Run( () =>
			{
				var rdClient = new Client();
				var serversFound = rdClient.Scan( scanDuration );
				rdClient.Dispose();
				return serversFound;
			});
		}

		public void Tick()
		{
			TickScan();
			TickNodes();
		}

		void TickNodes()
		{
			foreach( var kv in _nodes )
			{
				kv.Value.Tick();
			}
		}

		void TickScan()
		{
			if( scanFinished )
			{
				// check for next scan interval
				if( _scanTask == null || _scanTask.IsCompleted )
				{
					if( scanTimer.Elapsed.TotalSeconds > scanPeriod )
					{
						StartScan();
					}
				}
			}
			else // scan not yet finished
			{
				// check if scan task has completed
				if( _scanTask != null && _scanTask.IsCompleted )
				{
					scanFinished = true;

					ProcessScanResults( _scanTask.Result );

					// wait for another scan
					scanTimer.Restart();
				}
			}
		}

		// Adds new (not yet connected) nodes, removes the no longer reported ones (if disconnected)
		void ProcessScanResults( List<ServerDesc> serversFound )
		{
			// remember last scan result
			lastScanResult = serversFound;

			//// create not yet registered ones
			var serverDict = new Dictionary<NodeIdent, int>();
			foreach( var sd in serversFound )
			{
				var ni = NodeIdent.FromServerDesc( sd );
				serverDict[ni] = 1;	// remember server existence
			
			//	if( !_nodes.ContainsKey( ni ) )
			//	{
			//		_nodes[ni] = new Node( sd );
			//	}
			}

			// remove no longer wanted and no longer found by scan
			foreach( var node in _nodes.Values.ToList() )  // iterate a copy so that we can remove from orig dict
			{
				var ni = node.nodeIdent;

				if( !serverDict.ContainsKey( ni ) )	 // not found by scan
				{
					if( !node.Wanted )
					{
						// forget the node
						_nodes.Remove( ni );
						node.Dispose();
					}
				}
			}
		}

		// scan and check if given NodeIdent was found
		public ServerDesc FindInScanned( NodeIdent ni )
		{
			// try to find our node indent among scan result
			var sd = (from x in ScanResults where ni.Match(x) select x).FirstOrDefault();
			return sd;
		}

		// creates node or just increment its refcount
		public Node Alloc( NodeIdent ni )
		{
			Node node;
			if( !_nodes.TryGetValue( ni, out node ) )
			{
				node = new Node( this, ni );
				_nodes[ni] = node;
			}

			node.AddRef();

			return node;
		}

		// unref node and dispose if no references left
		public void Dealloc( NodeIdent ni )
		{
			Node node;
			if( !_nodes.TryGetValue( ni, out node ) )
			{
				return;
			}

			if( node.Unref() <= 0 )
			{
				_nodes.Remove( ni );

				node.Dispose();
			}
		}



		/// <summary>
		/// Finds node by its identifier
		/// </summary>
		/// <param name="ni"></param>
		/// <returns></returns>
		public Node Find( NodeIdent ni )
		{
			Node res;
			if( _nodes.TryGetValue( ni, out res ) )
			{
				return res;
			}
			return null;
		}

	}
}
