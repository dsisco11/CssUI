using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Represents a UI element which acts like a div or block spacer in HTML which occupys an entire row by itsself and eats up vertical space
    /// </summary>
    public class uiVerticalSpacer : cssElement
    {
        public static new readonly string CssTagName = "VerticalSpacer";

        #region Constructors
        public uiVerticalSpacer(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Style.ImplicitRules.BoxSizing.Set(EBoxSizingMode.ContentBox);
        }
        #endregion
    }
}
