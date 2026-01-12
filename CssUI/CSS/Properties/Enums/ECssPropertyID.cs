using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum ECssPropertyID : int
{
    [EnumData("line-height")]
    LineHeight,
    [EnumData("font-family")]
    FontFamily,
    [EnumData("font-weight")]
    FontWeight,
    [EnumData("font-style")]
    FontStyle,
    [EnumData("font-size")]
    FontSize,
    [EnumData("dpi-x")]
    DpiX,
    [EnumData("dpi-y")]
    DpiY,
    [EnumData("scroll-behavior")]
    ScrollBehavior,
    [EnumData("overflow-x")]
    OverflowX,
    [EnumData("overflow-y")]
    OverflowY,
    [EnumData("color")]
    Color,
    /// <summary>
    /// Docs: https://www.w3.org/TR/css-color-3/#opacity
    /// </summary>
    [EnumData("opacity")]
    Opacity,
    [EnumData("border-top-color")]
    BorderTopColor,
    [EnumData("border-right-color")]
    BorderRightColor,
    [EnumData("border-bottom-color")]
    BorderBottomColor,
    [EnumData("border-left-color")]
    BorderLeftColor,
    [EnumData("border-top-style")]
    BorderTopStyle,
    [EnumData("border-right-style")]
    BorderRightStyle,
    [EnumData("border-bottom-style")]
    BorderBottomStyle,
    [EnumData("border-left-style")]
    BorderLeftStyle,
    [EnumData("transform")]
    Transform,
    [EnumData("direction")]
    Direction,
    [EnumData("writing-mode")]
    WritingMode,
    [EnumData("text-align")]
    TextAlign,
    [EnumData("object-fit")]
    ObjectFit,
    [EnumData("display")]
    Display,
    /// <summary>
    /// Docs: https://www.w3.org/TR/CSS2/visuren.html#propdef-float
    /// </summary>
    [EnumData("float")]
    Float,
    /// <summary>
    /// Docs: https://www.w3.org/TR/CSS2/visuren.html#propdef-clear
    /// </summary>
    [EnumData("clear")]
    Clear,
    [EnumData("box-sizing")]
    BoxSizing,
    [EnumData("positioning")]
    Positioning,
    /// <summary>
    /// Docs: https://www.w3.org/TR/css3-images/#object-position
    /// </summary>
    [EnumData("object-position")]
    ObjectPosition,
    /*[EnumData("object-position-x")]
    ObjectPositionX,
    [EnumData("object-position-y")]
    ObjectPositionY,*/
    [EnumData("top")]
    Top,
    [EnumData("right")]
    Right,
    [EnumData("bottom")]
    Bottom,
    [EnumData("left")]
    Left,
    [EnumData("width")]
    Width,
    [EnumData("height")]
    Height,
    [EnumData("min-width")]
    MinWidth,
    [EnumData("min-height")]
    MinHeight,
    [EnumData("max-width")]
    MaxWidth,
    [EnumData("max-height")]
    MaxHeight,
    [EnumData("padding-top")]
    PaddingTop,
    [EnumData("padding-right")]
    PaddingRight,
    [EnumData("padding-bottom")]
    PaddingBottom,
    [EnumData("padding-left")]
    PaddingLeft,
    [EnumData("border-top-width")]
    BorderTopWidth,
    [EnumData("border-right-width")]
    BorderRightWidth,
    [EnumData("border-bottom-width")]
    BorderBottomWidth,
    [EnumData("border-left-width")]
    BorderLeftWidth,
    [EnumData("margin-top")]
    MarginTop,
    [EnumData("margin-right")]
    MarginRight,
    [EnumData("margin-bottom")]
    MarginBottom,
    [EnumData("margin-left")]
    MarginLeft,

    #region Flexbox Properties
    /// <summary>
    /// Specifies how flex items are placed in the flex container.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-direction
    /// </summary>
    [EnumData("flex-direction")]
    FlexDirection,
    /// <summary>
    /// Controls whether the flex container is single-line or multi-line.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-wrap
    /// </summary>
    [EnumData("flex-wrap")]
    FlexWrap,
    /// <summary>
    /// Specifies the flex grow factor.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-grow
    /// </summary>
    [EnumData("flex-grow")]
    FlexGrow,
    /// <summary>
    /// Specifies the flex shrink factor.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-shrink
    /// </summary>
    [EnumData("flex-shrink")]
    FlexShrink,
    /// <summary>
    /// Specifies the initial main size of a flex item.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-basis
    /// </summary>
    [EnumData("flex-basis")]
    FlexBasis,
    /// <summary>
    /// Controls the order of flex/grid items.
    /// Docs: https://www.w3.org/TR/css-display-3/#propdef-order
    /// </summary>
    [EnumData("order")]
    Order,
    #endregion

    #region Grid Properties
    /// <summary>
    /// Specifies the sizing of grid columns.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-columns
    /// </summary>
    [EnumData("grid-template-columns")]
    GridTemplateColumns,
    /// <summary>
    /// Specifies the sizing of grid rows.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-rows
    /// </summary>
    [EnumData("grid-template-rows")]
    GridTemplateRows,
    /// <summary>
    /// Specifies the sizing of implicitly-created columns.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-columns
    /// </summary>
    [EnumData("grid-auto-columns")]
    GridAutoColumns,
    /// <summary>
    /// Specifies the sizing of implicitly-created rows.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-rows
    /// </summary>
    [EnumData("grid-auto-rows")]
    GridAutoRows,
    /// <summary>
    /// Controls the auto-placement algorithm.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-flow
    /// </summary>
    [EnumData("grid-auto-flow")]
    GridAutoFlow,
    /// <summary>
    /// Specifies a grid item's start position in the column direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-start
    /// </summary>
    [EnumData("grid-column-start")]
    GridColumnStart,
    /// <summary>
    /// Specifies a grid item's end position in the column direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-end
    /// </summary>
    [EnumData("grid-column-end")]
    GridColumnEnd,
    /// <summary>
    /// Specifies a grid item's start position in the row direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-start
    /// </summary>
    [EnumData("grid-row-start")]
    GridRowStart,
    /// <summary>
    /// Specifies a grid item's end position in the row direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-end
    /// </summary>
    [EnumData("grid-row-end")]
    GridRowEnd,
    #endregion

    #region Alignment Properties
    /// <summary>
    /// Aligns flex lines or grid tracks within the container.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-content
    /// </summary>
    [EnumData("align-content")]
    AlignContent,
    /// <summary>
    /// Justifies content along the main/inline axis.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-content
    /// </summary>
    [EnumData("justify-content")]
    JustifyContent,
    /// <summary>
    /// Sets the default align-self for all items.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-items
    /// </summary>
    [EnumData("align-items")]
    AlignItems,
    /// <summary>
    /// Overrides align-items for a specific item.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-self
    /// </summary>
    [EnumData("align-self")]
    AlignSelf,
    /// <summary>
    /// Sets the default justify-self for all items.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-items
    /// </summary>
    [EnumData("justify-items")]
    JustifyItems,
    /// <summary>
    /// Overrides justify-items for a specific item.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-self
    /// </summary>
    [EnumData("justify-self")]
    JustifySelf,
    /// <summary>
    /// Sets the gap between rows.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-row-gap
    /// </summary>
    [EnumData("row-gap")]
    RowGap,
    /// <summary>
    /// Sets the gap between columns.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-column-gap
    /// </summary>
    [EnumData("column-gap")]
    ColumnGap,
    #endregion

    #region Fragmentation Properties
    /// <summary>
    /// Specifies whether a box should break before the principal box.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-before
    /// </summary>
    [EnumData("break-before")]
    BreakBefore,
    /// <summary>
    /// Specifies whether a box should break after the principal box.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-after
    /// </summary>
    [EnumData("break-after")]
    BreakAfter,
    /// <summary>
    /// Specifies whether a box should avoid breaks inside.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-inside
    /// </summary>
    [EnumData("break-inside")]
    BreakInside,
    /// <summary>
    /// Specifies the minimum number of lines in a block container
    /// that must be left before a fragmentation break.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-orphans
    /// </summary>
    [EnumData("orphans")]
    Orphans,
    /// <summary>
    /// Specifies the minimum number of lines in a block container
    /// that must be left after a fragmentation break.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-widows
    /// </summary>
    [EnumData("widows")]
    Widows,
    /// <summary>
    /// Specifies whether box decorations are cloned or sliced at fragment breaks.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-box-decoration-break
    /// </summary>
    [EnumData("box-decoration-break")]
    BoxDecorationBreak,
    #endregion

    #region List Style Properties
    /// <summary>
    /// Specifies the marker string for list items (disc, decimal, etc.).
    /// Docs: https://www.w3.org/TR/css-lists-3/#text-markers
    /// </summary>
    [EnumData("list-style-type")]
    ListStyleType,
    /// <summary>
    /// Specifies the marker image for list items.
    /// Docs: https://www.w3.org/TR/css-lists-3/#image-markers
    /// </summary>
    [EnumData("list-style-image")]
    ListStyleImage,
    /// <summary>
    /// Specifies whether the marker is inside or outside the principal box.
    /// Docs: https://www.w3.org/TR/css-lists-3/#list-style-position-property
    /// </summary>
    [EnumData("list-style-position")]
    ListStylePosition,
    #endregion


    [Ignore]
    MAX_VALUE,
}

