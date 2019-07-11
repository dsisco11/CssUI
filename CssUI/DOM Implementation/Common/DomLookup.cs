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
                return null;/* Enum has no index */

            return DomEnumTables.TABLE[index][Convert.ToInt32(Value)];
        }

        public static Ty? Enum_From_Keyword<Ty>(AtomicString Keyword) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
                return null;/* Enum has no index */

            return (Ty)DomEnumTables.KEYWORD[index][Keyword];
        }

        #endregion
    }
}
