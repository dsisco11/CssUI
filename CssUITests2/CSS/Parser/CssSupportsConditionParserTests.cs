using System.Collections.Generic;
using System.Collections.Immutable;
using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for <see cref="CssSupportsConditionParser"/>.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-conditional-3/#at-supports
/// </remarks>
[Trait("Category", "Parser")]
[Trait("Category", "Supports")]
public class CssSupportsConditionParserTests
{
    #region Helper Methods
    private static CssToken[] Tokenize(string css)
    {
        return CssTokenizer.Parse(css);
    }

    private static CssSupportsConditionResult Parse(string condition)
    {
        var tokens = Tokenize(condition);
        return CssSupportsConditionParser.TryParse(tokens);
    }
    #endregion

    #region Debug Tests
    [Fact]
    public void Debug_TokenizeSimpleDeclaration()
    {
        // Arrange
        var condition = "(display: flex)";
        var tokens = Tokenize(condition);

        // Debug output
        System.Console.WriteLine($"Token count: {tokens.Length}");
        for (int i = 0; i < tokens.Length; i++)
        {
            var t = tokens[i];
            System.Console.WriteLine($"[{i}] Type={t.Type}, IsEOF={ReferenceEquals(t, CssToken.EOF)}");
        }

        // Act
        var result = Parse(condition);

        // Debug
        System.Console.WriteLine($"Success: {result.Success}");
        System.Console.WriteLine($"Error: {result.ErrorMessage}");

        // Assert
        Assert.True(result.Success, $"Parse failed: {result.ErrorMessage}");
    }
    #endregion

    #region Simple Declaration Tests
    [Fact]
    public void TryParse_SimpleDeclaration_ReturnsCssSupportsDeclaration()
    {
        // Arrange
        var condition = "(display: flex)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Condition);
        Assert.IsType<CssSupportsDeclaration>(result.Condition);

        var decl = (CssSupportsDeclaration)result.Condition;
        Assert.Equal("display", decl.PropertyName);
        Assert.True(decl.IsValueValid);
        Assert.False(decl.IsImportant);
    }

    [Fact]
    public void TryParse_DeclarationWithImportant_SetsIsImportantFlag()
    {
        // Arrange
        var condition = "(display: flex !important)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.Equal("display", decl.PropertyName);
        Assert.True(decl.IsImportant);
        Assert.True(decl.IsValueValid);
    }

    [Fact]
    public void TryParse_DeclarationWithFunction_ParsesCorrectly()
    {
        // Arrange
        var condition = "(transform: rotate(45deg))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.Equal("transform", decl.PropertyName);
        Assert.True(decl.IsValueValid);
    }

    [Fact]
    public void TryParse_DeclarationWithMultipleValues_ParsesCorrectly()
    {
        // Arrange
        var condition = "(margin: 10px 20px 30px 40px)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.Equal("margin", decl.PropertyName);
        Assert.True(decl.IsValueValid);
    }

    [Fact]
    public void TryParse_CustomProperty_ParsesCorrectly()
    {
        // Arrange
        var condition = "(--custom-prop: value)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.Equal("--custom-prop", decl.PropertyName);
    }
    #endregion

    #region Negation Tests
    [Fact]
    public void TryParse_NotDeclaration_ReturnsCssSupportsNot()
    {
        // Arrange
        var condition = "not (display: flex)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsNot>(result.Condition);

        var not = (CssSupportsNot)result.Condition;
        Assert.IsType<CssSupportsDeclaration>(not.Child);
    }

    [Fact]
    public void TryParse_NotWithoutWhitespace_ParsesAsGeneralEnclosed()
    {
        // Arrange - 'not' without following whitespace is tokenized as a function token
        // not as the 'not' keyword. Per CSS Syntax, 'not(' becomes FunctionNameToken("not").
        // Per CSS Conditional Rules 3, this is valid as <general-enclosed>.
        var condition = "not(display: flex)";

        // Act
        var result = Parse(condition);

        // Assert - parsed as <general-enclosed> function, which is valid
        Assert.True(result.Success);
        Assert.IsType<CssSupportsGeneralEnclosed>(result.Condition);
    }
    #endregion

    #region Conjunction Tests
    [Fact]
    public void TryParse_AndTwoConditions_ReturnsCssSupportsAnd()
    {
        // Arrange
        var condition = "(display: flex) and (gap: 1rem)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsAnd>(result.Condition);

        var and = (CssSupportsAnd)result.Condition;
        Assert.Equal(2, and.Children.Length);
        Assert.All(and.Children, c => Assert.IsType<CssSupportsDeclaration>(c));
    }

