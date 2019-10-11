using Xunit;
using CssUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CssUI.Tests
{
    public class LookupTests
    {
        public List<Type> metaEnumList = new List<Type>();

        public LookupTests()
        {
            metaEnumList = new List<Type>();

            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().DefinedTypes;
            Attribute attr = null;

            foreach (Type type in allTypes)
            {
                attr = type.GetCustomAttribute(typeof(CssUI.Internal.MetaEnumAttribute));
                if (attr != null)
                {
                    metaEnumList.Add(type);
                }
            }
        }

        /// <summary>
        /// Ensures that all enum types flagged as a MetaEnum have been compiled into a LUT
        /// </summary>
        [Fact(DisplayName = "Assert Meta Definitions")]
        public void Test_Meta_Enums()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True(Lookup.Is_Declared(enumType, keyword));
                }
            }
        }



        [Fact()]
        public void TryKeywordTest()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    Assert.True(Lookup.TryKeyword(enumType, (IConvertible)value, out string _));
                }
            }
        }

        [Fact()]
        public void KeywordTest()
        {
            // Just make sure we wont ever get an exception thrown
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    Lookup.TryKeyword(enumType, (IConvertible)value, out string _);
                }
            }
        }

        [Fact()]
        public void TryDataTest()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    Assert.True(Lookup.TryData(enumType, (IConvertible)value, out EnumData _));
                }
            }
        }

        [Fact()]
        public void DataTest()
        {
            // Just make sure we wont ever get an exception thrown
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    Lookup.Data(enumType, (IConvertible)value);
                }
            }
        }

        [Fact()]
        public void TryEnumTest()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True(Lookup.TryEnum(enumType, keyword, out object outValue));
                    Assert.Equal(value, outValue);
                }
            }
        }

        [Fact()]
        public void EnumTest()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    var actual = Lookup.Enum(enumType, keyword);
                    Assert.Equal(value, actual);
                }
            }
        }

        [Fact()]
        public void Is_DeclaredTest()
        {
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Assert.True(Lookup.Is_Declared(enumType, keyword));
                }
            }
        }

        [Fact()]
        public void Get_KeywordsTest()
        {
            // Just make sure we wont ever get an exception thrown
            foreach (var enumType in metaEnumList)
            {
                /* Compile a list of all values from this enum */
                var allValues = Enum.GetValues(enumType);
                foreach (var value in allValues)
                {
                    string keyword = Enum.GetName(enumType, value);
                    Lookup.Get_Keywords(enumType);
                }
            }
        }
    }
}