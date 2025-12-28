using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUITests.Fixtures;

/// <summary>
/// Test fixture for layout and BoxModel testing.
/// Provides helpers to create Document→Element→Box chains with configured styles.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// var fixture = new LayoutTestFixture();
/// var div = fixture.CreateElement("div", style => {
///     style.Width.Set(200);
///     style.Height.Set(100);
/// });
/// fixture.ForceLayoutUpdate();
/// Assert.Equal(200, div.Box.Content.Width);
/// </code>
/// </remarks>
public class LayoutTestFixture : IDisposable
{
    #region Properties

    /// <summary>
    /// Default element creation options.
    /// </summary>
    private static readonly ElementCreationOptions DefaultOptions = new(string.Empty);

    /// <summary>
    /// The test document.
    /// </summary>
    public HTMLDocument Document { get; }

    /// <summary>
    /// The document element (html).
    /// </summary>
    public Element DocumentElement => Document.documentElement!;

    /// <summary>
    /// The body element for appending test elements.
    /// </summary>
    public Element Body { get; }

    /// <summary>
    /// Default viewport width for tests.
    /// </summary>
    public int ViewportWidth { get; set; } = 800;

    /// <summary>
    /// Default viewport height for tests.
    /// </summary>
    public int ViewportHeight { get; set; } = 600;

    #endregion

    #region Constructor

    public LayoutTestFixture()
    {
        Document = new HTMLDocument();

        // Create basic document structure: <html><body></body></html>
        var html = Document.createElement("html", DefaultOptions);
        Document.appendChild(html);

        Body = Document.createElement("body", DefaultOptions);
        html.appendChild(Body);

        // Set default styles on document element for containing block
        ConfigureRootStyles();
    }

    private void ConfigureRootStyles()
    {
        // Set the document element as a block with viewport dimensions
        DocumentElement.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        DocumentElement.Style.UserRules.Width.Set(ViewportWidth);
        DocumentElement.Style.UserRules.Height.Set(ViewportHeight);

        // Body fills the viewport by default
        Body.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        Body.Style.UserRules.Width.Set(ViewportWidth);
    }

    #endregion

    #region Element Creation

    /// <summary>
    /// Creates an element with configured styles and appends it to the body.
    /// </summary>
    /// <param name="tagName">The element tag name (e.g., "div", "span").</param>
    /// <param name="configureStyle">Optional action to configure the element's styles.</param>
    /// <returns>The created element.</returns>
    public Element CreateElement(string tagName, Action<CssComputedStyle>? configureStyle = null)
    {
        var element = Document.createElement(tagName, DefaultOptions);
        Body.appendChild(element);

        configureStyle?.Invoke(element.Style.UserRules);

        return element;
    }

