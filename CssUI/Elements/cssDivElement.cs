
namespace CssUI
{
    /// <summary>
    /// Acts like a DIV element in HTML
    /// </summary>
    public class cssDivElement : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Div"; } }

        #region Constructors
        public cssDivElement(string ID = null) : base(ID)
        {
            Style.UserRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
