using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.ShadowDOM;

/// <summary>
/// Unit tests for ShadowRoot and Shadow DOM functionality.
/// Spec: https://dom.spec.whatwg.org/#shadowroot
/// </summary>
public class ShadowRootTests
{
    #region Test Infrastructure

    private static Document CreateHTMLDocument()
    {
        var dom = new DOMImplementation();
        return dom.createHTMLDocument("Test");
    }

    private static Element CreateShadowableElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    #endregion

    #region attachShadow Open Mode Tests

    [Fact]
    public void AttachShadow_OpenMode_ReturnsShadowRoot()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert
        Assert.NotNull(shadowRoot);
        Assert.IsType<ShadowRoot>(shadowRoot);
    }

    [Fact]
    public void AttachShadow_OpenMode_SetsCorrectMode()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert
        Assert.Equal(EShadowRootMode.Open, shadowRoot.Mode);
    }

    [Fact]
    public void AttachShadow_OpenMode_ShadowRootPropertyReturnsIt()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert - Per DOM spec, shadowRoot property should return the shadow root for open mode
        Assert.Same(shadowRoot, div.shadowRoot);
    }

    [Fact]
    public void AttachShadow_OpenMode_ShadowRootHostIsElement()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert
        Assert.Same(div, shadowRoot.Host);
    }

    #endregion

    #region attachShadow Closed Mode Tests

    [Fact]
    public void AttachShadow_ClosedMode_ReturnsShadowRoot()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Assert
        Assert.NotNull(shadowRoot);
        Assert.IsType<ShadowRoot>(shadowRoot);
    }

    [Fact]
    public void AttachShadow_ClosedMode_SetsCorrectMode()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Assert
        Assert.Equal(EShadowRootMode.Closed, shadowRoot.Mode);
    }

    [Fact]
    public void AttachShadow_ClosedMode_ShadowRootPropertyReturnsNull()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Assert - Per DOM spec, shadowRoot property should return null for closed mode
        Assert.Null(div.shadowRoot);
    }

    [Fact]
    public void AttachShadow_ClosedMode_ShadowRootHostIsElement()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Assert - Even though shadowRoot property returns null, Host should still work
        Assert.Same(div, shadowRoot.Host);
    }

    #endregion

    #region attachShadow Validation Tests

    [Fact]
    public void AttachShadow_AlreadyHasOpenShadow_ThrowsNotSupportedError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Act & Assert
        Assert.Throws<NotSupportedError>(() =>
            div.attachShadow(new ShadowRootInit(EShadowRootMode.Open)));
    }

    [Fact]
    public void AttachShadow_AlreadyHasClosedShadow_ThrowsNotSupportedError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Act & Assert - Should throw even though shadowRoot returns null
        Assert.Throws<NotSupportedError>(() =>
            div.attachShadow(new ShadowRootInit(EShadowRootMode.Open)));
    }

    [Fact]
    public void AttachShadow_InvalidElement_ThrowsNotSupportedError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var img = CreateShadowableElement(doc, "img"); // img is not a shadowable element
        doc.body!.appendChild(img);

        // Act & Assert
        Assert.Throws<NotSupportedError>(() =>
            img.attachShadow(new ShadowRootInit(EShadowRootMode.Open)));
    }

    #endregion

    #region Shadowable Elements Tests

    [Theory]
    [InlineData("article")]
    [InlineData("aside")]
    [InlineData("blockquote")]
    [InlineData("body")]
    [InlineData("div")]
    [InlineData("footer")]
    [InlineData("h1")]
    [InlineData("h2")]
    [InlineData("h3")]
    [InlineData("h4")]
    [InlineData("h5")]
    [InlineData("h6")]
    [InlineData("header")]
    [InlineData("main")]
    [InlineData("nav")]
    [InlineData("p")]
    [InlineData("section")]
    [InlineData("span")]
    public void AttachShadow_StandardShadowableElements_Succeeds(string tagName)
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var element = CreateShadowableElement(doc, tagName);
        // Note: body element is special, we need to use a different parent
        if (tagName != "body")
        {
            doc.body!.appendChild(element);
        }

        // Act
        var shadowRoot = element.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert
        Assert.NotNull(shadowRoot);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("button")]
    [InlineData("img")]
    [InlineData("input")]
    [InlineData("table")]
    [InlineData("ul")]
    [InlineData("li")]
    [InlineData("form")]
    [InlineData("select")]
    [InlineData("textarea")]
    public void AttachShadow_NonShadowableElements_ThrowsNotSupportedError(string tagName)
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var element = CreateShadowableElement(doc, tagName);
        doc.body!.appendChild(element);

        // Act & Assert
        Assert.Throws<NotSupportedError>(() =>
            element.attachShadow(new ShadowRootInit(EShadowRootMode.Open)));
    }

    #endregion

    #region Shadow Root Content Tests

    [Fact]
    public void ShadowRoot_AppendChild_AddsToShadowTree()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));
        var span = CreateShadowableElement(doc, "span");

        // Act
        shadowRoot.appendChild(span);

        // Assert
        Assert.Same(span, shadowRoot.firstChild);
        Assert.Equal(1, shadowRoot.childNodes.Count);
    }

    [Fact]
    public void ShadowRoot_MultipleChildren_MaintainsOrder()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));
        var child1 = CreateShadowableElement(doc, "p");
        var child2 = CreateShadowableElement(doc, "span");
        var child3 = CreateShadowableElement(doc, "div");

        // Act
        shadowRoot.appendChild(child1);
        shadowRoot.appendChild(child2);
        shadowRoot.appendChild(child3);

        // Assert
        Assert.Same(child1, shadowRoot.firstChild);
        Assert.Same(child3, shadowRoot.lastChild);
        Assert.Equal(3, shadowRoot.childNodes.Count);
    }

    [Fact]
    public void ShadowRoot_TextContent_SetsAndGets()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Act
        shadowRoot.textContent = "Shadow content";

        // Assert
        Assert.Equal("Shadow content", shadowRoot.textContent);
    }

    [Fact]
    public void ShadowRoot_InnerHTML_Isolation()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));
        var shadowSpan = CreateShadowableElement(doc, "span");
        shadowSpan.textContent = "Shadow content";
        shadowRoot.appendChild(shadowSpan);

        // Act - Add content to the host element (light DOM)
        var lightSpan = CreateShadowableElement(doc, "span");
        lightSpan.textContent = "Light content";
        div.appendChild(lightSpan);

        // Assert - Shadow DOM and light DOM are separate
        Assert.Single(shadowRoot.childNodes);
        Assert.Single(div.childNodes);
        Assert.NotSame(shadowRoot.firstChild, div.firstChild);
    }

    #endregion

    #region ShadowRoot as DocumentFragment Tests

    [Fact]
    public void ShadowRoot_IsDocumentFragment()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert - ShadowRoot inherits from DocumentFragment
        Assert.IsAssignableFrom<DocumentFragment>(shadowRoot);
    }

    [Fact]
    public void ShadowRoot_NodeType_IsDocumentFragment()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var shadowRoot = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Assert
        Assert.Equal(ENodeType.DOCUMENT_FRAGMENT_NODE, shadowRoot.nodeType);
    }

    #endregion

    #region shadowRoot Property Access Tests

    [Fact]
    public void ShadowRoot_PropertyBeforeAttach_ReturnsNull()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);

        // Act & Assert - Before attaching shadow, property should be null
        Assert.Null(div.shadowRoot);
    }

    [Fact]
    public void ShadowRoot_OpenModeAccessFromHost_Succeeds()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        var attachedShadow = div.attachShadow(new ShadowRootInit(EShadowRootMode.Open));

        // Act
        var retrievedShadow = div.shadowRoot;

        // Assert
        Assert.NotNull(retrievedShadow);
        Assert.Same(attachedShadow, retrievedShadow);
    }

    [Fact]
    public void ShadowRoot_ClosedModeAccessFromHost_ReturnsNull()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateShadowableElement(doc, "div");
        doc.body!.appendChild(div);
        div.attachShadow(new ShadowRootInit(EShadowRootMode.Closed));

        // Act
        var retrievedShadow = div.shadowRoot;

        // Assert
        Assert.Null(retrievedShadow);
    }

    #endregion
}
