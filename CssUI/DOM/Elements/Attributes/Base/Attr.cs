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

        /// <summary>
        /// The actual value assigned to this attribute by the user
        /// </summary>
        private AttributeValue _value_assigned = null;
        /// <summary>
        /// The value being used for this attribute (an invalid/missing assigned value can cause this value to be assigned a default)
        /// </summary>
        private AttributeValue _value_used = null;

        public Element ownerElement { get; internal set; } = null;
        private WeakReference<AttributeDefinition> _definition = null;

        /// <summary>
        /// True if this attribute doesnt have an assigned value
        /// </summary>
        public bool IsMissingValue => (_value_assigned == null);
        public bool IsInvalidValue { get; private set; } = false;

        /// <summary>
        /// Returns <c>True</c> if this attribute is not Missing or Invalid
        /// </summary>
        public bool IsDefined => !(IsMissingValue || IsInvalidValue);
        #endregion

        #region Accessors

        public AttributeDefinition Definition
        {
            get
            {
                if (_definition == null)
                {
                    _definition = new WeakReference<AttributeDefinition>(AttributeDefinition.Lookup(localName, ownerElement.GetType()));
                }

                if (_definition.TryGetTarget(out AttributeDefinition outDef))
                {
                    return outDef;
                }

                return null;
            }
        }

        public override string nodeValue { get => Value.Data; set => _set_value(AttributeValue.Parse(localName, value)); }
        public override string textContent { get => Value.Data; set => _set_value(AttributeValue.Parse(localName, value)); }
        public override int nodeLength => childNodes.Count;

        /// <summary>
        /// Qualified Name
        /// </summary>
        /// Docs: https://dom.spec.whatwg.org/#concept-attribute-qualified-name
        public string Name { get => prefix==null ? localName.ToString() : string.Concat(prefix, ":", localName); }

        public AttributeValue Value {
            get => _value_used;
            set
            {
                /* 1) If attribute’s element is null, then set attribute’s value to value. */
                if (ownerElement == null)
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
            _value_assigned = newValue;

            if (IsMissingValue)
            {
                _value_used = Definition.MissingValueDefault;
            }
            else
            {
                try
                {
                    var def = Definition;
                    IsInvalidValue = false;
                    def.CheckAndThrow(newValue);

                    if (!(def.LowerRange is null) && newValue.AsRAW() < def.LowerRange)
                    {
                        newValue = def.MissingValueDefault;
                    }

                    if (!(def.UpperRange is null) && newValue.AsRAW() > def.UpperRange)
                    {
                        newValue = def.MissingValueDefault;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    IsInvalidValue = true;
                }

                if (IsInvalidValue)
                {
                    _value_used = Definition.InvalidValueDefault;
                }
                else
                {
                    _value_used = _value_assigned;
                }
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
