﻿using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.Devices;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;
using CssUI.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
#if ENABLE_HTML
using CssUI.HTML;
using CssUI.HTML.CustomElements;
#endif

namespace CssUI.DOM
{
    public class Element : ParentNode, INonDocumentTypeChildNode, ISlottable, ICssElement
    {/* Docs: https://dom.spec.whatwg.org/#interface-element */
        #region Internal Properties
        internal OrderedDictionary<AtomicName<EAttributeName>, Attr> AttributeList { get; private set; } = new OrderedDictionary<AtomicName<EAttributeName>, Attr>();
        internal OrderedDictionary<AtomicName<EAttributeName>, IAttributeTokenList> tokenListMap = new OrderedDictionary<AtomicName<EAttributeName>, IAttributeTokenList>();
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

        private string qualifiedName => (prefix is null) ? localName : string.Concat(prefix, ":", localName);

        private string _tagname = null;
        /// <summary>
        /// For example, if the element is an <img>, its tagName property is "IMG"
        /// </summary>
        public string tagName
        {
            get
            {/* Docs: https://dom.spec.whatwg.org/#element-html-uppercased-qualified-name */
                if (_tagname is null)
                {
                    if (StringCommon.StrEq(NamespaceURI, DOMCommon.HTMLNamespace) && ownerDocument is HTMLDocument)
                        _tagname = StringCommon.Transform(qualifiedName, UnicodeCommon.To_ASCII_Upper_Alpha);
                    else
                        _tagname = qualifiedName;
                }

                return _tagname;
            }
        }

        public DOMTokenList classList { get; private set; }

        public NamedNodeMap Attributes { get; private set; }
        #endregion

        #region Custom Element
#if ENABLE_HTML
        internal readonly Queue<IElementReaction> Custom_Element_Reaction_Queue = new Queue<IElementReaction>();
        /// <summary>
        /// This elements custom element state
        /// </summary>
        public ECustomElement CustomElementState { get; internal set; } = ECustomElement.Undefined;
        /// <summary>
        /// The custom element definition
        /// </summary>
        public WeakReference<CustomElementDefinition> Definition { get; internal set; } = null;
        internal ElementInternals internals = null;
#endif
        #endregion

        #region Node Overrides
        public override ENodeType nodeType => Enums.ENodeType.ELEMENT_NODE;
        public override string nodeName => localName.ToUpperInvariant();
        public override string nodeValue { get => null; set { /* specs say do nothing */ } }
        public override int nodeLength => childNodes.Count;
        public override string textContent
        {
            get
            {
                /* The textContent attribute’s getter must return the following, switching on context object: The descendant text content of the context object. */
                /* The descendant text content of a node node is the concatenation of the data of all the Text node descendants of node, in tree order. */
                var tree = new TreeWalker(this, ENodeFilterMask.SHOW_TEXT);
                StringBuilder sb = new StringBuilder();
                Node n;
                while ((n = tree.firstChild()) is object)
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
                Node.Dom_replace_all_within_node(node, parentNode);
            }
        }
        #endregion

        #region CSS
        public new CssPrincipalBox Box
        {
            get
            {
                return (CssPrincipalBox)base.Box;
            }
            set
            {
                base.Box = value;
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
        /// A completely unique identifier for this element 
        /// </summary>
        [CEReactions]
        public string id
        {/* The id attribute must reflect the "id" content attribute. */
            get => getAttribute(EAttributeName.ID).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.ID, AttributeValue.Parse(EAttributeName.ID, value)));
        }

        /// <summary>
        /// List of classes that apply to this element
        /// </summary>
        [CEReactions]
        public string className
        {/* The className attribute must reflect the "class" content attribute. */
            get => getAttribute(EAttributeName.Class).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Class, AttributeValue.Parse(EAttributeName.Class, value)));
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


        /// <summary>
        /// an elements 'is' attribute specifies its custom element class
        /// </summary>
        public string is_value
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-is-value */
#if ENABLE_HTML
            get => getAttribute(EAttributeName.IS).AsString();
            set => setAttribute(EAttributeName.IS, AttributeValue.From(value));
#else
            get => string.Empty;
            set { }
#endif
        }

#if ENABLE_HTML
        [CEReactions]
        public string slot
        {/* The slot attribute must reflect the "slot" content attribute. */
            get => getAttribute(EAttributeName.Slot).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Slot, AttributeValue.From(value)));
        }
#endif
        #endregion

        #region Slottable
