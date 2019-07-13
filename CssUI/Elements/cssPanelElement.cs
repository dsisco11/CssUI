using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Just a generic block-level panel element which can have other elements added to it.
    /// </summary>
    public class cssPanelElement : cssContainerElement
    {
        public static readonly new string CssTagName = "Panel";

        #region Constructors
        public cssPanelElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
