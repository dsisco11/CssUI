
namespace CssUI.Enums
{
    /// <summary>
    /// Dictates how the principal box itself participates in flow layout.
    /// </summary>
    public enum EOuterDisplayType : int
    {/* Docs: https://www.w3.org/TR/css-display-3/#the-display-properties */
        /// <summary>
        /// Block-level elements – those elements of the source document that are formatted visually as blocks (e.g., paragraphs) – are elements which generate a block-level principal box. 
        /// Values of the 'display' property that make an element block-level include: 'block', 'list-item', and 'table'. 
        /// Block-level boxes are boxes that participate in a block formatting context.
        /// </summary>
        Block = 0,
        /// <summary>
        /// 
        /// </summary>
        Inline,
        /// <summary>
        /// 
        /// </summary>
        Run_In,
    }
}
