using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public struct BoundaryPoint
    {
        #region Properties
        public Node node;
        public int offset;
        #endregion

        #region Constructors
        public BoundaryPoint(Node node, int offset)
        {
            this.node = node;
            this.offset = offset;
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is BoundaryPoint other)
            {
                return ReferenceEquals(node, other.node) && offset == other.offset;
            }

            return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BoundaryPoint A, BoundaryPoint B)
        {
            return ReferenceEquals(A.node, B.node) && A.offset == B.offset;
        }

        public static bool operator !=(BoundaryPoint A, BoundaryPoint B)
        {
            return !ReferenceEquals(A.node, B.node) || A.offset != B.offset;
        }
    }
}
