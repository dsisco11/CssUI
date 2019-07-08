using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public class Attr : Node
    {
        public override ENodeType nodeType => Enums.ENodeType.ATTRIBUTE_NODE;
        public override string nodeName => this.Name;
        private string localName => this.Name;
        public override string nodeValue { get => this.Value; set => this.Value = value; }
        public override string textContent { get => this.Value; set => this.Value = value; }
        /// <summary>
        /// Same as the localName
        /// </summary>
        public string Name { get; internal set; }

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
        public Attr(string Name, Element Owner)
        {
            this.Name = Name;
            this.ownerElement = Owner;
            this.ownerDocument = Owner.ownerDocument;
        }

        public Attr(string Name, Element Owner, Document document)
        {
            this.Name = Name;
            this.ownerElement = Owner;
            this.ownerDocument = document;
        }

        public Attr(string Name, Document document)
        {
            this.Name = Name;
            this.ownerDocument = document;
        }
        #endregion

        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Attr B))
                return false;

            return 0 == string.Compare(this.localName, B.localName) && 0 == string.Compare(this.Value, B.Value);
        }


    };
}
