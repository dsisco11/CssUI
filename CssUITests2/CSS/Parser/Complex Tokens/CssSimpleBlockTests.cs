using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssSimpleBlock - represents a bracketed block like {}, (), or [].
/// </summary>
public class CssSimpleBlockTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Constructor_WithBracketOpen_SetsStartToken()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.Bracket_Open, block.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Constructor_WithParenthOpen_SetsStartToken()
    {
        var block = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        Assert.Equal(ECssTokenType.Parenth_Open, block.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Constructor_WithSqBracketOpen_SetsStartToken()
    {
        var block = new CssSimpleBlock(SqBracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.SqBracket_Open, block.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Constructor_SetsTypeToSimpleBlock()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.SimpleBlock, block.Type);
    }
    #endregion

    #region Values Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_InitiallyEmpty()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        Assert.Empty(block.Values);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_CanAddSingleToken()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        block.Values.Add(new IdentToken("color"));

        Assert.Single(block.Values);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_CanAddMultipleTokens()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        block.Values.Add(new IdentToken("color"));
        block.Values.Add(ColonToken.Instance);
        block.Values.Add(WhitespaceToken.Space);
        block.Values.Add(new IdentToken("red"));

        Assert.Equal(4, block.Values.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_CanContainDeclaration()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        var decl = new CssDecleration("color".AsSpan());
        decl.Values.Add(new IdentToken("red"));
        block.Values.Add(decl);

        Assert.Single(block.Values);
        Assert.IsType<CssDecleration>(block.Values[0]);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_CanContainNestedBlock()
    {
        var outerBlock = new CssSimpleBlock(BracketOpenToken.Instance);
        var innerBlock = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        innerBlock.Values.Add(new NumberToken(ENumericTokenType.Integer, "42".AsSpan(), 42));

        outerBlock.Values.Add(innerBlock);

        Assert.Single(outerBlock.Values);
        Assert.IsType<CssSimpleBlock>(outerBlock.Values[0]);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Values_CanContainFunction()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        var func = new CssFunction("calc".AsSpan());
        func.Arguments.Add(new PercentageToken("100".AsSpan(), 100.0));
        block.Values.Add(func);

        Assert.Single(block.Values);
        Assert.IsType<CssFunction>(block.Values[0]);
    }
    #endregion

    #region Encode Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Encode_EmptyCurlyBlock_ReturnsOpenAndClose()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        var encoded = block.Encode();

        Assert.Contains("{", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Encode_EmptyParenthBlock_ReturnsOpenAndClose()
    {
        var block = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        var encoded = block.Encode();

        Assert.Contains("(", encoded);
        Assert.Contains(")", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Encode_WithContent_IncludesValues()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        block.Values.Add(new IdentToken("test"));

        var encoded = block.Encode();
        Assert.Contains("test", encoded);
    }
    #endregion

    #region Parser Integration Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesCurlyBraceBlock()
    {
        var parser = new CssParser("{ color: red }");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.Bracket_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesParenthesisBlock()
    {
        var parser = new CssParser("(1 + 2)");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.Parenth_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesSquareBracketBlock()
    {
        var parser = new CssParser("[attr=value]");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.SqBracket_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesNestedBlocks()
    {
        var parser = new CssParser("{ { nested } }");
        var values = parser.Parse_ComponentValue_List();

        var outerBlock = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(outerBlock);

        // Should contain nested block
        var innerBlock = outerBlock!.Values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(innerBlock);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesBlockWithDeclarations()
    {
        var parser = new CssParser("{ color: red; background: blue; }");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
        Assert.True(block!.Values.Count > 0);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesEmptyBlock()
    {
        var parser = new CssParser("{}");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_ParsesBlockWithFunction()
    {
        var parser = new CssParser("{ background: url(image.png); }");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }
    #endregion

    #region Block Type Discrimination Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void StartToken_CurlyBrace_IsBracketOpen()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.Bracket_Open, block.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void StartToken_Parenthesis_IsParenthOpen()
    {
        var block = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        Assert.Equal(ECssTokenType.Parenth_Open, block.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void StartToken_SquareBracket_IsSqBracketOpen()
    {
        var block = new CssSimpleBlock(SqBracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.SqBracket_Open, block.StartToken.Type);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_HandlesDeepNesting()
    {
        var parser = new CssParser("{ { { deeply { nested } } } }");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_HandlesMixedNesting()
    {
        var parser = new CssParser("( { [ mixed ] } )");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssSimpleBlock")]
    public void Parser_HandlesBlockWithWhitespace()
    {
        var parser = new CssParser("{   }");
        var values = parser.Parse_ComponentValue_List();

        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }
    #endregion
}
