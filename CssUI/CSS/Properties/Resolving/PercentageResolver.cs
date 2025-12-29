using System;

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
    public static CssValue Containing_Block_Logical_Width(ICssProperty Property, double Percent)
    {
        if (!Property.Owner.Box.Containing_Box_Explicit_Width)
        {
            return CssValue.Zero;
        }
        else
        {
            var resolved = Percent * CssCommon.Get_Logical_Width(Property.Owner.Style.WritingMode, Property.Owner.Box.Containing_Box);
            return CssValue.From_Dimension(resolved, ECssUnit.PX);
        }
    }

    /// <summary>
    /// Resolves a percentage against the containing block's logical height.
    /// Used for: height, top, bottom.
    /// Note: Per CSS 2.1, vertical margin/padding percentages resolve against WIDTH, not height.
    /// </summary>
    public static CssValue Containing_Block_Logical_Height(ICssProperty Property, double Percent)
    {
        if (!Property.Owner.Box.Containing_Box_Explicit_Height)
        {
            return CssValue.Zero;
        }
        else
        {
            var resolved = Percent * CssCommon.Get_Logical_Height(Property.Owner.Style.WritingMode, Property.Owner.Box.Containing_Box);
            return CssValue.From_Dimension(resolved, ECssUnit.PX);
        }
    }
}

