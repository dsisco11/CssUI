using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Specifies whether a box should float to the left, right, or not float at all.
/// Per CSS 2.2 §9.5: "A float is a box that is shifted to the left or right on the current line."
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#float-position
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EFloat : int
{
    /// <summary>
    /// The element does not float.
    /// </summary>
    [EnumData("none")]
    None = 0,

    /// <summary>
    /// The element generates a block box that is floated to the left.
    /// Content flows on the right side of the box, starting at the top (subject to the 'clear' property).
    /// </summary>
    [EnumData("left")]
    Left,

    /// <summary>
    /// Similar to 'left', except the box is floated to the right, and content flows on the left side of the box, starting at the top.
    /// </summary>
    [EnumData("right")]
    Right,

    /// <summary>
    /// The element generates a block box that is floated to the inline-start side (left in LTR, right in RTL).
    /// Logical property from CSS Logical Properties Level 1.
    /// Spec: https://www.w3.org/TR/css-logical-1/#float-clear
    /// </summary>
    [EnumData("inline-start")]
    InlineStart,

    /// <summary>
    /// The element generates a block box that is floated to the inline-end side (right in LTR, left in RTL).
    /// Logical property from CSS Logical Properties Level 1.
    /// Spec: https://www.w3.org/TR/css-logical-1/#float-clear
    /// </summary>
    [EnumData("inline-end")]
    InlineEnd
}
