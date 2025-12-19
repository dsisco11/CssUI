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

        #region Flexbox Properties
        /// <summary>
        /// Specifies how flex items are placed in the flex container.
        /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-direction
        /// </summary>
        [MetaKeyword("flex-direction")]
        FlexDirection,
        /// <summary>
        /// Controls whether the flex container is single-line or multi-line.
        /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-wrap
        /// </summary>
        [MetaKeyword("flex-wrap")]
        FlexWrap,
        /// <summary>
        /// Specifies the flex grow factor.
        /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-grow
        /// </summary>
        [MetaKeyword("flex-grow")]
        FlexGrow,
        /// <summary>
        /// Specifies the flex shrink factor.
        /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-shrink
        /// </summary>
        [MetaKeyword("flex-shrink")]
        FlexShrink,
        /// <summary>
        /// Specifies the initial main size of a flex item.
        /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-basis
        /// </summary>
        [MetaKeyword("flex-basis")]
        FlexBasis,
        /// <summary>
        /// Controls the order of flex/grid items.
        /// Docs: https://www.w3.org/TR/css-display-3/#propdef-order
        /// </summary>
        [MetaKeyword("order")]
        Order,
        #endregion

        #region Grid Properties
        /// <summary>
        /// Specifies the sizing of grid columns.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-columns
        /// </summary>
        [MetaKeyword("grid-template-columns")]
        GridTemplateColumns,
        /// <summary>
        /// Specifies the sizing of grid rows.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-rows
        /// </summary>
        [MetaKeyword("grid-template-rows")]
        GridTemplateRows,
        /// <summary>
        /// Specifies the sizing of implicitly-created columns.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-columns
        /// </summary>
        [MetaKeyword("grid-auto-columns")]
        GridAutoColumns,
        /// <summary>
        /// Specifies the sizing of implicitly-created rows.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-rows
        /// </summary>
        [MetaKeyword("grid-auto-rows")]
        GridAutoRows,
        /// <summary>
        /// Controls the auto-placement algorithm.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-flow
        /// </summary>
        [MetaKeyword("grid-auto-flow")]
        GridAutoFlow,
        /// <summary>
        /// Specifies a grid item's start position in the column direction.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-start
        /// </summary>
        [MetaKeyword("grid-column-start")]
        GridColumnStart,
        /// <summary>
        /// Specifies a grid item's end position in the column direction.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-end
        /// </summary>
        [MetaKeyword("grid-column-end")]
        GridColumnEnd,
        /// <summary>
        /// Specifies a grid item's start position in the row direction.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-start
        /// </summary>
        [MetaKeyword("grid-row-start")]
        GridRowStart,
        /// <summary>
        /// Specifies a grid item's end position in the row direction.
        /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-end
        /// </summary>
        [MetaKeyword("grid-row-end")]
        GridRowEnd,
        #endregion

        #region Alignment Properties
        /// <summary>
        /// Aligns flex lines or grid tracks within the container.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-content
        /// </summary>
        [MetaKeyword("align-content")]
        AlignContent,
        /// <summary>
        /// Justifies content along the main/inline axis.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-content
        /// </summary>
        [MetaKeyword("justify-content")]
        JustifyContent,
        /// <summary>
        /// Sets the default align-self for all items.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-items
        /// </summary>
        [MetaKeyword("align-items")]
        AlignItems,
        /// <summary>
        /// Overrides align-items for a specific item.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-self
        /// </summary>
        [MetaKeyword("align-self")]
        AlignSelf,
        /// <summary>
        /// Sets the default justify-self for all items.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-items
        /// </summary>
        [MetaKeyword("justify-items")]
        JustifyItems,
        /// <summary>
        /// Overrides justify-items for a specific item.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-self
        /// </summary>
        [MetaKeyword("justify-self")]
        JustifySelf,
        /// <summary>
        /// Sets the gap between rows.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-row-gap
        /// </summary>
        [MetaKeyword("row-gap")]
        RowGap,
        /// <summary>
        /// Sets the gap between columns.
        /// Docs: https://www.w3.org/TR/css-align-3/#propdef-column-gap
        /// </summary>
        [MetaKeyword("column-gap")]
        ColumnGap,
        #endregion

        #region Fragmentation Properties
        /// <summary>
        /// Specifies whether a box should break before the principal box.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-before
        /// </summary>
        [MetaKeyword("break-before")]
        BreakBefore,
        /// <summary>
        /// Specifies whether a box should break after the principal box.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-after
        /// </summary>
        [MetaKeyword("break-after")]
        BreakAfter,
        /// <summary>
        /// Specifies whether a box should avoid breaks inside.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-inside
        /// </summary>
        [MetaKeyword("break-inside")]
        BreakInside,
        /// <summary>
        /// Specifies the minimum number of lines in a block container 
        /// that must be left before a fragmentation break.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-orphans
        /// </summary>
        [MetaKeyword("orphans")]
        Orphans,
        /// <summary>
        /// Specifies the minimum number of lines in a block container 
        /// that must be left after a fragmentation break.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-widows
        /// </summary>
        [MetaKeyword("widows")]
        Widows,
        /// <summary>
        /// Specifies whether box decorations are cloned or sliced at fragment breaks.
        /// Docs: https://www.w3.org/TR/css-break-3/#propdef-box-decoration-break
        /// </summary>
        [MetaKeyword("box-decoration-break")]
        BoxDecorationBreak,
        #endregion


        MAX_VALUE,
    }
}

