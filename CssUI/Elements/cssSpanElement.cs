using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Acts like a SPAN element in HTML
    /// </summary>
    public class cssSpanElement : cssContainerElement
    {
        public override string TypeName { get { return "Span"; } }

        #region Constructors
        public cssSpanElement(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.INLINE);
            Layout = ELayoutMode.None;
        }
        #endregion
    }
}
