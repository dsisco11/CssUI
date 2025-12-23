using System;
using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssComponent - the base class for parser output components.
/// CssComponent is abstract, so tests verify its behavior through concrete subclasses.
/// </summary>
public class CssComponentTests
{
    #region Base Type Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_IsAbstract_CannotBeInstantiatedDirectly()
    {
        // CssComponent is abstract - we verify this through concrete implementations
        // All concrete implementations should inherit from CssComponent
        var declaration = new CssDecleration("test".AsSpan());
        Assert.IsAssignableFrom<CssComponent>(declaration);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_InheritsFromCssToken()
    {
        // CssComponent extends CssToken
        var declaration = new CssDecleration("test".AsSpan());
        Assert.IsAssignableFrom<CssToken>(declaration);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_HasTypeProperty()
    {
        // All CssComponents have a Type property from CssToken
        var declaration = new CssDecleration("test".AsSpan());
        Assert.Equal(ECssTokenType.Decleration, declaration.Type);
    }
    #endregion

    #region Concrete Implementation Type Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssDecleration_IsCssComponent()
    {
        var declaration = new CssDecleration("color".AsSpan());
        Assert.IsAssignableFrom<CssComponent>(declaration);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssSimpleBlock_IsCssComponent()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        Assert.IsAssignableFrom<CssComponent>(block);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssAtRule_IsCssComponent()
    {
        var atRule = new CssAtRule("media".AsSpan());
        Assert.IsAssignableFrom<CssComponent>(atRule);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssQualifiedRule_IsCssComponent()
    {
        var qualifiedRule = new CssQualifiedRule();
        Assert.IsAssignableFrom<CssComponent>(qualifiedRule);
    }
    #endregion

    #region Encode Method Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_EncodeMethod_IsAbstract_ImplementedBySubclasses()
    {
        // All concrete CssComponent subclasses must implement Encode()
        var declaration = new CssDecleration("test".AsSpan());
        var encoded = declaration.Encode();
        Assert.NotNull(encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_ToStringUsesEncode()
    {
        var declaration = new CssDecleration("color".AsSpan());
        declaration.Values.Add(new IdentToken("red"));

        // ToString should return the same as Encode
        Assert.Equal(declaration.Encode(), declaration.ToString());
    }
    #endregion

    #region Equality Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_EqualityByType_SameType()
    {
        var decl1 = new CssDecleration("color".AsSpan());
        var decl2 = new CssDecleration("background".AsSpan());

        // CssToken.Equals only compares Type, not content
        Assert.Equal(decl1.Type, decl2.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssComponent")]
    public void CssComponent_DifferentTypes_NotEqual()
    {
        var declaration = new CssDecleration("color".AsSpan());
        var atRule = new CssAtRule("media".AsSpan());

        Assert.NotEqual(declaration.Type, atRule.Type);
    }
    #endregion
}
