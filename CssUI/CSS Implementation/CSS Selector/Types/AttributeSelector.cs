using CssUI.CSS.Enums;
using CssUI.DOM;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    public class AttributeSelector : SimpleSelector
    {
        readonly NamespacePrefixToken Namespace;
        readonly string Attrib;
        readonly ECssAttributeOperator Operator = ECssAttributeOperator.None;
        readonly string Value = null;

        /// <summary>
        /// </summary>
        /// <param name="Attrib">The attribute name for this selector</param>
        /// <param name="Operator">String token that defines the method of comparison</param>
        /// <param name="Value"></param>
        public AttributeSelector(NamespacePrefixToken Namespace, string Attrib) : base(ECssSimpleSelectorType.AttributeSelector)
        {
            this.Namespace = Namespace;
            this.Attrib = Attrib;
            this.Operator = ECssAttributeOperator.Isset;
        }

        public AttributeSelector(NamespacePrefixToken Namespace, string Attrib, CssToken OperatorToken, string Value) : base(ECssSimpleSelectorType.AttributeSelector)
        {
            this.Namespace = Namespace;
            this.Attrib = Attrib;
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

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E)
        {
            switch (Operator)
            {
                // CSS 2.0 operators
                case ECssAttributeOperator.Isset:// isset
                    {
                        return E.hasAttribute(Attrib);
                    }
                case ECssAttributeOperator.Equals:// equals
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        return string.Compare(Value, E.getAttribute(Attrib)) == 0;
                    }
                case ECssAttributeOperator.PrefixedWith:// equals or prefixed-with
                    {
                        if (!E.hasAttribute(Attrib)) return false;
                        string val = E.getAttribute(Attrib);
                        if (string.Compare(Value, val) == 0) return true;
                        if (val.StartsWith(string.Concat(Value, '-'))) return true;
                        return false;
                    }
                case ECssAttributeOperator.Includes:// list-contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(Attrib)) return false;
                        return E.getAttribute(Attrib).Split(' ').Contains(Value);
                    }
                // Sub-string operators
                case ECssAttributeOperator.StartsWith:// starts-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(Attrib)) return false;
                        return E.getAttribute(Attrib).StartsWith(Value);
                    }
                case ECssAttributeOperator.EndsWith:// ends-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(Attrib)) return false;
                        return E.getAttribute(Attrib).EndsWith(Value);
                    }
                case ECssAttributeOperator.Contains:// contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.hasAttribute(Attrib)) return false;
                        return E.getAttribute(Attrib).Contains(Value);
                    }
                default:
                    throw new CssSelectorException($"Attribute selector operator ({Enum.GetName(typeof(ECssAttributeOperator), Operator)}) logic not implemented!");
            }
        }
    }
}