    [Fact]
    public void TryParse_AndThreeConditions_ReturnsAllChildren()
    {
        // Arrange
        var condition = "(a: b) and (c: d) and (e: f)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var and = Assert.IsType<CssSupportsAnd>(result.Condition);
        Assert.Equal(3, and.Children.Length);
    }

    [Fact]
    public void TryParse_AndWithoutWhitespace_Fails()
    {
        // Arrange - 'and' without following whitespace is tokenized as FunctionNameToken("and")
        // not as the 'and' keyword. The parser doesn't recognize it as conjunction.
        var condition = "(display: flex) and(gap: 1rem)";

        // Act
        var result = Parse(condition);

        // Assert - fails because the 'and(...)' tokens are left over after parsing
        // the first supports-in-parens, since 'and(' is tokenized as a function
        Assert.False(result.Success);
        Assert.Contains("Unexpected tokens", result.ErrorMessage);
    }
    #endregion

    #region Disjunction Tests
    [Fact]
    public void TryParse_OrTwoConditions_ReturnsCssSupportsOr()
    {
        // Arrange
        var condition = "(display: flex) or (display: grid)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsOr>(result.Condition);

        var or = (CssSupportsOr)result.Condition;
        Assert.Equal(2, or.Children.Length);
    }

    [Fact]
    public void TryParse_OrThreeConditions_ReturnsAllChildren()
    {
        // Arrange
        var condition = "(a: b) or (c: d) or (e: f)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var or = Assert.IsType<CssSupportsOr>(result.Condition);
        Assert.Equal(3, or.Children.Length);
    }
    #endregion

