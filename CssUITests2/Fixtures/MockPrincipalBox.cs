using CssUI;
using CssUI.CSS.BoxTree;
using CssUI.DOM;

namespace CssUITests.Fixtures;

/// <summary>
/// A testable subclass of <see cref="CssPrincipalBox"/> that allows direct control
/// over containing block and content area values without using reflection.
/// </summary>
/// <remarks>
/// <para>
/// This mock class is designed for unit testing BoxModel internal methods where
/// precise control over box dimensions is needed without running the full layout pipeline.
/// </para>
/// <para>
/// Key features:
/// <list type="bullet">
/// <item>Direct setter for containing block dimensions via <see cref="SetContainingBlock"/></item>
/// <item>Direct setter for content area via <see cref="SetContent"/></item>
/// <item>Overrides the virtual <see cref="Containing_Box"/> property for proper polymorphism</item>
/// </list>
/// </para>
/// </remarks>
public class MockPrincipalBox : CssPrincipalBox
{
    private Rect4f? _mockContainingBox;

    /// <summary>
    /// Creates a new mock principal box for the specified element.
    /// </summary>
    /// <param name="owner">The DOM element that owns this box.</param>
    /// <param name="parent">The parent box (can be null for root elements).</param>
    public MockPrincipalBox(Element owner, CssPrincipalBox? parent = null)
        : base(owner, parent)
    {
    }

    /// <summary>
    /// Gets or sets the containing block, returning the mock value if set,
    /// otherwise falling back to the base implementation.
    /// </summary>
    public override Rect4f Containing_Box
    {
        get
        {
            if (_mockContainingBox.HasValue)
            {
                return _mockContainingBox.Value;
            }
            return base.Containing_Box;
        }
        protected set
        {
            _mockContainingBox = value;
        }
    }

    /// <summary>
    /// Sets the containing block dimensions directly for testing purposes.
    /// </summary>
    /// <param name="containingBlock">The containing block rectangle to use.</param>
    public void SetContainingBlock(Rect4f containingBlock)
    {
        _mockContainingBox = containingBlock;
    }

    /// <summary>
    /// Sets the containing block dimensions using width and height.
    /// Creates a rectangle with Top=0, Right=width, Bottom=height, Left=0.
    /// </summary>
    /// <param name="width">The containing block width.</param>
    /// <param name="height">The containing block height.</param>
    public void SetContainingBlock(int width, int height)
    {
        _mockContainingBox = new Rect4f(0, width, height, 0);
    }

    /// <summary>
    /// Sets the content area dimensions directly for testing purposes.
    /// </summary>
    /// <param name="content">The content rectangle to use.</param>
    public void SetContent(Rect4f content)
    {
        Content = content;
    }

    /// <summary>
    /// Sets the content area dimensions using width and height.
    /// Creates a rectangle with Top=0, Right=width, Bottom=height, Left=0.
    /// </summary>
    /// <param name="width">The content area width.</param>
    /// <param name="height">The content area height.</param>
    public void SetContent(int width, int height)
    {
        Content = new Rect4f(0, width, height, 0);
    }

    /// <summary>
    /// Clears the mock containing block, allowing the base implementation
    /// to calculate it from the DOM tree.
    /// </summary>
    public void ClearMockContainingBlock()
    {
        _mockContainingBox = null;
    }
}
