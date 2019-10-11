using CssUI.Internal;

namespace CssUI.CSS
{
    [MetaEnum]
    public enum ECssPropertyID : int
    {
        [MetaKeyword("line-height")]
        LineHeight,
        [MetaKeyword("font-family")]
        FontFamily,
        [MetaKeyword("font-weight")]
        FontWeight,
        [MetaKeyword("font-style")]
        FontStyle,
        [MetaKeyword("font-size")]
        FontSize,
        [MetaKeyword("dpi-x")]
        DpiX,
        [MetaKeyword("dpi-y")]
        DpiY,
        [MetaKeyword("scroll-behavior")]
        ScrollBehavior,
        [MetaKeyword("overflow-x")]
        OverflowX,
        [MetaKeyword("overflow-y")]
        OverflowY,
        [MetaKeyword("color")]
        Color,
        /// <summary>
        /// Docs: https://www.w3.org/TR/css-color-3/#opacity
        /// </summary>
        [MetaKeyword("opacity")]
        Opacity,
        [MetaKeyword("border-top-color")]
        BorderTopColor,
        [MetaKeyword("border-right-color")]
        BorderRightColor,
        [MetaKeyword("border-bottom-color")]
        BorderBottomColor,
        [MetaKeyword("border-left-color")]
        BorderLeftColor,
        [MetaKeyword("border-top-style")]
        BorderTopStyle,
        [MetaKeyword("border-right-style")]
        BorderRightStyle,
        [MetaKeyword("border-bottom-style")]
        BorderBottomStyle,
        [MetaKeyword("border-left-style")]
        BorderLeftStyle,
        [MetaKeyword("transform")]
        Transform,
        [MetaKeyword("direction")]
        Direction,
        [MetaKeyword("writing-mode")]
        WritingMode,
        [MetaKeyword("text-align")]
        TextAlign,
        [MetaKeyword("object-fit")]
        ObjectFit,
        [MetaKeyword("display")]
        Display,
        [MetaKeyword("box-sizing")]
        BoxSizing,
        [MetaKeyword("positioning")]
        Positioning,
        /// <summary>
        /// Docs: https://www.w3.org/TR/css3-images/#object-position
        /// </summary>
        [MetaKeyword("object-position")]
        ObjectPosition,
        /*[MetaKeyword("object-position-x")]
        ObjectPositionX,
        [MetaKeyword("object-position-y")]
        ObjectPositionY,*/
        [MetaKeyword("top")]
        Top,
        [MetaKeyword("right")]
        Right,
        [MetaKeyword("bottom")]
        Bottom,
        [MetaKeyword("left")]
        Left,
        [MetaKeyword("width")]
        Width,
        [MetaKeyword("height")]
        Height,
        [MetaKeyword("min-width")]
        MinWidth,
        [MetaKeyword("min-height")]
        MinHeight,
        [MetaKeyword("max-width")]
        MaxWidth,
        [MetaKeyword("max-height")]
        MaxHeight,
        [MetaKeyword("padding-top")]
        PaddingTop,
        [MetaKeyword("padding-right")]
        PaddingRight,
        [MetaKeyword("padding-bottom")]
        PaddingBottom,
        [MetaKeyword("padding-left")]
        PaddingLeft,
        [MetaKeyword("border-top-width")]
        BorderTopWidth,
        [MetaKeyword("border-right-width")]
        BorderRightWidth,
        [MetaKeyword("border-bottom-width")]
        BorderBottomWidth,
        [MetaKeyword("border-left-width")]
        BorderLeftWidth,
        [MetaKeyword("margin-top")]
        MarginTop,
        [MetaKeyword("margin-right")]
        MarginRight,
        [MetaKeyword("margin-bottom")]
        MarginBottom,
        [MetaKeyword("margin-left")]
        MarginLeft,


        MAX_VALUE,
    }
}
