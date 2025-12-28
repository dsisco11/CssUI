using CssUI.CSS;
using CssUI.CSS.Formatting;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Tests for BlockFormattingContext.
/// See: https://www.w3.org/TR/CSS2/visuren.html#block-formatting
/// </summary>
public class BlockFormattingContextTests
{
    #region Instantiation Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "BFC")]
    public void BlockFormattingContext_CanBeInstantiated()
    {
        // Act
        var bfc = new BlockFormattingContext();

        // Assert
        Assert.NotNull(bfc);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "BFC")]
    public void BlockFormattingContext_ImplementsIFormattingContext()
    {
        // Act
        var bfc = new BlockFormattingContext();

        // Assert
        Assert.IsAssignableFrom<IFormattingContext>(bfc);
    }

    #endregion

    #region Flow Method Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "BFC")]
    public void Flow_WithNullNode_ThrowsArgumentNullException()
    {
        // Arrange
        var bfc = new BlockFormattingContext();

        // Act & Assert
        Assert.Throws<System.ArgumentNullException>(() => bfc.Flow(null!));
    }

    #endregion
}
