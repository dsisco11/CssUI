using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all of the different positioning 'scheme' types for elements
    /// </summary>
    [MetaEnum]
    public enum EPositioningScheme
    {
        /// <summary>Object is positioned according to normal flow logic</summary>
        [MetaKeyword("normal")]
        Normal,
        /// <summary>Object is laid out like in normal flow but then moved as far left or right as possible</summary>
        [MetaKeyword("float")]
        Float,
        /// <summary>Object is not positioned according to normal flow, it defines it's own position relative to a block other than its logical containing block</summary>
        [MetaKeyword("absolute")]
        Absolute
    }
}
