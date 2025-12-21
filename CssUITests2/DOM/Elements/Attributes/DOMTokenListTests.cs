using System.Collections.Generic;
using System.Linq;
using CssUI;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.Enums;
using Xunit;

namespace CssUITests.DOM.Elements.Attributes;

/// <summary>
/// Unit tests for DOMTokenList (classList) to verify it doesn't cause infinite loops.
/// </summary>
public class DOMTokenListTests
{
    #region Test Infrastructure

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    #endregion

    #region Basic classList Tests

    [Fact]
    public void ClassList_EmptyByDefault()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(element);

        // Assert
        Assert.Empty(element.classList);
    }

    [Fact]
    public void ClassList_SingleClass_IsAccessible()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        doc.documentElement!.appendChild(element);

        // Assert
        Assert.Single(element.classList);
    }

    [Fact]
    public void ClassList_MultipleClasses_AreAccessible()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "class1 class2 class3";
        doc.documentElement!.appendChild(element);

        // Assert
        Assert.Equal(3, element.classList.Length);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_ExistingClass_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.True(element.classList.Contains("test-class"));
    }

    [Fact]
    public void Contains_NonExistingClass_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.False(element.classList.Contains("other-class"));
    }

    [Fact]
    public void Contains_EmptyClassList_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.False(element.classList.Contains("any-class"));
    }

    [Fact]
    public void Contains_CaseSensitive_ReturnsCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "TestClass";
        doc.documentElement!.appendChild(element);

        // Assert - className should be case-insensitive for HTML
        Assert.True(element.classList.Contains("TestClass"));
        // Note: In non-HTML documents, this may be case-sensitive
    }

    #endregion

    #region ContainsAll Tests

    [Fact]
    public void ContainsAll_SingleClass_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        doc.documentElement!.appendChild(element);

        // Act
        var classes = new[] { new AtomicString("test-class", EAtomicStringFlags.CaseInsensitive) };
        var result = element.classList.ContainsAll(classes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsAll_MultipleClasses_AllPresent_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "class1 class2 class3";
        doc.documentElement!.appendChild(element);

        // Act
        var classes = new[]
        {
            new AtomicString("class1", EAtomicStringFlags.CaseInsensitive),
            new AtomicString("class2", EAtomicStringFlags.CaseInsensitive)
        };
        var result = element.classList.ContainsAll(classes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsAll_MultipleClasses_OneMissing_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "class1 class2";
        doc.documentElement!.appendChild(element);

        // Act
        var classes = new[]
        {
            new AtomicString("class1", EAtomicStringFlags.CaseInsensitive),
            new AtomicString("class3", EAtomicStringFlags.CaseInsensitive)
        };
        var result = element.classList.ContainsAll(classes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ContainsAll_EmptySearch_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        doc.documentElement!.appendChild(element);

        // Act
        var classes = System.Array.Empty<AtomicString>();
        var result = element.classList.ContainsAll(classes);

        // Assert - Empty set is always contained
        Assert.True(result);
    }

    [Fact]
    public void ContainsAll_NoInfiniteLoop()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "class1 class2 class3";
        doc.documentElement!.appendChild(element);

        // Act - This should complete quickly, not infinite loop
        var classes = new[]
        {
            new AtomicString("class1", EAtomicStringFlags.CaseInsensitive),
            new AtomicString("class2", EAtomicStringFlags.CaseInsensitive)
        };

        var task = System.Threading.Tasks.Task.Run(() => element.classList.ContainsAll(classes));
        var completed = task.Wait(System.TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(completed, "ContainsAll appears to be in an infinite loop");
        Assert.True(task.Result);
    }

    #endregion

    #region Integration Tests (simulating getElementsByClassName)

    [Fact]
    public void Integration_GetElementsByClassName_DoesNotInfiniteLoop()
    {
        // Arrange - This replicates the exact scenario from GetElementsByClassName_SingleClass_ReturnsMatching
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        element1.className = "test-class";
        element2.className = "other-class";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Act - With timeout protection
        System.Collections.Generic.List<Element>? results = null;
        var task = System.Threading.Tasks.Task.Run(() =>
        {
            results = doc.getElementsByClassName("test-class").ToList();
        });

        var completed = task.Wait(System.TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(completed, "getElementsByClassName appears to be in an infinite loop");
        Assert.NotNull(results);
        Assert.Single(results!);
        Assert.Same(element1, results![0]);
    }

    #endregion
}
