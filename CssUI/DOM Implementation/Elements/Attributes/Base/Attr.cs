using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    public class Attr : Node
    {/* Docs:  */
        #region Properties
        public override ENodeType nodeType => ENodeType.ATTRIBUTE_NODE;
        public override string nodeName => Name;

        /// <summary>
        /// Namespace name
        /// </summary>
        public string namespaceURI { get; private set; } = null;
        /// <summary>
        /// Namespace prefix
        /// </summary>
        public string prefix { get; private set; } = null;
        public AtomicName<EAttributeName> localName { get; private set; } = null;

        private AttributeValue _value = null;
        public Element ownerElement { get; internal set; } = null;
        private WeakReference<AttributeDefinition> _definition = null;

        private bool Is_MissingValue = false;
        private bool Is_InvalidValue = false;
        #endregion

        #region Accessors

        public AttributeDefinition Definition
        {
            get
            {
                if (ReferenceEquals(null, _definition))
                {
                    _definition = new WeakReference<AttributeDefinition>(AttributeDefinition.Lookup(localName));
                }

                if (_definition.TryGetTarget(out AttributeDefinition outDef))
                    return outDef;

                return null;
            }
        }

        public override string nodeValue { get => Value.Data; set => _set_value(AttributeValue.Parse(value, Definition)); }
        public override string textContent { get => Value.Data; set => _set_value(AttributeValue.Parse(value, Definition)); }
        public override int nodeLength { get => childNodes.Count; }

        /// <summary>
        /// Qualified Name
        /// </summary>
        /// Docs: https://dom.spec.whatwg.org/#concept-attribute-qualified-name
        public string Name { get => prefix==null ? localName.ToString() : string.Concat(prefix, ":", localName); }

        public AttributeValue Value {
            get => _value;
            set
            {
                /* 1) If attribute’s element is null, then set attribute’s value to value. */
                if (ReferenceEquals(null, ownerElement))
                {
                    _set_value(value);
                }
                else
                {
                    /* 2) Otherwise, change attribute from attribute’s element to value. */
                    ownerElement.setAttribute(localName, value);
                }
            }
        }
        #endregion

        #region Internal Utilities
        private void _set_value(AttributeValue newValue)
        {
            Is_MissingValue = ReferenceEquals(null, newValue) || ReferenceEquals(null, newValue.Data);
            Is_InvalidValue = false;

            try
            {
                Definition.CheckAndThrow(newValue);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Is_InvalidValue = true;
            }

            if (Is_MissingValue)
            {
                _value = Definition.MissingValueDefault;
            }
            else if (Is_InvalidValue)
            {
                _value = Definition.InvalidValueDefault;
            }
            else
            {
                _value = newValue;
            }
        }
        #endregion

        #region Constructors
        public Attr(AtomicName<EAttributeName> localName, Element Owner, string Namespace = null)
        {
            this.localName = localName;
            this.ownerElement = Owner;
            this.nodeDocument = Owner.nodeDocument;
            this.namespaceURI = Namespace;
        }

        public Attr(AtomicName<EAttributeName> localName, Element Owner, Document document, string Namespace = null)
        {
            this.localName = localName;
            this.ownerElement = Owner;
            this.nodeDocument = document;
            this.namespaceURI = Namespace;
        }

        public Attr(AtomicName<EAttributeName> localName, Document document, string Namespace = null)
        {
            this.localName = localName;
            this.nodeDocument = document;
            this.namespaceURI = Namespace;
        }
        #endregion

        public override bool Equals(object obj)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-equals */
            if (!base.Equals(obj))
                return false;

            if (!(obj is Attr B))
                return false;

            return localName.Equals(B.localName) && Value.Equals(B.Value);
        }
    };
}
