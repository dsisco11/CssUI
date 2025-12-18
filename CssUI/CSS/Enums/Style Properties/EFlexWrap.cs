using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Values for the flex-wrap property.
    /// Spec: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-wrap
    /// </summary>
    [MetaEnum]
    public enum EFlexWrap
    {
        /// <summary>
        /// The flex container is single-line.
        /// </summary>
        [MetaKeyword("nowrap")]
        NoWrap,

        /// <summary>
        /// The flex container is multi-line.
        /// </summary>
        [MetaKeyword("wrap")]
        Wrap,

        /// <summary>
        /// Same as wrap, but the cross-start and cross-end directions are swapped.
        /// </summary>
        [MetaKeyword("wrap-reverse")]
        WrapReverse
    }
}

