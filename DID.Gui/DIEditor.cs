using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using DID.Common;

namespace DID.Gui.DI
{
	public static class EditorFactory
	{
		public static Editor Create( IDumpItem di )
		{
			switch( di.Value.ValType )
			{
				case Variant.EValType.Int:
					return new IntEditor( di );

				case Variant.EValType.String:
					return new StringEditor( di );

				case Variant.EValType.Double:
				default:
					return null;
			}

		}

	}

	// Renders editing fields for a concrete dump item.
	// If confirmed, the value edited is written back to the DumpItem.
	public class Editor
	{
		protected IDumpItem di;

		public Editor( IDumpItem di )
		{
			this.di = di;
		}

		public virtual EFinish DrawUI()
		{
			return EFinish.None;
		}

		
		public enum EFinish
		{
			None,
			Confirmed,
			Cancelled
		}

		//public event System.Action<EFinish> Finished;

		//protected void FireFinished( EFinish finish )
		//{
		//	Finished?.Invoke( finish );
		//}

	}

	// [OK] and [Cancel] buttons finishes editing
	// If confirmed, value is written back to DI
	public class IntEditor : Editor
	{
		string input;
		Int64 value;

		public IntEditor( IDumpItem di )
		 : base( di )
		{
			input = di.Value.IntValue.ToString();
		}

		public override EFinish DrawUI()
		{
			var finish = EFinish.None;

			ImGui.InputText( "", ref input, 40 );

			ImGui.SameLine();

			if( ImGui.Button("OK") )
			{
				// convert value to int
				if( Int64.TryParse( input, out value ) )
				{
					di.Value.IntValue = value;
					finish = EFinish.Confirmed;
				}
			}

			ImGui.SameLine();

			if( ImGui.Button("Cancel") )
			{
				finish = EFinish.Cancelled;
			}

			return finish;
		}

	}

	public class StringEditor : Editor
	{
		string input;

		public StringEditor( IDumpItem di )
		 : base( di )
		{
			input = di.Value.StringValue;
		}

		public override EFinish DrawUI()
		{
			var finish = EFinish.None;

			float ww = ImGui.GetWindowWidth();
			ImGui.PushItemWidth( ww/3);
			ImGui.InputText( "", ref input, 8192 );
			ImGui.PopItemWidth();

			ImGui.SameLine();

			if( ImGui.Button("OK") )
			{
				di.Value.StringValue = input;
				finish = EFinish.Confirmed;
			}

			ImGui.SameLine();

			if( ImGui.Button("Close") )
			{
				finish = EFinish.Cancelled;
			}

			return finish;
		}

	}

}
