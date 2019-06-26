
namespace CssUI
{
    /// <summary>
    /// A block level container which displays a title text at the top of itself.
    /// </summary>
    public class uiHeaderGroup : cssContainerElement
    {
        public override string Default_CSS_TypeName { get { return "GroupBox"; } }

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
        public uiHeaderGroup(IParentElement Parent, string ID = null) : base(Parent, ID)
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
