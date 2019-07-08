using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System.Text;

namespace CssUI.DOM
{
    public class DocumentFragment : Node
    {
        public Element Host { get; private set; } = null;

        #region Node Implementation
        public override ENodeType nodeType => ENodeType.DOCUMENT_FRAGMENT_NODE;
        public override string nodeName => "#document-fragment";
        public override string nodeValue { get => null; set { /* specs say do nothing */ } }
        public override string textContent
        {
            get
            {
                /* The textContent attribute’s getter must return the following, switching on context object: The descendant text content of the context object. */
                /* The descendant text content of a node node is the concatenation of the data of all the Text node descendants of node, in tree order. */
                var tree = new TreeWalker(this, ENodeFilterMask.SHOW_TEXT);
                StringBuilder sb = new StringBuilder();
                Node n;
                while ((n = tree.firstChild()) != null)
                    sb.Append(n.textContent);

                return sb.ToString();
            }
            set
            {
                /* To string replace all with a string string within a node parent, run these steps: */
                /* 1) Let node be null. */
                Node node = null;
                /* 2) If string is not the empty string, then set node to a new Text node whose data is string and node document is parent’s node document. */
                if (!string.IsNullOrEmpty(value))
                    node = new Text(parentNode.ownerDocument, value);
                /* 3) Replace all with node within parent. */
                Node._replace_all_within_node(node, parentNode);
            }
        }
        #endregion

        public override int nodeLength => this.childNodes.Count;

        public DocumentFragment(Element Host)
        {
            this.Host = Host;
        }
    }
}
