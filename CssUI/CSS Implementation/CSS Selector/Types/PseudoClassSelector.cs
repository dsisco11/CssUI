using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    public class PseudoClassSelector : SimpleSelector
    {
        /// <summary>
        /// The pseudo class name
        /// </summary>
        protected readonly string Name;

        public PseudoClassSelector(string PseudoClass) : base(ESimpleSelectorType.PseudoClassSelector)
        {
            Name = PseudoClass;
        }

        public static PseudoClassSelector Create_Function(string Name, CssToken[] Args = null)
        {
            if (Name.Equals("not"))
            {
                return new PseudoClassSelectorNegationFunction(Name, new TokenStream(Args));
            }
            else if (Name.StartsWith("nth-"))
            {
                return new PseudoClassSelectorAnBFunction(Name, new TokenStream(Args));
            }

            return new PseudoClassSelectorFunction(Name, new List<CssToken>(Args));
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            switch (Name)
            {
                case "hover":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "active":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "focus":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "enabled":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "disabled":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "drop":
                    throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
                case "checked":
                    {
                        return (E.hasAttribute(EAttributeName.Checked, out Attr outChecked) && !string.IsNullOrEmpty(outChecked.Value.Get_String()) == true);
                    }
                case "indeterminate":
                    {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#indeterminate
                        return (E.hasAttribute(EAttributeName.Checked, out Attr outChecked) && outChecked.Value.Get_String().Equals("2"));
                    }
                case "empty":
                    return !E.hasChildNodes();
                case "root":
                    {
                        var root = E.getRootNode();
                        return (root == null || ReferenceEquals(E, root));
                    }
                default:
                    throw new CssSelectorException("Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

}
