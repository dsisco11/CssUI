using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class Comment : CharacterData
    {
        #region Node Overrides
        public override ENodeType nodeType => ENodeType.COMMENT_NODE;
        public override string nodeName => "#comment";
        #endregion

        #region Constructor
        public Comment(Document ownerDocument, string data = "") : base(ownerDocument, data)
        {
        }
        #endregion

        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Comment B))
                return false;

            return 0 == string.Compare(this.data, B.data);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 31 + (int)data.GetHashCode();
            return hash;
        }
    }
}
