namespace CssUI.CSS.Parser;

/// <summary>
/// Types of failures that can occur when matching a CSS grammar production.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#any-value"/>
public enum ECssProductionMatchFailure
{
    /// <summary>
    /// No failure - the production matched successfully.
    /// </summary>
    None = 0,

    /// <summary>
    /// The token sequence was empty, but the production requires at least one token.
    /// </summary>
    EmptySequence,

    /// <summary>
    /// A <c>&lt;bad-string-token&gt;</c> was encountered.
    /// </summary>
    /// <remarks>
    /// Bad string tokens are always invalid in both <c>&lt;declaration-value&gt;</c>
    /// and <c>&lt;any-value&gt;</c> productions.
    /// </remarks>
    BadStringToken,

    /// <summary>
    /// A <c>&lt;bad-url-token&gt;</c> was encountered.
    /// </summary>
    /// <remarks>
    /// Bad URL tokens are always invalid in both <c>&lt;declaration-value&gt;</c>
    /// and <c>&lt;any-value&gt;</c> productions.
    /// </remarks>
    BadUrlToken,

    /// <summary>
    /// An unmatched closing bracket (<c>&lt;)-token&gt;</c>, <c>&lt;]-token&gt;</c>, or <c>&lt;}-token&gt;</c>) was found.
    /// </summary>
    /// <remarks>
    /// Closing brackets without matching opening brackets are always invalid in both
    /// <c>&lt;declaration-value&gt;</c> and <c>&lt;any-value&gt;</c> productions.
    /// </remarks>
    UnmatchedCloseBracket,

    /// <summary>
    /// A top-level <c>&lt;semicolon-token&gt;</c> was found.
    /// </summary>
    /// <remarks>
    /// Invalid in <c>&lt;declaration-value&gt;</c>, but allowed in <c>&lt;any-value&gt;</c>.
    /// "Top-level" means not inside a paired bracket construct (parentheses, square brackets, or curly braces).
    /// </remarks>
    TopLevelSemicolon,

    /// <summary>
    /// A top-level <c>&lt;delim-token&gt;</c> with value "!" was found.
    /// </summary>
    /// <remarks>
    /// Invalid in <c>&lt;declaration-value&gt;</c>, but allowed in <c>&lt;any-value&gt;</c>.
    /// "Top-level" means not inside a paired bracket construct (parentheses, square brackets, or curly braces).
    /// </remarks>
    TopLevelExclamation
}
