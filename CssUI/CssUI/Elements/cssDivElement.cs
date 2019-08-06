using CssUI.CSS;
using CssUI.DOM;

namespace CssUI
{
    /// <summary>
    /// Acts like a DIV element in HTML
    /// </summary>
    public class cssDivElement : cssContainerElement
    {
        public static readonly new string CssTagName = "Div";

        #region Constructors
        public cssDivElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
