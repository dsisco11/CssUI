using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CssUI;

/// <summary>
/// Provides utility functions for looking up enum keywords and metadata.
/// </summary>
/// <remarks>
/// After migration to EnumRecords, prefer using the generated extension methods directly:
/// <list type="bullet">
/// <item><c>enumValue.Keyword()</c> — Get keyword for an enum value</item>
/// <item><c>TExtensions.TryFromKeyword(keyword, out result)</c> — Parse keyword to enum</item>
/// </list>
/// The methods in this class are retained for backward compatibility during migration.
/// </remarks>
public static class Lookup
{
    #region Caching
    /// <summary>
    /// Cache for extension type lookups to avoid repeated reflection.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Type?> _extensionsTypeCache = new();

    /// <summary>
    /// Cache for Keyword method lookups.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _keywordMethodCache = new();

    /// <summary>
    /// Cache for TryFromKeyword method lookups.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _tryFromKeywordMethodCache = new();

    /// <summary>
    /// Namespaces to search for generated extension classes.
    /// </summary>
    private static readonly string[] _searchNamespaces =
    [
        "", // Same namespace as enum
        "CssUI",
        "CssUI.CSS",
        "CssUI.CSS.Media",
        "CssUI.CSS.Enums",
        "CssUI.CSS.Internal",
        "CssUI.DOM",
        "CssUI.DOM.Enums",
        "CssUI.HTTP",
        "CssUI.HTML"
    ];

    /// <summary>
    /// Finds the generated extensions type for an enum.
    /// </summary>
    private static Type? GetExtensionsType(Type enumType)
    {
        return _extensionsTypeCache.GetOrAdd(enumType, static et =>
        {
            var extensionsClassName = $"{et.Name}Extensions";
            var assembly = et.Assembly;

            // Try the enum's own namespace first
            var candidate = assembly.GetType($"{et.Namespace}.{extensionsClassName}");
            if (candidate is not null) return candidate;

            // Search other namespaces
            foreach (var ns in _searchNamespaces)
            {
                var fullName = string.IsNullOrEmpty(ns) ? extensionsClassName : $"{ns}.{extensionsClassName}";
                candidate = assembly.GetType(fullName);
                if (candidate is not null) return candidate;
            }

            return null;
        });
    }

    /// <summary>
    /// Gets the Keyword extension method for an enum type.
    /// </summary>
    private static MethodInfo? GetKeywordMethod(Type enumType)
    {
        return _keywordMethodCache.GetOrAdd(enumType, static et =>
        {
            var extensionsType = GetExtensionsType(et);
            return extensionsType?.GetMethod("Keyword", [et]);
        });
    }

    /// <summary>
    /// Gets the TryFromKeyword extension method for an enum type.
    /// </summary>
    private static MethodInfo? GetTryFromKeywordMethod(Type enumType)
    {
        return _tryFromKeywordMethodCache.GetOrAdd(enumType, static et =>
        {
            var extensionsType = GetExtensionsType(et);
            return extensionsType?.GetMethod("TryFromKeyword", [typeof(string), et.MakeByRefType()]);
        });
    }
    #endregion
    #region Keywords
    /// <summary>
    /// Attempts to retrieve the keyword value for the specified enum value.
    /// </summary>
    /// <typeparam name="T">Enum for which the keyword is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outKeyword">Returned value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryKeyword<T>(T Value, [MaybeNullWhen(false)][NotNullWhen(true)] out string? outKeyword) where T : struct
    {
        return TryKeyword(typeof(T), Value!, out outKeyword);
    }

    /// <summary>
    /// Attempts to retrieve the keyword value for the specified enum value.
    /// </summary>
    /// <param name="enumType">Enum type</param>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outKeyword">Returned value</param>
    /// <returns>Success</returns>
    public static bool TryKeyword(Type enumType, object Value, [MaybeNullWhen(false)][NotNullWhen(true)] out string? outKeyword)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        ArgumentNullException.ThrowIfNull(Value);
        Contract.EndContractBlock();

        var keywordMethod = GetKeywordMethod(enumType);
        if (keywordMethod is null)
        {
            outKeyword = null;
            return false;
        }

