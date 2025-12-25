using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum ECssPropertyID : int
{
    [EnumRecordProperties("line-height")]
    LineHeight,
    [EnumRecordProperties("font-family")]
    FontFamily,
    [EnumRecordProperties("font-weight")]
    FontWeight,
    [EnumRecordProperties("font-style")]
    FontStyle,
    [EnumRecordProperties("font-size")]
    FontSize,
    [EnumRecordProperties("dpi-x")]
    DpiX,
    [EnumRecordProperties("dpi-y")]
    DpiY,
    [EnumRecordProperties("scroll-behavior")]
    ScrollBehavior,
    [EnumRecordProperties("overflow-x")]
    OverflowX,
    [EnumRecordProperties("overflow-y")]
    OverflowY,
    [EnumRecordProperties("color")]
    Color,
    /// <summary>
    /// Docs: https://www.w3.org/TR/css-color-3/#opacity
    /// </summary>
    [EnumRecordProperties("opacity")]
    Opacity,
    [EnumRecordProperties("border-top-color")]
    BorderTopColor,
    [EnumRecordProperties("border-right-color")]
    BorderRightColor,
    [EnumRecordProperties("border-bottom-color")]
    BorderBottomColor,
    [EnumRecordProperties("border-left-color")]
    BorderLeftColor,
    [EnumRecordProperties("border-top-style")]
    BorderTopStyle,
    [EnumRecordProperties("border-right-style")]
    BorderRightStyle,
    [EnumRecordProperties("border-bottom-style")]
    BorderBottomStyle,
    [EnumRecordProperties("border-left-style")]
    BorderLeftStyle,
    [EnumRecordProperties("transform")]
    Transform,
    [EnumRecordProperties("direction")]
    Direction,
    [EnumRecordProperties("writing-mode")]
    WritingMode,
    [EnumRecordProperties("text-align")]
    TextAlign,
    [EnumRecordProperties("object-fit")]
    ObjectFit,
    [EnumRecordProperties("display")]
    Display,
    [EnumRecordProperties("box-sizing")]
    BoxSizing,
    [EnumRecordProperties("positioning")]
    Positioning,
    /// <summary>
    /// Docs: https://www.w3.org/TR/css3-images/#object-position
    /// </summary>
    [EnumRecordProperties("object-position")]
    ObjectPosition,
    /*[EnumRecordProperties("object-position-x")]
    ObjectPositionX,
    [EnumRecordProperties("object-position-y")]
    ObjectPositionY,*/
    [EnumRecordProperties("top")]
    Top,
    [EnumRecordProperties("right")]
    Right,
    [EnumRecordProperties("bottom")]
    Bottom,
    [EnumRecordProperties("left")]
    Left,
    [EnumRecordProperties("width")]
    Width,
    [EnumRecordProperties("height")]
    Height,
    [EnumRecordProperties("min-width")]
    MinWidth,
    [EnumRecordProperties("min-height")]
    MinHeight,
    [EnumRecordProperties("max-width")]
    MaxWidth,
    [EnumRecordProperties("max-height")]
    MaxHeight,
    [EnumRecordProperties("padding-top")]
    PaddingTop,
    [EnumRecordProperties("padding-right")]
    PaddingRight,
    [EnumRecordProperties("padding-bottom")]
    PaddingBottom,
    [EnumRecordProperties("padding-left")]
    PaddingLeft,
    [EnumRecordProperties("border-top-width")]
    BorderTopWidth,
    [EnumRecordProperties("border-right-width")]
    BorderRightWidth,
    [EnumRecordProperties("border-bottom-width")]
    BorderBottomWidth,
    [EnumRecordProperties("border-left-width")]
    BorderLeftWidth,
    [EnumRecordProperties("margin-top")]
    MarginTop,
    [EnumRecordProperties("margin-right")]
    MarginRight,
    [EnumRecordProperties("margin-bottom")]
    MarginBottom,
    [EnumRecordProperties("margin-left")]
    MarginLeft,

    #region Flexbox Properties
    /// <summary>
    /// Specifies how flex items are placed in the flex container.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-direction
    /// </summary>
    [EnumRecordProperties("flex-direction")]
    FlexDirection,
    /// <summary>
    /// Controls whether the flex container is single-line or multi-line.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-wrap
    /// </summary>
    [EnumRecordProperties("flex-wrap")]
    FlexWrap,
    /// <summary>
    /// Specifies the flex grow factor.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-grow
    /// </summary>
    [EnumRecordProperties("flex-grow")]
    FlexGrow,
    /// <summary>
    /// Specifies the flex shrink factor.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-shrink
    /// </summary>
    [EnumRecordProperties("flex-shrink")]
    FlexShrink,
    /// <summary>
    /// Specifies the initial main size of a flex item.
    /// Docs: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-basis
    /// </summary>
    [EnumRecordProperties("flex-basis")]
    FlexBasis,
    /// <summary>
    /// Controls the order of flex/grid items.
    /// Docs: https://www.w3.org/TR/css-display-3/#propdef-order
    /// </summary>
    [EnumRecordProperties("order")]
    Order,
    #endregion

    #region Grid Properties
    /// <summary>
    /// Specifies the sizing of grid columns.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-columns
    /// </summary>
    [EnumRecordProperties("grid-template-columns")]
    GridTemplateColumns,
    /// <summary>
    /// Specifies the sizing of grid rows.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-template-rows
    /// </summary>
    [EnumRecordProperties("grid-template-rows")]
    GridTemplateRows,
    /// <summary>
    /// Specifies the sizing of implicitly-created columns.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-columns
    /// </summary>
    [EnumRecordProperties("grid-auto-columns")]
    GridAutoColumns,
    /// <summary>
    /// Specifies the sizing of implicitly-created rows.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-rows
    /// </summary>
    [EnumRecordProperties("grid-auto-rows")]
    GridAutoRows,
    /// <summary>
    /// Controls the auto-placement algorithm.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-flow
    /// </summary>
    [EnumRecordProperties("grid-auto-flow")]
    GridAutoFlow,
    /// <summary>
    /// Specifies a grid item's start position in the column direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-start
    /// </summary>
    [EnumRecordProperties("grid-column-start")]
    GridColumnStart,
    /// <summary>
    /// Specifies a grid item's end position in the column direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-column-end
    /// </summary>
    [EnumRecordProperties("grid-column-end")]
    GridColumnEnd,
    /// <summary>
    /// Specifies a grid item's start position in the row direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-start
    /// </summary>
    [EnumRecordProperties("grid-row-start")]
    GridRowStart,
    /// <summary>
    /// Specifies a grid item's end position in the row direction.
    /// Docs: https://www.w3.org/TR/css-grid-1/#propdef-grid-row-end
    /// </summary>
    [EnumRecordProperties("grid-row-end")]
    GridRowEnd,
    #endregion

    #region Alignment Properties
    /// <summary>
    /// Aligns flex lines or grid tracks within the container.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-content
    /// </summary>
    [EnumRecordProperties("align-content")]
    AlignContent,
    /// <summary>
    /// Justifies content along the main/inline axis.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-content
    /// </summary>
    [EnumRecordProperties("justify-content")]
    JustifyContent,
    /// <summary>
    /// Sets the default align-self for all items.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-items
    /// </summary>
    [EnumRecordProperties("align-items")]
    AlignItems,
    /// <summary>
    /// Overrides align-items for a specific item.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-align-self
    /// </summary>
    [EnumRecordProperties("align-self")]
    AlignSelf,
    /// <summary>
    /// Sets the default justify-self for all items.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-items
    /// </summary>
    [EnumRecordProperties("justify-items")]
    JustifyItems,
    /// <summary>
    /// Overrides justify-items for a specific item.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-justify-self
    /// </summary>
    [EnumRecordProperties("justify-self")]
    JustifySelf,
    /// <summary>
    /// Sets the gap between rows.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-row-gap
    /// </summary>
    [EnumRecordProperties("row-gap")]
    RowGap,
    /// <summary>
    /// Sets the gap between columns.
    /// Docs: https://www.w3.org/TR/css-align-3/#propdef-column-gap
    /// </summary>
    [EnumRecordProperties("column-gap")]
    ColumnGap,
    #endregion

    #region Fragmentation Properties
    /// <summary>
    /// Specifies whether a box should break before the principal box.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-before
    /// </summary>
    [EnumRecordProperties("break-before")]
    BreakBefore,
    /// <summary>
    /// Specifies whether a box should break after the principal box.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-after
    /// </summary>
    [EnumRecordProperties("break-after")]
    BreakAfter,
    /// <summary>
    /// Specifies whether a box should avoid breaks inside.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-break-inside
    /// </summary>
    [EnumRecordProperties("break-inside")]
    BreakInside,
    /// <summary>
    /// Specifies the minimum number of lines in a block container 
    /// that must be left before a fragmentation break.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-orphans
    /// </summary>
    [EnumRecordProperties("orphans")]
    Orphans,
    /// <summary>
    /// Specifies the minimum number of lines in a block container 
    /// that must be left after a fragmentation break.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-widows
    /// </summary>
    [EnumRecordProperties("widows")]
    Widows,
    /// <summary>
    /// Specifies whether box decorations are cloned or sliced at fragment breaks.
    /// Docs: https://www.w3.org/TR/css-break-3/#propdef-box-decoration-break
    /// </summary>
    [EnumRecordProperties("box-decoration-break")]
    BoxDecorationBreak,
    #endregion


    MAX_VALUE,
}

