using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
/// <para>
/// This class supports both EnumRecords 0.5+ static record classes ({EnumName}Record)
/// and the extension method pattern ({EnumName}Extensions). The Record class pattern
/// is preferred when available as it provides a cleaner API for static access.
/// </para>
/// <para>
/// For direct access without reflection, prefer using generated methods directly:
/// </para>
/// <list type="bullet">
/// <item><c>enumValue.Keyword()</c> — Extension method to get keyword for an enum value</item>
/// <item><c>{EnumName}Record.GetKeyword(value)</c> — Static method (EnumRecords 0.5+)</item>
/// <item><c>{EnumName}Record.TryFromKeyword(keyword, out result)</c> — Reverse lookup (EnumRecords 0.5+)</item>
/// <item><c>{EnumName}Record.GetKeywords()</c> — Get all keywords (EnumRecords 0.5+)</item>
/// </list>
/// <para>
/// The methods in this class use reflection and are retained for scenarios requiring
/// runtime type dispatch (e.g., when the enum type is not known at compile time).
/// </para>
/// </remarks>
public static class Lookup
{
    #region Caching
    /// <summary>
    /// Cache for extension type lookups to avoid repeated reflection.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Type?> _extensionsTypeCache = new();

    /// <summary>
    /// Cache for static record class type lookups (EnumRecords 0.5+ pattern: {EnumName}Record).
    /// </summary>
    private static readonly ConcurrentDictionary<Type, Type?> _recordTypeCache = new();

    /// <summary>
    /// Cache for Keyword method lookups (extension method pattern).
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _keywordMethodCache = new();

    /// <summary>
    /// Cache for GetKeyword method lookups (static record class pattern).
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _getKeywordMethodCache = new();

    /// <summary>
    /// Cache for TryFromKeyword method lookups.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _tryFromKeywordMethodCache = new();

    /// <summary>
    /// Cache for GetKeywords method lookups.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo?> _getKeywordsMethodCache = new();

    /// <summary>
    /// Namespaces to search for generated classes.
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
    /// Finds the generated extensions type for an enum ({EnumName}Extensions).
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
    /// Finds the generated static record class for an enum ({EnumName}Record).
    /// EnumRecords 0.5+ generates this class with direct static methods.
    /// </summary>
    private static Type? GetRecordType(Type enumType)
    {
        return _recordTypeCache.GetOrAdd(enumType, static et =>
        {
            var recordClassName = $"{et.Name}Record";
            var assembly = et.Assembly;

            // Try the enum's own namespace first
            var candidate = assembly.GetType($"{et.Namespace}.{recordClassName}");
            if (candidate is not null) return candidate;

            // Search other namespaces
            foreach (var ns in _searchNamespaces)
            {
                var fullName = string.IsNullOrEmpty(ns) ? recordClassName : $"{ns}.{recordClassName}";
                candidate = assembly.GetType(fullName);
                if (candidate is not null) return candidate;
            }

            return null;
        });
    }

    /// <summary>
    /// Gets the GetKeyword static method from the Record class (EnumRecords 0.5+ pattern).
    /// Signature: public static string GetKeyword(TEnum value)
    /// </summary>
    private static MethodInfo? GetGetKeywordMethod(Type enumType)
    {
        return _getKeywordMethodCache.GetOrAdd(enumType, static et =>
        {
            var recordType = GetRecordType(et);
            return recordType?.GetMethod("GetKeyword", [et]);
        });
    }

    /// <summary>
    /// Gets the Keyword extension method for an enum type (legacy pattern).
    /// Signature: public static string Keyword(this TEnum value)
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
    /// Gets the TryFromKeyword method for an enum type.
    /// Prefers {EnumName}Record.TryFromKeyword() over {EnumName}Extensions.TryFromKeyword().
    /// EnumRecords 0.5+ generates: TryFromKeyword(string value, out T? result)
    /// </summary>
    private static MethodInfo? GetTryFromKeywordMethod(Type enumType)
    {
        return _tryFromKeywordMethodCache.GetOrAdd(enumType, static et =>
        {
            // EnumRecords 0.5+ uses nullable out parameter: out T? (Nullable<T>)
            var nullableType = typeof(Nullable<>).MakeGenericType(et);
            var paramTypes = new[] { typeof(string), nullableType.MakeByRefType() };

            // Try Record class first (preferred in 0.5+)
            var recordType = GetRecordType(et);
            var method = recordType?.GetMethod("TryFromKeyword", paramTypes);
            if (method is not null) return method;

            // Fallback to Extensions class
            var extensionsType = GetExtensionsType(et);
            return extensionsType?.GetMethod("TryFromKeyword", paramTypes);
        });
    }

