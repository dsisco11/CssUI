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

        public static string Enum<Ty>(Ty Value) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            return DomEnumTables.TABLE[index][Convert.ToInt32(Value)];
        }

        public static Ty FromKeyword<Ty>(string Keyword) where Ty : struct
        {
            int index = DomEnumTables.Get_Enum_Index<Ty>();
            return (Ty)DomEnumTables.KEYWORD[index][new AtomicString(Keyword)];
        }

        #endregion
    }
}
