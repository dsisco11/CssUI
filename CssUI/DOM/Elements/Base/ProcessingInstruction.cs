using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class ProcessingInstruction : CharacterData
    {
        public string target { get; internal set; }

        #region Node Overrides
        public override ENodeType nodeType => ENodeType.PROCESSING_INSTRUCTION_NODE;
        public override string nodeName => this.target;
        #endregion

        #region Constructor
        public ProcessingInstruction(Document ownerDocument, string target, string data) : base(ownerDocument, data)
        {
            this.target = target;
        }
        #endregion


        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is ProcessingInstruction B))
                return false;

            return 0 == string.Compare(this.target, B.target) && 0 == string.Compare(this.data, B.data);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 31 + target.GetHashCode();
            hash = hash * 31 + data.GetHashCode();
            return hash;
        }
    }
}
