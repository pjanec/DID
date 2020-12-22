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
	/// Locates the last link of the DIChain identified by DIChainIdent.
	/// Provides the reference to the last link's DI. Null if not found.
	/// Makes sure all the DIs in the chain are kept published by the server as long as the DIChainHunter instance exists.
	/// You need to call Tick() peridically to update the DI
	/// </summary>

	public class DIChainHunter : Disposable
	{
		DIChainIdent diIdent;
		TopicHunter toHu;

		public TopicHunter TopicHunter { get { return toHu; } }

		/// <summary>
		/// Found links; as short as the number of links found.
		/// If same size as the number of links in the DIChainIdent, then all links were resolved 
		/// </summary>
		public List<IDumpItem> LinkDIs { get; private set; }

		/// <summary>
		/// The last DI in the DIChain.
		/// Can be non-null only if all previous DI in the chain have also been succesfully resolved.
		/// </summary>
		public IDumpItem LeafDI
		{
			get
			{
				if( diIdent.Names.Count == 0 || LinkDIs.Count < diIdent.Names.Count ) 
				{
					return null;
				}
				else
				{
					return LinkDIs[	diIdent.Names.Count-1 ];
				}
			}
		}

		public DIChainHunter( TopicHunter toHu, DIChainIdent diIdent )
		{
			this.toHu = toHu;
			this.diIdent = diIdent;
			this.LinkDIs = new List<IDumpItem>();
			
			// try to look up
			Tick();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				// unpublish the DIs
				foreach( var di in LinkDIs )
				{
					if( di != null )
					{
						toHu.NodeHunter.Unpublish( di );
					}
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Looks for the DIs or checks if it is still available.
		/// </summary>
		/// <param name="foundDIs">list of link DI objects found.
		/// If shorter than the total number of links, then not all were found...
		/// If same size as the total number of links, the last item is the final DI.
		/// </param>
		void Look( out List<IDumpItem> foundDIs )
		{
			IDumpItem parent = toHu.RootDI;
			int linkIdx = 0;
			foundDIs = new List<IDumpItem>();
			while( parent != null && linkIdx < diIdent.Names.Count )
			{
				// look up the DI within the parent's child list
				string linkName = diIdent.Names[linkIdx];
				IDumpItem linkDI = null;
				foreach( var childDI in parent.Children )
				{
					if( childDI != null && string.Compare( childDI.Name, linkName, StringComparison.OrdinalIgnoreCase )==0 )
					{
						linkDI = childDI;
						break;
					}
				}
				// add the found link to the list, make it the new parent
				if( linkDI != null )
				{
					foundDIs.Add( linkDI );
					linkIdx++;
					parent = linkDI;
				}
				else
				{
					break;
				}
			}
		}

		public void Tick()
		{
			List<IDumpItem> foundDIs;

			Look( out foundDIs );

			// publish all NEW links found
			for( int i=LinkDIs.Count; i < foundDIs.Count; i++ )
			{
				toHu.NodeHunter.Publish( foundDIs[i] );
			}

			// unpublish all the lost links
			for( int i=foundDIs.Count; i < LinkDIs.Count; i++ )
			{
				toHu.NodeHunter.Unpublish( LinkDIs[i] );
			}

			// remember the links found (warning - might be incomplete - not all links found...)
			LinkDIs = foundDIs;
		}

	}
}
