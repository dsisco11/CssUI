namespace CssUI.CSS.Parser;

/// <summary>
/// Defines the types of parse errors that can occur during CSS parsing.
/// These error types align with the CSS Syntax Level 3 specification.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#error-handling"/>
public enum ECssParseErrorType
{
    /// <summary>
    /// No error - used as default value.
    /// </summary>
    None = 0,

    #region Tokenizer Parse Errors (§4.3)
    /// <summary>
    /// A string was not properly terminated before a newline.
    /// Tokenizer returns a &lt;bad-string-token&gt;.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-string-token"/>
    BadString,

    /// <summary>
    /// A URL was malformed (e.g., contained invalid characters or was unterminated).
    /// Tokenizer returns a &lt;bad-url-token&gt;.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-url-token"/>
    BadUrl,

    /// <summary>
    /// An escape sequence was started with \ but followed by EOF.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-escaped-code-point"/>
    InvalidEscapeAtEof,

    /// <summary>
    /// A comment was not properly terminated before EOF.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-comment"/>
    UnterminatedComment,

    /// <summary>
    /// A string was not properly terminated before EOF.
    /// The string token is still returned, but this is a parse error.
    /// </summary>
    UnterminatedString,

    /// <summary>
    /// A URL was not properly terminated before EOF.
    /// The URL token is still returned, but this is a parse error.
    /// </summary>
    UnterminatedUrl,
    #endregion

    #region Parser Parse Errors (§5.4)
    /// <summary>
    /// An at-rule was not properly terminated before EOF.
    /// The at-rule is still returned.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-at-rule"/>
    UnterminatedAtRule,

    /// <summary>
    /// A qualified rule was not properly terminated (reached EOF without finding block).
    /// Nothing is returned for this qualified rule.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-qualified-rule"/>
    UnterminatedQualifiedRule,

    /// <summary>
    /// A simple block was not properly terminated before EOF.
    /// The block is still returned.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-simple-block"/>
    UnterminatedBlock,

    /// <summary>
    /// A function was not properly terminated before EOF.
    /// The function is still returned.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-function"/>
    UnterminatedFunction,

    /// <summary>
    /// A declaration did not have a colon after the property name.
    /// Nothing is returned for this declaration.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-declaration"/>
    MissingColonInDeclaration,

    /// <summary>
    /// An unexpected token was encountered in a declaration list context.
    /// Tokens are consumed and discarded until a semicolon or EOF.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-list-of-declarations"/>
    UnexpectedTokenInDeclarationList,

    /// <summary>
    /// An unexpected token was encountered in a style block context.
    /// Tokens are consumed and discarded until a semicolon or EOF.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-style-block"/>
    UnexpectedTokenInStyleBlock,
    #endregion

    #region Token Validation Errors
    /// <summary>
    /// An unmatched closing bracket token was encountered: }, ), or ].
    /// Per spec, these are preserved in the token stream but are always parse errors.
    /// </summary>
    UnmatchedClosingBracket,

    /// <summary>
    /// A &lt;bad-string-token&gt; was encountered during parsing.
    /// </summary>
    BadStringToken,

    /// <summary>
    /// A &lt;bad-url-token&gt; was encountered during parsing.
    /// </summary>
    BadUrlToken,
    #endregion

    #region Value Parse Errors
    /// <summary>
    /// An invalid hex color value was encountered.
    /// </summary>
    InvalidHexColor,

    /// <summary>
    /// An invalid URL syntax was encountered.
    /// </summary>
    InvalidUrlSyntax,

    /// <summary>
    /// An unhandled token type was encountered when parsing a CSS value.
    /// </summary>
    UnhandledTokenType,
    #endregion
}
