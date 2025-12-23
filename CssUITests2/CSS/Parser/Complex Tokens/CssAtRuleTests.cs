using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssAtRule - represents a CSS at-rule like @media, @import, @charset, etc.
/// </summary>
public class CssAtRuleTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Constructor_WithName_SetsNameProperty()
    {
        var atRule = new CssAtRule("media".AsSpan());
        Assert.Equal("media", atRule.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Constructor_WithName_SetsTypeToAtRule()
    {
        var atRule = new CssAtRule("import".AsSpan());
        Assert.Equal(ECssTokenType.AtRule, atRule.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Constructor_BlockDefaultsToNull()
    {
        var atRule = new CssAtRule("charset".AsSpan());
        Assert.Null(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Constructor_PreludeDefaultsToEmpty()
    {
        var atRule = new CssAtRule("media".AsSpan());
        Assert.Empty(atRule.Prelude);
    }
    #endregion

    #region Prelude Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Prelude_CanAddSingleToken()
    {
        var atRule = new CssAtRule("charset".AsSpan());
        atRule.Prelude.Add(new StringToken("UTF-8"));

        Assert.Single(atRule.Prelude);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Prelude_CanAddMultipleTokens()
    {
        var atRule = new CssAtRule("media".AsSpan());
        atRule.Prelude.Add(new IdentToken("screen"));
        atRule.Prelude.Add(WhitespaceToken.Space);
        atRule.Prelude.Add(new IdentToken("and"));
        atRule.Prelude.Add(WhitespaceToken.Space);

        Assert.Equal(4, atRule.Prelude.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Prelude_CanContainFunction()
    {
        var atRule = new CssAtRule("import".AsSpan());
        var func = new CssFunction("url".AsSpan());
        func.Arguments.Add(new StringToken("styles.css"));
        atRule.Prelude.Add(func);

        Assert.Single(atRule.Prelude);
        Assert.IsType<CssFunction>(atRule.Prelude[0]);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Prelude_CanContainSimpleBlock()
    {
        var atRule = new CssAtRule("media".AsSpan());
        var block = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        block.Values.Add(new IdentToken("color"));
        atRule.Prelude.Add(block);

        Assert.Single(atRule.Prelude);
        Assert.IsType<CssSimpleBlock>(atRule.Prelude[0]);
    }
    #endregion

    #region Block Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Block_CanBeSet()
    {
        var atRule = new CssAtRule("media".AsSpan());
        atRule.Block = new CssSimpleBlock(BracketOpenToken.Instance);

        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Block_CanContainRules()
    {
        var atRule = new CssAtRule("media".AsSpan());
        atRule.Block = new CssSimpleBlock(BracketOpenToken.Instance);

        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));
        atRule.Block.Values.Add(decl);

        Assert.Single(atRule.Block.Values);
    }
    #endregion

    #region Parser Integration Tests - Statement At-Rules
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesCharsetRule()
    {
        var parser = new CssParser("@charset \"UTF-8\";");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("charset", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesImportRule()
    {
        var parser = new CssParser("@import url('styles.css');");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("import", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesImportWithMedia()
    {
        var parser = new CssParser("@import url('print.css') print;");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("import", atRule!.Name);
        Assert.True(atRule.Prelude.Count > 0);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesNamespaceRule()
    {
        var parser = new CssParser("@namespace url(http://www.w3.org/1999/xhtml);");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("namespace", atRule!.Name);
    }
    #endregion

    #region Parser Integration Tests - Block At-Rules
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMediaRule()
    {
        var parser = new CssParser("@media screen { body { color: red; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("media", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesFontFaceRule()
    {
        var parser = new CssParser("@font-face { font-family: MyFont; src: url(myfont.woff); }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("font-face", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesKeyframesRule()
    {
        var parser = new CssParser("@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("keyframes", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesSupportsRule()
    {
        var parser = new CssParser("@supports (display: grid) { div { display: grid; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("supports", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesPageRule()
    {
        var parser = new CssParser("@page { margin: 1in; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("page", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesLayerRule()
    {
        var parser = new CssParser("@layer base { h1 { font-size: 2rem; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("layer", atRule!.Name);
    }
    #endregion

    #region Parser Integration Tests - Complex Media Queries
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMediaWithAnd()
    {
        var parser = new CssParser("@media screen and (min-width: 768px) { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.True(atRule!.Prelude.Count > 0);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMediaWithNot()
    {
        var parser = new CssParser("@media not print { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMediaWithOnly()
    {
        var parser = new CssParser("@media only screen { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
    }
    #endregion

    #region At-Rule In Declaration List Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_AtRuleInDeclarationList()
    {
        // At-rules can appear in declaration lists (e.g., @apply)
        var parser = new CssParser("@apply --theme; color: red;");
        var components = parser.Parse_Decleration_List().ToList();

        Assert.Equal(2, components.Count);
        Assert.IsType<CssAtRule>(components[0]);
    }
    #endregion

    #region Multiple At-Rules Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMultipleAtRules()
    {
        var parser = new CssParser("@charset \"UTF-8\"; @import url('a.css'); @import url('b.css');");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.All(rules, r => Assert.IsType<CssAtRule>(r));
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_ParsesMixedAtRulesAndQualifiedRules()
    {
        var parser = new CssParser("@charset \"UTF-8\"; body { color: red; } @media print { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.IsType<CssAtRule>(rules[0]);
        Assert.IsType<CssQualifiedRule>(rules[1]);
        Assert.IsType<CssAtRule>(rules[2]);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_HandlesEmptyAtRuleBlock()
    {
        var parser = new CssParser("@media screen { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.NotNull(atRule!.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_HandlesNestedAtRules()
    {
        var parser = new CssParser("@media screen { @supports (display: grid) { div { display: grid; } } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.NotNull(atRule!.Block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_HandlesVendorPrefixedAtRule()
    {
        var parser = new CssParser("@-webkit-keyframes fade { from { opacity: 0; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("-webkit-keyframes", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Parser_HandlesUnknownAtRule()
    {
        // Parser should handle unknown at-rules gracefully
        var parser = new CssParser("@custom-rule value { content: test; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("custom-rule", atRule!.Name);
    }
    #endregion

    #region Encode Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Encode_StatementAtRule_ContainsAtSymbol()
    {
        var atRule = new CssAtRule("charset".AsSpan());
        atRule.Prelude.Add(new StringToken("UTF-8"));
        atRule.Block = new CssSimpleBlock(BracketOpenToken.Instance); // Block is required for Encode

        var encoded = atRule.Encode();
        Assert.Contains("@", encoded);
        Assert.Contains("charset", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssAtRule")]
    public void Encode_BlockAtRule_ContainsBlock()
    {
        var atRule = new CssAtRule("media".AsSpan());
        atRule.Prelude.Add(new IdentToken("screen"));
        atRule.Block = new CssSimpleBlock(BracketOpenToken.Instance);

        var encoded = atRule.Encode();
        Assert.Contains("@", encoded);
        Assert.Contains("media", encoded);
        Assert.Contains("screen", encoded);
    }
    #endregion
}