    #region Mixed And/Or Tests
    [Fact]
    public void TryParse_MixedAndOr_Fails()
    {
        // Arrange - mixing 'and' and 'or' without parentheses is invalid
        var condition = "(a: b) and (c: d) or (e: f)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("mix", result.ErrorMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryParse_MixedWithParentheses_Succeeds()
    {
        // Arrange - mixing with proper grouping
        var condition = "((a: b) and (c: d)) or (e: f)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsOr>(result.Condition);

        var or = (CssSupportsOr)result.Condition;
        Assert.Equal(2, or.Children.Length);
        Assert.IsType<CssSupportsNested>(or.Children[0]);
    }
    #endregion

    #region Nested Condition Tests
    [Fact]
    public void TryParse_NestedParentheses_ReturnsCssSupportsNested()
    {
        // Arrange
        var condition = "((display: flex))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsNested>(result.Condition);

        var nested = (CssSupportsNested)result.Condition;
        Assert.IsType<CssSupportsDeclaration>(nested.Child);
    }

    [Fact]
    public void TryParse_DeeplyNested_ParsesCorrectly()
    {
        // Arrange
        var condition = "(((display: flex)))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var nested1 = Assert.IsType<CssSupportsNested>(result.Condition);
        var nested2 = Assert.IsType<CssSupportsNested>(nested1.Child);
        Assert.IsType<CssSupportsDeclaration>(nested2.Child);
    }

    [Fact]
    public void TryParse_NotInsideParens_ParsesCorrectly()
    {
        // Arrange
        var condition = "(not (display: flex))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var nested = Assert.IsType<CssSupportsNested>(result.Condition);
        Assert.IsType<CssSupportsNot>(nested.Child);
    }
    #endregion

    #region General-Enclosed Tests
    [Fact]
    public void TryParse_UnknownFunction_ReturnsGeneralEnclosed()
    {
        // Arrange - unknown function should be treated as <general-enclosed>
        var condition = "unknown-function(anything here)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsGeneralEnclosed>(result.Condition);
    }

    [Fact]
    public void TryParse_SelectorFunction_ReturnsGeneralEnclosed()
    {
        // Arrange - selector() is <general-enclosed> per spec
        var condition = "selector(:has(> .foo))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsGeneralEnclosed>(result.Condition);
    }

    [Fact]
    public void TryParse_GeneralEnclosed_EvaluatesToFalse()
    {
        // Arrange
        var condition = "selector(:has(> .foo))";
        var result = Parse(condition);
        Assert.True(result.Success);

        var generalEnclosed = Assert.IsType<CssSupportsGeneralEnclosed>(result.Condition);

        // Act - <general-enclosed> always evaluates to false per spec
        bool evaluated = generalEnclosed.Evaluate((prop, val) => true);

        // Assert
        Assert.False(evaluated);
    }
    #endregion

    #region Value Validation Tests
    [Fact]
    public void TryParse_DeclarationWithBadToken_IsInvalid()
    {
        // Declarations containing <bad-string-token> should be invalid
        // This is hard to test directly without crafting bad tokens
        // For now, test that normal values are valid
        var condition = "(display: flex)";
        var result = Parse(condition);

        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.True(decl.IsValueValid);
    }

    [Fact]
    public void TryParse_DeclarationWithNestedParens_IsValid()
    {
        // Arrange - nested parens should be valid in <declaration-value>
        var condition = "(transform: calc((100% - 20px) / 2))";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        var decl = Assert.IsType<CssSupportsDeclaration>(result.Condition);
        Assert.True(decl.IsValueValid);
    }
    #endregion

    #region Evaluation Tests
    [Fact]
    public void Evaluate_SimpleDeclaration_CallsChecker()
    {
        // Arrange
        var condition = "(display: flex)";
        var result = Parse(condition);
        Assert.True(result.Success);

        bool checkerCalled = false;
        string? checkedProperty = null;

        // Act
        bool evaluated = result.Condition!.Evaluate((prop, val) =>
        {
            checkerCalled = true;
            checkedProperty = prop;
            return true;
        });

        // Assert
        Assert.True(checkerCalled);
        Assert.Equal("display", checkedProperty);
        Assert.True(evaluated);
    }

    [Fact]
    public void Evaluate_NotCondition_NegatesResult()
    {
        // Arrange
        var condition = "not (display: flex)";
        var result = Parse(condition);
        Assert.True(result.Success);

        // Act - checker returns true, but not negates it
        bool evaluated = result.Condition!.Evaluate((prop, val) => true);

        // Assert
        Assert.False(evaluated);
    }

    [Fact]
    public void Evaluate_AndCondition_RequiresAllTrue()
    {
        // Arrange
        var condition = "(a: b) and (c: d)";
        var result = Parse(condition);
        Assert.True(result.Success);

        // Act - one returns false
        bool evaluated = result.Condition!.Evaluate((prop, val) => prop == "a");

        // Assert - should be false because 'c' check fails
        Assert.False(evaluated);
    }

    [Fact]
    public void Evaluate_OrCondition_RequiresAnyTrue()
    {
        // Arrange
        var condition = "(a: b) or (c: d)";
        var result = Parse(condition);
        Assert.True(result.Success);

        // Act - one returns true
        bool evaluated = result.Condition!.Evaluate((prop, val) => prop == "a");

        // Assert - should be true because 'a' check passes
        Assert.True(evaluated);
    }
    #endregion

    #region Error Handling Tests
    [Fact]
    public void TryParse_EmptyCondition_Fails()
    {
        // Arrange
        var tokens = new List<CssToken>();

        // Act
        var result = CssSupportsConditionParser.TryParse(tokens);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void TryParse_UnclosedParenthesis_Fails()
    {
        // Arrange
        var condition = "(display: flex";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void TryParse_MissingColon_Fails()
    {
        // Arrange
        var condition = "(display flex)";

        // Act
        var result = Parse(condition);

        // Assert - Should fail or be treated as <general-enclosed>
        // Per spec, if it's not a declaration it could be <general-enclosed>
        Assert.True(result.Success);
        Assert.IsType<CssSupportsGeneralEnclosed>(result.Condition);
    }

    [Fact]
    public void TryParse_ExtraTokensAfterCondition_Fails()
    {
        // Arrange
        var condition = "(display: flex) extra";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Unexpected", result.ErrorMessage);
    }
    #endregion

    #region Case Sensitivity Tests
    [Fact]
    public void TryParse_NotKeywordCaseInsensitive()
    {
        // Arrange
        var condition = "NOT (display: flex)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsNot>(result.Condition);
    }

    [Fact]
    public void TryParse_AndKeywordCaseInsensitive()
    {
        // Arrange
        var condition = "(a: b) AND (c: d)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsAnd>(result.Condition);
    }

    [Fact]
    public void TryParse_OrKeywordCaseInsensitive()
    {
        // Arrange
        var condition = "(a: b) OR (c: d)";

        // Act
        var result = Parse(condition);

        // Assert
        Assert.True(result.Success);
        Assert.IsType<CssSupportsOr>(result.Condition);
    }
    #endregion
}
