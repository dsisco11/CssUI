using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Implements block formatting context layout per CSS 2.2 §9.4.1 and §9.5 (floats).
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#block-formatting
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#floats
/// Spec: https://www.w3.org/TR/CSS2/box.html#collapsing-margins (§8.3.1)
/// </summary>
public class BlockFormattingContext : IFormattingContext
{
    #region Margin Collapse Tracking

    /// <summary>
    /// Tracks the state of margin collapsing during block layout.
    /// Per CSS 2.2 §8.3.1: In CSS, the adjoining margins of two or more boxes 
    /// (which might or might not be siblings) can combine to form a single margin.
    /// Margins are said to collapse.
    /// Spec: https://www.w3.org/TR/CSS2/box.html#collapsing-margins
    /// </summary>
    private class MarginCollapseState
    {
        /// <summary>
        /// List of positive margins pending collapse.
        /// Per §8.3.1: "When two or more margins collapse, the resulting margin width 
        /// is the maximum of the collapsing margins' widths."
        /// </summary>
        public List<double> PositiveMargins { get; } = new();

        /// <summary>
        /// List of negative margins pending collapse.
        /// Per §8.3.1: "In the case of negative margins, the maximum of the absolute 
        /// values of the negative adjoining margins is deducted from the maximum of 
        /// the positive adjoining margins."
        /// </summary>
        public List<double> NegativeMargins { get; } = new();

        /// <summary>
        /// Indicates whether margins are currently adjoining (can collapse).
        /// Margins are adjoining when there's no content, padding, or borders between them.
        /// </summary>
        public bool IsAdjoining { get; set; } = true;

        /// <summary>
        /// Tracks if we're at the start of the BFC (before any content).
        /// Used for parent/first-child margin collapse detection.
        /// </summary>
        public bool IsAtBfcStart { get; set; } = true;

        /// <summary>
        /// The box whose margins are currently being tracked for potential collapse.
        /// Used to detect parent/child and sibling collapse scenarios.
        /// </summary>
        public CssBoxTreeNode? CurrentBox { get; set; }

        /// <summary>
        /// Adds a margin to the collapse state.
        /// </summary>
        public void AddMargin(double margin)
        {
            if (margin >= 0)
            {
                PositiveMargins.Add(margin);
            }
            else
            {
                NegativeMargins.Add(margin);
            }
        }

        /// <summary>
        /// Calculates the collapsed margin value from all pending margins.
        /// Per CSS 2.2 §8.3.1:
        /// - All positive: max of all positive margins
        /// - All negative: most negative (min of all negative margins)
        /// - Mixed: max(positive) + min(negative)
        /// </summary>
        public double CalculateCollapsedMargin()
        {
            if (PositiveMargins.Count == 0 && NegativeMargins.Count == 0)
                return 0;

            double maxPositive = 0;
            double minNegative = 0;

            if (PositiveMargins.Count > 0)
            {
                foreach (var margin in PositiveMargins)
                {
                    maxPositive = Math.Max(maxPositive, margin);
                }
            }

            if (NegativeMargins.Count > 0)
            {
                foreach (var margin in NegativeMargins)
                {
                    minNegative = Math.Min(minNegative, margin);
                }
            }

            return maxPositive + minNegative;
        }

        /// <summary>
        /// Resets the collapse state for a new set of adjoining margins.
        /// </summary>
        public void Reset()
        {
            PositiveMargins.Clear();
            NegativeMargins.Clear();
            IsAdjoining = true;
            CurrentBox = null;
        }

        /// <summary>
        /// Breaks margin adjacency (prevents further collapsing).
        /// Called when content, padding, or borders appear between margins.
        /// </summary>
        public void BreakAdjacency()
        {
            IsAdjoining = false;
        }
    }

    /// <summary>
    /// Current margin collapse state for this BFC.
    /// </summary>
    private readonly MarginCollapseState _marginCollapseState = new();

    #endregion

    #region Margin Collapse Detection

    /// <summary>
    /// Detects if a box's top margin is adjoining with pending margins (can collapse).
    /// Per CSS 2.2 §8.3.1, margins are adjoining if:
    /// 1. Both belong to in-flow block-level boxes in the same BFC
    /// 2. No line boxes, clearance, padding, or borders separate them
    /// 3. Both belong to vertically-adjacent box edges
    /// </summary>
    private bool IsTopMarginAdjoining(CssPrincipalBox box)
    {
        if (!_marginCollapseState.IsAdjoining)
            return false;

        // Margins are not adjoining if the box:
        // - Has clearance (§8.3.1 rule about clearance)
        if (box.Clear != EClear.None)
            return false;

        // - Establishes a new BFC (floats, absolutely positioned, inline-blocks, etc.)
        if (box.IsFloating || box.IsAbsolutelyPositioned)
            return false;

        // - Has top border or padding
        // Note: Border/padding checks would go here when BoxModel areas are available during Flow()
        // For now, we assume no border/padding breaks adjacency

        return true;
    }

