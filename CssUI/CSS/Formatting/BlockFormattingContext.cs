using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Implements block formatting context layout per CSS 2.2 §9.4.1 and §9.5 (floats).
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#block-formatting
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#floats
/// </summary>
public class BlockFormattingContext : IFormattingContext
{
    #region Float Tracking

    /// <summary>
    /// Represents a floating box positioned within this BFC.
    /// Tracks the box's position and dimensions for float intrusion calculations.
    /// </summary>
    private readonly struct FloatArea
    {
        public CssBoxTreeNode Box { get; init; }
        public EFloat Side { get; init; }
        public double Top { get; init; }
        public double Bottom { get; init; }
        public double Left { get; init; }
        public double Right { get; init; }

        public FloatArea(CssBoxTreeNode box, EFloat side, double top, double left, double width, double height)
        {
            Box = box;
            Side = side;
            Top = top;
            Bottom = top + height;
            Left = left;
            Right = left + width;
        }
    }

    /// <summary>
    /// List of left-floated boxes in this BFC.
    /// Per CSS 2.2 §9.5: "A floated box is shifted to the left or right until its outer edge
    /// touches the containing block edge or the outer edge of another float."
    /// </summary>
    private readonly List<FloatArea> _leftFloats = new();

    /// <summary>
    /// List of right-floated boxes in this BFC.
    /// </summary>
    private readonly List<FloatArea> _rightFloats = new();

    #endregion

    #region Available Width Calculation

    /// <summary>
    /// Calculates the available width for content at a given vertical position,
    /// accounting for float intrusion from both sides.
    /// Per CSS 2.2 §9.5: "The border box of a table, a block-level replaced element,
    /// or an element in the normal flow that establishes a new block formatting context
    /// must not overlap the margin box of any floats in the same block formatting context."
    /// </summary>
    /// <param name="verticalPosition">The Y coordinate to check for float intrusion</param>
    /// <param name="containerWidth">The total width of the containing block</param>
    /// <param name="leftOffset">Output: The left edge offset (distance from container left)</param>
    /// <param name="availableWidth">Output: The available width between floats</param>
    private void GetAvailableWidth(double verticalPosition, double containerWidth, out double leftOffset, out double availableWidth)
    {
        leftOffset = 0;
        double rightIntrusion = 0;

        // Check left floats that intrude at this vertical position
        foreach (var leftFloat in _leftFloats)
        {
            if (verticalPosition >= leftFloat.Top && verticalPosition < leftFloat.Bottom)
            {
                leftOffset = Math.Max(leftOffset, leftFloat.Right);
            }
        }

        // Check right floats that intrude at this vertical position
        foreach (var rightFloat in _rightFloats)
        {
            if (verticalPosition >= rightFloat.Top && verticalPosition < rightFloat.Bottom)
            {
                rightIntrusion = Math.Max(rightIntrusion, containerWidth - rightFloat.Left);
            }
        }

        availableWidth = containerWidth - leftOffset - rightIntrusion;
    }

    /// <summary>
    /// Finds the lowest bottom edge of all floats that intrude at or below the given vertical position.
    /// Used for clear property implementation and float stacking.
    /// </summary>
    /// <param name="verticalPosition">The Y coordinate to start checking from</param>
    /// <param name="clearSide">Which floats to consider (Left, Right, Both)</param>
    /// <returns>The Y coordinate below which no relevant floats intrude, or the input position if no floats</returns>
    private double GetClearancePosition(double verticalPosition, EClear clearSide)
    {
        double clearanceY = verticalPosition;

        if (clearSide == EClear.Left || clearSide == EClear.Both)
        {
            foreach (var leftFloat in _leftFloats)
            {
                if (leftFloat.Top < clearanceY || (leftFloat.Top >= verticalPosition && leftFloat.Bottom > clearanceY))
                {
                    clearanceY = Math.Max(clearanceY, leftFloat.Bottom);
                }
            }
        }

        if (clearSide == EClear.Right || clearSide == EClear.Both)
        {
            foreach (var rightFloat in _rightFloats)
            {
                if (rightFloat.Top < clearanceY || (rightFloat.Top >= verticalPosition && rightFloat.Bottom > clearanceY))
                {
                    clearanceY = Math.Max(clearanceY, rightFloat.Bottom);
                }
            }
        }

        return clearanceY;
    }

    #endregion

    #region Float Positioning

