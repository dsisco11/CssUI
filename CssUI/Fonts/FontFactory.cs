using CssUI.Enums;
using SixLabors.Fonts;
using SixLabors.Fonts.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using xLog;

namespace CssUI.Fonts
{
    /// <summary>
    /// Provides access to fonts, caching and reusing common instances
    /// </summary>
    public static class FontFactory
    {
        private static ConcurrentDictionary<FontOptions, WeakReference<Font>> Cache = new ConcurrentDictionary<FontOptions, WeakReference<Font>>();

        /// <summary>
        /// We kind of just chuck weak references to font instances into this dictionary and let them die on their own when all elements using them are disposed.
        /// </summary>

        private static Font Create_New_Font(FontOptions Options)
        {
            float Size = (float)Options.Size;
            FontStyle Style = FontStyle.Regular;
            switch (Options.Style)
            {
                case EFontStyle.Normal:
                    Style = FontStyle.Regular;
                    break;
                case EFontStyle.Italic:
                case EFontStyle.Oblique:
                    Style = FontStyle.Italic;
                    break;
            }
            if (Options.Weight >= 600) Style = FontStyle.Bold;

            FontFamily family = null;
            // Try to find the user specified fonts
            family = FontManager.Select_From_List(Options.Families, Style, Options.Weight);
            if (family == null)
            {
                family = FontManager.Select_From_List(FontManager.Fallbacks, Style, Options.Weight);
            }


            return family.CreateFont(Size, Style);
        }

        public static Font Get(FontOptions Options)
        {
            // Check if we have this font cached
            if (Cache.TryGetValue(Options, out WeakReference<Font> ptr))
            {// check if its still alive
                if (ptr.TryGetTarget(out Font f))
                    return f;
            }

            Font font = Create_New_Font(Options);

            if (font != null)
                Cache[Options] = new WeakReference<Font>(font);

            return font;
        }
    }
}
