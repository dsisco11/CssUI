using System;
using System.Diagnostics.Contracts;

namespace CssUI.CSS.Internal;

public static partial class CssPropertyResolver
{

    public static CssValue Border_Width_Used(ICssProperty Property)
    {// Docs: https://www.w3.org/TR/css-backgrounds-3/#the-border-width
        ArgumentNullException.ThrowIfNull(Property);
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

        switch (Property.CssName)
        {
            case ECssPropertyID.BorderLeftWidth:
                {
                    if (0 != (Property.Owner.Style.Border_Left_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                        return CssValue.From_Dimension(0, ECssUnit.PX);
                }
                break;
            case ECssPropertyID.BorderTopWidth:
                {
                    if (0 != (Property.Owner.Style.Border_Top_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                        return CssValue.From_Dimension(0, ECssUnit.PX);
                }
                break;
            case ECssPropertyID.BorderRightWidth:
                {
                    if (0 != (Property.Owner.Style.Border_Right_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                        return CssValue.From_Dimension(0, ECssUnit.PX);
                }
                break;
            case ECssPropertyID.BorderBottomWidth:
                {
                    if (0 != (Property.Owner.Style.Border_Bottom_Style & ((EBorderStyle.None | EBorderStyle.Hidden))))
                        return CssValue.From_Dimension(0, ECssUnit.PX);
                }
                break;
        }

        return CssValue.From_Dimension(Width, ECssUnit.PX);
    }


    public static CssValue Min_Width_Used(ICssProperty Property)
    {// Docs: https://www.w3.org/TR/css-sizing-3/#min-size-properties
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
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
        ArgumentNullException.ThrowIfNull(Property);
        Contract.EndContractBlock();

        var prop = (Property as CssProperty);
        CssValue Value = prop.Computed;
        if (!Value.IsDefinite)
        {
            return CssValue.From(0);
        }
        return Value;
    }

    /// <summary>
    /// Computes the display property value, handling special cases per CSS Display 3.
    /// </summary>
    /// <remarks>
    /// Per CSS Display 3 §2.5: "display: contents" computes to "display: none" on replaced
    /// elements and other elements whose rendering is not entirely controlled by CSS.
    /// This includes: img, video, canvas, audio, iframe, embed, object, input, textarea,
    /// select, br, wbr, meter, progress, frame, frameset.
    ///
    /// Per CSS Display 3 §2.7: Automatic box type transformations (blockification/inlinification)
    /// - Absolutely positioned elements (position: absolute/fixed) → blockify
    /// - Floated elements (float != none) → blockify (requires Phase 15)
    /// - Children of flex/grid containers → blockify
    /// - Children of ruby containers → inlinify (not implemented)
    ///
    /// Spec: https://www.w3.org/TR/css-display-3/#valdef-display-contents
    /// Spec: https://www.w3.org/TR/css-display-3/#transformations
    /// </remarks>
    public static CssValue Display_Computed(ICssProperty Property)
    {
        ArgumentNullException.ThrowIfNull(Property);
        Contract.EndContractBlock();

        var prop = (Property as CssProperty);
        CssValue specifiedValue = prop.Specified!;

        // Check if the specified value is display: contents (EDisplayMode.CONTENT)
        if (specifiedValue.Type == ECssValueTypes.INTEGER || specifiedValue.Type == ECssValueTypes.KEYWORD)
        {
            var displayMode = specifiedValue.AsEnum<EDisplayMode>();

            // Per CSS Display 3 §2.5: display: contents computes to display: none
            // for replaced elements
            if (displayMode == EDisplayMode.CONTENT)
            {
                if (Property.Owner.Style.IsReplacedElement)
                {
                    return CssValue.From(EDisplayMode.NONE);
                }
            }

            // Per CSS Display 3 §2.7: Skip blockification for none/contents
            // "This has no effect on display types that generate no box at all"
            if (displayMode == EDisplayMode.NONE || displayMode == EDisplayMode.CONTENT)
            {
                return specifiedValue;
            }

            // Per CSS Display 3 §2.8: Root element's display type is always blockified
            if (Property.Owner.isRoot)
            {
                return Blockify(displayMode);
            }

            // Check for blockification triggers (CSS Display 3 §2.7)
            if (ShouldBlockify(Property))
            {
                return Blockify(displayMode);
            }
        }

        return specifiedValue;
    }

    /// <summary>
    /// Determines whether the element should be blockified per CSS Display 3 §2.7.
    /// </summary>
    private static bool ShouldBlockify(ICssProperty Property)
    {
        var style = Property.Owner.Style;

        // 1. Absolutely positioned elements (position: absolute or fixed)
        // Use Specified value to avoid circular dependency - Positioning has no computed resolver
        var positioningSpecified = style.Cascaded.Positioning.Specified;
        if (positioningSpecified is not null && positioningSpecified.HasValue)
        {
            var positioning = positioningSpecified.AsEnum<EBoxPositioning>();
            if (positioning == EBoxPositioning.Absolute || positioning == EBoxPositioning.Fixed)
            {
                return true;
            }
        }

        // 2. Floated elements (float != none)
        // @todo: Implement when Float property is added in Phase 15
        // var floatSpecified = style.Cascaded.Float?.Specified;
        // if (floatSpecified is not null && floatSpecified.HasValue)
        // {
        //     var floatValue = floatSpecified.AsEnum<EFloat>();
        //     if (floatValue != EFloat.None)
        //     {
        //         return true;
        //     }
        // }

        // 3. Children of flex/grid containers
        var parentElement = Property.Owner.parentElement;
        if (parentElement is not null)
        {
            // Use Specified value of parent's display to avoid circular dependencies
            var parentDisplaySpecified = parentElement.Style.Cascaded.Display.Specified;
            if (parentDisplaySpecified is not null && parentDisplaySpecified.HasValue)
            {
                var parentDisplay = parentDisplaySpecified.AsEnum<EDisplayMode>();
                if (parentDisplay == EDisplayMode.FLEX || parentDisplay == EDisplayMode.INLINE_FLEX ||
                    parentDisplay == EDisplayMode.GRID || parentDisplay == EDisplayMode.INLINE_GRID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Blockifies the display mode per CSS Display 3 §2.7.
    /// </summary>
    /// <remarks>
    /// Blockification rules:
    /// - Sets outer display type to 'block'
    /// - inline-block → block (loses flow-root for legacy reasons)
    /// - run-in flow-root → block (for consistency)
    /// - layout-internal types → block with flow inner display
    /// - Other inline types → block (preserving inner display type)
    /// </remarks>
    private static CssValue Blockify(EDisplayMode displayMode)
    {
        var displayType = new DisplayType(displayMode);

        // If already block-level, no transformation needed
        if (displayType.IsBlockLevel)
        {
            return CssValue.From(displayMode);
        }

        // Per CSS Display 3 §2.7:
        // "For legacy reasons, if an inline block box ('inline flow-root') is blockified,
        // it becomes a 'block' box (losing its 'flow-root' nature)."
        if (displayMode == EDisplayMode.INLINE_BLOCK)
        {
            return CssValue.From(EDisplayMode.BLOCK);
        }

        // inline-flex → flex
        if (displayMode == EDisplayMode.INLINE_FLEX)
        {
            return CssValue.From(EDisplayMode.FLEX);
        }

        // inline-grid → grid
        if (displayMode == EDisplayMode.INLINE_GRID)
        {
            return CssValue.From(EDisplayMode.GRID);
        }

        // inline-table → table
        if (displayMode == EDisplayMode.INLINE_TABLE)
        {
            return CssValue.From(EDisplayMode.TABLE);
        }

        // Per CSS Display 3 §2.7:
        // "If a layout-internal box is blockified, its inner display type converts to 'flow'
        // so that it becomes a block container."
        if (displayType.IsLayoutInternal)
        {
            return CssValue.From(EDisplayMode.BLOCK);
        }

        // inline → block (with flow inner)
        if (displayMode == EDisplayMode.INLINE)
        {
            return CssValue.From(EDisplayMode.BLOCK);
        }

        // For run-in and other cases, convert to block
        if (displayMode == EDisplayMode.RUN_IN)
        {
            return CssValue.From(EDisplayMode.BLOCK);
        }

        // Default: preserve the display mode as-is if we don't have a specific transformation
        // This shouldn't normally be reached for inline-level types
        return CssValue.From(displayMode);
    }
}