    /// <summary>
    /// Positions a floated box at the current vertical position, accounting for existing floats.
    /// Per CSS 2.2 §9.5.1: "A left-floating box that has another left-floating box to its left
    /// may not have its right outer edge to the right of its containing block's right edge."
    /// </summary>
    /// <param name="box">The box to position</param>
    /// <param name="currentY">The current vertical position (line box position)</param>
    /// <param name="containerWidth">The width of the containing block</param>
    private void PositionFloat(CssBoxTreeNode box, double currentY, double containerWidth)
    {
        if (box is not CssPrincipalBox principalBox)
            return;

        var floatSide = principalBox.Float;
        var boxWidth = box.Size.Width;
        var boxHeight = box.Size.Height;

        double positionY = currentY;
        double positionX;

        // Find a valid position for this float, moving down if necessary
        while (true)
        {
            GetAvailableWidth(positionY, containerWidth, out double leftOffset, out double availableWidth);

            // Check if the float fits at this vertical position
            if (boxWidth <= availableWidth)
            {
                // Position the float
                if (floatSide == EFloat.Left || floatSide == EFloat.InlineStart)
                {
                    positionX = leftOffset;
                }
                else // Right or InlineEnd
                {
                    positionX = containerWidth - (containerWidth - leftOffset - availableWidth) - boxWidth;
                }

                box.Position = new Point2f(positionX, positionY);

                // Add to float tracking list
                var floatArea = new FloatArea(box, floatSide, positionY, positionX, boxWidth, boxHeight);
                if (floatSide == EFloat.Left || floatSide == EFloat.InlineStart)
                {
                    _leftFloats.Add(floatArea);
                }
                else
                {
                    _rightFloats.Add(floatArea);
                }

                break;
            }
            else
            {
                // Float doesn't fit - move down past the nearest float
                double nextY = double.MaxValue;
                foreach (var leftFloat in _leftFloats)
                {
                    if (leftFloat.Top >= positionY || leftFloat.Bottom > positionY)
                    {
                        nextY = Math.Min(nextY, leftFloat.Bottom);
                    }
                }
                foreach (var rightFloat in _rightFloats)
                {
                    if (rightFloat.Top >= positionY || rightFloat.Bottom > positionY)
                    {
                        nextY = Math.Min(nextY, rightFloat.Bottom);
                    }
                }

                if (nextY == double.MaxValue)
                {
                    // No floats below - position at current location anyway
                    positionX = (floatSide == EFloat.Left || floatSide == EFloat.InlineStart) ? leftOffset : leftOffset + availableWidth - boxWidth;
                    box.Position = new Point2f(positionX, positionY);

                    var floatArea = new FloatArea(box, floatSide, positionY, positionX, boxWidth, boxHeight);
                    if (floatSide == EFloat.Left || floatSide == EFloat.InlineStart)
                    {
                        _leftFloats.Add(floatArea);
                    }
                    else
                    {
                        _rightFloats.Add(floatArea);
                    }
                    break;
                }

                positionY = nextY;
            }
        }
    }

    #endregion

    #region Layout Flow

    /// <summary>
    /// Performs block layout on the container and its children.
    /// Implements CSS 2.2 §9.4.1 (Block Formatting Contexts) and §9.5 (Floats).
    /// </summary>
    /// <returns>The content dimensions (width, height) of the laid out content.</returns>
    public Rect2f Flow(CssBoxTreeNode Node)
    {
        System.ArgumentNullException.ThrowIfNull(Node);
        Contract.EndContractBlock();

        // Clear float tracking for this layout pass
        _leftFloats.Clear();
        _rightFloats.Clear();

        double maxWidth = 0;
        double currentY = 0;
        double containerWidth = Node is CssPrincipalBox principalContainer ? principalContainer.Size.Width : 0;

        CssBoxTreeNode? current = Node.firstChild;
        while (current is not null)
        {
            // Check if this box is floated
            bool isFloated = current is CssPrincipalBox principalBox && principalBox.IsFloating;

            if (isFloated)
            {
                // Position float at current line position
                PositionFloat(current, currentY, containerWidth);

                // Track content dimensions including floats
                var floatSize = current.Size;
                var floatPos = current.Position;
                maxWidth = Math.Max(maxWidth, floatPos.X + floatSize.Width);
                currentY = Math.Max(currentY, floatPos.Y + floatSize.Height);
            }
            else
            {
                // Handle clear property
                if (current is CssPrincipalBox principalNonFloat && principalNonFloat.Clear != EClear.None)
                {
                    currentY = GetClearancePosition(currentY, principalNonFloat.Clear);
                }

                // Position normal flow box
                GetAvailableWidth(currentY, containerWidth, out double leftOffset, out double availableWidth);

                current.Position = new Point2f(leftOffset, currentY);

                // Track content dimensions
                var currentSize = current.Size;
                var currentPos = current.Position;
                maxWidth = Math.Max(maxWidth, currentPos.X + currentSize.Width);
                currentY = currentPos.Y + currentSize.Height;
            }

            current = current.nextSibling;
        }

        // Ensure content height includes all floats
        foreach (var leftFloat in _leftFloats)
        {
            currentY = Math.Max(currentY, leftFloat.Bottom);
        }
        foreach (var rightFloat in _rightFloats)
        {
            currentY = Math.Max(currentY, rightFloat.Bottom);
        }

        return new Rect2f(maxWidth, currentY);
    }

    #endregion
}
