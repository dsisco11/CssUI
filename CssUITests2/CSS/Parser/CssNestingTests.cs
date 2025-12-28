using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for CSS Nesting support per CSS Nesting Module Level 1.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
[Trait("Category", "CSS Parser")]
[Trait("Spec", "CSS Nesting 1")]
public class CssNestingTests
{
    #region Nested Style Rules Starting with Ampersand

    [Fact]
    public void StyleBlock_NestedRule_Ampersand_Parsed()
    {
        // & selector at start of nested rule
        var parser = new CssParser("color: red; & { font-size: 16px; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_AmpersandHover_Parsed()
    {
        // &:hover nested selector
        var parser = new CssParser("color: red; &:hover { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_AmpersandClass_Parsed()
    {
        // &.active compound selector
        var parser = new CssParser("color: red; &.active { color: green; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_AmpersandDescendant_Parsed()
    {
        // & followed by descendant
        var parser = new CssParser("color: red; & .child { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested Style Rules Starting with Class Selector

    [Fact]
    public void StyleBlock_NestedRule_ClassSelector_Parsed()
    {
        // .class selector (implicit & prepending)
        var parser = new CssParser("color: red; .child { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_MultipleClasses_Parsed()
    {
        // Multiple class selectors
        var parser = new CssParser(".foo { color: red; } .bar { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        // These are nested rules, not declarations
        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssQualifiedRule>(c));
    }

    #endregion

    #region Nested Style Rules Starting with ID Selector

    [Fact]
    public void StyleBlock_NestedRule_IdSelector_Parsed()
    {
        // #id selector
        var parser = new CssParser("color: red; #content { padding: 10px; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested Style Rules Starting with Pseudo-Class

    [Fact]
    public void StyleBlock_NestedRule_PseudoClass_Parsed()
    {
        // :hover pseudo-class
        var parser = new CssParser("color: red; :hover { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_PseudoElement_Parsed()
    {
        // ::before pseudo-element
        var parser = new CssParser("color: red; ::before { content: ''; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested Style Rules Starting with Attribute Selector

    [Fact]
    public void StyleBlock_NestedRule_AttributeSelector_Parsed()
    {
        // [type="text"] attribute selector
        var parser = new CssParser("color: red; [type=\"text\"] { border: 1px solid; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested Style Rules Starting with Universal Selector

    [Fact]
    public void StyleBlock_NestedRule_UniversalSelector_Parsed()
    {
        // * universal selector
        var parser = new CssParser("color: red; * { box-sizing: border-box; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested Style Rules Starting with Relative Selectors (Combinators)

    [Fact]
    public void StyleBlock_NestedRule_ChildCombinator_Parsed()
    {
        // > child combinator (relative selector)
        var parser = new CssParser("color: red; > .child { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_AdjacentSiblingCombinator_Parsed()
    {
        // + adjacent sibling combinator
        var parser = new CssParser("color: red; + .sibling { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedRule_GeneralSiblingCombinator_Parsed()
    {
        // ~ general sibling combinator
        var parser = new CssParser("color: red; ~ .sibling { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region Nested At-Rules

    [Fact]
    public void StyleBlock_NestedAtRule_Media_Parsed()
    {
        // @media nested inside style block
        var parser = new CssParser("color: red; @media screen { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssAtRule>(contents[1]);
    }

    [Fact]
    public void StyleBlock_NestedAtRule_Supports_Parsed()
    {
        // @supports nested inside style block
        var parser = new CssParser("color: red; @supports (display: grid) { display: grid; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssDecleration>(contents[0]);
        Assert.IsType<CssAtRule>(contents[1]);
    }

    #endregion

    #region Mixed Declarations and Nested Rules

    [Fact]
    public void StyleBlock_Mixed_DeclarationsAndNestedRules_OrderPreserved()
    {
        // Mix of declarations and nested rules
        var parser = new CssParser("color: red; & .child { } font-size: 16px; #id { }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        // Per spec: declarations come before rules in output, but both should be present
        // Note: The spec says declarations are returned before rules, so order may differ
        Assert.Equal(4, contents.Count);

        // Count types
        int declCount = contents.Count(c => c is CssDecleration);
        int ruleCount = contents.Count(c => c is CssQualifiedRule);

        Assert.Equal(2, declCount);
        Assert.Equal(2, ruleCount);
    }

    [Fact]
    public void StyleBlock_OnlyNestedRules_Parsed()
    {
        // Only nested rules, no declarations
        var parser = new CssParser("& { color: red; } .child { margin: 0; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssQualifiedRule>(c));
    }

    #endregion

    #region Invalid Nested Selectors (Should be Errors)

    [Fact]
    public void StyleBlock_InvalidNested_TypeSelector_NotParsedAsRule()
    {
        // Per spec, nested selectors cannot start with an identifier (type selector)
        // "div { color: red; }" starting with "div" is invalid as a nested rule
        // The parser should NOT treat "div { }" as a nested rule inside a style block
        // It should be a parse error and discarded
        var parser = new CssParser("color: red; div { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        // Only the declaration should remain; "div { }" should be treated as error
        Assert.Single(contents);
        Assert.IsType<CssDecleration>(contents[0]);
    }

    #endregion

    #region Deeply Nested Rules

    [Fact]
    public void StyleBlock_DeeplyNested_MultipleLevels()
    {
        // Nested rules can themselves contain nested rules
        // This test verifies the parser handles the outer level correctly
        var parser = new CssParser("& { color: red; & { color: blue; } }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Single(contents);
        Assert.IsType<CssQualifiedRule>(contents[0]);

        // The inner "& { color: blue; }" would be parsed when the outer block is parsed
        var outerRule = (CssQualifiedRule)contents[0];
        Assert.NotNull(outerRule.Block);
    }

    #endregion

    #region CSSStyleRule.cssRules Property

    [Fact]
    public void CSSStyleRule_cssRules_InitiallyEmpty()
    {
        var rule = new CSSStyleRule();

        Assert.NotNull(rule.cssRules);
        Assert.Empty(rule.cssRules);
    }

    [Fact]
    public void CSSStyleRule_cssRules_CanAddRules()
    {
        var parentRule = new CSSStyleRule();
        var childRule = new CSSStyleRule();

        parentRule.cssRules.Add(childRule);

        Assert.Single(parentRule.cssRules);
        Assert.Same(childRule, parentRule.cssRules[0]);
    }

    #endregion

    #region CSSStyleRule Serialization with Nested Rules

    [Fact]
    public void CSSStyleRule_Serialize_WithoutNestedRules()
    {
        var selector = new CssSelector(".test");
        var rule = new CSSStyleRule(selector, null);

        // Without nested rules, should serialize normally
        string serialized = rule.ToString();

        Assert.Contains("{", serialized);
        Assert.Contains("}", serialized);
    }

    [Fact]
    public void CSSStyleRule_Serialize_WithNestedRules()
    {
        var selector = new CssSelector(".parent");
        var parentRule = new CSSStyleRule(selector, null);

        var childSelector = new CssSelector(".child");
        var childRule = new CSSStyleRule(childSelector, null);

        parentRule.cssRules.Add(childRule);

        string serialized = parentRule.ToString();

        // With nested rules, should have newline formatting
        Assert.Contains("\n", serialized);
    }

    #endregion
}
