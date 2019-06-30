using CssUI.CSS;
using CssUI.Enums;
using CssUI.Internal;
using SixLabors.Fonts;
using SixLabors.Fonts.Exceptions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace CssUI.Fonts
{
    // DOCS: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/
    /// <summary>
    /// Provides Font Family fallbacks
    /// </summary>
    public static class FontManager
    {
        #region Propreties
        public static ConcurrentDictionary<AtomicString, List<CssValue>> GenericFamilyMap = new ConcurrentDictionary<AtomicString, List<CssValue>>();
        public static readonly List<string> Fallbacks = new List<string>();
        #endregion

        #region Constructor
        static FontManager()
        {
            Populate_Fallbacks();

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            switch(currentCulture.Name.ToLowerInvariant())
            {
                case EISO_15924.Latin:
                    Setup_Latin_Script_FontFamilys();
                    break;
                case EISO_15924.Greek:
                    Setup_Greek_Script_FontFamilys();
                    break;
                case EISO_15924.Cyrillic:
                    Setup_Cyrillic_Script_FontFamilys();
                    break;
                case EISO_15924.Japanese_alias_for_Han__Hiragana__Katakana:
                    Setup_Japanese_Script_FontFamilys();
                    break;
                case EISO_15924.Hebrew:
                    Setup_Hebrew_Script_FontFamilys();
                    break;
                case EISO_15924.Cherokee:
                    Setup_Cheroke_Script_FontFamilys();
                    break;
                case EISO_15924.Arabic:
                    Setup_Arabic_Script_FontFamilys();
                    break;
                default:
                    Setup_Latin_Script_FontFamilys();
                    break;

            }

        }
        #endregion

        #region FontFamily Fallback Population
        /// <summary>
        /// Populates the list of fallback font-family to use when the ones specified by a user dont work out.
        /// </summary>
        private static void Populate_Fallbacks()
        {
            // Populate our fallbacks list
            Fallbacks.Add("verdana");
            Fallbacks.Add("courier");
            Fallbacks.Add("arial");
            Fallbacks.Add("calibri");
            Fallbacks.Add("sans-serif");

            foreach (FontFamily family in SystemFonts.Families)
            {
                Fallbacks.Add(family.Name);
                xLog.Log.Info($"[FontFactory] Found font family: {family.Name}");
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
        #endregion

        #region Font Family Setup

        public static void Setup_Latin_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Times New Roman"), CssValue.From_String("Bodoni"), CssValue.From_String("Garamond"), CssValue.From_String("Minion Web"), CssValue.From_String("ITC Stone Serif"), CssValue.From_String("MS Georgia"), CssValue.From_String("Bitstream Cyberbit") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("MS Trebuchet"), CssValue.From_String("ITC Avant Garde Gothic"), CssValue.From_String("MS Arial"), CssValue.From_String("MS Verdana"), CssValue.From_String("Univers"), CssValue.From_String("Futura"), CssValue.From_String("ITC Stone Sans"), CssValue.From_String("Gill Sans"), CssValue.From_String("Akzidenz Grotesk"), CssValue.From_String("Helvetica") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { CssValue.From_String("Courier"), CssValue.From_String("MS Courier New"), CssValue.From_String("Prestige"), CssValue.From_String("Everson Mono") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() { CssValue.From_String("Caflisch Script"), CssValue.From_String("Adobe Poetica"), CssValue.From_String("Sanvito"), CssValue.From_String("Ex Ponto"), CssValue.From_String("Snell Roundhand"), CssValue.From_String("Zapf-Chancery") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() { CssValue.From_String("Alpha Geometrique"), CssValue.From_String("Critter"), CssValue.From_String("Cottonwood"), CssValue.From_String("FB Reactor"), CssValue.From_String("Studz") });
        }

        public static void Setup_Greek_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Bitstream Cyberbit") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("Attika"), CssValue.From_String("Typiko New Era"), CssValue.From_String("MS Tahoma"), CssValue.From_String("Monotype Gill Sans 571"), CssValue.From_String("Helvetica Greek") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { CssValue.From_String("MS Courier New"), CssValue.From_String("Everson Mono") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() {  });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() {  });
        }

        public static void Setup_Cyrillic_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Adobe Minion Cyrillic"), CssValue.From_String("Excelsior Cyrillic Upright"), CssValue.From_String("Monotype Albion 70"), CssValue.From_String("Bitstream Cyberbit"), CssValue.From_String("ER Bukinist") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("Helvetica Cyrillic"), CssValue.From_String("ER Univers"), CssValue.From_String("Lucida Sans Unicode"), CssValue.From_String("Bastion") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { CssValue.From_String("ER Kurier"), CssValue.From_String("Everson Mono") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() { CssValue.From_String("ER Architekt") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() {  });
        }

        public static void Setup_Japanese_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Ryumin Light-KL"), CssValue.From_String("Kyokasho ICA"), CssValue.From_String("Futo Min A101") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("Shin Go"), CssValue.From_String("Heisei Kaku Gothic W5") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { CssValue.From_String("Osaka Monospaced") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() {  });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() {  });
        }

        public static void Setup_Hebrew_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("New Peninim"), CssValue.From_String("Raanana"), CssValue.From_String("Bitstream Cyberbit") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("Arial Hebrew"), CssValue.From_String("MS Tahoma") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() {  });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() { CssValue.From_String("Corsiva") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() { });
        }

        public static void Setup_Cheroke_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Lo Cicero Cherokee") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() {  });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { CssValue.From_String("Everson Mono") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() {  });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() { });
        }

        public static void Setup_Arabic_Script_FontFamilys()
        {
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Serif, new List<CssValue>() { CssValue.From_String("Bitstream Cyberbit") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.SansSerif, new List<CssValue>() { CssValue.From_String("MS Tahoma") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Monospace, new List<CssValue>() { });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Cursive, new List<CssValue>() { CssValue.From_String("DecoType Naskh"), CssValue.From_String("Monotype Urdu 507") });
            GenericFamilyMap.TryAdd(ECssGenericFontFamily.Fantasy, new List<CssValue>() { });
        }
        #endregion


        #region Font resolution

        /// <summary>
        /// Translates a given font weight value into commonly used names for it
        /// </summary>
        /// <param name="Weight"></param>
        /// <returns></returns>
        private static string[] Translate_Font_Weight_To_Name(int Weight)
        {
            if (Weight <= 100) return new string[]{ "Thin" };
            else if (Weight <= 200) return new string[] { "Extra Light", "Ultra Light" };
            else if (Weight <= 300) return new string[] { "Light" };
            else if (Weight <= 400) return new string[] { "Normal" };
            else if (Weight <= 500) return new string[] { "Medium" };
            else if (Weight <= 600) return new string[] { "Semi Bold", "Demi Bold" };
            else if (Weight <= 700) return new string[] { "Bold" };
            else if (Weight <= 800) return new string[] { "Extra Bold", "Ultra Bold" };
            else if (Weight <= 900) return new string[] { "Black", "Heavy" };

            return  new string[0];
        }

        /// <summary>
        /// Attempts to pick the most ideal font available matching the given parameters
        /// </summary>
        /// <param name="Familys"></param>
        /// <param name="Style"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public static FontFamily Select_From_List(IEnumerable<string> Familys, FontStyle Style, int Weight)
        {
            // FIRST, we  prioritize the weight, and try to find a font that has a version for the desired weight
            string[] weights = Translate_Font_Weight_To_Name(Weight);
            try
            {
                foreach (string name in Familys)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    // try and find a weighted version of the font
                    foreach (string wPost in weights)
                    {
                        string weightedName = $"{name} {wPost}";
                        if (SystemFonts.TryFind(weightedName, out FontFamily family))
                        {
                            if (family.IsStyleAvailable(Style))
                            {
                                return family;
                            }
                        }
                    }
                }
            }
            catch (FontException ex)
            {
                xLog.Log.Error(ex);
            }

            // SECOND, we  prioritize the style, and try to find a font that supports this style
            try
            {
                foreach (string name in Familys)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    if (SystemFonts.TryFind(name, out FontFamily family))
                    {
                        if (family.IsStyleAvailable(Style))
                        {
                            return family;
                        }
                    }
                }
            }
            catch (FontException ex)
            {
                xLog.Log.Error(ex);
            }



            // LAST, we just try and find ANY font
            try
            {
                foreach (string name in Familys)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    if (SystemFonts.TryFind(name, out FontFamily family))
                    {
                        return family;
                    }
                }
            }
            catch (FontException ex)
            {
                xLog.Log.Error(ex);
            }

            return null;
        }
        #endregion
    }
}
