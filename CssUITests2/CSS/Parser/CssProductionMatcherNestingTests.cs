using System.Collections.Generic;
using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for CSS Nesting integration in CssProductionMatcher (Phase 11.7.7).
/// Validates ECssBlockContentsType-based token classification.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
[Trait("Category", "Parser")]
[Trait("Category", "CssNesting")]
public class CssProductionMatcherNestingTests
{
    #region StartsNestedRule Tests

    [Fact]
    public void StartsNestedRule_NestingSelector_ReturnsTrue()
    {
        // Arrange - & starts a nested rule
        var token = new DelimToken('&');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_ClassSelector_ReturnsTrue()
    {
        // Arrange - . starts a class selector (nested rule)
        var token = new DelimToken('.');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_UniversalSelector_ReturnsTrue()
    {
        // Arrange - * starts a universal selector (nested rule)
        var token = new DelimToken('*');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_ChildCombinator_ReturnsTrue()
    {
        // Arrange - > starts a relative selector (nested rule)
        var token = new DelimToken('>');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_AdjacentSiblingCombinator_ReturnsTrue()
    {
        // Arrange - + starts a relative selector (nested rule)
        var token = new DelimToken('+');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_GeneralSiblingCombinator_ReturnsTrue()
    {
        // Arrange - ~ starts a relative selector (nested rule)
        var token = new DelimToken('~');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_HashToken_ReturnsTrue()
    {
        // Arrange - # starts an ID selector (nested rule)
        var token = new HashToken(EHashTokenType.ID, "#test");

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_ColonToken_ReturnsTrue()
    {
        // Arrange - : starts a pseudo-class selector (nested rule)
        var token = new ColonToken();

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_SquareBracketOpen_ReturnsTrue()
    {
        // Arrange - [ starts an attribute selector (nested rule)
        var token = new SqBracketOpenToken();

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_AtKeyword_ReturnsTrue()
    {
        // Arrange - @media is a nested at-rule
        var token = new AtToken("media");

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsNestedRule_IdentToken_ReturnsFalse()
    {
        // Arrange - identifier starts a declaration, NOT a nested rule
        var token = new IdentToken("color");

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void StartsNestedRule_NullToken_ReturnsFalse()
    {
        // Act
        var result = CssProductionMatcher.StartsNestedRule(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void StartsNestedRule_OtherDelim_ReturnsFalse()
    {
        // Arrange - / is not a nested rule starter
        var token = new DelimToken('/');

        // Act
        var result = CssProductionMatcher.StartsNestedRule(token);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region StartsDeclaration Tests

    [Fact]
    public void StartsDeclaration_IdentToken_ReturnsTrue()
    {
        // Arrange - identifier starts a declaration (property name)
        var token = new IdentToken("margin");

        // Act
        var result = CssProductionMatcher.StartsDeclaration(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StartsDeclaration_HashToken_ReturnsFalse()
    {
        // Arrange - # starts a selector, not a declaration
        var token = new HashToken(EHashTokenType.ID, "#id");

        // Act
        var result = CssProductionMatcher.StartsDeclaration(token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void StartsDeclaration_NullToken_ReturnsFalse()
    {
        // Act
        var result = CssProductionMatcher.StartsDeclaration(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ClassifyTokenForContext - StyleBlock Tests

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_Ident_ReturnsDeclaration()
    {
        // Arrange
        var token = new IdentToken("color");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Declaration, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_NestingSelector_ReturnsNestedRule()
    {
        // Arrange
        var token = new DelimToken('&');

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.NestedRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_ClassSelector_ReturnsNestedRule()
    {
        // Arrange
        var token = new DelimToken('.');

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.NestedRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_AtKeyword_ReturnsAtRule()
    {
        // Arrange
        var token = new AtToken("media");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.AtRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_Semicolon_ReturnsSeparator()
    {
        // Arrange
        var token = SemicolonToken.Instance;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Separator, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_Whitespace_ReturnsWhitespace()
    {
        // Arrange
        var token = WhitespaceToken.Space;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Whitespace, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_EOF_ReturnsEndOfInput()
    {
        // Arrange
        var token = EOFToken.Instance;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.EndOfInput, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_BadString_ReturnsInvalid()
    {
        // Arrange
        var token = new BadStringToken("");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Invalid, result);
    }

    [Fact]
    public void ClassifyTokenForContext_StyleBlock_UnmatchedClose_ReturnsUnmatchedCloseBracket()
    {
        // Arrange
        var token = BracketCloseToken.Instance;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.Equal(ECssBlockContentClassification.UnmatchedCloseBracket, result);
    }

    #endregion

    #region ClassifyTokenForContext - DeclarationList Tests

    [Fact]
    public void ClassifyTokenForContext_DeclarationList_Ident_ReturnsDeclaration()
    {
        // Arrange
        var token = new IdentToken("color");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.DeclarationList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Declaration, result);
    }

    [Fact]
    public void ClassifyTokenForContext_DeclarationList_AtKeyword_ReturnsAtRule()
    {
        // Arrange
        var token = new AtToken("supports");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.DeclarationList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.AtRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_DeclarationList_ClassSelector_ReturnsInvalid()
    {
        // Arrange - nested rules NOT allowed in declaration-list
        var token = new DelimToken('.');

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.DeclarationList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Invalid, result);
    }

    #endregion

    #region ClassifyTokenForContext - RuleList Tests

    [Fact]
    public void ClassifyTokenForContext_RuleList_Ident_ReturnsQualifiedRule()
    {
        // Arrange - in rule-list, ident starts a selector (qualified rule)
        var token = new IdentToken("div");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.RuleList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.QualifiedRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_RuleList_AtKeyword_ReturnsAtRule()
    {
        // Arrange
        var token = new AtToken("media");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.RuleList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.AtRule, result);
    }

    [Fact]
    public void ClassifyTokenForContext_RuleList_ClassSelector_ReturnsQualifiedRule()
    {
        // Arrange
        var token = new DelimToken('.');

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.RuleList);

        // Assert
        Assert.Equal(ECssBlockContentClassification.QualifiedRule, result);
    }

    #endregion

    #region ClassifyTokenForContext - Stylesheet Tests

    [Fact]
    public void ClassifyTokenForContext_Stylesheet_CDO_ReturnsIgnored()
    {
        // Arrange - CDO (<!--) is ignored at top level
        var token = CdoToken.Instance;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.Stylesheet);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Ignored, result);
    }

    [Fact]
    public void ClassifyTokenForContext_Stylesheet_CDC_ReturnsIgnored()
    {
        // Arrange - CDC (-->) is ignored at top level
        var token = CdcToken.Instance;

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.Stylesheet);

        // Assert
        Assert.Equal(ECssBlockContentClassification.Ignored, result);
    }

    [Fact]
    public void ClassifyTokenForContext_Stylesheet_Ident_ReturnsQualifiedRule()
    {
        // Arrange
        var token = new IdentToken("body");

        // Act
        var result = CssProductionMatcher.ClassifyTokenForContext(token, ECssBlockContentsType.Stylesheet);

        // Assert
        Assert.Equal(ECssBlockContentClassification.QualifiedRule, result);
    }

    #endregion

    #region ValidateBlockContents Tests

    [Fact]
    public void ValidateBlockContents_EmptyList_ReturnsTrue()
    {
        // Arrange
        var tokens = new List<CssToken>();

        // Act
        var result = CssProductionMatcher.ValidateBlockContents(tokens, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateBlockContents_NullList_ReturnsTrue()
    {
        // Act
        var result = CssProductionMatcher.ValidateBlockContents(null!, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateBlockContents_ValidTokens_ReturnsTrue()
    {
        // Arrange
        var tokens = new List<CssToken>
        {
            new IdentToken("color"),
            ColonToken.Instance,
            new IdentToken("red"),
            SemicolonToken.Instance
        };

        // Act
        var result = CssProductionMatcher.ValidateBlockContents(tokens, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateBlockContents_BadString_ReturnsFalse()
    {
        // Arrange
        var tokens = new List<CssToken>
        {
            new IdentToken("content"),
            ColonToken.Instance,
            new BadStringToken("")
        };

        // Act
        var result = CssProductionMatcher.ValidateBlockContents(tokens, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateBlockContents_BadUrl_ReturnsFalse()
    {
        // Arrange
        var tokens = new List<CssToken>
        {
            new IdentToken("background"),
            ColonToken.Instance,
            new BadUrlToken()
        };

        // Act
        var result = CssProductionMatcher.ValidateBlockContents(tokens, ECssBlockContentsType.StyleBlock);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region StartsNestedRuleDelim Tests

    [Theory]
    [InlineData('&', true)]   // nesting selector
    [InlineData('.', true)]   // class selector
    [InlineData('*', true)]   // universal selector
    [InlineData('>', true)]   // child combinator
    [InlineData('+', true)]   // adjacent sibling combinator
    [InlineData('~', true)]   // general sibling combinator
    [InlineData('/', false)]  // not a nested rule starter
    [InlineData('=', false)]  // not a nested rule starter
    [InlineData('!', false)]  // not a nested rule starter
    public void StartsNestedRuleDelim_ReturnsExpectedResult(char delimChar, bool expected)
    {
        // Act
        var result = CssProductionMatcher.StartsNestedRuleDelim(delimChar);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion
}
