using System.Runtime.CompilerServices;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Media;
using CssUI.DOM;
using CssUI.HTTP;

namespace CssUI;

/// <summary>
/// Helper extensions for migrating from AtomicName patterns.
/// These bridge the gap during migration and can be removed after completion.
/// </summary>
public static class AtomicNameMigrationHelpers
{
    /// <summary>
    /// Attempts to parse a string as a known attribute name.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseAttribute(this string name, out EAttributeName result)
    {
        if (EAttributeNameExtensions.TryFromKeyword(name, out EAttributeName? nullable))
        {
            result = nullable.Value;
            return true;
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Attempts to parse a string as a known CSS property.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseCssProperty(this string name, out ECssPropertyID result)
    {
        if (ECssPropertyIDExtensions.TryFromKeyword(name, out ECssPropertyID? nullable))
        {
            result = nullable.Value;
            return true;
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Attempts to parse a string as a known URL scheme.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUrlScheme(this string name, out EUrlScheme result)
    {
        if (EUrlSchemeExtensions.TryFromKeyword(name, out EUrlScheme? nullable))
        {
            result = nullable.Value;
            return true;
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Attempts to parse a string as a known media feature name.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseMediaFeature(this string name, out EMediaFeatureName result)
    {
        if (EMediaFeatureNameExtensions.TryFromKeyword(name, out EMediaFeatureName? nullable))
        {
            result = nullable.Value;
            return true;
        }
        result = default;
        return false;
    }
}
