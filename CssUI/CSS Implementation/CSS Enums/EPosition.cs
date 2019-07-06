using CssUI.Internal;

namespace CssUI.Enums
{
    [CssEnum]
    public enum EPositioning : int
    {
        /// <summary>
        /// Element is normal, positioned via the normal layout logic
        /// </summary>
        [CssKeyword("static")]
        Static,
        /// <summary>
        /// The box's position is calculated according to the normal flow (this is called the position in normal flow). Then the box is offset relative to its normal position. When a box B is relatively positioned, the position of the following box is calculated as though B were not offset. The effect of 'position:relative' on table-row-group, table-header-group, table-footer-group, table-row, table-column-group, table-column, table-cell, and table-caption elements is undefined.
        /// </summary>
        [CssKeyword("relative")]
        Relative,
        /// <summary>
        /// The box's position (and possibly size) is specified with the 'top', 'right', 'bottom', and 'left' properties. These properties specify offsets with respect to the box's containing block. Absolutely positioned boxes are taken out of the normal flow. This means they have no impact on the layout of later siblings.
        /// </summary>
        [CssKeyword("absolute")]
        Absolute,
        /// <summary>
        /// The box's position is calculated according to the 'absolute' model, but in addition, the box is fixed with respect to some reference. IE: the viewport.
        /// </summary>
        [CssKeyword("fixed")]
        Fixed
    }
}
