namespace CssUI.CSS.Selectors;

/// <summary>
/// Defines the case-sensitivity behavior for attribute selector value matching.
/// Per W3C Selectors Level 4 §6.3, the 'i' and 's' modifiers control case-sensitivity.
/// </summary>
public enum EAttributeCaseSensitivity
{
    /// <summary>
    /// Default behavior: Case-sensitivity depends on the document language.
    /// For HTML, most attributes are case-sensitive, but some (like type on input) are case-insensitive.
    /// </summary>
    Default,

    /// <summary>
    /// The 'i' modifier: Forces case-insensitive matching for attribute values.
    /// Example: [type="TEXT" i] matches type="text", type="TEXT", type="Text", etc.
    /// </summary>
    CaseInsensitive,

    /// <summary>
    /// The 's' modifier: Forces case-sensitive matching for attribute values.
    /// Example: [type="text" s] only matches type="text", not type="TEXT".
    /// </summary>
    CaseSensitive
}
