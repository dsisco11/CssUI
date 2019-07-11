using System;
using CssUI.Internal;
using CssUI.CSS.Internal;

namespace CssUI
{
    /// <summary>
    /// Provides utility functions for translating code values into their CSS string values
    /// </summary>
    public static partial class CssLookup
    {
        #region Enum Lookup

        public static string Enum<Ty>(Ty Value) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
                return null;/* Enum has no index */
            return CssEnumTables.TABLE[index][Convert.ToInt32(Value)];
        }

        public static Ty? FromKeyword<Ty>(AtomicString Keyword) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
                return null;/* Enum has no index */
            return (Ty)CssEnumTables.KEYWORD[index][Keyword];
        }

        #endregion
    }
}
