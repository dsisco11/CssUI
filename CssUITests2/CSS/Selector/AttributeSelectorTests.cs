using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for attribute selectors ([attr], [attr=value], [attr~=value], etc.).
/// These tests verify that attribute selectors correctly match elements
/// based on their attribute presence and values.
/// </summary>
/// <remarks>
/// NOTE: Tests may be skipped due to parser bugs:
/// - Attribute selector parsing may cause NullReferenceException in CssParser
/// </remarks>
public class AttributeSelectorTests
{
    #region Test Infrastructure
    private const string AttributeSelectorBugSkipReason = "Bug: Attribute value selectors cause NullReferenceException in parser";

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

    #region Attribute Presence Tests [attr]
    [Fact]
    public void AttributePresence_MatchesElementWithAttribute()
    {
        // Arrange - [disabled] matches any element with the disabled attribute
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("disabled", AttributeValue.From(""));
        var selector = new CssSelector("[disabled]");

        // Debug output
        var selectorCount = selector.Count;
        var complexSelectorCount = selectorCount > 0 ? selector[0].Count : -1;
        var relativeSelectorCount = complexSelectorCount > 0 ? selector[0][0].Count : -1;
        var simpleSelectorType = relativeSelectorCount > 0 ? selector[0][0][0].GetType().Name : "N/A";

        // Check if element has the attribute
        var hasAttr = input.hasAttribute("disabled");

        // Act & Assert
        Assert.True(selector.Count > 0, $"Attribute presence selector should parse. Count={selectorCount}");
        Assert.True(selector[0].Count > 0, $"ComplexSelector should have RelativeSelectors. ComplexSelector.Count={complexSelectorCount}");
        Assert.True(selector[0][0].Count > 0, $"RelativeSelector should have SimpleSelectors. RelativeSelector.Count={relativeSelectorCount}, Type={simpleSelectorType}");
        Assert.True(hasAttr, "Element should have 'disabled' attribute");
        Assert.True(selector[0].Match(input), "[disabled] should match element with disabled attribute");
    }

    [Fact]
    public void AttributePresence_DoesNotMatchElementWithoutAttribute()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector("[disabled]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), "[disabled] should not match element without disabled attribute");
    }

    [Fact]
    public void AttributePresence_MatchesElementWithAnyValue()
    {
        // Arrange - [data-value] matches regardless of value
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        div.setAttribute("data-value", AttributeValue.From("anything"));
        var selector = new CssSelector("[data-value]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "[data-value] should match element with data-value attribute");
    }

    [Fact]
    public void AttributePresence_WithTypeSelector()
    {
        // Arrange - input[type] matches input elements with a type attribute
        var doc = CreateTestDocument();
        var inputWithType = CreateTestElement(doc, "input");
        inputWithType.setAttribute("type", AttributeValue.From("text"));
        var inputNoType = CreateTestElement(doc, "input");
        var selector = new CssSelector("input[type]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(inputWithType), "input[type] should match input with type");
        Assert.False(selector[0].Match(inputNoType), "input[type] should not match input without type");
    }
    #endregion

    #region Attribute Equals Tests [attr=value]
    [Fact]
    public void AttributeEquals_MatchesExactValue()
    {
        // Arrange - [type="text"] matches elements with type exactly equal to "text"
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type=\"text\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), "[type=\"text\"] should match input with type=text");
    }

    [Fact]
    public void AttributeEquals_DoesNotMatchPartialValue()
    {
        // Arrange - [type="text"] should not match "text-field"
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text-field"));
        var selector = new CssSelector("[type=\"text\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), "[type=\"text\"] should not match type=text-field");
    }

    [Fact]
    public void AttributeEquals_CaseSensitiveByDefault()
    {
        // Arrange - [type="TEXT"] should not match type="text" (case-sensitive)
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type=\"TEXT\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), "[type=\"TEXT\"] should not match type=text (case-sensitive)");
    }

    [Fact]
    public void AttributeEquals_CaseInsensitiveFlag()
    {
        // Arrange - [type="TEXT" i] should match type="text" with case-insensitive flag
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type=\"TEXT\" i]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector with i flag should parse");
        Assert.True(selector[0].Match(input), "[type=\"TEXT\" i] should match type=text (case-insensitive)");
    }

    [Fact]
    public void AttributeEquals_SingleQuotedValue()
    {
        // Arrange - [type='text'] with single quotes
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type='text']");

        // Act & Assert
        Assert.True(selector.Count > 0, "Single-quoted selector should parse");
        Assert.True(selector[0].Match(input), "[type='text'] should match");
    }

    [Fact]
    public void AttributeEquals_UnquotedValue()
    {
        // Arrange - [type=text] without quotes (valid for simple values)
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type=text]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Unquoted selector should parse");
        Assert.True(selector[0].Match(input), "[type=text] should match");
    }
    #endregion

