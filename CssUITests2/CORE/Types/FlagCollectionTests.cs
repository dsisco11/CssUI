using System;
using System.Collections.Generic;
using System.Linq;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CORE.Types;

/// <summary>
/// Tests for FlagCollection - a bit-array based flag collection used for tracking CSS property IDs.
/// </summary>
public class FlagCollectionTests
{
    // Use int as a simple flag type for testing
    private FlagCollection<int>? _collection;

    #region Constructor Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Constructor_WithValidLength_CreatesEmptyCollection()
    {
        // Arrange & Act
        _collection = new FlagCollection<int>(100);

        // Assert
        Assert.Equal(0, _collection.ActiveFlags);
        Assert.True(_collection.IsEmpty());
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Constructor_InitializesMemoryToZero()
    {
        // Arrange & Act
        _collection = new FlagCollection<int>(64);

        // Assert - No flags should be set (memory should be zeroed)
        for (int i = 0; i < 64; i++)
        {
            Assert.False(_collection.GetFlag(i), $"Flag {i} should not be set after construction");
        }
    }

    #endregion

    #region SetFlag/GetFlag Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void SetFlag_SingleFlag_SetsCorrectly()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);

        // Act
        _collection.SetFlag(5);

        // Assert
        Assert.True(_collection.GetFlag(5));
        Assert.Equal(1, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void SetFlag_MultipleFlags_SetsAllCorrectly()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        var flagsToSet = new[] { 0, 1, 31, 32, 33, 63, 64, 99 };

        // Act
        foreach (var flag in flagsToSet)
        {
            _collection.SetFlag(flag);
        }

        // Assert
        Assert.Equal(flagsToSet.Length, _collection.ActiveFlags);
        foreach (var flag in flagsToSet)
        {
            Assert.True(_collection.GetFlag(flag), $"Flag {flag} should be set");
        }
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void SetFlag_SameFlagTwice_DoesNotDoubleCount()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);

        // Act
        _collection.SetFlag(5);
        _collection.SetFlag(5);

        // Assert
        Assert.Equal(1, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void ClearFlag_SetFlag_ClearsCorrectly()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(5);

        // Act
        _collection.ClearFlag(5);

        // Assert
        Assert.False(_collection.GetFlag(5));
        Assert.Equal(0, _collection.ActiveFlags);
    }

    #endregion

    #region Enumerator Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_EmptyCollection_ReturnsNoItems()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Empty(items);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_SingleFlag_ReturnsSingleItem()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(42);

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Single(items);
        Assert.Equal(42, items[0]);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_MultipleFlagsInSameChunk_ReturnsAllInOrder()
    {
        // Arrange - flags 0-31 are in the first chunk (32-bit)
        _collection = new FlagCollection<int>(100);
        var flagsToSet = new[] { 0, 5, 15, 31 };
        foreach (var flag in flagsToSet)
        {
            _collection.SetFlag(flag);
        }

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Equal(flagsToSet.Length, items.Count);
        Assert.Equal(flagsToSet, items);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_FlagsAcrossMultipleChunks_ReturnsAllInOrder()
    {
        // Arrange - test across chunk boundaries (32-bit chunks)
        _collection = new FlagCollection<int>(200);
        var flagsToSet = new[] { 0, 31, 32, 63, 64, 95, 96, 127, 128, 159, 160, 191 };
        foreach (var flag in flagsToSet)
        {
            _collection.SetFlag(flag);
        }

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Equal(flagsToSet.Length, items.Count);
        Assert.Equal(flagsToSet, items);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_CountMatchesActiveFlags()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        var flagsToSet = new[] { 3, 17, 42, 88 };
        foreach (var flag in flagsToSet)
        {
            _collection.SetFlag(flag);
        }

        // Act
        var enumerated = _collection.ToList();

        // Assert - This is the critical test for the freeze bug
        Assert.Equal(_collection.ActiveFlags, enumerated.Count);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_OnlyBit0Set_ReturnsFlag0()
    {
        // Arrange - This was the specific bug case
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(0);

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Single(items);
        Assert.Equal(0, items[0]);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void GetEnumerator_AllFlagsInFirstChunk_ReturnsAll32()
    {
        // Arrange
        _collection = new FlagCollection<int>(64);
        for (int i = 0; i < 32; i++)
        {
            _collection.SetFlag(i);
        }

        // Act
        var items = _collection.ToList();

        // Assert
        Assert.Equal(32, items.Count);
        for (int i = 0; i < 32; i++)
        {
            Assert.Equal(i, items[i]);
        }
    }

    #endregion

    #region Or Operation Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Or_EmptyCollections_StaysEmpty()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        var other = new FlagCollection<int>(100);

        // Act
        _collection.Or(other);

        // Assert
        Assert.Equal(0, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Or_WithSetFlags_CombinesFlags()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(1);
        _collection.SetFlag(3);

        var other = new FlagCollection<int>(100);
        other.SetFlag(2);
        other.SetFlag(4);

        // Act
        _collection.Or(other);

        // Assert
        Assert.Equal(4, _collection.ActiveFlags);
        Assert.True(_collection.GetFlag(1));
        Assert.True(_collection.GetFlag(2));
        Assert.True(_collection.GetFlag(3));
        Assert.True(_collection.GetFlag(4));
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Or_OverlappingFlags_DoesNotDuplicate()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(1);
        _collection.SetFlag(2);

        var other = new FlagCollection<int>(100);
        other.SetFlag(2);
        other.SetFlag(3);

        // Act
        _collection.Or(other);

        // Assert
        Assert.Equal(3, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Or_DifferentSizeCollections_HandlesSafely()
    {
        // Arrange - smaller target, larger source
        _collection = new FlagCollection<int>(50);
        _collection.SetFlag(10);

        var other = new FlagCollection<int>(100);
        other.SetFlag(20);
        other.SetFlag(80); // Beyond _collection's length

        // Act - should not crash
        _collection.Or(other);

        // Assert
        Assert.True(_collection.GetFlag(10));
        Assert.True(_collection.GetFlag(20));
        // Flag 80 is beyond our length, so we can't check it
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Or_UpdatesActiveFlagsCorrectly()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        var other = new FlagCollection<int>(100);

        var flagsInCollection = new[] { 5, 10, 15 };
        var flagsInOther = new[] { 10, 20, 30 };

        foreach (var f in flagsInCollection) _collection.SetFlag(f);
        foreach (var f in flagsInOther) other.SetFlag(f);

        // Act
        _collection.Or(other);

        // Assert - should have union: 5, 10, 15, 20, 30 = 5 flags
        Assert.Equal(5, _collection.ActiveFlags);
        Assert.Equal(5, _collection.ToList().Count);
    }

    #endregion

    #region And Operation Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void And_EmptyCollections_StaysEmpty()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        var other = new FlagCollection<int>(100);

        // Act
        _collection.And(other);

        // Assert
        Assert.Equal(0, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void And_NoOverlap_ClearsAll()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(1);
        _collection.SetFlag(3);

        var other = new FlagCollection<int>(100);
        other.SetFlag(2);
        other.SetFlag(4);

        // Act
        _collection.And(other);

        // Assert
        Assert.Equal(0, _collection.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void And_WithOverlap_KeepsOnlyCommon()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(1);
        _collection.SetFlag(2);
        _collection.SetFlag(3);

        var other = new FlagCollection<int>(100);
        other.SetFlag(2);
        other.SetFlag(3);
        other.SetFlag(4);

        // Act
        _collection.And(other);

        // Assert
        Assert.Equal(2, _collection.ActiveFlags);
        Assert.False(_collection.GetFlag(1));
        Assert.True(_collection.GetFlag(2));
        Assert.True(_collection.GetFlag(3));
        Assert.False(_collection.GetFlag(4));
    }

    #endregion

    #region Clear Tests

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Clear_WithSetFlags_ClearsAll()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(5);
        _collection.SetFlag(50);
        _collection.SetFlag(95);

        // Act
        _collection.Clear();

        // Assert
        Assert.Equal(0, _collection.ActiveFlags);
        Assert.True(_collection.IsEmpty());
        Assert.False(_collection.GetFlag(5));
        Assert.False(_collection.GetFlag(50));
        Assert.False(_collection.GetFlag(95));
    }

    #endregion

    #region Integration Tests (Simulating Cascade Scenario)

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Cascade_Scenario_EnumeratorCountMatchesActiveFlags()
    {
        // Arrange - Simulate the Cascade method's usage pattern
        _collection = new FlagCollection<int>(300); // Similar to ECssPropertyID.MAX_VALUE

        // Create multiple "rule sets" with different properties set
        var ruleSet1 = new FlagCollection<int>(300);
        var ruleSet2 = new FlagCollection<int>(300);
        var ruleSet3 = new FlagCollection<int>(300);

        ruleSet1.SetFlag(10); // width
        ruleSet1.SetFlag(20); // height

        ruleSet2.SetFlag(20); // height (duplicate)
        ruleSet2.SetFlag(30); // margin

        ruleSet3.SetFlag(40); // padding
        ruleSet3.SetFlag(50); // display

        // Act - Simulate: foreach (var fields in allFields) targetFields.Or(fields)
        _collection.Or(ruleSet1);
        _collection.Or(ruleSet2);
        _collection.Or(ruleSet3);

        // Assert - This is what causes the freeze if broken
        int activeFlags = _collection.ActiveFlags;
        var enumeratedFlags = _collection.ToList();

        Assert.Equal(activeFlags, enumeratedFlags.Count);
        Assert.Equal(5, activeFlags); // 10, 20, 30, 40, 50
    }

    [Fact]
    [Trait("Category", "FlagCollection")]
    public void Cascade_Scenario_CanIterateMultipleTimes()
    {
        // Arrange
        _collection = new FlagCollection<int>(100);
        _collection.SetFlag(1);
        _collection.SetFlag(5);
        _collection.SetFlag(10);

        // Act - Iterate twice to ensure enumerator is reusable
        var first = _collection.ToList();
        var second = _collection.ToList();

        // Assert
        Assert.Equal(first.Count, second.Count);
        Assert.Equal(first, second);
    }

    #endregion
}
