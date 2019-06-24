
namespace CssUI.CSS
{
    /// <summary>
    /// Structure used by the <see cref="ElementPropertySystem"/> to pass block information to various functions when resolving final block values
    /// </summary>
    public class BlockProperties
    {
        public readonly int? Content_Width = null;
        public readonly int? Content_Height = null;
        public int? Intrinsic_Width;
        public int? Intrinsic_Height;
        public double? Intrinsic_Ratio;

        public CssValue Width;
        public CssValue Height;

        public CssValue Top;
        public CssValue Right;
        public CssValue Bottom;
        public CssValue Left;

        public CssValue Margin_Top;
        public CssValue Margin_Right;
        public CssValue Margin_Bottom;
        public CssValue Margin_Left;

        public CssValue Padding_Top;
        public CssValue Padding_Right;
        public CssValue Padding_Bottom;
        public CssValue Padding_Left;

        public int Border_Top;
        public int Border_Right;
        public int Border_Bottom;
        public int Border_Left;

        public BlockProperties()
        {
        }

        public BlockProperties(ElementPropertySystem Style, StyleRuleData Props)
        {
            this.Content_Width = Style.Content_Width;
            this.Content_Height = Style.Content_Height;
            this.Intrinsic_Width = Style.Intrinsic_Width;
            this.Intrinsic_Height = Style.Intrinsic_Height;
            this.Intrinsic_Ratio = Style.Intrinsic_Ratio;

            this.Border_Top = Style.Border_Top_Width;
            this.Border_Right = Style.Border_Right_Width;
            this.Border_Bottom = Style.Border_Bottom_Width;
            this.Border_Left = Style.Border_Left_Width;

            this.Width = Props.Width.Computed;
            this.Height = Props.Height.Computed;

            this.Top = Props.Top.Computed;
            this.Right = Props.Right.Computed;
            this.Bottom = Props.Bottom.Computed;
            this.Left = Props.Left.Computed;

            this.Margin_Top = Props.Margin_Top.Computed;
            this.Margin_Right = Props.Margin_Right.Computed;
            this.Margin_Bottom = Props.Margin_Bottom.Computed;
            this.Margin_Left = Props.Margin_Left.Computed;

            this.Padding_Top = Props.Padding_Top.Computed;
            this.Padding_Right = Props.Padding_Right.Computed;
            this.Padding_Bottom = Props.Padding_Bottom.Computed;
            this.Padding_Left = Props.Padding_Left.Computed;
        }

        public BlockProperties(BlockProperties B)
        {
            this.Content_Width = B.Content_Width;
            this.Content_Height = B.Content_Height;
            this.Intrinsic_Width = B.Intrinsic_Width;
            this.Intrinsic_Height = B.Intrinsic_Height;
            this.Intrinsic_Ratio = B.Intrinsic_Ratio;

            this.Width = B.Width;
            this.Height = B.Height;

            this.Top = B.Top;
            this.Right = B.Right;
            this.Bottom = B.Bottom;
            this.Left = B.Left;

            this.Margin_Top = B.Margin_Top;
            this.Margin_Right = B.Margin_Right;
            this.Margin_Bottom = B.Margin_Bottom;
            this.Margin_Left = B.Margin_Left;

            this.Padding_Top = B.Padding_Top;
            this.Padding_Right = B.Padding_Right;
            this.Padding_Bottom = B.Padding_Bottom;
            this.Padding_Left = B.Padding_Left;

            this.Border_Top = B.Border_Top;
            this.Border_Right = B.Border_Right;
            this.Border_Bottom = B.Border_Bottom;
            this.Border_Left = B.Border_Left;
        }
    }

}
