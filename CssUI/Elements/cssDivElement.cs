
namespace CssUI
{
    /// <summary>
    /// Acts like a DIV element in HTML
    /// </summary>
    public class cssDivElement : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Div"; } }

        #region Constructors
        public cssDivElement(IParentElement Parent, string ID = null) : base(Parent, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
