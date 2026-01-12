using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Specifies which sides of an element's box(es) may not be adjacent to an earlier floating box.
/// Per CSS 2.2 §9.5.2: "This property indicates which sides of an element's box(es) may not be
/// adjacent to an earlier floating box."
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#propdef-clear
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EClear : int
{
    /// <summary>
    /// No constraint on the box's position with respect to floats.
    /// </summary>
    [EnumData("none")]
    None = 0,

    /// <summary>
    /// The top border edge of the box is placed below the bottom outer edge of any
    /// left-floating boxes that resulted from elements earlier in the source document.
    /// </summary>
    [EnumData("left")]
    Left,

    /// <summary>
    /// The top border edge of the box is placed below the bottom outer edge of any
    /// right-floating boxes that resulted from elements earlier in the source document.
    /// </summary>
    [EnumData("right")]
    Right,

    /// <summary>
    /// The top border edge of the box is placed below the bottom outer edge of any
    /// left-floating and right-floating boxes that resulted from elements earlier in the source document.
    /// </summary>
    [EnumData("both")]
    Both,

    /// <summary>
    /// The top border edge of the box is placed below the bottom outer edge of any
    /// inline-start floating boxes (left in LTR, right in RTL) that resulted from elements
    /// earlier in the source document.
    /// Logical property from CSS Logical Properties Level 1.
    /// Spec: https://www.w3.org/TR/css-logical-1/#float-clear
    /// </summary>
    [EnumData("inline-start")]
    InlineStart,

    /// <summary>
    /// The top border edge of the box is placed below the bottom outer edge of any
    /// inline-end floating boxes (right in LTR, left in RTL) that resulted from elements
    /// earlier in the source document.
    /// Logical property from CSS Logical Properties Level 1.
    /// Spec: https://www.w3.org/TR/css-logical-1/#float-clear
    /// </summary>
    [EnumData("inline-end")]
    InlineEnd
}
