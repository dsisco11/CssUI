using System;
using System.Diagnostics.Contracts;
using CssUI.CSS.Enums;

namespace CssUI.CSS.Internal
{
    public static partial class CssPropertyResolver
    {

        public static CssValue Border_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-backgrounds-3/#the-border-width
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            if (prop.Computed.Type != ECssValueTypes.KEYWORD)
                return prop.Computed;


            var keyword = prop.Computed.AsEnum<EBorderSize>();
            int Width = 0;


            switch (keyword)
            {
                case EBorderSize.Thin:
                    {
                        Width = 1;
                        break;
                    }
                case EBorderSize.Medium:
                    {
                        Width = 3;
                        break;
                    }
                case EBorderSize.Thick:
                    {
                        Width = 5;
                        break;
                    }
            }

            switch (Property.CssName.EnumValue)
            {
                case ECssPropertyID.BorderLeftWidth:
                    {
                        if (0 != (Property.Owner.Style.Border_Left_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From(0, ECssUnit.PX);
                    }
                    break;
                case ECssPropertyID.BorderTopWidth:
                    {
                        if (0 != (Property.Owner.Style.Border_Top_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From(0, ECssUnit.PX);
                    }
                    break;
                case ECssPropertyID.BorderRightWidth:
                    {
                        if (0 != (Property.Owner.Style.Border_Right_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From(0, ECssUnit.PX);
                    }
                    break;
                case ECssPropertyID.BorderBottomWidth:
                    {
                        if (0 != (Property.Owner.Style.Border_Bottom_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                            return CssValue.From(0, ECssUnit.PX);
                    }
                    break;
            }

            return CssValue.From(Width, ECssUnit.PX);
        }


        public static CssValue Min_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EBoxSize>();
                switch (keyword)
                {
                    case EBoxSize.Min_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Min_Content.Width);
                        }
                    case EBoxSize.Max_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Max_Content.Width);
                        }
                    case EBoxSize.Fit_Content:
                        {// XXX: Implement this!
                            throw new NotImplementedException();
                        }
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }
            else if (Value.Type == ECssValueTypes.AUTO)
            {
                /* 
                 * For min-width/min-height, specifies an automatic minimum size. 
                 * Unless otherwise defined by the relevant layout module, however, it resolves to a used value of 0. 
                 * For backwards-compatibility, the resolved value of this keyword is zero for boxes of all [CSS2] display types: block and inline boxes, inline blocks, and all the table layout boxes. 
                 * It also resolves to zero when no box is generated.
                 */

                //switch (prop.Owner.Box.DisplayGroup)
                //{
                //    case EBoxDisplayGroup.BLOCK:
                //    case EBoxDisplayGroup.INLINE:
                //    case EBoxDisplayGroup.INLINE_BLOCK:
                //    default:
                //        {
                //            return CssValue.Zero;
                //        }
                //}
                return CssValue.Zero;
            }

            return Value;
        }

        public static CssValue Min_Height_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EBoxSize>();
                switch (keyword)
                {
                    case EBoxSize.Min_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Min_Content.Height);
                        }
                    case EBoxSize.Max_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Max_Content.Height);
                        }
                    case EBoxSize.Fit_Content:
                        {// XXX: Implement this!
                            throw new NotImplementedException();
                        }
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }
            else if (Value.Type == ECssValueTypes.AUTO)
            {
                /* 
                 * For min-width/min-height, specifies an automatic minimum size. 
                 * Unless otherwise defined by the relevant layout module, however, it resolves to a used value of 0. 
                 * For backwards-compatibility, the resolved value of this keyword is zero for boxes of all [CSS2] display types: block and inline boxes, inline blocks, and all the table layout boxes. 
                 * It also resolves to zero when no box is generated.
                 */

                //switch (prop.Owner.Box.DisplayGroup)
                //{
                //    case EBoxDisplayGroup.BLOCK:
                //    case EBoxDisplayGroup.INLINE:
                //    case EBoxDisplayGroup.INLINE_BLOCK:
                //    default:
                //        {
                //            return CssValue.Zero;
                //        }
                //}
                return CssValue.Zero;
            }

            return Value;
        }

        public static CssValue Max_Width_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EBoxSize>();
                switch (keyword)
                {
                    case EBoxSize.Min_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Min_Content.Width);
                        }
                    case EBoxSize.Max_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Max_Content.Width);
                        }
                    case EBoxSize.Fit_Content:
                        {// XXX: Implement this!
                            throw new NotImplementedException();
                        }
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }

            return Value;
        }

        public static CssValue Max_Height_Used(ICssProperty Property)
        {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;

            if (Value.Type == ECssValueTypes.KEYWORD)
            {
                var keyword = Value.AsEnum<EBoxSize>();
                switch (keyword)
                {
                    case EBoxSize.Min_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Min_Content.Height);
                        }
                    case EBoxSize.Max_Content:
                        {
                            if (prop.Owner.Style.WritingMode != EWritingMode.Horizontal_TB)
                                return prop.Definition.Initial;

                            return CssValue.From(prop.Owner.Box.Max_Content.Height);
                        }
                    case EBoxSize.Fit_Content:
                        {// XXX: Implement this!
                            throw new NotImplementedException();
                        }
                    default:
                        throw new NotImplementedException($"Keyword '{keyword}' is not implemented!");
                }
            }

            return Value;
        }



        public static CssValue Definite_Or_Zero_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }


        public static CssValue Box_Top_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }

        public static CssValue Box_Right_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }

        public static CssValue Box_Bottom_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }

        public static CssValue Box_Left_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }



        public static CssValue Position_Computed(ICssProperty Property)
        {/* Docs: https://www.w3.org/TR/css-backgrounds-3/#propdef-background-position */
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Specified;
            if (!Value.IsCollection)
            {
                throw new CssPropertyException($"Cannot resolve computed value for '{Property.CssName}' because it is not a collection");
            }

            //switch

            return Value;
        }

        public static CssValue Position_Used(ICssProperty Property)
        {
            if (Property is null) throw new ArgumentNullException(nameof(Property));
            Contract.EndContractBlock();

            var prop = (Property as CssProperty);
            CssValue Value = prop.Computed;
            if (!Value.IsDefinite)
            {
                return CssValue.From(0);
            }
            return Value;
        }
    }
}
