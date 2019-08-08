using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Internal;
using CssUI.DOM.Media;
using CssUI.DOM.Nodes;
using CssUI.HTML;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using xLog;

namespace CssUI.DOM
{
    public class Document : ParentNode, IGlobalEventCallbacks, IDocumentAndElementEventCallbacks
    {/* Docs: https://dom.spec.whatwg.org/#document */

        #region Backing Values
        private EDesignMode _designMode = EDesignMode.OFF;
        #endregion

        #region Rendering
        /// <summary>
        /// The top layer is an ordered set of elements, rendered in the order they appear in the set. The last element in the set is rendered last, and thus appears on top.
        /// </summary>
        /// Docs: https://fullscreen.spec.whatwg.org/#top-layer
        internal LinkedList<Node> topLayer = new LinkedList<Node>();
        #endregion

        #region Internal Properties
        internal ILogger Log = LogFactory.GetLogger(nameof(Document));

        /* XXX: This never fires because we dont have a system to notify the document its not active yet */
        internal readonly ManualResetEvent Active_State_Change_Signal = new ManualResetEvent(false);
        internal LinkedList<MediaQueryList> _mediaQueryLists = new LinkedList<MediaQueryList>();
        internal BrowsingContext BrowsingContext = null;

        [Obsolete("Use defaultView in stead", true)]
        internal Window window
        {/* https://html.spec.whatwg.org/multipage/window-object.html#dom-document-defaultview */
            get
            {
                return BrowsingContext?.WindowProxy;
            }
        }
        internal CssUnitResolver cssUnitResolver;

        /// <summary>
        /// Returns the initial contaiing block
        /// </summary>
        internal DOMRect Initial_Containing_Block
        {/* Docs: https://www.w3.org/TR/css-display-3/#initial-containing-block */
            get
            {
                return documentElement?.Box.Content.getBoundingClientRect();
            }
        }
        #endregion

        #region Properties
        public Window defaultView
        {/* https://html.spec.whatwg.org/multipage/window-object.html#dom-document-defaultview */
            get
            {
                return BrowsingContext?.WindowProxy;
            }
        }

        /// <summary>
        /// The area within which element layout is performed
        /// (The viewport used for layout)
        /// </summary>
        public readonly Viewport Viewport;

        public readonly EQuirksMode Mode = EQuirksMode.NoQuirks;
        public readonly string URL = "/";
        public readonly string Origin = null;

        public readonly DocumentType doctype;
        public readonly string contentType;
        public readonly DOMImplementation implementation = new DOMImplementation();

        public EDesignMode DesignMode
        {
            get => _designMode;
            set
            {
                var oldValue = _designMode;
                _designMode = value;

                /* When the designMode changes from being disabled to being enabled, 
                 * the user agent must immediately reset the document's active range's start and end boundary points to be at the start of the Document 
                 * and then run the focusing steps for the document element of the Document, if non-null. */
                if (value == EDesignMode.ON && oldValue == EDesignMode.OFF)
                {
                    Range activeRange = getSelection();
                    if (activeRange != null)
                    {
                        activeRange.collapse(true);
                    }

                    if (documentElement != null)
                    {
                        DOMCommon.Run_Focusing_Steps(documentElement);
                    }
                }
            }
        }
        /// <summary>
        /// Returns the Element that is the root element of the document (for example, the <html> element for HTML documents).
        /// </summary>
        public Element documentElement { get; private set; }

        /* XXX: */
        public LinkedList<HTMLImageElement> Images = new LinkedList<HTMLImageElement>();

        private ConcurrentDictionary<KeyCombination, Action> KeyCommands = new ConcurrentDictionary<KeyCombination, Action>();
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
            cssUnitResolver = new CssUnitResolver(this, true);
        }
        #endregion

