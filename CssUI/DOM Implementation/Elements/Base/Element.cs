using CssUI.CSS;
using CssUI.CSS.Internal;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Interfaces;
using CssUI.DOM.Internal;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI.DOM
{
    public class Element : ParentNode, INonDocumentTypeChildNode, ISlottable
    {
        #region Internal Properties
        internal OrderedDictionary<AtomicName<EAttributeName>, Attr> AttributeList { get; private set; } = new OrderedDictionary<AtomicName<EAttributeName>, Attr>();

        internal readonly Queue<IElementReaction> Custom_Element_Reaction_Queue = new Queue<IElementReaction>();
        #endregion

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
        /// This elements custom element state
        /// </summary>
        public ECustomElement CustomElementState { get; protected set; } = ECustomElement.Undefined;
        public DOMTokenList classList { get; private set; }

        public NamedNodeMap Attributes { get; private set; }
        #endregion

        #region CSS
        /// <summary>
        /// The layout box for this element
        /// </summary>
        public CssPrincipalBox Box { get; internal set; }
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

        /// <summary>
        /// an elements 'is' attribute specifies its custom element class
        /// </summary>
        public string is_value
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-is-value */
            get => getAttribute(EAttributeName.IS).Get_String();
            set => setAttribute(EAttributeName.IS, AttributeValue.From_String(value));
        }

        /// <summary>
        /// A completely unique identifier for this element 
        /// </summary>
        [CEReactions]
        public string id
        {/* The id attribute must reflect the "id" content attribute. */
            get => getAttribute(EAttributeName.ID).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.ID, AttributeValue.Parse(EAttributeName.ID, value)));
        }

        /// <summary>
        /// List of classes that apply to this element
        /// </summary>
        [CEReactions]
        public string className
        {/* The className attribute must reflect the "class" content attribute. */
            get => getAttribute(EAttributeName.Class).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Class, AttributeValue.Parse(EAttributeName.Class, value)));
        }

        /// <summary>
        /// Returns true if this element has any attributes defined
        /// </summary>
        /// <returns></returns>
        public bool hasAttributes() => AttributeList.Count > 0;

        /// <summary>
        /// Returns a list of names for all defined attributes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> getAttributeNames() => AttributeList.Select(a => a.Name);


        [CEReactions]
        public string slot
        {/* The slot attribute must reflect the "slot" content attribute. */
            get => getAttribute(EAttributeName.Slot).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Slot, AttributeValue.From_String(value)));
        }
        #endregion

        #region Slottable
        /// <summary>
        /// Returns the slot name
        /// </summary>
        public string Name
        {/* Docs: https://dom.spec.whatwg.org/#slotable-name */
            get => getAttribute(EAttributeName.Name).Get_String();
            set
            {
                AttributeValue val = getAttribute(EAttributeName.Name);
                string oldValue = val.Get_String();

                if (oldValue.Equals(value))
                    return;

                if (value == null && oldValue.Length <= 0)
                    return;

                if (value.Length <= 0 && oldValue == null)
                    return;

                if (string.IsNullOrEmpty(value))
                {
                    setAttribute(EAttributeName.Name, null);
                }
                else
                {
                    setAttribute(EAttributeName.Name, AttributeValue.From_String(value));
                }
                /* 6) If element is assigned, then run assign slotables for element’s assigned slot. */
                if (isAssigned)
                {
                    DOMCommon.Assign_Slottables(assignedSlot);
                }
                /* 7) Run assign a slot for element. */
                DOMCommon.Assign_A_Slot(this);
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
        internal Element(Document document, string localName, string prefix="", string Namespace="") : base()
        {/* Docs: https://dom.spec.whatwg.org/#interface-element */
            this.nodeDocument = document;
            this.localName = localName;
            this.prefix = prefix;
            this.NamespaceURI = Namespace;
            this.classList = new DOMTokenList(this, EAttributeName.Class);

            this.Style = new ElementPropertySystem(this);
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
        internal bool is_root => ReferenceEquals(null, parentNode);

        /// <summary>
        /// An element is being rendered if it has any associated CSS layout boxes, SVG layout boxes, or some equivalent in other styling languages.
        /// </summary>
        /// XXX: Implement this logic
        internal virtual bool is_being_rendered => !ReferenceEquals(null, Box);

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
                 // Get mouse coords
                 // test if they intersect our bounding rect
                 return this.getBoundingClientRect()
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
        internal bool Is_Defined
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-defined */
            get => CustomElementState == Enums.ECustomElement.Uncustomized || CustomElementState == Enums.ECustomElement.Custom;
        }

        /// <summary>
        /// Is this element a custom element
        /// <para>An element whose custom element state is "custom" is said to be custom.</para>
        /// </summary>
        internal bool Is_Custom
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-custom */
            get => CustomElementState == Enums.ECustomElement.Custom;
        }

        internal override bool Is_ShadowHost
        {/* Docs: https://dom.spec.whatwg.org/#element-shadow-host */
            get => !ReferenceEquals(null, shadowRoot);
        }

        internal bool is_potentially_scrollable
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#potentially-scrollable */
            get
            {
                /* An element is potentially scrollable if all of the following conditions are true: */
                /* The element has an associated CSS layout box. */
                if (!ReferenceEquals(null, this.Box))
                {/* The element is not the HTML body element, or it is and the root element’s used value of the overflow-x or overflow-y properties is not visible. */
                    var root = getRootNode() as Element;
                    if (!ReferenceEquals(this, ownerDocument.body) || (ReferenceEquals(this, ownerDocument.body) && (root.Style.Overflow_X != EOverflowMode.Visible || root.Style.Overflow_Y != EOverflowMode.Visible)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Do any of our elements exceed our bounds?
        /// </summary>
        internal bool has_overflow
        {
            get
            {
                if (0 != (Style.Overflow_X & (EOverflowMode.Clip)) || 0 != (Style.Overflow_Y & (EOverflowMode.Clip)))
                    return false;

                var bounds = this.getBoundingClientRect();
                if (bounds.left < Box.Padding.Left || bounds.right > Box.Padding.Right)
                    return true;
                if (bounds.top < Box.Padding.Top || bounds.bottom > Box.Padding.Bottom)
                    return true;

                return false;
            }
        }
        #endregion

        #region Internal Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool find_attribute(AtomicName<EAttributeName> Name, out Attr outAttrib)
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace */
            if (!AttributeList.TryGetValue(Name, out Attr attr))
            {
                outAttrib = null;
                return false;
            }

            outAttrib = attr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool find_attribute(AtomicName<EAttributeName> Name, string Namespace, out Attr outAttrib)
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace */
            if (!AttributeList.TryGetValue(Name, out Attr attr))
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
            if (!AttributeList.TryGetValue(qualifiedName, out Attr attr))
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
            if (!AttributeList.TryGetValue(localName, out Attr attr))
            {
                outAttrib = null;
                return false;
            }

            outAttrib = attr;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void change_attribute(Attr attr, AttributeValue oldValue, AttributeValue newValue)
        {
            /* To change an attribute attribute from an element element to value, run these steps: */
            /* 1) Queue an attribute mutation record for element with attribute’s local name, attribute’s namespace, and attribute’s value. */
            MutationRecord.Queue_Attribute_Mutation_Record(this, attr.Name, attr.namespaceURI, oldValue);

            /* 2) If element is custom, then enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing attribute’s local name, attribute’s value, value, and attribute’s namespace. */
            CEReactions.Enqueue_Reaction(this, EReactionName.AttributeChanged, attr.localName, attr.Value, attr.namespaceURI);

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
            CEReactions.Enqueue_Reaction(this, EReactionName.AttributeChanged, attr.localName, null, attr.Value, attr.namespaceURI);

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
            CEReactions.Enqueue_Reaction(this, EReactionName.AttributeChanged, attr.localName, attr.Value, null, attr.namespaceURI);

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
            CEReactions.Enqueue_Reaction(this, EReactionName.AttributeChanged, oldAttr.localName, oldAttr.Value, newAttr.Value, oldAttr.namespaceURI);

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
        public AttributeValue getAttribute(AtomicName<EAttributeName> Name)
        {
            Attr attr = AttributeList[qualifiedName];
            /* 2) If attr is null, return null. */
            /* 3) Return attr’s value. */
            return attr?.Value;
        }

        [Obsolete("Attributes should be specified via an AtomicName instance", true)]
        public string getAttribute(string qualifiedName)
        {
            return null;
        }

        public Attr getAttributeNode(AtomicName<EAttributeName> Name)
        {
            return this.AttributeList[Name];
        }

        [Obsolete("Attributes should be specified via an AtomicName instance", true)]
        public Attr getAttributeNode(string qualifiedName)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            return this.AttributeList[qualifiedName];
        }



        [CEReactions]
        public void setAttribute(AtomicName<EAttributeName> Name, AttributeValue value)
        {
            CEReactions.Wrap_CEReaction(this, () =>
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
            });
        }

        [CEReactions]
        [Obsolete("Attributes must be assigned through AttributeValue objects", true)]
        public void setAttribute(AtomicName<EAttributeName> Name, string value)
        {
        }

        [CEReactions]
        [Obsolete("Attributes should be specified via an AtomicName instance", true)]
        public void setAttribute(string qualifiedName, string value)
        {
        }

        [CEReactions]
        public Attr setAttributeNode(Attr attr)
        {
            return ReactionsCommon.Wrap_CEReaction(this, () =>
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
            });
        }



        [CEReactions]
        public void removeAttribute(AtomicName<EAttributeName> Name)
        {
            CEReactions.Wrap_CEReaction(this, () =>
            {
                find_attribute(Name, out Attr attr);
                if (!ReferenceEquals(attr, null))
                {
                    remove_attribute(attr);
                }
            });
        }

        [CEReactions]
        [Obsolete("Attributes should be specified via an AtomicName instance", true)]
        public void removeAttribute(string qualifiedName)
        {
            CEReactions.Wrap_CEReaction(this, () =>
            {
                find_attribute(qualifiedName, out Attr attr);
                if (!ReferenceEquals(attr, null))
                {
                    remove_attribute(attr);
                }
            });
        }

        [CEReactions]
        public Attr removeAttributeNode(Attr attr)
        {
            return ReactionsCommon.Wrap_CEReaction(this, () =>
            {
                find_attribute(attr.Name, out Attr outAttr);
                if (!ReferenceEquals(attr, null))
                {
                    remove_attribute(attr);
                }

                return attr;
            });
        }



        [CEReactions]
        public bool toggleAttribute(AtomicName<EAttributeName> Name, bool? force = null)
        {
            return ReactionsCommon.Wrap_CEReaction(this, () =>
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
                        var newAttr = new Attr(Name, this.ownerDocument) { Value = AttributeValue.Parse(Name, string.Empty) };
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
            });
        }

        [CEReactions, Obsolete("Use toggleAttribute(AtomicName<EAttributeName> Name, bool? force)", true)]
        public bool toggleAttribute(string qualifiedName, bool? force = null)
        {
            return ReactionsCommon.Wrap_CEReaction(this, () =>
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
                        var newAttr = new Attr(qualifiedName, this.ownerDocument) { Value = AttributeValue.Parse(qualifiedName, string.Empty) };
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
            });
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

        [Obsolete("Attributes should be specified via an AtomicName instance", true)]
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
        private ShadowRoot _shadow_root = null;
        public ShadowRoot shadowRoot
        {
            get
            {
                if (ReferenceEquals(null, _shadow_root))
                    return null;

                if (_shadow_root.Mode == EShadowRootMode.Closed)
                    return null;

                return _shadow_root;
            }

            private set
            {
                _shadow_root = value;
            }
        }


        ShadowRoot attachShadow(ShadowRootInit init)
        {/* Docs: https://dom.spec.whatwg.org/#dom-element-attachshadow */
            if (!NamespaceURI.Equals(DOMCommon.HTMLNamespace))
            {
                throw new NotSupportedError("Elements must be in the HTML namespace to support a ShadowDOM");
            }

            /* SKIP THIS */
            /* 2) If context object’s local name is not a valid custom element name, "article", "aside", "blockquote", "body", "div", "footer", "h1", "h2", "h3", "h4", "h5", "h6", "header", "main" "nav", "p", "section", or "span", then throw a "NotSupportedError" DOMException */
            /* 3) If context object’s local name is a valid custom element name, or context object’s is value is not null, then:
                    Let definition be the result of looking up a custom element definition given context object’s node document, its namespace, its local name, and its is value.
                    If definition is not null and definition’s disable shadow is true, then throw a "NotSupportedError" DOMException. 
            */
            /* XXX: Custom element stuff here */
            /* 4) If context object is a shadow host, then throw an "NotSupportedError" DOMException. */
            if (this.Is_ShadowHost)
            {
                throw new NotSupportedError("Cannot attach a shadow root to an element which already has a shadow root");
            }

            var shadow = new ShadowRoot(this, this.ownerDocument, init.Mode);
            this.shadowRoot = shadow;
            return shadow;

        }
        #endregion

        #region Geometry / Client Rects
        public LinkedList<DOMRect> getClientRects()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getclientrects */
            /* XXX: SVG layout box handling needed here */
            LinkedList<DOMRect> Rects = new LinkedList<DOMRect>();
            /* 1) If the element on which it was invoked does not have an associated layout box return an empty sequence and stop this algorithm. */
            if (ReferenceEquals(null, this.Box))
                return Rects;
            /* 2) If the element has an associated SVG layout box return a sequence containing a single DOMRect object that describes the bounding box of the element as defined by the SVG specification, applying the transforms that apply to the element and its ancestors. */
            /* 3) Return a sequence containing static DOMRect objects in content order, one for each box fragment, describing its border area (including those with a height or width of zero) with the following constraints: */
            foreach (CssBoxFragment fragment in Box.Fragments)
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

        #region Scrolling Internal Utility

        internal bool scroll_bounds_into_view(Element ancestor, DOMRect elementBoundingBorderBox, ScrollBox ancestorScrollBox, EScrollLogicalPosition block, EScrollLogicalPosition inline, EScrollBehavior behavior)
        {
            /* Edge definitions: https://www.w3.org/TR/cssom-view-1/#beginning-edges */
            double scrollingBoxEdgeA = 0; double elementEdgeA = 0;/* X */
            double scrollingBoxEdgeB = 0; double elementEdgeB = 0;/* X */
            double scrollingBoxEdgeC = 0; double elementEdgeC = 0;/* Y */
            double scrollingBoxEdgeD = 0; double elementEdgeD = 0;/* Y */

            var scrollBox = ancestorScrollBox;
            var scrollArea = scrollBox.ScrollArea;

            var OverflowBlock = scrollBox.Overflow_Block;
            var OverflowInline = scrollBox.Overflow_Inline;

            switch (OverflowBlock)
            {
                case CSS.Enums.EOverflowDirection.Leftward:
                    {
                        switch (OverflowInline)
                        {
                            case CSS.Enums.EOverflowDirection.Upward:
                                {
                                    scrollingBoxEdgeA = scrollArea.bottom;
                                    scrollingBoxEdgeB = scrollArea.top;
                                    scrollingBoxEdgeC = scrollArea.right;
                                    scrollingBoxEdgeD = scrollArea.left;

                                    elementEdgeA = elementBoundingBorderBox.bottom;
                                    elementEdgeB = elementBoundingBorderBox.top;
                                    elementEdgeC = elementBoundingBorderBox.right;
                                    elementEdgeD = elementBoundingBorderBox.left;
                                }
                                break;
                            case CSS.Enums.EOverflowDirection.Downward:
                                {
                                    scrollingBoxEdgeA = scrollArea.top;
                                    scrollingBoxEdgeB = scrollArea.bottom;
                                    scrollingBoxEdgeC = scrollArea.left;
                                    scrollingBoxEdgeD = scrollArea.right;

                                    elementEdgeA = elementBoundingBorderBox.top;
                                    elementEdgeB = elementBoundingBorderBox.bottom;
                                    elementEdgeC = elementBoundingBorderBox.left;
                                    elementEdgeD = elementBoundingBorderBox.right;
                                }
                                break;
                        }
                    }
                    break;
                case CSS.Enums.EOverflowDirection.Rightward:
                    {
                        switch (OverflowInline)
                        {
                            case CSS.Enums.EOverflowDirection.Upward:
                                {
                                    scrollingBoxEdgeA = scrollArea.bottom;
                                    scrollingBoxEdgeB = scrollArea.top;
                                    scrollingBoxEdgeC = scrollArea.left;
                                    scrollingBoxEdgeD = scrollArea.right;

                                    elementEdgeA = elementBoundingBorderBox.bottom;
                                    elementEdgeB = elementBoundingBorderBox.top;
                                    elementEdgeC = elementBoundingBorderBox.left;
                                    elementEdgeD = elementBoundingBorderBox.right;
                                }
                                break;
                            case CSS.Enums.EOverflowDirection.Downward:
                                {
                                    scrollingBoxEdgeA = scrollArea.top;
                                    scrollingBoxEdgeB = scrollArea.bottom;
                                    scrollingBoxEdgeC = scrollArea.left;
                                    scrollingBoxEdgeD = scrollArea.right;

                                    elementEdgeA = elementBoundingBorderBox.top;
                                    elementEdgeB = elementBoundingBorderBox.bottom;
                                    elementEdgeC = elementBoundingBorderBox.left;
                                    elementEdgeD = elementBoundingBorderBox.right;
                                }
                                break;
                        }
                    }
                    break;
            }

            /* 7) Let element width be the distance between element edge C and element edge D. */
            var elementWidth = elementEdgeD - elementEdgeC;
            var scrollingBoxWidth = scrollingBoxEdgeD - scrollingBoxEdgeC;

            /* 9) Let position be the scroll position scrolling box would have by following these steps: */
            DOMPoint position = new DOMPoint();
            switch (block)
            {
                case EScrollLogicalPosition.Start:
                    position.x = (scrollingBoxEdgeA - elementEdgeA);
                    break;
                case EScrollLogicalPosition.End:
                    position.x = (scrollingBoxEdgeB - (elementEdgeB - elementEdgeA));
                    break;
                case EScrollLogicalPosition.Center:
                    position.x = ((scrollingBoxEdgeB - scrollingBoxEdgeA) * 0.5) - ((elementEdgeB - elementEdgeA) * 0.5);
                    break;
                case EScrollLogicalPosition.Nearest:
                    {
                        /* If element edge A and element edge B are both outside scrolling box edge A and scrolling box edge B: Do nothing. */
                        if (elementEdgeA < scrollingBoxEdgeA && elementEdgeB > scrollingBoxEdgeB)
                        {
                            break;
                        }

                        /* If element edge A is outside scrolling box edge A and element width is less than scrolling box width */
                        if (elementEdgeA < scrollingBoxEdgeA && elementWidth < scrollingBoxWidth)
                        {/* Align element edge A with scrolling box edge A. */
                            position.x = (scrollingBoxEdgeA - elementEdgeA);
                        }

                        /* If element edge B is outside scrolling box edge B and element width is greater than scrolling box width */
                        if (elementEdgeB > scrollingBoxEdgeB && elementWidth > scrollingBoxWidth)
                        {/* Align element edge A with scrolling box edge A. */
                            position.x = (scrollingBoxEdgeA - elementEdgeA);
                        }

                        /* If element edge A is outside scrolling box edge A and element width is greater than scrolling box width */
                        if (elementEdgeA < scrollingBoxEdgeA && elementWidth > scrollingBoxWidth)
                        {/* Align element edge B with scrolling box edge B. */
                            position.x = (scrollingBoxEdgeB - (elementEdgeB - elementEdgeA));
                        }

                        /* If element edge B is outside scrolling box edge B and element width is less than scrolling box width */
                        if (elementEdgeB > scrollingBoxEdgeB && elementWidth < scrollingBoxWidth)
                        {/* Align element edge B with scrolling box edge B. */
                            position.x = (scrollingBoxEdgeB - (elementEdgeB - elementEdgeA));
                        }
                    }
                    break;
            }

            switch (inline)
            {
                case EScrollLogicalPosition.Start:
                    position.y = (scrollingBoxEdgeC - elementEdgeC);
                    break;
                case EScrollLogicalPosition.End:
                    position.y = (scrollingBoxEdgeD - (elementEdgeD - elementEdgeC));
                    break;
                case EScrollLogicalPosition.Center:
                    position.y = ((scrollingBoxEdgeD - scrollingBoxEdgeC) * 0.5) - ((elementEdgeD - elementEdgeC) * 0.5);
                    break;
                case EScrollLogicalPosition.Nearest:
                    {
                        /* If element edge C and element edge D are both outside scrolling box edge C and scrolling box edge D */
                        if (elementEdgeC < scrollingBoxEdgeC && elementEdgeD > scrollingBoxEdgeD)
                        {
                        }
                        /* If element edge C is outside scrolling box edge C and element width is less than scrolling box width */
                        if (elementEdgeC < scrollingBoxEdgeC && elementWidth < scrollingBoxWidth)
                        {
                            position.y = (scrollingBoxEdgeC - elementEdgeC);
                        }
                        /* If element edge D is outside scrolling box edge D and element width is greater than scrolling box width */
                        if (elementEdgeD > scrollingBoxEdgeD && elementWidth > scrollingBoxWidth)
                        {
                            position.y = (scrollingBoxEdgeC - elementEdgeC);
                        }
                        /* If element edge C is outside scrolling box edge C and element width is greater than scrolling box width */
                        if (elementEdgeC < scrollingBoxEdgeC && elementWidth > scrollingBoxWidth)
                        {
                            position.y = (scrollingBoxEdgeD - (elementEdgeD - elementEdgeC));
                        }
                        /* If element edge D is outside scrolling box edge D and element width is less than scrolling box width */
                        if (elementEdgeD > scrollingBoxEdgeD && elementWidth < scrollingBoxWidth)
                        {
                            position.y = (scrollingBoxEdgeC - elementEdgeC);
                        }
                    }
                    break;
            }

            /* 10) If position is the same as scrolling box’s current scroll position, and scrolling box does not have an ongoing smooth scroll, abort these steps. */
            if (MathExt.floatEq(position.x, scrollBox.ScrollX) && MathExt.floatEq(position.y, scrollBox.ScrollY) && !scrollBox.IsScrolling)
                return false;

            Element associatedElement = null;
            if (ReferenceEquals(null, scrollBox.Owner))
            {/* Viewport */
                associatedElement = scrollBox.View.document.activeElement;
            }
            else
            {/* Element */
                associatedElement = scrollBox.Owner;
            }

            scrollBox.Perform_Scroll(position, ancestor, behavior);
            return true;
        }

        /// <summary>
        /// Internal utility function for scrolling the current element into view. its a long process that is probably referenced a few different times so it gets its own function.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="inline"></param>
        internal void scroll_element_into_view(EScrollLogicalPosition block, EScrollLogicalPosition inline, EScrollBehavior behavior = EScrollBehavior.Auto)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#scroll-an-element-into-view */
            /* To scroll an element into view element, with a ScrollIntoViewOptions dictionary options, 
             * means to run these steps for each ancestor element or viewport that establishes a scrolling box scrolling box, 
             * in order of innermost to outermost scrolling box: */

            var tree = new TreeWalker(this, ENodeFilterMask.SHOW_ALL);
            Node node = tree.parentNode();
            while (!ReferenceEquals(null, node))
            {
                if (node is Element ancestor)
                {
                    /* 1) If the Document associated with element is not same origin with the Document associated with the element or viewport associated with box, terminate these steps. */
                    if (!ancestor.ownerDocument.Origin.Equals(ownerDocument.Origin))
                        break;

                    scroll_bounds_into_view(ancestor, this.getBoundingClientRect(), ancestor.ScrollBox, block, inline, behavior);
                }
            }

            /* Now also do the viewport... */
            scroll_bounds_into_view(null, this.getBoundingClientRect(), ownerDocument.Viewport.ScrollBox, block, inline, behavior);
        }

        internal void scroll_element(double x, double y, EScrollBehavior behavior = EScrollBehavior.Auto)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#scroll-an-element */
            /* 1) Let box be element’s associated scrolling box. */
            var box = ScrollBox;
            var scrollArea = box.ScrollArea;
            /* 2) If box has rightward overflow direction */
            if (ScrollBox.Overflow_Block == CSS.Enums.EOverflowDirection.Rightward)
            {
                x = MathExt.Max(0, MathExt.Min(x, scrollArea.width - Box.Padding.Width));
            }
            else
            {
                x = MathExt.Min(0, MathExt.Max(x, Box.Padding.Width - scrollArea.width));
            }

            /* 3) If box has downward overflow direction */
            if (ScrollBox.Overflow_Inline == CSS.Enums.EOverflowDirection.Downward)
            {
                y = MathExt.Max(0, MathExt.Min(y, scrollArea.height - Box.Padding.Height));
            }
            else
            {
                y = MathExt.Min(0, MathExt.Max(y, Box.Padding.Height - scrollArea.height));
            }

            /* 4) Let position be the scroll position box would have by aligning scrolling area x-coordinate x with the left of box and aligning scrolling area y-coordinate y with the top of box. */
            var origin = box.Origin;
            double deltaX = x - origin.x;
            double deltaY = y - origin.y;
            var position = new DOMPoint(box.ScrollX + deltaX, box.ScrollY + deltaY);
            /* 5) If position is the same as box’s current scroll position, and box does not have an ongoing smooth scroll, abort these steps. */
            if (position.Equals(box.ScrollX, box.ScrollY) && !box.IsScrolling)
            {
                return;
            }

            box.Perform_Scroll(position, this, behavior);
        }
        #endregion

        #region Scrolling
        internal ScrollBox ScrollBox = null;

        public double scrollTop
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrolltop */
            get
            {
                if (!ownerDocument.Is_FullyActive)
                    return 0;

                if (ReferenceEquals(null, ownerDocument.defaultView))
                    return 0;

                if (is_root && ownerDocument.Mode == EQuirksMode.Quirks)
                    return 0;

                if (is_root)
                    return ownerDocument.defaultView.scrollY;
                
                if (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return ownerDocument.defaultView.scrollY;

                if (ReferenceEquals(null, Box))
                    return 0;

                /* 9) Return the y-coordinate of the scrolling area at the alignment point with the top of the padding edge of the element. */
                /* XXX: The following might not actually be whats wanted by the specifications. What im going to do is combine the scrolling areas offset to our padding edge */
                /*      its possible that instead whats wanted is the actual non-relative coordinate point where the scrolling area MEETS the top padding edge */
                return Box.Padding.Top - ScrollBox.ScrollY;
            }
            set
            {
                double y = value;
                y = CssCommon.Normalize_Non_Finite(y);
                var document = ownerDocument;
                var window = document.defaultView;

                if (!document.Is_FullyActive)
                    return;

                if (ReferenceEquals(null, window))
                    return;

                if (is_root && document.Mode == EQuirksMode.Quirks)
                    return;

                if (is_root)
                {
                    window.Scroll(window.scrollX, y);
                    return;
                }

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                {
                    window.Scroll(window.scrollX, y);
                    return;
                }

                if (ReferenceEquals(null, Box) || ReferenceEquals(null, ScrollBox) || !has_overflow)
                    return;

                scroll_element(scrollLeft, y, EScrollBehavior.Auto);
            }
        }
        public double scrollLeft
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollleft */
            get
            {
                var document = ownerDocument;
                if (!document.Is_FullyActive)
                    return 0;

                var window = document.defaultView;
                if (ReferenceEquals(null, window))
                    return 0;

                if (is_root && document.Mode == EQuirksMode.Quirks)
                    return 0;

                if (is_root)
                    return window.scrollX;

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return window.scrollX;

                if (ReferenceEquals(null, Box))
                    return 0;

                /* 9) Return the x-coordinate of the scrolling area at the alignment point with the left of the padding edge of the element. */
                /* XXX: The following might not actually be whats wanted by the specifications. What im going to do is combine the scrolling areas offset to our padding edge */
                /*      its possible that instead whats wanted is the actual non-relative coordinate point where the scrolling area MEETS the top padding edge */
                return Box.Padding.Left - ScrollBox.ScrollX;
            }

            set
            {
                double x = CssCommon.Normalize_Non_Finite(value);
                var document = ownerDocument;
                if (!document.Is_FullyActive)
                    return;

                var window = document.defaultView;
                if (ReferenceEquals(null, window))
                    return;

                if (is_root && document.Mode == EQuirksMode.Quirks)
                    return;

                if (is_root)
                {
                    window.Scroll(x, window.scrollY);
                    return;
                }

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                {
                    window.Scroll(x, window.scrollY);
                    return;
                }

                if (ReferenceEquals(null, Box) || ReferenceEquals(null, ScrollBox) || !has_overflow)
                    return;

                scroll_element(x, scrollTop, EScrollBehavior.Auto);
            }
        }
        public long scrollWidth
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollwidth */
            get
            {
                var document = ownerDocument;
                if (!document.Is_FullyActive)
                    return 0;

                double viewportWidth = 0;
                if (!ReferenceEquals(null, document.Viewport) && !ReferenceEquals(null, document?.Viewport?.ScrollBox))
                {
                    viewportWidth = document.Viewport.Width - (document.Viewport.ScrollBox?.VScrollBar?.width ?? 0);
                }

                if (is_root && document.Mode != EQuirksMode.Quirks)
                    return (long)MathExt.Max(viewportWidth, document.Viewport.ScrollBox.ScrollArea.width);

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return (long)MathExt.Max(viewportWidth, document.Viewport.ScrollBox.ScrollArea.width);

                if (ReferenceEquals(null, Box))
                    return 0;

                return (long)ScrollBox.ScrollArea.width;
            }
        }
        public long scrollHeight
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollheight */
            get
            {
                var document = ownerDocument;
                if (!document.Is_FullyActive)
                    return 0;

                double viewportHeight = 0;
                if (!ReferenceEquals(null, document.Viewport) && !ReferenceEquals(null, document?.Viewport?.ScrollBox))
                {
                    viewportHeight = document.Viewport.Height - (document.Viewport.ScrollBox?.HScrollBar?.height ?? 0);
                }

                if (is_root && document.Mode != EQuirksMode.Quirks)
                    return (long)MathExt.Max(viewportHeight, document.Viewport.ScrollBox.ScrollArea.height);

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return (long)MathExt.Max(viewportHeight, document.Viewport.ScrollBox.ScrollArea.height);

                if (ReferenceEquals(null, Box))
                    return 0;

                return (long)ScrollBox.ScrollArea.height;
            }
        }

        public long clientTop
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-clienttop */
            get
            {
                if (ReferenceEquals(null, Box) || Box.OuterDisplayType == CssUI.Enums.EOuterDisplayType.Inline)
                    return 0;

                /* 2) Return the computed value of the border-top-width property plus the height of any scrollbar rendered between the top padding edge and the top border edge, 
                 * ignoring any transforms that apply to the element and its ancestors. */
                double retVal = Style.Border_Top_Width;
                retVal += MathExt.floatEq(0, ScrollBox.HScrollBar.top) ? ScrollBox.HScrollBar.height : 0;
                return (long)retVal;
            }
        }
        public long clientLeft
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-clientleft */
            get
            {
                if (ReferenceEquals(null, Box) || Box.OuterDisplayType == CssUI.Enums.EOuterDisplayType.Inline)
                    return 0;

                /* 2) Return the computed value of the border-left-width property plus the width of any scrollbar rendered between the left padding edge and the left border edge, 
                 * ignoring any transforms that apply to the element and its ancestors. */
                double retVal = Style.Border_Left_Width;
                retVal += MathExt.floatEq(0, ScrollBox.VScrollBar.left) ? ScrollBox.VScrollBar.width: 0;
                return (long)retVal;
            }
        }
        public long clientWidth
        {/* https://www.w3.org/TR/cssom-view-1/#dom-element-clientwidth */
            get
            {
                if (ReferenceEquals(null, Box) || Box.OuterDisplayType == CssUI.Enums.EOuterDisplayType.Inline)
                    return 0;

                /* 2) If the element is the root element and the element’s node document is not in quirks mode, 
                 * sor if the element is the HTML body element and the element’s node document is in quirks mode, 
                 * return the viewport width excluding the size of a rendered scroll bar (if any). */
                if ((is_root && ownerDocument.Mode != EQuirksMode.Quirks) || (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks))
                {
                    double vpSize = ownerDocument.Viewport.Width;
                    vpSize -= ReferenceEquals(null, ownerDocument.Viewport.ScrollBox.VScrollBar) ? 0 : ownerDocument.Viewport.ScrollBox.VScrollBar.width;
                    return (long)vpSize;
                }

                /* 3) Return the width of the padding edge excluding the width of any rendered scrollbar between the padding edge and the border edge, ignoring any transforms that apply to the element and its ancestors. */
                double clientSize = Box.Padding.Width;
                clientSize -= ReferenceEquals(null, ScrollBox.VScrollBar) ? 0 : ScrollBox.VScrollBar.width;
                return (long)clientSize;
            }
        }
        public long clientHeight
        {
            get
            {
                if (ReferenceEquals(null, Box) || Box.OuterDisplayType == CssUI.Enums.EOuterDisplayType.Inline)
                    return 0;

                /* 2) If the element is the root element and the element’s node document is not in quirks mode, 
                 * or if the element is the HTML body element and the element’s node document is in quirks mode, 
                 * return the viewport height excluding the size of a rendered scroll bar (if any). */
                if ((is_root && ownerDocument.Mode != EQuirksMode.Quirks) || (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks))
                {
                    double vpSize = ownerDocument.Viewport.Height;
                    vpSize -= ReferenceEquals(null, ownerDocument.Viewport.ScrollBox.HScrollBar) ? 0 : ownerDocument.Viewport.ScrollBox.HScrollBar.height;
                    return (long)vpSize;
                }

                /* 3) Return the height of the padding edge excluding the height of any rendered scrollbar between the padding edge and the border edge, ignoring any transforms that apply to the element and its ancestors. */
                double clientSize = Box.Padding.Height;
                clientSize -= ReferenceEquals(null, ScrollBox.HScrollBar) ? 0 : ScrollBox.HScrollBar.height;
                return (long)clientSize;
            }
        }

        /// <summary>
        /// Ensures this element is visible by scrolling any of its containing elements required such that it is shown.
        /// </summary>
        public void ScrollIntoView()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollintoview */

            /* 4) If arg is not specified or is true, let the block dictionary member of options have the value "start", and let the inline dictionary member of options have the value "nearest". */
            EScrollLogicalPosition block = EScrollLogicalPosition.Start;
            EScrollLogicalPosition inline = EScrollLogicalPosition.Nearest;

            if (ReferenceEquals(null, Box))
                return;

            /* 7) Scroll the element into view with the options. */
            scroll_element_into_view(block, inline);
            /* "Optionally perform some other action that brings the element to the user’s attention." */
        }

        /// <summary>
        /// Ensures this element is visible by scrolling any of its containing elements required such that it is shown.
        /// </summary>
        public void ScrollIntoView(bool scroll_to_start)
        {
            /* 4) If arg is not specified or is true, let the block dictionary member of options have the value "start", and let the inline dictionary member of options have the value "nearest". */
            EScrollLogicalPosition block = EScrollLogicalPosition.Start;
            EScrollLogicalPosition inline = EScrollLogicalPosition.Nearest;
            /* 5) If arg is false, let the block dictionary member of options have the value "end", and let the inline dictionary member of options have the value "nearest". */
            if (!scroll_to_start)
            {
                block = EScrollLogicalPosition.End;
                inline = EScrollLogicalPosition.Nearest;
            }

            if (ReferenceEquals(null, Box))
                return;

            /* 7) Scroll the element into view with the options. */
            scroll_element_into_view(block, inline);
            /* "Optionally perform some other action that brings the element to the user’s attention." */
        }

        /// <summary>
        /// Ensures this element is visible by scrolling any of its containing elements required such that it is shown.
        /// </summary>
        public void ScrollIntoView(ScrollIntoViewOptions options)
        {
            if (ReferenceEquals(null, Box))
                return;

            /* 7) Scroll the element into view with the options. */
            scroll_element_into_view(options.Block, options.Inline);
            /* "Optionally perform some other action that brings the element to the user’s attention." */
        }

        /// <summary>
        /// See: <see cref="ScrollTo(ScrollToOptions)"/>
        /// </summary>
        public void Scroll(ScrollToOptions options) => ScrollTo(options);

        /// <summary>
        /// See: <see cref="ScrollTo(double, double)"/>
        /// </summary>
        public void Scroll(double x, double y) => ScrollTo(x, y);

        /// <summary>
        /// Scrolls this elements scrollbox to the given position
        /// </summary>
        public void ScrollTo(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scroll */
            double x = !options.left.HasValue ? scrollLeft : CssCommon.Normalize_Non_Finite(options.left.Value);
            double y = !options.top.HasValue ? scrollTop : CssCommon.Normalize_Non_Finite(options.top.Value);
            /* 3) Let document be the element’s node document. */
            var document = ownerDocument;
            /* 4) If document is not the active document, terminate these steps. */
            if (!document.Is_FullyActive) return;
            /* 5) Let window be the value of document’s defaultView attribute. */
            var window = document.defaultView;
            /* 6) If window is null, terminate these steps. */
            if (ReferenceEquals(null, window)) return;

            /* 8) If the element is the root element invoke scroll() on window with scrollX on window as first argument and y as second argument, and terminate these steps. */
            if (is_root)
            {
                window.Scroll(window.scrollX, y);
                return;
            }

            /* 9) If the element is the HTML body element, document is in quirks mode, and the element is not potentially scrollable, invoke scroll() on window with options as the only argument, and terminate these steps. */
            if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
            {
                window.Scroll(options);
                return;
            }

            /* 10) If the element does not have any associated CSS layout box, the element has no associated scrolling box, or the element has no overflow, terminate these steps. */
            if (ReferenceEquals(null, this.Box) || ReferenceEquals(null, ScrollBox) || !has_overflow)
            {
                return;
            }

            scroll_element(x, y, options.behavior);
        }

        /// <summary>
        /// Scrolls this elements scrollbox to the given position
        /// </summary>
        /// <param name="x">X-Coordinate position to scroll to</param>
        /// <param name="y">Y-Coordinate position to scroll to</param>
        public void ScrollTo(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scroll */

            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);
            var options = new ScrollToOptions(EScrollBehavior.Auto, x, y);
            /* 3) Let document be the element’s node document. */
            var document = ownerDocument;
            /* 4) If document is not the active document, terminate these steps. */
            if (!document.Is_FullyActive) return;
            /* 5) Let window be the value of document’s defaultView attribute. */
            var window = document.defaultView;
            /* 6) If window is null, terminate these steps. */
            if (ReferenceEquals(null, window)) return;

            /* 8) If the element is the root element invoke scroll() on window with scrollX on window as first argument and y as second argument, and terminate these steps. */
            if (is_root)
            {
                window.Scroll(window.scrollX, y);
                return;
            }

            /* 9) If the element is the HTML body element, document is in quirks mode, and the element is not potentially scrollable, invoke scroll() on window with options as the only argument, and terminate these steps. */
            if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
            {
                window.Scroll(options);
                return;
            }

            /* 10) If the element does not have any associated CSS layout box, the element has no associated scrolling box, or the element has no overflow, terminate these steps. */
            if (ReferenceEquals(null, this.Box) || ReferenceEquals(null, ScrollBox) || !has_overflow)
            {
                return;
            }

            scroll_element(x, y, options.behavior);
        }

        /// <summary>
        /// Scrolls this elements contents by a given relative ammount
        /// </summary>
        /// <param name="options"></param>
        public void ScrollBy(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollby */
            double top = !options.top.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.top.Value);
            double left = !options.left.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.left.Value);

            top += scrollTop;
            left += scrollLeft;
            /* 5) Act as if the scroll() method was invoked with options as the only argument. */

            ScrollTo(left, top);
        }

        /// <summary>
        /// Scrolls this elements contents by a given relative ammount
        /// </summary>
        public void ScrollBy(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-scrollby */
            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);

            x += scrollLeft;
            y += scrollTop;

            var options = new ScrollToOptions(EScrollBehavior.Auto, x, y);
            ScrollTo(options);
        }
        #endregion
    }
}
