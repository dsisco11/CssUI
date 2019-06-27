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
        private static List<string> Fallbacks = new List<string>();

        static FontFactory()
        {
            // Populate our fallbacks list
            Fallbacks.Add("calibri");
            Fallbacks.Add("sans-serif");
            Fallbacks.Add("verdana");
            Fallbacks.Add("arial");

            foreach (FontFamily family in SystemFonts.Families)
            {
                Fallbacks.Add(family.Name);
            }

            // Cleanup the list ahead of time


            List<string> failedFallbacks = new List<string>();
            foreach (string familyName in Fallbacks)
            {
                if (!SystemFonts.TryFind(familyName, out FontFamily family))
                {
                    failedFallbacks.Add(familyName);
                }
            }
            // Remove all missing familys
            foreach (string fn in failedFallbacks)
            {
                Fallbacks.Remove(fn);
            }

        }
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

            Font font = null;
            // Try to find the user specified fonts
            font = Select_From_List(Options.Families, Size, Style);
            if (font==null)
            {
                font = Select_From_List(Fallbacks, Size, Style);
            }

            return font;
        }

        private static Font Select_From_List(ICollection<string> Familys, float Size, FontStyle Style)
        {
            Font font = null;
            foreach (string name in Familys)
            {
                if (string.IsNullOrEmpty(name)) continue;
                try
                {
                    if (SystemFonts.TryFind(name, out FontFamily family))
                    {
                        font = family.CreateFont(Size, Style);
                        if (font != null)
                            break;
                    }
                }
                catch (FontException ex)
                {
                    xLog.Log.Error(ex);
                }
            }

            return font;
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
