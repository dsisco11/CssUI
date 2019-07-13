using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Acts like a SPAN element in HTML
    /// </summary>
    public class cssSpanElement : cssContainerElement
    {
        public static readonly new string CssTagName = "span";

        #region Constructors
        public cssSpanElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.INLINE);
            Layout = ELayoutMode.None;
        }
        #endregion
    }
}
