﻿using System;
using CssUI.Internal;
using CssUI.CSS.Internal;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class CssLookup
    {
        #region Enum Lookup

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
        public static string[] Get_Keywords(Type enumType)
        {
            int index = CssEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return CssEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }
        #endregion
    }
}
