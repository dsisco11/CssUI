using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using xLog;

namespace CssUI.DOM
{
    public class Document : ParentNode, IGlobalEventCallbacks, IDocumentAndElementEventCallbacks
    {/* Docs: https://dom.spec.whatwg.org/#document */

        #region Properties
        internal ILogger Log = LogFactory.GetLogger(nameof(Document));
        internal Window window;
        public readonly string Origin = null;
        public readonly DocumentType doctype;
        public readonly string contentType;
        public readonly DOMImplementation implementation = new DOMImplementation();
        public Element documentElement { get; private set; }
        #endregion

        #region Node Implementation
        public override ENodeType nodeType => ENodeType.DOCUMENT_NODE;
        public override string nodeName => "#document";
        public override string nodeValue { get => null; set { /* specs say do nothing */ } }
        public override string textContent { get => null; set { /* specs say do nothing */ } }

        public override int nodeLength => this.childNodes.Count;

        public readonly new Document ownerDocument;
        #endregion

        #region Constructor
        protected Document(DocumentType doctype, string contentType = null, string origin = null)
        {
            this.doctype = doctype;
            this.contentType = contentType;
            this.Origin = origin;
        }
        #endregion



        #region DOM tree accessors
        /* dir indicates the text direction of this document, but we determine this elsewhere in CssUI */
        // [CEReactions] attribute DOMString dir;

        /// <summary>
        /// The root CssUI element
        /// </summary>
        /* The body element of a document is the first of the html element's children that is either a body element or a frameset element, or null if there is no such element. */
        public HTMLElement body { get; }


        /// <summary>
        /// If qualifiedName is "*" returns a collection of all descendant elements.
        /// Otherwise, returns a collection of all descendant elements whose qualified name is qualifiedName. (Matches case-insensitively against elements in the HTML namespace within an HTML document.)
        /// </summary>
        /// <param name="qualifiedName"></param>
        public IEnumerable<Element> getElementsByTagName(string qualifiedName)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-getelementsbytagname */
            /* When invoked with the same argument, the same HTMLCollection object may be returned as returned by an earlier call. */
            return DOMCommon.Get_Elements_By_Qualified_Name(this, qualifiedName);
        }

