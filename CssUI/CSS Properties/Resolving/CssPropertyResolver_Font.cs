using CssUI.CSS;
using CssUI.Enums;
using CssUI.Fonts;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.Internal
{
    /*
     * In this file goes all the font related resolvers
     */
    public static partial class CssPropertyResolver
    {
        public static dynamic Font_Size_Used(ICssProperty Property)
        {// SEE: https://www.w3.org/TR/css-fonts-3/#font-size-prop
            double? v = (Property as CssProperty).Computed.Resolve();
            if (!v.HasValue)
                throw new CssException("Unable to resolve absolute length for Used value.");
            double Size = v.Value;
            if (Size < 9.0)// The specifications say we shouldnt render a font less than 9px in size
                Size = 9.0;

            // Round the size up to the nearest whole pixel value
            Size = Math.Round(Size, MidpointRounding.AwayFromZero);

            return CssValue.From_Number(Size);
        }

        public static dynamic Font_Weight_Computed(ICssProperty Property)
        {// SEE: https://drafts.csswg.org/css-fonts-4/#valdef-font-weight-bolder
            // handle font-weight related keywords
            // This function wouldnt even be called if the value werent a keyword
            string keyword = ((Property as CssProperty).Specified.Value as string).ToLower();
            int Weight = 400;

            switch (keyword)
            {
                case "normal":
                    Weight = 400;
                    break;
                case "bold":
                    Weight = 700;
                    break;
                case "bolder":
                    {// https://drafts.csswg.org/css-fonts-4/#relative-weights
                        CssValue inherited = Property.Find_Inherited_Value();
                        int w = inherited.Value;

                        if (w < 100) Weight = 400;
                        else if (w >= 100 && w < 350) Weight = 400;
                        else if (w >= 350 && w < 550) Weight = 700;
                        else if (w >= 550 && w < 750) Weight = 900;
                        else if (w >= 750 && w < 900) Weight = 900;
                        else Weight = 900;
                    }
                    break;
                case "lighter":
                    {// https://drafts.csswg.org/css-fonts-4/#relative-weights
                        CssValue inherited = Property.Find_Inherited_Value();
                        int w = inherited.Value;

                        if (w < 100) Weight = 100;
                        else if (w >= 100 && w < 350) Weight = 100;
                        else if (w >= 350 && w < 550) Weight = 100;
                        else if (w >= 550 && w < 750) Weight = 400;
                        else if (w >= 750 && w < 900) Weight = 700;
                        else Weight = 700;
                    }
                    break;

            }

            return CssValue.From_Int(Weight);
        }

        public static CssValue Font_Size_Restrict(ICssProperty Property)
        {
            double? v = (Property as CssProperty).Used.Resolve();
            if (!v.HasValue)
                throw new CssException("Unable to resolve absolute length for Used value.");

            // Per the CSS specifications we should not create a font under 9pt
            double val = Math.Max(v.Value, 9.0);

            return new CssValue(EStyleDataType.NUMBER, val);
        }

        /// <summary>
        /// Filters font family values
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        public static dynamic Font_Family_Used(ICssProperty Property)
        {
            CssValueList curValues = (Property as CssMultiValueProperty).Computed;
            List<CssValue> retValues = new List<CssValue>();

            foreach(CssValue val in curValues)
            {
                switch(val.Type)
                {
                    case EStyleDataType.KEYWORD:
                        {// Replace generic font-family keywords with a list of our fallback font-familys for that family
                            switch (val.Value as string)
                            {
                                case ECssGenericFontFamily.Serif:
                                case ECssGenericFontFamily.SansSerif:
                                case ECssGenericFontFamily.Monospace:
                                case ECssGenericFontFamily.Cursive:
                                case ECssGenericFontFamily.Fantasy:
                                    {
                                        if (FontManager.GenericFamilyMap.TryGetValue(val.Value as string, out List<CssValue> GenericFontFamilys))
                                            retValues.AddRange(GenericFontFamilys);
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException($"Unknown font-family keyword '{val.Value.ToString()}'");
                            }

                        }
                        break;
                    case EStyleDataType.STRING:
                        {// Remove any invalid font-familys
                            foreach(FontFamily family in SystemFonts.Families)
                            {
                                if (0 == Unicode.CaselessCompare(val.Value, family.Name))
                                {// Found it!
                                    retValues.Add(val);
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        {
                            retValues.Add(val);
                        }
                        break;
                }
            }

            return (retValues.Count > 0) ? new CssValueList(retValues) : null;
        }

    }
}
