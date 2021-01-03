using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DID.Common
{
	public class DIFormatter
	{
		public static String PrimitiveToStr( Variant pr )
		{
			switch( pr.ValType )
			{
				case Variant.EValType.Invalid:
					return "<Invalid>";

				case Variant.EValType.Int:
					return $"{pr.IntValue}";

				case Variant.EValType.Double:
					return $"{pr.DoubleValue}";

				case Variant.EValType.String:
					return $"{pr.StringValue}";

				default:
					return $"<Unknown Value Type> {pr.ValType}";
			}
		}

		/// <summary>
		/// Returns the "key = " part of the usual "key = value" textual representation of the dump item
		/// </summary>
		/// <param name="includeSeparator">include '=' or space after the key </param>
		public static String KeyToStr( IDumpItem di, bool includeSeparator=true )
		{
			string s = "";

			if ( di.ArrayIndex >= 0 )
			{
				s += $"[{di.ArrayIndex}]";
				if( includeSeparator ) s+=" ";
			}

			if ( !String.IsNullOrEmpty(di.Name) )
			{
				s += di.Name;
				if( includeSeparator ) s += " = ";
			}

			return s;
		}

		/// <summary>
		/// Returns the "value" part of the usual "key = value" textual representation of the dump item
		/// </summary>
		public static String ValToStr( IDumpItem di )
		{
			var pr = di.Value;

			switch( di.Type )
			{
				case EDIType.Invalid:
				{
					return $"Invalid";
				}

				case EDIType.Primitive:
				{
					return PrimitiveToStr( pr );
				}

				case EDIType.Enum:
				{
					return $"Enum {di.TypeName} {pr.IntValue}";
				}

				case EDIType.Struct:
				{
					if( pr.ValType == Variant.EValType.Invalid )
					{
						return $"Struct {di.TypeName}";
					}
					else // structure with a value => probably some artifical folded subtree (not an ordinary data struct header)
					{
						return PrimitiveToStr( pr );
					}
				}
		
				case EDIType.Array:
				{
					return $"Array ({di.ArraySize})";
				}

				default:
					return "<invalid>";

			}
		}

		public static string str1( IDumpItem di, string keyOverride=null, bool showKey=true )
		{
			var key = "";
			if( !string.IsNullOrEmpty(keyOverride) )
			{
				key = keyOverride + " = ";
			}
			else
			if( showKey )
			{
				key = DIFormatter.KeyToStr( di );
			}

			var value = DIFormatter.ValToStr( di );

			var s = $"{key}{value}";

			return s;
		}

	}
}