        /// <summary>
        /// If namespace and localName are "*" returns a collection of all descendant elements.
        /// If only namespace is "*" returns a collection of all descendant elements whose local name is localName.
        /// If only localName is "*" returns a collection of all descendant elements whose namespace is namespace.
        /// Otherwise, returns a collection of all descendant elements whose namespace is namespace and local name is localName.
        /// </summary>
        /// <param name="Namespace"></param>
        /// <param name="localName"></param>
        public IEnumerable<Element> getElementsByTagNameNS(string Namespace, string localName)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-getelementsbytagnamens */
            return DOMCommon.Get_Elements_By_Namespace_And_Local_Name(this, Namespace, localName);
        }

        /// <summary>
        /// Returns a collection of the elements in the object on which the method was invoked (a document or an element) that have all the classes given by classNames. 
        /// The classNames argument is interpreted as a space-separated list of classes.
        /// </summary>
        /// <param name="classNames"></param>
        public IEnumerable<Element> getElementsByClassName(string classNames)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-getelementsbyclassname */
            return DOMCommon.Get_Elements_By_Class_Name(this, classNames);
        }
        #endregion

        #region Element Creation

        /// <summary>
        /// Returns an element with localName as local name (if document is an HTML document, localName gets lowercased). 
        /// The element’s namespace is the HTML namespace when document is an HTML document or document’s content type is "application/xhtml+xml", and null otherwise.
        /// If localName does not match the Name production an "InvalidCharacterError" DOMException will be thrown.
        /// When supplied, options’s is can be used to create a customized built-in element.
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Element createElement(string localName, ElementCreationOptions options)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-createelement */
            if (!XMLCommon.Is_Valid_Name(localName))
                throw new InvalidCharacterError($"localName \"{localName}\" is not a valid XML identifier");

            if (this is HTMLDocument)
                localName = localName.ToLowerInvariant();

            string Namespace = null;
            if (this is HTMLDocument || this.contentType == "application/xhtml+xml")
                Namespace = DOMCommon.HTMLNamespace;

            return DOMCommon.Create_Element(this, localName, Namespace);
        }

        /// <summary>
        /// Returns an element with namespace namespace. Its namespace prefix will be everything before ":" (U+003E) in qualifiedName or null. Its local name will be everything after ":" (U+003E) in qualifiedName or qualifiedName.
        /// If localName does not match the Name production an "InvalidCharacterError" DOMException will be thrown.
        /// If one of the following conditions is true a "NamespaceError" DOMException will be thrown:
        /// localName does not match the QName production.
        /// Namespace prefix is not null and namespace is the empty string.
        /// Namespace prefix is "xml" and namespace is not the XML namespace.
        /// qualifiedName or namespace prefix is "xmlns" and namespace is not the XMLNS namespace.
        /// namespace is the XMLNS namespace and neither qualifiedName nor namespace prefix is "xmlns".
        /// When supplied, options’s is can be used to create a customized built-in element.
        /// </summary>
        /// <param name="Namespace"></param>
        /// <param name="qualifiedName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Element createElementNS(string Namespace, string qualifiedName, ElementCreationOptions options)
        {/* Docs: https://dom.spec.whatwg.org/#dom-document-createelementns */
            return DOMCommon.createElementNS(this, qualifiedName, Namespace);
        }

        /// <summary>
        /// Returns a DocumentFragment node.
        /// </summary>
        /// <returns></returns>
        public DocumentFragment createDocumentFragment()
        {
            return new DocumentFragment(null, this);
        }

        /// <summary>
        /// Returns a Text node whose data is data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Text createTextNode(string data)
        {
            return new Text(this, data);
        }

        /// <summary>
        /// Returns a CDATASection node whose data is data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CDATASection createCDATASection(string data)
        {
            if (this is HTMLDocument)
                throw new NotSupportedError("CDATA nodes are not supported by HTML.");

            if (data.Contains("]]"))
                throw new InvalidCharacterError($"Data strings for {nameof(CDATASection)} nodes cannot contain \"]]\"");

            return new CDATASection(this, data);
        }

        /// <summary>
        /// Returns a Comment node whose data is data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Comment createComment(string data)
        {
            return new Comment(this, data);
        }

        /// <summary>
        /// Returns a ProcessingInstruction node whose target is target and data is data. 
        /// If target does not match the Name production an "InvalidCharacterError" DOMException will be thrown. 
        /// If data contains "?>" an "InvalidCharacterError" DOMException will be thrown.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ProcessingInstruction createProcessingInstruction(string target, string data)
        {
            if (!XMLCommon.Is_Valid_Name(target))
                throw new InvalidCharacterError($"Target: \"{target}\" is not a valid XML identifier!");

            if (data.Contains("?>"))
                throw new InvalidCharacterError($"The data for {nameof(ProcessingInstruction)} nodes cannot contain the sequence: \"?>\"");

            return new ProcessingInstruction(this, target, data);
        }

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

        // [CEReactions] boolean execCommand(string commandId, optional boolean showUI = false, optional DOMString value = "");
        /*bool queryCommandEnabled(string commandId);
        bool queryCommandIndeterm(string commandId);
        bool queryCommandState(string commandId);
        bool queryCommandSupported(string commandId);
        string queryCommandValue(string commandId);*/
        #endregion

        /// <summary>
        /// Returns a new <see cref="Range"/> object for the document.
        /// </summary>
        /// <returns></returns>
        public Range createRange()
        {
            return new Range(this);
        }

        public override EventTarget get_the_parent(Event @event)
        {
            /* A document’s get the parent algorithm, given an event, returns null if event’s type attribute value is "load" or document does not have a browsing context, and the document’s relevant global object otherwise. */
            /* Note: We arent a browser implementation so we will never have a browsing context, knowing this I think we should direguard the check and allow Document to return itsself except for when event is "load" */
            //if (0 == string.Compare(@event.type, "load"))
            if (@event.type == EEventName.Load)
                return null;

            return this;
        }

        #region Node importing
        /// <summary>
        /// Returns a copy of node. If deep is true, the copy also includes the node’s descendants.
        /// If node is a document or a shadow root, throws a "NotSupportedError" DOMException.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public Node importNode(Node node, bool deep = false)
        {
            if (node is Document)
                throw new NotSupportedError($"A document node cannot be imported into another document node.");

            if (node is ShadowRoot)
                throw new NotSupportedError($"A shadow root node may not be imported into a document node.");

            return node.cloneNode(deep);
        }

        /// <summary>
        /// Moves node from another document and returns it.
        /// If node is a document, throws a "NotSupportedError" DOMException or, if node is a shadow root, throws a "HierarchyRequestError" DOMException.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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
        #endregion


        #region Document Event Handlers
        public event EventCallback onCopy
        {
            add => handlerMap.Add(EEventName.Copy, value);
            remove => handlerMap.Remove(EEventName.Copy, value);
        }

        public event EventCallback onCut
        {
            add => handlerMap.Add(EEventName.Cut, value);
            remove => handlerMap.Remove(EEventName.Cut, value);
        }

        public event EventCallback onPaste
        {
            add => handlerMap.Add(EEventName.Paste, value);
            remove => handlerMap.Remove(EEventName.Paste, value);
        }
        #endregion
    }
}
