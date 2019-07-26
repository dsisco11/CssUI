using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    /* XXX: Finish this, we need to add the rest of the attribute operators */
    public class AttributeSelector : SimpleSelector
    {
        readonly NamespacePrefixToken Namespace;
        readonly AtomicName<EAttributeName> AttributeName;
        readonly ECssAttributeOperator Operator = ECssAttributeOperator.None;
        readonly string Value = null;

        #region Constructor
        /// <summary>
        /// </summary>
        /// <param name="Attrib">The attribute name for this selector</param>
        /// <param name="Operator">String token that defines the method of comparison</param>
        /// <param name="Value"></param>
        public AttributeSelector(NamespacePrefixToken Namespace, string Attrib) : base(ESimpleSelectorType.AttributeSelector)
        {
            this.Namespace = Namespace;
            this.AttributeName = Attrib;
            this.Operator = ECssAttributeOperator.Isset;
        }

        public AttributeSelector(NamespacePrefixToken Namespace, string Attrib, CssToken OperatorToken, string Value) : base(ESimpleSelectorType.AttributeSelector)
        {
            this.Namespace = Namespace;
            this.AttributeName = Attrib;
            if (Value == null) Value = string.Empty;
            this.Value = Value;

            if (OperatorToken == null || OperatorToken.Type == ECssTokenType.Delim && (OperatorToken as DelimToken).Value == '>')
            {
                this.Operator = ECssAttributeOperator.Isset;
            }
            else
            {
                switch (OperatorToken.Type)
                {
                    case ECssTokenType.Delim:
                        {
                            if ((OperatorToken as DelimToken).Value == '=')
                                this.Operator = ECssAttributeOperator.Equals;
                        }
                        break;
                    case ECssTokenType.Dash_Match:
                        this.Operator = ECssAttributeOperator.PrefixedWith;
                        break;
                    case ECssTokenType.Include_Match:
                        this.Operator = ECssAttributeOperator.Includes;
                        break;
                    case ECssTokenType.Prefix_Match:
                        this.Operator = ECssAttributeOperator.StartsWith;
                        break;
                    case ECssTokenType.Suffix_Match:
                        this.Operator = ECssAttributeOperator.EndsWith;
                        break;
                    case ECssTokenType.Substring_Match:
                        this.Operator = ECssAttributeOperator.Contains;
                        break;
                    default:
                        throw new CssSelectorException("Attribute selector: operator token-to-enum translation not implemented for (", OperatorToken, ")!");
                }
            }
        }
        #endregion

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            switch (Operator)
            {
                // CSS 2.0 operators
                case ECssAttributeOperator.Isset:// isset
                    {
                        return E.hasAttribute(AttributeName);
                    }
                case ECssAttributeOperator.Equals:// equals
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        return StringCommon.Streq(Value.AsSpan(), E.getAttribute(AttributeName).Get_String().AsSpan());
                    }
                case ECssAttributeOperator.PrefixedWith:// equals or prefixed-with
                    {
                        if (!E.hasAttribute(AttributeName)) return false;
                        string val = E.getAttribute(AttributeName).Get_String();
                        if (StringCommon.Streq(Value.AsSpan(),val.AsSpan())) return true;
                        if (val.StartsWith(string.Concat(Value, '-'))) return true;
                        return false;
                    }
                case ECssAttributeOperator.Includes:// list-contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(AttributeName)) return false;
                        return E.getAttribute(AttributeName).Split(' ').Contains(Value);
                    }
                // Sub-string operators
                case ECssAttributeOperator.StartsWith:// starts-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(AttributeName)) return false;
                        return E.getAttribute(AttributeName).StartsWith(Value);
                    }
                case ECssAttributeOperator.EndsWith:// ends-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(AttributeName)) return false;
                        return E.getAttribute(AttributeName).EndsWith(Value);
                    }
                case ECssAttributeOperator.Contains:// contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(AttributeName)) return false;
                        return E.getAttribute(AttributeName).Contains(Value);
                    }
                default:
                    throw new CssSelectorException($"Attribute selector operator ({Enum.GetName(typeof(ECssAttributeOperator), Operator)}) logic not implemented!");
            }
        }
    }
}
