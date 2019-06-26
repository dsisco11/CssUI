
namespace CssUI
{
    /// <summary>
    /// Acts like a SPAN element in HTML
    /// </summary>
    public class cssSpanElement : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Span"; } }

        #region Constructors
        public cssSpanElement(IParentElement Parent, string ID = null) : base(Parent, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.INLINE);
            Layout = ELayoutMode.None;
        }
        #endregion
    }
}
