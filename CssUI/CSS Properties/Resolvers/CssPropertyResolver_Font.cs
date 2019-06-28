using CssUI.CSS;
using SixLabors.Fonts;
using System;

namespace CssUI.Internal
{
    /*
     * In this file goes all the font related resolvers
     */
    public static partial class CssPropertyResolver
    {

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

        public static dynamic Font_Family_Used(ICssProperty Property)
        {// Replace generic family identifiers with a list of our fallback font for that family
            CssValueList Values = (Property as CssMultiValueProperty).Specified;


        }

        public static dynamic Font_Family_Restrict(ICssProperty Property)
        {
            string familyName = (Property as CssMultiValueProperty).Used;
            // check if the font exhists
            SystemFonts.TryFind(familyName, out FontFamily Family);
        }
    }
}
