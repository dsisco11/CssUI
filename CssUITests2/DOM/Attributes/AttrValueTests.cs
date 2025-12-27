using CssUI;
using CssUI.DOM;
using CssUI.DOM.Enums;
using Xunit;

namespace CssUITests.DOM.Attributes;

/// <summary>
/// Unit tests for Attr class value assignment and retrieval.
/// Investigates potential issues with Attr.Value property behavior.
/// Spec: https://dom.spec.whatwg.org/#interface-attr
/// </summary>
public class AttrValueTests
{
    #region Test Infrastructure

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    #endregion

    #region Attr Creation Tests

    [Fact]
    public void Attr_Creation_HasCorrectLocalName()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";

        // Act
        var attr = new Attr(attrName, element);

        // Assert
        Assert.Equal(attrName, attr.localName);
    }

    [Fact]
    public void Attr_Creation_HasCorrectOwnerElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";

        // Act
        var attr = new Attr(attrName, element);

        // Assert
        Assert.Same(element, attr.ownerElement);
    }

    [Fact]
    public void Attr_Creation_InitialValueIsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";

        // Act
        var attr = new Attr(attrName, element);

        // Assert
        Assert.True(attr.IsMissingValue, "Newly created Attr should have missing value");
    }

    #endregion

    #region Attr.Value Assignment Tests

    [Fact]
    public void Attr_ValueAssignment_SetsValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        var value = AttributeValue.From("test-value");

        // Act
        attr.Value = value;

        // Assert
        Assert.NotNull(attr.Value);
        Assert.Equal("test-value", attr.Value.Data);
    }

    [Fact]
    public void Attr_ValueAssignment_EmptyString_SetsValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        var value = AttributeValue.From(string.Empty);

        // Act
        attr.Value = value;

        // Assert
        Assert.NotNull(attr.Value);
        Assert.False(attr.IsMissingValue, "Attr should not be missing after setting empty string value");
    }

    [Fact]
    public void Attr_ValueAssignment_Integer_SetsValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-count";
        var attr = new Attr(attrName, element);
        var value = AttributeValue.From(42);

        // Act
        attr.Value = value;

        // Assert
        Assert.NotNull(attr.Value);
        Assert.Equal("42", attr.Value.Data);
    }

    [Fact]
    public void Attr_ValueAssignment_PreservesValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        var value = AttributeValue.From("original");

        // Act
        attr.Value = value;
        var retrievedValue = attr.Value;

        // Assert
        Assert.Same(value, retrievedValue);
    }

    #endregion

    #region Attr.Value Retrieval Tests

    [Fact]
    public void Attr_nodeValue_MatchesValueData()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        attr.Value = AttributeValue.From("test-value");

        // Act & Assert
        Assert.Equal("test-value", attr.nodeValue);
    }

    [Fact]
    public void Attr_textContent_MatchesValueData()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        attr.Value = AttributeValue.From("test-value");

        // Act & Assert
        Assert.Equal("test-value", attr.textContent);
    }

    [Fact]
    public void Attr_nodeValue_Assignment_UpdatesValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);

        // Act
        attr.nodeValue = "new-value";

        // Assert
        Assert.Equal("new-value", attr.Value?.Data);
    }

    #endregion

    #region IsMissingValue / IsInvalidValue Tests

    [Fact]
    public void Attr_IsMissingValue_TrueWhenNoValueSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);

        // Assert
        Assert.True(attr.IsMissingValue);
    }

    [Fact]
    public void Attr_IsMissingValue_FalseAfterValueSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);

        // Act
        attr.Value = AttributeValue.From("value");

        // Assert
        Assert.False(attr.IsMissingValue);
    }

    [Fact]
    public void Attr_IsDefined_TrueForValidValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        attr.Value = AttributeValue.From("valid");

        // Assert
        Assert.True(attr.IsDefined);
    }

    [Fact]
    public void Attr_IsDefined_FalseForMissingValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);

        // Assert
        Assert.False(attr.IsDefined);
    }

    #endregion

    #region getAttributeNode Tests

    [Fact]
    public void GetAttributeNode_ReturnsAttrWithCorrectValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        element.setAttribute(attrName, AttributeValue.From("test-value"));

        // Act
        var attr = element.getAttributeNode(attrName);

        // Assert
        Assert.NotNull(attr);
        Assert.Equal("test-value", attr.Value?.Data);
    }

    [Fact]
    public void GetAttributeNode_NonExistent_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-nonexistent";

        // Act
        var attr = element.getAttributeNode(attrName);

        // Assert
        Assert.Null(attr);
    }

    [Fact]
    public void SetAttributeNode_AddsNewAttribute()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        var attr = new Attr(attrName, element);
        attr.Value = AttributeValue.From("value");

        // Act
        var oldAttr = element.setAttributeNode(attr);

        // Assert
        Assert.Null(oldAttr); // No previous attribute
        Assert.True(element.hasAttribute(attrName));
    }

    [Fact]
    public void SetAttributeNode_ReplacesExisting_ReturnsOldAttr()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        element.setAttribute(attrName, AttributeValue.From("old-value"));

        var newAttr = new Attr(attrName, element);
        newAttr.Value = AttributeValue.From("new-value");

        // Act
        var oldAttr = element.setAttributeNode(newAttr);

        // Assert
        Assert.NotNull(oldAttr);
        Assert.Equal("old-value", oldAttr.Value?.Data);
        Assert.Equal("new-value", element.getAttribute(attrName)?.Data);
    }

    [Fact]
    public void RemoveAttributeNode_RemovesAndReturnsAttr()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var attrName = "data-test";
        element.setAttribute(attrName, AttributeValue.From("value"));
        var attr = element.getAttributeNode(attrName);

        // Act
        var removedAttr = element.removeAttributeNode(attr!);

        // Assert
        Assert.NotNull(removedAttr);
        Assert.False(element.hasAttribute(attrName));
    }

    #endregion
}
