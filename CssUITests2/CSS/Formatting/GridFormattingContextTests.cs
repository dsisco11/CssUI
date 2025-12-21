using CssUI.CSS;
using CssUI.CSS.Formatting;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for GridFormattingContext.
/// Tests the CSS Grid Layout algorithm (https://www.w3.org/TR/css-grid-1/).
/// </summary>
public class GridFormattingContextTests
{
    #region Basic Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void GridFormattingContext_CanBeInstantiated()
    {
        var context = new GridFormattingContext();
        Assert.NotNull(context);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridFormattingContext_ImplementsIFormattingContext()
    {
        var context = new GridFormattingContext();
        Assert.IsAssignableFrom<IFormattingContext>(context);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridFormattingContext_Flow_HandlesNullGracefully()
    {
        var context = new GridFormattingContext();
        Assert.Throws<System.ArgumentNullException>(() => context.Flow(null));
    }

    #endregion

    #region Property Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void EGridAutoFlow_HasExpectedValues()
    {
        Assert.Equal(0, (int)EGridAutoFlow.Row);
        Assert.Equal(1, (int)EGridAutoFlow.Column);
        Assert.Equal(2, (int)EGridAutoFlow.Dense);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void EGridAutoFlow_DenseCanCombineWithRow()
    {
        var flow = EGridAutoFlow.Row | EGridAutoFlow.Dense;
        Assert.True((flow & EGridAutoFlow.Dense) != 0);
        Assert.True((flow & EGridAutoFlow.Column) == 0);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void EGridAutoFlow_DenseCanCombineWithColumn()
    {
        var flow = EGridAutoFlow.Column | EGridAutoFlow.Dense;
        Assert.True((flow & EGridAutoFlow.Dense) != 0);
        Assert.True((flow & EGridAutoFlow.Column) != 0);
    }

    #endregion

    #region Display Mode Integration Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void EDisplayMode_HasGridValues()
    {
        // Verify grid display modes exist
        Assert.True(System.Enum.IsDefined(typeof(EDisplayMode), "GRID"));
        Assert.True(System.Enum.IsDefined(typeof(EDisplayMode), "INLINE_GRID"));
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void EInnerDisplayType_HasGridValue()
    {
        Assert.True(System.Enum.IsDefined(typeof(CssUI.CSS.Enums.EInnerDisplayType), "Grid"));
    }

    #endregion

    #region Property ID Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void ECssPropertyID_HasGridProperties()
    {
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridTemplateColumns"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridTemplateRows"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoColumns"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoRows"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridAutoFlow"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridColumnStart"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridColumnEnd"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridRowStart"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "GridRowEnd"));
    }

    #endregion

    #region Gap Property Tests

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "Gap")]
    public void ECssPropertyID_HasGapProperties()
    {
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "RowGap"));
        Assert.True(System.Enum.IsDefined(typeof(ECssPropertyID), "ColumnGap"));
    }

    #endregion
}
