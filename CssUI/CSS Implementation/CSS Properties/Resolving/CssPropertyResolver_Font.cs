using CssUI.CSS;
using CssUI.CSS.Internal;
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

        public static dynamic Font_Size_Computed(ICssProperty Property)
        {// SEE: https://www.w3.org/TR/css-fonts-3/#font-size-prop
            CssValue value = (Property as CssProperty).Specified;
            if (value == null)
            {
                throw new CssException($"Unable to resolve absolute length for Computed \"{Property.CssName}\" value.");
            }

            double Size = UserAgent.DEFAULT_FONT_SIZE; /* CssUIs' base font size */
            double Scaling = 1.0; /* Scaling to apply to our base size, used if the specified value is a keyword */


            if (value.HasValue)
            {
                if (value.IsDefinite)
                {
                    Size = (double)value.Value;
                }
                else
                {
                    switch (value.Type)
                    {
                        case ECssValueType.DIMENSION:
                            {
                                Scaling = 1.0;
                                Size = value.Resolve(Property.Owner.Style.unitResolver);
                            }
                            break;
                        case ECssValueType.KEYWORD:
                            {
                                string keyword = value.Value;
                                if (!CssLookup.TryEnum(keyword, out EFontSize outFontSize))
                                {
                                    throw new CssException($"");
                                    throw new CssException($"Unable to resolve absolute length for Computed \"{Property.CssName}\" value.");
                                }
                                switch (outFontSize)
                                {
                                    /* First off, if the value is an <absolute> size keyword we associate it with a scale and apply it to our base size */
                                    case EFontSize.XXSmall:
                                    case EFontSize.XSmall:
                                    case EFontSize.Small:
                                    case EFontSize.Medium:
                                    case EFontSize.Large:
                                    case EFontSize.XLarge:
                                    case EFontSize.XXLarge:
                                        Scaling = CssCommon.Get_Font_Size_Keyword_Scaling_Factor(outFontSize);
                                        break;
                                    /* Second, if the value is a <relative> size keyword we find the computed font size of our parent and increase/decrease it to get our value  */
                                    case EFontSize.Smaller:
                                        {
                                            double parentSize = (double)Property.Owner.Parent.Style.Cascaded.FontSize.Computed.Value;
                                            int scaleIndex = CssCommon.Get_Font_Scaling_Step_Index_From_Size(parentSize);
                                            Scaling = CssCommon.Get_Font_Scaling_From_Step_Index(scaleIndex - 1);
                                        }
                                        break;
                                    case EFontSize.Larger:
                                        {
                                            double parentSize = (double)Property.Owner.Parent.Style.Cascaded.FontSize.Computed.Value;
                                            int scaleIndex = CssCommon.Get_Font_Scaling_Step_Index_From_Size(parentSize);
                                            Scaling = CssCommon.Get_Font_Scaling_From_Step_Index(scaleIndex + 1);
                                        }
                                        break;
                                }
                            }
                            break;
                    }


                    double finalSize = Size * Scaling;
                    return CssValue.From_Number(finalSize);
                }
            }



            // Round the size up to the nearest whole pixel value
            Size = Math.Round(Size, MidpointRounding.AwayFromZero);
            return CssValue.From_Number(Size);
        }

        public static dynamic Font_Size_Used(ICssProperty Property)
        {// SEE: https://www.w3.org/TR/css-fonts-3/#font-size-prop
            double? v = (Property as CssProperty).Computed.Resolve();
            if (!v.HasValue)
            {
                throw new CssException($"Unable to resolve absolute length for Used \"{Property.CssName}\" value.");
            }

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
            if (!CssLookup.TryEnum(keyword, out EFontWeight outEnum))
            {
                throw new CssException($"Unable to resolve absolute length for Computed \"{Property.CssName}\" value.");
            }
            int Weight = 400;

            switch (outEnum)
            {
                case EFontWeight.Normal:
                    Weight = 400;
                    break;
                case EFontWeight.Bold:
                    Weight = 700;
                    break;
                case EFontWeight.Bolder:
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
                case EFontWeight.Lighter:
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
                    case ECssValueType.KEYWORD:
                        {// Replace generic font-family keywords with a list of our fallback font-familys for that family
                            string familyKeyword = val.Value as string;
                            if (!CssLookup.TryEnum(familyKeyword, out EGenericFontFamily outFamily))
                            {
                                throw new CssException($"Unable to resolve absolute length for Used \"{Property.CssName}\" value.");
                            }

                            switch (outFamily)
                            {
                                case EGenericFontFamily.Serif:
                                case EGenericFontFamily.SansSerif:
                                case EGenericFontFamily.Monospace:
                                case EGenericFontFamily.Cursive:
                                case EGenericFontFamily.Fantasy:
                                    {
                                        if (FontManager.GenericFamilyMap.TryGetValue(outFamily, out List<CssValue> GenericFontFamilys))
                                            retValues.AddRange(GenericFontFamilys);
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException($"Unknown font-family keyword '{val.Value.ToString()}'");
                            }

                        }
                        break;
                    case ECssValueType.STRING:
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
