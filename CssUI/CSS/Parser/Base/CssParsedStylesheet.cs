using System;
using System.Collections.Generic;

namespace CssUI.CSS.Serialization;

/// <summary>
/// Represents the result of parsing a stylesheet per CSS Syntax Level 3 §5.3.3.
/// Contains the parsed rules and optionally the location URL of the stylesheet.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#parse-stylesheet"/>
public sealed class CssParsedStylesheet
{
    #region Properties
    /// <summary>
    /// The URL location of the stylesheet, or null if not provided.
    /// </summary>
    /// <remarks>
    /// Per CSS Syntax Level 3 §5.3.3: The stylesheet has its location set to location (or null, if location was not passed).
    /// </remarks>
    public Uri? Location { get; }

    /// <summary>
    /// The list of rules parsed from the stylesheet.
    /// </summary>
    public IReadOnlyList<CssComponent> Rules { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new stylesheet result with the given rules and optional location.
    /// </summary>
    /// <param name="rules">The parsed rules.</param>
    /// <param name="location">The optional URL location of the stylesheet.</param>
    public CssParsedStylesheet(IEnumerable<CssComponent> rules, Uri? location = null)
    {
        Rules = rules as IReadOnlyList<CssComponent> ?? new List<CssComponent>(rules);
        Location = location;
    }
    #endregion
}
