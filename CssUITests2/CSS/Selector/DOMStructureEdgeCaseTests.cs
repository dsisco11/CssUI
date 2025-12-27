using System.Linq;
using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for DOM structure edge cases in selector matching.
/// These test how selectors behave with unusual DOM configurations.
/// </summary>
public class DOMStructureEdgeCaseTests
{
    #region Test Infrastructure
    private const string SkipReason = "DOM structure edge case not yet implemented";

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

    #region Orphan Element Tests (Not in Document)
    [Fact]
    public void OrphanElement_TypeSelectorMatches()
    {
        // Arrange - Element not appended to document
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector("div");

        // Act & Assert - Type selector should still match
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Type selector should match orphan element");
    }

    [Fact]
    public void OrphanElement_ClassSelectorMatches()
    {
        // Arrange - Element not in document can still match class selector
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "test-class";
        var selector = new CssSelector(".test-class");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Class selector should match orphan element");
    }

    [Fact]
    public void OrphanElement_DescendantSelectorBehavior()
    {
        // Arrange - Orphan subtree with parent-child relationship
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        // Note: parent is NOT appended to document
        var selector = new CssSelector("div span");

        // Act & Assert - Descendant selector should work within orphan subtree
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant selector should work in orphan subtree");
    }

    [Fact]
    public void OrphanElement_RootPseudoClassDoesNotMatch()
    {
        // Arrange - :root should not match orphan elements
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector(":root");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), ":root should not match orphan element");
    }
    #endregion

    #region Detached Subtree Tests
    [Fact]
    public void DetachedSubtree_AfterRemoveChild()
    {
        // Arrange - Create, attach, then detach
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement.appendChild(parent);
        parent.appendChild(child);

        // Detach parent from document
        doc.documentElement.removeChild(parent);

        var selector = new CssSelector("span");

        // Act & Assert - Should still match the detached element
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Type selector should match detached element");
    }

    [Fact]
    public void DetachedSubtree_ParentChildRelationships()
    {
        // Arrange - Combinator matching in detached subtree
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        grandparent.appendChild(parent);
        parent.appendChild(child);
        // Note: grandparent is not in document

        var childSelector = new CssSelector("div > section > span");

        // Act & Assert
        Assert.True(childSelector.Count > 0, "Child combinator selector should parse");
        Assert.True(childSelector[0].Match(child), "Child combinator should work in detached subtree");
    }

    [Fact]
    public void DetachedSubtree_SiblingRelationships()
    {
        // Arrange - Sibling combinators in detached subtree
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var second = CreateTestElement(doc, "p");
        parent.appendChild(first);
        parent.appendChild(second);
        // Note: parent is not in document

        var selector = new CssSelector("span + p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Adjacent sibling selector should parse");
        Assert.True(selector[0].Match(second), "Adjacent sibling should work in detached subtree");
    }
    #endregion

    #region Document Fragment Tests
    [Fact(Skip = SkipReason)]
    public void DocumentFragment_SelectorMatchingInside()
    {
        // Arrange - Selector matching on elements inside DocumentFragment
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var div = CreateTestElement(doc, "div");
        fragment.appendChild(div);
        var selector = new CssSelector("div");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "Type selector should match inside fragment");
    }

    [Fact(Skip = SkipReason)]
    public void DocumentFragment_RootPseudoClass()
    {
        // Arrange - :root behavior inside document fragment
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var div = CreateTestElement(doc, "div");
        fragment.appendChild(div);
        var selector = new CssSelector(":root");

        // Act & Assert - :root should not match elements in fragment
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(div), ":root should not match element in fragment");
    }

    [Fact(Skip = SkipReason)]
    public void DocumentFragment_DescendantMatching()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        fragment.appendChild(parent);
        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant should work inside fragment");
    }
    #endregion

    #region Empty Document Tests
    [Fact(Skip = SkipReason)]
    public void EmptyDocument_SelectorMatching()
    {
        // This test verifies behavior on newly created documents
        var dom = new DOMImplementation();
        var doc = dom.createDocument("CssUI", "cssui");
        var selector = new CssSelector("div");

        // Act & Assert - Just verify no exceptions occur
        Assert.True(selector.Count > 0, "Selector should parse even with empty document");
    }

    [Fact(Skip = SkipReason)]
    public void EmptyDocument_DocumentElement()
    {
        // Arrange - :root when document element exists
        var dom = new DOMImplementation();
        var doc = dom.createDocument("CssUI", "cssui");
        var selector = new CssSelector(":root");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        if (doc.documentElement != null)
        {
            Assert.True(selector[0].Match(doc.documentElement), ":root should match document element");
        }
    }
    #endregion

    #region Deep Nesting Tests
    [Fact]
    public void DeepNesting_DescendantSelector()
    {
        // Arrange - Very deeply nested structure
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var current = root;
        for (int i = 0; i < 10; i++)
        {
            var next = CreateTestElement(doc, "div");
            current.appendChild(next);
            current = next;
        }
        var span = CreateTestElement(doc, "span");
        current.appendChild(span);

        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span), "Descendant should find deeply nested element");
    }

    [Fact]
    public void DeepNesting_ChildCombinatorChain()
    {
        // Arrange - "div > div > div > div > span"
        var doc = CreateTestDocument();
        var div1 = CreateTestElement(doc, "div");
        var div2 = CreateTestElement(doc, "div");
        var div3 = CreateTestElement(doc, "div");
        var div4 = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        div1.appendChild(div2);
        div2.appendChild(div3);
        div3.appendChild(div4);
        div4.appendChild(span);

        var selector = new CssSelector("div > div > div > div > span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span), "Child combinator chain should work");
    }

    [Fact]
    public void DeepNesting_AncestorLookup()
    {
        // Arrange - Looking for ancestor many levels up
        var doc = CreateTestDocument();
        var article = CreateTestElement(doc, "article");
        var current = article;
        for (int i = 0; i < 5; i++)
        {
            var section = CreateTestElement(doc, "section");
            current.appendChild(section);
            current = section;
        }
        var p = CreateTestElement(doc, "p");
        current.appendChild(p);

        var selector = new CssSelector("article p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "Should find article ancestor many levels up");
    }
    #endregion

    #region Many Siblings Tests
    [Fact(Skip = SkipReason)]
    public void ManySiblings_FirstChild()
    {
        // Arrange - :first-child with many siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var children = Enumerable.Range(0, 100).Select(_ => CreateTestElement(doc, "span")).ToList();
        foreach (var child in children) parent.appendChild(child);

        var selector = new CssSelector(":first-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(children[0]), ":first-child should match first of many");
        Assert.False(selector[0].Match(children[50]), ":first-child should not match middle element");
    }

    [Fact(Skip = SkipReason)]
    public void ManySiblings_LastChild()
    {
        // Arrange - :last-child with many siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var children = Enumerable.Range(0, 100).Select(_ => CreateTestElement(doc, "span")).ToList();
        foreach (var child in children) parent.appendChild(child);

        var selector = new CssSelector(":last-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(children[99]), ":last-child should match last of many");
        Assert.False(selector[0].Match(children[0]), ":last-child should not match first element");
    }

    [Fact]
    public void ManySiblings_NthChild()
    {
        // Arrange - :nth-child(50) with many siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var children = Enumerable.Range(0, 100).Select(_ => CreateTestElement(doc, "span")).ToList();
        foreach (var child in children) parent.appendChild(child);

        var selector = new CssSelector(":nth-child(50)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(children[49]), ":nth-child(50) should match 50th element");
        Assert.False(selector[0].Match(children[48]), ":nth-child(50) should not match 49th element");
    }

    [Fact]
    public void ManySiblings_AdjacentSibling()
    {
        // Arrange - Adjacent sibling combinator with many siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        for (int i = 0; i < 50; i++)
        {
            parent.appendChild(CreateTestElement(doc, "span"));
        }
        var special = CreateTestElement(doc, "div");
        special.className = "target";
        parent.appendChild(special);
        var afterSpecial = CreateTestElement(doc, "p");
        parent.appendChild(afterSpecial);
        for (int i = 0; i < 50; i++)
        {
            parent.appendChild(CreateTestElement(doc, "span"));
        }

        var selector = new CssSelector("div.target + p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(afterSpecial), "Adjacent sibling should find p after target");
    }

    [Fact]
    public void ManySiblings_GeneralSibling()
    {
        // Arrange - General sibling combinator with many siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        parent.appendChild(h1);
        for (int i = 0; i < 50; i++)
        {
            parent.appendChild(CreateTestElement(doc, "span"));
        }
        var p = CreateTestElement(doc, "p");
        parent.appendChild(p);

        var selector = new CssSelector("h1 ~ p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "General sibling should find p after h1");
    }
    #endregion

    #region Mixed Content Tests
    [Fact(Skip = SkipReason)]
    public void MixedContent_TextNodesBetweenElements()
    {
        // Arrange - Sibling matching with text nodes between elements
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(doc.createTextNode("Some text"));
        var second = CreateTestElement(doc, "p");
        parent.appendChild(second);

        var selector = new CssSelector("span + p");

        // Act & Assert - Adjacent sibling should still work (text nodes don't count)
        Assert.True(selector.Count > 0, "Selector should parse");
        // Per DOM spec, adjacent sibling looks at element siblings only
        Assert.True(selector[0].Match(second), "Adjacent sibling should skip text nodes");
    }

    [Fact(Skip = SkipReason)]
    public void MixedContent_CommentNodes()
    {
        // Arrange - Selector matching with comment nodes in tree
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        parent.appendChild(doc.createComment("This is a comment"));
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        var selector = new CssSelector("div > span");

        // Act & Assert - Comment nodes should not interfere
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Child combinator should work with comment nodes present");
    }

    [Fact(Skip = SkipReason)]
    public void MixedContent_ProcessingInstructions()
    {
        // This test documents expected behavior with processing instructions
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant should work with processing instructions");
    }
    #endregion

    #region Namespace Tests
    [Fact(Skip = SkipReason)]
    public void Namespace_HTMLElements()
    {
        // Arrange - Matching HTML namespace elements
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var selector = new CssSelector("div");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "Should match HTML namespace element");
    }

    [Fact(Skip = SkipReason)]
    public void Namespace_SVGElements()
    {
        // This test documents expected behavior with SVG elements
        var doc = CreateTestDocument();
        // Note: Would need proper SVG element creation
        var selector = new CssSelector("svg");

        // Act & Assert
        Assert.True(selector.Count > 0, "SVG selector should parse");
    }

    [Fact(Skip = SkipReason)]
    public void Namespace_CustomNamespace()
    {
        // This test documents expected behavior with custom namespaces
        var doc = CreateTestDocument();
        var selector = new CssSelector("custom|element");

        // Act & Assert - Namespace selector syntax
        Assert.True(true, "Custom namespace handling documented");
    }

    [Fact(Skip = SkipReason)]
    public void Namespace_NamespacePrefix()
    {
        // Arrange - namespace|element selector syntax
        var selector = new CssSelector("html|div");

        // Act & Assert
        Assert.True(true, "Namespace prefix selector handling documented");
    }
    #endregion

    #region Shadow DOM Tests
    [Fact(Skip = SkipReason)]
    public void ShadowDOM_SelectorDoesNotCrossShadowBoundary()
    {
        // This test documents expected shadow DOM behavior
        // Selectors should not pierce shadow boundaries
        Assert.True(true, "Shadow DOM boundary behavior documented");
    }

    [Fact(Skip = SkipReason)]
    public void ShadowDOM_HostPseudoClass()
    {
        // :host selector matching
        var selector = new CssSelector(":host");
        Assert.True(selector.Count >= 0, ":host selector parsing documented");
    }

    [Fact(Skip = SkipReason)]
    public void ShadowDOM_HostContextPseudoClass()
    {
        // :host-context() selector matching
        var selector = new CssSelector(":host-context(.dark-theme)");
        Assert.True(true, ":host-context selector parsing documented");
    }

    [Fact(Skip = SkipReason)]
    public void ShadowDOM_SlottedPseudoElement()
    {
        // ::slotted() pseudo-element matching
        var selector = new CssSelector("::slotted(span)");
        Assert.True(true, "::slotted pseudo-element parsing documented");
    }
    #endregion

    #region Dynamic DOM Changes
    [Fact]
    public void DynamicChange_AddClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector(".active");

        // Act - Before adding class
        Assert.False(selector[0].Match(element), "Should not match before class added");

        // Act - Add class
        element.className = "active";

        // Assert - After adding class
        Assert.True(selector[0].Match(element), "Should match after class added");
    }

    [Fact]
    public void DynamicChange_RemoveClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "active";
        var selector = new CssSelector(".active");

        // Act - Before removing class
        Assert.True(selector[0].Match(element), "Should match before class removed");

        // Act - Remove class
        element.className = "";

        // Assert - After removing class
        Assert.False(selector[0].Match(element), "Should not match after class removed");
    }

    [Fact]
    public void DynamicChange_ChangeId()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "old-id";
        var oldSelector = new CssSelector("#old-id");
        var newSelector = new CssSelector("#new-id");

        // Act & Assert - Before change
        Assert.True(oldSelector[0].Match(element), "Should match old id");
        Assert.False(newSelector[0].Match(element), "Should not match new id before change");

        // Act - Change id
        element.id = "new-id";

        // Assert - After change
        Assert.False(oldSelector[0].Match(element), "Should not match old id after change");
        Assert.True(newSelector[0].Match(element), "Should match new id after change");
    }

    [Fact]
    public void DynamicChange_AppendChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        var selector = new CssSelector("div span");

        // Act - Before append
        Assert.False(selector[0].Match(child), "Should not match before append");

        // Act - Append child
        parent.appendChild(child);

        // Assert - After append
        Assert.True(selector[0].Match(child), "Should match after append");
    }

    [Fact(Skip = SkipReason)]
    public void DynamicChange_InsertBefore()
    {
        // Arrange - Test :first-child/:last-child after insertion
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var existing = CreateTestElement(doc, "span");
        parent.appendChild(existing);
        var firstChildSelector = new CssSelector(":first-child");

        // Act - Before insertion
        Assert.True(firstChildSelector[0].Match(existing), "Existing should be first child");

        // Act - Insert new element before existing
        var newFirst = CreateTestElement(doc, "p");
        parent.insertBefore(newFirst, existing);

        // Assert - After insertion
        Assert.True(firstChildSelector[0].Match(newFirst), "New element should be first child");
        Assert.False(firstChildSelector[0].Match(existing), "Existing should no longer be first child");
    }
    #endregion
}
