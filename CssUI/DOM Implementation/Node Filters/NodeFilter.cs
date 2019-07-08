using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public abstract class NodeFilter
    {
        public abstract ENodeFilterResult acceptNode(Node node);
    }
}
