using System;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses CSS url() functions into <see cref="CssUrl"/> values.
/// </summary>
/// <remarks>
/// Per CSS Syntax Level 3:
/// - url() with unquoted value is tokenized as &lt;url-token&gt; (handled by CssParser directly)
/// - url("...") with quoted string is tokenized as &lt;function-token&gt; (handled here)
/// Docs: https://www.w3.org/TR/css-syntax-3/#url-token-diagram
/// </remarks>
internal static class CssUrlFunctionParser
{
    /// <summary>
    /// Attempts to parse a URL from a CSS function with quoted string argument.
    /// Handles: url("path"), url('path')
    /// </summary>
    /// <param name="func">The CSS function token.</param>
    /// <param name="result">The parsed URL if successful.</param>
    /// <returns>True if the function was a valid url() with a quoted string.</returns>
    public static bool TryParseUrlFunction(CssFunction func, out CssUrl result)
    {
        result = CssUrl.Empty;

        if (func is null)
            return false;

        // Check for url() function name (case-insensitive per CSS spec)
        if (!func.Name.Equals("url", StringComparison.OrdinalIgnoreCase))
            return false;

        // The url() function should have a single string argument
        var args = func.Arguments;
        if (args is null || args.Count == 0)
        {
            // url() with no args is valid - empty URL
            result = CssUrl.Empty;
            return true;
        }

        // Skip whitespace to find the string argument
        CssToken? stringToken = null;
        foreach (var token in args)
        {
            if (token is WhitespaceToken)
                continue;

            if (token is StringToken str)
            {
                stringToken = str;
                break;
            }

            // url() function form only accepts quoted strings
            // Raw URLs come as <url-token>, not <function-token>
            return false;
        }

        if (stringToken is StringToken strTok)
        {
            result = new CssUrl(strTok.Value ?? string.Empty);
            return true;
        }

        // No string found - this is valid, empty URL
        result = CssUrl.Empty;
        return true;
    }
}
