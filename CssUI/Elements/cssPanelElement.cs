
namespace CssUI
{
    /// <summary>
    /// Just a generic block-level panel element which can have other elements added to it.
    /// </summary>
    public class cssPanelElement : cssContainerElement
    {
        public override string TypeName { get { return "Panel"; } }

        #region Constructors
        public cssPanelElement(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
