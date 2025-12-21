using CssUI.CSS;
using CssUI.CSS.Formatting;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for FlexFormattingContext.
/// Tests the CSS Flexible Box Layout algorithm (https://www.w3.org/TR/css-flexbox-1/).
/// </summary>
public class FlexFormattingContextTests
{
    #region Basic Tests

    [Fact]
    [Trait("Category", "Flex")]
    public void FlexFormattingContext_CanBeInstantiated()
    {
        var context = new FlexFormattingContext();
        Assert.NotNull(context);
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void FlexFormattingContext_ImplementsIFormattingContext()
    {
        var context = new FlexFormattingContext();
        Assert.IsAssignableFrom<IFormattingContext>(context);
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void FlexFormattingContext_Flow_HandlesNullGracefully()
    {
        var context = new FlexFormattingContext();
        Assert.Throws<System.ArgumentNullException>(() => context.Flow(null));
    }

    #endregion

    #region Property Tests

    [Fact]
    [Trait("Category", "Flex")]
    public void EFlexDirection_HasExpectedValues()
    {
        Assert.Equal(0, (int)EFlexDirection.Row);
        Assert.Equal(1, (int)EFlexDirection.RowReverse);
        Assert.Equal(2, (int)EFlexDirection.Column);
        Assert.Equal(3, (int)EFlexDirection.ColumnReverse);
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void EFlexWrap_HasExpectedValues()
    {
        Assert.Equal(0, (int)EFlexWrap.NoWrap);
        Assert.Equal(1, (int)EFlexWrap.Wrap);
        Assert.Equal(2, (int)EFlexWrap.WrapReverse);
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void EJustifyContent_HasFlexValues()
    {
        // Verify flex-specific values exist
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "FlexStart"));
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "FlexEnd"));
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "Center"));
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceBetween"));
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceAround"));
        Assert.True(System.Enum.IsDefined(typeof(EJustifyContent), "SpaceEvenly"));
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void EAlignItems_HasFlexValues()
    {
        // Verify flex-specific values exist
        Assert.True(System.Enum.IsDefined(typeof(EAlignItems), "FlexStart"));
        Assert.True(System.Enum.IsDefined(typeof(EAlignItems), "FlexEnd"));
        Assert.True(System.Enum.IsDefined(typeof(EAlignItems), "Center"));
        Assert.True(System.Enum.IsDefined(typeof(EAlignItems), "Stretch"));
        Assert.True(System.Enum.IsDefined(typeof(EAlignItems), "Baseline"));
    }

    #endregion

    #region Display Mode Integration Tests

    [Fact]
    [Trait("Category", "Flex")]
    public void EDisplayMode_HasFlexValues()
    {
        // Verify flex display modes exist
        Assert.True(System.Enum.IsDefined(typeof(EDisplayMode), "FLEX"));
        Assert.True(System.Enum.IsDefined(typeof(EDisplayMode), "INLINE_FLEX"));
    }

    [Fact]
    [Trait("Category", "Flex")]
    public void EInnerDisplayType_HasFlexValue()
    {
        Assert.True(System.Enum.IsDefined(typeof(CssUI.CSS.Enums.EInnerDisplayType), "Flex"));
    }

    #endregion

    #region Gap Property Tests

    [Fact]
    [Trait("Category", "Flex")]
    [Trait("Category", "Gap")]
    public void ECssPropertyID_HasGapProperties()
    {
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "RowGap"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "ColumnGap"));
    }

    #endregion
}
