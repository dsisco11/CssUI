using CssUI.CSS;
using CssUI.Enums;
using System;

namespace CssUI.Internal
{
    public static partial class CssPropertyResolver
    {

        public static dynamic Border_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-backgrounds-3/#the-border-width
            var prop = (Property as CssProperty);
            if (prop.Computed.Type != EStyleDataType.KEYWORD)
                return prop.Computed;


            string keyword = (prop.Specified.Value as string).ToLower();
            int Width = 0;

            switch (keyword)
            {
                case "thin":
                    Width = 1;
                    break;
                case "medium":
                    Width = 3;
                    break;
                case "thick":
                    Width = 5;
                    break;
            }

            switch (Property.CssName)
            {
                case "border-left-width":
                    {
                        if (0 != (Property.Owner.Style.Border_Left_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From_Length(0, EStyleUnit.PX);
                    }
                    break;
                case "border-top-width":
                    {
                        if (0 != (Property.Owner.Style.Border_Top_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From_Length(0, EStyleUnit.PX);
                    }
                    break;
                case "border-right-width":
                    {
                        if (0 != (Property.Owner.Style.Border_Right_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From_Length(0, EStyleUnit.PX);
                    }
                    break;
                case "border-bottom-width":
                    {
                        if (0 != (Property.Owner.Style.Border_Bottom_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From_Length(0, EStyleUnit.PX);
                    }
                    break;
            }

            return CssValue.From_Length(Width, EStyleUnit.PX);
        }


        public static dynamic Min_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == EStyleDataType.KEYWORD)
            {
                string keyword = (Value.Value as string).ToLower();
                switch (keyword)
                {
                    case "min-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Min_Content.Width);
                        }
                        break;
                    case "max-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Max_Content.Width);
                        }
                        break;
                    case "fit-content":
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }
            else if (Value.Type == EStyleDataType.AUTO)
            {
                /* 
                 * For min-width/min-height, specifies an automatic minimum size. 
                 * Unless otherwise defined by the relevant layout module, however, it resolves to a used value of 0. 
                 * For backwards-compatibility, the resolved value of this keyword is zero for boxes of all [CSS2] display types: block and inline boxes, inline blocks, and all the table layout boxes. 
                 * It also resolves to zero when no box is generated.
                 */

                switch (prop.Owner.Box.DisplayGroup)
                {
                    case EBoxDisplayGroup.BLOCK:
                    case EBoxDisplayGroup.INLINE:
                    case EBoxDisplayGroup.INLINE_BLOCK:
                    default:
                        {
                            return CssValue.Zero;
                        }
                        break;
                }
            }

            return Value;
        }

        public static dynamic Min_Height_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == EStyleDataType.KEYWORD)
            {
                string keyword = (Value.Value as string).ToLower();
                switch (keyword)
                {
                    case "min-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Min_Content.Height);
                        }
                        break;
                    case "max-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Max_Content.Height);
                        }
                        break;
                    case "fit-content":
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }
            else if (Value.Type == EStyleDataType.AUTO)
            {
                /* 
                 * For min-width/min-height, specifies an automatic minimum size. 
                 * Unless otherwise defined by the relevant layout module, however, it resolves to a used value of 0. 
                 * For backwards-compatibility, the resolved value of this keyword is zero for boxes of all [CSS2] display types: block and inline boxes, inline blocks, and all the table layout boxes. 
                 * It also resolves to zero when no box is generated.
                 */

                switch (prop.Owner.Box.DisplayGroup)
                {
                    case EBoxDisplayGroup.BLOCK:
                    case EBoxDisplayGroup.INLINE:
                    case EBoxDisplayGroup.INLINE_BLOCK:
                    default:
                        {
                            return CssValue.Zero;
                        }
                        break;
                }
            }

            return Value;
        }

        public static dynamic Max_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == EStyleDataType.KEYWORD)
            {
                string keyword = (Value.Value as string).ToLower();
                switch (keyword)
                {
                    case "min-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Min_Content.Width);
                        }
                        break;
                    case "max-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Max_Content.Width);
                        }
                        break;
                    case "fit-content":
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }

            return Value;
        }

        public static dynamic Max_Height_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == EStyleDataType.KEYWORD)
            {
                string keyword = (Value.Value as string).ToLower();
                switch (keyword)
                {
                    case "min-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Min_Content.Height);
                        }
                        break;
                    case "max-content":
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From_Int(prop.Owner.Box.Max_Content.Height);
                        }
                        break;
                    case "fit-content":
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }

            return Value;
        }
    }
}
