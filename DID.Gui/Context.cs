using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using DID.Client;

namespace DID.Gui
{
	public class Context
	{
		public NodeRegistry Nodes = new NodeRegistry();

		public List<Widget> Widgets = new List<Widget>();

		// singleton instance keeper
		private static Context instance=null;
    
		private Context()
		{
		}

		public void Dispose()
		{
			// dispose widgets
			foreach( var w in Widgets )
			{
				w.Dispose();
			}
		}


		public static Context Instance
		{
			get
			{
				if (instance==null)
				{
					instance = new Context();
				}
				return instance;
			}
		}

		
		List<Widget> widgetsToAdd = new	List<Widget>();
		List<Widget> widgetsToRemove = new List<Widget>();

		// call this from DrawUI
		public void AddWidget( Widget w )
		{
			widgetsToAdd.Add( w );
		}

		public void RemoveWidgetDispose( Widget w )
		{
			widgetsToRemove.Add( w );
		}

		void LateAddWidgets()
		{
			foreach( var w in widgetsToAdd )
			{
				Widgets.Add( w );
			}
			widgetsToAdd.Clear();
		}

		void LateRemoveWidgets()
		{
			foreach( var w in widgetsToRemove )
			{
				Widgets.Remove( w );
				w.Dispose();
			}
			widgetsToRemove.Clear();
		}

		void TickWidgets()
		{
			foreach( var w in Widgets )
			{
				w.Tick();
			}
		}

		public void Tick()
		{
			Nodes.Tick();
			LateRemoveWidgets();
			LateAddWidgets();
			TickWidgets();
		}

		void DrawWindowlessWidgets()
		{
			// draw those NOT having own window
			foreach( var w in Widgets )
			{
				if( !w.HasOwnWindow )
				{
					w.DrawUI();
				}
			}
		}

		void DrawWindowedWidgets()
		{
			// draw those having own window
			foreach( var w in Widgets )
			{
				if( w.HasOwnWindow )
				{
					ImGuiWindowFlags wndFlags = 0; //ImGuiWindowFlags.ChildWindow;
					bool opened = true;
					if( ImGui.Begin( w.TitleInternal, ref opened, wndFlags ) )
					{
						w.DrawUI();
						ImGui.End();
					}
					if( !opened )
					{
						RemoveWidgetDispose( w );
					}
				}
			}
		}

		public void DrawUI()
		{
			DrawWindowlessWidgets();
			DrawWindowedWidgets();
		}


	}
}
