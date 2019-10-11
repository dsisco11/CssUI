using CssUI.Internal;

namespace CssUI.CSS
{
    [MetaEnum]
    public enum EScrollBehavior
    {
        /// <summary>
        /// The scrolling box is scrolled in an instant fashion.
        /// </summary>
        [MetaKeyword("auto")]
        Auto,

        /// <summary>
        /// The scrolling box is scrolled in a smooth fashion using a user-agent-defined timing function over a user-agent-defined period of time.
        /// </summary>
        [MetaKeyword("smooth")]
        Smooth,
        //[DomKeyword("instant")] /* Instant is not defined as a specifiable value, it is an internal value only */
        Instant,
    }
}
