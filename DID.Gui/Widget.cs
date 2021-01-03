using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using DID.Common;

namespace DID.Gui
{
	public class Widget : Disposable
	{
		public bool HasOwnWindow { get; set; }
		public string Title { get; set; }
		public string UniqueUiId = Guid.NewGuid().ToString();
		public string TitleInternal { get { return (Title == null ? "<no title>"+ "##" + UniqueUiId : Title); } } 

		public Widget()
		{
		}

		public virtual void Tick()
		{
		}

		public virtual void DrawUI()
		{
			// to be overridden
			// .. here draw your ImGui content
		}

		public void BringToFront()
		{
			ImGui.SetWindowFocus( TitleInternal );
		}
	}
}
