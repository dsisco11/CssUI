using CssUI.CSS;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Internal;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using CssUI.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI.DOM
{
    public class Element : ParentNode, INonDocumentTypeChildNode, ISlottable
    {
        #region Properties
        /// <summary>
        /// This elements official namespace name string
        /// </summary>
        public readonly string NamespaceURI = null;
        /// <summary>
        /// This elements namespace prefix eg: "html" or "xml"
        /// </summary>
        public readonly string prefix = null;
        /// <summary>
        /// Local part of the qualified name of an element.
        /// </summary>
        public string localName { get; set; }

        private string qualifiedName => (prefix == null) ? localName : string.Concat(prefix, ":", localName);

        private string _tagname = null;
        /// <summary>
        /// For example, if the element is an <img>, its tagName property is "IMG"
        /// </summary>
        public string tagName
        {
            get
            {/* Docs: https://dom.spec.whatwg.org/#element-html-uppercased-qualified-name */
                if (ReferenceEquals(null, _tagname))
                {
                    if (NamespaceURI == DOMCommon.HTMLNamespace && ownerDocument is HTMLDocument)
                        _tagname = new string(qualifiedName.Select(c => ASCIICommon.To_ASCII_Upper_Alpha(c)).ToArray());
                    else
                        _tagname = qualifiedName;
                }

                return _tagname;
            }
        }

        /// <summary>
        /// Tis elements custom element state
        /// </summary>
        public ECustomElement CustomElementState { get; protected set; } = ECustomElement.Undefined;
        public DOMTokenList classList { get; private set; }

        internal OrderedDictionary<AtomicName<EAttributeName>, Attr> AttributeList { get; private set; } = new OrderedDictionary<AtomicName<EAttributeName>, Attr>();
        public NamedNodeMap Attributes { get; private set; }
        #endregion

        #region CSS
        /// <summary>
        /// The layout box for this element
        /// </summary>
        internal CssLayoutBox Box;
        public readonly ElementPropertySystem Style;
        #endregion

        #region Node Overrides
        public override ENodeType nodeType => Enums.ENodeType.ELEMENT_NODE;
        public override string nodeName => this.localName.ToUpperInvariant();
        public override string nodeValue { get => null; set { /* specs say do nothing */ } }
        public override int nodeLength => this.childNodes.Count;
        public override string textContent {
            get
            {
                /* The textContent attribute’s getter must return the following, switching on context object: The descendant text content of the context object. */
                /* The descendant text content of a node node is the concatenation of the data of all the Text node descendants of node, in tree order. */
                var tree = new TreeWalker(this, ENodeFilterMask.SHOW_TEXT);
                StringBuilder sb = new StringBuilder();
                Node n;
                while((n = tree.firstChild()) != null)
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

        #region Accessors
        /*
         * Some Notes on 'Reflection':
         * IDL attributes that are defined to 'reflect' a content attribute of a given name, must have a getter and setter that follow these steps:
         * getter
         *      Return the result of running get an attribute value given context object and name.
         * setter
         *      Set an attribute value for the context object using name and the given value.
         */

        public string id
        {/* The id attribute must reflect the "id" content attribute. */
            get => this.getAttribute(EAttributeName.ID);
            set => this.setAttribute(EAttributeName.ID, value);
        }

        public string className
        {/* The className attribute must reflect the "class" content attribute. */
            get => this.getAttribute(EAttributeName.Class);
            set => this.setAttribute(EAttributeName.Class, value);
        }

        public IEnumerable<string> getAttributeNames() => this.AttributeList.Select(a => a.Name);
        public bool hasAttributes() => this.AttributeList.Count > 0;
        #endregion

        #region Slottable
        public string Name
        {/* Docs: https://dom.spec.whatwg.org/#slotable-name */
            get => getAttribute(EAttributeName.Name);

            set
            {
                var oldValue = getAttribute(EAttributeName.Name);
                if (0 == string.Compare(value, oldValue)) return;
                if (value == null && oldValue.Count() <= 0) return;
                if (value.Count() <= 0 && oldValue == null) return;
                if (string.IsNullOrEmpty(value))
                {
                    setAttribute(EAttributeName.Name, string.Empty);
                }
                else
                {
                    setAttribute(EAttributeName.Name, value);
                }
                /* 6) If element is assigned, then run assign slotables for element’s assigned slot. */
                if (this.isAssigned)
                {
                    DOMCommon.assign_slottables(assignedSlot);
                }
                /* 7) Run assign a slot for element. */
                DOMCommon.assign_a_slot(this);
            }
        }

        /* Docs: https://dom.spec.whatwg.org/#slotable-assigned-slot */
        public ISlot assignedSlot { get; set; } = null;
        #endregion

        #region INonDocumentTypeChildNode Implementation
        public Element previousElementSibling
        {
            get
            {
                Node n = previousSibling;
                while (!ReferenceEquals(null, n) && !(n is Element)) { n = n.previousSibling; }
                return n as Element;
            }
        }

        public Element nextElementSibling
        {
            get
            {
                Node n = nextSibling;
                while (!ReferenceEquals(null, n) && !(n is Element)) { n = n.nextSibling; }
                return n as Element;
            }
        }
        #endregion

        #region Constructors
        internal Element(Document document, string localName, string prefix="", string Namespace="")
        {/* Docs: https://dom.spec.whatwg.org/#interface-element */
            this.ownerDocument = document;
            this.localName = localName;
            this.prefix = prefix;
            this.NamespaceURI = Namespace;
            this.classList = new DOMTokenList(this, EAttributeName.Class);
        }
        #endregion

        #region Equality
        public override bool Equals(object obj)
        {/* https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Element B))
                return false;

            /* Ensure all attributes match */
            foreach (Attr attr in this.AttributeList)
            {
                if (!attr.Equals(B.getAttribute(attr.Name)))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            for(int i=0; i< AttributeList.Count; i++)
            {
                hash = hash * 31 + AttributeList[i].GetHashCode();
            }
            return hash;
        }
        #endregion

        #region Internal States
        /// <summary>
        /// An element is being rendered if it has any associated CSS layout boxes, SVG layout boxes, or some equivalent in other styling languages.
        /// </summary>
        /// XXX: Implement this logic
        internal virtual bool is_being_rendered { get { return true; } }


        internal bool is_in_formal_activation_state
        {/* Docs:  */
            get
            {
                /*
                 * An element is said to be in a formal activation state between the time the user begins to indicate an 
                 * intent to trigger the element's activation behavior and either the time the user stops indicating an intent to trigger the element's activation behavior, 
                 * or the time the element's activation behavior has finished running, which ever comes first.
                 */
                return is_actively_pointed_at;
            }
        }

        /// <summary>
        /// Returns if the user is indicating(hovering) at this element and their pointing device is in the "down" state
        /// </summary>
        internal bool is_actively_pointed_at
        {/* Docs:  */
            get
            {
                /*
                 * An element is said to be being actively pointed at while the user indicates the element using a pointing device while that pointing device is in the "down" state 
                 * (e.g. for a mouse, between the time the mouse button is pressed and the time it is depressed; for a finger in a multitouch environment, while the finger is touching the display surface).
                 */
            }
        }

        /// <summary>
        /// Returns if the user is indicating (hovering overtop) this element with a pointing device
        /// </summary>
        internal bool is_designated
        {/* Docs:  */
            get
            {
                /*
                 * An element that the user indicates using a pointing device.
                 * An element that has a descendant that the user indicates using a pointing device.
                 * An element that is the labeled control of a label element that is currently matching :hover.
                 */
                 return 
            }
        }

        /// <summary>
        /// Does this element currently have focus
        /// </summary>
        internal bool is_focused
        {/* Docs:  */
            get
            {
                /*
                 * For the purposes of the CSS :focus pseudo-class, an element has the focus when its top-level browsing context has the system focus, 
                 * it is not itself a browsing context container, and it is one of the elements listed in the focus chain of the currently focused area of the top-level browsing context.
                 */
                var focusedElement = ownerDocument.activeElement;
                if (ReferenceEquals(this, focusedElement))
                {
                    return true;
                }

                IEnumerable<Node> focusChain = DOMCommon.Get_Focus_Chain(focusedElement);
                foreach (Node node in focusChain)
                {
                    if (ReferenceEquals(this, node))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// A node (in particular elements and text nodes) can be marked as inert. 
        /// When a node is inert, then the user agent must act as if the node was absent for the purposes of targeting user interaction events, 
        /// may ignore the node for the purposes of text search user interfaces (commonly known as "find in page"), 
        /// and may prevent the user from selecting text in that node. User agents should allow the user to override the restrictions on search and text selection, however.
        /// </summary>
        internal bool inert = false;
        internal bool is_expressly_inert
        {/* Docs:  */
            get => (this.inert && !parentElement.inert);
        }

        /// <summary>
        /// Is this element a properly defined element
        /// </summary>
        internal bool is_defined
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-defined */
            get => CustomElementState == Enums.ECustomElement.Uncustomized || CustomElementState == Enums.ECustomElement.Custom;
        }

        internal bool is_shadowhost
        {/* Docs: https://dom.spec.whatwg.org/#element-shadow-host */
            get => !ReferenceEquals(null, shadowRoot);
        }
        #endregion

        #region Internal Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool find_attribute(AtomicName<EAttributeName> Name, out Attr outAttrib)
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace */
            if (!this.AttributeList.TryGetValue(Name, out Attr attr))
            {
                outAttrib = null;
                return false;
            }

            outAttrib = attr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool find_attribute(string qualifiedName, out Attr outAttrib)
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace */
            qualifiedName = qualifiedName.ToLowerInvariant();
            if (!this.AttributeList.TryGetValue(qualifiedName, out Attr attr))
            {
                outAttrib = null;
                return false;
            }

            outAttrib = attr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool find_attribute(string localName, string Namespace, out Attr outAttrib)
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace */
            localName = string.Concat(Namespace, ":", localName.ToLowerInvariant());
            if (!this.AttributeList.TryGetValue(localName, out Attr attr))
            {
                outAttrib = null;
                return false;
            }

            outAttrib = attr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void change_attribute(Attr attr, string oldValue, string newValue)
        {
            /* To change an attribute attribute from an element element to value, run these steps: */
            /* 1) Queue an attribute mutation record for element with attribute’s local name, attribute’s namespace, and attribute’s value. */
            MutationRecord.Queue_Attribute_Mutation_Record(this, attr.Name, attr.namespaceURI, oldValue);
            /* 2) If element is custom, then enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing attribute’s local name, attribute’s value, value, and attribute’s namespace. */
            /* 3) Run the attribute change steps with element, attribute’s local name, attribute’s value, value, and attribute’s namespace. */
            /* 4) Set attribute’s value to value. */
            attr.Value = newValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void append_attribute(Attr attr)
        {
            /* To append an attribute attribute to an element element, run these steps: */
            /* 1) Queue an attribute mutation record for element with attribute’s local name, attribute’s namespace, and null. */
            MutationRecord.Queue_Attribute_Mutation_Record(this, attr.Name, attr.namespaceURI, attr.Value);
            /* 2) If element is custom, then enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing attribute’s local name, null, attribute’s value, and attribute’s namespace. */
            /* 3) Run the attribute change steps with element, attribute’s local name, null, attribute’s value, and attribute’s namespace. */
            change_attribute(attr, null, attr.Value);
            /* 4) Append attribute to element’s attribute list. */
            this.AttributeList.Add(attr.Name.ToLowerInvariant(), attr);
            /* 5) Set attribute’s element to element. */
            attr.ownerElement = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void remove_attribute(Attr attr)
        {
            /* To remove an attribute attribute from an element element, run these steps: */
            /* 1) Queue an attribute mutation record for element with attribute’s local name, attribute’s namespace, and attribute’s value. */
            MutationRecord.Queue_Attribute_Mutation_Record(this, attr.Name, attr.namespaceURI, attr.Value);
            /* 2) If element is custom, then enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing attribute’s local name, attribute’s value, null, and attribute’s namespace. */
            /* 3) Run the attribute change steps with element, attribute’s local name, attribute’s value, null, and attribute’s namespace. */
            change_attribute(attr, attr.Value, null);
            /* 4) Remove attribute from element’s attribute list. */
            this.AttributeList.Remove(attr.Name.ToLowerInvariant());
            /* 5) Set attribute’s element to null. */
            attr.ownerElement = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void replace_attribute(Attr oldAttr, Attr newAttr)
        {
            /* To replace an attribute oldAttr by an attribute newAttr in an element element, run these steps: */
            /* 1) Queue an attribute mutation record for element with oldAttr’s local name, oldAttr’s namespace, and oldAttr’s value. */
            MutationRecord.Queue_Attribute_Mutation_Record(this, oldAttr.Name, oldAttr.namespaceURI, oldAttr.Value); // REDUNDANT
            /* 2) If element is custom, then enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing oldAttr’s local name, oldAttr’s value, newAttr’s value, and oldAttr’s namespace. */
            /* 3) Run the attribute change steps with element, oldAttr’s local name, oldAttr’s value, newAttr’s value, and oldAttr’s namespace. */
            change_attribute(oldAttr, oldAttr.Value, newAttr.Value);
            /* 4) Replace oldAttr by newAttr in element’s attribute list. */
            // find_attribute(oldAttr.Name, out Attr _, out int outIndex);
            this.AttributeList[oldAttr.Name.ToLowerInvariant()] = newAttr;
            /* 5) Set oldAttr’s element to null. */
            oldAttr.ownerElement = null;
            /* 6) Set newAttr’s element to element. */
            newAttr.ownerElement = this;
        }
        #endregion

        #region HTML Attribute Management
        public string getAttribute(AtomicName<EAttributeName> Name)
        {
            Attr attr = this.AttributeList[qualifiedName];
            /* 2) If attr is null, return null. */
            /* 3) Return attr’s value. */
            return attr?.Value;
        }

        public string getAttribute(string qualifiedName)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            Attr attr = this.AttributeList[qualifiedName];
            /* 2) If attr is null, return null. */
            /* 3) Return attr’s value. */
            return attr?.Value;
        }

        public Attr getAttributeNode(AtomicName<EAttributeName> Name)
        {
            return this.AttributeList[Name];
        }

        public Attr getAttributeNode(string qualifiedName)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            return this.AttributeList[qualifiedName];
        }


        public void setAttribute(AtomicName<EAttributeName> Name, string value)
        {
            find_attribute(Name, out Attr attr);

            if (ReferenceEquals(attr, null))
            {
                Attr newAttr = new Attr(Name.Name, this);
                newAttr.Value = value;
                append_attribute(newAttr);
                return;
            }

            change_attribute(attr, attr.Value, value);
        }

        public void setAttribute(string qualifiedName, string value)
        {
            find_attribute(qualifiedName, out Attr attr);

            if (ReferenceEquals(attr, null))
            {
                Attr newAttr = new Attr(qualifiedName, this);
                newAttr.Value = value;
                append_attribute(newAttr);
                return;
            }

            change_attribute(attr, attr.Value, value);
        }
        
        public Attr setAttributeNode(Attr attr)
        {
            find_attribute(attr.Name, out Attr oldAttr);
            if (ReferenceEquals(attr, oldAttr))
                return attr;

            if (!ReferenceEquals(oldAttr, null))
            {
                replace_attribute(oldAttr, attr);
            }
            else
            {
                append_attribute(attr);
            }

            return oldAttr;
        }

        public void removeAttribute(AtomicName<EAttributeName> Name)
        {
            find_attribute(Name, out Attr attr);
            if (!ReferenceEquals(attr, null))
            {
                remove_attribute(attr);
            }
        }

        public void removeAttribute(string qualifiedName)
        {
            find_attribute(qualifiedName, out Attr attr);
            if (!ReferenceEquals(attr, null))
            {
                remove_attribute(attr);
            }
        }

        public Attr removeAttributeNode(Attr attr)
        {
            find_attribute(attr.Name, out Attr outAttr);
            if (!ReferenceEquals(attr, null))
            {
                remove_attribute(attr);
            }

            return attr;
        }

        public bool toggleAttribute(AtomicName<EAttributeName> Name, bool? force = null)
        {
            /* 1) If qualifiedName does not match the Name production in XML, then throw an "InvalidCharacterError" DOMException. */
            /* 2) If the context object is in the HTML namespace and its node document is an HTML document, then set qualifiedName to qualifiedName in ASCII lowercase. */
            /* 3) Let attribute be the first attribute in the context object’s attribute list whose qualified name is qualifiedName, and null otherwise. */
            find_attribute(Name, out Attr attr);
            /* 4) If attribute is null, then: */
            if (ReferenceEquals(null, attr))
            {
                /* 1) If force is not given or is true, create an attribute whose local name is qualifiedName, value is the empty string, and node document is the context object’s node document, then append this attribute to the context object, and then return true. */
                if (!force.HasValue || force.Value)
                {
                    var newAttr = new Attr(Name, this.ownerDocument) { Value = string.Empty };
                    append_attribute(newAttr);
                    return true;
                }
                /* 2) Return false. */
                return false;
            }
            /* 5) Otherwise, if force is not given or is false, remove an attribute given qualifiedName and the context object, and then return false. */
            if (!force.HasValue || !force.Value)
            {
                remove_attribute(attr);
                return false;
            }
            /* 6) Return true. */
            return true;
        }

        public bool toggleAttribute(string qualifiedName, bool? force = null)
        {
            /* 1) If qualifiedName does not match the Name production in XML, then throw an "InvalidCharacterError" DOMException. */
            /* 2) If the context object is in the HTML namespace and its node document is an HTML document, then set qualifiedName to qualifiedName in ASCII lowercase. */
            qualifiedName = qualifiedName.ToLowerInvariant();
            /* 3) Let attribute be the first attribute in the context object’s attribute list whose qualified name is qualifiedName, and null otherwise. */
            find_attribute(qualifiedName, out Attr attr);
            /* 4) If attribute is null, then: */
            if (ReferenceEquals(null, attr))
            {
                /* 1) If force is not given or is true, create an attribute whose local name is qualifiedName, value is the empty string, and node document is the context object’s node document, then append this attribute to the context object, and then return true. */
                if (!force.HasValue || force.Value)
                {
                    var newAttr = new Attr(qualifiedName, this.ownerDocument) { Value = string.Empty };
                    append_attribute(newAttr);
                    return true;
                }
                /* 2) Return false. */
                return false;
            }
            /* 5) Otherwise, if force is not given or is false, remove an attribute given qualifiedName and the context object, and then return false. */
            if (!force.HasValue || !force.Value)
            {
                remove_attribute(attr);
                return false;
            }
            /* 6) Return true. */
            return true;
        }

        public bool hasAttribute(AtomicName<EAttributeName> Name)
        {
            if (this.AttributeList.TryGetValue(Name, out Attr attr))
                return !ReferenceEquals(null, attr.Value);

            return false;
        }

        public bool hasAttribute(AtomicName<EAttributeName> Name, out Attr outAttr)
        {
            if (this.AttributeList.TryGetValue(Name, out Attr attr))
            {
                outAttr = attr;
                return !ReferenceEquals(null, attr.Value);
            }

            outAttr = null;
            return false;
        }

        public bool hasAttribute(string qualifiedName)
        {
            if (this.AttributeList.TryGetValue(qualifiedName.ToLowerInvariant(), out Attr attr))
                return !ReferenceEquals(null, attr.Value);

            return false;
        }
        #endregion

        #region Element Matching / CSS Selectors
        /// <summary>
        /// Returns the first (starting at element) inclusive ancestor that matches selectors, and null otherwise.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public Element closest(string selectors)
        {/* Docs: https://dom.spec.whatwg.org/#dom-element-closest */
            /* The closest(selectors) method, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var Selector = new CssSelector(selectors);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (ReferenceEquals(Selector, null))
            {
                throw new DomSyntaxError("Could not parse selector.");
            }
            /* 3) Let elements be context object’s inclusive ancestors that are elements, in reverse tree order. */
            TreeWalker tree = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
            //* 4) For each element in elements, if match a selector against an element, using s, element, and :scope element context object, returns success, return element. [SELECTORS4] *//*
            while(true)
            {
                Node n = tree.parentNode();
                if (ReferenceEquals(n, null))
                    break;

                Element element = (Element)n;
                if (Selector.Match(element, this))
                    return element;
            }
            /* 5) Return null */
            return null;
        }

        /// <summary>
        /// Returns true if matching selectors against element’s root yields element, and false otherwise.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public bool matches(string selectors)
        {/* https://dom.spec.whatwg.org/#dom-element-matches */
            /* The matches(selectors) and webkitMatchesSelector(selectors) methods, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var s = new CssSelector(selectors);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (ReferenceEquals(s, null))
            {
                throw new DomSyntaxError("Could not parse selector.");
            }
            /* 3) Return true if the result of match a selector against an element, using s, element, and :scope element context object, returns success, and false otherwise. [SELECTORS4] */
            return s.Match(this, this);
        }


        /// <summary>
        /// If qualifiedName is "*" returns a collection of all descendant elements.
        /// Otherwise, returns a collection of all descendant elements whose qualified name is qualifiedName. (Matches case-insensitively against elements in the HTML namespace within an HTML document.)
        /// </summary>
        /// <param name="qualifiedName"></param>
        public IEnumerable<Element> getElementsByTagName(string qualifiedName)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagname */
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
        public IEnumerable<Element> getElementsByTagNameNS(string Namespace, string qualifiedName)
        {/* Docs:  */
            return DOMCommon.Get_Elements_By_Namespace_And_Local_Name(this, Namespace, qualifiedName);
        }

        /// <summary>
        /// Returns a collection of the elements in the object on which the method was invoked (a document or an element) that have all the classes given by classNames. 
        /// The classNames argument is interpreted as a space-separated list of classes.
        /// </summary>
        /// <param name="classNames"></param>
        public IEnumerable<Element> getElementsByClassName(string classNames)
        {/* Docs: https://dom.spec.whatwg.org/#dom-element-getelementsbyclassname */
            return DOMCommon.Get_Elements_By_Class_Name(this, classNames);
        }
        #endregion

        #region Shadow DOM
        /* XXX: ShadowDOM stuff here */
        public ShadowRoot shadowRoot { get; private set; }


        // Docs: https://dom.spec.whatwg.org/#dom-element-attachshadow
        ShadowRoot attachShadow(ShadowRootInit init);
        #endregion


        #region Client Rects
        public LinkedList<DOMRect> getClientRects()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getclientrects */
            /* XXX: SVG layout box handling needed here */
            LinkedList<DOMRect> Rects = new LinkedList<DOMRect>();
            /* 1) If the element on which it was invoked does not have an associated layout box return an empty sequence and stop this algorithm. */
            if (ReferenceEquals(null, this.Box))
                return Rects;
            /* 2) If the element has an associated SVG layout box return a sequence containing a single DOMRect object that describes the bounding box of the element as defined by the SVG specification, applying the transforms that apply to the element and its ancestors. */
            /* 3) Return a sequence containing static DOMRect objects in content order, one for each box fragment, describing its border area (including those with a height or width of zero) with the following constraints: */
            foreach (CssBoxFragment fragment in Box)
            {
                /* 1) Apply the transforms that apply to the element and its ancestors. */
                /* 2) If the element on which the method was invoked has a computed value for the display property of table or inline-table include both the table box and the caption box, if any, but not the anonymous container box. */
                /* 3) Replace each anonymous block box with its child box(es) and repeat this until no anonymous block boxes are left in the final list. */

                /* XXX: Transforms must be applied here */
                if (fragment is CssAnonymousBox anonymousBox)
                {
                    foreach(var child in anonymousBox)
                    {
                        var r = new DOMRect(child.Border.Left, child.Border.Top, child.Border.Width, child.Border.Height);
                        Rects.AddLast(r);
                    }
                }
                else
                {
                    var r = new DOMRect(fragment.Border.Left, fragment.Border.Top, fragment.Border.Width, fragment.Border.Height);
                    Rects.AddLast(r);
                }
            }

            return Rects;
        }

        public DOMRect getBoundingClientRect()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getboundingclientrect */
            return DOMCommon.getBoundingClientRect(this.getClientRects());
        }
        #endregion
    }
}
