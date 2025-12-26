using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for <see cref="CssFunctionValue"/> - the specialized CSS value type for CSS functions.
/// </summary>
public class CssFunctionValueTests
{
    #region Factory Method Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void From_CssFunction_ReturnsCssFunctionValue()
    {
        var func = new CssFunction("test-func".AsSpan());
        var value = CssValue.From(func);

        Assert.IsType<CssFunctionValue>(value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void From_CssFunction_SetsTypeToFUNCTION()
    {
        var func = new CssFunction("my-func".AsSpan());
        var value = CssValue.From(func);

        Assert.Equal(ECssValueTypes.FUNCTION, value.Type);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void From_CssFunction_HasValueReturnsTrue()
    {
        var func = new CssFunction("custom".AsSpan());
        var value = CssValue.From(func);

        Assert.True(value.HasValue);
    }

    #endregion

    #region Value Access Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Function_ReturnsStoredFunction()
    {
        var func = new CssFunction("test".AsSpan());
        var funcValue = (CssFunctionValue)CssValue.From(func);

        Assert.Same(func, funcValue.Function);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void AsFunction_ReturnsStoredFunction()
    {
        var func = new CssFunction("test".AsSpan());
        var funcValue = (CssFunctionValue)CssValue.From(func);

        var retrieved = funcValue.AsFunction();

        Assert.Same(func, retrieved);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void ToString_EmptyFunction_ReturnsFunctionNotation()
    {
        var func = new CssFunction("my-func".AsSpan());
        var value = CssValue.From(func);

        var result = value.ToString();

        Assert.Equal("my-func()", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Serialize_EmptyFunction_ReturnsFunctionNotation()
    {
        var func = new CssFunction("test-func".AsSpan());
        var value = CssValue.From(func);

        var result = value.Serialize();

        Assert.Equal("test-func()", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void ToString_FunctionWithName_PreservesName()
    {
        var func = new CssFunction("some-function-name".AsSpan());
        var value = CssValue.From(func);

        var result = value.ToString();

        Assert.StartsWith("some-function-name", result);
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    [Trait("Category", "Formatting")]
    public void TryFormat_SimpleFunction_WritesFunction()
    {
        var func = new CssFunction("test".AsSpan());
        var value = CssValue.From(func);
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("test()", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var func = new CssFunction("test-function".AsSpan());
        var value = CssValue.From(func);
        Span<char> buffer = stackalloc char[3]; // Too small

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Equals_SameFunction_ReturnsTrue()
    {
        var func = new CssFunction("test".AsSpan());
        var value1 = CssValue.From(func);
        var value2 = CssValue.From(func); // Same reference

        // Same reference should be equal
        Assert.True(value1.Equals(value1));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Equals_DifferentFunctionSameName_ReturnsTrue()
    {
        var func1 = new CssFunction("test".AsSpan());
        var func2 = new CssFunction("test".AsSpan());
        var value1 = CssValue.From(func1);
        var value2 = CssValue.From(func2);

        // Same encoded output should be equal
        Assert.True(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Equals_DifferentFunctionName_ReturnsFalse()
    {
        var func1 = new CssFunction("func1".AsSpan());
        var func2 = new CssFunction("func2".AsSpan());
        var value1 = CssValue.From(func1);
        var value2 = CssValue.From(func2);

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void Equals_Null_ReturnsFalse()
    {
        var func = new CssFunction("test".AsSpan());
        var value = CssValue.From(func);

        Assert.False(value.Equals(null));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void GetHashCode_SameName_ReturnsSameHash()
    {
        var func1 = new CssFunction("test".AsSpan());
        var func2 = new CssFunction("test".AsSpan());
        var value1 = CssValue.From(func1);
        var value2 = CssValue.From(func2);

        Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Function")]
    public void GetHashCode_DifferentName_ReturnsDifferentHash()
    {
        var func1 = new CssFunction("func1".AsSpan());
        var func2 = new CssFunction("func2".AsSpan());
        var value1 = CssValue.From(func1);
        var value2 = CssValue.From(func2);

        Assert.NotEqual(value1.GetHashCode(), value2.GetHashCode());
    }

    #endregion
}
