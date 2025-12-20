using CssUI.DOM;
using Xunit;

namespace CssUI.CSS.Selector.Tests;

/// <summary>
/// Tests for input/form-related pseudo-class selectors.
/// See: https://www.w3.org/TR/selectors-4/#ui-pseudos
/// </summary>
public class InputPseudoClassTests
{
    #region Test Infrastructure
    private const string SkipReason = "Input pseudo-class not yet implemented or requires form element state";

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

    #region :enabled Tests
    [Fact(Skip = SkipReason)]
    public void Enabled_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":enabled");

        // Assert
        Assert.True(selector.Count > 0, ":enabled selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Enabled_MatchesEnabledFormControl()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        // input is enabled by default (no disabled attribute)
        var selector = new CssSelector(":enabled");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":enabled should match enabled input");
    }

    [Fact(Skip = SkipReason)]
    public void Enabled_DoesNotMatchDisabledControl()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("disabled", AttributeValue.From(""));
        var selector = new CssSelector(":enabled");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), ":enabled should not match disabled input");
    }

    [Fact(Skip = SkipReason)]
    public void Enabled_OnlyAppliesToFormElements()
    {
        // Arrange - :enabled typically only matches form-associated elements
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var selector = new CssSelector(":enabled");

        // Act & Assert - div is not a form control, so shouldn't match :enabled
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(div), ":enabled should not match non-form elements");
    }
    #endregion

    #region :disabled Tests
    [Fact(Skip = SkipReason)]
    public void Disabled_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":disabled");

        // Assert
        Assert.True(selector.Count > 0, ":disabled selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Disabled_MatchesDisabledFormControl()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("disabled", AttributeValue.From(""));
        var selector = new CssSelector(":disabled");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":disabled should match disabled input");
    }

    [Fact(Skip = SkipReason)]
    public void Disabled_DoesNotMatchEnabledControl()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector(":disabled");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), ":disabled should not match enabled input");
    }

    [Fact(Skip = SkipReason)]
    public void Disabled_InheritsFromFieldset()
    {
        // Arrange - Controls in a disabled fieldset are also disabled
        var doc = CreateTestDocument();
        var fieldset = CreateTestElement(doc, "fieldset");
        fieldset.setAttribute("disabled", AttributeValue.From(""));
        var input = CreateTestElement(doc, "input");
        fieldset.appendChild(input);
        var selector = new CssSelector(":disabled");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":disabled should match input in disabled fieldset");
    }
    #endregion

    #region :checked Tests
    [Fact(Skip = SkipReason)]
    public void Checked_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":checked");

        // Assert
        Assert.True(selector.Count > 0, ":checked selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Checked_MatchesCheckedCheckbox()
    {
        // Arrange
        var doc = CreateTestDocument();
        var checkbox = CreateTestElement(doc, "input");
        checkbox.setAttribute("type", AttributeValue.From("checkbox"));
        checkbox.setAttribute("checked", AttributeValue.From(""));
        var selector = new CssSelector(":checked");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(checkbox), ":checked should match checked checkbox");
    }

    [Fact(Skip = SkipReason)]
    public void Checked_MatchesCheckedRadio()
    {
        // Arrange
        var doc = CreateTestDocument();
        var radio = CreateTestElement(doc, "input");
        radio.setAttribute("type", AttributeValue.From("radio"));
        radio.setAttribute("checked", AttributeValue.From(""));
        var selector = new CssSelector(":checked");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(radio), ":checked should match checked radio");
    }

    [Fact(Skip = SkipReason)]
    public void Checked_MatchesSelectedOption()
    {
        // Arrange
        var doc = CreateTestDocument();
        var option = CreateTestElement(doc, "option");
        option.setAttribute("selected", AttributeValue.From(""));
        var selector = new CssSelector(":checked");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(option), ":checked should match selected option");
    }

    [Fact(Skip = SkipReason)]
    public void Checked_DoesNotMatchUnchecked()
    {
        // Arrange
        var doc = CreateTestDocument();
        var checkbox = CreateTestElement(doc, "input");
        checkbox.setAttribute("type", AttributeValue.From("checkbox"));
        var selector = new CssSelector(":checked");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(checkbox), ":checked should not match unchecked checkbox");
    }
    #endregion

    #region :indeterminate Tests
    [Fact(Skip = SkipReason)]
    public void Indeterminate_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":indeterminate");

        // Assert
        Assert.True(selector.Count > 0, ":indeterminate selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Indeterminate_MatchesProgressWithNoValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var progress = CreateTestElement(doc, "progress");
        // progress without value attribute is indeterminate
        var selector = new CssSelector(":indeterminate");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(progress), ":indeterminate should match progress without value");
    }
    #endregion

    #region :required Tests
    [Fact(Skip = SkipReason)]
    public void Required_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":required");

        // Assert
        Assert.True(selector.Count > 0, ":required selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Required_MatchesRequiredInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("required", AttributeValue.From(""));
        var selector = new CssSelector(":required");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":required should match required input");
    }

    [Fact(Skip = SkipReason)]
    public void Required_DoesNotMatchOptionalInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector(":required");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), ":required should not match optional input");
    }
    #endregion

    #region :optional Tests
    [Fact(Skip = SkipReason)]
    public void Optional_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":optional");

        // Assert
        Assert.True(selector.Count > 0, ":optional selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Optional_MatchesOptionalInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector(":optional");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":optional should match optional input");
    }

    [Fact(Skip = SkipReason)]
    public void Optional_DoesNotMatchRequiredInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("required", AttributeValue.From(""));
        var selector = new CssSelector(":optional");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), ":optional should not match required input");
    }
    #endregion

    #region :valid Tests
    [Fact(Skip = SkipReason)]
    public void Valid_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":valid");

        // Assert
        Assert.True(selector.Count > 0, ":valid selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Valid_MatchesValidInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("email"));
        input.setAttribute("value", AttributeValue.From("test@example.com"));
        var selector = new CssSelector(":valid");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        // Note: Actual validation depends on implementation
    }
    #endregion

    #region :invalid Tests
    [Fact(Skip = SkipReason)]
    public void Invalid_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":invalid");

        // Assert
        Assert.True(selector.Count > 0, ":invalid selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Invalid_MatchesRequiredEmptyInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("required", AttributeValue.From(""));
        var selector = new CssSelector(":invalid");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":invalid should match required input with no value");
    }
    #endregion

    #region :in-range Tests
    [Fact(Skip = SkipReason)]
    public void InRange_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":in-range");

        // Assert
        Assert.True(selector.Count > 0, ":in-range selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void InRange_MatchesValueInRange()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("number"));
        input.setAttribute("min", AttributeValue.From("0"));
        input.setAttribute("max", AttributeValue.From("100"));
        input.setAttribute("value", AttributeValue.From("50"));
        var selector = new CssSelector(":in-range");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":in-range should match value within range");
    }
    #endregion

    #region :out-of-range Tests
    [Fact(Skip = SkipReason)]
    public void OutOfRange_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":out-of-range");

        // Assert
        Assert.True(selector.Count > 0, ":out-of-range selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void OutOfRange_MatchesValueOutsideRange()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("type", AttributeValue.From("number"));
        input.setAttribute("min", AttributeValue.From("0"));
        input.setAttribute("max", AttributeValue.From("100"));
        input.setAttribute("value", AttributeValue.From("150"));
        var selector = new CssSelector(":out-of-range");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":out-of-range should match value outside range");
    }
    #endregion

    #region :read-only Tests
    [Fact(Skip = SkipReason)]
    public void ReadOnly_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":read-only");

        // Assert
        Assert.True(selector.Count > 0, ":read-only selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void ReadOnly_MatchesReadOnlyInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("readonly", AttributeValue.From(""));
        var selector = new CssSelector(":read-only");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":read-only should match readonly input");
    }

    [Fact(Skip = SkipReason)]
    public void ReadOnly_MatchesNonEditableElements()
    {
        // Arrange - Non-editable elements like div are always read-only
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var selector = new CssSelector(":read-only");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), ":read-only should match non-editable elements");
    }
    #endregion

    #region :read-write Tests
    [Fact(Skip = SkipReason)]
    public void ReadWrite_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":read-write");

        // Assert
        Assert.True(selector.Count > 0, ":read-write selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void ReadWrite_MatchesEditableInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector(":read-write");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":read-write should match editable input");
    }

    [Fact(Skip = SkipReason)]
    public void ReadWrite_MatchesContentEditable()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        div.setAttribute("contenteditable", AttributeValue.From("true"));
        var selector = new CssSelector(":read-write");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), ":read-write should match contenteditable element");
    }
    #endregion

    #region :placeholder-shown Tests
    [Fact(Skip = SkipReason)]
    public void PlaceholderShown_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":placeholder-shown");

        // Assert
        Assert.True(selector.Count > 0, ":placeholder-shown selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void PlaceholderShown_MatchesEmptyInputWithPlaceholder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("placeholder", AttributeValue.From("Enter text..."));
        var selector = new CssSelector(":placeholder-shown");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":placeholder-shown should match empty input with placeholder");
    }

    [Fact(Skip = SkipReason)]
    public void PlaceholderShown_DoesNotMatchFilledInput()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("placeholder", AttributeValue.From("Enter text..."));
        input.setAttribute("value", AttributeValue.From("Some text"));
        var selector = new CssSelector(":placeholder-shown");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(input), ":placeholder-shown should not match filled input");
    }
    #endregion

    #region :default Tests
    [Fact(Skip = SkipReason)]
    public void Default_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector(":default");

        // Assert
        Assert.True(selector.Count > 0, ":default selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Default_MatchesDefaultButton()
    {
        // Arrange
        var doc = CreateTestDocument();
        var form = CreateTestElement(doc, "form");
        var button = CreateTestElement(doc, "button");
        button.setAttribute("type", AttributeValue.From("submit"));
        form.appendChild(button);
        var selector = new CssSelector(":default");

        // Act & Assert - First submit button in a form is the default
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(button), ":default should match default submit button");
    }

    [Fact(Skip = SkipReason)]
    public void Default_MatchesDefaultSelectedOption()
    {
        // Arrange
        var doc = CreateTestDocument();
        var option = CreateTestElement(doc, "option");
        option.setAttribute("selected", AttributeValue.From(""));
        var selector = new CssSelector(":default");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(option), ":default should match default selected option");
    }
    #endregion

    #region Combined Input Pseudo-classes
    [Fact(Skip = SkipReason)]
    public void EnabledAndRequired_MatchesBoth()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        input.setAttribute("required", AttributeValue.From(""));
        var selector = new CssSelector(":enabled:required");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), ":enabled:required should match enabled required input");
    }

    [Fact(Skip = SkipReason)]
    public void NotDisabled_MatchesEnabledElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var input = CreateTestElement(doc, "input");
        var selector = new CssSelector("input:not(:disabled)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(input), "input:not(:disabled) should match enabled input");
    }
    #endregion
}
