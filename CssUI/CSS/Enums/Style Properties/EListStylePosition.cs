using EnumRecords;

namespace CssUI.CSS.Enums;

/// <summary>
/// Specifies whether the ::marker is rendered inline (inside) or positioned just outside the list item (outside).
/// Spec: https://www.w3.org/TR/css-lists-3/#list-style-position-property
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EListStylePosition : int
{
    /// <summary>
    /// The marker box is placed outside the principal block box.
    /// This is the default value.
    /// </summary>
    [EnumData("outside")]
    Outside,

    /// <summary>
    /// The marker is an inline element at the start of the list item's contents.
    /// </summary>
    [EnumData("inside")]
    Inside,
}
