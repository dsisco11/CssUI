using System;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Layout;

namespace CssUI.CSS.Internal;

/// <summary>
/// A function that transforms a percentage into an absolute value.
/// Called during layout phase when boxes are guaranteed to exist.
/// </summary>
/// <param name="Property">The property being resolved</param>
/// <param name="Percent">Decimal in the range 0.0 - 1.0</param>
/// <returns>The resolved absolute value</returns>
public delegate CssValue CssPercentageResolver(ICssProperty Property, double Percent);

public static class CssPercentageResolvers
{
    /// <summary>
    /// Resolves a percentage value for a property during the layout phase.
    /// Uses the property's defined Percentage_Resolver from its StyleDefinition.
    /// </summary>
    /// <param name="property">The property to resolve</param>
    /// <returns>The resolved value, or the original computed value if not a percentage</returns>
    public static CssValue ResolvePercentageIfNeeded(CssProperty property)
    {
        var computed = property.Computed;
        if (computed is null)
        {
            return CssValue.Null;
        }

        // Not a percentage - return as-is
        if (computed.Type != ECssValueTypes.PERCENT)
        {
            return computed;
        }

        // Get the property's percentage resolver
        var resolver = property.Definition?.Percentage_Resolver;
        if (resolver is null)
        {
            // No resolver defined - this is unexpected for layout properties
            throw new InvalidOperationException(
                $"Property {property.CssName} has percentage value but no Percentage_Resolver defined. " +
                $"Definition is {(property.Definition is null ? "null" : "not null")}");
        }

        // CssPercentValue stores values in 0-100 range, convert to 0-1 range for resolver
        double percentDecimal = computed.AsDecimal() / 100.0;

        return resolver(property, percentDecimal);
    }

    /// <summary>
    /// Resolves a percentage against the containing block's logical width.
    /// Used for: width, margin-left, margin-right, padding-left, padding-right, left, right.
    /// </summary>
    /// <remarks>
    /// If a layout cycle is detected (ancestor being resolved), returns zero to break the cycle.
    /// See CSS 2.2 §10.5 for percentage height resolution rules.
    /// </remarks>
    public static CssValue Containing_Block_Logical_Width(ICssProperty Property, double Percent)
    {
        // Validate that the box exists - percentage resolution only happens during layout
        // when boxes are guaranteed to exist. If box is null, it indicates a pipeline bug.
        var box = Property.Owner.Box ?? throw new InvalidOperationException(
            $"Cannot resolve percentage for property '{Property.CssName}' on element " +
            $"<{Property.Owner.nodeName ?? "unknown"}>: Box is null. " +
            $"Percentage resolution should only occur during layout phase when boxes exist.");

        // Check for layout cycle
        if (box is CssPrincipalBox principalBox)
        {
            var tracker = LayoutCycleTracker.Current;
            var resolvingAncestor = tracker.FindResolvingAncestor(principalBox);
            if (resolvingAncestor is not null)
            {
                Log.Warn($"[LayoutCycle] Width percentage cycle detected: " +
                    $"<{principalBox.Owner?.nodeName ?? "unknown"}> depends on ancestor " +
                    $"<{resolvingAncestor.Owner?.nodeName ?? "unknown"}> which is being resolved. " +
                    $"Treating as zero.");
                return CssValue.Zero;
            }
        }

        if (!box.Containing_Box_Explicit_Width)
        {
            return CssValue.Zero;
        }
        else
        {
            var resolved = Percent * CssCommon.Get_Logical_Width(Property.Owner.Style.WritingMode, box.Containing_Box);
            return CssValue.From_Dimension(resolved, ECssUnit.PX);
        }
    }

    /// <summary>
    /// Resolves a percentage against the containing block's logical height.
    /// Used for: height, top, bottom.
    /// Note: Per CSS 2.1, vertical margin/padding percentages resolve against WIDTH, not height.
    /// </summary>
    /// <remarks>
    /// If a layout cycle is detected (ancestor being resolved), returns zero to break the cycle.
    /// Per CSS 2.2 §10.5: "If the height of the containing block is not specified explicitly
    /// (i.e., it depends on content height), and this element is not absolutely positioned,
    /// the value computes to 'auto'."
    /// </remarks>
    public static CssValue Containing_Block_Logical_Height(ICssProperty Property, double Percent)
    {
        // Validate that the box exists - percentage resolution only happens during layout
        // when boxes are guaranteed to exist. If box is null, it indicates a pipeline bug.
        var box = Property.Owner.Box ?? throw new InvalidOperationException(
            $"Cannot resolve percentage for property '{Property.CssName}' on element " +
            $"<{Property.Owner.nodeName ?? "unknown"}>: Box is null. " +
            $"Percentage resolution should only occur during layout phase when boxes exist.");

        // Check for layout cycle
        if (box is CssPrincipalBox principalBox)
        {
            var tracker = LayoutCycleTracker.Current;
            var resolvingAncestor = tracker.FindResolvingAncestor(principalBox);
            if (resolvingAncestor is not null)
            {
                Log.Warn($"[LayoutCycle] Height percentage cycle detected: " +
                    $"<{principalBox.Owner?.nodeName ?? "unknown"}> depends on ancestor " +
                    $"<{resolvingAncestor.Owner?.nodeName ?? "unknown"}> which is being resolved. " +
                    $"Treating as zero.");
                return CssValue.Zero;
            }
        }

        if (!box.Containing_Box_Explicit_Height)
        {
            return CssValue.Zero;
        }
        else
        {
            var resolved = Percent * CssCommon.Get_Logical_Height(Property.Owner.Style.WritingMode, box.Containing_Box);
            return CssValue.From_Dimension(resolved, ECssUnit.PX);
        }
    }
}

