using System;
using System.Buffers;
using System.Collections.Generic;
using CssUI.CSS.Serialization;
using CssUI.DOM;

namespace CssUI.CSS.Media;

public class MediaQuery : ISpanFormattable
{/* https://drafts.csswg.org/mediaqueries-4/#media */
    #region Static Properties
    /// <summary>
    /// Gets a MediaQuery that never matches (equivalent to "not all").
    /// Per CSS Media Queries Level 4, invalid media queries evaluate to "not all".
    /// </summary>
    public static MediaQuery NotAll { get; } = new MediaQuery(
        EMediaQueryModifier.Not,
        EMediaType.All,
        new LinkedList<IMediaCondition>());
    #endregion

    #region Properties
    public EMediaQueryModifier Modifier { get; private set; }
    public EMediaType MediaType { get; private set; }
    public LinkedList<IMediaCondition> Conditions { get; private set; }
    #endregion

    #region Constructors
    public MediaQuery(EMediaQueryModifier modifier, EMediaType mediaType, LinkedList<IMediaCondition> conditions)
    {
        Modifier = modifier;
        MediaType = mediaType;
        Conditions = conditions;
    }
    #endregion

    /// <summary>
    /// Returns <c>true</c> if this query matches the given <see cref="Document"/>
    /// </summary>
    /// <param name="document">The document to test for a match against</param>
    public bool Matches(Document document)
    {
        if (MediaType != EMediaType.All && MediaType != document.defaultView.screen.MediaType)
            return false;

        foreach (IMediaCondition condition in Conditions)
        {
            if (!condition.Matches(document))
            {
                return false;
            }
        }

        return true;
    }

    #region Formatting
    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        int written;

        // Handle modifier
        if (Modifier == EMediaQueryModifier.Not)
        {
            if (!"not ".TryCopyTo(destination))
                return false;
            charsWritten += 4;
        }

        string type = Serializer.Identifier<EMediaType>(MediaType);

        /* 3) If the media query does not contain media features append type, to s, then return s. */
        if (Conditions.Count <= 0)
        {
            if (!type.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += type.Length;
            return true;
        }

        /* 4) If type is not "all" or if the media query is negated append type, followed by a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
        if (MediaType != EMediaType.All || Modifier == EMediaQueryModifier.Not)
        {
            if (!type.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += type.Length;

            if (!" and ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 5;
        }

        /* No clue why we would need to do this but its not possible with our system (atleast not easily) */
        /* 5) Sort the media features in lexicographical order. */
        //List<MediaFeature> features = query.Conditions.Where(cond => cond is MediaFeature feature && feature.IsValid).OrderBy(feature => CssLookup.Keyword_From_Enum(feature.Name)).ToList();

        bool first = true;
        foreach (IMediaCondition condition in Conditions)
        {
            /* 4) If this is not the last media feature append a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
            if (!first)
            {
                if (!" and ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 5;
            }

            if (!condition.TryFormat(destination[charsWritten..], out written, format, provider))
                return false;
            charsWritten += written;
            first = false;
        }

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Estimate buffer size: modifier(4) + type(10) + conditions
        int estimatedSize = 64 + (Conditions.Count * 32);
        char[]? rented = null;
        Span<char> buffer = estimatedSize <= 256
            ? stackalloc char[256]
            : (rented = ArrayPool<char>.Shared.Rent(estimatedSize));

        try
        {
            if (TryFormat(buffer, out int charsWritten, default, formatProvider))
                return new string(buffer[..charsWritten]);

            // Fallback for unexpectedly large output
            rented = ArrayPool<char>.Shared.Rent(estimatedSize * 2);
            buffer = rented;
            if (TryFormat(buffer, out charsWritten, default, formatProvider))
                return new string(buffer[..charsWritten]);

            throw new InvalidOperationException("Buffer too small for MediaQuery serialization");
        }
        finally
        {
            if (rented != null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);
    #endregion
}

