using CssUI.CSS;

namespace CssUI
{
    public class cssProgressBarElement : cssScrollableElement
    {
        public override string Default_CSS_TypeName { get { return "ProgressBar"; } }

        #region Values
        float value = 0f;
        public float Value { get { return value; } set { this.value = value; update_bar(); } }
        public cssColor clrProgBar { get { return bar.ColorBackground; } set { bar.ColorBackground = value; } }
        #endregion

        #region Controls
        cssBox bar;
        #endregion

        #region Constructors
        public cssProgressBarElement(string ID = null) : base(ID)
        {
            Style.UserRules.Padding_Top.Set(3);
            Style.UserRules.Padding_Right.Set(3);
            Style.UserRules.Padding_Bottom.Set(3);
            Style.UserRules.Padding_Left.Set(3);

            Border = new uiBorderStyle(2, 2, 2, 2);

            bar = new cssBox("Bar");
            bar.Style.UserRules.Height.Set(CssValue.Pct_OneHundred);
            bar.Color = new cssColor(1f, 1f, 1f, 1f);
            Add(bar);
        }
        #endregion

        void update_bar()
        {
            int bWidth = (int)((float)Block.Width * value);
            bar.Style.UserRules.Width.Set(bWidth);
        }
        
    }
}