    /// <summary>
    /// Detects if a box's bottom margin is adjoining with the next element's top margin.
    /// Used for adjacent sibling margin collapse and empty block collapse detection.
    /// </summary>
    private bool IsBottomMarginAdjoining(CssPrincipalBox box)
    {
        if (!_marginCollapseState.IsAdjoining)
            return false;

        // Bottom margin is adjoining if:
        // - Box has no bottom border or padding
        // - Box is not floating or absolutely positioned
        if (box.IsFloating || box.IsAbsolutelyPositioned)
            return false;

        return true;
    }

    /// <summary>
    /// Detects if a box is empty (has no content, padding, or borders).
    /// Empty boxes can have their top and bottom margins collapse together.
    /// Per CSS 2.2 §8.3.1: "If the top and bottom margins of a box are adjoining, 
    /// then it is possible for margins to collapse through it."
    /// </summary>
    private bool IsEmptyBox(CssPrincipalBox box)
    {
        // A box is empty if:
        // - It has no in-flow children
        // - It has zero computed height (and no min-height)
        // - It has no top/bottom border or padding

        // Check for children
        if (box.firstChild != null)
        {
            // Has children - check if they're all out of flow
            var child = box.firstChild;
            while (child != null)
            {
                if (child is CssPrincipalBox childBox)
                {
                    if (!childBox.IsFloating && !childBox.IsAbsolutelyPositioned)
                        return false; // Has in-flow child
                }
                child = child.nextSibling;
            }
        }

        // If we get here, no in-flow children
        // Would also check height, min-height, border, padding
        // For now, return true if no in-flow children
        return true;
    }

    /// <summary>
    /// Handles parent/first-child margin collapse.
    /// Per CSS 2.2 §8.3.1: "The top margin of an in-flow block element collapses 
    /// with its first in-flow block-level child's top margin if the element has 
    /// no top border, no top padding, and the child has no clearance."
    /// </summary>
    private void HandleParentFirstChildCollapse(CssPrincipalBox parent, CssPrincipalBox firstChild)
    {
        // Check conditions for parent/first-child collapse:
        // 1. Both are in-flow block-level
        // 2. Parent has no top border/padding
        // 3. Child has no clearance

        if (firstChild.Clear != EClear.None)
            return; // Clearance prevents collapse

        if (parent.IsFloating || parent.IsAbsolutelyPositioned)
            return; // Out-of-flow parent

        if (firstChild.IsFloating || firstChild.IsAbsolutelyPositioned)
            return; // Out-of-flow child

        // If conditions met, mark that parent and first-child margins should collapse
        _marginCollapseState.IsAdjoining = true;
    }

    /// <summary>
    /// Handles parent/last-child margin collapse.
    /// Per CSS 2.2 §8.3.1: "The bottom margin of an in-flow block-level element 
    /// collapses with its last in-flow block-level child's bottom margin if the 
    /// element has no bottom border, no bottom padding, and the height is 'auto'."
    /// </summary>
    private void HandleParentLastChildCollapse(CssPrincipalBox parent, CssPrincipalBox lastChild)
    {
        // Check conditions for parent/last-child collapse:
        // 1. Both are in-flow block-level
        // 2. Parent has no bottom border/padding
        // 3. Parent has auto height (not explicitly set)

        if (parent.IsFloating || parent.IsAbsolutelyPositioned)
            return; // Out-of-flow parent

        if (lastChild.IsFloating || lastChild.IsAbsolutelyPositioned)
            return; // Out-of-flow child

        // Would check if parent height is auto
        // For now, assume it can collapse
        _marginCollapseState.IsAdjoining = true;
    }

    /// <summary>
    /// Handles adjacent sibling margin collapse.
    /// Per CSS 2.2 §8.3.1: "The bottom margin of a box and the top margin of its 
    /// following sibling can collapse if there's no intervening content."
    /// </summary>
    private void HandleAdjacentSiblingCollapse(CssPrincipalBox box, CssPrincipalBox nextSibling)
    {
        // Adjacent sibling margins collapse if:
        // 1. Both are in-flow block-level
        // 2. No line boxes, clearance, padding, or borders between them

        if (box.IsFloating || box.IsAbsolutelyPositioned)
            return;

        if (nextSibling.IsFloating || nextSibling.IsAbsolutelyPositioned)
            return;

        if (nextSibling.Clear != EClear.None)
            return; // Clearance on next sibling prevents collapse

        _marginCollapseState.IsAdjoining = true;
    }

    #endregion

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
