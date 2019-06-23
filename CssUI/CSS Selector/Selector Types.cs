using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    /// <summary>
    /// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
    /// </summary>
    public abstract class CssSimpleSelector
    {
        public ECssSimpleSelectorType Type { get; protected set; }

        public CssSimpleSelector(ECssSimpleSelectorType Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        abstract public bool Matches(cssElement E);
    }

    /// <summary>
    /// A universal selector matches any element in any namespace
    /// <para>Universal-selectors MUST be seperate from the Type-selector class because they are ignores when calculating the selectors specificity!</para>
    /// </summary>
    public class CssUniversalSelector : CssSimpleSelector
    {
        public CssUniversalSelector() : base(ECssSimpleSelectorType.UniversalSelector)
        {
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return true;
        }
    }

    /// <summary>
    /// A type-selector matches an elements <see cref="cssElement.TypeName"/>
    /// </summary>
    public class CssTypeSelector : CssSimpleSelector
    {
        /// <summary>
        /// The namespace to restrict this type matcher too.
        /// <para>'*' if it matches ANY namespace</para>
        /// <para></para>
        /// </summary>
        readonly string Namespace;
        readonly string TypeName;

        public CssTypeSelector(string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            if (string.Compare("*", TypeName) == 0) throw new CssParserError("Caught attempt to create a TypeSelector with the UniversalSelector symbol(*)!");
            this.Namespace = "*";// Match ANY namespace
            this.TypeName = TypeName;
        }

        public CssTypeSelector(string Namespace, string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace;
            this.TypeName = TypeName;
        }

        public CssTypeSelector(NamespacePrefixToken Namespace, string TypeName) : base(ECssSimpleSelectorType.TypeSelector)
        {
            this.Namespace = Namespace?.Value;
            this.TypeName = TypeName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            // Note: not even sure the UI system will ever HAVE the concept of "namespaces" as those are really for web domain names
            if (Namespace != null)
            {
                if (string.Compare(Namespace, "*") != 0)
                {// Perform namespace matching
                }
            }
            else
            {// ONLY match null namespaces (what?!?)
            }

            return string.Compare(TypeName, E.TypeName, true) == 0;
        }
    }

    public enum ECssAttributeOperator
    {
        /// <summary></summary>
        None,
        /// <summary>Matches if the attribute has ANY set value</summary>
        Isset,
        /// <summary>Matches if the attribute is exactly equal to our value</summary>
        Equals,
        /// <summary>Matches values equal to our own or which are prefixed with "{ourValue}-" </summary>
        PrefixedWith,
        /// <summary>Matches if our value is present in the attribute value when viewed as a space-seperated list of values</summary>
        Includes,
        /// <summary>Substring starts with</summary>
        StartsWith,
        /// <summary>Substring ends with</summary>
        EndsWith,
        /// <summary>Substring contains</summary>
        Contains,
    }

    public class CssAttributeSelector : CssSimpleSelector
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
        public CssAttributeSelector(NamespacePrefixToken Namespace, string Attrib) : base(ECssSimpleSelectorType.AttributeSelector)
        {
            this.Namespace = Namespace;
            this.Attrib = Attrib;
            this.Operator = ECssAttributeOperator.Isset;
        }

        public CssAttributeSelector(NamespacePrefixToken Namespace, string Attrib, CssToken OperatorToken, string Value) : base(ECssSimpleSelectorType.AttributeSelector)
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
                        throw new CssSelectorError("Attribute selector: operator token-to-enum translation not implemented for (", OperatorToken, ")!");
                }
            }
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Operator)
            {
                // CSS 2.0 operators
                case  ECssAttributeOperator.Isset:// isset
                    {
                        return E.Has_Attribute(Attrib);
                    }
                case ECssAttributeOperator.Equals:// equals
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        return string.Compare(Value, E.Get_Attribute<string>(Attrib)) == 0;
                    }
                case ECssAttributeOperator.PrefixedWith:// equals or prefixed-with
                    {
                        if (!E.Has_Attribute(Attrib)) return false;
                        string val = E.Get_Attribute<string>(Attrib);
                        if (string.Compare(Value, val) == 0) return true;
                        if (val.StartsWith(string.Concat(Value, '-'))) return true;
                        return false;
                    }
                case ECssAttributeOperator.Includes:// list-contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.Has_Attribute(Attrib)) return false;
                        return E.Get_Attribute<string>(Attrib).Split(' ').Contains(Value);
                    }
                // Sub-string operators
                case ECssAttributeOperator.StartsWith:// starts-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.Has_Attribute(Attrib)) return false;
                        return E.Get_Attribute<string>(Attrib).StartsWith(Value);
                    }
                case ECssAttributeOperator.EndsWith:// ends-with
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.Has_Attribute(Attrib)) return false;
                        return E.Get_Attribute<string>(Attrib).EndsWith(Value);
                    }
                case ECssAttributeOperator.Contains:// contains
                    {
                        if (string.IsNullOrEmpty(Value)) return false;
                        if (!E.Has_Attribute(Attrib)) return false;
                        return E.Get_Attribute<string>(Attrib).Contains(Value);
                    }
                default:
                    throw new CssSelectorError("Attribute selector operator (", Enum.GetName(typeof(ECssAttributeOperator), Operator), ") logic not implemented!");
            }
        }
    }

    public class CssClassSelector : CssSimpleSelector
    {
        readonly string ClassName;

        public CssClassSelector(string ClassName) : base(ECssSimpleSelectorType.ClassSelector)
        {
            this.ClassName = ClassName;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return E.Has_Class(ClassName);
        }
    }

    public class CssIDSelector : CssSimpleSelector
    {
        readonly string MatchID;

        public CssIDSelector(string MatchID) : base(ECssSimpleSelectorType.IDSelector)
        {
            this.MatchID = MatchID;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            return string.Compare(E.ID.ToLowerInvariant(), MatchID) == 0;
        }
    }

    public class CssPseudoClassSelector : CssSimpleSelector
    {
        protected readonly string Name;

        public CssPseudoClassSelector(string PseudoClass) : base(ECssSimpleSelectorType.PseudoClassSelector)
        {
            this.Name = PseudoClass;
        }

        public static CssPseudoClassSelector Create_Function(string Name, List<CssToken> Args = null)
        {
            if (string.Compare("not", Name)==0)
            {
                return new CssPseudoClassSelectorNegationFunction(Name, new CssTokenStream(Args));
            }
            else if (Name.StartsWith("nth-"))
            {
                return new CssPseudoClassSelectorAnBFunction(Name, new CssTokenStream(Args));
            }

            return new CssPseudoClassSelectorFunction(Name, Args);
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                case "hover":
                    return E.IsMouseOver;
                case "active":
                    return E.IsActive;
                case "focus":
                    return E.HasFocus;
                case "enabled":
                    return E.IsEnabled;
                case "disabled":
                    return !E.IsEnabled;
                case "drop":
                    return E.AcceptsDragDrop;
                case "checked":
                    return (E.Has_Attribute("checked") && E.Get_Attribute<bool>("checked") == true);
                case "indeterminate":
                    {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#indeterminate
                        return (E.Has_Attribute("checked") && E.Get_Attribute<int>("checked") == 2);
                    }
                case "empty":
                    return E.IsEmpty;
                case "root":
                    return (E.Root == null);
                default:
                    throw new CssSelectorError("Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

    public class CssPseudoElementSelector : CssSimpleSelector
    {
        protected readonly string Name;

        public CssPseudoElementSelector(string Name) : base(ECssSimpleSelectorType.PseudoElementSelector)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                default:
                    throw new CssSelectorError("Selector pseudo-element (", Name, ") logic not implemented!");
            }
        }
    }

}
