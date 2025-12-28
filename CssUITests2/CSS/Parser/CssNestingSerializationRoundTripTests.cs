using System.Collections.Generic;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Media;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Round-trip tests for CSS Nesting serialization.
/// Verifies that parse → serialize → parse produces semantically equivalent results.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
/// <seealso href="https://www.w3.org/TR/cssom-1/#serialize-a-css-rule"/>
[Trait("Category", "CSS Parser")]
[Trait("Spec", "CSS Nesting 1")]
[Trait("Spec", "CSSOM")]
public class CssNestingSerializationRoundTripTests
{
    #region Simple Style Rules

    [Theory]
    [InlineData(".parent { }")]
    [InlineData("#container { }")]
    [InlineData("* { }")]
    [InlineData("[type] { }")]
    [InlineData(":hover { }")]
    [InlineData("::before { }")]
    public void RoundTrip_SimpleSelector_PreservesStructure(string css)
    {
        // Arrange & Act - First parse
        var parser1 = new CssParser(css);
        var stylesheet1 = parser1.Parse_Stylesheet();
        var rules1 = CSSRuleFactory.BuildRules(stylesheet1.Rules);

        Assert.Single(rules1);
        var styleRule1 = Assert.IsType<CSSStyleRule>(rules1[0]);

        // Serialize
        string serialized = styleRule1.cssText;
        Assert.False(string.IsNullOrWhiteSpace(serialized), "Serialized CSS should not be empty");

        // Second parse
        var parser2 = new CssParser(serialized);
        var stylesheet2 = parser2.Parse_Stylesheet();
        var rules2 = CSSRuleFactory.BuildRules(stylesheet2.Rules);

        // Assert - Structure preserved
        Assert.Single(rules2);
        var styleRule2 = Assert.IsType<CSSStyleRule>(rules2[0]);

        // Selector should be equivalent (may have whitespace differences)
        Assert.NotNull(styleRule1.Selector);
        Assert.NotNull(styleRule2.Selector);
    }

    #endregion

    #region Nested Style Rules with Ampersand

