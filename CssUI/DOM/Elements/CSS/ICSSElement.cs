using CssUI.CSS;
using CssUI.CSS.BoxTree;

namespace CssUI.DOM.Nodes
{
    public interface ICssElement : INode
    {
        CssPrincipalBox Box { get; }
        StyleProperties Style { get; }
    }
}
