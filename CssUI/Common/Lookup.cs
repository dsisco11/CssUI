using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
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
    #region Keywords
    /// <summary>
    /// Attempts to retrieve the keyword value for the specified enum value
    /// </summary>
    /// <typeparam name="T">Enum for which the keyword is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outKeyword">Returned value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryKeyword<T>(T Value, [MaybeNullWhen(false)][NotNullWhen(true)] out string? outKeyword) where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outKeyword = null;
            return false;
        }

        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        //outKeyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
        outKeyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
        return true;
    }

    /// <summary>
    /// Attempts to retrieve the keyword value for the specified enum value
    /// </summary>
    /// <typeparam name="T">Enum for which the keyword is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outKeyword">Returned value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryKeyword(Type enumType, object Value, [MaybeNullWhen(false)][NotNullWhen(true)] out string? outKeyword)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        ArgumentNullException.ThrowIfNull(Value);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outKeyword = null;
            return false;
        }

        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        //outKeyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
        outKeyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
        return true;
    }


    /// <summary>
    /// Retrieves the keyword for the specified enum value.
    /// </summary>
    /// <typeparam name="T">Enum for which the keyword is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Keyword</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static string Keyword<T>(T Value) where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex > -1)
        {
            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            string keyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
            if (keyword is not null)
            {
                return keyword;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(typeof(T), Value)} in meta-enum table");
    }

    /// <summary>
    /// Retrieves the keyword for the specified enum value.
    /// </summary>
    /// <param name="enumType">Enum for which the keyword is listed</param>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Keyword</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static string Keyword(Type enumType, object Value)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex > -1)
        {
            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            string keyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
            if (keyword is not null)
            {
                return keyword;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(enumType, Value)} in meta-enum table");
    }
    #endregion

    #region Data
    /// <summary>
    /// Attempts to retrieve the metadata value for the specified enum value
    /// </summary>
    /// <typeparam name="T">Enum for which the metadata is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outData">Returned value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryData<T>(T Value, [MaybeNullWhen(false)][NotNullWhen(true)] out EnumData? outData) where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outData = null;
            return false;
        }

        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        outData = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value));
        return true;
    }

    /// <summary>
    /// Attempts to retrieve the metadata value for the specified enum value
    /// </summary>
    /// <typeparam name="Ty">Enum for which the metadata is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <param name="outData">Returned value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryData(Type enumType, object Value, out EnumData? outData)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outData = null;
            return false;
        }

        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        outData = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value));
        return true;
    }


    /// <summary>
    /// Retrieves the metadata for the specified enum value.
    /// </summary>
    /// <typeparam name="T">Enum for which the metadata is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Enum metadata</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static EnumData Data<T>(T Value) where T : struct
    {
        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        int vIdx = CastTo<int>.From(Value);
        if (vIdx < 0) throw new IndexOutOfRangeException();
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex > -1)
        {
            if (vIdx > 0 && vIdx < EnumMetaTable.Count(enumIndex))
            {
                var dataLookup = EnumMetaTable.Get(enumIndex, vIdx);
                return dataLookup;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(typeof(T), Value)} in meta-enum table");
    }


    /// <summary>
    /// Retrieves the metadata for the specified enum value.
    /// </summary>
    /// <typeparam name="Ty">Enum for which the metadata is listed</typeparam>
    /// <param name="Value">Enum value to lookup</param>
    /// <returns>Enum metadata</returns>
    /// <exception cref="Exception">If the keyword cannot be found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static EnumData Data(Type enumType, object Value)
    {
        /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
        ArgumentNullException.ThrowIfNull(enumType);
        int vIdx = CastTo<int>.From(Value);
        if (vIdx < 0) throw new IndexOutOfRangeException();
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex > -1)
        {
            if (vIdx > 0 && vIdx < EnumMetaTable.Count(enumIndex))
            {
                var dataLookup = EnumMetaTable.Get(enumIndex, vIdx);
                return dataLookup;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(enumType, Value)} in meta-enum table");
    }
    #endregion

    #region Enums
    /// <summary>
    /// Attempts to retrieve an enum value from a given keyword
    /// </summary>
    /// <typeparam name="T">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <param name="outEnum">Returned enum value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryEnum<T>(AtomicString Keyword, out T outEnum) where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outEnum = default;
            return false;
        }

        if (!EnumMetaTable.KEYWORD[enumIndex].TryGetValue(Keyword, out object? outValue))
        {
            outEnum = default;
            return false;
        }

        outEnum = CastTo<T>.From(outValue!);
        return true;
    }

    /// <summary>
    /// Attempts to retrieve an enum value from a given keyword
    /// </summary>
    /// <typeparam name="Ty">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <param name="outEnum">Returned enum value</param>
    /// <returns>Success</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool TryEnum(Type enumType, AtomicString Keyword, out object? outEnum)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex < 0)
        {
            /* Enum has no index */
            outEnum = null;
            return false;
        }


        if (!EnumMetaTable.KEYWORD[enumIndex].TryGetValue(Keyword, out object? outValue))
        {
            outEnum = default;
            return false;
        }

        outEnum = outValue;
        return true;
    }


    /// <summary>
    /// Retrieves an enum value from a given keyword
    /// </summary>
    /// <typeparam name="T">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <returns>Enum value</returns>
    /// <exception cref="Exception">Throws if the keyword does not exist in the lookup table</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static T Enum<T>(AtomicString Keyword) where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        if (enumIndex > -1)
        {
            if (EnumMetaTable.KEYWORD[enumIndex].TryGetValue(Keyword, out var outValue))
            {
                return (T)outValue;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {Keyword} in meta-enum table");
    }

    /// <summary>
    /// Retrieves an enum value from a given keyword
    /// </summary>
    /// <typeparam name="T">The enum type to return</typeparam>
    /// <param name="Keyword">Keyword to lookup enum value for</param>
    /// <returns>Enum value</returns>
    /// <exception cref="Exception">Throws if the keyword does not exist in the lookup table</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static object Enum(Type enumType, AtomicString Keyword)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex > -1)
        {
            if (EnumMetaTable.KEYWORD[enumIndex].TryGetValue(Keyword, out var outValue))
            {
                return outValue;
            }
        }

        throw new Exception($"Unable to find keyword for enum value {Keyword} in meta-enum table");
    }

    #endregion

    #region Checks
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool Is_Declared<T>() where T : struct
    {
        int enumIndex = EnumMetaTable.Meta.Lookup<T>();
        return (enumIndex > 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool Is_Declared(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        return (enumIndex > 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool Is_Declared(Type enumType, AtomicString Keyword)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        Contract.EndContractBlock();

        int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
        if (enumIndex < 0)
            return false;/* Enum has no index */

        return EnumMetaTable.KEYWORD[enumIndex]?.ContainsKey(Keyword) ?? false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
    public static bool Is_Declared<T>(AtomicString Keyword) where T : struct
    {
        if (!EnumMetaTable.Meta.Lookup<T>(out int outIndex))
            return false;/* Enum has no index */

        return EnumMetaTable.KEYWORD[outIndex]?.ContainsKey(Keyword) ?? false;
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

