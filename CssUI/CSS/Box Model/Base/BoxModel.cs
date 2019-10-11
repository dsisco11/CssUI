using CssUI.DOM;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using System.Runtime.CompilerServices;
using CssUI.CSS.BoxTree;
using System.Diagnostics.Contracts;
using System;

namespace CssUI.CSS
{
    /* PREFACE:
     * An explination for some of the inlining here.
     * Some of these functions are private and only called a few times by the other algorithms, 
     * a minor optimization is to remove a jmp call for these functions. 
     * Over the course of MANY nodes this could have a positive impact and the memory impact should be very negligable.
     * So I'm sacrificing a few KB of memory for a little speed since these functions are called a lot.
     */


    public static class BoxModel
    {

        public static void Resolve(CssPrincipalBox Box, CssComputedStyle Cascaded)
        {
            if (Box is null) throw new ArgumentNullException(nameof(Box));
            if (Cascaded is null) throw new ArgumentNullException(nameof(Cascaded));
            Contract.EndContractBlock();


            Resolve_Box_Properties_Used_Value(Box, Cascaded, out CssValue Left, out CssValue MarginLeft, out CssValue Width, out CssValue MarginRight, out CssValue Right, out CssValue Top, out CssValue MarginTop, out CssValue Height, out CssValue MarginBottom, out CssValue Bottom);

            Cascaded.Width.Set_Computed_Value(Width);
            Cascaded.Height.Set_Computed_Value(Height);

            Cascaded.Top.Set_Computed_Value(Top);
            Cascaded.Right.Set_Computed_Value(Right);
            Cascaded.Bottom.Set_Computed_Value(Bottom);
            Cascaded.Left.Set_Computed_Value(Left);

            Cascaded.Margin_Top.Set_Computed_Value(MarginTop);
            Cascaded.Margin_Right.Set_Computed_Value(MarginRight);
            Cascaded.Margin_Bottom.Set_Computed_Value(MarginBottom);
            Cascaded.Margin_Left.Set_Computed_Value(MarginLeft);
        }
        /// <summary>
        /// Resolves 'Used' values for the following properties:
        /// Width, Height, Top, Right, Bottom, Left, Margin-Left, Margin-Right
        /// </summary>
        private static void Resolve_Box_Properties_Used_Value(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue Left, out CssValue MarginLeft, out CssValue Width, out CssValue MarginRight, out CssValue Right, out CssValue Top, out CssValue MarginTop, out CssValue Height, out CssValue MarginBottom, out CssValue Bottom)
        {
            Resolve_Horizontal(Box, Cascaded, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight);
            Resolve_Vertical(Box, Cascaded, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom);

            /*this.Cascaded.Left.Set_Used(outLeft);
            this.Cascaded.Margin_Left.Set_Used(outMarginLeft);
            this.Cascaded.Width.Set_Used(outWidth);
            this.Cascaded.Margin_Right.Set_Used(outMarginRight);
            this.Cascaded.Right.Set_Used(outRight);*/

            /*this.Cascaded.Top.Set_Used(Top);
            this.Cascaded.Margin_Top.Set_Used(MarginTop);
            this.Cascaded.Height.Set_Used(Height);
            this.Cascaded.Margin_Bottom.Set_Used(MarginBottom);
            this.Cascaded.Bottom.Set_Used(Bottom);*/

            /* Horizontal */
            Left = outLeft;
            MarginLeft = outMarginLeft;
            Width = outWidth;
            MarginRight = outMarginRight;
            Right = outRight;

            /* Vertical */
            Top = outTop;
            MarginTop = outMarginTop;
            Height = outHeight;
            MarginBottom = outMarginBottom;
            Bottom = outBottom;
        }

        /// <summary>
        /// Resolves all horizontal sizing properties
        /// </summary>
        private static void Resolve_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight)
        {
            CssValue Left = Cascaded.Left.Computed;
            CssValue MarginLeft = Cascaded.Margin_Left.Computed;
            CssValue Width = Cascaded.Width.Computed;
            CssValue MarginRight = Cascaded.Margin_Right.Computed;
            CssValue Right = Cascaded.Right.Computed;

            Calculate_Horizontal(Box, Cascaded, ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

            /*
             * However, for replaced elements with an intrinsic ratio and both 'width' and 'height' specified as 'auto', the algorithm is as follows:
             * Select from the table the resolved height and width values for the appropriate constraint violation. 
             * Take the max-width and max-height as max(min, max) so that min ≤ max holds true. 
             * In this table w and h stand for the results of the width and height computations ignoring the 'min-width', 'min-height', 'max-width' and 'max-height' properties. 
             * Normally these are the intrinsic width and height, but they may not be in the case of replaced elements with intrinsic ratios.
             */
            bool autoWidth = Cascaded.Width.Computed.IsAuto;
            bool autoHeight = Cascaded.Height.Computed.IsAuto;

            var Min_Width = Cascaded.Min_Width.Actual;
            var Max_Width = Cascaded.Max_Width.Actual;

            if (Box.IsReplacedElement && Box.Intrinsic_Ratio.HasValue && autoWidth && autoHeight)
            {
                CssValue Height = Cascaded.Height.Computed;
                bool changed = Constrain_Width_Height(Cascaded, ref Width, ref Height);
                if (changed) Calculate_Horizontal(Box, Cascaded, ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);
            }
            else
            {
                if (Max_Width.HasValue)
                {
                    //if (Max_Width.Value.CompareTo(Width.AsDecimal()) < 0)// Width > Max_Width
                    if (Width.AsDecimal() > Max_Width.Value)// Width > Max_Width
                    {
                        Width = CssValue.From(Max_Width.Value);
                        Calculate_Horizontal(Box, Cascaded, ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);
                    }
                }

                //if (Min_Width.CompareTo(Width.AsDecimal()) > 0)// Width < Min_Width
                if (Width.AsDecimal() < Min_Width)// Width < Min_Width
                {
                    Width = CssValue.From(Min_Width);
                    Calculate_Horizontal(Box, Cascaded, ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);
                }
            }


            outLeft = Left;
            outMarginLeft = MarginLeft;
            outWidth = Width;
            outMarginRight = MarginRight;
            outRight = Right;
        }

        /// <summary>
        /// Resolves all vertical sizing properties
        /// </summary>
        private static void Resolve_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom)
        {
            CssValue Top = Cascaded.Top.Computed;
            CssValue MarginTop = Cascaded.Margin_Top.Computed;
            CssValue Height = Cascaded.Height.Computed;
            CssValue MarginBottom = Cascaded.Margin_Bottom.Computed;
            CssValue Bottom = Cascaded.Bottom.Computed;
            /*
             * However, for replaced elements with both 'width' and 'height' computed as 'auto', 
             * use the algorithm under 'Minimum and maximum widths' above to find the used width and height. 
             * Then apply the rules under "Computing heights and margins" above, using the resulting width and height as if they were the computed values.
             */
            bool autoWidth = Cascaded.Width.Computed.IsAuto;
            bool autoHeight = Cascaded.Height.Computed.IsAuto;

            var Min_Height = Cascaded.Min_Height.Actual;
            var Max_Height = Cascaded.Max_Height.Actual;

            if (Box.IsReplacedElement && autoWidth && autoHeight)
            {
                CssValue Width = Cascaded.Width.Computed;
                Constrain_Width_Height(Cascaded, ref Width, ref Height);
                Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);
            }
            else
            {
                if (Max_Height.HasValue)
                {
                    //if (Max_Height.Value.CompareTo(Height.AsDecimal()) < 0)// Height.AsDecimal() > Max_Height.Value
                    if (Height.AsDecimal() > Max_Height.Value)// Height.AsDecimal() > Max_Height.Value
                    {
                        Height = CssValue.From(Max_Height.Value);
                        Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);
                    }
                }

