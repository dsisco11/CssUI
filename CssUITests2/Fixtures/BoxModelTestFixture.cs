using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;

namespace CssUITests.Fixtures;

/// <summary>
/// Test fixture specifically designed for BoxModel internal method tests.
/// Creates minimal DOM structures with direct box/style access, bypassing the full
/// box tree generation infrastructure that has known issues.
/// </summary>
/// <remarks>
/// <para>
/// This fixture is designed for testing the algorithmic logic of BoxModel methods
/// (Resolve_Horizontal, Resolve_Vertical, Calculate_Horizontal, Calculate_Vertical,
/// Constrain_Width_Height) without requiring full layout pipeline execution.
/// </para>
/// <para>
/// Key differences from LayoutTestFixture:
/// <list type="bullet">
/// <item>Uses <see cref="MockPrincipalBox"/> for direct control over box dimensions</item>
/// <item>Manually sets containing block dimensions without reflection</item>
/// <item>Allows direct manipulation of cascaded style values</item>
/// <item>Does not trigger the problematic TreeNodeList.Remove code path</item>
/// </list>
/// </para>
/// </remarks>
public class BoxModelTestFixture : IDisposable
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
    /// Default containing block width for tests.
    /// </summary>
    public int DefaultContainingBlockWidth { get; set; } = 800;

    /// <summary>
    /// Default containing block height for tests.
    /// </summary>
    public int DefaultContainingBlockHeight { get; set; } = 600;

    #endregion

    #region Constructor

    public BoxModelTestFixture()
    {
        Document = new HTMLDocument();

        // Create basic document structure: <html><body></body></html>
        var html = Document.createElement("html", DefaultOptions);
        Document.appendChild(html);

        Body = Document.createElement("body", DefaultOptions);
        html.appendChild(Body);

        // Configure root element styles and boxes
        ConfigureRootStyles();
    }

    private void ConfigureRootStyles()
    {
        // Set the document element as a block with fixed dimensions
        DocumentElement.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        DocumentElement.Style.UserRules.Width.Set(DefaultContainingBlockWidth);
        DocumentElement.Style.UserRules.Height.Set(DefaultContainingBlockHeight);
        DocumentElement.Style.Cascade();

        // Create and assign mock box for document element
        var htmlBox = new MockPrincipalBox(DocumentElement, null);
        DocumentElement.Box = htmlBox;
        // Document element's containing block is the viewport
        htmlBox.SetContainingBlock(DefaultContainingBlockWidth, DefaultContainingBlockHeight);
        // Set explicit dimensions on the html box Content
        htmlBox.SetContent(DefaultContainingBlockWidth, DefaultContainingBlockHeight);

        // Body fills the viewport by default
        Body.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        Body.Style.UserRules.Width.Set(DefaultContainingBlockWidth);
        Body.Style.UserRules.Height.Set(DefaultContainingBlockHeight);
        Body.Style.Cascade();

        // Create and assign mock box for body
        var bodyBox = new MockPrincipalBox(Body, htmlBox);
        Body.Box = bodyBox;
        // Body's containing block is the document element
        bodyBox.SetContainingBlock(DefaultContainingBlockWidth, DefaultContainingBlockHeight);
        // Set explicit dimensions on the body box Content
        bodyBox.SetContent(DefaultContainingBlockWidth, DefaultContainingBlockHeight);
    }

    #endregion

    #region Element Creation

    /// <summary>
    /// Creates a simple element with configured styles, appends it to the body,
    /// and initializes its box directly (bypassing box tree generation).
    /// </summary>
    /// <param name="configureStyle">Action to configure the element's cascaded styles.</param>
    /// <param name="containingBlockWidth">The containing block width to use.</param>
    /// <param name="containingBlockHeight">The containing block height to use.</param>
    /// <returns>A tuple of (Element, MockPrincipalBox, CssComputedStyle).</returns>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateTestElement(
        Action<CssComputedStyle> configureStyle,
        int? containingBlockWidth = null,
        int? containingBlockHeight = null)
    {
        var element = Document.createElement("div", DefaultOptions);
        Body.appendChild(element);

        // Configure the user rules
        configureStyle(element.Style.UserRules);

        // Trigger style cascade
        element.Style.Cascade();

        // Create mock box directly, bypassing tree generation
        var box = new MockPrincipalBox(element, null);

        // Set the element's box reference (Node.Box has internal set)
        element.Box = box;

        // Set containing block dimensions directly
        var cbWidth = containingBlockWidth ?? DefaultContainingBlockWidth;
        var cbHeight = containingBlockHeight ?? DefaultContainingBlockHeight;
        box.SetContainingBlock(cbWidth, cbHeight);

        return (element, box, element.Style.Cascaded);
    }

    /// <summary>
    /// Creates a block element with specified dimensions for Constrain_Width_Height tests.
    /// </summary>
    /// <param name="width">Initial width in pixels.</param>
    /// <param name="height">Initial height in pixels.</param>
    /// <param name="configureStyle">Optional additional style configuration.</param>
    /// <returns>A tuple of (Element, MockPrincipalBox, CssComputedStyle).</returns>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateBlockElement(
        int? width = null,
        int? height = null,
        Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            if (width.HasValue)
                style.Width.Set(width.Value);
            if (height.HasValue)
                style.Height.Set(height.Value);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates an inline element (display: inline).
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 §10.3.1: The 'width' property does not apply to inline non-replaced elements.
    /// Per CSS 2.1 §10.6.1: The 'height' property does not apply to inline non-replaced elements.
    /// Auto margins become 0.
    /// </remarks>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateInlineElement(
        Action<CssComputedStyle>? configureStyle = null,
        int? containingBlockWidth = null,
        int? containingBlockHeight = null)
    {
        return CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.INLINE);
            configureStyle?.Invoke(style);
        }, containingBlockWidth, containingBlockHeight);
    }

    /// <summary>
    /// Creates an inline-block element with specified dimensions.
    /// </summary>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateInlineBlockElement(
        int? width = null,
        int? height = null,
        Action<CssComputedStyle>? configureStyle = null)
    {
        return CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            if (width.HasValue)
                style.Width.Set(width.Value);
            if (height.HasValue)
                style.Height.Set(height.Value);
            configureStyle?.Invoke(style);
        });
    }

    /// <summary>
    /// Creates a floating element.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 §10.3.5: Auto margins become 0. Auto width uses shrink-to-fit.
    /// Note: Float property is not yet implemented (Phase 15), so this simulates
    /// floating behavior by setting display group. The Float property would normally
    /// trigger blockification and FLOATING display group.
    /// </remarks>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateFloatingElement(
        int? width = null,
        int? height = null,
        Action<CssComputedStyle>? configureStyle = null,
        int? containingBlockWidth = null,
        int? containingBlockHeight = null)
    {
        // NOTE: Since Float property is not implemented (Phase 15), we cannot truly create
        // a floating element. The box's DisplayGroup is derived from computed styles.
        // For now, we create a block element. Tests for FLOATING DisplayGroup will need
        // to either mock the box or wait for Phase 15 implementation.
        return CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            // Float property would go here when implemented
            if (width.HasValue)
                style.Width.Set(width.Value);
            if (height.HasValue)
                style.Height.Set(height.Value);
            configureStyle?.Invoke(style);
        }, containingBlockWidth, containingBlockHeight);
    }

    /// <summary>
    /// Creates an absolutely positioned element.
    /// </summary>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateAbsoluteElement(
        Action<CssComputedStyle>? configureStyle = null,
        int? containingBlockWidth = null,
        int? containingBlockHeight = null)
    {
        return CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Absolute);
            configureStyle?.Invoke(style);
        }, containingBlockWidth, containingBlockHeight);
    }

    /// <summary>
    /// Creates a replaced element (e.g., img) with intrinsic dimensions for testing
    /// the CSS 2.1 §10.4 constraint table for replaced elements with intrinsic ratios.
    /// </summary>
    /// <param name="intrinsicWidth">The intrinsic width of the replaced element.</param>
    /// <param name="intrinsicHeight">The intrinsic height of the replaced element.</param>
    /// <param name="configureStyle">Optional additional style configuration.</param>
    /// <param name="containingBlockWidth">The containing block width to use.</param>
    /// <param name="containingBlockHeight">The containing block height to use.</param>
    /// <returns>A tuple of (Element, MockPrincipalBox, CssComputedStyle).</returns>
    /// <remarks>
    /// This method creates an img element which is a replaced element per CSS spec.
    /// The intrinsic dimensions are set on the box to simulate an image with known dimensions.
    /// Note: The actual intrinsic ratio handling in Constrain_Width_Height may need to be
    /// implemented separately as it requires tracking whether the element is replaced and
    /// its intrinsic ratio.
    /// </remarks>
    public (Element element, MockPrincipalBox box, CssComputedStyle cascaded) CreateReplacedElement(
        int intrinsicWidth,
        int intrinsicHeight,
        Action<CssComputedStyle>? configureStyle = null,
        int? containingBlockWidth = null,
        int? containingBlockHeight = null)
    {
        // Create an img element (replaced element per CSS spec)
        var element = Document.createElement("img", DefaultOptions);
        Body.appendChild(element);

        // Configure the user rules
        element.Style.UserRules.Display.Set(EDisplayMode.INLINE); // img default display
        configureStyle?.Invoke(element.Style.UserRules);

        // Trigger style cascade
        element.Style.Cascade();

        // Create mock box directly, bypassing tree generation
        var box = new MockPrincipalBox(element, null);

        // Set the element's box reference
        element.Box = box;

        // Set containing block dimensions directly
        var cbWidth = containingBlockWidth ?? DefaultContainingBlockWidth;
        var cbHeight = containingBlockHeight ?? DefaultContainingBlockHeight;
        box.SetContainingBlock(cbWidth, cbHeight);

        // Set the intrinsic dimensions on the box content area to simulate replaced element
        // Note: Real replaced element handling would also set IntrinsicWidth/IntrinsicHeight
        // properties if available on the box type
        box.SetContent(intrinsicWidth, intrinsicHeight);

        return (element, box, element.Style.Cascaded);
    }

    #endregion

    #region Style Helpers

    /// <summary>
    /// Sets margin values on a style.
    /// </summary>
    public static void SetMargins(CssComputedStyle style, CssValue? left = null, CssValue? right = null,
        CssValue? top = null, CssValue? bottom = null)
    {
        if (left is not null) style.Margin_Left.Set(left);
        if (right is not null) style.Margin_Right.Set(right);
        if (top is not null) style.Margin_Top.Set(top);
        if (bottom is not null) style.Margin_Bottom.Set(bottom);
    }

    /// <summary>
    /// Sets padding values on a style.
    /// </summary>
    public static void SetPadding(CssComputedStyle style, int? left = null, int? right = null,
        int? top = null, int? bottom = null)
    {
        if (left.HasValue) style.Padding_Left.Set(left.Value);
        if (right.HasValue) style.Padding_Right.Set(right.Value);
        if (top.HasValue) style.Padding_Top.Set(top.Value);
        if (bottom.HasValue) style.Padding_Bottom.Set(bottom.Value);
    }

    /// <summary>
    /// Sets border widths on a style.
    /// </summary>
    public static void SetBorders(CssComputedStyle style, int? left = null, int? right = null,
        int? top = null, int? bottom = null)
    {
        if (left.HasValue) style.Border_Left_Width.Set(left.Value);
        if (right.HasValue) style.Border_Right_Width.Set(right.Value);
        if (top.HasValue) style.Border_Top_Width.Set(top.Value);
        if (bottom.HasValue) style.Border_Bottom_Width.Set(bottom.Value);
    }

    /// <summary>
    /// Sets position offsets on a style.
    /// </summary>
    public static void SetOffsets(CssComputedStyle style, CssValue? left = null, CssValue? right = null,
        CssValue? top = null, CssValue? bottom = null)
    {
        if (left is not null) style.Left.Set(left);
        if (right is not null) style.Right.Set(right);
        if (top is not null) style.Top.Set(top);
        if (bottom is not null) style.Bottom.Set(bottom);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        // No unmanaged resources to clean up
        GC.SuppressFinalize(this);
    }

    #endregion
}
