namespace CssUI.CSS.Parser;

/// <summary>
/// Specifies how the CSS parser handles parse errors.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#tokenizing-and-parsing"/>
public enum EParseErrorMode
{
    /// <summary>
    /// Recover from parse errors gracefully, discarding the minimum amount of content
    /// and continuing to parse. This is the default behavior per CSS Syntax Level 3.
    /// Errors are reported via <see cref="ICssParseErrorReporter"/> but do not throw.
    /// </summary>
    Recover,

    /// <summary>
    /// Abort parsing at the first parse error by throwing an exception.
    /// Use this mode for strict validation or conformance checking.
    /// </summary>
    Abort
}
