
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public abstract class AbstractRange
    {/* Docs: https://dom.spec.whatwg.org/#interface-abstractrange */
        #region Properties
        public Node startContainer { get; set; }
        public int startOffset { get; set; }
        public Node endContainer { get; set; }
        public int endOffset { get; set; }
        public bool collapsed { get => (ReferenceEquals(startContainer, endContainer) && startOffset == endOffset); }
        #endregion
    }
}
