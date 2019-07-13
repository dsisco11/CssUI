using System;
using CssUI.Internal;
using CssUI.DOM.Internal;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class DomLookup
    {
        #region Enum Lookup

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

        public static bool Is_Declared(Type enumType, AtomicString Keyword)
        {
            int index = DomEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return false;/* Enum has no index */

            return DomEnumTables.KEYWORD[index]?.ContainsKey(Keyword) ?? false;
        }

        #endregion
    }
}
