using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public class Attr : Node
    {/* Docs:  */
        public override ENodeType nodeType => Enums.ENodeType.ATTRIBUTE_NODE;
        public override string nodeName => this.Name;

        /// <summary>
        /// Namespace name
        /// </summary>
        public string namespaceURI { get; private set; } = null;
        /// <summary>
        /// Namespace prefix
        /// </summary>
        public string prefix { get; private set; } = null;
        public AtomicName<EAttributeName> localName { get; private set; } = null;
        public override string nodeValue { get => this.Value; set => this.Value = value; }
        public override string textContent { get => this.Value; set => this.Value = value; }
        public override int nodeLength { get => this.childNodes.Count; }
        /// <summary>
        /// Qualified Name
        /// </summary>
        /// Docs: https://dom.spec.whatwg.org/#concept-attribute-qualified-name
        public string Name { get => prefix==null ? localName.ToString() : string.Concat(prefix, ":", localName); }

        private string _value = string.Empty;
        public string Value {
            get => _value;
            set
            {
                /* 1) If attribute’s element is null, then set attribute’s value to value. */
                if (ReferenceEquals(this.ownerElement, null))
                {
                    _value = value;
                }
                else
                {
                    /* 2) Otherwise, change attribute from attribute’s element to value. */
                    this.ownerElement.setAttribute(this.localName, value);
                }
            }
        }

        public Element ownerElement { get; internal set; } = null;

        #region Constructors
        public Attr(AtomicName<EAttributeName> localName, Element Owner, string Namespace = null)
        {
            this.localName = localName;
            this.ownerElement = Owner;
            this.ownerDocument = Owner.ownerDocument;
            this.namespaceURI = Namespace;
        }

        public Attr(AtomicName<EAttributeName> localName, Element Owner, Document document, string Namespace = null)
        {
            this.localName = localName;
            this.ownerElement = Owner;
            this.ownerDocument = document;
            this.namespaceURI = Namespace;
        }

        public Attr(AtomicName<EAttributeName> localName, Document document, string Namespace = null)
        {
            this.localName = localName;
            this.ownerDocument = document;
            this.namespaceURI = Namespace;
        }
        #endregion

        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Attr B))
                return false;

            return this.localName == B.localName && this.Value.Equals(B.Value);
        }


    };
}
