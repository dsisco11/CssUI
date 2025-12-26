using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using CssUI;

namespace CssUITests.CORE;

public class LookupTests
{
    public List<Type> metaEnumList = new List<Type>();

    // Helper method to check if a field has EnumDataAttribute (checking by name since the type is embedded)
    private static bool HasEnumDataAttribute(FieldInfo field)
    {
        return field.GetCustomAttributes(false)
            .Any(attr => attr.GetType().FullName == "EnumRecords.EnumDataAttribute");
    }

    // Helper method to get only enum values that have EnumDataAttribute
    private static IEnumerable<object> GetValuesWithEnumDataAttribute(Type enumType)
    {
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            if (HasEnumDataAttribute(field))
            {
                yield return field.GetValue(null)!;
            }
        }
    }

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

        // Filter to only enums that have at least one member with EnumData attribute
        var enumsWithMembers = metaEnumList.Where(enumType =>
            Enum.GetValues(enumType).Length > 0 &&
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        Assert.NotEmpty(enumsWithMembers); // Ensure we found some enums with members

        foreach (var enumType in enumsWithMembers)
        {
            Assert.True(Lookup.Is_Declared(enumType), $"Enum {enumType.Name} should have EnumRecords extensions");
        }
    }



    [Fact()]
    public void TryKeywordTest()
    {
        // Get enums that have members with EnumData attributes
        var enumsWithMembers = metaEnumList.Where(enumType =>
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        foreach (var enumType in enumsWithMembers)
        {
            // Only test values that actually have EnumData attributes
            var valuesWithKeywords = GetValuesWithEnumDataAttribute(enumType).ToList();
            foreach (var value in valuesWithKeywords)
            {
                Assert.True(Lookup.TryKeyword(enumType, (IConvertible)value, out string? keyword),
                    $"TryKeyword failed for {enumType.Name}.{value}");
            }
        }
    }

    [Fact()]
    public void KeywordTest()
    {
        // Get enums that have members with EnumData attributes
        var enumsWithMembers = metaEnumList.Where(enumType =>
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        // Just make sure we wont ever get an exception thrown
        foreach (var enumType in enumsWithMembers)
        {
            // Only test values that actually have EnumData attributes
            var valuesWithKeywords = GetValuesWithEnumDataAttribute(enumType).ToList();
            foreach (var value in valuesWithKeywords)
            {
                Lookup.TryKeyword(enumType, (IConvertible)value, out string? _);
            }
        }
    }

    // Note: TryData and Data tests removed - those methods were deprecated and removed
    // as part of the EnumRecords migration. Use generated extension methods instead.

    [Fact()]
    public void TryEnumTest()
    {
        // Get enums that have members with EnumData attributes
        var enumsWithMembers = metaEnumList.Where(enumType =>
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        foreach (var enumType in enumsWithMembers)
        {
            // Only test values that actually have EnumData attributes
            var valuesWithKeywords = GetValuesWithEnumDataAttribute(enumType).ToList();
            foreach (var value in valuesWithKeywords)
            {
                // Get the keyword for this enum value using TryKeyword
                Assert.True(Lookup.TryKeyword(enumType, (IConvertible)value, out string? keyword),
                    $"TryKeyword failed for {enumType.Name}.{value}");
                Assert.NotNull(keyword);

                // Now verify we can look up the enum value by keyword
                Assert.True(Lookup.TryEnum(enumType, keyword, out var outValue),
                    $"TryEnum failed for keyword '{keyword}' from {enumType.Name}");
                Assert.Equal(value, outValue);
            }
        }
    }

    [Fact()]
    public void EnumTest()
    {
        // Get enums that have members with EnumData attributes
        var enumsWithMembers = metaEnumList.Where(enumType =>
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        foreach (var enumType in enumsWithMembers)
        {
            // Only test values that actually have EnumData attributes
            var valuesWithKeywords = GetValuesWithEnumDataAttribute(enumType).ToList();
            foreach (var value in valuesWithKeywords)
            {
                // Get the keyword for this enum value using TryKeyword
                Assert.True(Lookup.TryKeyword(enumType, (IConvertible)value, out string? keyword),
                    $"TryKeyword failed for {enumType.Name}.{value}");
                Assert.NotNull(keyword);

                // Now verify we can look up the enum value by keyword
                var actual = Lookup.Enum(enumType, keyword);
                Assert.Equal(value, actual);
            }
        }
    }

    [Fact()]
    public void Is_DeclaredTest()
    {
        // Get enums that have members with EnumData attributes
        var enumsWithMembers = metaEnumList.Where(enumType =>
            enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Any(HasEnumDataAttribute)).ToList();

        foreach (var enumType in enumsWithMembers)
        {
            // Only test values that actually have EnumData attributes
            var valuesWithKeywords = GetValuesWithEnumDataAttribute(enumType).ToList();
            foreach (var value in valuesWithKeywords)
            {
                // Get the keyword for this enum value using TryKeyword
                Assert.True(Lookup.TryKeyword(enumType, (IConvertible)value, out string? keyword),
                    $"TryKeyword failed for {enumType.Name}.{value}");
                Assert.NotNull(keyword);

                // Verify the keyword is declared
                Assert.True(Lookup.Is_Declared(enumType, keyword),
                    $"Is_Declared returned false for keyword '{keyword}' from {enumType.Name}");
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
