using CssUI.DOM.Enums;
using CssUI.DOM.Events;

namespace CssUI.DOM.Nodes
{
    public interface INode : IEventTarget
    {
        ChildNodeList childNodes { get; }
        Node firstChild { get; }
        int index { get; }
        bool isAssigned { get; }
        bool isConnected { get; }
        Node lastChild { get; }
        Node nextSibling { get; }
        int nodeLength { get; }
        string nodeName { get; }
        ENodeType nodeType { get; }
        string nodeValue { get; set; }
        Document ownerDocument { get; }
        Element parentElement { get; }
        Node parentNode { get; }
        Node previousSibling { get; }
        string textContent { get; set; }

        Node appendChild(Node node);
        Node cloneNode(bool deep = false);
        EDocumentPosition compareDocumentPosition(Node other);
        bool contains(Node other);
        Node getRootNode(GetRootNodeOptions options = null);
        bool hasChildNodes();
        Node insertBefore(Node node, Node child);
        bool isEqualNode(Node otherNode);
        void normalize();
        Node removeChild(Node child);
        Node replaceChild(Node node, Node child);
    }
}
