namespace CssUI.CSS.Enums
{
    /// <summary>
    /// the inner display type, which defines (if it is a non-replaced element) the kind of formatting context it generates, 
    /// dictating how its descendant boxes are laid out. (The inner display of a replaced element is outside the scope of CSS.)
    /// </summary>
    public enum EInnerDisplayType : byte
    {/* Docs: https://www.w3.org/TR/css-display-3/#inner-model */
        /// <summary>
        /// This element does not generate boxes.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// The element lays out its contents using flow layout (block-and-inline layout).
        /// If its outer display type is inline or run-in, and it is participating in a block or inline formatting context, then it generates an inline box.
        /// Otherwise it generates a block container box.
        /// Depending on the value of other properties (such as position, float, or overflow) and whether it is itself participating in a block or inline formatting context, it either establishes a new block formatting context for its contents or integrates its contents into its parent formatting context.See CSS2.1 Chapter 9. [CSS2] A block container that establishes a new block formatting context is considered to have a used inner display type of flow-root.
        /// </summary>
        Flow,
        /// <summary>
        /// The element generates a block container box, and lays out its contents using flow layout. It always establishes a new block formatting context for its contents.
        /// </summary>
        Flow_Root,
        /// <summary>
        /// The element generates a principal table wrapper box that establishes a block formatting context, and which contains an additionally-generated table grid box that establishes a table formatting context.
        /// </summary>
        Table,
        /// <summary>
        /// The element generates a principal flex container box and establishes a flex formatting context.
        /// </summary>
        Flex,
        /// <summary>
        /// The element generates a principal grid container box, and establishes a grid formatting context.
        /// </summary>
        Grid,
        /// <summary>
        /// The element generates a ruby container box and establishes a ruby formatting context in addition to integrating its base-level contents into its parent formatting context (if it is inline) or generating a wrapper box of the appropriate outer display type (if it is not).
        /// </summary>
        Ruby
    }
}
