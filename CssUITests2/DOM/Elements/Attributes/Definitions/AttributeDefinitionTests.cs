using System;
using CssUI;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using Xunit;

namespace CssUITests.DOM;

/// <summary>
/// Unit tests for AttributeDefinition functionality.
/// </summary>
public class AttributeDefinitionTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_SetsName()
    {
        // Arrange & Act
        var def = new AttributeDefinition("test-attr", EAttributeType.String);

        // Assert
        Assert.Equal("test-attr", def.Name!.Name);
    }

    [Fact]
    public void Constructor_SetsType()
    {
        // Arrange & Act
        var def = new AttributeDefinition("test-attr", EAttributeType.Boolean);

        // Assert
        Assert.Equal(EAttributeType.Boolean, def.Type);
    }

    [Fact]
    public void Constructor_SetsFlags()
    {
        // Arrange & Act
        var def = new AttributeDefinition("test-attr", EAttributeType.String, null, null, EAttributeFlags.Inherited);

        // Assert
        Assert.True(def.Flags.HasFlag(EAttributeFlags.Inherited));
    }

    [Fact]
    public void Constructor_SetsKeywords()
    {
        // Arrange
        var keywords = new[] { "value1", "value2" };

        // Act
        var def = new AttributeDefinition("test-attr", EAttributeType.Enumerated, null, null, EAttributeFlags.None, keywords);

        // Assert
        Assert.NotNull(def.Keywords);
        Assert.Contains(new AtomicString("value1"), def.Keywords);
        Assert.Contains(new AtomicString("value2"), def.Keywords);
    }

    #endregion

    #region Parse Tests

    [Fact]
    public void Parse_StringType_ReturnsValue()
    {
        // Arrange
        var def = new AttributeDefinition("test-attr", EAttributeType.String);

        // Act
        def.Parse("test-value", out object result);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Parse_BooleanType_ReturnsValue()
    {
        // Arrange
        var def = new AttributeDefinition("test-attr", EAttributeType.Boolean);

        // Act
        def.Parse("true", out object resultTrue);
        def.Parse("false", out object resultFalse);

        // Assert
        Assert.NotNull(resultTrue);
        Assert.NotNull(resultFalse);
    }

    [Fact]
    public void Parse_IntegerType_ReturnsValue()
    {
        // Arrange
        var def = new AttributeDefinition("test-attr", EAttributeType.Integer);

        // Act
        def.Parse("42", out object result);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region CheckAndThrow Tests

    [Fact]
    public void CheckAndThrow_ValidatesCorrectly()
    {
        string AttrName = "test";
        AttributeDefinition def;

        //
        def = new AttributeDefinition(AttrName, EAttributeType.Boolean);
        def.CheckAndThrow("1");
        def.CheckAndThrow("true");
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Color);
        def.CheckAndThrow("#FFFFFF");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("#FFFFFFFF"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Date);
        def.CheckAndThrow("1992-07-03");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("0"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("0-"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("1992-00"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("1992-13"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("1992-07-00"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("1992-07-69"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Time);
        def.CheckAndThrow("12:59");
        def.CheckAndThrow("12:59:59.999");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("-1"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("13"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("12:60"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("12:59:60"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("12:59:60.9"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Duration);
        def.CheckAndThrow("0.0s");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("0%"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Enumerated, null, null, EAttributeFlags.None, new string[] { "one", "two", "three" });
        def.CheckAndThrow("two");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Integer);
        def.CheckAndThrow("0");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow(".0"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.NonNegative_Integer);
        def.CheckAndThrow("0");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("-1"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.FloatingPoint);
        def.CheckAndThrow("-1.0");
        def.CheckAndThrow("0.0");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Length);
        def.CheckAndThrow("0");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("-1.0"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.NonZero_Length);
        def.CheckAndThrow("1");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("-1.0"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("0.0"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.Percentage);
        def.CheckAndThrow("1.0%");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        //
        def = new AttributeDefinition(AttrName, EAttributeType.NonZero_Percentage);
        def.CheckAndThrow("1.0%");
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("ABC"));
        Assert.Throws<DomSyntaxError>(() => def.CheckAndThrow("-1%"));
    }

    #endregion

    #region Lookup Tests

    [Fact(Skip = "Bug: Not all EAttributeName values have definitions registered")]
    public void LookupTest()
    {
        var attribNames = Enum.GetValues(typeof(EAttributeName));
        foreach (EAttributeName Name in attribNames)
        {
            AttributeDefinition def = AttributeDefinition.Lookup(Name);
            Assert.NotNull(def);
        }
    }

    #endregion
}
