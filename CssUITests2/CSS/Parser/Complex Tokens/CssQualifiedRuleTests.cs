using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssQualifiedRule - represents a CSS style rule with a selector prelude and declaration block.
/// </summary>
public class CssQualifiedRuleTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Constructor_SetsTypeToQualifiedRule()
    {
        var rule = new CssQualifiedRule();
        Assert.Equal(ECssTokenType.QualifiedRule, rule.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Constructor_PreludeDefaultsToEmpty()
    {
        var rule = new CssQualifiedRule();
        Assert.Empty(rule.Prelude);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Constructor_BlockDefaultsToBracketOpen()
    {
        var rule = new CssQualifiedRule();
        Assert.NotNull(rule.Block);
        Assert.Equal(ECssTokenType.Bracket_Open, rule.Block.StartToken.Type);
    }
    #endregion

    #region Prelude Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddSingleSelector()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("div"));

        Assert.Single(rule.Prelude);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddClassSelector()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new DelimToken('.'));
        rule.Prelude.Add(new IdentToken("container"));

        Assert.Equal(2, rule.Prelude.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddIdSelector()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new HashToken(EHashTokenType.ID, "main".AsSpan()));

        Assert.Single(rule.Prelude);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddCombinedSelectors()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("div"));
        rule.Prelude.Add(WhitespaceToken.Space);
        rule.Prelude.Add(new DelimToken('.'));
        rule.Prelude.Add(new IdentToken("child"));

        Assert.Equal(4, rule.Prelude.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddSelectorList()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("h1"));
        rule.Prelude.Add(CommaToken.Instance);
        rule.Prelude.Add(WhitespaceToken.Space);
        rule.Prelude.Add(new IdentToken("h2"));
        rule.Prelude.Add(CommaToken.Instance);
        rule.Prelude.Add(WhitespaceToken.Space);
        rule.Prelude.Add(new IdentToken("h3"));

        Assert.Equal(7, rule.Prelude.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddAttributeSelector()
    {
        var rule = new CssQualifiedRule();
        var attrBlock = new CssSimpleBlock(SqBracketOpenToken.Instance);
        attrBlock.Values.Add(new IdentToken("type"));
        attrBlock.Values.Add(new DelimToken('='));
        attrBlock.Values.Add(new StringToken("text"));
        rule.Prelude.Add(attrBlock);

        Assert.Single(rule.Prelude);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddPseudoClass()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("a"));
        rule.Prelude.Add(ColonToken.Instance);
        rule.Prelude.Add(new IdentToken("hover"));

        Assert.Equal(3, rule.Prelude.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Prelude_CanAddPseudoElement()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("p"));
        rule.Prelude.Add(ColonToken.Instance);
        rule.Prelude.Add(ColonToken.Instance);
        rule.Prelude.Add(new IdentToken("first-line"));

        Assert.Equal(4, rule.Prelude.Count);
    }
    #endregion

    #region Block Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Block_CanAddDeclaration()
    {
        var rule = new CssQualifiedRule();
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));
        rule.Block.Values.Add(decl);

        Assert.Single(rule.Block.Values);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Block_CanAddMultipleDeclarations()
    {
        var rule = new CssQualifiedRule();

        var decl1 = new CssDecleration("color".AsSpan());
        decl1.Values.Add(new IdentToken("red"));
        rule.Block.Values.Add(decl1);

        var decl2 = new CssDecleration("background".AsSpan());
        decl2.Values.Add(new IdentToken("blue"));
        rule.Block.Values.Add(decl2);

        Assert.Equal(2, rule.Block.Values.Count);
    }
    #endregion

    #region Parser Integration Tests - Simple Selectors
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesElementSelector()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
        Assert.Contains(rule!.Prelude, t => t.Type == ECssTokenType.Ident);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesClassSelector()
    {
        var parser = new CssParser(".container { margin: 0; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesIdSelector()
    {
        var parser = new CssParser("#main { width: 100%; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
        Assert.Contains(rule!.Prelude, t => t.Type == ECssTokenType.Hash);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesUniversalSelector()
    {
        var parser = new CssParser("* { margin: 0; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }
    #endregion

    #region Parser Integration Tests - Combinator Selectors
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesDescendantSelector()
    {
        var parser = new CssParser("div p { color: blue; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
        Assert.True(rule!.Prelude.Count > 1);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesChildSelector()
    {
        var parser = new CssParser("div > p { color: blue; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesAdjacentSiblingSelector()
    {
        var parser = new CssParser("h1 + p { margin-top: 0; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesGeneralSiblingSelector()
    {
        var parser = new CssParser("h1 ~ p { color: gray; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }
    #endregion

    #region Parser Integration Tests - Pseudo Selectors
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesPseudoClass()
    {
        var parser = new CssParser("a:hover { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesPseudoElement()
    {
        var parser = new CssParser("p::first-line { font-weight: bold; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesFunctionalPseudoClass()
    {
        var parser = new CssParser("li:nth-child(2n+1) { background: #eee; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesNegationPseudoClass()
    {
        var parser = new CssParser("p:not(.special) { color: black; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }
    #endregion

    #region Parser Integration Tests - Attribute Selectors
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesAttributePresence()
    {
        var parser = new CssParser("[disabled] { opacity: 0.5; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesAttributeEquals()
    {
        var parser = new CssParser("[type=\"text\"] { border: 1px solid; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesAttributeStartsWith()
    {
        var parser = new CssParser("[href^=\"https\"] { color: green; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }
    #endregion

    #region Parser Integration Tests - Selector Lists
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesSelectorList()
    {
        var parser = new CssParser("h1, h2, h3 { font-family: sans-serif; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
        Assert.Contains(rule!.Prelude, t => t.Type == ECssTokenType.Comma);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesComplexSelectorList()
    {
        var parser = new CssParser("div.container, section#main, article > p { margin: 0; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }
    #endregion

    #region Multiple Rules Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_ParsesMultipleRules()
    {
        var parser = new CssParser("h1 { color: red; } p { margin: 10px; } div { padding: 5px; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.All(rules, r => Assert.IsType<CssQualifiedRule>(r));
    }
    #endregion

    #region Encode Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Encode_WithSelector_ContainsPrelude()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("div"));

        var encoded = rule.Encode();
        Assert.Contains("div", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Encode_WithDeclaration_ContainsBlock()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("div"));

        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));
        rule.Block.Values.Add(decl);

        var encoded = rule.Encode();
        Assert.Contains("div", encoded);
        Assert.Contains("{", encoded);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesEmptyDeclarationBlock()
    {
        var parser = new CssParser("div { }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesWhitespaceInSelector()
    {
        var parser = new CssParser("   div   .container   { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesDeeplyNestedSelectors()
    {
        var parser = new CssParser("body div.wrapper section article p span { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesMultipleDeclarations()
    {
        var parser = new CssParser("div { color: red; background: blue; margin: 10px; padding: 5px; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var rule = rules[0] as CssQualifiedRule;
        Assert.NotNull(rule);
        Assert.True(rule!.Block.Values.Count > 0);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesTrailingSemicolon()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Parser_HandlesMissingSemicolon()
    {
        var parser = new CssParser("div { color: red }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
    }
    #endregion

    #region Roundtrip Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssQualifiedRule")]
    public void Roundtrip_SimpleRule_PreservesStructure()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();
        var original = rules[0] as CssQualifiedRule;

        Assert.NotNull(original);
        var encoded = original!.Encode();

        var parser2 = new CssParser(encoded);
        var rules2 = parser2.Parse_Rule_List().ToList();
        var reparsed = rules2[0] as CssQualifiedRule;

        Assert.NotNull(reparsed);
        Assert.NotNull(reparsed!.Block);
    }
    #endregion
}
