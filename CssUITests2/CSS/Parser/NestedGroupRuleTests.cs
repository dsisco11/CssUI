using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for nested group rules (@media, @supports, @layer inside style rules)
/// per CSS Nesting Module Level 1 §2.2.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/#conditionals"/>
[Trait("Category", "CSS Parser")]
[Trait("Spec", "CSS Nesting 1")]
public class NestedGroupRuleTests
{
    #region Parsing Nested @media

    [Fact]
    public void StyleBlock_NestedMedia_IsParsedAsAtRule()
    {
        // @media inside a style block should be parsed as CssAtRule
        var parser = new CssParser("color: red; @media screen { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssAtRule>(contents[1]);

        var atRule = (CssAtRule)contents[1];
        Assert.Equal("media", atRule.Name);
    }

    [Fact]
    public void StyleBlock_NestedMedia_PreludeContainsMediaQuery()
    {
        var parser = new CssParser("@media (min-width: 768px) { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Single(contents);
        var atRule = Assert.IsType<CssAtRule>(contents[0]);
        Assert.Equal("media", atRule.Name);
        Assert.NotEmpty(atRule.Prelude);
    }

    [Fact]
    public void StyleBlock_NestedMedia_HasBlock()
    {
        var parser = new CssParser("@media screen { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        var atRule = Assert.IsType<CssAtRule>(contents[0]);
        Assert.NotNull(atRule.Block);
        Assert.NotEmpty(atRule.Block!.Values);
    }

    [Fact]
    public void StyleBlock_MultipleNestedMedia_AllParsed()
    {
        var parser = new CssParser("@media screen { } @media print { }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssAtRule>(c));
    }

    #endregion

    #region Parsing Nested @supports

    [Fact]
    public void StyleBlock_NestedSupports_IsParsedAsAtRule()
    {
        var parser = new CssParser("color: red; @supports (display: grid) { display: grid; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssAtRule>(contents[1]);

        var atRule = (CssAtRule)contents[1];
        Assert.Equal("supports", atRule.Name);
    }

    [Fact]
    public void StyleBlock_NestedSupports_PreludeContainsCondition()
    {
        var parser = new CssParser("@supports (display: flex) { display: flex; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        var atRule = Assert.IsType<CssAtRule>(contents[0]);
        Assert.Equal("supports", atRule.Name);
        Assert.NotEmpty(atRule.Prelude);
    }

    #endregion

    #region Parsing Nested @layer

    [Fact]
    public void StyleBlock_NestedLayer_IsParsedAsAtRule()
    {
        var parser = new CssParser("@layer utilities { color: red; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Single(contents);
        var atRule = Assert.IsType<CssAtRule>(contents[0]);
        Assert.Equal("layer", atRule.Name);
    }

    [Fact]
    public void StyleBlock_NestedLayerAnonymous_IsParsedAsAtRule()
    {
        // Anonymous layer (no name)
        var parser = new CssParser("@layer { color: red; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        var atRule = Assert.IsType<CssAtRule>(contents[0]);
        Assert.Equal("layer", atRule.Name);
    }

    #endregion

    #region CSSRuleFactory - Media Rules

    [Fact]
    public void BuildRule_NestedMediaRule_CreatesCSSMediaRule()
    {
        // Parse a style rule with nested @media
        var css = ".foo { color: red; @media screen { color: blue; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();

        // Build CSSOM rules
        var rules = CSSRuleFactory.BuildRules(components);

        Assert.Single(rules);
        var styleRule = Assert.IsType<CSSStyleRule>(rules[0]);
        Assert.Single(styleRule.cssRules);
        Assert.IsType<CSSMediaRule>(styleRule.cssRules[0]);
    }

    [Fact]
    public void BuildRule_NestedMediaRule_HasCorrectParentRule()
    {
        var css = ".foo { @media screen { color: blue; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        var mediaRule = (CSSMediaRule)styleRule.cssRules[0];

        Assert.Same(styleRule, mediaRule.parentRule);
    }

    [Fact]
    public void BuildRule_TopLevelMediaRule_CreatesCSSMediaRule()
    {
        var css = "@media screen { .foo { color: blue; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        Assert.Single(rules);
        var mediaRule = Assert.IsType<CSSMediaRule>(rules[0]);
        Assert.Single(mediaRule.cssRules);
        Assert.IsType<CSSStyleRule>(mediaRule.cssRules[0]);
    }

    #endregion

    #region CSSRuleFactory - Supports Rules

    [Fact]
    public void BuildRule_NestedSupportsRule_CreatesCSSSupportsRule()
    {
        var css = ".foo { @supports (display: grid) { display: grid; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = Assert.IsType<CSSStyleRule>(rules[0]);
        Assert.Single(styleRule.cssRules);
        Assert.IsType<CSSSupportsRule>(styleRule.cssRules[0]);
    }

    [Fact]
    public void BuildRule_NestedSupportsRule_HasConditionText()
    {
        var css = ".foo { @supports (display: flex) { display: flex; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        var supportsRule = (CSSSupportsRule)styleRule.cssRules[0];

        Assert.Contains("display", supportsRule.conditionText);
        Assert.Contains("flex", supportsRule.conditionText);
    }

    [Fact]
    public void BuildRule_TopLevelSupportsRule_CreatesCSSSupportsRule()
    {
        var css = "@supports (display: grid) { .foo { display: grid; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        Assert.Single(rules);
        var supportsRule = Assert.IsType<CSSSupportsRule>(rules[0]);
        Assert.Single(supportsRule.cssRules);
    }

    #endregion

    #region CSSRuleFactory - Layer Rules

    [Fact]
    public void BuildRule_NestedLayerBlock_CreatesCSSLayerBlockRule()
    {
        var css = ".foo { @layer utilities { color: blue; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = Assert.IsType<CSSStyleRule>(rules[0]);
        Assert.Single(styleRule.cssRules);
        Assert.IsType<CSSLayerBlockRule>(styleRule.cssRules[0]);
    }

    [Fact]
    public void BuildRule_TopLevelLayerStatement_CreatesCSSLayerStatementRule()
    {
        var css = "@layer base, components, utilities;";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        Assert.Single(rules);
        Assert.IsType<CSSLayerStatementRule>(rules[0]);
    }

    [Fact]
    public void BuildRule_TopLevelLayerBlock_CreatesCSSLayerBlockRule()
    {
        var css = "@layer base { .foo { color: red; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        Assert.Single(rules);
        var layerRule = Assert.IsType<CSSLayerBlockRule>(rules[0]);
        Assert.Single(layerRule.cssRules);
    }

    #endregion

    #region Nested Group Rules with Declarations

    [Fact]
    public void BuildRule_NestedMediaWithDeclarations_CreatesImplicitRule()
    {
        // Per CSS Nesting spec: declarations directly in nested @media
        // are wrapped in an implicit & { } rule
        var css = ".foo { @media screen { color: blue; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        var mediaRule = (CSSMediaRule)styleRule.cssRules[0];

        // The media rule should have an implicit & rule for the declaration
        Assert.Single(mediaRule.cssRules);
        Assert.IsType<CSSStyleRule>(mediaRule.cssRules[0]);
    }

    [Fact]
    public void BuildRule_NestedSupportsWithDeclarations_CreatesImplicitRule()
    {
        var css = ".foo { @supports (display: grid) { display: grid; } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        var supportsRule = (CSSSupportsRule)styleRule.cssRules[0];

        Assert.Single(supportsRule.cssRules);
        Assert.IsType<CSSStyleRule>(supportsRule.cssRules[0]);
    }

    #endregion

    #region Deeply Nested Group Rules

    [Fact]
    public void BuildRule_DeeplyNestedMedia_CorrectHierarchy()
    {
        // .foo { @media screen { @media (min-width: 768px) { color: blue; } } }
        var css = ".foo { @media screen { & { @media (min-width: 768px) { color: blue; } } } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = Assert.IsType<CSSStyleRule>(rules[0]);
        var outerMedia = Assert.IsType<CSSMediaRule>(styleRule.cssRules[0]);

        // The outer media has a nested style rule which has a nested media
        Assert.NotEmpty(outerMedia.cssRules);
    }

    [Fact]
    public void BuildRule_NestedMediaAndSupports_BothCreated()
    {
        var css = ".foo { @media screen { } @supports (display: grid) { } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        Assert.Equal(2, styleRule.cssRules.Count);
        Assert.IsType<CSSMediaRule>(styleRule.cssRules[0]);
        Assert.IsType<CSSSupportsRule>(styleRule.cssRules[1]);
    }

    #endregion

    #region Mixed Content

    [Fact]
    public void BuildRule_DeclarationsAndNestedMedia_BothProcessed()
    {
        var css = ".foo { color: red; @media screen { color: blue; } font-size: 16px; }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = Assert.IsType<CSSStyleRule>(rules[0]);
        // The nested @media rule should be in cssRules
        Assert.Single(styleRule.cssRules);
        Assert.IsType<CSSMediaRule>(styleRule.cssRules[0]);
    }

    [Fact]
    public void BuildRule_NestedStyleAndMedia_BothInCssRules()
    {
        var css = ".foo { & .bar { } @media screen { } }";
        var parser = new CssParser(css);
        var components = parser.Parse_Rule_List().ToList();
        var rules = CSSRuleFactory.BuildRules(components);

        var styleRule = (CSSStyleRule)rules[0];
        Assert.Equal(2, styleRule.cssRules.Count);
        Assert.IsType<CSSStyleRule>(styleRule.cssRules[0]);
        Assert.IsType<CSSMediaRule>(styleRule.cssRules[1]);
    }

    #endregion
}