        try
        {
            outKeyword = keywordMethod.Invoke(null, [Value])?.ToString();
            return outKeyword is not null;
        }
        catch
        {
            outKeyword = null;
            return false;
        }
    }

    /// <summary>
    /// Retrieves the keyword for the specified enum value.
    /// </summary>
    /// <typeparam name="T">Enum for which the keyword is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Keyword</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Keyword<T>(T Value) where T : struct
    {
        return Keyword(typeof(T), Value!);
    }

    /// <summary>
    /// Retrieves the keyword for the specified enum value.
    /// </summary>
    /// <param name="enumType">Enum type</param>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Keyword</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    public static string Keyword(Type enumType, object Value)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        ArgumentNullException.ThrowIfNull(Value);
        Contract.EndContractBlock();

        var keywordMethod = GetKeywordMethod(enumType);
        if (keywordMethod is not null)
        {
            var result = keywordMethod.Invoke(null, [Value])?.ToString();
            if (result is not null)
            {
                return result;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(enumType, Value)} in EnumRecords extensions");
    }
    #endregion

    #region Enums
    /// <summary>
    /// Attempts to retrieve an enum value from a given keyword.
    /// </summary>
    /// <typeparam name="T">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <param name="outEnum">Returned enum value</param>
    /// <returns>Success</returns>
    public static bool TryEnum<T>(AtomicString Keyword, out T outEnum) where T : struct
    {
        if (TryEnum(typeof(T), Keyword, out var result) && result is T typedResult)
        {
            outEnum = typedResult;
            return true;
        }

        outEnum = default;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve an enum value from a given keyword.
    /// </summary>
    /// <param name="enumType">The enum type</param>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <param name="outEnum">Returned enum value</param>
    /// <returns>Success</returns>
    public static bool TryEnum(Type enumType, AtomicString Keyword, out object? outEnum)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        var tryFromKeywordMethod = GetTryFromKeywordMethod(enumType);
        if (tryFromKeywordMethod is null)
        {
            outEnum = null;
            return false;
        }

        // TryFromKeyword has signature: bool TryFromKeyword(string keyword, out T result)
        var parameters = new object?[] { Keyword.ToString(), null };
        var success = (bool?)tryFromKeywordMethod.Invoke(null, parameters) ?? false;

        if (success)
        {
            outEnum = parameters[1];
            return true;
        }

        outEnum = null;
        return false;
    }

    /// <summary>
    /// Retrieves an enum value from a given keyword.
    /// </summary>
    /// <typeparam name="T">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <returns>Enum value</returns>
    /// <exception cref="Exception">Throws if the keyword does not exist</exception>
    public static T Enum<T>(AtomicString Keyword) where T : struct
    {
        if (TryEnum<T>(Keyword, out var result))
        {
            return result;
        }

        throw new Exception($"Unable to find enum value for keyword '{Keyword}'");
    }

    /// <summary>
    /// Retrieves an enum value from a given keyword.
    /// </summary>
    /// <param name="enumType">The enum type</param>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <returns>Enum value</returns>
    /// <exception cref="Exception">Throws if the keyword does not exist</exception>
    public static object Enum(Type enumType, AtomicString Keyword)
    {
        if (TryEnum(enumType, Keyword, out var result) && result is not null)
        {
            return result;
        }

        throw new Exception($"Unable to find enum value for keyword '{Keyword}'");
    }
    #endregion

    #region Checks
    /// <summary>
    /// Checks if an enum type has EnumRecords extensions.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>True if the enum has generated extensions</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Declared<T>() where T : struct
    {
        return GetExtensionsType(typeof(T)) is not null;
    }

    /// <summary>
    /// Checks if an enum type has EnumRecords extensions.
    /// </summary>
    /// <param name="enumType">The enum type</param>
    /// <returns>True if the enum has generated extensions</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Declared(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        return GetExtensionsType(enumType) is not null;
    }

    /// <summary>
    /// Checks if a keyword is valid for the given enum type.
    /// </summary>
    /// <param name="enumType">The enum type</param>
    /// <param name="Keyword">The keyword to check</param>
    /// <returns>True if the keyword maps to an enum value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Declared(Type enumType, AtomicString Keyword)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        return TryEnum(enumType, Keyword, out _);
    }

    /// <summary>
    /// Checks if a keyword is valid for the given enum type.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="Keyword">The keyword to check</param>
    /// <returns>True if the keyword maps to an enum value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Declared<T>(AtomicString Keyword) where T : struct
    {
        return TryEnum<T>(Keyword, out _);
    }
    #endregion

    #region Fetches
    /// <summary>
    /// Returns ALL keywords defined for the given enum.
    /// </summary>
    /// <remarks>
    /// This method uses reflection to invoke the EnumRecords-generated Keyword() extension method.
    /// For hot paths, consider caching the result or using the generated extension methods directly.
    /// </remarks>
    /// <param name="enumType">The enum type</param>
    /// <returns>Array of keyword strings</returns>
    public static string[] Get_Keywords(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        // Find the generated extension class (e.g., EFlexDirectionExtensions)
        var extensionsClassName = $"{enumType.Name}Extensions";
        var extensionsType = enumType.Assembly.GetType($"{enumType.Namespace}.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.CSS.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.CSS.Media.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.CSS.Enums.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.CSS.Internal.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.DOM.{extensionsClassName}")
                          ?? enumType.Assembly.GetType($"CssUI.HTTP.{extensionsClassName}");

        if (extensionsType is null)
            return Array.Empty<string>();

        // Find the Keyword extension method
        var keywordMethod = extensionsType.GetMethod("Keyword", new[] { enumType });
        if (keywordMethod is null)
            return Array.Empty<string>();

        // Get all enum values and map to keywords
        var enumValues = System.Enum.GetValues(enumType);
        var keywords = new string[enumValues.Length];

        for (int i = 0; i < enumValues.Length; i++)
        {
            var result = keywordMethod.Invoke(null, new[] { enumValues.GetValue(i) });
            keywords[i] = result?.ToString() ?? string.Empty;
        }

        return keywords.Where(static k => !string.IsNullOrEmpty(k)).ToArray();
    }

    /// <summary>
    /// Returns ALL keywords defined for the given enum.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>Array of keyword strings</returns>
    /// <remarks>
    /// This method uses reflection to invoke the EnumRecords-generated Keyword() extension method.
    /// For hot paths, consider caching the result or using the generated extension methods directly.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] Get_Keywords<T>() where T : struct, System.Enum
    {
        return Get_Keywords(typeof(T));
    }
    #endregion
}

