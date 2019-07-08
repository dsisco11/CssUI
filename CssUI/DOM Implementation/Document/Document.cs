using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using xLog;

namespace CssUI.DOM
{
    public class Document : Node, IGlobalEventHandlers, IDocumentAndElementEventHandlers
    {
        internal ILogger Log = LogFactory.GetLogger(nameof(Document));

        #region Node Implementation
        public override ENodeType nodeType => ENodeType.DOCUMENT_NODE;
        public override string nodeName => "#document";
        public override string nodeValue { get => null; set { /* specs say do nothing */ } }
        public override string textContent { get => null; set { /* specs say do nothing */ } }

        public override int nodeLength => this.childNodes.Count;

        public override Document ownerDocument { get => null; }
        #endregion

        #region DOM tree accessors
        getter object (DOMString name);
        /* dir indicates the text direction of this document, but we determine this elsewhere in CssUI */
        // [CEReactions] attribute DOMString dir;
        /// <summary>
        /// The root CssUI element
        /// </summary>
        /* The body element of a document is the first of the html element's children that is either a body element or a frameset element, or null if there is no such element. */
        public HTMLElement body { get; private set; }
        IList<Node> getElementsByName(string elementName);
        #endregion
        
        #region user interaction
        public readonly Window defaultView;
        /// <summary>
        /// Returns the deepest element in the document through which or to which key events are being routed. This is, roughly speaking, the focused element in the document.
        /// </summary>
        public Element activeElement { get; private set; }

        /// <summary>
        /// Returns true if key events are being routed through or to the document; otherwise, returns false. Roughly speaking, this corresponds to the document, or a document nested inside this one, being focused.
        /// </summary>
        /// <returns></returns>
        public bool hasFocus();
        [CEReactions] attribute string designMode;
        // [CEReactions] boolean execCommand(string commandId, optional boolean showUI = false, optional DOMString value = "");
        bool queryCommandEnabled(string commandId);
        bool queryCommandIndeterm(string commandId);
        bool queryCommandState(string commandId);
        bool queryCommandSupported(string commandId);
        string queryCommandValue(string commandId);
        #endregion



        public Node adoptNode(Node node)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-adoptnode */
            if (node is Document) throw new NotSupportedError();
            /* NOTE: Shadow DOM stuff here */

            /* Docs: https://dom.spec.whatwg.org/#concept-node-adopt */
            /* 1) Let oldDocument be node’s node document. */
            var oldDocument = node.ownerDocument;
            /* 2) If node’s parent is not null, remove node from its parent. */
            if (!ReferenceEquals(node.parentNode, null))
                _remove_node_from_parent(node.parentNode, node);
            /* 3) If document is not oldDocument, then: */
            if (!ReferenceEquals(this, oldDocument))
            {
                var descendentsList = DOMCommon.Get_Shadow_Including_Inclusive_Descendents(node);
                /* 1) For each inclusiveDescendant in node’s shadow-including inclusive descendants: */
                /* NOTE: Shadow DOM stuff here */
                foreach (Node inclusiveDescendant in descendentsList)
                {
                    /* 1) Set inclusiveDescendant’s node document to document. */
                    inclusiveDescendant.ownerDocument = this;
                    /* 2) If inclusiveDescendant is an element, then set the node document of each attribute in inclusiveDescendant’s attribute list to document. */
                    if (inclusiveDescendant is Element element)
                    {
                        foreach (var attr in element.AttributeList)
                        {
                            attr.ownerDocument = this;
                        }
                    }
                }
                /* 2) For each inclusiveDescendant in node’s shadow-including inclusive descendants that is custom, enqueue a custom element callback reaction with inclusiveDescendant, callback name "adoptedCallback", and an argument list containing oldDocument and document. */
                /* NOTE: Custom element stuff here */
                /* 3) For each inclusiveDescendant in node’s shadow-including inclusive descendants, in shadow-including tree order, run the adopting steps with inclusiveDescendant and oldDocument. */
                foreach (Node inclusiveDescendant in descendentsList)
                {
                    this.adoptNode(inclusiveDescendant);
                }
            }

            return node;
        }
    }
}
