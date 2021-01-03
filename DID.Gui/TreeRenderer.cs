using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using DID.Common;
using DID.Client;

namespace DID.Gui.DI
{
	// renders DI tree, allow to open editors for editable dump items

	class DIInfo
	{
		NodeHunter noHu;
		public IDumpItem di;
		public RngInfo rngInfo;
		public DI.Editor editor;
		public bool ShowKey = true;
		public string KeyOverride = null;

		// We can't control di.Expand directly as same dumpItem can be shown by multiple widgets.
		// We need to use node subscriptions instead to centralize the di.Expand control.
		bool _Expanded;
		public bool Expanded
		{
			get { return _Expanded; }
			set
			{
				if( _Expanded != value )
				{
					if( value )
					{
						noHu.Publish( di );
					}
					else
					{
						noHu.Unpublish( di );
					}
					_Expanded = value;
				}

				//// this makes the DI to be published by the server
				//di.Expand = value;
			}
		}

		public DIInfo( NodeHunter noHu, IDumpItem di )
		{
			this.noHu = noHu;
			this.di = di;
		}
	}

	class RngInfo
	{
		public bool Shown;
		public bool TailEnabled;
		public int TailShowFrom;
	}
	
	public class TreeRenderer : Disposable
	{
		Dictionary<IDumpItem, DIInfo> diInfos = new Dictionary<IDumpItem, DIInfo>();
		IDumpItem rootDI;
		DataRepo bm;
		NodeHunter noHu;

		public bool AllowTree = true; // just for the root DI; whether to show as expandable tree (or just show the root item)
		public bool AllowEdit = true; // for all DIs shown; show edit controls if the DI supports editing 
		public string KeyOverride = null; // just for the root DI; replaces the key part in "key=value" with something else
		public bool ShowKey = true; // just for the root DI; whether to display the "key=" part
		
		public TreeRenderer( NodeHunter noHu, IDumpItem rootDI )
		{
			this.noHu = noHu;
			this.bm = noHu.Node.DataRepo;
			this.rootDI = rootDI;

			// setup the root DI
			var diInfo = new DIInfo( noHu, rootDI );
			diInfo.ShowKey = ShowKey;
			diInfo.KeyOverride = KeyOverride;
			diInfos[rootDI] = diInfo;

		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if( disposing )
			{
			}
		}

		public void DrawUI()
		{
			// if root changes from null to something, start rendering it

			DrawDI( rootDI );
		}


		protected void DrawDI( IDumpItem di )
		{
			if( di == null )
			{
				ImGui.Text("---");
				return;
			}

			DIInfo diInfo = null;
			if( !diInfos.TryGetValue( di, out diInfo ) )
			{
				diInfo = new DIInfo( noHu, di );
				diInfos[di] = diInfo;

			}

			var t = di.Type;
			bool isTree = (di.Children.Count > 0) || (t != EDIType.Primitive);
			
			if( !AllowTree )
				isTree=false;

			// red color if dirty
			bool popColor = false;
			//if ( di.IsDirty )
			//{
			//	ImGui.PushStyleColor( ImGuiCol.Text,  new System.Numerics.Vector4( 1.0f, 0.4f, 0.4f, 1.0f ) );
			//	popColor = true;
			//}

			Action finalizeLine = () =>
			{
				if ( popColor )
				{
					ImGui.PopStyleColor();
				}
			};

			String s = DIFormatter.str1( di, diInfo.KeyOverride, diInfo.ShowKey );

			if ( isTree )
			{
				bool nodeOpened = ImGui.TreeNodeEx( s, diInfo.Expanded ? ImGuiTreeNodeFlags.DefaultOpen : 0 );
				diInfo.Expanded = nodeOpened;
				finalizeLine();

				if( di.Type == EDIType.Array )
					DrawArrayControls( diInfo );

				if( AllowEdit )
					DrawEditControls( diInfo );
				

				if ( nodeOpened )
				{
					// render subitems
					for ( int i = 0; i < di.Children.Count; i++ )
					{
						IDumpItem subi = di.Children[i];
						DrawDI( subi );
					}

					//// draw referenced dump items
					//if( t == EDIType.DumpItem )
					//{
					//	var refDI = di.DumpItemVal;
					//	if( refDI != null )
					//	{
					//		DITreeRenderer.DrawDI( refDI );
					//	}
					//}

					ImGui.TreePop();
				}
			}
			else
			{
				string indent = "   ";

				ImGui.TextWrapped( indent + s );

				finalizeLine();

				if( AllowEdit )
					DrawEditControls( diInfo );
			}



		}

		static System.Numerics.Vector4 colorBtnEnabled = new System.Numerics.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
		static System.Numerics.Vector4 colorBtnDisabled = new System.Numerics.Vector4( 0.8f, 0.8f, 0.8f, 1.0f );

