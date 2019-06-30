using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// A block level container which displays a title text at the top of itself.
    /// </summary>
    public class uiHeaderGroup : cssContainerElement
    {
        public override string TypeName { get { return "GroupBox"; } }

        #region Components
        cssTextElement text;
        #endregion

        #region Accessors
        public string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }
        #endregion

        #region Constructors
        public uiHeaderGroup(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;

            text = new cssTextElement(this);
            text.Style.ImplicitRules.Display.Set(EDisplayMode.BLOCK);
            //text.TextFont = new System.Drawing.Font(text.TextFont.FontFamily, 20);
        }
        #endregion
    }
}
