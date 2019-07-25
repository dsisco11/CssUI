﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class DomLookup
    {
        #region Keywords
        /// <summary>
        /// Attempts to retrieve the keyword value for the specified enum value
        /// </summary>
        /// <typeparam name="Ty">Enum for which the keyword is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <param name="outKeyword">Returned value</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool TryKeyword<Ty>(Ty Value, out string outKeyword) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outKeyword = null;
                return false;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            outKeyword = DomEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
            return true;
        }


        /// <summary>
        /// Retrieves the keyword for the specified enum value.
        /// </summary>
        /// <typeparam name="Ty">Enum for which the keyword is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <returns>Keyword</returns>
        /// <exception cref="CssException">If the keyword cannot be found</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string Keyword<Ty>(Ty Value) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index > -1)
            {
                /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                string keyword = DomEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
                if (!ReferenceEquals(null, keyword))
                {
                    return keyword;
                }
            }

            throw new DOMException($"Unable to find keyword for enum value {System.Enum.GetName(typeof(Ty), Value)} in DOM enum table");
        }
        #endregion

        #region Enums
        /// <summary>
        /// Attempts to retrieve an enum value from a given keyword
        /// </summary>
        /// <typeparam name="Ty">The enum type to return</typeparam>
        /// <param name="Keyword">Keyword to lookup enum value for</param>
        /// <param name="outEnum">Returned enum value</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool TryEnum<Ty>(AtomicString Keyword, out Ty outEnum) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outEnum = default(Ty);
                return false;
            }

            outEnum = (Ty)DomEnumTables.KEYWORD[index][Keyword];
            return true;
        }


        /// <summary>
        /// Attempts to retrieve an enum value from a given keyword
        /// </summary>
        /// <param name="EnumName">Name of the enum to search</param>
        /// <param name="Keyword">Keyword to lookup enum value for</param>
        /// <param name="outEnum">Returned enum value</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool TryEnum(string EnumName, AtomicString Keyword, out dynamic outEnum)
        {
            int index = DomEnumTables.Lookup_Enum_Index(EnumName);
            if (index < 0)
            {
                /* Enum has no index */
                outEnum = 0;
                return false;
            }

            outEnum = DomEnumTables.KEYWORD[index][Keyword];
            return true;
        }


        /// <summary>
        /// Retrieves an enum value from a given keyword
        /// </summary>
        /// <typeparam name="Ty">The enum type to return</typeparam>
        /// <param name="Keyword">Keyword to lookup enum value for</param>
        /// <returns>Enum value</returns>
        /// <exception cref="CssException">Throws if the keyword does not exist in the lookup table</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static Ty Enum<Ty>(AtomicString Keyword) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index > -1)
            {
                if (DomEnumTables.KEYWORD[index].TryGetValue(Keyword, out dynamic outEnum))
                {
                    return (Ty)outEnum;
                }
            }

            throw new CssException($"Unable to find keyword for enum value {Keyword} in CSS enum table");
        }

        /// <summary>
        /// Retrieves an enum value from a given keyword
        /// </summary>
        /// <param name="EnumName">Name of the enum to search</param>
        /// <param name="Keyword">Keyword to lookup enum value for</param>
        /// <returns>Enum value</returns>
        /// <exception cref="CssException">Throws if the keyword does not exist in the lookup table</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static dynamic Enum(string EnumName, AtomicString Keyword)
        {
            int index = DomEnumTables.Lookup_Enum_Index(EnumName);
            if (index > -1)
            {
                if (DomEnumTables.KEYWORD[index].TryGetValue(Keyword, out dynamic outEnum))
                {
                    return outEnum;
                }
            }

            throw new CssException($"Unable to find keyword for enum value {Keyword} in CSS enum table");
        }

        #endregion



        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Is_Declared(Type enumType, AtomicString Keyword)
        {
            int index = DomEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return false;/* Enum has no index */

            return DomEnumTables.KEYWORD[index]?.ContainsKey(Keyword) ?? false;
        }


        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords(Type enumType)
        {
            int index = DomEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return DomEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }

        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords<Ty>()
        {
            int index = DomEnumTables.Lookup_Enum_Index(typeof(Ty).Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return DomEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }
        #endregion
    }
}
