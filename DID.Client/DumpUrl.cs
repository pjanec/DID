using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DID.Client
{
	public class DumpUrl  : IEquatable<DumpUrl>
	{
		public NodeIdent NodeIdent;
        public TopicIdent TopicIdent;
        public DIChainIdent DIChainIdent;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NodeIdent);
        }

        public bool Equals(DumpUrl p)
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
            return (NodeIdent == p.NodeIdent) && (TopicIdent == p.TopicIdent) && (DIChainIdent == p.DIChainIdent);
        }

        public override int GetHashCode()
        {
            return NodeIdent.GetHashCode() ^ TopicIdent.GetHashCode() ^ DIChainIdent.GetHashCode();
        }

        public static bool operator ==(DumpUrl lhs, DumpUrl rhs)
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

        public static bool operator !=(DumpUrl lhs, DumpUrl rhs)
        {
            return !(lhs == rhs);
        }

		public static DumpUrl Parse( string s )
		{
            var nodeIdent = NodeIdent.Parse( s, out s );
            var topicIdent = nodeIdent!=null ? TopicIdent.Parse( s, out s ) : null;
            var diChainIdent = topicIdent!=null ? DIChainIdent.Parse( s ) : null;

			return new DumpUrl()
            {
                NodeIdent = nodeIdent,
                TopicIdent = topicIdent,
                DIChainIdent = diChainIdent
            };		
		}

		public override string ToString()
		{
			return $"{NodeIdent}:{TopicIdent}:{DIChainIdent}";
		}

	}
}
