using CssUI.Internal;
using System;

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
            return CssEnumTables.TABLE[index][Convert.ToInt32(Value)];
        }

        public static Ty FromKeyword<Ty>(string Keyword) where Ty : struct
        {
            int index = CssEnumTables.Get_Enum_Index<Ty>();
            return (Ty)CssEnumTables.KEYWORD[index][new AtomicString(Keyword)];
        }

        #endregion
    }
}
