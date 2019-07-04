using CssUI.CSS;

namespace CssUI
{
    public class cssProgressBarElement : cssScrollableElement
    {
        public override string TypeName { get { return "ProgressBar"; } }

        #region Values
        float value = 0f;
        public float Value { get { return value; } set { this.value = value; update_bar(); } }
        public cssColor clrProgBar { get { return bar.ColorBackground; } set { bar.ColorBackground = value; } }
        #endregion

        #region Controls
        cssBox bar;
        #endregion

        #region Constructors
        public cssProgressBarElement(IParentElement Parent, string className = null, string ID = null) : base(Parent, className, ID)
        {
            Style.ImplicitRules.Padding_Top.Set(3);
            Style.ImplicitRules.Padding_Right.Set(3);
            Style.ImplicitRules.Padding_Bottom.Set(3);
            Style.ImplicitRules.Padding_Left.Set(3);

            Border = new uiBorderStyle(2, 2, 2, 2);

            bar = new cssBox(this);
            bar.Style.ImplicitRules.Height.Set(CssValue.Pct_OneHundred);
            bar.Color = new cssColor(1f, 1f, 1f, 1f);
        }
        #endregion

        void update_bar()
        {
            int bWidth = (int)((float)Box.Width * value);
            bar.Style.ImplicitRules.Width.Set(bWidth);
        }
        
    }
}
