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
        protected readonly string Name;

        public PseudoClassSelector(string PseudoClass) : base(ESimpleSelectorType.PseudoClassSelector)
        {
            this.Name = PseudoClass;
        }

        public static PseudoClassSelector Create_Function(string Name, List<CssToken> Args = null)
        {
            if (string.Compare("not", Name) == 0)
            {
                return new PseudoClassSelectorNegationFunction(Name, new TokenStream(Args));
            }
            else if (Name.StartsWith("nth-"))
            {
                return new PseudoClassSelectorAnBFunction(Name, new TokenStream(Args));
            }

            return new PseudoClassSelectorFunction(Name, Args);
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
                    return (E.hasAttribute("checked") && !string.IsNullOrEmpty(E.getAttribute("checked")) == true);
                case "indeterminate":
                    {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#indeterminate
                        return (E.hasAttribute("checked") && string.Compare(E.getAttribute("checked"), "2")==0);
                    }
                case "empty":
                    return !E.hasChildNodes();
                case "root":
                    {
                        var root = E.getRootNode();
                        return (ReferenceEquals(null, root) || ReferenceEquals(E, root));
                    }
                default:
                    throw new CssSelectorException("Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

}
