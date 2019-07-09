using CssUI.CSS.Enums;
using CssUI.DOM;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    public class PseudoClassSelector : SimpleSelector
    {
        protected readonly string Name;

        public PseudoClassSelector(string PseudoClass) : base(ECssSimpleSelectorType.PseudoClassSelector)
        {
            this.Name = PseudoClass;
        }

        public static PseudoClassSelector Create_Function(string Name, List<CssToken> Args = null)
        {
            if (string.Compare("not", Name) == 0)
            {
                return new PseudoClassSelectorNegationFunction(Name, new CssTokenStream(Args));
            }
            else if (Name.StartsWith("nth-"))
            {
                return new PseudoClassSelectorAnBFunction(Name, new CssTokenStream(Args));
            }

            return new PseudoClassSelectorFunction(Name, Args);
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E)
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