    #region Attribute Contains Word Tests [attr~=value]
    [Fact]
    public void AttributeContainsWord_MatchesWordInSpaceSeparatedList()
    {
        // Arrange - [class~="warning"] matches class containing "warning" as a whole word
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        div.setAttribute("class", AttributeValue.From("alert warning urgent"));
        var selector = new CssSelector("[class~=\"warning\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "[class~=\"warning\"] should match class containing warning as word");
    }

    [Fact]
    public void AttributeContainsWord_DoesNotMatchPartialWord()
    {
        // Arrange - [class~="warn"] should not match "warning"
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        div.setAttribute("class", AttributeValue.From("warning"));
        var selector = new CssSelector("[class~=\"warn\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(div), "[class~=\"warn\"] should not match partial word warning");
    }

    [Fact]
    public void AttributeContainsWord_MatchesSingleWord()
    {
        // Arrange - [class~="active"] matches when class is exactly "active"
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        div.setAttribute("class", AttributeValue.From("active"));
        var selector = new CssSelector("[class~=\"active\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "[class~=\"active\"] should match single word class");
    }
    #endregion

    #region Attribute Starts With Tests [attr^=value]
    [Fact]
    public void AttributeStartsWith_MatchesPrefix()
    {
        // Arrange - [href^="https"] matches href starting with "https"
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("https://example.com"));
        var selector = new CssSelector("[href^=\"https\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "[href^=\"https\"] should match href starting with https");
    }

    [Fact]
    public void AttributeStartsWith_DoesNotMatchMiddle()
    {
        // Arrange - [href^="example"] should not match "https://example.com"
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("https://example.com"));
        var selector = new CssSelector("[href^=\"example\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(a), "[href^=\"example\"] should not match value in middle");
    }

    [Fact]
    public void AttributeStartsWith_MatchesExactValue()
    {
        // Arrange - [type^="text"] matches type="text" exactly too
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type^=\"text\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), "[type^=\"text\"] should match type=text");
    }
    #endregion

    #region Attribute Ends With Tests [attr$=value]
    [Fact]
    public void AttributeEndsWith_MatchesSuffix()
    {
        // Arrange - [href$=".pdf"] matches href ending with ".pdf"
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("document.pdf"));
        var selector = new CssSelector("[href$=\".pdf\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "[href$=\".pdf\"] should match href ending with .pdf");
    }

    [Fact]
    public void AttributeEndsWith_DoesNotMatchMiddle()
    {
        // Arrange - [href$="document"] should not match "document.pdf"
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("document.pdf"));
        var selector = new CssSelector("[href$=\"document\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(a), "[href$=\"document\"] should not match value in middle");
    }

    [Fact]
    public void AttributeEndsWith_ImageExtensions()
    {
        // Arrange - [src$=".jpg"], [src$=".png"], etc.
        var doc = CreateTestDocument();
        var img = CreateTestElement(doc, "img");
        img.setAttribute("src", AttributeValue.From("photo.jpg"));
        var jpgSelector = new CssSelector("[src$=\".jpg\"]");
        var pngSelector = new CssSelector("[src$=\".png\"]");

        // Act & Assert
        Assert.True(jpgSelector.Count > 0, "JPG selector should parse");
        Assert.True(jpgSelector[0].Match(img), "[src$=\".jpg\"] should match");
        Assert.False(pngSelector[0].Match(img), "[src$=\".png\"] should not match .jpg file");
    }
    #endregion

    #region Attribute Contains Tests [attr*=value]
    [Fact]
    public void AttributeContains_MatchesSubstring()
    {
        // Arrange - [href*="example"] matches href containing "example" anywhere
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("https://example.com/page"));
        var selector = new CssSelector("[href*=\"example\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "[href*=\"example\"] should match href containing example");
    }

    [Fact]
    public void AttributeContains_MatchesAtStart()
    {
        // Arrange - [href*="https"] matches at start
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("https://example.com"));
        var selector = new CssSelector("[href*=\"https\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "[href*=\"https\"] should match at start");
    }

    [Fact]
    public void AttributeContains_MatchesAtEnd()
    {
        // Arrange - [href*=".com"] matches at end
        var doc = CreateTestDocument();
        var a = CreateTestElement(doc, "a");
        a.setAttribute("href", AttributeValue.From("https://example.com"));
        var selector = new CssSelector("[href*=\".com\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "[href*=\".com\"] should match at end");
    }
    #endregion

    #region Attribute Dash Match Tests [attr|=value]
    [Fact]
    public void AttributeDashMatch_MatchesExactValue()
    {
        // Arrange - [lang|="en"] matches lang="en"
        var doc = CreateTestDocument();
        var p = CreateTestElement(doc, "p");
        p.setAttribute("lang", AttributeValue.From("en"));
        var selector = new CssSelector("[lang|=\"en\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "[lang|=\"en\"] should match lang=en");
    }

    [Fact]
    public void AttributeDashMatch_MatchesHyphenatedPrefix()
    {
        // Arrange - [lang|="en"] matches lang="en-US"
        var doc = CreateTestDocument();
        var p = CreateTestElement(doc, "p");
        p.setAttribute("lang", AttributeValue.From("en-US"));
        var selector = new CssSelector("[lang|=\"en\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "[lang|=\"en\"] should match lang=en-US");
    }

    [Fact]
    public void AttributeDashMatch_DoesNotMatchWithoutHyphen()
    {
        // Arrange - [lang|="en"] should not match lang="english"
        var doc = CreateTestDocument();
        var p = CreateTestElement(doc, "p");
        p.setAttribute("lang", AttributeValue.From("english"));
        var selector = new CssSelector("[lang|=\"en\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(p), "[lang|=\"en\"] should not match lang=english");
    }
    #endregion

    #region Combined Attribute Tests
    [Fact]
    public void TypeAndAttribute_CombinedMatching()
    {
        // Arrange - input[type="text"] combines type and attribute selector
        var doc = CreateTestDocument();
        var textInput = CreateTestElement(doc, "input");
        textInput.setAttribute("type", AttributeValue.From("text"));
        var buttonInput = CreateTestElement(doc, "input");
        buttonInput.setAttribute("type", AttributeValue.From("button"));
        var textDiv = CreateTestElement(doc, "div");
        textDiv.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("input[type=\"text\"]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(textInput), "input[type=\"text\"] should match text input");
        Assert.False(selector[0].Match(buttonInput), "input[type=\"text\"] should not match button input");
        Assert.False(selector[0].Match(textDiv), "input[type=\"text\"] should not match div");
    }

    [Fact]
    public void MultipleAttributes_AllMustMatch()
    {
        // Arrange - [type="text"][required] must match both
        var doc = CreateTestDocument();
        var requiredText = CreateTestElement(doc, "input");
        requiredText.setAttribute("type", AttributeValue.From("text"));
        requiredText.setAttribute("required", AttributeValue.From(""));
        var optionalText = CreateTestElement(doc, "input");
        optionalText.setAttribute("type", AttributeValue.From("text"));
        var selector = new CssSelector("[type=\"text\"][required]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(requiredText), "Should match element with both attributes");
        Assert.False(selector[0].Match(optionalText), "Should not match element missing required");
    }

    [Fact]
    public void AttributeWithClass_CombinedMatching()
    {
        // Arrange - .btn[disabled] combines class and attribute
        var doc = CreateTestDocument();
        var disabledBtn = CreateTestElement(doc, "button");
        disabledBtn.className = "btn";
        disabledBtn.setAttribute("disabled", AttributeValue.From(""));
        var enabledBtn = CreateTestElement(doc, "button");
        enabledBtn.className = "btn";
        var selector = new CssSelector(".btn[disabled]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(disabledBtn), ".btn[disabled] should match disabled button");
        Assert.False(selector[0].Match(enabledBtn), ".btn[disabled] should not match enabled button");
    }

    [Fact]
    public void AttributeWithId_CombinedMatching()
    {
        // Arrange - #myId[data-active] combines id and attribute
        var doc = CreateTestDocument();
        var activeElement = CreateTestElement(doc, "div");
        activeElement.id = "myId";
        activeElement.setAttribute("data-active", AttributeValue.From("true"));
        var inactiveElement = CreateTestElement(doc, "div");
        inactiveElement.id = "myId";
        var selector = new CssSelector("#myId[data-active]");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(activeElement), "#myId[data-active] should match");
        Assert.False(selector[0].Match(inactiveElement), "#myId[data-active] should not match without attribute");
    }
    #endregion

    #region Specificity Tests
    [Fact]
    public void AttributeSelector_HasCorrectSpecificity()
    {
        // Arrange - Attribute selectors have same specificity as class (0,1,0)
        var classSelector = new CssSelector(".class");
        var attrSelector = new CssSelector("[type]");

        // Act & Assert
        Assert.True(classSelector.Count > 0, "Class selector should parse");
        Assert.True(attrSelector.Count > 0, "Attribute selector should parse");
        Assert.Equal(classSelector[0].Get_Specificity(), attrSelector[0].Get_Specificity());
    }
    #endregion
}
