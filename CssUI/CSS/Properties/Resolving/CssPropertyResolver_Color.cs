using System;
using System.Diagnostics.Contracts;
using CssUI.CSS.Enums;
using CssUI.Rendering;

namespace CssUI.CSS.Internal
{
    public static partial class CssPropertyResolver
    {

        public static CssValue Color_Specified(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            if (prop.Specified.Type != ECssValueTypes.KEYWORD)
                return prop.Specified;

            CssValue Value = prop.Specified;
            var keyword = Value.AsEnum<EColor>();
            switch (keyword)
            {
                case EColor.CurrentColor:
                    {/* If the ‘currentColor’ keyword is set on the ‘color’ property itself, it is treated as ‘color: inherit’. */
                        if (Property.CssName == ECssPropertyID.Color)
                        {
                            return Property.Find_Inherited_Value();
                        }
                    }
                    break;
            }

            return Value;
        }


        public static CssValue Color_Computed(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EColor>();
                switch (keyword)
                {
                    case EColor.CurrentColor:
                        {/* This gets translated in the "Used" stage */
                            return Value;
                        }
                    case EColor.Transparent:
                        {/* The computed value of the keyword ‘transparent’ is the quadruplet of all zero numerical RGBA values, e.g. rgba(0,0,0,0). */
                            return new CssValue(ECssValueTypes.COLOR, 0x0);
                        }
                    default:
                        {
                            if (!Lookup.TryData(keyword, out EnumData outData))
                            {
                                throw new CssPropertyException($"No meta-enum data found for keyword '{keyword}'");
                            }

                            return CssValue.From(new Color((int)outData.Data[1], (int)outData.Data[2], (int)outData.Data[3], (int)outData.Data[4]));
                        }
                }
            }

            return Value;
        }

        public static CssValue Color_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EColor>();
                switch (keyword)
                {
                    case EColor.CurrentColor:
                        {/* The used value of the ‘currentColor’ keyword is the computed value of the ‘color’ property. */
                            return Property.Owner.Style.Cascaded.Color.Computed;
                        }
                }
            }

            return Value;
        }


        public static CssValue Opacity_Computed(ICssProperty Property)
        {/* Docs: https://www.w3.org/TR/css-color-3/#opacity */
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            double n = Value.AsDecimal();
            if (n < 0d)
            {
                return CssValue.From(0d);
            }
            else if (n > 1d)
            {
                return CssValue.From(1d);
            }

            return Value;
        }

    }
}