		void DrawEditControls( DIInfo diInfo )
		{
			var di = diInfo.di;

			ImGui.PushID( di.GetHashCode() );

			if( di.Action != null )
			{
				ImGui.SameLine();

				// grayed if action request not yet processed
				ImGui.PushStyleColor( ImGuiCol.Text, di.FireAction ? colorBtnDisabled : colorBtnEnabled );
				var pressed = ImGui.Button("Act!");
				ImGui.PopStyleColor();

				if( pressed )
				{
					// rise the action request
					di.FireAction = true;

					//// do it using message - more reliable
					//var ev = new Bnet.DM.RemoteDiag.FireAction() { DI = di };
					//bm.SendEvent( ev );

				}
			}

			DI.Editor editor = diInfo.editor;

			// Edit button if no editor exists yet
			if( di.Writable && editor == null )
			{
				ImGui.SameLine();

				// grayed if write request not yet processed
				ImGui.PushStyleColor( ImGuiCol.Text, di.WriteBack ? colorBtnDisabled : colorBtnEnabled );
				var pressed = ImGui.Button("Edit");
				ImGui.PopStyleColor();

				if( pressed )
				{
					// add new editor
					editor = DI.EditorFactory.Create( di );
					if( editor != null )
					{
						diInfo.editor = editor;
					}
				}
			}

			// if there is an editor opened for this item, render it
			if( editor != null )
			{
				ImGui.SameLine();

				var finish = editor.DrawUI();

				if( finish == DI.Editor.EFinish.Confirmed )
				{
					// set write-back request flag
					di.WriteBack = true;

					// do it using message - more reliable
					//var ev = new Bnet.DM.RemoteDiag.WriteBack() { DI = di, Val_Primitive = editor.Value };
					//bm.SendEvent( ev );

					//// remove editor
					//diInfo.editor = null;
				}

				if( finish == DI.Editor.EFinish.Cancelled )
				{
					// remove editor
					diInfo.editor = null;
				}
			}

			ImGui.PopID();
		}

		void DrawArrayControls( DIInfo diInfo )
		{
			var di = diInfo.di;

			ImGui.PushID( di.GetHashCode() );

			RngInfo rngInfo = diInfo.rngInfo;
			if( rngInfo==null || !rngInfo.Shown )
			{
				ImGui.SameLine();
				if( ImGui.Button("rng") )
				{
					if( rngInfo == null )
					{
						rngInfo = new RngInfo();
						diInfo.rngInfo = rngInfo;	
					}
					rngInfo.Shown = true;
				}
			}

			if( rngInfo != null )
			{

				int showFrom = di.ArrayShowFrom;
				int showCount = System.Math.Min( System.Math.Max( 1, di.ArrayShowTo - di.ArrayShowFrom + 1 ), di.ArraySize );

				if( rngInfo.Shown )
				{
					ImGui.SameLine();
					if( ImGui.Button("[x]") )
					{
						rngInfo.Shown = false;
						//diInfo.rngInfo = null; 
						// warning: rnginfo never removed once opened - probably not a problem...
					}


					bool chg = false;
					float ww = ImGui.GetWindowWidth();
					ImGui.PushItemWidth( ww/10 );
					//ImGui.SameLine();
					if( ImGui.Button("Up") )
					{
						chg = true;
						showFrom -= showCount;
						if( showFrom < 0 ) showFrom = 0;
					}
					ImGui.SameLine();
					if( ImGui.Button("Down") )
					{
						chg = true;
						showFrom += showCount;
						if( showFrom > di.ArraySize )
							showFrom -= showCount;
					}
					ImGui.SameLine();
					chg |= ImGui.SliderInt( "From", ref showFrom, 0, di.ArraySize );
					ImGui.SameLine();
					chg |= ImGui.SliderInt( "Cnt", ref showCount, 0, System.Math.Max(di.ArraySize, 10) );
					ImGui.PopItemWidth();
					ImGui.SameLine();
					if( ImGui.Checkbox("Tail", ref rngInfo.TailEnabled ) )
					{
						// start showing tail on checking the tick box
						if( rngInfo.TailEnabled )
						{
							// initially show few lines from tail up
							rngInfo.TailShowFrom = System.Math.Max( 0, di.ArraySize - showCount/2 );
							di.ArrayShowFrom = rngInfo.TailShowFrom;
							di.ArrayShowTo = rngInfo.TailShowFrom + showCount - 1;
						}
					}

					if( chg )
					{
						di.ArrayShowFrom = showFrom;
						di.ArrayShowTo = showFrom + showCount - 1;
					}
				}

				if( rngInfo.TailEnabled )
				{
					if( di.ArraySize > rngInfo.TailShowFrom + showCount )
					{
						rngInfo.TailShowFrom = di.ArraySize-1;
						di.ArrayShowFrom = rngInfo.TailShowFrom;
						di.ArrayShowTo = rngInfo.TailShowFrom + showCount - 1;
					}
				}
			}

			ImGui.PopID();
		}
	}
}
