
namespace CssUI
{
    /// <summary>
    /// Just a generic block-level panel element which can have other elements added to it.
    /// </summary>
    public class cssPanelElement : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Panel"; } }

        #region Constructors
        public cssPanelElement(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
