namespace CssUI.CSS.Parser;

/// <summary>
/// Classifies how a token should be interpreted within a CSS block context.
/// </summary>
/// <remarks>
/// <para>
/// This enum is used by <see cref="CssProductionMatcher.ClassifyTokenForContext"/>
/// to determine how a token should be processed based on the current parsing context
/// (<see cref="ECssBlockContentsType"/>).
/// </para>
/// <para>
/// The classification determines whether a token starts:
/// </para>
/// <list type="bullet">
/// <item><description>A declaration (property: value)</description></item>
/// <item><description>A nested style rule (CSS Nesting)</description></item>
/// <item><description>A qualified rule (regular selector rule)</description></item>
/// <item><description>An at-rule (@media, @supports, etc.)</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#declaration-rule-list"/>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
public enum ECssBlockContentClassification
{
    /// <summary>
    /// Token is invalid in this context.
    /// </summary>
    Invalid,

    /// <summary>
    /// Token is whitespace (should be skipped).
    /// </summary>
    Whitespace,

    /// <summary>
    /// Token is a separator (semicolon).
    /// </summary>
    Separator,

    /// <summary>
    /// Token indicates end of input (EOF).
    /// </summary>
    EndOfInput,

    /// <summary>
    /// Token should be ignored (e.g., CDO/CDC at top level).
    /// </summary>
    Ignored,

    /// <summary>
    /// Token starts a declaration (property: value).
    /// </summary>
    /// <remarks>
    /// In style-block context, this is an identifier token that starts a CSS property declaration.
    /// </remarks>
    Declaration,

    /// <summary>
    /// Token starts a nested style rule (CSS Nesting).
    /// </summary>
    /// <remarks>
    /// Only valid in <see cref="ECssBlockContentsType.StyleBlock"/> context.
    /// Tokens that start nested rules: &amp;, ., *, &gt;, +, ~, #, :, [
    /// </remarks>
    NestedRule,

    /// <summary>
    /// Token starts a qualified rule (selector { ... }).
    /// </summary>
    /// <remarks>
    /// In rule-list and stylesheet contexts, most tokens start qualified rules.
    /// In style-block context, nested rules are classified separately.
    /// </remarks>
    QualifiedRule,

    /// <summary>
    /// Token starts an at-rule (@media, @supports, etc.).
    /// </summary>
    AtRule,

    /// <summary>
    /// Token is an unmatched closing bracket (parse error).
    /// </summary>
    /// <remarks>
    /// Unmatched <c>)</c>, <c>]</c>, or <c>}</c> tokens are parse errors
    /// that should be reported but allow parsing to continue.
    /// </remarks>
    UnmatchedCloseBracket
}
