using CssUI.DOM.Internal;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Describes the <see cref="Document"/>s quirks mode.
    /// </summary>
    [DomEnum]
    public enum EQuirksMode
    {
        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("quirks")]
        Quirks,

        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("no-quirks")]
        NoQuirks,

        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("limited-quirks")]
        LimitedQuirks
    }
}
