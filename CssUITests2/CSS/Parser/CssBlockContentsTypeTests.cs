using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for <see cref="ECssBlockContentsType"/> and <see cref="CssParser.Parse_Block_Contents"/>
/// per CSS Syntax Level 3 §8.1.
/// </summary>
[Trait("Category", "CSS Parser")]
[Trait("Spec", "CSS Syntax 3 §8.1")]
public class CssBlockContentsTypeTests
{
    #region Parse_Block_Contents with ECssBlockContentsType.StyleBlock

    [Fact]
    public void ParseBlockContents_StyleBlock_ParsesDeclarations()
    {
        var parser = new CssParser("color: red; font-size: 16px;");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssDecleration>(c));
    }

    [Fact]
    public void ParseBlockContents_StyleBlock_ParsesNestedAtRules()
    {
        var parser = new CssParser("color: red; @media screen { }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        // Should have 1 declaration + 1 at-rule
        Assert.Equal(2, contents.Count);
    }

    [Fact]
    public void ParseBlockContents_StyleBlock_ParsesNestedQualifiedRulesWithAmpersand()
    {
        var parser = new CssParser("color: red; &:hover { }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock).ToList();

        // Should have 1 declaration + 1 qualified rule (nested)
        Assert.Equal(2, contents.Count);
    }

    #endregion

    #region Parse_Block_Contents with ECssBlockContentsType.DeclarationList

    [Fact]
    public void ParseBlockContents_DeclarationList_ParsesDeclarations()
    {
        var parser = new CssParser("src: url(font.woff); font-style: normal;");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.DeclarationList).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssDecleration>(c));
    }

    [Fact]
    public void ParseBlockContents_DeclarationList_ParsesAtRules()
    {
        // @font-face allows at-rules mixed with declarations
        var parser = new CssParser("src: url(font.woff); @layer base;");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.DeclarationList).ToList();

        Assert.Equal(2, contents.Count);
    }

    #endregion

    #region Parse_Block_Contents with ECssBlockContentsType.RuleList

    [Fact]
    public void ParseBlockContents_RuleList_ParsesQualifiedRules()
    {
        var parser = new CssParser(".foo { color: red; } .bar { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.RuleList).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssQualifiedRule>(c));
    }

    [Fact]
    public void ParseBlockContents_RuleList_ParsesAtRules()
    {
        var parser = new CssParser("@charset 'utf-8'; @import url(other.css);");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.RuleList).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssAtRule>(c));
    }

    [Fact]
    public void ParseBlockContents_RuleList_ConsumeCdoAndCdc()
    {
        // Rule list with top-level=false should consume CDO/CDC as part of qualified rules
        var parser = new CssParser("<!-- .foo { } -->");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.RuleList).ToList();

        // CDO/CDC consumed into qualified rules when not top-level
        Assert.NotEmpty(contents);
    }

    #endregion

    #region Parse_Block_Contents with ECssBlockContentsType.Stylesheet

    [Fact]
    public void ParseBlockContents_Stylesheet_ParsesQualifiedRules()
    {
        var parser = new CssParser(".foo { color: red; } .bar { color: blue; }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.Stylesheet).ToList();

        Assert.Equal(2, contents.Count);
        Assert.All(contents, c => Assert.IsType<CssQualifiedRule>(c));
    }

    [Fact]
    public void ParseBlockContents_Stylesheet_IgnoresCdoAndCdc()
    {
        // Stylesheet (top-level=true) should IGNORE CDO/CDC tokens
        var parser = new CssParser("<!-- -->");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.Stylesheet).ToList();

        // CDO/CDC ignored at stylesheet level
        Assert.Empty(contents);
    }

    [Fact]
    public void ParseBlockContents_Stylesheet_ParsesMixedContent()
    {
        var parser = new CssParser("@import url(foo.css); .bar { }");
        var contents = parser.Parse_Block_Contents(ECssBlockContentsType.Stylesheet).ToList();

        Assert.Equal(2, contents.Count);
        Assert.IsType<CssAtRule>(contents[0]);
        Assert.IsType<CssQualifiedRule>(contents[1]);
    }

    #endregion

    #region ECssBlockContentsType Enum Values

    [Fact]
    public void ECssBlockContentsType_HasAllExpectedValues()
    {
        // Verify all four production types exist per §8.1
        Assert.True(System.Enum.IsDefined(typeof(ECssBlockContentsType), ECssBlockContentsType.StyleBlock));
        Assert.True(System.Enum.IsDefined(typeof(ECssBlockContentsType), ECssBlockContentsType.DeclarationList));
        Assert.True(System.Enum.IsDefined(typeof(ECssBlockContentsType), ECssBlockContentsType.RuleList));
        Assert.True(System.Enum.IsDefined(typeof(ECssBlockContentsType), ECssBlockContentsType.Stylesheet));
    }

    #endregion
}