    /// <summary>
    /// Gets the GetKeywords method for an enum type.
    /// Prefers {EnumName}Record.GetKeywords() over {EnumName}Extensions.GetKeywords().
    /// </summary>
    private static MethodInfo? GetGetKeywordsMethod(Type enumType)
    {
        return _getKeywordsMethodCache.GetOrAdd(enumType, static et =>
        {
            // Try Record class first (preferred in 0.5+)
            var recordType = GetRecordType(et);
            var method = recordType?.GetMethod("GetKeywords", Type.EmptyTypes);
            if (method is not null) return method;

            // Fallback to Extensions class
            var extensionsType = GetExtensionsType(et);
            return extensionsType?.GetMethod("GetKeywords", Type.EmptyTypes);
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

        // Try Record class GetKeyword() first (EnumRecords 0.5+ pattern)
        var getKeywordMethod = GetGetKeywordMethod(enumType);
        if (getKeywordMethod is not null)
        {
            try
            {
                outKeyword = getKeywordMethod.Invoke(null, [Value])?.ToString();
                return outKeyword is not null;
            }
            catch
            {
                // Fall through to extension method pattern
            }
        }

        // Fallback to extension method Keyword() pattern
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

        // Try Record class GetKeyword() first (EnumRecords 0.5+ pattern)
        var getKeywordMethod = GetGetKeywordMethod(enumType);
        if (getKeywordMethod is not null)
        {
            try
            {
                var result = getKeywordMethod.Invoke(null, [Value])?.ToString();
                if (result is not null)
                {
                    return result;
                }
            }
            catch
            {
                // Fall through to extension method pattern
            }
        }

        // Fallback to extension method Keyword() pattern
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
        return GetRecordType(typeof(T)) is not null || GetExtensionsType(typeof(T)) is not null;
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

        return GetRecordType(enumType) is not null || GetExtensionsType(enumType) is not null;
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
    /// This method calls the EnumRecords-generated GetKeywords() extension method.
    /// For hot paths, consider caching the result or using the generated extension methods directly.
    /// Falls back to iterating over enum values if GetKeywords() is not available.
    /// </remarks>
    /// <param name="enumType">The enum type</param>
    /// <returns>Array of keyword strings</returns>
    public static string[] Get_Keywords(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        // Try to use the EnumRecords-generated GetKeywords() method (0.4+)
        var getKeywordsMethod = GetGetKeywordsMethod(enumType);
        if (getKeywordsMethod is not null)
        {
            try
            {
                var result = getKeywordsMethod.Invoke(null, null);
                if (result is IReadOnlyList<string> keywordList)
                {
                    return [.. keywordList];
                }
            }
            catch
            {
                // Fall through to fallback implementation
            }
        }

        // Fallback: iterate over enum values and call Keyword() for each
        var keywordMethod = GetKeywordMethod(enumType);
        if (keywordMethod is null)
            return [];

        // Get all enum values and map to keywords
        var enumValues = System.Enum.GetValues(enumType);
        var keywords = new List<string>(enumValues.Length);

        for (int i = 0; i < enumValues.Length; i++)
        {
            try
            {
                var result = keywordMethod.Invoke(null, [enumValues.GetValue(i)]);
                if (result is string keyword && !string.IsNullOrEmpty(keyword))
                {
                    keywords.Add(keyword);
                }
            }
            catch
            {
                // Skip enum values that don't have a keyword defined (e.g., [Ignore] members)
            }
        }

        return [.. keywords];
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

