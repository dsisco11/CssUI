using System.Collections.Generic;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CORE;

/// <summary>
/// Tests for FlagCollection to ensure enumeration works correctly.
/// Using int to avoid any enum-related complexity.
/// </summary>
public class FlagCollectionTests
{
    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_SetFlag_IncrementsActiveFlags()
    {
        // Arrange
        var fc = new FlagCollection<int>(100);

        // Act
        fc.SetFlag(0);
        fc.SetFlag(5);
        fc.SetFlag(99);

        // Assert
        Assert.Equal(3, fc.ActiveFlags);
    }

    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_GetEnumerator_ReturnsAllSetFlags()
    {
        // Arrange
        var fc = new FlagCollection<int>(100);
        fc.SetFlag(0);
        fc.SetFlag(5);
        fc.SetFlag(99);

        // Act
        var flags = new List<int>();
        foreach (var f in fc)
        {
            flags.Add(f);
        }

        // Assert
        Assert.Equal(3, flags.Count);
        Assert.Contains(0, flags);
        Assert.Contains(5, flags);
        Assert.Contains(99, flags);
    }

    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_GetEnumerator_CountMatchesActiveFlags()
    {
        // Arrange
        var fc = new FlagCollection<int>(100);
        fc.SetFlag(0);
        fc.SetFlag(1);
        fc.SetFlag(2);
        fc.SetFlag(31); // End of first chunk
        fc.SetFlag(32); // Start of second chunk
        fc.SetFlag(63); // End of second chunk
        fc.SetFlag(64); // Start of third chunk

        // Act
        int count = 0;
        foreach (var _ in fc)
        {
            count++;
        }

        // Assert
        Assert.Equal(fc.ActiveFlags, count);
    }

    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_Or_CombinesFlags()
    {
        // Arrange
        var fc1 = new FlagCollection<int>(100);
        fc1.SetFlag(0);
        fc1.SetFlag(5);

        var fc2 = new FlagCollection<int>(100);
        fc2.SetFlag(5);
        fc2.SetFlag(10);

        // Act
        fc1.Or(fc2);

        // Assert
        Assert.Equal(3, fc1.ActiveFlags); // 0, 5, 10
        Assert.True(fc1.GetFlag(0));
        Assert.True(fc1.GetFlag(5));
        Assert.True(fc1.GetFlag(10));
    }

    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_EmptyCollection_ReturnsNoFlags()
    {
        // Arrange
        var fc = new FlagCollection<int>(100);

        // Act
        int count = 0;
        foreach (var _ in fc)
        {
            count++;
        }

        // Assert
        Assert.Equal(0, fc.ActiveFlags);
        Assert.Equal(0, count);
    }

    [Fact(Timeout = 5000)]
    [Trait("Category", "Unit")]
    public void FlagCollection_InitializedToZero()
    {
        // Arrange & Act
        var fc = new FlagCollection<int>(100);

        // Assert
        Assert.Equal(0, fc.ActiveFlags);
        for (int i = 0; i < 100; i++)
        {
            Assert.False(fc.GetFlag(i));
        }
    }
}
