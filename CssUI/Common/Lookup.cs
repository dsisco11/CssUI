using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
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
        public static bool TryKeyword<T>(T Value, out string outKeyword) where T : struct
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
        public static bool TryKeyword(Type enumType, object Value, out string outKeyword)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            if (Value is null) throw new ArgumentNullException(nameof(Value));
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
                if (keyword is object)
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
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            Contract.EndContractBlock();

            int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
            if (enumIndex > -1)
            {
                /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                string keyword = EnumMetaTable.Get(enumIndex, CastTo<int>.From(Value)).Keyword;
                if (keyword is object)
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
        public static bool TryData<T>(T Value, out EnumData outData) where T : struct
        {
            int enumIndex = EnumMetaTable.Meta.Lookup<T>();
            if (enumIndex < 0)
            {
                /* Enum has no index */
                outData = default;
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
        public static bool TryData(Type enumType, object Value, out EnumData outData)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            Contract.EndContractBlock();

            int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
            if (enumIndex < 0)
            {
                /* Enum has no index */
                outData = default;
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
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
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

            outEnum = (T)EnumMetaTable.KEYWORD[enumIndex][Keyword];
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
        public static bool TryEnum(Type enumType, AtomicString Keyword, out object outEnum)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            Contract.EndContractBlock();

            int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
            if (enumIndex < 0)
            {
                /* Enum has no index */
                outEnum = null;
                return false;
            }

            outEnum = EnumMetaTable.KEYWORD[enumIndex][Keyword];
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
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
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
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            Contract.EndContractBlock();

            int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
            return (enumIndex > 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Is_Declared(Type enumType, AtomicString Keyword)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
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
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords(Type enumType)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            Contract.EndContractBlock();

            int enumIndex = EnumMetaTable.Meta.Lookup(enumType.TypeHandle);
            if (enumIndex < 0)
                return Array.Empty<string>();/* Enum has no index */

            return EnumMetaTable.KEYWORD[enumIndex].Keys.Cast<string>().ToArray();
        }

        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords<T>()
        {
            int enumIndex = EnumMetaTable.Meta.Lookup<T>();
            if (enumIndex < 0)
                return Array.Empty<string>();/* Enum has no index */

            return EnumMetaTable.KEYWORD[enumIndex].Keys.Cast<string>().ToArray();
        }
        #endregion
    }
}
