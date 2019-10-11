using System;
using System.Diagnostics.Contracts;
using CssUI.Internal;
#if DISABLE_FONT_SYSTEM
#else
using System.Collections.Generic;
using System.Text;
using CssUI.CSS;
using CssUI.Enums;
using CssUI.Fonts;
using SixLabors.Fonts;
#endif

namespace CssUI.CSS.Internal
{
    /*
     * In this file goes all the font related resolvers
     */
    public static partial class CssPropertyResolver
    {

        public static CssValue Font_Size_Computed(ICssProperty Property)
        {/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */
            if (Property == null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            CssValue value = (Property as CssProperty).Specified;
            if (value is null)
            {
                throw new CssException($"Unable to resolve absolute length for Computed \"{Property.CssName}\" value.");
            }
            Contract.EndContractBlock();

            double Size = UserAgent.DEFAULT_FONT_SIZE; /* CssUIs' base font size */
            double Scaling = 1.0; /* Scaling to apply to our base size, used if the specified value is a keyword */


            if (value.HasValue)
            {
                if (value.IsDefinite)
                {
                    Size = value.AsDecimal();
                }
                else
                {
                    switch (value.Type)
                    {
                        case ECssValueTypes.DIMENSION:
                            {
                                Scaling = 1.0;
                                Size = value.Resolve(Property.Owner.ownerDocument.cssUnitResolver);
                            }
                            break;
                        case ECssValueTypes.KEYWORD:
                            {
                                var keyword = value.AsEnum<EFontSize>();
                                switch (keyword)
                                {
                                    /* First off, if the value is an <absolute> size keyword we associate it with a scale and apply it to our base size */
                                    case EFontSize.XXSmall:
                                    case EFontSize.XSmall:
                                    case EFontSize.Small:
                                    case EFontSize.Medium:
                                    case EFontSize.Large:
                                    case EFontSize.XLarge:
                                    case EFontSize.XXLarge:
                                        Scaling = CssCommon.Get_Font_Size_Keyword_Scaling_Factor(keyword);
                                        break;
                                    /* Second, if the value is a <relative> size keyword we find the computed font size of our parent and increase/decrease it to get our value  */
                                    case EFontSize.Smaller:
                                        {
                                            double parentSize = Property.Owner.Style.Cascaded.FontSize.Computed.AsDecimal();
                                            int scaleIndex = CssCommon.Get_Font_Scaling_Step_Index_From_Size(parentSize);
                                            Scaling = CssCommon.Get_Font_Scaling_From_Step_Index(scaleIndex - 1);
                                        }
                                        break;
                                    case EFontSize.Larger:
                                        {
                                            double parentSize = Property.Owner.Style.Cascaded.FontSize.Computed.AsDecimal();
                                            int scaleIndex = CssCommon.Get_Font_Scaling_Step_Index_From_Size(parentSize);
                                            Scaling = CssCommon.Get_Font_Scaling_From_Step_Index(scaleIndex + 1);
                                        }
                                        break;
                                }
                            }
                            break;
                    }


                    double finalSize = Size * Scaling;
                    return CssValue.From(finalSize);
                }
            }



            // Round the size up to the nearest whole pixel value
            Size = CssCommon.SnapToPixel(Size);
            return CssValue.From(Size);
        }

        public static CssValue Font_Size_Used(ICssProperty Property)
        {/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */
            if (Property == null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            double? v = (Property as CssProperty).Computed.Resolve();
            if (!v.HasValue)
            {
                throw new CssException($"Unable to resolve absolute length for Used \"{Property.CssName}\" value.");
            }
            Contract.EndContractBlock();

            double Size = v.Value;
            if (Size < 9.0)// The specifications say we shouldnt render a font less than 9px in size
                Size = 9.0;

            // Round the size up to the nearest whole pixel value
            Size = Math.Round(Size, MidpointRounding.AwayFromZero);

            return CssValue.From(Size);
        }

        public static CssValue Font_Weight_Computed(ICssProperty Property)
        {/* Docs: https://drafts.csswg.org/css-fonts-4/#valdef-font-weight-bolder */
            if (Property == null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            // handle font-weight related keywords
            // This function wouldnt even be called if the value werent a keyword
            var keyword = (Property as CssProperty).Specified.AsEnum<EFontWeight>();
            int Weight = 400;

            switch (keyword)
            {
                case EFontWeight.Normal:
                    Weight = 400;
                    break;
                case EFontWeight.Bold:
                    Weight = 700;
                    break;
                case EFontWeight.Bolder:
                    {/* Docs: https://drafts.csswg.org/css-fonts-4/#relative-weights */
                        CssValue inherited = Property.Find_Inherited_Value();
                        int w = inherited.AsInteger();

                        if (w < 100) Weight = 400;
                        else if (w >= 100 && w < 350) Weight = 400;
                        else if (w >= 350 && w < 550) Weight = 700;
                        else if (w >= 550 && w < 750) Weight = 900;
                        else if (w >= 750 && w < 900) Weight = 900;
                        else Weight = 900;
                    }
                    break;
                case EFontWeight.Lighter:
                    {/* Docs: https://drafts.csswg.org/css-fonts-4/#relative-weights */
                        CssValue inherited = Property.Find_Inherited_Value();
                        int w = inherited.AsInteger();

                        if (w < 100) Weight = 100;
                        else if (w >= 100 && w < 350) Weight = 100;
                        else if (w >= 350 && w < 550) Weight = 100;
                        else if (w >= 550 && w < 750) Weight = 400;
                        else if (w >= 750 && w < 900) Weight = 700;
                        else Weight = 700;
                    }
                    break;

            }

            return CssValue.From(Weight);
        }

        /// <summary>
        /// Filters font family values
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        public static CssValueList Font_Family_Used(ICssProperty Property)
        {
            if (Property == null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

#if DISABLE_FONT_SYSTEM
            return null;
#else
            CssValueList curValues = (Property as CssMultiValueProperty).Computed;
            List<CssValue> retValues = new List<CssValue>();

            foreach (CssValue val in curValues)
            {
                switch (val.Type)
                {
                    case ECssValueType.KEYWORD:
                        {// Replace generic font-family keywords with a list of our fallback font-familys for that family
                            var familyKeyword = val.AsEnum<EGenericFontFamily>();

                            switch (familyKeyword)
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
                            foreach (FontFamily family in SystemFonts.Families)
                            {
                                if (0 == Unicode.CaselessCompare(val.AsString, family.Name))
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
#endif
        }
    }
}
