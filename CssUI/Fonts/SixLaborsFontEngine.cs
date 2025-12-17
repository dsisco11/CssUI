using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CssUI.CSS;

#if !DISABLE_FONT_SYSTEM
using SixLabors.Fonts;
#endif

namespace CssUI.Fonts;

/// <summary>
/// IFontEngine implementation using SixLabors.Fonts.
/// </summary>
public sealed class SixLaborsFontEngine : IFontEngine
{
#if DISABLE_FONT_SYSTEM
    // Stub implementation when font system is disabled
    
    public FontHandle ResolveFont(ReadOnlySpan<string> familyNames, float size, EFontWeight weight, EFontStyle style)
        => FontHandle.Null;
    
    public FontHandle ResolveFont(EGenericFontFamily genericFamily, float size, EFontWeight weight, EFontStyle style)
        => FontHandle.Null;
    
    public FontHandle GetDefaultFont(float size)
        => FontHandle.Null;
    
    public FontMetricsData GetMetrics(FontHandle font)
        => new FontMetricsData { EmSize = 16f, XHeight = 8f, CapHeight = 12f, Ascender = 14f, Descender = -4f, LineGap = 2f };
    
    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text)
        => new TextMeasurement { Width = text.Length * 8f, Height = 16f, Baseline = 12f };
    
    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text, float maxWidth)
        => MeasureText(font, text);
    
    public float GetCharAdvance(FontHandle font, char c)
        => 8f;
    
    public bool IsValid(FontHandle font)
        => false;
    
    public void Release(FontHandle font) { }

#else
    
    private int _nextId = 1;
    private readonly ConcurrentDictionary<int, Font> _fonts = new();
    private readonly ConcurrentDictionary<FontOptions, int> _optionsToId = new();
    
    public FontHandle ResolveFont(ReadOnlySpan<string> familyNames, float size, EFontWeight weight, EFontStyle style)
    {
        var families = new List<string>();
        foreach (var name in familyNames)
        {
            if (!string.IsNullOrEmpty(name))
                families.Add(name);
        }
        
        var options = new FontOptions(families, size, (int)weight, style);
        return GetOrCreateFont(options);
    }
    
    public FontHandle ResolveFont(EGenericFontFamily genericFamily, float size, EFontWeight weight, EFontStyle style)
    {
        // Get family names from FontManager's generic family map
        var families = new List<string>();
        if (FontManager.GenericFamilyMap.TryGetValue(genericFamily, out var cssValues))
        {
            foreach (var val in cssValues)
            {
                if (val.Type == ECssValueTypes.STRING)
                    families.Add(val.AsString());
            }
        }
        
        // Add fallbacks
        families.AddRange(FontManager.Fallbacks);
        
        var options = new FontOptions(families, size, (int)weight, style);
        return GetOrCreateFont(options);
    }
    
    public FontHandle GetDefaultFont(float size)
    {
        return ResolveFont(EGenericFontFamily.SansSerif, size, EFontWeight.Normal, EFontStyle.Normal);
    }
    
    public FontMetricsData GetMetrics(FontHandle font)
    {
        if (!_fonts.TryGetValue(font.Id, out var f))
        {
            return new FontMetricsData
            {
                EmSize = 16f,
                XHeight = 8f,
                CapHeight = 12f,
                Ascender = 14f,
                Descender = -4f,
                LineGap = 2f
            };
        }
        
        // SixLabors.Fonts provides metrics through the Font object
        var emSize = f.Size;
        
        // Approximate metrics based on em size
        // These would need proper implementation using font tables
        return new FontMetricsData
        {
            EmSize = emSize,
            XHeight = emSize * 0.5f,      // Approximate x-height
            CapHeight = emSize * 0.7f,    // Approximate cap height
            Ascender = emSize * 0.8f,     // Approximate ascender
            Descender = -emSize * 0.2f,   // Approximate descender
            LineGap = emSize * 0.1f       // Approximate line gap
        };
    }
    
    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text)
    {
        if (!_fonts.TryGetValue(font.Id, out var f))
        {
            // Fallback measurement
            return new TextMeasurement
            {
                Width = text.Length * 8f,
                Height = 16f,
                Baseline = 12f
            };
        }
        
        var str = new string(text);
        var size = TextMeasurer.Measure(str, new RendererOptions(f));
        
        return new TextMeasurement
        {
            Width = size.Width,
            Height = size.Height,
            Baseline = f.Size * 0.8f  // Approximate baseline
        };
    }
    
    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text, float maxWidth)
    {
        if (!_fonts.TryGetValue(font.Id, out var f))
        {
            return MeasureText(font, text);
        }
        
        var str = new string(text);
        var options = new RendererOptions(f)
        {
            WrappingWidth = maxWidth
        };
        var size = TextMeasurer.Measure(str, options);
        
        return new TextMeasurement
        {
            Width = size.Width,
            Height = size.Height,
            Baseline = f.Size * 0.8f
        };
    }
    
    public float GetCharAdvance(FontHandle font, char c)
    {
        if (!_fonts.TryGetValue(font.Id, out var f))
        {
            return 8f;
        }
        
        var size = TextMeasurer.Measure(c.ToString(), new RendererOptions(f));
        return size.Width;
    }
    
    public bool IsValid(FontHandle font)
    {
        return !font.IsNull && _fonts.ContainsKey(font.Id);
    }
    
    public void Release(FontHandle font)
    {
        // Fonts are cached, so we don't actually release them immediately
        // They will be garbage collected when no longer referenced
    }
    
    private FontHandle GetOrCreateFont(FontOptions options)
    {
        // Check if we already have this font
        if (_optionsToId.TryGetValue(options, out var existingId))
        {
            if (_fonts.ContainsKey(existingId))
                return new FontHandle(existingId);
        }
        
        // Create new font
        var font = CreateFont(options);
        if (font == null)
            return FontHandle.Null;
        
        var id = _nextId++;
        _fonts[id] = font;
        _optionsToId[options] = id;
        
        return new FontHandle(id);
    }
    
    private Font? CreateFont(FontOptions options)
    {
        float size = (float)options.Size;
        FontStyle style = FontStyle.Regular;
        
        switch (options.Style)
        {
            case EFontStyle.Normal:
                style = FontStyle.Regular;
                break;
            case EFontStyle.Italic:
            case EFontStyle.Oblique:
                style = FontStyle.Italic;
                break;
        }
        
        if (options.Weight >= 600) 
            style = FontStyle.Bold;
        
        // Try to find a matching font family
        FontFamily family = FontManager.Select_From_List(options.Families, style, options.Weight);
        if (family == default)
        {
            family = FontManager.Select_From_List(FontManager.Fallbacks, style, options.Weight);
        }
        
        if (family == default)
            return null;
        
        return family.CreateFont(size, style);
    }
#endif
}
