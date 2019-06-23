
namespace CssUI
{
    /// <summary>
    /// Acts like a SPAN element in HTML
    /// </summary>
    public class cssSpanElement : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Span"; } }

        #region Constructors
        public cssSpanElement(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.INLINE);
            Layout = ELayoutMode.None;
        }
        #endregion
    }
}
