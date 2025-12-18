using System;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Engine interface for font resolution, metrics, and text measurement.
    /// Implementations handle font loading and caching internally.
    /// </summary>
    public interface IFontEngine
    {
        /// <summary>
        /// Resolve a font from CSS font properties with fallback chain.
        /// </summary>
        /// <param name="familyNames">Font family names in priority order.</param>
        /// <param name="size">Font size in pixels.</param>
        /// <param name="weight">Font weight (100-900).</param>
        /// <param name="style">Font style (normal, italic, oblique).</param>
        /// <returns>A handle to the resolved font, or <see cref="FontHandle.Null"/> if resolution fails.</returns>
        FontHandle ResolveFont(ReadOnlySpan<string> familyNames, float size, EFontWeight weight, EFontStyle style);

        /// <summary>
        /// Resolve a font using a generic CSS font family.
        /// </summary>
        FontHandle ResolveFont(EGenericFontFamily genericFamily, float size, EFontWeight weight, EFontStyle style);

        /// <summary>
        /// Get the system default/fallback font.
        /// </summary>
        /// <param name="size">Font size in pixels.</param>
        /// <returns>A handle to the default font.</returns>
        FontHandle GetDefaultFont(float size);

        /// <summary>
        /// Get font metrics for CSS unit resolution (em, ex, ch, lh).
        /// </summary>
        /// <param name="font">The font handle.</param>
        /// <returns>Font metrics, or default values if handle is invalid.</returns>
        FontMetricsData GetMetrics(FontHandle font);

        /// <summary>
        /// Measure text dimensions for layout and intrinsic sizing.
        /// </summary>
        /// <param name="font">The font handle.</param>
        /// <param name="text">The text to measure.</param>
        /// <returns>Text measurement result.</returns>
        TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text);

        /// <summary>
        /// Measure text with a maximum width constraint (for wrapping calculations).
        /// </summary>
        /// <param name="font">The font handle.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="maxWidth">Maximum width before wrapping.</param>
        /// <returns>Text measurement result.</returns>
        TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text, float maxWidth);

        /// <summary>
        /// Get the advance width of a character (for 'ch' CSS unit).
        /// </summary>
        /// <param name="font">The font handle.</param>
        /// <param name="c">The character to measure.</param>
        /// <returns>The advance width in pixels.</returns>
        float GetCharAdvance(FontHandle font, char c);

        /// <summary>
        /// Check if a font handle is still valid.
        /// </summary>
        bool IsValid(FontHandle font);

        /// <summary>
        /// Release a font handle when no longer needed.
        /// The engine may cache fonts internally, so this is a hint that the handle
        /// is no longer in use rather than an immediate release.
        /// </summary>
        void Release(FontHandle font);
    }
}

