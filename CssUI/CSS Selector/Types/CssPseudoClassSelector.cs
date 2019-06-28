using System.Collections.Generic;

namespace CssUI.CSS
{

    public class CssPseudoClassSelector : CssSimpleSelector
    {
        protected readonly string Name;

        public CssPseudoClassSelector(string PseudoClass) : base(ECssSimpleSelectorType.PseudoClassSelector)
        {
            this.Name = PseudoClass;
        }

        public static CssPseudoClassSelector Create_Function(string Name, List<CssToken> Args = null)
        {
            if (string.Compare("not", Name) == 0)
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
                    throw new CssSelectorException("Selector pseudo-class (", Name, ") logic not implemented!");
            }
        }
    }

}
