using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Client
{
	public class DIChainIdent  : IEquatable<DIChainIdent>
	{
		public List<string> Names; // names of the chain segments from top to bottom

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DIChainIdent);
        }

        public bool Equals(DIChainIdent p)
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
            return Names.SequenceEqual(p.Names);
        }

        public override int GetHashCode()
        {
            int hc = 0;
            foreach( var n in Names )
            {
                hc ^= n.GetHashCode();
            }
            return hc;
        }

        public static bool operator ==(DIChainIdent lhs, DIChainIdent rhs)
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

        public static bool operator !=(DIChainIdent lhs, DIChainIdent rhs)
        {
            return !(lhs == rhs);
        }

		public static DIChainIdent Parse( string s )
		{
            List<string> res = new List<string>();
			if( !string.IsNullOrEmpty( s ) )
            {
                var segm = s.Split( new char[] {':'} );
                res = segm.ToList();
            }
            return new DIChainIdent() { Names = res };
		}

		public override string ToString()
		{
			return string.Join(":", Names);
		}

	}
}
