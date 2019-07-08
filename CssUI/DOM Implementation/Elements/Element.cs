using CssUI.CSS;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using CssUI.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI.DOM
{
    public class Element : Node
    {
        #region Properties
        public string localName { get; set; }
        public readonly string tagName;

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

        public DOMTokenList classList { get; private set; }

        internal OrderedDictionary<AtomicString, Attr> AttributeList { get; private set; } = new OrderedDictionary<AtomicString, Attr>();
        public NamedNodeMap Attributes { get; private set; }
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
            get => this.getAttribute("id");
            set => this.setAttribute("id", value);
        }

        public string className
        {/* The className attribute must reflect the "class" content attribute. */
            get => this.getAttribute("class");
            set => this.setAttribute("class", value);
        }

        IEnumerable<string> getAttributeNames() => this.AttributeList.Select(a => a.Name);
        public bool hasAttributes() => this.AttributeList.Count > 0;
        #endregion

        #region Constructors
        public Element(Document ownerDocument, string localName)
        {/* Docs: https://dom.spec.whatwg.org/#interface-element */
            this.ownerDocument = ownerDocument;
            this.localName = localName;
            this.classList = new DOMTokenList(this, "class");
        }
        #endregion

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

        #region Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void find_attribute(string qualifiedName, out Attr outAttrib)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            /* Attr attr = null;
             int attrIndex = 0;
             for (int i = 0; i < this.AttributeList.Count; i++)
             {
                 var a = this.AttributeList[i];
                 if (0 == string.Compare(a.Name, qualifiedName))
                 {
                     attr = a;
                     attrIndex = i;
                     break;
                 }
             }*/
            if (!this.AttributeList.TryGetValue(qualifiedName, out Attr attr))
            {
                outAttrib = null;
                return;
            }

            outAttrib = attr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void change_attribute(Attr attr, string oldValue, string newValue)
        {
            /* To change an attribute attribute from an element element to value, run these steps: */
            /* 1) Queue an attribute mutation record for element with attribute’s local name, attribute’s namespace, and attribute’s value. */
            new MutationRecord(EMutationType.Attributes, this, attr.Name, oldValue).QueueRecord();
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
            new MutationRecord(EMutationType.Attributes, this, attr.Name, attr.Value).QueueRecord();
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
            new MutationRecord(EMutationType.Attributes, this, attr.Name, attr.Value).QueueRecord();
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
            new MutationRecord(EMutationType.Attributes, this, oldAttr.Name, oldAttr.Value).QueueRecord(); // REDUNDANT
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

        #region Attribute Stuff
        public string getAttribute(string qualifiedName)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            Attr attr = this.AttributeList[qualifiedName];
            /* 2) If attr is null, return null. */
            /* 3) Return attr’s value. */
            return attr?.Value;
        }
        public Attr getAttributeNode(string qualifiedName)
        {
            qualifiedName = qualifiedName.ToLowerInvariant();
            return this.AttributeList[qualifiedName];
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

        public bool toggleAttribute(string qualifiedName, bool? force = null)
        {
            /* 1) If qualifiedName does not match the Name production in XML, then throw an "InvalidCharacterError" DOMException. */
            /* 2) If the context object is in the HTML namespace and its node document is an HTML document, then set qualifiedName to qualifiedName in ASCII lowercase. */
            qualifiedName = qualifiedName.ToLowerInvariant();
            /* 3) Let attribute be the first attribute in the context object’s attribute list whose qualified name is qualifiedName, and null otherwise. */
            find_attribute(qualifiedName, out Attr attr);
            /* 4) If attribute is null, then: */
            if (ReferenceEquals(attr, null))
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
        public bool hasAttribute(string qualifiedName)
        {
            if (this.AttributeList.TryGetValue(qualifiedName.ToLowerInvariant(), out Attr _))
                return true;

            return false;
        }
        #endregion



        public Element closest(string selectors)
        {
            /* The closest(selectors) method, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var s = CssSelector.Parse_Selector(selectors);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (ReferenceEquals(s, null))
            {
                throw new SyntaxError("Could not parse selector.");
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
                if (s.Query(element))
                    return element;
            }
            /* 5) Return null */
            return null;
        }

        public bool matches(string selectors)
        {
            /* The matches(selectors) and webkitMatchesSelector(selectors) methods, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var s = CssSelector.Parse_Selector(selectors);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (ReferenceEquals(s, null))
            {
                throw new SyntaxError("Could not parse selector.");
            }
            /* 3) Return true if the result of match a selector against an element, using s, element, and :scope element context object, returns success, and false otherwise. [SELECTORS4] */
            return s.Query(this);
        }

        public IEnumerable<Element> getElementsByTagName(string qualifiedName)
        {/* https://dom.spec.whatwg.org/#concept-getelementsbytagname */
            /* 1) If qualifiedName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches only descendant elements. */
            if (0 == string.Compare(qualifiedName, "\u002A"))
            {
                List<Element> collection = new List<Element>();
                var tree = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
                Node n = tree.nextNode();
                while(!ReferenceEquals(n, null))
                {
                    collection.Add((Element)n);
                    n = tree.nextNode();
                }

                return collection;
            }
            /* 2) Otherwise, if root’s node document is an HTML document, return a HTMLCollection rooted at root, whose filter matches the following descendant elements: */
            /* Whose namespace is the HTML namespace and whose qualified name is qualifiedName, in ASCII lowercase. */

            string qn = qualifiedName.ToLowerInvariant();
            List<Element> retList = new List<Element>();
            var treeWalker = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
            Node node = treeWalker.nextNode();
            while (!ReferenceEquals(node, null))
            {
                if (0 == string.Compare(qn, (node as Element).localName.ToLowerInvariant()))
                    retList.Add((Element)node);

                node = treeWalker.nextNode();
            }

            return retList;
        }

        public IEnumerable<Element> getElementsByClassName(string classNames)
        {
            /* The list of elements with class names classNames for a node root is the HTMLCollection returned by the following algorithm: */
            /* 1) Let classes be the result of running the ordered set parser on classNames. */
            var classes = DOMCommon.Parse_Ordered_Set(classNames);
            /* 2) If classes is the empty set, return an empty HTMLCollection. */
            if (classes.Count <= 0)
                return new Element[] { };
            /* 3) Return a HTMLCollection rooted at root, whose filter matches descendant elements that have all their classes in classes. */

            List<Element> retList = new List<Element>();
            var treeWalker = new TreeWalker(this, ENodeFilterMask.SHOW_ELEMENT);
            Node node = treeWalker.nextNode();
            while (!ReferenceEquals(node, null))
            {
                var element = (node as Element);
                bool match = true;
                foreach(string className in element.classList)
                {
                    if (!classes.Contains(className))
                    {
                        match = false;
                        break;
                    }
                }

                if (match) retList.Add((Element)node);

                node = treeWalker.nextNode();
            }
            /* When invoked with the same argument, the same HTMLCollection object may be returned as returned by an earlier call. */

            return retList;
        }
    }
}
