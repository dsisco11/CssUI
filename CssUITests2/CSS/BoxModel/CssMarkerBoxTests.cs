using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Tests for list-item marker box generation per CSS Display 3 §2.3 and CSS Lists 3.
/// </summary>
[Trait("Category", "ListItem")]
[Trait("Category", "BoxGeneration")]
public class CssMarkerBoxTests
{
    private static Element CreateListItem(LayoutTestFixture fixture, Element parent, EListStyleType markerType = EListStyleType.Disc)
    {
        var element = fixture.Document.createElement("li", new ElementCreationOptions(string.Empty));
        parent.appendChild(element);
        element.Style.UserRules.Display.Set(EDisplayMode.LIST_ITEM);
        element.Style.UserRules.ListStyleType.Set(markerType);
        return element;
    }

    private static Element CreateElement(LayoutTestFixture fixture, string tagName, Element parent)
    {
        var element = fixture.Document.createElement(tagName, new ElementCreationOptions(string.Empty));
        parent.appendChild(element);
        return element;
    }

    #region Marker Box Generation Tests

    [Fact]
    public void ListItem_GeneratesMarkerBox()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(listItem.Box);
        Assert.True(listItem.Box.childNodes.Count > 0, "List item should have children (marker box)");
        Assert.IsType<CssMarkerBox>(listItem.Box.firstChild);
    }

    [Fact]
    public void ListItem_MarkerBox_IsFirstChild()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);
        listItem.textContent = "Item text";

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(listItem.Box);
        Assert.IsType<CssMarkerBox>(listItem.Box.firstChild);
    }

    [Fact]
    public void ListItem_WithNoneMarkerType_NoMarkerBox()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.None);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(listItem.Box);
        // No marker box should be generated when list-style-type is none
        var firstChild = listItem.Box.firstChild;
        if (firstChild is not null)
        {
            Assert.IsNotType<CssMarkerBox>(firstChild);
        }
    }

    #endregion

    #region Marker Type Tests

    [Theory]
    [InlineData(EListStyleType.Disc, "\u2022 ")] // •
    [InlineData(EListStyleType.Circle, "\u25CB ")] // ○
    [InlineData(EListStyleType.Square, "\u25A0 ")] // ■
    public void MarkerBox_BulletTypes_GeneratesCorrectString(EListStyleType markerType, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, markerType);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(EListStyleType.Decimal, 1, "1. ")]
    [InlineData(EListStyleType.Decimal, 5, "5. ")]
    [InlineData(EListStyleType.Decimal, 10, "10. ")]
    public void MarkerBox_DecimalType_GeneratesCorrectNumber(EListStyleType markerType, int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, markerType);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(EListStyleType.DecimalLeadingZero, 1, "01. ")]
    [InlineData(EListStyleType.DecimalLeadingZero, 9, "09. ")]
    [InlineData(EListStyleType.DecimalLeadingZero, 10, "10. ")]
    public void MarkerBox_DecimalLeadingZero_GeneratesCorrectNumber(EListStyleType markerType, int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, markerType);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(1, "a. ")]
    [InlineData(2, "b. ")]
    [InlineData(26, "z. ")]
    [InlineData(27, "aa. ")]
    public void MarkerBox_LowerAlpha_GeneratesCorrectLetter(int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.LowerAlpha);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(1, "A. ")]
    [InlineData(2, "B. ")]
    [InlineData(26, "Z. ")]
    [InlineData(27, "AA. ")]
    public void MarkerBox_UpperAlpha_GeneratesCorrectLetter(int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.UpperAlpha);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(1, "i. ")]
    [InlineData(4, "iv. ")]
    [InlineData(5, "v. ")]
    [InlineData(9, "ix. ")]
    [InlineData(10, "x. ")]
    [InlineData(50, "l. ")]
    [InlineData(100, "c. ")]
    [InlineData(500, "d. ")]
    [InlineData(1000, "m. ")]
    public void MarkerBox_LowerRoman_GeneratesCorrectNumeral(int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.LowerRoman);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(1, "I. ")]
    [InlineData(4, "IV. ")]
    [InlineData(5, "V. ")]
    [InlineData(9, "IX. ")]
    [InlineData(10, "X. ")]
    public void MarkerBox_UpperRoman_GeneratesCorrectNumeral(int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.UpperRoman);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    [Theory]
    [InlineData(1, "\u03B1. ")] // α
    [InlineData(2, "\u03B2. ")] // β
    [InlineData(3, "\u03B3. ")] // γ
    public void MarkerBox_LowerGreek_GeneratesCorrectLetter(int counterValue, string expectedMarker)
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body, EListStyleType.LowerGreek);

        // Act
        fixture.ForceLayoutUpdate();
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        markerBox?.SetCounterValue(counterValue);

        // Assert
        Assert.NotNull(markerBox);
        Assert.Equal(expectedMarker, markerBox.MarkerString);
    }

    #endregion

    #region Marker Position Tests

    [Fact]
    public void MarkerBox_OutsidePosition_IsBlockLevel()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);
        listItem.Style.UserRules.ListStylePosition.Set(EListStylePosition.Outside);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        Assert.NotNull(markerBox);
        Assert.Equal(EListStylePosition.Outside, markerBox.Position);
        Assert.True(markerBox.DisplayType.IsBlockLevel);
    }

    [Fact]
    public void MarkerBox_InsidePosition_IsInlineLevel()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);
        listItem.Style.UserRules.ListStylePosition.Set(EListStylePosition.Inside);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var markerBox = listItem.Box?.firstChild as CssMarkerBox;
        Assert.NotNull(markerBox);
        Assert.Equal(EListStylePosition.Inside, markerBox.Position);
        Assert.True(markerBox.DisplayType.IsInlineLevel);
    }

    #endregion

    #region Counter Value Tests

    [Fact]
    public void MultipleListItems_HaveSequentialCounterValues()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var ul = CreateElement(fixture, "ul", fixture.Body);

        var item1 = CreateListItem(fixture, ul, EListStyleType.Decimal);
        var item2 = CreateListItem(fixture, ul, EListStyleType.Decimal);
        var item3 = CreateListItem(fixture, ul, EListStyleType.Decimal);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var marker1 = item1.Box?.firstChild as CssMarkerBox;
        var marker2 = item2.Box?.firstChild as CssMarkerBox;
        var marker3 = item3.Box?.firstChild as CssMarkerBox;

        Assert.NotNull(marker1);
        Assert.NotNull(marker2);
        Assert.NotNull(marker3);

        Assert.Equal(1, marker1.CounterValue);
        Assert.Equal(2, marker2.CounterValue);
        Assert.Equal(3, marker3.CounterValue);
    }

    [Fact]
    public void NestedListItems_HaveIndependentCounters()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        
        var outerUl = CreateElement(fixture, "ul", fixture.Body);

        var outerItem1 = CreateListItem(fixture, outerUl, EListStyleType.Decimal);

        var innerUl = CreateElement(fixture, "ul", outerItem1);

        var innerItem1 = CreateListItem(fixture, innerUl, EListStyleType.Decimal);
        var innerItem2 = CreateListItem(fixture, innerUl, EListStyleType.Decimal);

        var outerItem2 = CreateListItem(fixture, outerUl, EListStyleType.Decimal);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert - outer list counters
        var outerMarker1 = outerItem1.Box?.firstChild as CssMarkerBox;
        var outerMarker2 = outerItem2.Box?.firstChild as CssMarkerBox;
        Assert.NotNull(outerMarker1);
        Assert.NotNull(outerMarker2);
        Assert.Equal(1, outerMarker1.CounterValue);
        Assert.Equal(2, outerMarker2.CounterValue);

        // Assert - inner list counters (should restart from 1)
        var innerMarker1 = innerItem1.Box?.firstChild as CssMarkerBox;
        var innerMarker2 = innerItem2.Box?.firstChild as CssMarkerBox;
        Assert.NotNull(innerMarker1);
        Assert.NotNull(innerMarker2);
        Assert.Equal(1, innerMarker1.CounterValue);
        Assert.Equal(2, innerMarker2.CounterValue);
    }

    #endregion

    #region DisplayType Tests

    [Fact]
    public void ListItem_HasBlockOuterDisplayType()
    {
        // Per CSS Display 3 §2.3: If no outer display type value is specified,
        // the principal box's outer display type defaults to 'block'.
        
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var displayType = new DisplayType(listItem.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
    }

    [Fact]
    public void ListItem_HasFlowRootInnerDisplayType()
    {
        // Per CSS Display 3 §2.3: If no inner display type value is specified,
        // the principal box's inner display type defaults to 'flow'.
        // However, our implementation uses Flow_Root for block containers.
        
        // Arrange
        using var fixture = new LayoutTestFixture();
        var listItem = CreateListItem(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var displayType = new DisplayType(listItem.Style.Display);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    #endregion
}
