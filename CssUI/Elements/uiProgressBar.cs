using CssUI.CSS;

namespace CssUI
{
    public class uiProgressBar : ScrollableElement
    {
        public override string Default_CSS_TypeName { get { return "ProgressBar"; } }

        #region Values
        float value = 0f;
        public float Value { get { return value; } set { this.value = value; update_bar(); } }
        public cssColor clrProgBar { get { return bar.ColorBackground; } set { bar.ColorBackground = value; } }
        #endregion

        #region Controls
        uiBox bar;
        #endregion

        #region Constructors
        public uiProgressBar(string ID = null) : base(ID)
        {
            Style.User.Padding_Top.Set(3);
            Style.User.Padding_Right.Set(3);
            Style.User.Padding_Bottom.Set(3);
            Style.User.Padding_Left.Set(3);

            Border = new uiBorderStyle(2, 2, 2, 2);

            bar = new uiBox("Bar");
            bar.Style.User.Height.Set(CSSValue.Pct_OneHundred);
            bar.Color = new cssColor(1f, 1f, 1f, 1f);
            Add(bar);
        }
        #endregion

        void update_bar()
        {
            int bWidth = (int)((float)Block.Width * value);
            bar.Style.User.Width.Set(bWidth);
        }
        
    }
}
