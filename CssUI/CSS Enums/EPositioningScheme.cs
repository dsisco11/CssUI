using CssUI.Internal;

namespace CssUI.Enums
{
    /// <summary>
    /// Defines all of the different positioning 'scheme' types for elements
    /// </summary>
    [CssEnum]
    public enum EPositioningScheme
    {
        /// <summary>Object is positioned according to normal flow logic</summary>
        [CssKeyword("normal")]
        Normal,
        /// <summary>Object is laid out like in normal flow but then moved as far left or right as possible</summary>
        [CssKeyword("float")]
        Float,
        /// <summary>Object is not positioned according to normal flow, it defines it's own position relative to a block other than its logical containing block</summary>
        [CssKeyword("absolute")]
        Absolute
    }
}