        #region Event Loops
        internal void evaluate_media_queries_and_report_changes()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#evaluate-media-queries-and-report-changes */
            foreach(MediaQueryList mediaList in _mediaQueryLists)
            {
                mediaList.Evaluate();
            }
        }
        #endregion
        
        #region DOM tree accessors
        /* dir indicates the text direction of this document, but we determine this elsewhere in CssUI */
        // [CEReactions] attribute DOMString dir;

        /// <summary>
        /// The root CssUI element
        /// </summary>
        /* The body element of a document is the first of the html element's children that is either a body element or a frameset element, or null if there is no such element. */
        public HTMLElement body
        {
            get => DOMCommon.Get_Nth_Child_OfType<HTMLBodyElement>(documentElement, 1);
        }


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

        #region Internal States
        internal bool Is_FullyActive
        {/* Docs: https://html.spec.whatwg.org/multipage/#fully-active */
            get => (BrowsingContext != null) && ReferenceEquals(this, BrowsingContext.activeDocument);
        }
        #endregion

        #region Keyboard Commands
        /* XXX: KeyCommand processing on keyboard input */

        /// <summary>
        /// Returns true if the given <paramref name="command"/> was able to be registered to <paramref name="combo"/>
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Register_Key_Command(KeyCombination combo, Action command)
        {
            return KeyCommands.TryAdd(combo, command);
        }

        /// <summary>
        /// Returns true if the given key-combination <paramref name="combo"/> was able to be unegistered
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public bool Unregister_Key_Command(KeyCombination combo)
        {
            return KeyCommands.TryRemove(combo, out _);
        }
        #endregion

        #region User Interaction
        /// <summary>
        /// The term focusable area is used to refer to regions of the interface that can become the target of keyboard input. Focusable areas can be elements, parts of elements, or other regions managed by the user agent.
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/interaction.html#focused-area-of-the-document
        internal FocusableArea focusedArea = null;


        /// <summary>
        /// Returns the deepest element in the document through which or to which key events are being routed. This is, roughly speaking, the focused element in the document.
        /// </summary>
        public Element activeElement { get; private set; }

        /// <summary>
        /// Returns true if key events are being routed through or to the document; otherwise, returns false. Roughly speaking, this corresponds to the document, or a document nested inside this one, being focused.
        /// </summary>
        /// <returns></returns>
        public bool hasFocus()
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#has-focus-steps */
            var candidate = BrowsingContext.Get_Top_Level_Browsing_Context().activeDocument;
            while (true)
            {
                if (ReferenceEquals(this, candidate))
                {
                    return true;
                }
                else if (candidate.focusedArea is IBrowsingContextContainer contextContainer && contextContainer.Nested_Browsing_Context != null)
                {
                    candidate = contextContainer.Nested_Browsing_Context.activeDocument;
                }
                else
                {
                    return false;
                }
            }
        }

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
            if (node is ShadowRoot) throw new HierarchyRequestError("Cannot adopt a shadow-root into a document");

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
                foreach (Node inclusiveDescendant in descendentsList)
                {
                    /* 1) Set inclusiveDescendant’s node document to document. */
                    inclusiveDescendant.nodeDocument = this;
                    /* 2) If inclusiveDescendant is an element, then set the node document of each attribute in inclusiveDescendant’s attribute list to document. */
                    if (inclusiveDescendant is Element element)
                    {
                        foreach (var attr in element.AttributeList)
                        {
                            attr.nodeDocument = this;
                        }
                    }
                }

                /* 2) For each inclusiveDescendant in node’s shadow-including inclusive descendants that is custom, enqueue a custom element callback reaction with inclusiveDescendant, callback name "adoptedCallback", and an argument list containing oldDocument and document. */
                /* 3) For each inclusiveDescendant in node’s shadow-including inclusive descendants, in shadow-including tree order, run the adopting steps with inclusiveDescendant and oldDocument. */
                foreach (Node inclusiveDescendant in descendentsList)
                {
                    if (inclusiveDescendant is Element childElement)
                    {
                        CEReactions.Enqueue_Reaction(childElement, EReactionName.Adopted, oldDocument, this);
                    }
                    adoptNode(inclusiveDescendant);
                }
            }

            return node;
        }
        #endregion

        #region CSSOM Extensions
        public Element elementFromPoint(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-document-elementfrompoint */

            /* 1) If either argument is negative, x is greater than the viewport width excluding the size of a rendered scroll bar (if any), or y is greater than the viewport height excluding the size of a rendered scroll bar (if any), or there is no viewport associated with the document, return null and terminate these steps. */
            if (Viewport == null) return null;

            var vWidth = Viewport.Width - Viewport.ScrollBox?.VScrollBar.width ?? 0;
            var vHeight = Viewport.Height - Viewport.ScrollBox?.HScrollBar.height ?? 0;
            if (x < 0 || y < 0 || x > vWidth || y > vHeight)
            {
                return null;
            }

            /* 2) If there is a layout box in the viewport that would be a target for hit testing at coordinates x,y, when applying the transforms that apply to the descendants of the viewport, return the associated element and terminate these steps. */
            var filter = new FilterLayoutBoxHitTest(x, y);
            var hit = DOMCommon.Get_Nth_Descendant_OfType<Element>(this, 1, filter, ENodeFilterMask.SHOW_ELEMENT);
            if (hit != null)
            {
                return hit;
            }

            return documentElement;
        }

        public IReadOnlyCollection<Element> elementsFromPoint(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-document-elementsfrompoint */

            /* 1) If either argument is negative, x is greater than the viewport width excluding the size of a rendered scroll bar (if any), or y is greater than the viewport height excluding the size of a rendered scroll bar (if any), or there is no viewport associated with the document, return null and terminate these steps. */
            /*if (Viewport == null) return null;

            var vWidth = Viewport.Width - Viewport.ScrollBox?.VScrollBar.width ?? 0;
            var vHeight = Viewport.Height - Viewport.ScrollBox?.HScrollBar.height ?? 0;
            if (x < 0 || y < 0 || x > vWidth || y > vHeight)
            {
                return null;
            }

            var hits = new LinkedList<Element>();
            */
            /* XXX: Finish this sometime after CSS painting system is done */
            throw new NotImplementedException();
        }

        public CaretPosition caretPositionFromPoint(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-document-caretpositionfrompoint */

            /* 1) If there is no viewport associated with the document, return null. */
            /*if (Viewport == null) return null;
            *//* 2) If either argument is negative, x is greater than the viewport width excluding the size of a rendered scroll bar (if any), y is greater than the viewport height excluding the size of a rendered scroll bar (if any) return null. *//*
            var vWidth = Viewport.Width - Viewport.ScrollBox?.VScrollBar.width ?? 0;
            var vHeight = Viewport.Height - Viewport.ScrollBox?.HScrollBar.height ?? 0;
            if (x < 0 || y < 0 || x > vWidth || y > vHeight)
            {
                return null;
            }
            */

            /* XXX: Finish this sometime after CSS painting system is done */
            throw new NotImplementedException();
        }

        public Element scrollingElement
        {
            get
            {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-document-scrollingelement */
                if (Mode == EQuirksMode.Quirks)
                {
                    /* 1) If the HTML body element exists, and it is not potentially scrollable, return the HTML body element and abort these steps. */
                    if (body != null && !body.is_potentially_scrollable) return body;
                    /* 2) Return null and abort these steps. */
                    return null;
                }

                return documentElement;
            }
        }
        #endregion

        #region ID Map
        Dictionary<AtomicString, WeakReference<Element>> Element_ID_Map = new Dictionary<AtomicString, WeakReference<Element>>();

        public Element getElementByID(AtomicString id)
        {
            if (Element_ID_Map.TryGetValue(id, out WeakReference<Element> weakRef))
            {
                if (weakRef.TryGetTarget(out Element element))
                {
                    return element;
                }
            }

            return null;
        }

        internal void Update_Element_ID(Element element, AttributeValue oldValue, AttributeValue newValue)
        {
            AtomicString oldKey = oldValue.Get_Atomic();
            AtomicString newKey = newValue.Get_Atomic();
            WeakReference<Element> weakRef;

            if (oldKey.Equals(newKey)) return;

            if (Element_ID_Map.TryGetValue(oldKey, out weakRef))
            {
                Element_ID_Map.Remove(oldKey);
            }

            if (weakRef == null)
            {
                weakRef = new WeakReference<Element>(element);
            }

            if (!Element_ID_Map.ContainsKey(newKey))
            {
                Element_ID_Map.Add(newValue.Get_Atomic(), weakRef);
            }
            else
            {
                /* It appears there is already an element that has this ID, so we need to traverse the tree and get the first tree-order element with this ID, thats the true owner */
                NodeFilter idFilter = new FilterAttribute(EAttributeName.ID, newValue);
                var owner = (Element)DOMCommon.Get_Nth_Ancestor(documentElement, 1, idFilter, ENodeFilterMask.SHOW_ELEMENT);
                /* This is the canonical owner of this ID */
                weakRef.SetTarget(owner);
                Element_ID_Map[newKey] = weakRef;
            }
        }
        #endregion

        #region Event Loop
        public void Run_Event_Loop()
        {
            /* XXX: Event loop */
        }
        #endregion


        #region Document Events
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

        #region Window Events

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onAbort
        {
            add => handlerMap.Add(EEventName.Abort, value);
            remove => handlerMap.Remove(EEventName.Abort, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onAuxClick
        {
            add => handlerMap.Add(EEventName.AuxClick, value);
            remove => handlerMap.Remove(EEventName.AuxClick, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onBlur
        {
            add => handlerMap.Add(EEventName.Blur, value);
            remove => handlerMap.Remove(EEventName.Blur, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCancel
        {
            add => handlerMap.Add(EEventName.Cancel, value);
            remove => handlerMap.Remove(EEventName.Cancel, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCanPlay
        {
            add => handlerMap.Add(EEventName.CanPlay, value);
            remove => handlerMap.Remove(EEventName.CanPlay, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCanPlayThrough
        {
            add => handlerMap.Add(EEventName.CanPlayThrough, value);
            remove => handlerMap.Remove(EEventName.CanPlayThrough, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onChange
        {
            add => handlerMap.Add(EEventName.Change, value);
            remove => handlerMap.Remove(EEventName.Change, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onClick
        {
            add => handlerMap.Add(EEventName.Click, value);
            remove => handlerMap.Remove(EEventName.Click, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onClose
        {
            add => handlerMap.Add(EEventName.Close, value);
            remove => handlerMap.Remove(EEventName.Close, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onContextMenu
        {
            add => handlerMap.Add(EEventName.ContextMenu, value);
            remove => handlerMap.Remove(EEventName.ContextMenu, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onCueChange
        {
            add => handlerMap.Add(EEventName.CueChange, value);
            remove => handlerMap.Remove(EEventName.CueChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDblClick
        {
            add => handlerMap.Add(EEventName.DoubleClick, value);
            remove => handlerMap.Remove(EEventName.DoubleClick, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDrag
        {
            add => handlerMap.Add(EEventName.Drag, value);
            remove => handlerMap.Remove(EEventName.Drag, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragEnd
        {
            add => handlerMap.Add(EEventName.DragEnd, value);
            remove => handlerMap.Remove(EEventName.DragEnd, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragEnter
        {
            add => handlerMap.Add(EEventName.DragEnter, value);
            remove => handlerMap.Remove(EEventName.DragEnter, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragExit
        {
            add => handlerMap.Add(EEventName.DragExit, value);
            remove => handlerMap.Remove(EEventName.DragExit, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragLeave
        {
            add => handlerMap.Add(EEventName.DragLeave, value);
            remove => handlerMap.Remove(EEventName.DragLeave, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragOver
        {
            add => handlerMap.Add(EEventName.DragOver, value);
            remove => handlerMap.Remove(EEventName.DragOver, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDragStart
        {
            add => handlerMap.Add(EEventName.DragStart, value);
            remove => handlerMap.Remove(EEventName.DragStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDrop
        {
            add => handlerMap.Add(EEventName.Drop, value);
            remove => handlerMap.Remove(EEventName.Drop, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onDurationChange
        {
            add => handlerMap.Add(EEventName.DurationChange, value);
            remove => handlerMap.Remove(EEventName.DurationChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onEmptied
        {
            add => handlerMap.Add(EEventName.Emptied, value);
            remove => handlerMap.Remove(EEventName.Emptied, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onEnded
        {
            add => handlerMap.Add(EEventName.Ended, value);
            remove => handlerMap.Remove(EEventName.Ended, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onFocus
        {
            add => handlerMap.Add(EEventName.Focus, value);
            remove => handlerMap.Remove(EEventName.Focus, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onFormData
        {
            add => handlerMap.Add(EEventName.FormData, value);
            remove => handlerMap.Remove(EEventName.FormData, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onInput
        {
            add => handlerMap.Add(EEventName.Input, value);
            remove => handlerMap.Remove(EEventName.Input, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onInvalid
        {
            add => handlerMap.Add(EEventName.Invalid, value);
            remove => handlerMap.Remove(EEventName.Invalid, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyDown
        {
            add => handlerMap.Add(EEventName.KeyDown, value);
            remove => handlerMap.Remove(EEventName.KeyDown, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyPress
        {
            add => handlerMap.Add(EEventName.KeyPress, value);
            remove => handlerMap.Remove(EEventName.KeyPress, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onKeyUp
        {
            add => handlerMap.Add(EEventName.KeyUp, value);
            remove => handlerMap.Remove(EEventName.KeyUp, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoad
        {
            add => handlerMap.Add(EEventName.Load, value);
            remove => handlerMap.Remove(EEventName.Load, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadedData
        {
            add => handlerMap.Add(EEventName.LoadedData, value);
            remove => handlerMap.Remove(EEventName.LoadedData, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadedMetadata
        {
            add => handlerMap.Add(EEventName.LoadedMetadata, value);
            remove => handlerMap.Remove(EEventName.LoadedMetadata, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadEnd
        {
            add => handlerMap.Add(EEventName.LoadEnd, value);
            remove => handlerMap.Remove(EEventName.LoadEnd, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onLoadStart
        {
            add => handlerMap.Add(EEventName.LoadStart, value);
            remove => handlerMap.Remove(EEventName.LoadStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseDown
        {
            add => handlerMap.Add(EEventName.MouseDown, value);
            remove => handlerMap.Remove(EEventName.MouseDown, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseEnter
        {
            add => handlerMap.Add(EEventName.MouseEnter, value);
            remove => handlerMap.Remove(EEventName.MouseEnter, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseLeave
        {
            add => handlerMap.Add(EEventName.MouseLeave, value);
            remove => handlerMap.Remove(EEventName.MouseLeave, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseMove
        {
            add => handlerMap.Add(EEventName.MouseMove, value);
            remove => handlerMap.Remove(EEventName.MouseMove, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseOut
        {
            add => handlerMap.Add(EEventName.MouseOut, value);
            remove => handlerMap.Remove(EEventName.MouseOut, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseOver
        {
            add => handlerMap.Add(EEventName.MouseOver, value);
            remove => handlerMap.Remove(EEventName.MouseOver, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onMouseUp
        {
            add => handlerMap.Add(EEventName.MouseUp, value);
            remove => handlerMap.Remove(EEventName.MouseUp, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onWheel
        {
            add => handlerMap.Add(EEventName.Wheel, value);
            remove => handlerMap.Remove(EEventName.Wheel, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPause
        {
            add => handlerMap.Add(EEventName.Pause, value);
            remove => handlerMap.Remove(EEventName.Pause, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPlay
        {
            add => handlerMap.Add(EEventName.Play, value);
            remove => handlerMap.Remove(EEventName.Play, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onPlaying
        {
            add => handlerMap.Add(EEventName.Playing, value);
            remove => handlerMap.Remove(EEventName.Playing, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onProgress
        {
            add => handlerMap.Add(EEventName.Progress, value);
            remove => handlerMap.Remove(EEventName.Progress, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onRateChange
        {
            add => handlerMap.Add(EEventName.RateChange, value);
            remove => handlerMap.Remove(EEventName.RateChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onReset
        {
            add => handlerMap.Add(EEventName.Reset, value);
            remove => handlerMap.Remove(EEventName.Reset, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onResize
        {
            add => handlerMap.Add(EEventName.Resize, value);
            remove => handlerMap.Remove(EEventName.Resize, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onScroll
        {
            add => handlerMap.Add(EEventName.Scroll, value);
            remove => handlerMap.Remove(EEventName.Scroll, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSecurityPolicyViolation
        {
            add => handlerMap.Add(EEventName.SecurityPolicyViolation, value);
            remove => handlerMap.Remove(EEventName.SecurityPolicyViolation, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSeeked
        {
            add => handlerMap.Add(EEventName.Seeked, value);
            remove => handlerMap.Remove(EEventName.Seeked, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSeeking
        {
            add => handlerMap.Add(EEventName.Seeking, value);
            remove => handlerMap.Remove(EEventName.Seeking, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelect
        {
            add => handlerMap.Add(EEventName.Select, value);
            remove => handlerMap.Remove(EEventName.Select, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onStalled
        {
            add => handlerMap.Add(EEventName.Stalled, value);
            remove => handlerMap.Remove(EEventName.Stalled, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSubmit
        {
            add => handlerMap.Add(EEventName.Submit, value);
            remove => handlerMap.Remove(EEventName.Submit, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSuspend
        {
            add => handlerMap.Add(EEventName.Suspend, value);
            remove => handlerMap.Remove(EEventName.Suspend, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onTimeUpdate
        {
            add => handlerMap.Add(EEventName.TimeUpdate, value);
            remove => handlerMap.Remove(EEventName.TimeUpdate, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onToggle
        {
            add => handlerMap.Add(EEventName.Toggle, value);
            remove => handlerMap.Remove(EEventName.Toggle, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onVolumeChange
        {
            add => handlerMap.Add(EEventName.VolumeChange, value);
            remove => handlerMap.Remove(EEventName.VolumeChange, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onWaiting
        {
            add => handlerMap.Add(EEventName.Waiting, value);
            remove => handlerMap.Remove(EEventName.Waiting, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelectStart
        {
            add => handlerMap.Add(EEventName.SelectStart, value);
            remove => handlerMap.Remove(EEventName.SelectStart, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventCallback onSelectionChange
        {
            add => handlerMap.Add(EEventName.SelectionChange, value);
            remove => handlerMap.Remove(EEventName.SelectionChange, value);
        }
        #endregion

    }
}
