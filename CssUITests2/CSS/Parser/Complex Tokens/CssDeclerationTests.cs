using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssDecleration - represents a CSS declaration like "color: red" or "margin: 10px 20px".
/// </summary>
public class CssDeclerationTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Constructor_WithName_SetsNameProperty()
    {
        var decl = new CssDecleration("color".AsSpan());
        Assert.Equal("color", decl.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Constructor_WithName_SetsTypeToDecleration()
    {
        var decl = new CssDecleration("margin".AsSpan());
        Assert.Equal(ECssTokenType.Decleration, decl.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Constructor_WithHyphenatedName_PreservesName()
    {
        var decl = new CssDecleration("background-color".AsSpan());
        Assert.Equal("background-color", decl.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Constructor_WithCustomProperty_PreservesDoubleDash()
    {
        var decl = new CssDecleration("--my-variable".AsSpan());
        Assert.Equal("--my-variable", decl.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Constructor_ImportantDefaultsFalse()
    {
        var decl = new CssDecleration("color".AsSpan());
        Assert.False(decl.Important);
    }
    #endregion

    #region Values Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Values_InitiallyEmpty()
    {
        var decl = new CssDecleration("color".AsSpan());
        Assert.Empty(decl.Values);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Values_CanAddSingleToken()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));

        Assert.Single(decl.Values);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Values_CanContainFunction()
    {
        var decl = new CssDecleration("background".AsSpan());
        var func = new CssFunction("url".AsSpan());
        func.Arguments.Add(new StringToken("image.png"));
        decl.Values.Add(func);

        Assert.Single(decl.Values);
        Assert.IsType<CssFunction>(decl.Values[0]);
    }
    #endregion

    #region Important Flag Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Important_CanBeSetToTrue()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Important = true;
        Assert.True(decl.Important);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Important_CanBeToggled()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Important = true;
        Assert.True(decl.Important);
        decl.Important = false;
        Assert.False(decl.Important);
    }
    #endregion

    #region Encode Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_EmptyValue_ReturnsNameWithColonAndSemicolon()
    {
        var decl = new CssDecleration("color".AsSpan());
        var encoded = decl.Encode();

        Assert.Contains("color", encoded);
        Assert.Contains(":", encoded);
        Assert.Contains(";", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_WithIdentValue_ReturnsCorrectFormat()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));

        var encoded = decl.Encode();
        Assert.Equal("color: red;", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_WithDimensionValue_ReturnsCorrectFormat()
    {
        var decl = new CssDecleration("width".AsSpan());
        decl.Values.Add(new DimensionToken(ENumericTokenType.Integer, "100".AsSpan(), 100, "px".AsSpan()));

        var encoded = decl.Encode();
        Assert.Equal("width: 100px;", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_WithImportant_ContainsImportantFlag()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));
        decl.Important = true;

        var encoded = decl.Encode();
        Assert.Contains("!important", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_WithoutImportant_DoesNotContainImportantFlag()
    {
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));

        var encoded = decl.Encode();
        Assert.DoesNotContain("!important", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Encode_CustomProperty_PreservesDoubleDash()
    {
        var decl = new CssDecleration("--my-color".AsSpan());
        decl.Values.Add(new IdentToken("blue"));

        var encoded = decl.Encode();
        Assert.StartsWith("--my-color", encoded);
    }
    #endregion

    #region Parser Integration Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesSimpleDeclaration()
    {
        var parser = new CssParser("color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("color", decl!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithDimension()
    {
        var parser = new CssParser("width: 100px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("width", decl!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithPercentage()
    {
        var parser = new CssParser("width: 50%;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("width", decl!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithImportant()
    {
        var parser = new CssParser("color: red !important;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.True(decl!.Important);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesMultipleDeclarations()
    {
        var parser = new CssParser("color: red; background: blue; margin: 10px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Equal(3, declarations.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithFunction()
    {
        var parser = new CssParser("background: url('image.png');");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Contains(decl!.Values, v => v.Type == ECssTokenType.Function);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesCustomProperty()
    {
        var parser = new CssParser("--primary-color: #ff0000;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("--primary-color", decl!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesShorthandProperty()
    {
        var parser = new CssParser("margin: 10px 20px 30px 40px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("margin", decl!.Name);
        // Should have multiple dimension tokens
        var dimensions = decl.Values.Where(v => v.Type == ECssTokenType.Dimension).ToList();
        Assert.Equal(4, dimensions.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithHash()
    {
        var parser = new CssParser("color: #ff5500;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Contains(decl!.Values, v => v.Type == ECssTokenType.Hash);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_ParsesDeclarationWithString()
    {
        var parser = new CssParser("content: \"Hello World\";");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Contains(decl!.Values, v => v.Type == ECssTokenType.String);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_HandlesMissingSemicolon()
    {
        var parser = new CssParser("color: red");
        var declarations = parser.Parse_Decleration_List().ToList();

        // Parser should handle missing semicolon
        Assert.Single(declarations);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Parser_HandlesExtraWhitespace()
    {
        var parser = new CssParser("  color  :  red  ;  ");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("color", decl!.Name);
    }
    #endregion

    #region Roundtrip Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Roundtrip_SimpleDeclaration_PreservesContent()
    {
        var parser = new CssParser("color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();
        var original = declarations[0] as CssDecleration;

        Assert.NotNull(original);
        var encoded = original!.Encode();

        var parser2 = new CssParser(encoded);
        var declarations2 = parser2.Parse_Decleration_List().ToList();
        var reparsed = declarations2[0] as CssDecleration;

        Assert.NotNull(reparsed);
        Assert.Equal(original.Name, reparsed!.Name);
        Assert.Equal(original.Important, reparsed.Important);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssDecleration")]
    public void Roundtrip_DeclarationWithImportant_PreservesFlag()
    {
        var parser = new CssParser("color: red !important;");
        var declarations = parser.Parse_Decleration_List().ToList();
        var original = declarations[0] as CssDecleration;

        Assert.NotNull(original);
        var encoded = original!.Encode();

        var parser2 = new CssParser(encoded);
        var declarations2 = parser2.Parse_Decleration_List().ToList();
        var reparsed = declarations2[0] as CssDecleration;

        Assert.NotNull(reparsed);
        Assert.True(reparsed!.Important);
    }
    #endregion
}
