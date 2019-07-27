using System;
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
        /// <typeparam name="Ty">Enum for which the keyword is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <param name="outKeyword">Returned value</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool TryKeyword<Ty>(Ty Value, out string outKeyword) where Ty : struct
        {
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outKeyword = null;
                return false;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            outKeyword = CSSUIEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)].Keyword;
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
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index > -1)
            {
                /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                string keyword = CSSUIEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)].Keyword;
                if (!ReferenceEquals(null, keyword))
                {
                    return keyword;
                }
            }

            throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(typeof(Ty), Value)} in CssUI enum table");
        }
        #endregion

        #region Data
        /// <summary>
        /// Attempts to retrieve the metadata value for the specified enum value
        /// </summary>
        /// <typeparam name="Ty">Enum for which the metadata is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <param name="outData">Returned value</param>
        /// <returns>Success</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool TryData<Ty>(Ty Value, out EnumData outData) where Ty : struct
        {
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outData = null;
                return false;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            outData = CSSUIEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
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
        public static bool TryData(Type enumType, dynamic Value, out EnumData outData)
        {
            int index = CSSUIEnumTables.Get_Enum_Index(enumType.TypeHandle);
            if (index < 0)
            {
                /* Enum has no index */
                outData = null;
                return false;
            }

            /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
            outData = CSSUIEnumTables.TABLE[index][CastTo<int>.From(Value)];
            return true;
        }


        /// <summary>
        /// Retrieves the metadata for the specified enum value.
        /// </summary>
        /// <typeparam name="Ty">Enum for which the metadata is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <returns>Enum metadata</returns>
        /// <exception cref="CssException">If the keyword cannot be found</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static EnumData Data<Ty>(Ty Value) where Ty : struct
        {
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index > -1)
            {
                /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                var dataLookup = CSSUIEnumTables.TABLE[index][CastTo<int>.From<Ty>(Value)];
                if (!ReferenceEquals(null, dataLookup))
                {
                    return dataLookup;
                }
            }

            throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(typeof(Ty), Value)} in CssUI enum table");
        }


        /// <summary>
        /// Retrieves the metadata for the specified enum value.
        /// </summary>
        /// <typeparam name="Ty">Enum for which the metadata is listed</typeparam>
        /// <param name="Value">Enum value to lookup</param>
        /// <returns>Enum metadata</returns>
        /// <exception cref="CssException">If the keyword cannot be found</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static EnumData Data(Type enumType, dynamic Value)
        {
            int index = CSSUIEnumTables.Get_Enum_Index(enumType.TypeHandle);
            if (index > -1)
            {
                /* /!\ This conversion will fucking EXPLODE if the given generic type does not have an integer backing type /!\ */
                var dataLookup = CSSUIEnumTables.TABLE[index][CastTo<int>.From(Value)];
                if (!ReferenceEquals(null, dataLookup))
                {
                    return dataLookup;
                }
            }

            throw new Exception($"Unable to find keyword for enum value {System.Enum.GetName(enumType, Value)} in CssUI enum table");
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
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index < 0)
            {
                /* Enum has no index */
                outEnum = default(Ty);
                return false;
            }

            outEnum = (Ty)CSSUIEnumTables.KEYWORD[index][Keyword];
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
        public static bool TryEnum(Type enumType, AtomicString Keyword, out dynamic outEnum)
        {
            int index = CSSUIEnumTables.Get_Enum_Index(enumType.TypeHandle);
            if (index < 0)
            {
                /* Enum has no index */
                outEnum = null;
                return false;
            }

            outEnum = CSSUIEnumTables.KEYWORD[index][Keyword];
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
            int index = CSSUIEnumTables.Get_Enum_Index<Ty>();
            if (index > -1)
            {
                if (CSSUIEnumTables.KEYWORD[index].TryGetValue(Keyword, out dynamic outEnum))
                {
                    return (Ty)outEnum;
                }
            }

            throw new Exception($"Unable to find keyword for enum value {Keyword} in CssUI enum table");
        }

        #endregion


        #region Checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Is_Declared(Type enumType, AtomicString Keyword)
        {
            int index = CSSUIEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return false;/* Enum has no index */

            return CSSUIEnumTables.KEYWORD[index]?.ContainsKey(Keyword) ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static bool Is_Declared<Ty>(AtomicString Keyword) where Ty : struct
        {
            if (!CSSUIEnumTables.Lookup_Index<Ty>(out int outIndex))
                return false;/* Enum has no index */

            return CSSUIEnumTables.KEYWORD[outIndex]?.ContainsKey(Keyword) ?? false;
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
            int index = CSSUIEnumTables.Lookup_Enum_Index(enumType.Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return CSSUIEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }

        /// <summary>
        /// Returns ALL keywords defined for the given enum
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Small function which is called frequently in loops, inline it
        public static string[] Get_Keywords<Ty>()
        {
            int index = CSSUIEnumTables.Lookup_Enum_Index(typeof(Ty).Name);
            if (index < 0)
                return new string[0];/* Enum has no index */

            return CSSUIEnumTables.KEYWORD[index].Keys.Cast<string>().ToArray();
        }
        #endregion
    }
}
