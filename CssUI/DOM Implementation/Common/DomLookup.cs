using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.DOM.Internal;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class DomLookup
    {
        #region Enum Lookup

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Keyword_From_Enum<Ty>(Ty Value, out string outKeyword) where Ty : struct
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


        [Obsolete("Encourages bad code, instead use: Keyword_From_Enum<Ty>(Ty Value, out string outKeyword)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string Keyword_From_Enum<Ty>(Ty Value) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                return null;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            return DomEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Enum_From_Keyword<Ty>(AtomicString Keyword, out Ty outEnum) where Ty : struct
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

        [Obsolete("Encourages bad code, instead use: Enum_From_Keyword<Ty>(string, out Ty outEnum)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static Ty? Enum_From_Keyword<Ty>(AtomicString Keyword) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                return null;
            }

            return (Ty)DomEnumTables.KEYWORD[index][Keyword];
        }

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
