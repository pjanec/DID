using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using DID.Common;
using DID.Client;

namespace DID.Gui
{
	/// <summary>
	/// Shows DI's value
	/// </summary>
	public class WatchItemWidget : Widget
	{
		public bool ShowKey = true; // show "name = value"? false=show just the value
		public string KeyOverride = null; // null = use name from the DI
		public bool AllowTree = true; // if true, fully fledged DI tree renderer is used

		DumpUrlHunter urlHunter;
		DI.TreeRenderer treeRend;

		public WatchItemWidget( DumpUrl url )
		{
			urlHunter = new DumpUrlHunter( Context.Instance.Nodes, url );
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if( disposing )
			{
				urlHunter.Dispose();
			}
		}

		public override void Tick()
		{
			base.Tick();
			urlHunter.Tick();
		}

		public override void DrawUI()
		{
			var di = urlHunter.DIHunter.LeafDI;

			// create/destroy tree renderer id desired
			if( treeRend==null && di!=null )
			{
				treeRend = new DI.TreeRenderer( urlHunter.NodeHunter, di );
				treeRend.AllowTree = AllowTree;
				treeRend.KeyOverride = KeyOverride;
			}
			if( treeRend !=null && di==null )
			{
				treeRend.Dispose();
				treeRend = null;
			}

			// render
			if( di == null )
			{
				ImGui.TextColored( new System.Numerics.Vector4( 1, 0, 0, 1 ), "---" );
			}
			else
			{
				treeRend.DrawUI();
			}
		}
	}
}
