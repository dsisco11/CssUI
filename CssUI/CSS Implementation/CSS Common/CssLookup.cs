using System;
using CssUI.CSS.Internal;
using System.Linq;
using CssUI.CSS.Media;
using System.Runtime.CompilerServices;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class CssLookup
    {
        #region Definition Lookups
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static MediaDefinition Lookup_Def(AtomicName<EMediaFeatureName> Name)
        {
            if (CssDefinitions.MediaDefinitions.TryGetValue(Name, out MediaDefinition def))
            {
                return def;
            }

            return null;
        }

        /*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static StyleDefinition Lookup_Def(AtomicName<EMediaFeatureName> Name)
        {
            if (CssDefinitions.StyleDefinitions.TryGetValue(Name, out StyleDefinition def))
            {
                return def;
            }

            return null;
        }
        */
        #endregion

        #region Enum Lookup


        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Keyword_From_Enum<Ty>(Ty Value, out string outKeyword) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outKeyword = null;
                return false;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            outKeyword = CssEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
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
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                throw new CssException($"Unable to find keyword for enum value {Enum.GetName(typeof(Ty), Value)} in CSS enum table");
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            string keyword = CssEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
            if (ReferenceEquals(null, keyword))
            {
                throw new CssException($"Unable to find keyword for enum value {Enum.GetName(typeof(Ty), Value)} in CSS enum table");
            }

            return keyword;
        }

        [Obsolete("Encourages bad code, instead use: Keyword_From_Enum<Ty>(Ty Value, out string outKeyword)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string Keyword_From_Enum<Ty>(Ty Value) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                return null;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            return CssEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Enum_From_Keyword<Ty>(AtomicString Keyword, out Ty outEnum) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outEnum = default(Ty);
                return false;
            }

            outEnum = (Ty)CssEnumTables.KEYWORD[index][Keyword];
            return true;
        }

        [Obsolete("Encourages bad code, instead use: Enum_From_Keyword<Ty>(string, out Ty outEnum)")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static Ty? Enum_From_Keyword<Ty>(AtomicString Keyword) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                return null;
            }
            return (Ty)CssEnumTables.KEYWORD[index][Keyword];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Is_Declared(Type enumType, AtomicString Keyword)
        {
            int index = CssEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return false;/* Enum has no index */

            return CssEnumTables.KEYWORD[index]?.ContainsKey(Keyword) ?? false;
        }

        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords(Type enumType)
        {
            int index = CssEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return CssEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }

        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords<Ty>()
        {
            int index = CssEnumTables.Lookup_Enum_Index(typeof(Ty).Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return CssEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }
        #endregion
    }
}
