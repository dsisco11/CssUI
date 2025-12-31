using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.CSS.Layout;
using CssUI.DOM;

namespace CssUI.CSS;

/* PREFACE:
 * An explination for some of the inlining here.
 * Some of these functions are private and only called a few times by the other algorithms,
 * a minor optimization is to remove a jmp call for these functions.
 * Over the course of MANY nodes this could have a positive impact and the memory impact should be very negligable.
 * So I'm sacrificing a few KB of memory for a little speed since these functions are called a lot.
 */


public static class BoxModel
{
    #region Shrink-to-Fit Helper

    /// <summary>
    /// Calculates shrink-to-fit width using intrinsic size calculations.
    /// </summary>
    /// <remarks>
    /// Shrink-to-fit width is: min(max(min-content, available), max-content)
    /// See: https://www.w3.org/TR/CSS2/visudet.html#shrink-to-fit-float
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double CalculateShrinkToFitWidth(CssPrincipalBox box, double availableWidth)
    {
        // Get intrinsic size (cached)
        var intrinsic = box.GetIntrinsicSize();

        // Use fit-content algorithm: clamp(min-content, max-content, available)
        return IntrinsicSizeCalculator.FitContent(intrinsic.Inline, availableWidth);
    }

    #endregion

    /// <summary>
    /// Resolves used values for all box properties (width, height, margins, and offsets).
    /// This is the original combined method that calls both ResolveWidth and ResolveHeight.
    /// </summary>
    public static void Resolve(CssPrincipalBox Box, CssComputedStyle Cascaded)
    {
        ArgumentNullException.ThrowIfNull(Box);
        ArgumentNullException.ThrowIfNull(Cascaded);
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
    /// Resolves used values for horizontal properties only: width, margin-left, margin-right, left, right.
    /// This should be called during the top-down width resolution pass.
    /// </summary>
    /// <remarks>
    /// Width resolution is top-down because percentages resolve against the containing block's width,
    /// which is determined by the parent element.
    /// See: https://www.w3.org/TR/CSS2/visudet.html#Computing_widths_and_margins
    /// </remarks>
    public static void ResolveWidth(CssPrincipalBox Box, CssComputedStyle Cascaded)
    {
        ArgumentNullException.ThrowIfNull(Box);
        ArgumentNullException.ThrowIfNull(Cascaded);
        Contract.EndContractBlock();

        var tracker = LayoutCycleTracker.Current;
        if (!tracker.BeginWidthResolution(Box))
        {
            // Cycle detected - use auto for width
            Cascaded.Width.Set_Computed_Value(CssValue.Auto);
            return;
        }

        try
        {
            Resolve_Horizontal(Box, Cascaded, out CssValue Left, out CssValue MarginLeft, out CssValue Width, out CssValue MarginRight, out CssValue Right);

            Cascaded.Width.Set_Computed_Value(Width);
            Cascaded.Left.Set_Computed_Value(Left);
            Cascaded.Right.Set_Computed_Value(Right);
            Cascaded.Margin_Left.Set_Computed_Value(MarginLeft);
            Cascaded.Margin_Right.Set_Computed_Value(MarginRight);

            // Update the box's Content area width so children can use it as their containing block
            // during the top-down width resolution pass.
            Box.UpdateContentWidth();
        }
        finally
        {
            tracker.EndWidthResolution(Box);
        }
    }

    /// <summary>
    /// Resolves used values for vertical properties only: height, margin-top, margin-bottom, top, bottom.
    /// This should be called during the bottom-up height resolution pass, after Flow() has been called.
    /// </summary>
    /// <param name="Box">The principal box to resolve</param>
    /// <param name="Cascaded">The computed style</param>
    /// <param name="contentHeight">
    /// Optional content height from Flow(). When height is 'auto', this value is used instead of
    /// calculating height from the box's Content_Height property. This allows proper bottom-up
    /// height resolution where children's heights inform parent's height.
    /// </param>
    /// <remarks>
    /// Height resolution is bottom-up because 'height: auto' depends on the content height,
    /// which is only known after child elements have been laid out.
    /// See: https://www.w3.org/TR/CSS2/visudet.html#Computing_heights_and_margins
    /// </remarks>
    public static void ResolveHeight(CssPrincipalBox Box, CssComputedStyle Cascaded, double? contentHeight = null)
    {
        ArgumentNullException.ThrowIfNull(Box);
        ArgumentNullException.ThrowIfNull(Cascaded);
        Contract.EndContractBlock();

        var tracker = LayoutCycleTracker.Current;
        if (!tracker.BeginHeightResolution(Box))
        {
            // Cycle detected - use auto for height
            Cascaded.Height.Set_Computed_Value(CssValue.Auto);
            return;
        }

        try
        {
            // If contentHeight is provided and height is auto, set Content_Height
            // so the height resolution algorithm can use it
            if (contentHeight.HasValue && Cascaded.Height.Computed.IsAuto)
            {
                // Set_Content_Height takes int?, so we convert from double
                Box.Set_Content_Height((int)contentHeight.Value);
            }

            Resolve_Vertical(Box, Cascaded, out CssValue Top, out CssValue MarginTop, out CssValue Height, out CssValue MarginBottom, out CssValue Bottom);

            Cascaded.Height.Set_Computed_Value(Height);
            Cascaded.Top.Set_Computed_Value(Top);
            Cascaded.Bottom.Set_Computed_Value(Bottom);
            Cascaded.Margin_Top.Set_Computed_Value(MarginTop);
            Cascaded.Margin_Bottom.Set_Computed_Value(MarginBottom);
        }
        finally
        {
            tracker.EndHeightResolution(Box);
        }
    }

    /// <summary>
    /// Resolves 'Used' values for the following properties:
    /// Width, Height, Top, Right, Bottom, Left, Margin-Left, Margin-Right
    /// </summary>
    internal static void Resolve_Box_Properties_Used_Value(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue Left, out CssValue MarginLeft, out CssValue Width, out CssValue MarginRight, out CssValue Right, out CssValue Top, out CssValue MarginTop, out CssValue Height, out CssValue MarginBottom, out CssValue Bottom)
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
    internal static void Resolve_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight)
    {
        // Resolve percentages using each property's defined Percentage_Resolver (CSS Values 4 §5.1.1)
        // This is done during layout phase when boxes are guaranteed to exist.
        CssValue Left = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Left);
        CssValue MarginLeft = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Margin_Left);
        CssValue Width = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Width);
        CssValue MarginRight = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Margin_Right);
        CssValue Right = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Right);

        // First pass: resolve auto values
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
            // Only apply min/max constraints if width is no longer auto
            // (auto should have been resolved by Calculate_Horizontal)
            if (!Width.IsAuto)
            {
                bool needsRecalc = false;

                if (Max_Width.HasValue && Width.AsDecimal() > Max_Width.Value)
                {
                    Width = CssValue.From(Max_Width.Value);
                    needsRecalc = true;
                }

                // Per CSS 2.1 §10.4: If min-width > max-width, max-width is ignored (min wins)
                if (Width.AsDecimal() < Min_Width)
                {
                    Width = CssValue.From(Min_Width);
                    needsRecalc = true;
                }

                // Recalculate margins if width was constrained
                if (needsRecalc)
                {
                    Calculate_Horizontal(Box, Cascaded, ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);
                }
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
    internal static void Resolve_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom)
    {
        // Resolve percentages using each property's defined Percentage_Resolver (CSS Values 4 §5.1.1)
        // This is done during layout phase when boxes are guaranteed to exist.
        CssValue Top = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Top);
        CssValue MarginTop = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Margin_Top);
        CssValue Height = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Height);
        CssValue MarginBottom = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Margin_Bottom);
        CssValue Bottom = CssPercentageResolvers.ResolvePercentageIfNeeded(Cascaded.Bottom);

        // CSS 2.1 §10.5: If the height of the containing block is not specified explicitly
        // (i.e., it depends on content height), and this element is not absolutely positioned,
        // a percentage height is treated as 'auto'.
        // Check if height was originally a percentage and CB height is not explicit
        bool isAbsolutelyPositioned = Box.DisplayGroup == EBoxDisplayGroup.ABSOLUTELY_POSITIONED;
        bool percentageResolvedToAuto = false;
        if (!isAbsolutelyPositioned && Cascaded.Height.Computed?.Type == ECssValueTypes.PERCENT)
        {
            // Only treat as auto if the containing block doesn't have explicit height
            if (!Box.Containing_Box_Explicit_Height)
            {
                Height = CssValue.Auto;
                percentageResolvedToAuto = true;
            }
        }

        /*
         * However, for replaced elements with both 'width' and 'height' computed as 'auto',
         * use the algorithm under 'Minimum and maximum widths' above to find the used width and height.
         * Then apply the rules under "Computing heights and margins" above, using the resulting width and height as if they were the computed values.
         */
        bool autoWidth = Cascaded.Width.Computed?.IsAuto ?? true;
        bool autoHeight = Height.IsAuto;

        var Min_Height = Cascaded.Min_Height.Actual;
        var Max_Height = Cascaded.Max_Height.Actual;

        if (Box.IsReplacedElement && autoWidth && autoHeight)
        {
            CssValue Width = Cascaded.Width.Computed ?? CssValue.Auto;
            Constrain_Width_Height(Cascaded, ref Width, ref Height);
            Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);
        }
        else if (percentageResolvedToAuto)
        {
            // CSS 2.1 §10.5: Percentage height resolved to auto because containing block
            // has auto height. We only resolve margins here, not the height itself.
            // The height stays 'auto' until content dimensions are known.
            if (MarginTop.IsAuto) MarginTop = CssValue.Zero;
            if (MarginBottom.IsAuto) MarginBottom = CssValue.Zero;
            // Height stays CssValue.Auto - it was set above and should remain auto
        }
        else
        {
            // First pass: resolve auto values
            Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);

            // Only apply min/max constraints if Height is no longer auto
            if (!Height.IsAuto)
            {
                bool needsRecalc = false;

                if (Max_Height.HasValue && Height.AsDecimal() > Max_Height.Value)
                {
                    Height = new CssIntegerValue(Max_Height.Value);
                    needsRecalc = true;
                }

                // Per CSS 2.1 §10.7: If min-height > max-height, max-height is ignored (min wins)
                if (Height.AsDecimal() < Min_Height)
                {
                    Height = new CssIntegerValue(Min_Height);
                    needsRecalc = true;
                }

                if (needsRecalc)
                {
                    Calculate_Vertical(Box, Cascaded, ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);
                }
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
    internal static bool Constrain_Width_Height(CssComputedStyle Cascaded, ref CssValue Width, ref CssValue Height)
    {/* Docs: https://www.w3.org/TR/CSS22/visudet.html#min-max-widths */
        /*
        * Select from the table the resolved height and width values for the appropriate constraint violation.
        * Take the max-width and max-height as max(min, max) so that min ≤ max holds true.
        * In this table w and h stand for the results of the width and height computations ignoring the 'min-width', 'min-height', 'max-width' and 'max-height' properties.
        * Normally these are the intrinsic width and height, but they may not be in the case of replaced elements with intrinsic ratios.
        */

        var Min_Width = Cascaded.Min_Width.Actual;
        // When max-width/max-height is not set (null), use double.MaxValue to indicate no constraint
        var Max_Width = Cascaded.Max_Width.Actual.HasValue
            ? Math.Max(Min_Width, Cascaded.Max_Width.Actual.Value)
            : double.MaxValue;

        var Min_Height = Cascaded.Min_Height.Actual;
        var Max_Height = Cascaded.Max_Height.Actual.HasValue
            ? Math.Max(Min_Height, Cascaded.Max_Height.Actual.Value)
            : double.MaxValue;

        var w = Width.AsDecimal();
        var h = Height.AsDecimal();

        // Calculate the intrinsic ratio (width/height)
        var ratio = h > 0 ? w / h : 1.0;

        // Determine constraint violations
        bool wViolatesMax = w > Max_Width;
        bool wViolatesMin = w < Min_Width;
        bool hViolatesMax = h > Max_Height;
        bool hViolatesMin = h < Min_Height;

        double newWidth = w;
        double newHeight = h;
        bool valuesChanged = false;

        // CSS 2.1 §10.4 constraint violation table:
        // Check each case and apply the appropriate constraint while maintaining ratio

        if (wViolatesMax && hViolatesMax)
        {
            // Both exceed max - scale down to fit within both constraints
            double wScaled = Max_Width;
            double hForWScaled = Max_Width / ratio;
            double hScaled = Max_Height;
            double wForHScaled = Max_Height * ratio;

            // Use the more constrained dimension
            if (hForWScaled <= Max_Height)
            {
                newWidth = wScaled;
                newHeight = Math.Max(Min_Height, hForWScaled);
            }
            else
            {
                newWidth = Math.Max(Min_Width, wForHScaled);
                newHeight = hScaled;
            }
            valuesChanged = true;
        }
        else if (wViolatesMin && hViolatesMin)
        {
            // Both below min - scale up to meet both constraints
            double wScaled = Min_Width;
            double hForWScaled = Min_Width / ratio;
            double hScaled = Min_Height;
            double wForHScaled = Min_Height * ratio;

            // Use the more constraining dimension (which results in larger size)
            if (hForWScaled >= Min_Height)
            {
                newWidth = Math.Min(Max_Width, wScaled);
                newHeight = Math.Min(Max_Height, hForWScaled);
            }
            else
            {
                newWidth = Math.Min(Max_Width, wForHScaled);
                newHeight = Math.Min(Max_Height, hScaled);
            }
            valuesChanged = true;
        }
        else if (wViolatesMax && hViolatesMin)
        {
            // Width too big, height too small - conflicting constraints
            newWidth = Max_Width;
            newHeight = Min_Height;
            valuesChanged = true;
        }
        else if (wViolatesMin && hViolatesMax)
        {
            // Width too small, height too big - conflicting constraints
            newWidth = Min_Width;
            newHeight = Max_Height;
            valuesChanged = true;
        }
        else if (wViolatesMax)
        {
            // Only width exceeds max - scale down width and proportionally scale height
            newWidth = Max_Width;
            newHeight = Math.Max(Min_Height, Max_Width / ratio);
            valuesChanged = true;
        }
        else if (wViolatesMin)
        {
            // Only width below min - scale up width and proportionally scale height
            newWidth = Min_Width;
            newHeight = Min_Width / ratio;
            // Clamp height to max if it exceeds
            if (Max_Height < double.MaxValue && newHeight > Max_Height)
            {
                newHeight = Max_Height;
            }
            valuesChanged = true;
        }
        else if (hViolatesMax)
        {
            // Only height exceeds max - scale down height and proportionally scale width
            newHeight = Max_Height;
            newWidth = Math.Max(Min_Width, Max_Height * ratio);
            valuesChanged = true;
        }
        else if (hViolatesMin)
        {
            // Only height below min - scale up height and proportionally scale width
            newHeight = Min_Height;
            newWidth = Min_Height * ratio;
            // Clamp width to max if it exceeds
            if (Max_Width < double.MaxValue && newWidth > Max_Width)
            {
                newWidth = Max_Width;
            }
            valuesChanged = true;
        }

        if (valuesChanged)
        {
            Width = new CssNumberValue(newWidth);
            Height = new CssNumberValue(newHeight);
        }

        return valuesChanged;
    }


    #region Calculate Horizontal
    /// <summary>
    /// Calculates all horizontal property values using the ones given
    /// </summary>
    internal static void Calculate_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, ref CssValue Left, ref CssValue MarginLeft, ref CssValue Width, ref CssValue MarginRight, ref CssValue Right)
    {
        Calculate_Horizontal(Box, Cascaded, Left, MarginLeft, Width, MarginRight, Right, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight);

        Left = outLeft;
        MarginLeft = outMarginLeft;
        Width = outWidth;
        MarginRight = outMarginRight;
        Right = outRight;
    }

    /// <summary>
    /// Calculates all horizontal property values using the ones given
    /// </summary>
    internal static void Calculate_Horizontal(CssPrincipalBox Box, CssComputedStyle Cascaded, CssValue Left, CssValue MarginLeft, CssValue Width, CssValue MarginRight, CssValue Right, out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth, out CssValue outMarginRight, out CssValue outRight)
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

                        if (Width.IsAuto && !autoHeight && Box.Intrinsic_Ratio.HasValue)
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
                            Width = CssValue.From_Dimension(300, ECssUnit.PX);
                        }
                    }
                }
                break;
            case EBoxDisplayGroup.BLOCK:
                {
                    /* 10.3.3 Block-level, non-replaced elements in normal flow */
                    if (!Box.IsReplacedElement)
                    {/* 10.3.3 Block-level, non-replaced elements in normal flow */
                        /*
                         * The constraint equation is:
                         * 'margin-left' + 'border-left-width' + 'padding-left' + 'width' + 'padding-right' + 'border-right-width' + 'margin-right' = width of containing block
                         *
                         * Per CSS 2.1 §10.3.3, the resolution order is:
                         * 1. If 'width' is not 'auto' and exceeds container, set auto margins to 0
                         * 2. If 'width' is 'auto', set auto margins to 0 and solve for width
                         * 3. If both margins are 'auto' (and width is not), they're equal (centering)
                         * 4. If one margin is 'auto', it gets remaining space
                         * 5. If over-constrained (no autos), adjust margin-right (LTR) or margin-left (RTL)
                         */

                        var containingWidth = CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box);

                        // Step 1: Handle exceeds case
                        /* If 'width' is not 'auto' and the total exceeds the containing block width,
                           then any 'auto' values for margins are treated as zero. */
                        if (!Width.IsAuto)
                        {
                            var totalNonAuto = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                            if (totalNonAuto > containingWidth)
                            {
                                if (MarginLeft.IsAuto)
                                {
                                    MarginLeft = CssValue.Zero;
                                    marginLeft = 0;
                                }
                                if (MarginRight.IsAuto)
                                {
                                    MarginRight = CssValue.Zero;
                                    marginRight = 0;
                                }
                            }
                        }

                        // Step 2: Handle width: auto
                        if (Width.IsAuto)
                        {
                            // If 'width' is set to 'auto', any other 'auto' values become '0'
                            if (MarginLeft.IsAuto)
                            {
                                MarginLeft = CssValue.Zero;
                                marginLeft = 0;
                            }
                            if (MarginRight.IsAuto)
                            {
                                MarginRight = CssValue.Zero;
                                marginRight = 0;
                            }
                            // Width follows from the equality
                            var total = marginLeft + BorderLeft + PaddingLeft + PaddingRight + BorderRight + marginRight;
                            Width = CssValue.From(Math.Max(0, containingWidth - total));
                        }
                        // Step 3: Both margins auto (and width is not) - centering
                        else if (MarginLeft.IsAuto && MarginRight.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            var availableSpace = containingWidth - usedSpace;
                            var eachMargin = availableSpace / 2.0;
                            MarginLeft = CssValue.From(eachMargin);
                            MarginRight = CssValue.From(eachMargin);
                        }
                        // Step 4: Single auto margin
                        else if (MarginLeft.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight;
                            MarginLeft = CssValue.From(Math.Max(0, containingWidth - usedSpace));
                        }
                        else if (MarginRight.IsAuto)
                        {
                            var usedSpace = marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            MarginRight = CssValue.From(Math.Max(0, containingWidth - usedSpace));
                        }
                        // Step 5: Over-constrained (no autos)
                        else
                        {
                            // All values are specified - over-constrained
                            // Adjust margin-right (LTR) or margin-left (RTL) to satisfy constraint
                            switch (Direction)
                            {
                                case EDirection.LTR:
                                    {
                                        var usedSpace = marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                                        MarginRight = CssValue.From(containingWidth - usedSpace);
                                    }
                                    break;
                                case EDirection.RTL:
                                    {
                                        var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight;
                                        MarginLeft = CssValue.From(containingWidth - usedSpace);
                                    }
                                    break;
                            }
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

                        if (Width.IsAuto && !autoHeight && Box.Intrinsic_Ratio.HasValue)
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
                            Width = CssValue.From_Dimension(300, ECssUnit.PX);
                        }
                        #endregion

                        /* Apply margin rules for block-level replaced elements (same as non-replaced) */
                        var containingWidth = CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box);

                        // Handle exceeds case
                        if (!Width.IsAuto)
                        {
                            var totalNonAuto = (marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight);
                            if (totalNonAuto > containingWidth)
                            {
                                if (MarginLeft.IsAuto)
                                {
                                    MarginLeft = CssValue.Zero;
                                    marginLeft = 0;
                                }
                                if (MarginRight.IsAuto)
                                {
                                    MarginRight = CssValue.Zero;
                                    marginRight = 0;
                                }
                            }
                        }

                        // Both margins auto - centering
                        if (MarginLeft.IsAuto && MarginRight.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            var availableSpace = containingWidth - usedSpace;
                            var eachMargin = availableSpace / 2.0;
                            MarginLeft = CssValue.From(eachMargin);
                            MarginRight = CssValue.From(eachMargin);
                        }
                        // Single auto margin
                        else if (MarginLeft.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight;
                            MarginLeft = CssValue.From(Math.Max(0, containingWidth - usedSpace));
                        }
                        else if (MarginRight.IsAuto)
                        {
                            var usedSpace = marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            MarginRight = CssValue.From(Math.Max(0, containingWidth - usedSpace));
                        }
                        // Over-constrained (no autos)
                        else
                        {
                            switch (Direction)
                            {
                                case EDirection.LTR:
                                    {
                                        var usedSpace = marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                                        MarginRight = CssValue.From(containingWidth - usedSpace);
                                    }
                                    break;
                                case EDirection.RTL:
                                    {
                                        var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight;
                                        MarginLeft = CssValue.From(containingWidth - usedSpace);
                                    }
                                    break;
                            }
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

                            Width = CssValue.From(CalculateShrinkToFitWidth(Box, available_width));
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

                        if (Width.IsAuto && !autoHeight && Box.Intrinsic_Ratio.HasValue)
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
                            Width = CssValue.From_Dimension(300, ECssUnit.PX);
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
                                Width = CssValue.From(CalculateShrinkToFitWidth(Box, avail));

                                /* Solve for 'right' */
                                eqRes = (Left.AsDecimal() + marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight + 0);
                                Right = CssValue.From(Math.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes));
                            }
                            else if (Direction == EDirection.RTL)
                            {
                                Right = CssValue.From(Box.Layout_Pos_X);
                                /* Apply Rule #1 */
                                /* the width is shrink-to-fit . Then solve for 'left' */
                                var eqRes = (0 + marginLeft + BorderLeft + PaddingLeft + 0 + PaddingRight + BorderRight + marginRight + Right.AsDecimal());
                                var avail = (CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                                Width = CssValue.From(CalculateShrinkToFitWidth(Box, avail));

                                /* Solve for 'left' */
                                eqRes = (0 + marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight + Right.AsDecimal());
                                Left = CssValue.From(Math.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes));

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
                                Width = CssValue.From(CalculateShrinkToFitWidth(Box, avail));

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
                                Width = CssValue.From(CalculateShrinkToFitWidth(Box, avail));
                                /* Solve for 'right' */
                                eqRes = (Left.AsDecimal() + MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + MarginRight.AsDecimal() + 0);
                                Right = CssValue.From(CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - eqRes);
                            }
                            else if (Left.IsAuto && !Width.IsAuto && !Right.IsAuto)
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

                        if (Width.IsAuto && !autoHeight && Box.Intrinsic_Ratio.HasValue)
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
                            Width = CssValue.From_Dimension(300, ECssUnit.PX);
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
                            Width = CssValue.From(CalculateShrinkToFitWidth(Box, avail));
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

                        if (Width.IsAuto && !autoHeight && Box.Intrinsic_Ratio.HasValue)
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
                            Width = CssValue.From_Dimension(300, ECssUnit.PX);
                        }
                    }
                }
                break;

            default:
                {
                    // INVALID or unhandled display type - treat as block for width calculation
                    // This ensures we don't leave width/margins unresolved
                    if (Width.IsAuto)
                    {
                        if (MarginLeft.IsAuto)
                            MarginLeft = CssValue.Zero;
                        if (MarginRight.IsAuto)
                            MarginRight = CssValue.Zero;

                        var total = (MarginLeft.AsDecimal() + BorderLeft + PaddingLeft + PaddingRight + BorderRight + MarginRight.AsDecimal());
                        Width = CssValue.From(Math.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - total));
                    }
                    else
                    {
                        // Both margins auto - centering
                        if (MarginLeft.IsAuto && MarginRight.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            var availableSpace = CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - usedSpace;
                            var eachMargin = availableSpace / 2.0;
                            MarginLeft = CssValue.From(eachMargin);
                            MarginRight = CssValue.From(eachMargin);
                        }
                        else if (MarginLeft.IsAuto)
                        {
                            var usedSpace = BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight + marginRight;
                            MarginLeft = CssValue.From(Math.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - usedSpace));
                        }
                        else if (MarginRight.IsAuto)
                        {
                            var usedSpace = marginLeft + BorderLeft + PaddingLeft + Width.AsDecimal() + PaddingRight + BorderRight;
                            MarginRight = CssValue.From(Math.Max(0, CssCommon.Get_Logical_Width(WritingMode, Box.Containing_Box) - usedSpace));
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
    internal static void Calculate_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, ref CssValue Top, ref CssValue MarginTop, ref CssValue Height, ref CssValue MarginBottom, ref CssValue Bottom, CssValue? WidthOverride = null)
    {
        Calculate_Vertical(Box, Cascaded, Top, MarginTop, Height, MarginBottom, Bottom, WidthOverride!, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom);

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
    internal static void Calculate_Vertical(CssPrincipalBox Box, CssComputedStyle Cascaded, CssValue Top, CssValue MarginTop, CssValue Height, CssValue MarginBottom, CssValue Bottom, CssValue Width, out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight, out CssValue outMarginBottom, out CssValue outBottom)
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
            switch (Box.DisplayGroup)
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
                            var eq = (Width.AsDecimal() * Math.Min(2, (150 / Width.AsDecimal())));
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

                            // Per CSS 2.1 §10.6.3: Only compute height when 'height' is 'auto'
                            // Specified heights should be used as-is
                            if (Height.IsAuto)
                            {
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
                                // Calculate space used by everything except margins
                                var usedSpace = Top.AsDecimal() + BorderTop + PaddingTop + heightVal + PaddingBottom + BorderBottom + Bottom.AsDecimal();
                                var containingHeight = CssCommon.Get_Logical_Height(WritingMode, Box.Containing_Box);
                                var availForMargins = containingHeight - usedSpace;

                                // Per CSS 2.1 §10.6.4: If centering would result in negative margins,
                                // set margin-top to zero and solve for margin-bottom
                                if (availForMargins < 0)
                                {
                                    MarginTop = CssValue.Zero;
                                    // Solve constraint: top + 0 + borders/padding + height + margin-bottom + bottom = CB height
                                    // margin-bottom = CB height - top - borders/padding - height - bottom
                                    MarginBottom = CssValue.From(availForMargins);
                                }
                                else
                                {
                                    MarginTop = MarginBottom = CssValue.From(availForMargins / 2);
                                }
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
                        else if (Height.IsAuto && !Top.IsAuto && !Bottom.IsAuto)
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
                            var eq = (Width.AsDecimal() * Math.Min(2, (150 / Width.AsDecimal())));
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
    internal static double Get_Height_For_Block_Formatting_Context(CssPrincipalBox Box)
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
            Element? node;

            // find our first inline-level element and its top-margin-edge
            node = Box.Owner?.firstElementChild;
            while (node is not null)
            {
                if (node.Box.DisplayType.Outer == EOuterDisplayType.Inline)
                {
                    topEdge = node.Box.Margin.Top;
                    break;
                }

                node = node.nextElementSibling;
            }

            // find our last inline-level element and its bottom-margin-edge
            node = Box.Owner?.lastElementChild;
            while (node is not null)
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
            Element? node;

            // find our first block-level element and its top-margin-edge
            node = Box.Owner?.firstElementChild;
            while (node is not null)
            {
                if (node.Box.DisplayType.Outer == EOuterDisplayType.Block)
                {
                    topEdge = node.Box.Margin.Top;
                    break;
                }

                node = node.nextElementSibling;
            }

            // find our last block-level element and its bottom-margin-edge
            node = Box.Owner?.lastElementChild;
            while (node is not null)
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
