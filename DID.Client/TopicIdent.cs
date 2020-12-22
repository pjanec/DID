using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Client
{
	public class TopicIdent  : IEquatable<TopicIdent>
	{
		public string TypeName; // multi-segment dot-separated name of the topic
		public string InstName; // the name of topic's RootDI

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TopicIdent);
        }

        public bool Equals(TopicIdent p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return
                string.Compare(TypeName, p.TypeName, StringComparison.OrdinalIgnoreCase)==0 &&
                string.Compare(InstName, p.InstName, StringComparison.OrdinalIgnoreCase)==0;
        }

        public override int GetHashCode()
        {
            return TypeName.GetHashCode() ^ InstName.GetHashCode();
        }

        public static bool operator ==(TopicIdent lhs, TopicIdent rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TopicIdent lhs, TopicIdent rhs)
        {
            return !(lhs == rhs);
        }

		public static TopicIdent Parse( string s, out string rest )
		{
			var segm = s.Split( new char[] {':'}, 3 );
			if( segm.Length >= 2 )
			{
				try{
                    rest = segm.Length > 2 ? segm[2] : string.Empty;
					return new TopicIdent() { TypeName = segm[0], InstName=segm[1] };		
				}
				catch( FormatException ex ) // int parsing problem
				{
				}
			}
            rest = s;
			return null;
		}

		public static TopicIdent Parse( string s )
		{
            string rest;
            return Parse( s, out rest );
		}

		public override string ToString()
		{
			return $"{TypeName}:{InstName}";
		}

	}
}