    /// <summary>
    /// Creates a block-level element with specified dimensions.
    /// </summary>
    /// <param name="width">Width in pixels.</param>
    /// <param name="height">Height in pixels.</param>
    /// <param name="configureStyle">Optional additional style configuration.</param>
    /// <returns>The created element.</returns>
    public Element CreateBlock(int width, int height, Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(width);
            style.Height.Set(height);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates an inline-block element with specified dimensions.
    /// </summary>
    /// <param name="width">Width in pixels.</param>
    /// <param name="height">Height in pixels.</param>
    /// <param name="configureStyle">Optional additional style configuration.</param>
    /// <returns>The created element.</returns>
    public Element CreateInlineBlock(int width, int height, Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateElement("span", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            style.Width.Set(width);
            style.Height.Set(height);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates a flex container with optional configuration.
    /// </summary>
    /// <param name="configureStyle">Optional style configuration.</param>
    /// <returns>The created flex container element.</returns>
    public Element CreateFlexContainer(Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.FLEX);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates a grid container with optional configuration.
    /// </summary>
    /// <param name="configureStyle">Optional style configuration.</param>
    /// <returns>The created grid container element.</returns>
    public Element CreateGridContainer(Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.GRID);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates a child element inside the specified parent.
    /// </summary>
    /// <param name="parent">The parent element.</param>
    /// <param name="tagName">The element tag name.</param>
    /// <param name="configureStyle">Optional style configuration.</param>
    /// <returns>The created child element.</returns>
    public Element CreateChild(Element parent, string tagName, Action<CssComputedStyle>? configureStyle = null)
    {
        var element = Document.createElement(tagName, DefaultOptions);
        parent.appendChild(element);

        configureStyle?.Invoke(element.Style.UserRules);

        return element;
    }

    #endregion

    #region Layout Triggering

    /// <summary>
    /// Forces a style cascade update on all elements.
    /// </summary>
    public void ForceCascade()
    {
        // Trigger cascade by visiting all elements
        CascadeElement(DocumentElement);
    }

    private void CascadeElement(Element element)
    {
        // Trigger cascade on this element
        element.Style.Cascade();

        // Cascade children
        var child = element.firstElementChild;
        while (child != null)
        {
            CascadeElement(child);
            child = child.nextElementSibling;
        }
    }

    /// <summary>
    /// Forces box tree generation for all elements.
    /// </summary>
    public void ForceBoxGeneration()
    {
        // Set flags to trigger box generation
        DocumentElement.SetFlag(ENodeFlags.NeedsBoxUpdate);

        // Generate box tree
        CssBoxTree.Generate_Tree(Document);
    }

    /// <summary>
    /// Forces a complete layout update: cascade + box generation.
    /// </summary>
    public void ForceLayoutUpdate()
    {
        ForceCascade();
        ForceBoxGeneration();
    }

    /// <summary>
    /// Gets the principal box for an element, generating it if necessary.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The element's principal box, or null if display: none.</returns>
    public CssPrincipalBox? GetBox(Element element)
    {
        if (element.Box == null)
        {
            ForceLayoutUpdate();
        }
        return element.Box;
    }

    #endregion

    #region Assertion Helpers

    /// <summary>
    /// Verifies that an element has a box.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns>The element's box.</returns>
    /// <exception cref="InvalidOperationException">If the element has no box.</exception>
    public CssPrincipalBox RequireBox(Element element)
    {
        ForceLayoutUpdate();
        return element.Box ?? throw new InvalidOperationException(
            $"Element <{element.localName}> has no box. Check if display is 'none'.");
    }

    #endregion

    #region Containing Block Helpers

    /// <summary>
    /// Sets up a positioned containing block with specified dimensions.
    /// </summary>
    /// <param name="width">Width in pixels.</param>
    /// <param name="height">Height in pixels.</param>
    /// <param name="position">Positioning type (default: relative).</param>
    /// <returns>The containing block element.</returns>
    public Element CreateContainingBlock(int width, int height, EBoxPositioning position = EBoxPositioning.Relative)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(position);
            style.Width.Set(width);
            style.Height.Set(height);
        });
    }

    /// <summary>
    /// Creates an absolutely positioned element inside a containing block.
    /// </summary>
    /// <param name="containingBlock">The containing block element.</param>
    /// <param name="configureStyle">Style configuration for the positioned element.</param>
    /// <returns>The absolutely positioned element.</returns>
    public Element CreateAbsolutelyPositioned(Element containingBlock, Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateChild(containingBlock, "div", style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            configureStyle?.Invoke(style);
        });
    }

    #endregion

    #region Margin Helpers

    /// <summary>
    /// Creates a block with specified margins.
    /// </summary>
    public Element CreateBlockWithMargins(int width, int height, int marginTop, int marginRight, int marginBottom, int marginLeft)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(width);
            style.Height.Set(height);
            style.Margin_Top.Set(marginTop);
            style.Margin_Right.Set(marginRight);
            style.Margin_Bottom.Set(marginBottom);
            style.Margin_Left.Set(marginLeft);
        });
    }

    /// <summary>
    /// Creates a block with auto left and right margins (for centering).
    /// </summary>
    public Element CreateCenteredBlock(int width, int height)
    {
        return CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(width);
            style.Height.Set(height);
            style.Margin_Left.Assigned = CssValue.Auto;
            style.Margin_Right.Assigned = CssValue.Auto;
        });
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Document.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}

/// <summary>
/// Static helper methods for quick layout testing without fixture setup.
/// </summary>
public static class LayoutTestHelper
{
    /// <summary>
    /// Creates a minimal document with a single styled element.
    /// </summary>
    public static (HTMLDocument Document, Element Element, CssPrincipalBox? Box) CreateStyledElement(
        string tagName,
        Action<CssComputedStyle> configureStyle)
    {
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement(tagName, configureStyle);
        fixture.ForceLayoutUpdate();
        return (fixture.Document, element, element.Box);
    }
}
