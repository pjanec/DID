using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DID.Common;

namespace DID.Client
{
	/// <summary>
	/// Unique "Name" of the sever node
	/// </summary>
	public class NodeIdent  : IEquatable<NodeIdent>
	{
		public string NodeType;
		public string NodeName;
		public int NodeId;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NodeIdent);
        }

        public bool Equals(NodeIdent p)
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
                string.Compare(NodeType, p.NodeType, StringComparison.OrdinalIgnoreCase)==0 &&
                string.Compare(NodeName, p.NodeName, StringComparison.OrdinalIgnoreCase)==0 &&
                (NodeId == p.NodeId);
        }

        public override int GetHashCode()
        {
            return NodeType.GetHashCode() ^ NodeName.GetHashCode() ^ NodeId.GetHashCode();
        }

        public static bool operator ==(NodeIdent lhs, NodeIdent rhs)
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

        public static bool operator !=(NodeIdent lhs, NodeIdent rhs)
        {
            return !(lhs == rhs);
        }

		public static NodeIdent Parse( string s, out string rest )
		{
			var segm = s.Split( new char[] {':'}, 4 );
			if( segm.Length >= 3 )
			{
				try{
                    rest = segm.Length > 3 ? segm[3] : string.Empty;
					return new NodeIdent() { NodeType = segm[0], NodeName=segm[1], NodeId= int.Parse(segm[2]) };		
				}
				catch( FormatException ex ) // int parsing problem
				{
				}
			}
            rest = s;
			return null;
		}

		public static NodeIdent Parse( string s )
		{
            string rest;
            return Parse( s, out rest );
		}

		public override string ToString()
		{
			return $"{NodeType}:{NodeName}:{NodeId}";
		}

		public static NodeIdent FromServerDesc( ServerDesc sd )
		{
			return new NodeIdent() { NodeType=sd.NodeType, NodeName=sd.NodeName, NodeId=sd.NodeId };
		}

		public bool Match( ServerDesc sd )
		{
			return (
                string.Compare( sd.NodeType, NodeType, StringComparison.OrdinalIgnoreCase )==0 &&
				string.Compare( sd.NodeName, NodeName, StringComparison.OrdinalIgnoreCase )==0 &&
				sd.NodeId == NodeId );
		}


	}
}
