
namespace CssUI
{
    /// <summary>
    /// Acts like a DIV element in HTML
    /// </summary>
    public class cssDivElement : cssContainerElement
    {
        public override string TypeName { get { return "Div"; } }

        #region Constructors
        public cssDivElement(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
