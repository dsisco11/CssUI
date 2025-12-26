using System;
using System.Buffers;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Defines a generic rule for CSS, this could be anything from an @media rule to the familiar Style rule.
/// These values held by this class and all its subtypes are UNINTERPRETED and essentially just hold and organize the rules as text.
/// </summary>
/// <remarks>
/// Implements <see cref="ISpanFormattable"/> for efficient serialization per CSSOM §6.4 "Serialize a CSS rule".
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#css-rules"/>
public abstract class CSSRule : ISpanFormattable
{
    #region Properties
    public readonly ECssRuleType type;
    public string cssText => ToString();
    public readonly CSSRule? parentRule;
    public readonly CSSStyleSheet? parentStyleSheet;
    #endregion

    #region Constructors
    protected CSSRule(ECssRuleType type, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
    {
        this.type = type;
        this.parentRule = parentRule;
        this.parentStyleSheet = parentStyleSheet;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    public abstract bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Estimate buffer size based on rule type
        int estimatedSize = EstimateSerializationSize();
        char[]? rented = null;
        Span<char> buffer = estimatedSize <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(estimatedSize));

        try
        {
            if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
                return new string(buffer[..charsWritten]);

            // Fallback for unexpectedly large output
            if (rented != null)
                ArrayPool<char>.Shared.Return(rented);
            rented = ArrayPool<char>.Shared.Rent(estimatedSize * 4);
            buffer = rented;
            if (TryFormat(buffer, out charsWritten, format.AsSpan(), formatProvider))
                return new string(buffer[..charsWritten]);

            throw new InvalidOperationException("Buffer too small for CSS rule serialization");
        }
        finally
        {
            if (rented != null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <summary>
    /// Estimates the buffer size needed for serialization. Override in derived classes for better estimates.
    /// </summary>
    protected virtual int EstimateSerializationSize() => 256;
    #endregion

    #region Parsing
    public static CSSRule From_String(string rule)
    {
        throw new NotImplementedException();
    }
    #endregion
}

