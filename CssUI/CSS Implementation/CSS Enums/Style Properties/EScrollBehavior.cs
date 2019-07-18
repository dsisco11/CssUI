using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    [CssEnum]
    public enum EScrollBehavior
    {
        /// <summary>
        /// The scrolling box is scrolled in an instant fashion.
        /// </summary>
        [CssKeyword("auto")]
        Auto,

        /// <summary>
        /// The scrolling box is scrolled in a smooth fashion using a user-agent-defined timing function over a user-agent-defined period of time.
        /// </summary>
        [CssKeyword("smooth")]
        Smooth,
        //[DomKeyword("instant")] /* Instant is not defined as a specifiable value, it is an internal value only */
        Instant,
    }
}
