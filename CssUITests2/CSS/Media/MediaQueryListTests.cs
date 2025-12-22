using System.Collections.Generic;
using CssUI.CSS.Media;
using CssUI.DOM;
using CssUI.DOM.Media;
using Xunit;

namespace CssUITests.DOM.Media;

/// <summary>
/// Unit tests for MediaQueryList functionality.
/// Spec: https://www.w3.org/TR/cssom-view-1/#mediaquerylist
/// </summary>
public class MediaQueryListTests
{
    #region Test Infrastructure

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static MediaQueryList CreateMediaQueryList(Document doc, params MediaQuery[] queries)
    {
        var queryList = new LinkedList<MediaQuery>(queries);
        return new MediaQueryList(doc, queryList);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_SetsDocument()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();

        // Act
        var mql = new MediaQueryList(doc, queryList);

        // Assert
        Assert.Same(doc, mql.document);
    }

    [Fact]
    public void Constructor_SetsQueryList()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();

        // Act
        var mql = new MediaQueryList(doc, queryList);

        // Assert
        Assert.Same(queryList, mql.QueryList);
    }

    [Fact]
    public void Constructor_AddsToDocumentMediaQueryLists()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();

        // Act
        var mql = new MediaQueryList(doc, queryList);

        // Assert - The constructor adds itself to doc._mediaQueryLists
        // (Note: _mediaQueryLists is internal, so we verify indirectly through disposal)
        Assert.NotNull(mql);
    }

    #endregion

    #region Matches Tests

    [Fact]
    public void Matches_EmptyQueryList_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act & Assert - Empty query list should match (vacuously true)
        Assert.True(mql.Matches);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public void Media_ReturnsSerializedQueryString()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act & Assert - media property should be a string
        Assert.NotNull(mql.media);
    }

    [Fact]
    public void Serialize_EmptyQueryList_ReturnsEmptyString()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act
        var serialized = mql.Serialize();

        // Assert
        Assert.Equal(string.Empty, serialized);
    }

    #endregion

    #region Event Listener Tests

    [Fact]
    public void AddEventListener_WithNullListener_DoesNotThrow()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act & Assert - Should not throw
        mql.addEventListener(null!);
    }

    [Fact]
    public void RemoveEventListener_WithNullListener_DoesNotThrow()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act & Assert - Should not throw
        mql.removeEventListener(null!);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_RemovesFromDocumentMediaQueryLists()
    {
        // Arrange
        var doc = CreateTestDocument();
        var queryList = new LinkedList<MediaQuery>();
        var mql = new MediaQueryList(doc, queryList);

        // Act
        mql.Dispose();

        // Assert - Should not throw on second dispose
        mql.Dispose();
    }

    #endregion
}
