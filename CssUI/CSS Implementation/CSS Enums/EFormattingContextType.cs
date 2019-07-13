
namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Boxes in the normal flow belong to a formatting context, 
    /// which in CSS 2.2 may be table, block or inline. 
    /// In future levels of CSS, other types of formatting context will be introduced. 
    /// Block-level boxes participate in a block formatting context. 
    /// Inline-level boxes participate in an inline formatting context. 
    /// Table formatting contexts are described in the chapter on tables.
    /// </summary>
    public enum EFormattingContextType : int
    {/* Docs: https://www.w3.org/TR/CSS22/visuren.html#block-formatting */
        /// <summary>
        /// No formatting context, usually things like floats
        /// </summary>
        None = 0,
        /// <summary>
        /// In a block formatting context, boxes are laid out one after the other, vertically, beginning at the top of a containing block. 
        /// The vertical distance between two sibling boxes is determined by the 'margin' properties. 
        /// Vertical margins between adjacent block-level boxes in a block formatting context collapse.
        /// </summary>
        Block,
        /// <summary>
        /// In an inline formatting context, boxes are laid out horizontally, one after the other, beginning at the top of a containing block.
        /// </summary>
        Inline,
    }
}
