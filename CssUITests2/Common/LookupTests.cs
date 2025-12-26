using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using CssUI;
using EnumRecords;

namespace CssUITests.CORE;

public class LookupTests
{
    public List<Type> metaEnumList = new List<Type>();

    public LookupTests()
    {
        metaEnumList = new List<Type>();

        // Find all enums decorated with EnumRecordAttribute<T> (EnumRecords package)
        var allTypes = typeof(CssUI.CSS.ECssUnit).Assembly.DefinedTypes;

        foreach (Type type in allTypes)
        {
            if (!type.IsEnum) continue;

            // Check if the enum has any EnumRecordAttribute<T> (generic attribute)
            var attrs = type.GetCustomAttributes(inherit: false);
            foreach (var attr in attrs)
            {
                var attrType = attr.GetType();
                if (attrType.IsGenericType && attrType.GetGenericTypeDefinition().Name.StartsWith("EnumRecordAttribute"))
                {
                    metaEnumList.Add(type);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Ensures that all enum types flagged with EnumRecordAttribute have extensions generated
    /// </summary>
    [Fact(DisplayName = "Assert EnumRecords Extensions")]
    public void Test_Meta_Enums()
    {
        Assert.NotEmpty(metaEnumList); // Ensure we found some enums

        foreach (var enumType in metaEnumList)
        {
            Assert.True(Lookup.Is_Declared(enumType), $"Enum {enumType.Name} should have EnumRecords extensions");
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

    // Note: TryData and Data tests removed - those methods were deprecated and removed
    // as part of the EnumRecords migration. Use generated extension methods instead.

    [Fact()]
    public void TryEnumTest()
    {
        foreach (var enumType in metaEnumList)
        {
            /* Compile a list of all values from this enum */
            var allValues = Enum.GetValues(enumType);
            foreach (var value in allValues)
            {
                var keyword = Enum.GetName(enumType, value);
                Assert.NotNull(keyword);
                Assert.True(Lookup.TryEnum(enumType, keyword, out var outValue));
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
                var keyword = Enum.GetName(enumType, value);
                Assert.NotNull(keyword);
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
                var keyword = Enum.GetName(enumType, value);
                Assert.NotNull(keyword);
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
                var keyword = Enum.GetName(enumType, value);
                Lookup.Get_Keywords(enumType);
            }
        }
    }
}
