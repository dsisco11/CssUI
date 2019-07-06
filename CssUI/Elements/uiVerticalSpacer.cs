using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Represents a UI element which acts like a div or block spacer in HTML which occupys an entire row by itsself and eats up vertical space
    /// </summary>
    public class uiVerticalSpacer : cssElement
    {
        public override string TypeName { get { return "VerticalSpacer"; } }

        #region Constructors
        public uiVerticalSpacer(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Style.ImplicitRules.BoxSizing.Set(EBoxSizingMode.ContentBox);
        }
        #endregion
    }
}
