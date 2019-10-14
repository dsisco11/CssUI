using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CssUI.Internal;
using CssUI.CORE;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CssUI
{
    /// <summary>
    /// Compiles (at runtime) a table of metadata for all values of any enums marked with the <see cref="MetaEnumAttribute"/>.
    /// Metadata is specified for each member of an enum using the <see cref="MetaKeywordAttribute"/>.
    /// </summary>
    internal static class EnumMetaTable
    {
        #region Lookup Tables
        public static readonly MetaTable<MetaEnumAttribute> Meta = new MetaTable<MetaEnumAttribute>(100);
        private static readonly EnumData[][] TABLE;
        /// <summary>
        /// Maps enum meta-keyword strings to their actual code constant
        /// </summary>
        public static readonly Dictionary<AtomicString, object>[] KEYWORD;
        /// <summary>
        /// Stores whether the given meta enum is a flags enum 
        /// </summary>
        private static readonly bool[] IS_FLAGS;
        #endregion

        /// <summary>
        /// Populates the metadata tables
        /// </summary>
        static EnumMetaTable()
        {
            // Allocate space for the first level of our tables
            IS_FLAGS = new bool[Meta.Length];
            TABLE = new EnumData[Meta.Length][];
            KEYWORD = new Dictionary<AtomicString, object>[Meta.Length];

            // Loop through all meta-enums and populate both of their tables
            for (int enumIndex = 0; enumIndex < Meta.Length; enumIndex++)
            {
                // Find all the meta-values for this enum
                (Type, MetaEnumAttribute) metaInfo = Meta[enumIndex];

                bool hasFlagsAttribute = (metaInfo.Item1.GetCustomAttribute<FlagsAttribute>() is object);
                IS_FLAGS[enumIndex] = hasFlagsAttribute;

                (int, MetaKeywordAttribute)[] metaValues = Get_Meta_Enum_Values(metaInfo.Item1);
                // Initialize the tables for this enum
                int maxValue = metaValues.Max(x => x.Item1);
                TABLE[enumIndex] = new EnumData[maxValue + 1];
                KEYWORD[enumIndex] = new Dictionary<AtomicString, object>(maxValue + 1);
                // Populate the tables
                foreach ((int, MetaKeywordAttribute) metaValue in metaValues)
                {
                    int enumValue = metaValue.Item1;
                    MetaKeywordAttribute metadata = metaValue.Item2;
                    if (metadata is null) continue;

                    int valueIndex = Get_Value_Index(enumIndex, enumValue);

                    var data = new EnumData(metadata.Keyword, metadata.Values);
                    TABLE[enumIndex][valueIndex] = data;
                    KEYWORD[enumIndex].Add(new AtomicString(metadata.Keyword), valueIndex);
                }
            }

        }

        #region Helpers
        public static int IntCast<T>(object o) where T : struct
        {
            int retVal;
            T obj = (T)o;
            retVal = Unsafe.As<T, int>(ref obj);
            //retVal = Unsafe.Unbox<int>(obj);
            return retVal;
        }
        public static object EnumMaxValue<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Max();
        }

        public static unsafe (int, MetaKeywordAttribute)[] Get_Meta_Enum_Values(Type metaEnum)
        {
            if (metaEnum is null) throw new ArgumentNullException(nameof(metaEnum));
            Contract.EndContractBlock();
            
            Type underlyingType = metaEnum.GetEnumUnderlyingType();
            var castMethod = typeof(EnumMetaTable).GetMethod("IntCast").MakeGenericMethod(underlyingType);
            var enumMaxMethod = typeof(EnumMetaTable).GetMethod("EnumMaxValue").MakeGenericMethod(metaEnum);
            object bigValue = enumMaxMethod.Invoke(null, null);
            //int maxValue = Unsafe.Unbox<int>(bigValue);
            int maxValue = (int)castMethod.Invoke(null, new object[] { bigValue });

            int TotalValues = 1 + maxValue;
            var RetVal = new (int, MetaKeywordAttribute)[TotalValues];
            Array metaValues = Enum.GetValues(metaEnum);
            foreach (object o in metaValues)
            {
                var u = Enum.ToObject(metaEnum, o);
                int i = (int)castMethod.Invoke(null, new object[] { u });

                if (i < 0) continue;

                string valueName = Enum.GetName(metaEnum, u);
                MemberInfo[] memberInfos = metaEnum.GetMember(valueName);
                var enumValueMemberInfo = memberInfos.First(m => m.DeclaringType == metaEnum);
                var valueAttribute = enumValueMemberInfo.GetCustomAttribute<MetaKeywordAttribute>(false);

                RetVal[i] = (i, valueAttribute);
            }

            return RetVal;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Get_Value_Index(int enumIndex, int enumValue)
        {
            // If the enum is a flags type then it's values index is actually its set bitnumber
            if (IS_FLAGS[enumIndex])
            {
                return (int)BitOperations.CountLeadingZeros((uint)enumValue);
            }

            return enumValue;
        }

        /// <summary>
        /// Retreives enum metadata from the LUT
        /// </summary>
        /// <param name="enumIndex"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumData Get(int enumIndex, int enumValue)
        {
            return TABLE[enumIndex][Get_Value_Index(enumIndex, enumValue)];
        }

        /// <summary>
        /// Retreives the length of an enums LUT
        /// </summary>
        /// <param name="enumIndex"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(int enumIndex)
        {
            return TABLE[enumIndex].Length;
        }

        /// <summary>
        /// Dumps all compiled metadata into the given file as a Json structure
        /// </summary>
        /// <param name="filename"></param>
        public static async Task Dump_Metadata(string filename)
        {
            using System.IO.Stream Stream = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            await JsonSerializer.SerializeAsync(Stream, TABLE).ConfigureAwait(continueOnCapturedContext: false);
        }

    }
}
