
namespace CssUI
{
    /// <summary>
    /// Represents a UI element which acts like a div or block spacer in HTML which occupys an entire row by itsself and eats up vertical space
    /// </summary>
    public class uiVerticalSpacer : cssElement
    {
        public override string Default_CSS_TypeName { get { return "VerticalSpacer"; } }

        #region Constructors
        public uiVerticalSpacer(IParentElement Parent, string ID = null) : base(Parent, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Style.ImplicitRules.BoxSizing.Set(EBoxSizingMode.CONTENT);
        }
        #endregion
    }
}
