using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EScrollBehavior
{
    /// <summary>
    /// The scrolling box is scrolled in an instant fashion.
    /// </summary>
    [EnumRecordProperties("auto")]
    Auto,

    /// <summary>
    /// The scrolling box is scrolled in a smooth fashion using a user-agent-defined timing function over a user-agent-defined period of time.
    /// </summary>
    [EnumRecordProperties("smooth")]
    Smooth,
    //[DomKeyword("instant")] /* Instant is not defined as a specifiable value, it is an internal value only */
    Instant,
}