                //if (Min_Height.CompareTo(Height.AsDecimal()) > 0)// Height.AsDecimal() < Min_Height
                if (Height.AsDecimal() < Min_Height)// Height.AsDecimal() < Min_Height
                {
                    Height = CssValue.From(Min_Height);
                    Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);
                }
            }

            outTop = Top;
            outMarginTop = MarginTop;
            outHeight = Height;
            outMarginBottom = MarginBottom;
            outBottom = Bottom;
        }

        /// <summary>
        /// Constrains a given Width/Height value according to the CSS specifications for constraining Replaced element sizes
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="outWidth"></param>
        /// <param name="outHeight"></param>
        /// <returns><c>True</c> is the values changed</returns>
        private static bool Constrain_Width_Height(CssComputedStyle Cascaded, ref CssValue Width, ref CssValue Height)
        {/* Docs: https://www.w3.org/TR/CSS22/visudet.html#min-max-widths */
            /*
            * Select from the table the resolved height and width values for the appropriate constraint violation. 
            * Take the max-width and max-height as max(min, max) so that min ≤ max holds true. 
            * In this table w and h stand for the results of the width and height computations ignoring the 'min-width', 'min-height', 'max-width' and 'max-height' properties. 
            * Normally these are the intrinsic width and height, but they may not be in the case of replaced elements with intrinsic ratios.
            */

            var Min_Width = Cascaded.Min_Width.Actual;
            var Max_Width = MathExt.Max(Min_Width, Cascaded.Max_Width.Actual ?? 0);

            var Min_Height = Cascaded.Min_Height.Actual;
            var Max_Height = MathExt.Max(Min_Height, Cascaded.Max_Height.Actual ?? 0);

            var width = Width.AsDecimal();
            var height = Height.AsDecimal();

            var fwidth = (float)width;
            var fheight = (float)height;

            var wMinRatio = (Min_Width / fwidth);
            var wMaxRatio = (Max_Width / fwidth);
            var hMinRatio = (Min_Height / fheight);
            var hMaxRatio = (Max_Height / fheight);

            bool valuesChanged = true;
            if (width > Max_Width) // W > max-width
            {
                Width = CssValue.From(Max_Width);
                Height = CssValue.From(MathExt.Max(Max_Width * (fheight / fwidth), Min_Height));
            }
            else if (width < Min_Width) // W < min-width
            {
                Width = CssValue.From(Min_Width);
                Height = CssValue.From(MathExt.Min(Min_Width * (fheight / fwidth), Max_Height));
            }
            else if (height > Max_Height) // H > max-height
            {
                Width = CssValue.From(MathExt.Max(Max_Height * fwidth / fheight, Min_Width));
                Height = CssValue.From(Max_Height);
            }
            else if (height < Min_Height)// H < min-height
            {
                Width = CssValue.From(MathExt.Min(Min_Height * fwidth / fheight, Max_Width));
                Height = CssValue.From(Min_Height);
            }
            else if (width > Max_Width && height > Max_Height && wMaxRatio <= hMaxRatio)
            {
                Width = CssValue.From(Max_Width);
                Height = CssValue.From(MathExt.Max(Min_Height, Max_Width * (fheight / fwidth)));
            }
            else if (width > Max_Width && height > Max_Height && wMaxRatio > hMaxRatio)
            {
                Width = CssValue.From(MathExt.Max(Min_Width, Max_Height * fwidth / fheight));
                Height = CssValue.From(Max_Height);
            }
            else if (width < Min_Width && height < Min_Height && wMinRatio <= hMinRatio)
            {
                Width = CssValue.From(MathExt.Min(Max_Width, Min_Height * fwidth / fheight));
                Height = CssValue.From(Min_Height);
            }
            else if (width < Min_Width && height < Min_Height && wMinRatio > hMinRatio)
            {
                Width = CssValue.From(Min_Width);
                Height = CssValue.From(MathExt.Min(Max_Height, Min_Width * (fheight / fwidth)));
            }
            else if (width < Min_Width && height > Max_Height)
            {
                Width = CssValue.From(Min_Width);
                Height = CssValue.From(Max_Height);
            }
            else if (width > Max_Width && height < Min_Height)
            {
                Width = CssValue.From(Max_Width);
                Height = CssValue.From(Max_Height);
            }
            else
            {/* Width / Height do not change */
                valuesChanged = false;
            }

            return valuesChanged;
        }


        #region Calculate Horizontal
        /// <summary>
        /// Calculates all horizontal property values using the ones given
        /// </summary>
        private static void Calculate_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, ref CssValue Left, ref CssValue MarginLeft, ref CssValue Width, ref CssValue MarginRight, ref CssValue Right)
        {
            Calculate_Horizontal(Box, Cascaded, Left, MarginLeft, Width, MarginRight, Right, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight);

            Left        = outLeft;
            MarginLeft  = outMarginLeft;
            Width       = outWidth;
            MarginRight = outMarginRight;
            Right       = outRight;
        }

        /// <summary>
        /// Calculates all horizontal property values using the ones given
        /// </summary>
        private static void Calculate_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, CssValue Left, CssValue MarginLeft, CssValue Width, CssValue MarginRight, CssValue Right, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight)
        {// Docs: https://www.w3.org/TR/CSS22/visudet.html#Computing_widths_and_margins
         /*
          * The values of an element's 'width', 'margin-left', 'margin-right', 'left' and 'right' properties as used for layout depend on the type of box generated and on each other. (The value used for layout is sometimes referred to as the used value.) In principle, the values used are the same as the computed values, with 'auto' replaced by some suitable value, and percentages calculated based on the containing block, but there are exceptions. The following situations need to be distinguished:
          * 
          * inline, non-replaced elements
          * inline, replaced elements
          * block-level, non-replaced elements in normal flow
          * block-level, replaced elements in normal flow
          * floating, non-replaced elements
          * floating, replaced elements
          * absolutely positioned, non-replaced elements
          * absolutely positioned, replaced elements
          * 'inline-block', non-replaced elements in normal flow
          * 'inline-block', replaced elements in normal flow
          */

            /* Setup some commonly used variables*/
            bool autoHeight = Cascaded.Height.Computed.IsAuto;

            EDirection Direction = Cascaded.Direction.Actual;
            EWritingMode WritingMode = Cascaded.WritingMode.Actual;

            CssValue Height = Cascaded.Height.Computed;

            var PaddingLeft = (double)Cascaded.Padding_Left.Actual;
            var PaddingRight = (double)Cascaded.Padding_Right.Actual;

            var BorderLeft = (double)Cascaded.Border_Left_Width.Actual;
            var BorderRight = (double)Cascaded.Border_Right_Width.Actual;

            var marginLeft = (MarginLeft.IsAuto ? 0 : MarginLeft.AsDecimal());
            var marginRight = (MarginRight.IsAuto ? 0 : MarginRight.AsDecimal());


            switch (Box.DisplayGroup)
            {
                case EBoxDisplayGroup.INLINE:
                    {
                        if (!Box.IsReplacedElement)
                        {// The 'width' property does not apply. A computed value of 'auto' for 'margin-left' or 'margin-right' becomes a used value of '0'.
                            MarginLeft = (MarginLeft.IsAuto ? CssValue.Zero : MarginLeft);
                            MarginRight = (MarginRight.IsAuto ? CssValue.Zero : MarginRight);
                        }

                        if (Box.IsReplacedElement)
                        {
                            // A computed value of 'auto' for 'margin-left' or 'margin-right' becomes a used value of '0'.
                            MarginLeft = (MarginLeft.IsAuto ? CssValue.Zero : MarginLeft);
                            MarginRight = (MarginRight.IsAuto ? CssValue.Zero : MarginRight);

                            if (Width.IsAuto && autoHeight)
                            {
                                // If 'height' and 'width' both have computed values of 'auto' and the element also has an intrinsic width, then that intrinsic width is the used value of 'width'.
                                if (Box.Intrinsic_Width.HasValue)
                                {
                                    Width = CssValue.From(Box.Intrinsic_Width.Value);
                                }
                                else if (Box.Intrinsic_Height.HasValue && Box.Intrinsic_Ratio.HasValue) /* If 'height' and 'width' both have computed values of 'auto' and the element has no intrinsic width, but does have an intrinsic height and intrinsic ratio;  then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                {
                                    Width = CssValue.From((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value));
                                }
                            }

                            if (Width.IsAuto && !autoHeight)
                            {/* or if 'width' has a computed value of 'auto', 'height' has some other computed value, and the element does have an intrinsic ratio; then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                Width = CssValue.From((Height.AsDecimal() * Box.Intrinsic_Ratio.Value));
                            }

                            if (Width.IsAuto && autoHeight && Box.Intrinsic_Ratio.HasValue && !Box.Intrinsic_Height.HasValue && !Box.Intrinsic_Width.HasValue)
                            {/* If 'height' and 'width' both have computed values of 'auto' and the element has an intrinsic ratio but no intrinsic height or width, then the used value of 'width' is undefined in CSS 2.1. However, it is suggested that, if the containing block's width does not itself depend on the replaced element's width, then the used value of 'width' is calculated from the constraint equation used for block-level, non-replaced elements in normal flow. */
                                if (!Box.Containing_Box_Dependent)
                                {/* 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block */
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From( CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            else if(Width.IsAuto && Box.Intrinsic_Width.HasValue) /* Otherwise, if 'width' has a computed value of 'auto', and the element has an intrinsic width, then that intrinsic width is the used value of 'width'. */
                            {
                                Width = CssValue.From(Box.Intrinsic_Width.Value);
                            }
                            else /* Otherwise, if 'width' has a computed value of 'auto', but none of the conditions above are met, then the used value of 'width' becomes 300px. If 300px is too wide to fit the device, UAs should use the width of the largest rectangle that has a 2:1 ratio and fits the device instead. */
                            {
                                Width = CssValue.From(300, ECssUnit.PX);
                            }
                        }
                    }
                    break;
                case EBoxDisplayGroup.BLOCK:
                    {
                        /* 10.3.3 Block-level, non-replaced elements in normal flow */
                        if (!Box.IsReplacedElement)
                        {/* 10.3.3 Block-level, non-replaced elements in normal flow */
                            /* If 'width' is not 'auto' and 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' (plus any of 'margin-left' or 'margin-right' that are not 'auto') is larger than the width of the containing block, then any 'auto' values for 'margin-left' or 'margin-right' are, for the following rules, treated as zero. */
                            bool Exceeds = (!Width.IsAuto && CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) < (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight));
                            // I suppose the specs want us to ignore this auto section and just set the margins to 0?
                            if (Exceeds)
                            {
                                if (MarginLeft.IsAuto)
                                    MarginLeft = CssValue.Zero;
                                if (MarginRight.IsAuto)
                                    MarginRight = CssValue.Zero;
                            }

                            bool singleAuto = (MarginLeft.IsAuto ^ Width.IsAuto ^ MarginRight.IsAuto);
                            bool OverConstrained = !(MarginLeft.IsAuto && Width.IsAuto && MarginRight.IsAuto);

                            /* If all of the above have a computed value other than 'auto', the values are said to be "over-constrained" and one of the used values will have to be different from its computed value. If the 'direction' property of the containing block has the value 'ltr', the specified value of 'margin-right' is ignored and the value is calculated so as to make the equality true. If the value of 'direction' is 'rtl', this happens to 'margin-left' instead. */
                            if (OverConstrained)
                            {
                                switch(Direction)
                                {
                                    case EDirection.LTR:
                                        {
                                            var eqRes = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                            MarginRight = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        }
                                        break;
                                    case EDirection.RTL:
                                        {
                                            var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                                            MarginLeft = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        }
                                        break;
                                }
                            }
                            else if (singleAuto)
                            {/* If there is exactly one value specified as 'auto', its used value follows from the equality. */
                                if (Width.IsAuto)
                                {
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (MarginLeft.IsAuto)
                                {
                                    var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                                    MarginLeft = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (MarginRight.IsAuto)
                                {
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                    MarginRight = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }

                            if (Width.IsAuto)
                            {// If 'width' is set to 'auto', any other 'auto' values become '0' and 'width' follows from the resulting equality.
                                double total = 0;
                                total += (MarginLeft.IsAuto ? 0 : MarginLeft.AsDecimal());
                                total += BorderLeft;
                                total += PaddingLeft;

                                total += PaddingRight;
                                total += BorderRight;
                                total += (MarginRight.IsAuto ? 0 : MarginRight.AsDecimal());

                                Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - total);
                            }

                            if (MarginLeft.IsAuto && MarginRight.IsAuto)
                            {/* If both 'margin-left' and 'margin-right' are 'auto', their used values are equal. This horizontally centers the element with respect to the edges of the containing block. */
                                var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                MarginLeft = CssValue.From(avail / 2);
                                MarginRight = CssValue.From(avail / 2);
                            }
                        }
                        /* 10.3.4 Block-level, replaced elements in normal flow */
                        if (Box.IsReplacedElement)
                        {/* The used value of 'width' is determined as for inline replaced elements. Then the rules for non-replaced block-level elements are applied to determine the margins.*/

                            #region Width - inline replaced
                            if (Width.IsAuto && autoHeight)
                            {
                                // If 'height' and 'width' both have computed values of 'auto' and the element also has an intrinsic width, then that intrinsic width is the used value of 'width'.
                                if (Box.Intrinsic_Width.HasValue)
                                {
                                    Width = CssValue.From(Box.Intrinsic_Width.Value);
                                }
                                else if (Box.Intrinsic_Height.HasValue && Box.Intrinsic_Ratio.HasValue) /* If 'height' and 'width' both have computed values of 'auto' and the element has no intrinsic width, but does have an intrinsic height and intrinsic ratio;  then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                {
                                    Width = CssValue.From((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value));
                                }
                            }

                            if (Width.IsAuto && !autoHeight)
                            {/* or if 'width' has a computed value of 'auto', 'height' has some other computed value, and the element does have an intrinsic ratio; then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                Width = CssValue.From((Height.AsDecimal() * Box.Intrinsic_Ratio.Value));
                            }

                            if (Width.IsAuto && autoHeight && Box.Intrinsic_Ratio.HasValue && !Box.Intrinsic_Height.HasValue && !Box.Intrinsic_Width.HasValue)
                            {/* If 'height' and 'width' both have computed values of 'auto' and the element has an intrinsic ratio but no intrinsic height or width, then the used value of 'width' is undefined in CSS 2.1. However, it is suggested that, if the containing block's width does not itself depend on the replaced element's width, then the used value of 'width' is calculated from the constraint equation used for block-level, non-replaced elements in normal flow. */
                                if (!Box.Containing_Box_Dependent)
                                {/* 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block */
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            else if (Width.IsAuto && Box.Intrinsic_Width.HasValue) /* Otherwise, if 'width' has a computed value of 'auto', and the element has an intrinsic width, then that intrinsic width is the used value of 'width'. */
                            {
                                Width = CssValue.From(Box.Intrinsic_Width.Value);
                            }
                            else /* Otherwise, if 'width' has a computed value of 'auto', but none of the conditions above are met, then the used value of 'width' becomes 300px. If 300px is too wide to fit the device, UAs should use the width of the largest rectangle that has a 2:1 ratio and fits the device instead. */
                            {
                                Width = CssValue.From(300, ECssUnit.PX);
                            }
                            #endregion

                            /* If 'width' is not 'auto' and 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' (plus any of 'margin-left' or 'margin-right' that are not 'auto') is larger than the width of the containing block, then any 'auto' values for 'margin-left' or 'margin-right' are, for the following rules, treated as zero. */
                            bool Exceeds = (!Width.IsAuto && CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) < (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight));
                            // I suppose the specs want us to ignore this auto section and just set the margins to 0?
                            if (Exceeds)
                            {
                                if (MarginLeft.IsAuto)
                                    MarginLeft = CssValue.Zero;
                                if (MarginRight.IsAuto)
                                    MarginRight = CssValue.Zero;
                            }
                            
                            bool singleAuto = (MarginLeft.IsAuto ^ Width.IsAuto ^ MarginRight.IsAuto);
                            bool OverConstrained = !(MarginLeft.IsAuto && Width.IsAuto && MarginRight.IsAuto);

                            /* If all of the above have a computed value other than 'auto', the values are said to be "over-constrained" and one of the used values will have to be different from its computed value. If the 'direction' property of the containing block has the value 'ltr', the specified value of 'margin-right' is ignored and the value is calculated so as to make the equality true. If the value of 'direction' is 'rtl', this happens to 'margin-left' instead. */
                            if (OverConstrained)
                            {
                                switch (Direction)
                                {
                                    case EDirection.LTR:
                                        {
                                            var eqRes = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                            MarginRight = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        }
                                        break;
                                    case EDirection.RTL:
                                        {
                                            var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                                            MarginLeft = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        }
                                        break;
                                }
                            }
                            else if (singleAuto)
                            {
                                if (MarginLeft.IsAuto)
                                {
                                    var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                                    MarginLeft = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (MarginRight.IsAuto)
                                {
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                    MarginRight = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }

                            if (MarginLeft.IsAuto && MarginRight.IsAuto)
                            {/* If both 'margin-left' and 'margin-right' are 'auto', their used values are equal. This horizontally centers the element with respect to the edges of the containing block. */
                                var eqRes = (BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight);
                                var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                MarginLeft = CssValue.From(avail / 2);
                                MarginRight = CssValue.From(avail / 2);
                            }
                        }
                    }
                    break;
                case EBoxDisplayGroup.FLOATING:
                    {
                        /* 10.3.5 Floating, non-replaced elements */
                        if (Box.IsReplacedElement)
                        {
                            /* If 'margin-left', or 'margin-right' are computed as 'auto', their used value is '0'. */
                            if (MarginLeft.IsAuto) MarginLeft = CssValue.Zero;
                            if (MarginRight.IsAuto) MarginRight = CssValue.Zero;

                            /* 
                             * If 'width' is computed as 'auto', the used value is the "shrink-to-fit" width.
                             * Calculation of the shrink-to-fit width is similar to calculating the width of a table cell using the automatic table layout algorithm. Roughly: calculate the preferred width by formatting the content without breaking lines other than where explicit line breaks occur, and also calculate the preferred minimum width, e.g., by trying all possible line breaks. CSS 2.1 does not define the exact algorithm. Thirdly, find the available width: in this case, this is the width of the containing block minus the used values of 'margin-left', 'border-left-width', 'padding-left', 'padding-right', 'border-right-width', 'margin-right', and the widths of any relevant scroll bars. 
                             */
                             if (Width.IsAuto)
                            {
                                var sbWidth = CssCommon.SnapToPixel(Box.Owner?.ScrollBox?.VScrollBar?.Width ?? 0);
                                var total = (marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight + sbWidth);
                                var available_width = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - total);

                                Width = CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, available_width));
                            }
                        }

                        /* 10.3.6 Floating, replaced elements */
                        if (!Box.IsReplacedElement)
                        {
                            /* If 'margin-left', or 'margin-right' are computed as 'auto', their used value is '0'. */
                            if (MarginLeft.IsAuto) MarginLeft = CssValue.Zero;
                            if (MarginRight.IsAuto) MarginRight = CssValue.Zero;

                            /* The used value of 'width' is determined as for inline replaced elements. */
                            if (Width.IsAuto && autoHeight)
                            {
                                // If 'height' and 'width' both have computed values of 'auto' and the element also has an intrinsic width, then that intrinsic width is the used value of 'width'.
                                if (Box.Intrinsic_Width.HasValue)
                                {
                                    Width = CssValue.From(Box.Intrinsic_Width.Value);
                                }
                                else if (Box.Intrinsic_Height.HasValue && Box.Intrinsic_Ratio.HasValue) /* If 'height' and 'width' both have computed values of 'auto' and the element has no intrinsic width, but does have an intrinsic height and intrinsic ratio;  then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                {
                                    Width = CssValue.From((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value));
                                }
                            }

                            if (Width.IsAuto && !autoHeight)
                            {/* or if 'width' has a computed value of 'auto', 'height' has some other computed value, and the element does have an intrinsic ratio; then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                Width = CssValue.From((Height.AsDecimal() * Box.Intrinsic_Ratio.Value));
                            }

                            if (Width.IsAuto && autoHeight && Box.Intrinsic_Ratio.HasValue && !Box.Intrinsic_Height.HasValue && !Box.Intrinsic_Width.HasValue)
                            {/* If 'height' and 'width' both have computed values of 'auto' and the element has an intrinsic ratio but no intrinsic height or width, then the used value of 'width' is undefined in CSS 2.1. However, it is suggested that, if the containing block's width does not itself depend on the replaced element's width, then the used value of 'width' is calculated from the constraint equation used for block-level, non-replaced elements in normal flow. */
                                if (!Box.Containing_Box_Dependent)
                                {/* 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block */
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            else if (Width.IsAuto && Box.Intrinsic_Width.HasValue) /* Otherwise, if 'width' has a computed value of 'auto', and the element has an intrinsic width, then that intrinsic width is the used value of 'width'. */
                            {
                                Width = CssValue.From(Box.Intrinsic_Width.Value);
                            }
                            else /* Otherwise, if 'width' has a computed value of 'auto', but none of the conditions above are met, then the used value of 'width' becomes 300px. If 300px is too wide to fit the device, UAs should use the width of the largest rectangle that has a 2:1 ratio and fits the device instead. */
                            {
                                Width = CssValue.From(300, ECssUnit.PX);
                            }
                        }
                    }
                    break;
                case EBoxDisplayGroup.ABSOLUTELY_POSITIONED:
                    {
                        /* 10.3.7 Absolutely positioned, non-replaced elements */
                        if (!Box.IsReplacedElement)
                        {
                            /* 
                             * The constraint that determines the used values for these elements is:
                             * 'left' + 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' + 'right' = width of containing block 
                             */

                            if (Left.IsAuto && Width.IsAuto && Right.IsAuto)
                            {/* If all three of 'left', 'width', and 'right' are 'auto': First set any 'auto' values for 'margin-left' and 'margin-right' to 0 */
                                if (MarginLeft.IsAuto) MarginLeft = CssValue.Zero;
                                if (MarginRight.IsAuto) MarginRight = CssValue.Zero;

                                if (Direction == EDirection.LTR)
                                {
                                    Left = CssValue.From(Box.Layout_Pos_X);
                                    /* Apply Rule #3 */
                                    /* the width is shrink-to-fit . Then solve for 'right' */
                                    var eqRes = (Left.AsDecimal() + marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight + 0);
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    Width =  CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, avail));

                                    /* Solve for 'right' */
                                    eqRes = (Left.AsDecimal() + marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight + 0);
                                    Right = CssValue.From(MathExt.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes));
                                }
                                else if(Direction == EDirection.RTL)
                                {
                                    Right = CssValue.From(Box.Layout_Pos_X);
                                    /* Apply Rule #1 */
                                    /* the width is shrink-to-fit . Then solve for 'left' */
                                    var eqRes = (0 + marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight + Right.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    Width = CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, avail));

                                    /* Solve for 'left' */
                                    eqRes = (0 + marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight + Right.AsDecimal());
                                    Left = CssValue.From(MathExt.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes));

                                }
                            }
                            else if (!Left.IsAuto && !Width.IsAuto && !Right.IsAuto)
                            {
                                if (MarginLeft.IsAuto && MarginRight.IsAuto)
                                {/* If none of the three is 'auto': If both 'margin-left' and 'margin-right' are 'auto', solve the equation under the extra constraint that the two margins get equal values, unless this would make them negative, in which case when direction of the containing block is 'ltr' ('rtl'), set 'margin-left' ('margin-right') to zero and solve for 'margin-right' ('margin-left'). If one of 'margin-left' or 'margin-right' is 'auto', solve the equation for that value. If the values are over-constrained, ignore the value for 'left' (in case the 'direction' property of the containing block is 'rtl') or 'right' (in case 'direction' is 'ltr') and solve for that value. */
                                    var eqRes = (Left.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + Right.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    if (avail >= 0)
                                    {
                                        MarginLeft = CssValue.From(avail / 2);
                                        MarginRight = CssValue.From(avail / 2);
                                    }
                                    else// Negative
                                    {
                                        if (Direction == EDirection.LTR)
                                        {
                                            MarginLeft = CssValue.Zero;
                                            MarginRight = CssValue.From(avail);
                                        }
                                        else if (Direction == EDirection.RTL)
                                        {
                                            MarginLeft = CssValue.From(avail);
                                            MarginRight = CssValue.Zero;
                                        }
                                    }
                                }
                                else if (MarginLeft.IsAuto ^ MarginRight.IsAuto) /* If one of 'margin-left' or 'margin-right' is 'auto', solve the equation for that value. */
                                {
                                    if (MarginLeft.IsAuto)
                                    {
                                        var eqRes = (Left.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        MarginLeft = CssValue.From(avail);
                                    }
                                    else if (MarginRight.IsAuto)
                                    {
                                        var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + Right.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        MarginRight = CssValue.From(avail);
                                    }
                                }
                                else if (!MarginLeft.IsAuto && !MarginRight.IsAuto) /* If the values are over-constrained, ignore the value for 'left' (in case the 'direction' property of the containing block is 'rtl') or 'right' (in case 'direction' is 'ltr') and solve for that value. */
                                {
                                    if (Direction == EDirection.LTR)
                                    {
                                        var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        Right = CssValue.From(avail);
                                    }
                                    else if (Direction == EDirection.RTL)
                                    {
                                        var eqRes = (MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        Left = CssValue.From(avail);
                                    }
                                }
                            }
                            else
                            {/* Otherwise, set 'auto' values for 'margin-left' and 'margin-right' to 0, and pick the one of the following six rules that applies. */
                                if (MarginLeft.IsAuto) MarginLeft = CssValue.Zero;
                                if (MarginRight.IsAuto) MarginRight = CssValue.Zero;

                                if (Left.IsAuto && Width.IsAuto && !Right.IsAuto)
                                {
                                    /* Apply Rule #1 */
                                    /* the width is shrink-to-fit . Then solve for 'left' */
                                    var eqRes = (0 + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    Width = CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, avail));

                                    /* Solve for 'left' */
                                    eqRes = (0 + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                    Left = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (Left.IsAuto && !Width.IsAuto && Right.IsAuto)
                                {
                                    /* Apply Rule #2 */
                                    /* if the 'direction' property of the element establishing the static-position containing block is 'ltr' set 'left' to the static position, otherwise set 'right' to the static position. Then solve for 'left' (if 'direction is 'rtl') or 'right' (if 'direction' is 'ltr'). */
                                    if (Direction == EDirection.LTR)
                                    {
                                        Left = CssValue.From(Box.Layout_Pos_X);

                                        var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + 0);
                                        var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                        Right = CssValue.From(avail);

                                    }
                                    else if (Direction == EDirection.RTL)
                                    {
                                        Right = CssValue.From(Box.Layout_Pos_X);

                                        var eqRes = (0 + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                        Left = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    }
                                }
                                else if (!Left.IsAuto && Width.IsAuto && Right.IsAuto)
                                {/* Apply Rule #3 */
                                    /* Width is shrink-to-fit */
                                    var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + MarginRight.AsDecimal() + 0);
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                    Width = CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, avail));
                                    /* Solve for 'right' */
                                    eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + 0);
                                    Right = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if(Left.IsAuto && !Width.IsAuto && !Right.IsAuto)
                                {/* Apply Rule #4 */
                                    /* Solve for 'left' */
                                    var eqRes = (0 + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                    Left = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (!Left.IsAuto && Width.IsAuto && !Right.IsAuto)
                                {/* Apply Rule #5 */
                                    /* Solve for 'width' */
                                    var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + MarginRight.AsDecimal() + Right.AsDecimal());
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (!Left.IsAuto && !Width.IsAuto && Right.IsAuto)
                                {/* Apply Rule #6 */
                                    /* Solve for 'right' */
                                    var eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + 0);
                                    Right = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                        }

                        /* 10.3.8 Absolutely positioned, replaced elements */
                        if (Box.IsReplacedElement)
                        {
                            /* The used value of 'width' is determined as for inline replaced elements */
                            if (Width.IsAuto && autoHeight)
                            {
                                // If 'height' and 'width' both have computed values of 'auto' and the element also has an intrinsic width, then that intrinsic width is the used value of 'width'.
                                if (Box.Intrinsic_Width.HasValue)
                                {
                                    Width = CssValue.From(Box.Intrinsic_Width.Value);
                                }
                                else if (Box.Intrinsic_Height.HasValue && Box.Intrinsic_Ratio.HasValue) /* If 'height' and 'width' both have computed values of 'auto' and the element has no intrinsic width, but does have an intrinsic height and intrinsic ratio;  then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                {
                                    Width = CssValue.From((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value));
                                }
                            }

                            if (Width.IsAuto && !autoHeight)
                            {/* or if 'width' has a computed value of 'auto', 'height' has some other computed value, and the element does have an intrinsic ratio; then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                Width = CssValue.From((Height.AsDecimal() * Box.Intrinsic_Ratio.Value));
                            }

                            if (Width.IsAuto && autoHeight && Box.Intrinsic_Ratio.HasValue && !Box.Intrinsic_Height.HasValue && !Box.Intrinsic_Width.HasValue)
                            {/* If 'height' and 'width' both have computed values of 'auto' and the element has an intrinsic ratio but no intrinsic height or width, then the used value of 'width' is undefined in CSS 2.1. However, it is suggested that, if the containing block's width does not itself depend on the replaced element's width, then the used value of 'width' is calculated from the constraint equation used for block-level, non-replaced elements in normal flow. */
                                if (!Box.Containing_Box_Dependent)
                                {/* 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block */
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            else if (Width.IsAuto && Box.Intrinsic_Width.HasValue) /* Otherwise, if 'width' has a computed value of 'auto', and the element has an intrinsic width, then that intrinsic width is the used value of 'width'. */
                            {
                                Width = CssValue.From(Box.Intrinsic_Width.Value);
                            }
                            else /* Otherwise, if 'width' has a computed value of 'auto', but none of the conditions above are met, then the used value of 'width' becomes 300px. If 300px is too wide to fit the device, UAs should use the width of the largest rectangle that has a 2:1 ratio and fits the device instead. */
                            {
                                Width = CssValue.From(300, ECssUnit.PX);
                            }

                            if (Left.IsAuto && Right.IsAuto)
                            {
                                if (Direction == EDirection.LTR)
                                    Left = CssValue.From(Box.Layout_Pos_X);
                                else if (Direction == EDirection.RTL)
                                    Right = CssValue.From(Box.Layout_Pos_X);
                            }

                            if (Left.IsAuto ^ Right.IsAuto)
                            {
                                if (MarginLeft.IsAuto)
                                    MarginLeft = CssValue.Zero;
                                if (MarginRight.IsAuto)
                                    MarginRight = CssValue.Zero;
                            }

                            if (MarginLeft.IsAuto && MarginRight.IsAuto)
                            {
                                var eqRes = 0d;
                                eqRes += (Left.IsAuto ? 0 : Left.AsDecimal());
                                eqRes += (BorderLeft + PaddingLeft);
                                eqRes += (Width.IsAuto ? 0 : Width.AsDecimal());
                                eqRes += (BorderRight + PaddingRight);
                                eqRes += (Right.IsAuto ? 0 : Right.AsDecimal());
                                var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);

                                if (avail >= 0)
                                {
                                    var mval = avail / 2;
                                    MarginLeft = CssValue.From(mval);
                                    MarginRight = CssValue.From(mval);
                                }
                                else
                                {
                                    if (Direction == EDirection.LTR)
                                    {
                                        MarginLeft = CssValue.Zero;
                                        MarginRight = CssValue.From(avail);
                                    }
                                    else if (Direction == EDirection.RTL)
                                    {
                                        MarginLeft = CssValue.From(avail);
                                        MarginRight = CssValue.Zero;
                                    }
                                }
                            }
                            /* If at this point there is an 'auto' left, solve the equation for that value. */
                            if (MarginLeft.IsAuto || MarginRight.IsAuto)
                            {
                                if (MarginLeft.IsAuto)
                                {
                                    var eqRes = 0.0;
                                    eqRes += (Left.IsAuto ? 0 : Left.AsDecimal());
                                    eqRes += (MarginLeft.IsAuto ? 0 : MarginLeft.AsDecimal());
                                    eqRes += (BorderLeft + PaddingLeft);
                                    eqRes += (Width.IsAuto ? 0 : Width.AsDecimal());
                                    eqRes += (BorderRight + PaddingRight);
                                    eqRes += (MarginRight.IsAuto ? 0 : MarginRight.AsDecimal());
                                    eqRes += (Right.IsAuto ? 0 : Right.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);

                                    MarginLeft = CssValue.From(avail);
                                }
                                if (MarginRight.IsAuto)
                                {
                                    var eqRes = 0.0;
                                    eqRes += (Left.IsAuto ? 0 : Left.AsDecimal());
                                    eqRes += (MarginLeft.IsAuto ? 0 : MarginLeft.AsDecimal());
                                    eqRes += (BorderLeft + PaddingLeft);
                                    eqRes += (Width.IsAuto ? 0 : Width.AsDecimal());
                                    eqRes += (BorderRight + PaddingRight);
                                    eqRes += (MarginRight.IsAuto ? 0 : MarginRight.AsDecimal());
                                    eqRes += (Right.IsAuto ? 0 : Right.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);

                                    MarginRight = CssValue.From(avail);
                                }
                            }

                            bool OverConstrained = !(Left.IsAuto && MarginLeft.IsAuto && Width.IsAuto && MarginRight.IsAuto && Right.IsAuto);
                            if (OverConstrained)
                            {
                                if (Direction == EDirection.LTR)
                                {
                                    var eqRes = (MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + 0);
                                    Right = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (Direction == EDirection.RTL)
                                {
                                    var eqRes = (0 + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal());
                                    Left = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }

                        }
                    }
                    break;
                case EBoxDisplayGroup.INLINE_BLOCK:
                    {
                        /* 10.3.9 'Inline-block', non-replaced elements in normal flow */
                        if (!Box.IsReplacedElement)
                        {
                            if (Width.IsAuto)
                            {/* the width is shrink-to-fit (as defined for FLOATING elements). Then solve for 'left' */
                                var sbWidth = CssCommon.SnapToPixel(Box.Owner?.ScrollBox?.VScrollBar?.Width ?? 0);
                                var eqRes = (marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight + sbWidth);
                                var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                Width = CssValue.From(MathExt.Clamp(Box.Preferred_Width, Box.Min_Content.Width, avail));
                            }

                            if (MarginLeft.IsAuto) MarginLeft = CssValue.Zero;
                            if (MarginRight.IsAuto) MarginRight = CssValue.Zero;
                        }

                        /* 10.3.10 'Inline-block', replaced elements in normal flow */
                        if (Box.IsReplacedElement)
                        {/* Exactly as inline replaced elements. */
                            // A computed value of 'auto' for 'margin-left' or 'margin-right' becomes a used value of '0'.
                            MarginLeft = (MarginLeft.IsAuto ? CssValue.Zero : MarginLeft);
                            MarginRight = (MarginRight.IsAuto ? CssValue.Zero : MarginRight);

                            if (Width.IsAuto && autoHeight)
                            {
                                // If 'height' and 'width' both have computed values of 'auto' and the element also has an intrinsic width, then that intrinsic width is the used value of 'width'.
                                if (Box.Intrinsic_Width.HasValue)
                                {
                                    Width = CssValue.From(Box.Intrinsic_Width.Value);
                                }
                                else if (Box.Intrinsic_Height.HasValue && Box.Intrinsic_Ratio.HasValue) /* If 'height' and 'width' both have computed values of 'auto' and the element has no intrinsic width, but does have an intrinsic height and intrinsic ratio;  then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                {
                                    Width = CssValue.From((Box.Intrinsic_Height.Value * Box.Intrinsic_Ratio.Value));
                                }
                            }

                            if (Width.IsAuto && !autoHeight)
                            {/* or if 'width' has a computed value of 'auto', 'height' has some other computed value, and the element does have an intrinsic ratio; then the used value of 'width' is: (used height) * (intrinsic ratio) */
                                Width = CssValue.From((Height.AsDecimal() * Box.Intrinsic_Ratio.Value));
                            }

                            if (Width.IsAuto && autoHeight && Box.Intrinsic_Ratio.HasValue && !Box.Intrinsic_Height.HasValue && !Box.Intrinsic_Width.HasValue)
                            {/* If 'height' and 'width' both have computed values of 'auto' and the element has an intrinsic ratio but no intrinsic height or width, then the used value of 'width' is undefined in CSS 2.1. However, it is suggested that, if the containing block's width does not itself depend on the replaced element's width, then the used value of 'width' is calculated from the constraint equation used for block-level, non-replaced elements in normal flow. */
                                if (!Box.Containing_Box_Dependent)
                                {/* 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block */
                                    var eqRes = (marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight);
                                    Width = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            else if (Width.IsAuto && Box.Intrinsic_Width.HasValue) /* Otherwise, if 'width' has a computed value of 'auto', and the element has an intrinsic width, then that intrinsic width is the used value of 'width'. */
                            {
                                Width = CssValue.From(Box.Intrinsic_Width.Value);
                            }
                            else /* Otherwise, if 'width' has a computed value of 'auto', but none of the conditions above are met, then the used value of 'width' becomes 300px. If 300px is too wide to fit the device, UAs should use the width of the largest rectangle that has a 2:1 ratio and fits the device instead. */
                            {
                                Width = CssValue.From(300, ECssUnit.PX);
                            }
                        }
                    }
                    break;
            }

            outLeft = Left;
            outMarginLeft = MarginLeft;
            outWidth = Width;
            outMarginRight = MarginRight;
            outRight = Right;
        }

        #endregion

        #region Calculate Vertical
        /// <summary>
        /// Calculates all horizontal property values using the ones given
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Calculate_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, ref CssValue Top, ref CssValue MarginTop, ref CssValue Height, ref CssValue MarginBottom, ref CssValue Bottom, CssValue WidthOverride = null)
        {
            Calculate_Vertical(Box, Cascaded, Top, MarginTop, Height, MarginBottom, Bottom, WidthOverride, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom);

            Top = outTop;
            MarginTop = outMarginTop;
            Height = outHeight;
            MarginBottom = outMarginBottom;
            Bottom = outBottom;
        }

        /// <summary>
        /// Calculates all horizontal property values using the ones given
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Calculate_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, CssValue Top, CssValue MarginTop, CssValue Height, CssValue MarginBottom, CssValue Bottom, CssValue Width, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom)
        {// Docs: https://www.w3.org/TR/CSS22/visudet.html#Computing_heights_and_margins
         /*
          * The values of an element's 'width', 'margin-left', 'margin-right', 'left' and 'right' properties as used for layout depend on the type of box generated and on each other. (The value used for layout is sometimes referred to as the used value.) In principle, the values used are the same as the computed values, with 'auto' replaced by some suitable value, and percentages calculated based on the containing block, but there are exceptions. The following situations need to be distinguished:
          * 
          * inline, non-replaced elements
          * inline, replaced elements
          * block-level, non-replaced elements in normal flow
          * block-level, replaced elements in normal flow
          * floating, non-replaced elements
          * floating, replaced elements
          * absolutely positioned, non-replaced elements
          * absolutely positioned, replaced elements
          * 'inline-block', non-replaced elements in normal flow
          * 'inline-block', replaced elements in normal flow
          */

            /* Setup some commonly used variables*/
            bool autoWidth;
            if (Width is null)
            {
                Width = Cascaded.Width.Used;
                autoWidth = Cascaded.Width.Computed.IsAuto;
            }
            else
            {
                autoWidth = Width.IsAuto;
            }

            EWritingMode WritingMode = Cascaded.WritingMode.Actual;
            var PaddingTop = Cascaded.Padding_Top.Actual;
            var PaddingBottom = Cascaded.Padding_Bottom.Actual;

            var BorderTop = Cascaded.Border_Top_Width.Actual;
            var BorderBottom = Cascaded.Border_Bottom_Width.Actual;

            //var marginTop = (MarginTop.IsAuto ? 0 : MarginTop.AsDecimal());
            //var marginBottom = (MarginBottom.IsAuto ? 0 : MarginBottom.AsDecimal());

            if (Box.IsReplacedElement)
            {
                switch(Box.DisplayGroup)
                {
                    case EBoxDisplayGroup.INLINE:
                    case EBoxDisplayGroup.BLOCK:
                    case EBoxDisplayGroup.INLINE_BLOCK:
                    case EBoxDisplayGroup.FLOATING:
                        {
                            if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                            if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                            if (Height.IsAuto && autoWidth && Box.Intrinsic_Height.HasValue)
                            {
                                Height = CssValue.From(Box.Intrinsic_Height.Value);
                            }
                            else if (Height.IsAuto && Box.Intrinsic_Ratio.HasValue)
                            {
                                Height = CssValue.From((Width.AsDecimal() / Box.Intrinsic_Ratio.Value));
                            }
                            else if (Height.IsAuto && Box.Intrinsic_Height.HasValue)
                            {
                                Height = CssValue.From(Box.Intrinsic_Height.Value);
                            }
                            else if (Height.IsAuto)
                            {
                                /*
                                 * Otherwise, if 'height' has a computed value of 'auto', but none of the conditions above are met, 
                                 * then the used value of 'height' must be set to the height of the largest rectangle that has a 2:1 ratio, 
                                 * has a height not greater than 150px, and has a width not greater than the device width.
                                 */
                                /* Formula:  h = w * Min(2 (150/w)) */
                                var eq = (Width.AsDecimal() * MathExt.Min(2, (150 / Width.AsDecimal())));
                                Height = CssValue.From(eq);
                            }
                        }
                        break;
                }
            }


            switch (Box.DisplayGroup)
            {
                case EBoxDisplayGroup.INLINE:
                    {
                        if (!Box.IsReplacedElement)
                        {
                            /* The 'height' property does not apply. */
                        }
                    }
                    break;
                case EBoxDisplayGroup.BLOCK:
                    {
                        /* 10.6.3 Block-level non-replaced elements in normal flow when 'overflow' computes to 'visible' */
                        if (!Box.IsReplacedElement)
                        {/* This section also applies to block-level non-replaced elements in normal flow when 'overflow' does not compute to 'visible' but has been propagated to the viewport. */
                         /* If 'margin-top', or 'margin-bottom' are 'auto', their used value is 0. If 'height' is 'auto', the height depends on whether the element has any block-level children and whether it has padding or borders: */

                            if (Box.Style.Overflow_X == EOverflowMode.Visible)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                // XXX: Yea, implement this logic later.

                                /*
                                 * The element's height is the distance from its top content edge to the first applicable of the following:
                                 * 
                                 * 1) the bottom edge of the last line box, if the box establishes a inline formatting context with one or more lines
                                 * 2) the bottom edge of the bottom (possibly collapsed) margin of its last in-flow child, if the child's bottom margin does not collapse with the element's bottom margin
                                 * 3) the bottom border edge of the last in-flow child whose top margin doesn't collapse with the element's bottom margin
                                 * 4) zero, otherwise
                                 */

                                if (Box.Content_Height.HasValue)
                                    Height = CssValue.From(Box.Content_Height.Value);
                                else
                                    Height = CssValue.Zero;
                            }
                            else
                            {
                                /* If 'margin-top', or 'margin-bottom' are 'auto', their used value is 0. If 'height' is 'auto', the height depends on the element's descendants per 10.6.7. */
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                if (Height.IsAuto)
                                {
                                    Height = CssValue.From(Get_Height_For_Block_Formatting_Context(Box));
                                }
                            }
                        }
                    }
                    break;
                case EBoxDisplayGroup.ABSOLUTELY_POSITIONED:
                    {
                        /* 10.6.4 Absolutely positioned, non-replaced elements */
                        if (!Box.IsReplacedElement)
                        {
                            /* 
                             * For absolutely positioned elements, the used values of the vertical dimensions must satisfy this constraint:
                             * 'top' + 'margin-top' + 'border-top-width' + 'padding-top' + 'height' + 'padding-bottom' + 'border-bottom-width' + 'margin-bottom' + 'bottom' = height of containing block 
                             */

                            if (Top.IsAuto && Height.IsAuto && Bottom.IsAuto)
                            {
                                Top = CssValue.From(Box.Layout_Pos_Y);

                                /* Apply Rule #3 */
                                /* the height is based on the content per 10.6.7, set 'auto' values for 'margin-top' and 'margin-bottom' to 0, and solve for 'bottom' */
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                var h = Get_Height_For_Block_Formatting_Context(Box);
                                Height = CssValue.From(h);

                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (!Top.IsAuto && !Height.IsAuto && !Bottom.IsAuto)
                            {/* If none of the three are 'auto': If both 'margin-top' and 'margin-bottom' are 'auto', solve the equation under the extra constraint that the two margins get equal values. If one of 'margin-top' or 'margin-bottom' is 'auto', solve the equation for that value. If the values are over-constrained, ignore the value for 'bottom' and solve for that value. */
                                var heightVal = (Height.IsAuto ? 0 : Height.AsDecimal());
                                if (MarginTop.IsAuto && MarginBottom.IsAuto)
                                {
                                    var eqRes = (Top.AsDecimal() + 0 + BorderTop + PaddingTop + heightVal + PaddingBottom + BorderBottom + 0 + Bottom.AsDecimal());
                                    var avail = (CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                    MarginTop = MarginBottom = CssValue.From(avail/2);
                                }
                                else if (MarginTop.IsAuto ^ MarginBottom.IsAuto)
                                {/* Only a single margin is 'auto' */
                                    if (MarginTop.IsAuto)
                                    {
                                        var eqRes = (Top.AsDecimal() + 0 + BorderTop + PaddingTop + heightVal + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                        MarginTop = CssValue.From(avail / 2);
                                    }
                                    else if (MarginBottom.IsAuto)
                                    {
                                        var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + heightVal + PaddingBottom + BorderBottom + 0 + Bottom.AsDecimal());
                                        var avail = (CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                        MarginBottom = CssValue.From(avail / 2);
                                    }
                                }
                                else// margins are overconstrained
                                {/* Resolve for 'bottom' */
                                    var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + heightVal + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                    Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }
                            /*
                             * Otherwise, pick the one of the following six rules that applies.
                             * 1) 'top' and 'height' are 'auto' and 'bottom' is not 'auto', then the height is based on the content per 10.6.7, set 'auto' values for 'margin-top' and 'margin-bottom' to 0, and solve for 'top'
                             * 2) 'top' and 'bottom' are 'auto' and 'height' is not 'auto', then set 'top' to the static position, set 'auto' values for 'margin-top' and 'margin-bottom' to 0, and solve for 'bottom'
                             * 3) 'height' and 'bottom' are 'auto' and 'top' is not 'auto', then the height is based on the content per 10.6.7, set 'auto' values for 'margin-top' and 'margin-bottom' to 0, and solve for 'bottom'
                             * 4) 'top' is 'auto', 'height' and 'bottom' are not 'auto', then set 'auto' values for 'margin-top' and 'margin-bottom' to 0, and solve for 'top'
                             * 5) 'height' is 'auto', 'top' and 'bottom' are not 'auto', then 'auto' values for 'margin-top' and 'margin-bottom' are set to 0 and solve for 'height'
                             * 6) 'bottom' is 'auto', 'top' and 'height' are not 'auto', then set 'auto' values for 'margin-top' and 'margin-bottom' to 0 and solve for 'bottom'
                             */
                            if (Top.IsAuto && Height.IsAuto && !Bottom.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                Height = CssValue.From(Get_Height_For_Block_Formatting_Context(Box));

                                /* Solve for 'top' */
                                var eqRes = (0 + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                Top = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (Top.IsAuto && Bottom.IsAuto && !Height.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                Top = CssValue.From(Box.Layout_Pos_Y);

                                /* Solve for 'bottom' */
                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (Height.IsAuto && Bottom.IsAuto && !Top.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                Height = CssValue.From(Get_Height_For_Block_Formatting_Context(Box));

                                /* Solve for 'bottom' */
                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (Top.IsAuto && !Height.IsAuto && !Bottom.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                /* Solve for 'top' */
                                var eqRes = (0 + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                Top = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if(Height.IsAuto && !Top.IsAuto && !Bottom.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                /* Solve for 'top' */
                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + 0 + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                Height = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (Bottom.IsAuto && !Top.IsAuto && !Height.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                                /* Solve for 'bottom' */
                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }

                        }

                        /* 10.6.5 Absolutely positioned, replaced elements */
                        if (Box.IsReplacedElement)
                        {
                            /* The used value of 'height' is determined as for inline replaced elements */
                            if (Height.IsAuto && autoWidth && Box.Intrinsic_Height.HasValue)
                            {
                                Height = CssValue.From(Box.Intrinsic_Height.Value);
                            }
                            else if (Height.IsAuto && Box.Intrinsic_Ratio.HasValue)
                            {
                                Height = CssValue.From((Width.AsDecimal() / Box.Intrinsic_Ratio.Value));
                            }
                            else if (Height.IsAuto && Box.Intrinsic_Height.HasValue)
                            {
                                Height = CssValue.From(Box.Intrinsic_Height.Value);
                            }
                            else if (Height.IsAuto)
                            {
                                /*
                                 * Otherwise, if 'height' has a computed value of 'auto', but none of the conditions above are met, 
                                 * then the used value of 'height' must be set to the height of the largest rectangle that has a 2:1 ratio, 
                                 * has a height not greater than 150px, and has a width not greater than the device width.
                                 */
                                /* Formula:  h = w * Min(2 (150/w)) */
                                var eq = (Width.AsDecimal() * MathExt.Min(2, (150 / Width.AsDecimal())));
                                Height = CssValue.From(eq);
                            }

                            /* If 'margin-top' or 'margin-bottom' is specified as 'auto' its used value is determined by the rules below. */
                            if (Top.IsAuto && Bottom.IsAuto)
                            {
                                Top = CssValue.From(Box.Layout_Pos_Y);
                            }

                            if (Bottom.IsAuto)
                            {
                                if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                                if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;
                            }

                            if (MarginTop.IsAuto & MarginBottom.IsAuto)
                            {/* If at this point both 'margin-top' and 'margin-bottom' are still 'auto', solve the equation under the extra constraint that the two margins must get equal values. */

                                var eqRes = ((Top.IsAuto ? Top.AsDecimal() : 0) + 0 + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + 0 + (Bottom.IsAuto ? Bottom.AsDecimal() : 0));
                                var avail = (CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                MarginTop = MarginBottom = CssValue.From(avail / 2);
                            }

                            /* If at this point there is only one 'auto' left, solve the equation for that value. */
                            if (Top.IsAuto ^ MarginTop.IsAuto ^ MarginBottom.IsAuto ^ Bottom.IsAuto)
                            {
                                if (Top.IsAuto)
                                {
                                    var eqRes = (0 + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                    Top = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (MarginTop.IsAuto)
                                {
                                    var eqRes = (Top.AsDecimal() + 0 + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + Bottom.AsDecimal());
                                    MarginTop = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (MarginBottom.IsAuto)
                                {
                                    var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + 0 + Bottom.AsDecimal());
                                    MarginBottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                }
                                else if (Bottom.IsAuto)
                                {
                                    var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                    Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                                }
                            }

                            /* If at this point the values are over-constrained, ignore the value for 'bottom' and solve for that value. */
                            if (!Top.IsAuto && !MarginTop.IsAuto && !MarginBottom.IsAuto && !Bottom.IsAuto)
                            {
                                var eqRes = (Top.AsDecimal() + MarginTop.AsDecimal() + BorderTop + PaddingTop + Height.AsDecimal() + PaddingBottom + BorderBottom + MarginBottom.AsDecimal() + 0);
                                Bottom = CssValue.From(CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box) - eqRes);
                            }
                        }
                    }
                    break;
                case EBoxDisplayGroup.FLOATING:
                case EBoxDisplayGroup.INLINE_BLOCK:
                    {
                        if (!Box.IsReplacedElement)
                        {
                            /* If 'margin-top', or 'margin-bottom' are 'auto', their used value is 0. If 'height' is 'auto', the height depends on the element's descendants per 10.6.7. */
                            if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
                            if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;

                            if (Height.IsAuto)
                            {
                                Height = CssValue.From(Get_Height_For_Block_Formatting_Context(Box));
                            }
                        }
                    }
                    break;
            }

            outTop = Top;
            outMarginTop = MarginTop;
            outHeight = Height;
            outMarginBottom = MarginBottom;
            outBottom = Bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Get_Height_For_Block_Formatting_Context(CssPrincipalBox Box)
        {
            /* 
             * If it only has inline-level children, the height is the distance between the top of the topmost line box and the bottom of the bottommost line box.
             * If it has block-level children, the height is the distance between the top margin-edge of the topmost block-level child box and the bottom margin-edge of the bottommost block-level child box.
             * Absolutely positioned children are ignored, and relatively positioned boxes are considered without their offset. Note that the child box may be an anonymous block box.
             * In addition, if the element has any floating descendants whose bottom margin edge is below the element's bottom content edge, then the height is increased to include those edges. Only floats that participate in this block formatting context are taken into account, e.g., floats inside absolutely positioned descendants or other floats are not. 
             */

            if (!Box.HasBlockLevelChildren)
            {
                var topEdge = 0d;
                var bottomEdge = 0d;
                Element node;

                // find our first inline-level element and its top-margin-edge
                node = Box.Owner.firstElementChild;
                while (node is object)
                {
                    if (node.Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    {
                        topEdge = node.Box.Margin.Top;
                        break;
                    }

                    node = node.nextElementSibling;
                }

                // find our last inline-level element and its bottom-margin-edge
                node = Box.Owner.lastElementChild;
                while (node is object)
                {
                    if (node.Box.DisplayType.Outer == EOuterDisplayType.Inline)
                    {
                        bottomEdge = node.Box.Margin.Bottom;
                        break;
                    }

                    node = node.previousElementSibling;
                }

                return (bottomEdge - topEdge);
            }
            else
            {
                var topEdge = 0d;
                var bottomEdge = 0d;
                Element node;

                // find our first block-level element and its top-margin-edge
                node = Box.Owner.firstElementChild;
                while (node is object)
                {
                    if (node.Box.DisplayType.Outer == EOuterDisplayType.Block)
                    {
                        topEdge = node.Box.Margin.Top;
                        break;
                    }

                    node = node.nextElementSibling;
                }

                // find our last block-level element and its bottom-margin-edge
                node = Box.Owner.lastElementChild;
                while (node is object)
                {
                    if (node.Box.DisplayType.Outer == EOuterDisplayType.Block)
                    {
                        bottomEdge = node.Box.Margin.Bottom;
                        break;
                    }

                    node = node.previousElementSibling;
                }
                
                return (bottomEdge - topEdge);
            }
        }
        #endregion

    }
}
