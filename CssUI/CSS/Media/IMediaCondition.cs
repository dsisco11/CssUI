using System;
using CssUI.DOM;

namespace CssUI.CSS.Media;

/// <summary>
/// Represents a media query condition that can be evaluated against a document.
/// </summary>
/// <seealso href="https://www.w3.org/TR/mediaqueries-4/#media-condition"/>
public interface IMediaCondition : ISpanFormattable
{
    /// <summary>
    /// Evaluates whether this media condition matches the given document's state.
    /// </summary>
    /// <param name="document">The document to evaluate against.</param>
    /// <returns><c>true</c> if the condition matches; otherwise, <c>false</c>.</returns>
    bool Matches(Document document);
}