#if ENABLE_HTML
        private string _slot_Name = string.Empty;
        /// <summary>
        /// Returns the slot name
        /// </summary>
        public string Slot_Name
        {/* Docs: https://dom.spec.whatwg.org/#slotable-name */
            get => _slot_Name;
            set
            {
                string oldValue = _slot_Name;

                if (oldValue.Equals(value))
                    return;

                if (ReferenceEquals(value, null) && oldValue.Length <= 0)
                    return;

                if (value.Length <= 0 && ReferenceEquals(oldValue, null))
                    return;

                if (string.IsNullOrEmpty(value))
                {
                    _slot_Name = null;
                }
                else
                {
                    _slot_Name = value;
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
#endif
        #endregion

        #region INonDocumentTypeChildNode Implementation
#if USE_FUNCTIONS_FOR_NODE_RELATIONSHIP_LINKS
        public Element previousElementSibling
        {
            get
            {
                Node n = previousSibling;
                while ((n != null) && !(n is Element)) { n = n.previousSibling; }
                return n as Element;
            }
        }

        public Element nextElementSibling
        {
            get
            {
                Node n = nextSibling;
                while ((n != null) && !(n is Element)) { n = n.nextSibling; }
                return n as Element;
            }
        }
#else
        /// <summary>
        /// Holds a pointer to this element within it's parent's linked-list of child elements
        /// </summary>
        internal LinkedListNode<Element> ptrSelfRef = null;
        public Element previousElementSibling => ptrSelfRef?.Previous?.Value;
        public Element nextElementSibling => ptrSelfRef?.Next?.Value;
#endif

        #endregion

        #region Constructors
        public Element(Document document, string localName, string prefix = "", string Namespace = "") : base()
        {/* Docs: https://dom.spec.whatwg.org/#interface-element */
            nodeDocument = document;
            this.localName = localName;
            this.prefix = prefix;
            NamespaceURI = Namespace;
            classList = new DOMTokenList(this, EAttributeName.Class);

            Style = new StyleProperties(this);
        }
        #endregion


        #region Equality
        public override bool Equals(object obj)
        {/* https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Element B))
                return false;


            if (AttributeList.Count != B.AttributeList.Count)
                return false;

            if (!StringCommon.StrEq(NamespaceURI, B.NamespaceURI))
                return false;

            if (!StringCommon.StrEq(prefix, B.prefix))
                return false;

            if (!StringCommon.StrEq(localName, B.localName))
                return false;

            /* If A is an element, each attribute in its attribute list has an attribute that equals an attribute in B’s attribute list. */
            foreach (Attr attr in AttributeList)
            {
                Attr bAttr = B.getAttributeNode(attr.localName);
                if (bAttr == null)
                    return false;

                if (!attr.Equals(bAttr))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            for (int i = 0; i < AttributeList.Count; i++)
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
        internal virtual bool is_being_rendered => (Box is object);

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
                foreach (PointerDevice pointer in PointerDevice.Get_All())
                {
                    if (0 != (pointer.Buttons & EPointerButtonFlags.Left)) continue;/* Skip any pointing device that isnt in the "down" state */

                    if (CssUI.Geometry.Intersects(Box.ClickArea, pointer.GetBoundingClientRect()))
                        return true;
                }

                return false;
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

                foreach (PointerDevice pointer in PointerDevice.Get_All())
                {
                    if (CssUI.Geometry.Intersects(Box.ClickArea, pointer.GetBoundingClientRect()))
                        return true;
                }

#if ENABLE_HTML
                if (this is ILableableElement labelable)
                {
                    CssSelector hoverSelector = new CssSelector(":hover");
                    var labels = labelable.labels;
                    foreach (HTMLLabelElement lbl in labels)
                    {
                        if (hoverSelector.Match(lbl))
                        {
                            return true;
                        }
                    }
                }
#endif

                var tree = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
                var descendant = tree.nextNode();
                while (descendant != null)
                {
                    if (descendant is Element descendantElement)
                    {
                        foreach (PointerDevice pointer in PointerDevice.Get_All())
                        {
                            if (CssUI.Geometry.Intersects(descendantElement.Box.ClickArea, pointer.GetBoundingClientRect()))
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Does this element currently have focus
        /// </summary>
        internal bool is_focused
        {/* Docs:  */
            get
            {
                /* An element that is the DOM anchor of a focusable area is said to gain focus when that focusable area becomes the currently focused area of a top-level browsing context. 
                 * When an element is the DOM anchor of a focusable area of the currently focused area of a top-level browsing context, it is focused. */

                /*
                 * For the purposes of the CSS :focus pseudo-class, an element has the focus when its top-level browsing context has the system focus, 
                 * it is not itself a browsing context container, and it is one of the elements listed in the focus chain of the currently focused area of the top-level browsing context.
                 */
                var focusedElement = ownerDocument?.defaultView?.FocusedArea?.DOMAnchor;
                if (ReferenceEquals(this, focusedElement))
                {
                    return true;
                }

                var focusChain = DOMCommon.Get_Focus_Chain(focusedElement);
                foreach (FocusableArea area in focusChain)
                {
                    if (ReferenceEquals(this, area.DOMAnchor))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Is this element a properly defined element
        /// </summary>
        internal bool Is_Defined
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-defined */
#if ENABLE_HTML
            get => CustomElementState == Enums.ECustomElement.Uncustomized || CustomElementState == Enums.ECustomElement.Custom;
#else
            get => false;
#endif
        }

        /// <summary>
        /// Is this element a custom element
        /// <para>An element whose custom element state is "custom" is said to be custom.</para>
        /// </summary>
        internal bool isCustom
        {/* Docs: https://dom.spec.whatwg.org/#concept-element-custom */
#if ENABLE_HTML
            get => CustomElementState == Enums.ECustomElement.Custom;
#else
            get => false;
#endif
        }

        internal override bool Is_ShadowHost
        {/* Docs: https://dom.spec.whatwg.org/#element-shadow-host */
#if ENABLE_HTML
            get => shadowRoot != null;
#else
            get => false;
#endif
        }

        internal bool is_potentially_scrollable
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#potentially-scrollable */
            get
            {
                /* An element is potentially scrollable if all of the following conditions are true: */
                /* The element has an associated CSS layout box. */
                if (Box != null)
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

                var bounds = getBoundingClientRect();
                if (bounds.Left < Box.Padding.Left || bounds.Right > Box.Padding.Right)
                    return true;
                if (bounds.Top < Box.Padding.Top || bounds.Bottom > Box.Padding.Bottom)
                    return true;

                return false;
            }
        }
        #endregion

        #region Customizable Steps
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, ReadOnlyMemory<char> Namespace)
        {
            if (tokenListMap.Count > 0)
            {
                if (tokenListMap.TryGetValue(localName, out IAttributeTokenList tokenList))
                {
                    tokenList.run_attribute_change_steps(element, localName, oldValue, newValue, Namespace);
                }
            }


            if (localName.IsCustom)
            {
                return;
            }

            switch (localName.EnumValue)
            {
                case EAttributeName.ID:
                    {
                        ownerDocument.Update_Element_ID(element, oldValue, newValue);
                    }
                    break;
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
            run_attribute_change_steps(this, attr.localName, attr.Value, newValue, attr.namespaceURI.AsMemory());
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
            //AttributeList.Add(attr.Name.ToLowerInvariant(), attr);
            AttributeList.Add(attr.localName, attr);

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
            //AttributeList.Remove(attr.Name.ToLowerInvariant());
            AttributeList.Remove(attr.localName);
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
            //string lowerName = StringCommon.Transform(oldAttr.Name.AsMemory(), UnicodeCommon.To_ASCII_Lower_Alpha);
            //AttributeList[lowerName] = newAttr;

            AttributeList[oldAttr.localName] = newAttr;

            /* 5) Set oldAttr’s element to null. */
            oldAttr.ownerElement = null;

            /* 6) Set newAttr’s element to element. */
            newAttr.ownerElement = this;
        }
        #endregion


        #region Attribute Management
        public AttributeValue getAttribute(AtomicName<EAttributeName> Name)
        {
            if (!AttributeList.TryGetValue(Name, out Attr outAttr))
                return null;

            /* 2) If attr is null, return null. */
            /* 3) Return attr’s value. */
            return outAttr.Value;
        }
        public Attr getAttributeNode(AtomicName<EAttributeName> Name)
        {
            if (!AttributeList.TryGetValue(Name, out Attr outAttr))
                return null;

            return outAttr;
        }


        [CEReactions]
        public void setAttribute(AtomicName<EAttributeName> Name, AttributeValue value)
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                find_attribute(Name, out Attr attr);

                if (attr is null)
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
        public Attr setAttributeNode(Attr attr)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                find_attribute(attr.Name, out Attr oldAttr);
                if (ReferenceEquals(attr, oldAttr))
                    return attr;

                if (oldAttr is object)
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
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                find_attribute(Name, out Attr attr);
                if (attr is object)
                {
                    remove_attribute(attr);
                }
            });
        }
        [CEReactions]
        public Attr removeAttributeNode(Attr attr)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                find_attribute(attr.Name, out Attr outAttr);
                if (attr is object)
                {
                    remove_attribute(attr);
                }

                return attr;
            });
        }


        [CEReactions]
        public bool toggleAttribute(AtomicName<EAttributeName> Name, bool? force = null)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                /* 1) If qualifiedName does not match the Name production in XML, then throw an "InvalidCharacterError" DOMException. */
                if (!XMLCommon.Is_Valid(Name.Name))
                {
                    throw new InvalidCharacterError($"The given name is not a valid XML name: \"{Name.Name}\"");
                }

                /* 2) If the context object is in the HTML namespace and its node document is an HTML document, then set qualifiedName to qualifiedName in ASCII lowercase. */
                /* Dont need to */

                /* 3) Let attribute be the first attribute in the context object’s attribute list whose qualified name is qualifiedName, and null otherwise. */
                find_attribute(Name, out Attr attr);
                /* 4) If attribute is null, then: */
                if (attr == null)
                {
                    /* 1) If force is not given or is true, create an attribute whose local name is qualifiedName, value is the empty string, and node document is the context object’s node document, then append this attribute to the context object, and then return true. */
                    if (!force.HasValue || force.Value)
                    {
                        var newAttr = new Attr(Name, nodeDocument) { Value = AttributeValue.Parse(Name, string.Empty) };
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
            if (AttributeList.TryGetValue(Name, out Attr attr))
                return attr.Value != null;

            return false;
        }
        public bool hasAttribute(AtomicName<EAttributeName> Name, out Attr outAttr)
        {
            if (AttributeList.TryGetValue(Name, out Attr attr))
            {
                outAttr = attr;
                return attr.Value != null;
            }

            outAttr = null;
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
            if (Selector is null)
            {
                throw new DomSyntaxError("Could not parse selector.");
            }
            /* 3) Let elements be context object’s inclusive ancestors that are elements, in reverse tree order. */
            TreeWalker tree = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
            //* 4) For each element in elements, if match a selector against an element, using s, element, and :scope element context object, returns success, return element. [SELECTORS4] *//*
            while (true)
            {
                Node n = tree.parentNode();
                if (n is null)
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
            if (s is null)
            {
                throw new DomSyntaxError($"Could not parse selector: \"{selectors}\"");
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

#if ENABLE_HTML
        private ShadowRoot _shadow_root = null;
        public ShadowRoot shadowRoot
        {
            get
            {
                if (_shadow_root == null)
                    return null;

                if (_shadow_root.Mode == EShadowRootMode.Closed)
                    return null;

                return _shadow_root;
            }

            internal set
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

            /* XXX: Implement checking for these html elements */
            /* 2) If context object’s local name is not a valid custom element name, "article", "aside", "blockquote", "body", "div", "footer", "h1", "h2", "h3", "h4", "h5", "h6", "header", "main" "nav", "p", "section", or "span", then throw a "NotSupportedError" DOMException */
            bool isValidCustomName = HTMLCommon.Is_Valid_Custom_Element_Name(localName.AsMemory());
            if (!isValidCustomName)
            {
                throw new NotSupportedError($"Cannot attach ShadowDOM to invalid custom element");
            }
            /* 3) If context object’s local name is a valid custom element name, or context object’s is value is not null, then:
                    Let definition be the result of looking up a custom element definition given context object’s node document, its namespace, its local name, and its is value.
                    If definition is not null and definition’s disable shadow is true, then throw a "NotSupportedError" DOMException. 
            */
            if (isValidCustomName || !ReferenceEquals(null, is_value))
            {
                var definition = nodeDocument.defaultView.customElements.Lookup(nodeDocument, NamespaceURI, localName, is_value);
                if (definition != null && definition.bDisableShadow)
                {
                    throw new NotSupportedError($"Cannot attach ShadowDOM to custom element whose definition has ShadowDOM disabled");
                }
            }

            /* 4) If context object is a shadow host, then throw an "NotSupportedError" DOMException. */
            if (Is_ShadowHost)
            {
                throw new NotSupportedError("Cannot attach a ShadowDOM to an element which already has one");
            }

            var shadow = new ShadowRoot(this, nodeDocument, init.Mode);
            /* 6) Set context object’s shadow root to shadow. */
            shadowRoot = shadow;
            return shadow;

        }
#endif
        #endregion

        #region ChildNode Implementation
        /// <summary>
        /// Inserts nodes just before node, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void before(params Node[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-before */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viablePreviousSibling = DOMCommon.Get_Nth_Preceeding(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);
                if (viablePreviousSibling == null)
                {
                    viablePreviousSibling = parent.firstChild;
                }
                else
                {
                    viablePreviousSibling = viablePreviousSibling.nextSibling;
                }

                Dom_pre_insert_node(node, parent, viablePreviousSibling);
            });
        }

        /// <summary>
        /// Inserts nodes just before node, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void before(params string[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-before */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viablePreviousSibling = null;// DOMCommon.Get_Nth_Preceeding(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);
                if (viablePreviousSibling == null)
                {
                    viablePreviousSibling = parent.firstChild;
                }
                else
                {
                    viablePreviousSibling = viablePreviousSibling.nextSibling;
                }

                Dom_pre_insert_node(node, parent, viablePreviousSibling);
            });
        }

        /// <summary>
        /// Inserts nodes just after node, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void after(params Node[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-after */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viableNextSibling = DOMCommon.Get_Nth_Following(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);
                if (viableNextSibling == null)
                {
                    viableNextSibling = parent.firstChild;
                }
                else
                {
                    viableNextSibling = viableNextSibling.nextSibling;
                }

                Dom_pre_insert_node(node, parent, viableNextSibling);
            });
        }

        /// <summary>
        /// Inserts nodes just after node, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void after(params string[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-after */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viableNextSibling = null;// DOMCommon.Get_Nth_Following(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);
                if (viableNextSibling == null)
                {
                    viableNextSibling = parent.firstChild;
                }
                else
                {
                    viableNextSibling = viableNextSibling.nextSibling;
                }

                Dom_pre_insert_node(node, parent, viableNextSibling);
            });
        }

        /// <summary>
        /// Replaces node with nodes, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void replaceWith(params Node[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-replacewith */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viableNextSibling = DOMCommon.Get_Nth_Following(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);

                /* 5) If context object’s parent is parent, replace the context object with node within parent. */
                /* (We check this because context object could have been inserted into node.) */
                if (ReferenceEquals(parentNode, parent))
                {
                    parent.replaceChild(this, node);
                }
                else
                {
                    Dom_pre_insert_node(node, parent, viableNextSibling);
                }
            });
        }

        /// <summary>
        /// Replaces node with nodes, while replacing strings in nodes with equivalent Text nodes.
        /// </summary>
        /// <exception cref="HierarchyRequestError">if the constraints of the node tree are violated.</exception>
        [CEReactions]
        public void replaceWith(params string[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-replacewith */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                var parent = parentNode;
                if (parent == null)
                {
                    return;
                }

                Node viableNextSibling = null;// DOMCommon.Get_Nth_Following(this, 1, new FilterNotOneOf(nodes.AsMemory()));
                var node = Dom_convert_nodes_into_node(nodeDocument, nodes);

                /* 5) If context object’s parent is parent, replace the context object with node within parent. */
                /* (We check this because context object could have been inserted into node.) */
                if (ReferenceEquals(parentNode, parent))
                {
                    parent.replaceChild(this, node);
                }
                else
                {
                    Dom_pre_insert_node(node, parent, viableNextSibling);
                }
            });
        }

        /// <summary>
        /// Removes node.
        /// </summary>
        [CEReactions]
        public void remove()
        {/* Docs: https://dom.spec.whatwg.org/#dom-childnode-remove */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                if (parentNode == null)
                {
                    return;
                }

                parentNode.removeChild(this);
            });
        }
        #endregion

        #region Geometry / Client Rects
        public LinkedList<DOMRect> getClientRects()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getclientrects */
            /* XXX: SVG layout box handling needed here */
            LinkedList<DOMRect> Rects = new LinkedList<DOMRect>();
            /* 1) If the element on which it was invoked does not have an associated layout box return an empty sequence and stop this algorithm. */
            if (Box is null)
            {
                return Rects;
            }
            /* 2) If the element has an associated SVG layout box return a sequence containing a single DOMRect object that describes the bounding box of the element as defined by the SVG specification, applying the transforms that apply to the element and its ancestors. */
            /* 3) Return a sequence containing static DOMRect objects in content order, one for each box fragment, describing its border area (including those with a height or width of zero) with the following constraints: */
            if (!Box.InFragmentedFlow)
            {
                /* 1) Apply the transforms that apply to the element and its ancestors. */
                /* 2) If the element on which the method was invoked has a computed value for the display property of table or inline-table include both the table box and the caption box, if any, but not the anonymous container box. */
                /* 3) Replace each anonymous block box with its child box(es) and repeat this until no anonymous block boxes are left in the final list. */

                /* XXX: Transforms must be applied here */
                LinkedList<CssBox> subBoxes = new LinkedList<CssBox>(new CssBox[] { Box });
                if (subBoxes.First.Value is CssAnonymousBox anonymousBox)
                {
                    // Start a loop that goes until we cant find anymore anonymous boxes
                    bool hasAnon = true;
                    while (hasAnon)
                    {
                        hasAnon = false;
                        // Search all items in our current list
                        LinkedListNode<CssBox> node = subBoxes.First;
                        while (node is object)
                        {
                            CssBox child = node.Value;
                            var NextNode = node.Next;
                            // If we find an anonymous box then:
                            // 1) Add all of it's child boxes into the list after it
                            // 2) remove the anonymous-box node from the list
                            if (child is CssAnonymousBox childAnon)
                            {
                                hasAnon = true;
                                foreach (CssBox sub in childAnon.childNodes)
                                {
                                    subBoxes.AddAfter(node, sub);
                                }

                                subBoxes.Remove(node);
                            }

                            node = NextNode;
                        }
                    }
                }


                // Add all subboxes to the list
                foreach (CssBox box in subBoxes)
                {
                    DOMRect Rect;
                    if (box is CssPrincipalBox principal)
                    {
                        Rect = new DOMRect(principal.Border.Left, principal.Border.Top, principal.Border.Width, principal.Border.Height);
                    }
                    else
                    {
                        Rect = new DOMRect(box.Position.X, box.Position.Y, box.Size.Width, box.Size.Height);
                    }
                    Rects.AddLast(Rect);
                }
            }
            else
            {
                // 1) Apply the transforms that apply to the element and its ancestors. 
                // 2) If the element on which the method was invoked has a computed value for the display property of table or inline-table include both the table box and the caption box, if any, but not the anonymous container box. 
                // 3) Replace each anonymous block box with its child box(es) and repeat this until no anonymous block boxes are left in the final list. 

                /*foreach (CssBoxFragment fragment in Box.Fragments)
                {
                    // XXX: Transforms must be applied here
                    if (box is CssAnonymousBox anonymousBox)
                    {
                        foreach (var child in anonymousBox.childNodes)
                        {
                            var r = new DOMRect(child.Border.Left, child.Border.Top, child.Border.Width, child.Border.Height);
                            Rects.AddLast(r);
                        }
                    }
                    else
                    {
                        var r = new DOMRect(box.Border.Left, box.Border.Top, box.Border.Width, box.Border.Height);
                        Rects.AddLast(r);
                    }
                }*/
                throw new NotImplementedException();
            }

            return Rects;
        }

        public DOMRect getBoundingClientRect()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getboundingclientrect */
            return DOMCommon.getBoundingClientRect(getClientRects());
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
                                    scrollingBoxEdgeA = scrollArea.Bottom;
                                    scrollingBoxEdgeB = scrollArea.Top;
                                    scrollingBoxEdgeC = scrollArea.Right;
                                    scrollingBoxEdgeD = scrollArea.Left;

                                    elementEdgeA = elementBoundingBorderBox.Bottom;
                                    elementEdgeB = elementBoundingBorderBox.Top;
                                    elementEdgeC = elementBoundingBorderBox.Right;
                                    elementEdgeD = elementBoundingBorderBox.Left;
                                }
                                break;
                            case CSS.Enums.EOverflowDirection.Downward:
                                {
                                    scrollingBoxEdgeA = scrollArea.Top;
                                    scrollingBoxEdgeB = scrollArea.Bottom;
                                    scrollingBoxEdgeC = scrollArea.Left;
                                    scrollingBoxEdgeD = scrollArea.Right;

                                    elementEdgeA = elementBoundingBorderBox.Top;
                                    elementEdgeB = elementBoundingBorderBox.Bottom;
                                    elementEdgeC = elementBoundingBorderBox.Left;
                                    elementEdgeD = elementBoundingBorderBox.Right;
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
                                    scrollingBoxEdgeA = scrollArea.Bottom;
                                    scrollingBoxEdgeB = scrollArea.Top;
                                    scrollingBoxEdgeC = scrollArea.Left;
                                    scrollingBoxEdgeD = scrollArea.Right;

                                    elementEdgeA = elementBoundingBorderBox.Bottom;
                                    elementEdgeB = elementBoundingBorderBox.Top;
                                    elementEdgeC = elementBoundingBorderBox.Left;
                                    elementEdgeD = elementBoundingBorderBox.Right;
                                }
                                break;
                            case CSS.Enums.EOverflowDirection.Downward:
                                {
                                    scrollingBoxEdgeA = scrollArea.Top;
                                    scrollingBoxEdgeB = scrollArea.Bottom;
                                    scrollingBoxEdgeC = scrollArea.Left;
                                    scrollingBoxEdgeD = scrollArea.Right;

                                    elementEdgeA = elementBoundingBorderBox.Top;
                                    elementEdgeB = elementBoundingBorderBox.Bottom;
                                    elementEdgeC = elementBoundingBorderBox.Left;
                                    elementEdgeD = elementBoundingBorderBox.Right;
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
            if ((position.x ==  scrollBox.ScrollX) && (position.y ==  scrollBox.ScrollY) && !scrollBox.IsScrolling)
                return false;

            Element associatedElement = null;
            if (scrollBox.Owner == null)
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
            while (node != null)
            {
                if (node is Element ancestor)
                {
                    /* 1) If the Document associated with element is not same origin with the Document associated with the element or viewport associated with box, terminate these steps. */
                    if (!UrlOrigin.IsSameOrigin(ancestor.ownerDocument.Origin, ownerDocument.Origin))
                        break;

                    scroll_bounds_into_view(ancestor, getBoundingClientRect(), ancestor.ScrollBox, block, inline, behavior);
                }
            }

            /* Now also do the viewport... */
            scroll_bounds_into_view(null, getBoundingClientRect(), ownerDocument.Viewport.ScrollBox, block, inline, behavior);
        }

        internal void scroll_element(double x, double y, EScrollBehavior behavior = EScrollBehavior.Auto)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#scroll-an-element */
            /* 1) Let box be element’s associated scrolling box. */
            var box = ScrollBox;
            var scrollArea = box.ScrollArea;
            /* 2) If box has rightward overflow direction */
            if (ScrollBox.Overflow_Block == CSS.Enums.EOverflowDirection.Rightward)
            {
                x = MathExt.Max(0, MathExt.Min(x, scrollArea.Width - Box.Padding.Width));
            }
            else
            {
                x = MathExt.Min(0, MathExt.Max(x, Box.Padding.Width - scrollArea.Width));
            }

            /* 3) If box has downward overflow direction */
            if (ScrollBox.Overflow_Inline == CSS.Enums.EOverflowDirection.Downward)
            {
                y = MathExt.Max(0, MathExt.Min(y, scrollArea.Height - Box.Padding.Height));
            }
            else
            {
                y = MathExt.Min(0, MathExt.Max(y, Box.Padding.Height - scrollArea.Height));
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

                if (ownerDocument.defaultView == null)
                    return 0;

                if (isRoot && ownerDocument.Mode == EQuirksMode.Quirks)
                    return 0;

                if (isRoot)
                    return ownerDocument.defaultView.scrollY;

                if (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return ownerDocument.defaultView.scrollY;

                if (Box == null)
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

                if (window == null)
                    return;

                if (isRoot && document.Mode == EQuirksMode.Quirks)
                    return;

                if (isRoot)
                {
                    window.Scroll(window.scrollX, y);
                    return;
                }

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                {
                    window.Scroll(window.scrollX, y);
                    return;
                }

                if (Box == null || ScrollBox == null || !has_overflow)
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
                if (window == null)
                    return 0;

                if (isRoot && document.Mode == EQuirksMode.Quirks)
                    return 0;

                if (isRoot)
                    return window.scrollX;

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return window.scrollX;

                if (Box == null)
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
                if (window == null)
                    return;

                if (isRoot && document.Mode == EQuirksMode.Quirks)
                    return;

                if (isRoot)
                {
                    window.Scroll(x, window.scrollY);
                    return;
                }

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                {
                    window.Scroll(x, window.scrollY);
                    return;
                }

                if (Box == null || ScrollBox == null || !has_overflow)
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
                if (document.Viewport != null && document?.Viewport?.ScrollBox != null)
                {
                    viewportWidth = document.Viewport.Width - (document.Viewport.ScrollBox?.VScrollBar?.Width ?? 0);
                }

                if (isRoot && document.Mode != EQuirksMode.Quirks)
                    return (long)MathExt.Max(viewportWidth, document.Viewport.ScrollBox.ScrollArea.Width);

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return (long)MathExt.Max(viewportWidth, document.Viewport.ScrollBox.ScrollArea.Width);

                if (Box == null)
                    return 0;

                return (long)ScrollBox.ScrollArea.Width;
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
                if (document.Viewport != null && document?.Viewport?.ScrollBox != null)
                {
                    viewportHeight = document.Viewport.Height - (document.Viewport.ScrollBox?.HScrollBar?.Height ?? 0);
                }

                if (isRoot && document.Mode != EQuirksMode.Quirks)
                    return (long)MathExt.Max(viewportHeight, document.Viewport.ScrollBox.ScrollArea.Height);

                if (ReferenceEquals(this, document.body) && document.Mode == EQuirksMode.Quirks && !is_potentially_scrollable)
                    return (long)MathExt.Max(viewportHeight, document.Viewport.ScrollBox.ScrollArea.Height);

                if (Box == null)
                    return 0;

                return (long)ScrollBox.ScrollArea.Height;
            }
        }

        public long clientTop
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-clienttop */
            get
            {
                if (Box == null || Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    return 0;

                /* 2) Return the computed value of the border-top-width property plus the height of any scrollbar rendered between the top padding edge and the top border edge, 
                 * ignoring any transforms that apply to the element and its ancestors. */
                double retVal = Style.Border_Top_Width;
                retVal += (0 ==  ScrollBox.HScrollBar.Top) ? ScrollBox.HScrollBar.Height : 0;
                return (long)retVal;
            }
        }
        public long clientLeft
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-clientleft */
            get
            {
                if (Box == null || Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    return 0;

                /* 2) Return the computed value of the border-left-width property plus the width of any scrollbar rendered between the left padding edge and the left border edge, 
                 * ignoring any transforms that apply to the element and its ancestors. */
                double retVal = Style.Border_Left_Width;
                retVal += (0 ==  ScrollBox.VScrollBar.Left) ? ScrollBox.VScrollBar.Width : 0;
                return (long)retVal;
            }
        }
        public long clientWidth
        {/* https://www.w3.org/TR/cssom-view-1/#dom-element-clientwidth */
            get
            {
                if (Box == null || Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    return 0;

                /* 2) If the element is the root element and the element’s node document is not in quirks mode, 
                 * sor if the element is the HTML body element and the element’s node document is in quirks mode, 
                 * return the viewport width excluding the size of a rendered scroll bar (if any). */
                if ((isRoot && ownerDocument.Mode != EQuirksMode.Quirks) || (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks))
                {
                    double vpSize = ownerDocument.Viewport.Width;
                    vpSize -= (null == ownerDocument.Viewport.ScrollBox.VScrollBar) ? 0 : ownerDocument.Viewport.ScrollBox.VScrollBar.Width;
                    return (long)vpSize;
                }

                /* 3) Return the width of the padding edge excluding the width of any rendered scrollbar between the padding edge and the border edge, ignoring any transforms that apply to the element and its ancestors. */
                double clientSize = Box.Padding.Width;
                clientSize -= (null == ScrollBox.VScrollBar) ? 0 : ScrollBox.VScrollBar.Width;
                return (long)clientSize;
            }
        }
        public long clientHeight
        {
            get
            {
                if (Box == null || Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    return 0;

                /* 2) If the element is the root element and the element’s node document is not in quirks mode, 
                 * or if the element is the HTML body element and the element’s node document is in quirks mode, 
                 * return the viewport height excluding the size of a rendered scroll bar (if any). */
                if ((isRoot && ownerDocument.Mode != EQuirksMode.Quirks) || (ReferenceEquals(this, ownerDocument.body) && ownerDocument.Mode == EQuirksMode.Quirks))
                {
                    double vpSize = ownerDocument.Viewport.Height;
                    vpSize -= (null == ownerDocument.Viewport.ScrollBox.HScrollBar) ? 0 : ownerDocument.Viewport.ScrollBox.HScrollBar.Height;
                    return (long)vpSize;
                }

                /* 3) Return the height of the padding edge excluding the height of any rendered scrollbar between the padding edge and the border edge, ignoring any transforms that apply to the element and its ancestors. */
                double clientSize = Box.Padding.Height;
                clientSize -= (null == ScrollBox.HScrollBar) ? 0 : ScrollBox.HScrollBar.Height;
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

            if (Box == null)
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

            if (Box == null)
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
            if (Box == null)
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
            if (window == null)
            {
                return;
            }

            /* 8) If the element is the root element invoke scroll() on window with scrollX on window as first argument and y as second argument, and terminate these steps. */
            if (isRoot)
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
            if (Box == null || ScrollBox == null || !has_overflow)
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
            if (window == null)
            {
                return;
            }

            /* 8) If the element is the root element invoke scroll() on window with scrollX on window as first argument and y as second argument, and terminate these steps. */
            if (isRoot)
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
            if (Box == null || ScrollBox == null || !has_overflow)
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
