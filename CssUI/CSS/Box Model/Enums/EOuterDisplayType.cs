namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Defines the *-level of a box, whether it is block-level, inline-level, or other.
    /// Dictates how the principal box itself participates in flow layout.
    /// </summary>
    public enum EOuterDisplayType : byte
    {/* Docs: https://www.w3.org/TR/css-display-3/#outer-role */

        /// <summary>
        /// This element does not generate boxes.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Block-level elements – those elements of the source document that are formatted visually as blocks (e.g., paragraphs) – are elements which generate a block-level principal box. 
        /// Values of the 'display' property that make an element block-level include: 'block', 'list-item', and 'table'. 
        /// Block-level boxes are boxes that participate in a block formatting context.
        /// </summary>
        Block,
        /// <summary>
        /// The element generates a box that is inline-level when placed in flow layout.
        /// </summary>
        Inline,
        /// <summary>
        /// The element generates an run-in box, which is a type of inline-level box with special behavior that attempts to merge it into a subsequent block container. See § 3 Run-In Layout for details.
        /// </summary>
        Run_In,
    }
}