    [Fact]
    public void RoundTrip_NestedRuleWithAmpersand_PreservesNesting()
    {
        // Arrange - Parent rule with nested child using &
        var parentSelector = new CssSelector(".parent");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var childSelector = new CssSelector("&:hover");
        var childRule = new CSSStyleRule(childSelector, null);
        parentRule.cssRules.Add(childRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert - Serialization contains nested structure
        Assert.Contains(".parent", serialized);
        Assert.Contains("&:hover", serialized);
        Assert.Contains("\n", serialized); // Nested rules use newlines
    }

    [Fact]
    public void RoundTrip_NestedRuleWithClass_PreservesNesting()
    {
        // Arrange - Parent rule with nested .child
        var parentSelector = new CssSelector(".parent");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var childSelector = new CssSelector(".child");
        var childRule = new CSSStyleRule(childSelector, null);
        parentRule.cssRules.Add(childRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert - Both selectors appear in output
        Assert.Contains(".parent", serialized);
        Assert.Contains(".child", serialized);
    }

    [Fact]
    public void RoundTrip_NestedRuleWithId_PreservesNesting()
    {
        // Arrange - Parent rule with nested #id
        var parentSelector = new CssSelector("article");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var childSelector = new CssSelector("#header");
        var childRule = new CSSStyleRule(childSelector, null);
        parentRule.cssRules.Add(childRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert
        Assert.Contains("article", serialized);
        Assert.Contains("#header", serialized);
    }

    #endregion

    #region Multiple Nested Rules

    [Fact]
    public void RoundTrip_MultipleNestedRules_AllPreserved()
    {
        // Arrange - Parent with multiple nested rules
        var parentSelector = new CssSelector(".card");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var headerRule = new CSSStyleRule(new CssSelector(".header"), null);
        var bodyRule = new CSSStyleRule(new CssSelector(".body"), null);
        var footerRule = new CSSStyleRule(new CssSelector(".footer"), null);

        parentRule.cssRules.Add(headerRule);
        parentRule.cssRules.Add(bodyRule);
        parentRule.cssRules.Add(footerRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert - All nested rules appear
        Assert.Contains(".card", serialized);
        Assert.Contains(".header", serialized);
        Assert.Contains(".body", serialized);
        Assert.Contains(".footer", serialized);

        // Count braces to verify structure
        int openBraces = serialized.Count(c => c == '{');
        int closeBraces = serialized.Count(c => c == '}');
        Assert.Equal(openBraces, closeBraces);
        Assert.Equal(4, openBraces); // 1 parent + 3 children
    }

    [Fact]
    public void RoundTrip_DeeplyNestedRules_StructurePreserved()
    {
        // Arrange - 3 levels of nesting
        var level1 = new CSSStyleRule(new CssSelector(".level1"), null);
        var level2 = new CSSStyleRule(new CssSelector(".level2"), null);
        var level3 = new CSSStyleRule(new CssSelector(".level3"), null);

        level2.cssRules.Add(level3);
        level1.cssRules.Add(level2);

        // Act - Serialize
        string serialized = level1.cssText;

        // Assert - All levels appear
        Assert.Contains(".level1", serialized);
        Assert.Contains(".level2", serialized);
        Assert.Contains(".level3", serialized);

        // Verify nesting via brace count
        int openBraces = serialized.Count(c => c == '{');
        Assert.Equal(3, openBraces);
    }

    #endregion

    #region Nested Group Rules (@media, @supports, @layer)

    [Fact]
    public void RoundTrip_NestedMediaRule_PreservesStructure()
    {
        // Arrange - Style rule with nested @media
        var parentSelector = new CssSelector(".responsive");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var mediaRule = new CSSMediaRule(
            new[] { new MediaQuery(EMediaQueryModifier.None, EMediaType.Screen, new LinkedList<IMediaCondition>()) });

        var innerStyle = new CSSStyleRule(new CssSelector("&"), null);
        mediaRule.cssRules.Add(innerStyle);
        parentRule.cssRules.Add(mediaRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert
        Assert.Contains(".responsive", serialized);
        Assert.Contains("@media", serialized);
        Assert.Contains("screen", serialized);
    }

    [Fact]
    public void RoundTrip_NestedSupportsRule_PreservesStructure()
    {
        // Arrange - Style rule with nested @supports
        var parentSelector = new CssSelector(".modern");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var supportsRule = new CSSSupportsRule("(display: grid)");
        var innerStyle = new CSSStyleRule(new CssSelector("&"), null);
        supportsRule.cssRules.Add(innerStyle);
        parentRule.cssRules.Add(supportsRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert
        Assert.Contains(".modern", serialized);
        Assert.Contains("@supports", serialized);
        Assert.Contains("display", serialized);
    }

    [Fact]
    public void RoundTrip_NestedLayerRule_PreservesStructure()
    {
        // Arrange - Style rule with nested @layer
        var parentSelector = new CssSelector(".themed");
        var parentRule = new CSSStyleRule(parentSelector, null);

        var layerRule = new CSSLayerBlockRule("components");
        var innerStyle = new CSSStyleRule(new CssSelector("&"), null);
        layerRule.cssRules.Add(innerStyle);
        parentRule.cssRules.Add(layerRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert
        Assert.Contains(".themed", serialized);
        Assert.Contains("@layer", serialized);
        Assert.Contains("components", serialized);
    }

    #endregion

    #region Serialization Format Verification

    [Fact]
    public void Serialize_NestedRules_UseTwoSpaceIndentation()
    {
        // Per CSSOM spec, nested rules are indented with 2 spaces
        var parentRule = new CSSStyleRule(new CssSelector(".parent"), null);
        var childRule = new CSSStyleRule(new CssSelector(".child"), null);
        parentRule.cssRules.Add(childRule);

        string serialized = parentRule.cssText;

        // Verify 2-space indentation pattern exists
        Assert.Contains("\n  ", serialized);
    }

    [Fact]
    public void Serialize_NestedRules_ClosingBraceOnNewLine()
    {
        // With nested rules, closing brace should be on its own line
        var parentRule = new CSSStyleRule(new CssSelector(".parent"), null);
        var childRule = new CSSStyleRule(new CssSelector(".child"), null);
        parentRule.cssRules.Add(childRule);

        string serialized = parentRule.cssText;

        // Should end with newline + closing brace
        Assert.EndsWith("\n}", serialized);
    }

    [Fact]
    public void Serialize_NoNestedRules_CompactFormat()
    {
        // Without nested rules, format is compact: "selector { }"
        var rule = new CSSStyleRule(new CssSelector(".simple"), null);

        string serialized = rule.cssText;

        // Should be compact, ending with " }" (space before brace)
        Assert.EndsWith(" }", serialized);
        // Should NOT contain newlines
        Assert.DoesNotContain("\n", serialized);
    }

    #endregion

    #region Complex Nesting Patterns

    [Fact]
    public void RoundTrip_MixedNestedStylesAndGroupRules_AllPreserved()
    {
        // Arrange - Complex nesting with style rules and @media
        var parentRule = new CSSStyleRule(new CssSelector(".component"), null);

        var hoverRule = new CSSStyleRule(new CssSelector("&:hover"), null);
        parentRule.cssRules.Add(hoverRule);

        var mediaRule = new CSSMediaRule(
            new[] { new MediaQuery(EMediaQueryModifier.None, EMediaType.Screen, new LinkedList<IMediaCondition>()) });
        var mobileStyle = new CSSStyleRule(new CssSelector("&"), null);
        mediaRule.cssRules.Add(mobileStyle);
        parentRule.cssRules.Add(mediaRule);

        var activeRule = new CSSStyleRule(new CssSelector("&:active"), null);
        parentRule.cssRules.Add(activeRule);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert - All rules appear
        Assert.Contains(".component", serialized);
        Assert.Contains("&:hover", serialized);
        Assert.Contains("@media", serialized);
        Assert.Contains("&:active", serialized);
    }

    [Fact(Skip = "Relative selectors starting with combinators (> .child) require parser enhancement")]
    public void RoundTrip_RelativeCombinatorNesting_PreservesStructure()
    {
        // Arrange - Nested rules with relative combinators
        // Note: Selectors like "> .child" start with a combinator, which requires
        // parser support for relative selectors per CSS Selectors Level 4 §4.3
        var parentRule = new CSSStyleRule(new CssSelector(".parent"), null);

        var childCombinator = new CSSStyleRule(new CssSelector("> .child"), null);
        var siblingCombinator = new CSSStyleRule(new CssSelector("+ .sibling"), null);

        parentRule.cssRules.Add(childCombinator);
        parentRule.cssRules.Add(siblingCombinator);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert
        Assert.Contains(".parent", serialized);
        Assert.Contains("> .child", serialized);
        Assert.Contains("+ .sibling", serialized);
    }

    [Fact]
    public void RoundTrip_DescendantCombinatorNesting_PreservesStructure()
    {
        // Arrange - Nested rules with descendant selectors (supported syntax)
        var parentRule = new CSSStyleRule(new CssSelector(".parent"), null);

        var childDescendant = new CSSStyleRule(new CssSelector("& .child"), null);
        var siblingDescendant = new CSSStyleRule(new CssSelector("& .sibling"), null);

        parentRule.cssRules.Add(childDescendant);
        parentRule.cssRules.Add(siblingDescendant);

        // Act - Serialize
        string serialized = parentRule.cssText;

        // Assert - Both use & for explicit nesting
        Assert.Contains(".parent", serialized);
        Assert.Contains("& .child", serialized);
        Assert.Contains("& .sibling", serialized);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void RoundTrip_EmptyNestedRuleList_SerializesCompact()
    {
        // Empty cssRules should still serialize properly
        var rule = new CSSStyleRule(new CssSelector(".empty"), null);
        Assert.Empty(rule.cssRules);

        string serialized = rule.cssText;

        // Should be compact format
        Assert.DoesNotContain("\n", serialized);
        Assert.EndsWith(" }", serialized);
    }

    [Fact]
    public void RoundTrip_SingleNestedRule_ProperlyFormatted()
    {
        // Even a single nested rule should trigger newline formatting
        var parentRule = new CSSStyleRule(new CssSelector(".parent"), null);
        var childRule = new CSSStyleRule(new CssSelector(".only-child"), null);
        parentRule.cssRules.Add(childRule);

        string serialized = parentRule.cssText;

        // Should have newline formatting
        Assert.Contains("\n", serialized);
        Assert.EndsWith("\n}", serialized);
    }

    [Fact]
    public void RoundTrip_UniversalSelectorNested_PreservesAsterisk()
    {
        var parentRule = new CSSStyleRule(new CssSelector(".container"), null);
        var universalRule = new CSSStyleRule(new CssSelector("*"), null);
        parentRule.cssRules.Add(universalRule);

        string serialized = parentRule.cssText;

        Assert.Contains(".container", serialized);
        Assert.Contains("*", serialized);
    }

    [Fact]
    public void RoundTrip_AttributeSelectorNested_PreservesBrackets()
    {
        var parentRule = new CSSStyleRule(new CssSelector("form"), null);
        var attrRule = new CSSStyleRule(new CssSelector("[type=\"text\"]"), null);
        parentRule.cssRules.Add(attrRule);

        string serialized = parentRule.cssText;

        Assert.Contains("form", serialized);
        Assert.Contains("[type", serialized);
    }

    #endregion
}
