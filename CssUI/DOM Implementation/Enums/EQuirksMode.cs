using CssUI.Internal;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Describes the <see cref="Document"/>s quirks mode.
    /// </summary>
    [MetaEnum]
    public enum EQuirksMode
    {
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("quirks")]
        Quirks,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("no-quirks")]
        NoQuirks,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("limited-quirks")]
        LimitedQuirks
    }
}
