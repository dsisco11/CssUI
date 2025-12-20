using System;
using CssUI.CSS.BoxTree;
using CssUI.DOM;

namespace CssUI.CSS;

/// <summary>
/// Service interface for calculating intrinsic sizes for text content.
/// </summary>
public interface ITextIntrinsicSizer
{
    /// <summary>
    /// Calculate intrinsic size for text content within a box.
    /// </summary>
    IntrinsicSize Calculate(CssPrincipalBox box, IntrinsicSizeContext context);

    /// <summary>
    /// Calculate intrinsic size for a text run with specific properties.
    /// </summary>
    IntrinsicSize CalculateTextRun(ReadOnlySpan<char> text, FontHandle font, double lineHeight);
}

/// <summary>
/// Calculates intrinsic sizes for text content.
/// </summary>
/// <remarks>
/// For text:
/// - min-content inline size: width of the longest word (break at soft wrap opportunities)
/// - max-content inline size: width of all text on a single line
/// - block size: line height (typically same for both min/max)
/// 
/// See: https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes
/// </remarks>
public sealed class TextIntrinsicSizer : ITextIntrinsicSizer
{
    private readonly IFontService _fontService;

    public TextIntrinsicSizer(IFontService fontService)
    {
        _fontService = fontService ?? throw new ArgumentNullException(nameof(fontService));
    }

    /// <summary>
    /// Calculate intrinsic size for text content within a box.
    /// </summary>
    public IntrinsicSize Calculate(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        // Get the owning element to access text content
        var element = box.Owner;
        if (element == null)
            return IntrinsicSize.Zero;

        // Collect all text content from the element
        var textContent = CollectTextContent(element);
        if (string.IsNullOrEmpty(textContent))
            return IntrinsicSize.Zero;

        // Get font handle for measurement
        var font = box.Style?.Font ?? FontHandle.Null;
        if (font.IsNull)
        {
            font = _fontService.GetDefaultFont(16f);
        }

        // Calculate max-content: measure entire text on single line
        var fullMeasurement = _fontService.MeasureText(font, textContent.AsSpan());
        double maxContentInline = fullMeasurement.Width;
        double lineHeight = GetLineHeight(box, fullMeasurement.Height);

        // Calculate min-content: find longest "word" (break at soft wrap opportunities)
        double minContentInline = CalculateMinContentWidth(textContent, font);

        return new IntrinsicSize(
            new IntrinsicAxisSize(minContentInline, maxContentInline),
            IntrinsicAxisSize.Definite(lineHeight));
    }

    /// <summary>
    /// Calculate min-content width by finding the longest unbreakable segment.
    /// </summary>
    private double CalculateMinContentWidth(string text, FontHandle font)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        double maxWordWidth = 0;
        int wordStart = 0;
        bool inWord = false;

        for (int i = 0; i <= text.Length; i++)
        {
            bool isBreakOpportunity = i == text.Length || IsSoftWrapOpportunity(text, i);

            if (isBreakOpportunity && inWord)
            {
                // End of a word - measure it
                var word = text.AsSpan(wordStart, i - wordStart);
                if (word.Length > 0)
                {
                    var measurement = _fontService.MeasureText(font, word);
                    maxWordWidth = Math.Max(maxWordWidth, measurement.Width);
                }
                inWord = false;
            }
            else if (!isBreakOpportunity && !inWord)
            {
                // Start of a new word
                wordStart = i;
                inWord = true;
            }
        }

        return maxWordWidth;
    }

    /// <summary>
    /// Determine if position i in text is a soft wrap opportunity.
    /// </summary>
    /// <remarks>
    /// Simplified soft wrap detection. Full implementation would consider:
    /// - Unicode line breaking algorithm (UAX #14)
    /// - CSS word-break, overflow-wrap properties
    /// - Language-specific rules
    /// </remarks>
    private static bool IsSoftWrapOpportunity(string text, int i)
    {
        if (i <= 0 || i >= text.Length)
            return false;

        char c = text[i];
        char prev = text[i - 1];

        // Space characters are wrap opportunities
        if (char.IsWhiteSpace(c))
            return true;

        // After certain punctuation
        if (prev == '-' || prev == '/')
            return true;

        // Before certain punctuation that typically starts new segments
        if (c == '(' || c == '[' || c == '{')
            return true;

        // CJK characters can break anywhere (simplified)
        if (IsCJKCharacter(c) || IsCJKCharacter(prev))
            return true;

        return false;
    }

    /// <summary>
    /// Check if character is in CJK range (simplified check).
    /// </summary>
    private static bool IsCJKCharacter(char c)
    {
        // CJK Unified Ideographs and common ranges
        return (c >= 0x4E00 && c <= 0x9FFF) ||  // CJK Unified Ideographs
               (c >= 0x3400 && c <= 0x4DBF) ||  // CJK Extension A
               (c >= 0x3000 && c <= 0x303F) ||  // CJK Symbols and Punctuation
               (c >= 0x3040 && c <= 0x309F) ||  // Hiragana
               (c >= 0x30A0 && c <= 0x30FF) ||  // Katakana
               (c >= 0xAC00 && c <= 0xD7AF);    // Hangul Syllables
    }

    /// <summary>
    /// Collect text content from an element and its descendants.
    /// </summary>
    private static string CollectTextContent(Element element)
    {
        // Use the element's text content if available
        return element.textContent ?? string.Empty;
    }

    /// <summary>
    /// Get the effective line height for the box.
    /// </summary>
    private static double GetLineHeight(CssPrincipalBox box, double fontLineHeight)
    {
        if (box.Style == null)
            return fontLineHeight;

        double lineHeight = box.Style.LineHeight;

        // If line-height is 'normal' or not set, use font metrics
        if (lineHeight <= 0)
            return fontLineHeight;

        return lineHeight;
    }

    /// <summary>
    /// Calculate intrinsic size for a text run with specific properties.
    /// </summary>
    public IntrinsicSize CalculateTextRun(
        ReadOnlySpan<char> text,
        FontHandle font,
        double lineHeight)
    {
        if (text.IsEmpty || font.IsNull)
            return IntrinsicSize.Zero;

        // Max-content: full text width
        var fullMeasurement = _fontService.MeasureText(font, text);
        double maxContentInline = fullMeasurement.Width;

        // Min-content: longest word
        double minContentInline = CalculateMinContentWidth(text.ToString(), font);

        // Use provided line height or fallback to measured height
        double blockSize = lineHeight > 0 ? lineHeight : fullMeasurement.Height;

        return new IntrinsicSize(
            new IntrinsicAxisSize(minContentInline, maxContentInline),
            IntrinsicAxisSize.Definite(blockSize));
    }
}
