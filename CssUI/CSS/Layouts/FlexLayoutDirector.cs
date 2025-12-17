using CssUI.CSS.Internal;
using System;
using System.Collections.Generic;

namespace CssUI.CSS.Layouts
{
    /// <summary>
    /// Implements CSS Flexible Box Layout Module Level 1.
    /// Spec: https://www.w3.org/TR/css-flexbox-1/
    /// </summary>
    public sealed class FlexLayoutDirector : LayoutDirectorBase
    {
        #region Internal Types

        /// <summary>
        /// Represents a flex item during layout calculations.
        /// </summary>
        private class FlexItem
        {
            public cssElement Element { get; set; }

            /// <summary>
            /// The flex container's main axis maps to this item's axis.
            /// </summary>
            public float FlexBaseSize { get; set; }

            /// <summary>
            /// The hypothetical main size is the item's flex base size clamped by min/max main size.
            /// </summary>
            public float HypotheticalMainSize { get; set; }

            /// <summary>
            /// The target main size after resolving flexible lengths.
            /// </summary>
            public float TargetMainSize { get; set; }

            /// <summary>
            /// The hypothetical cross size before alignment.
            /// </summary>
            public float HypotheticalCrossSize { get; set; }

            /// <summary>
            /// The used cross size after alignment.
            /// </summary>
            public float UsedCrossSize { get; set; }

            /// <summary>
            /// Position on main axis (after alignment).
            /// </summary>
            public float MainAxisPosition { get; set; }

            /// <summary>
            /// Position on cross axis (after alignment).
            /// </summary>
            public float CrossAxisPosition { get; set; }

            /// <summary>
            /// Whether this item is frozen during flexible length resolution.
            /// </summary>
            public bool Frozen { get; set; }

            /// <summary>
            /// The item's flex grow factor.
            /// </summary>
            public float FlexGrow { get; set; }

            /// <summary>
            /// The item's flex shrink factor.
            /// </summary>
            public float FlexShrink { get; set; }

            /// <summary>
            /// Scaled flex shrink factor (flex shrink * flex base size).
            /// </summary>
            public float ScaledFlexShrinkFactor => FlexShrink * FlexBaseSize;

            /// <summary>
            /// Whether the item has a violated min/max constraint.
            /// </summary>
            public bool IsMinViolation { get; set; }
            public bool IsMaxViolation { get; set; }
        }

        /// <summary>
        /// Represents a flex line containing multiple flex items.
        /// </summary>
        private class FlexLine
        {
            public List<FlexItem> Items { get; } = new List<FlexItem>();

            /// <summary>
            /// The cross size of this flex line.
            /// </summary>
            public float CrossSize { get; set; }

            /// <summary>
            /// Position of this line on the cross axis.
            /// </summary>
            public float CrossAxisPosition { get; set; }
        }

        #endregion

        #region Fields

        private List<FlexItem> _flexItems;
        private List<FlexLine> _flexLines;

        // Container properties (cached for performance)
        private EFlexDirection _flexDirection;
        private EFlexWrap _flexWrap;
        private float _availableMainSpace;
        private float _availableCrossSpace;

        #endregion

        #region Constructors

        public FlexLayoutDirector() { }

        #endregion

        #region ILayoutDirector Implementation

        public override CssBoxArea Handle(IParentElement Owner, cssElement[] controls)
        {
            Reset();
            var owner = Owner as cssElement;
            if (owner == null || controls == null || controls.Length == 0)
            {
                return layoutBlock;
            }

            // Cache container properties
            // TODO: Read from owner.Style once flex properties are implemented
            _flexDirection = EFlexDirection.Row; // Default
            _flexWrap = EFlexWrap.NoWrap; // Default

            // Determine available space
            Rect2i layoutArea = Owner.Get_Layout_Area();
            _availableMainSpace = IsMainAxisHorizontal ? layoutArea.Width : layoutArea.Height;
            _availableCrossSpace = IsMainAxisHorizontal ? layoutArea.Height : layoutArea.Width;

            // §9.1 Generate flex items
            GenerateFlexItems(controls);

            // §9.2 Determine main size and collect into lines
            DetermineMainSizeAndCollectIntoLines();

            // §9.3 Resolve flexible lengths
            foreach (var line in _flexLines)
            {
                ResolveFlexibleLengths(line);
            }

            // §9.4 Determine cross sizes
            DetermineCrossSizes();

            // §9.5 Main-axis alignment
            PerformMainAxisAlignment();

            // §9.6 Cross-axis alignment
            PerformCrossAxisAlignment();

            // Apply final positions to elements
            ApplyFinalPositions();

            return layoutBlock;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether the main axis is horizontal (row/row-reverse) or vertical (column/column-reverse).
        /// </summary>
        private bool IsMainAxisHorizontal =>
            _flexDirection == EFlexDirection.Row || _flexDirection == EFlexDirection.RowReverse;

        /// <summary>
        /// Whether the main axis direction is reversed.
        /// </summary>
        private bool IsMainAxisReversed =>
            _flexDirection == EFlexDirection.RowReverse || _flexDirection == EFlexDirection.ColumnReverse;

        /// <summary>
        /// Whether flex lines wrap.
        /// </summary>
        private bool IsMultiLine => _flexWrap != EFlexWrap.NoWrap;

        /// <summary>
        /// Whether wrap direction is reversed.
        /// </summary>
        private bool IsWrapReversed => _flexWrap == EFlexWrap.WrapReverse;

        #endregion

        #region §9.1 Initial Setup

        /// <summary>
        /// Generate flex items from child elements.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#algo-init
        /// </summary>
        private void GenerateFlexItems(cssElement[] controls)
        {
            _flexItems = new List<FlexItem>(controls.Length);
            _flexLines = new List<FlexLine>();

            foreach (var element in controls)
            {
                // Skip elements that don't participate in layout
                if (!element.Affects_Layout) continue;

                // Absolutely positioned children don't participate in flex layout
                if (element.Style.Positioning == EBoxPositioning.Absolute ||
                    element.Style.Positioning == EBoxPositioning.Fixed)
                {
                    continue;
                }

                var item = new FlexItem
                {
                    Element = element,
                    // TODO: Read actual flex properties once implemented
                    FlexGrow = 0f,   // Default
                    FlexShrink = 1f, // Default
                    Frozen = false
                };

                _flexItems.Add(item);
            }
        }

        #endregion

        #region §9.2 Line Length Determination

        /// <summary>
        /// Determine the main size of flex items and collect them into flex lines.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#algo-main-container
        /// </summary>
        private void DetermineMainSizeAndCollectIntoLines()
        {
            // Calculate flex base size and hypothetical main size for each item
            foreach (var item in _flexItems)
            {
                item.FlexBaseSize = CalculateFlexBaseSize(item);
                item.HypotheticalMainSize = ClampToMinMax(item, item.FlexBaseSize);
            }

            // Collect items into flex lines
            if (!IsMultiLine)
            {
                // Single-line: all items go into one line
                var line = new FlexLine();
                line.Items.AddRange(_flexItems);
                _flexLines.Add(line);
            }
            else
            {
                // Multi-line: wrap items based on available main space
                CollectItemsIntoLines();
            }
        }

        /// <summary>
        /// Calculate the flex base size for an item.
        /// </summary>
        private float CalculateFlexBaseSize(FlexItem item)
        {
            // TODO: Implement full flex-basis resolution
            // For now, use the item's content size
            var box = item.Element.Box.Content;
            return IsMainAxisHorizontal ? box.Width : box.Height;
        }

        /// <summary>
        /// Clamp a size to the item's min/max constraints on the main axis.
        /// </summary>
        private float ClampToMinMax(FlexItem item, float size)
        {
            // TODO: Get actual min/max from style
            float minMain = 0;
            float maxMain = float.MaxValue;
            return Math.Max(minMain, Math.Min(maxMain, size));
        }

        /// <summary>
        /// Collect flex items into lines for multi-line flex containers.
        /// </summary>
        private void CollectItemsIntoLines()
        {
            var currentLine = new FlexLine();
            float currentLineMainSize = 0;

            foreach (var item in _flexItems)
            {
                float itemMainSize = item.HypotheticalMainSize;

                // Check if item fits on current line
                if (currentLine.Items.Count > 0 &&
                    currentLineMainSize + itemMainSize > _availableMainSpace)
                {
                    // Start a new line
                    _flexLines.Add(currentLine);
                    currentLine = new FlexLine();
                    currentLineMainSize = 0;
                }

                currentLine.Items.Add(item);
                currentLineMainSize += itemMainSize;
            }

            // Add the last line
            if (currentLine.Items.Count > 0)
            {
                _flexLines.Add(currentLine);
            }
        }

        #endregion

        #region §9.3 & §9.7 Resolve Flexible Lengths

        /// <summary>
        /// Resolve flexible lengths for items in a flex line.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#resolve-flexible-lengths
        /// </summary>
        private void ResolveFlexibleLengths(FlexLine line)
        {
            // Step 1: Determine used flex factor
            float totalHypotheticalMainSize = 0;
            foreach (var item in line.Items)
            {
                totalHypotheticalMainSize += item.HypotheticalMainSize;
            }

            bool isGrowing = totalHypotheticalMainSize < _availableMainSpace;

            // Step 2: Size inflexible items and freeze
            foreach (var item in line.Items)
            {
                float flexFactor = isGrowing ? item.FlexGrow : item.FlexShrink;
                if (flexFactor == 0 || (isGrowing && item.FlexBaseSize > item.HypotheticalMainSize) ||
                    (!isGrowing && item.FlexBaseSize < item.HypotheticalMainSize))
                {
                    item.TargetMainSize = item.HypotheticalMainSize;
                    item.Frozen = true;
                }
            }

            // Step 3: Calculate initial free space
            float initialFreeSpace = _availableMainSpace;
            foreach (var item in line.Items)
            {
                initialFreeSpace -= item.Frozen ? item.TargetMainSize : item.FlexBaseSize;
            }

            // Step 4: Loop until all items are frozen
            while (true)
            {
                // 4a: Check for unfrozen items
                bool hasUnfrozen = false;
                foreach (var item in line.Items)
                {
                    if (!item.Frozen)
                    {
                        hasUnfrozen = true;
                        break;
                    }
                }
                if (!hasUnfrozen) break;

                // 4b: Calculate remaining free space
                float remainingFreeSpace = _availableMainSpace;
                float totalFlexFactor = 0;
                foreach (var item in line.Items)
                {
                    if (item.Frozen)
                    {
                        remainingFreeSpace -= item.TargetMainSize;
                    }
                    else
                    {
                        remainingFreeSpace -= item.FlexBaseSize;
                        totalFlexFactor += isGrowing ? item.FlexGrow : item.ScaledFlexShrinkFactor;
                    }
                }

                // 4c: Distribute free space
                if (totalFlexFactor > 0 && remainingFreeSpace != 0)
                {
                    foreach (var item in line.Items)
                    {
                        if (item.Frozen) continue;

                        float ratio = isGrowing
                            ? item.FlexGrow / totalFlexFactor
                            : item.ScaledFlexShrinkFactor / totalFlexFactor;

                        if (isGrowing)
                        {
                            item.TargetMainSize = item.FlexBaseSize + (remainingFreeSpace * ratio);
                        }
                        else
                        {
                            item.TargetMainSize = item.FlexBaseSize - (Math.Abs(remainingFreeSpace) * ratio);
                        }
                    }
                }
                else
                {
                    // No flex factors, use hypothetical main size
                    foreach (var item in line.Items)
                    {
                        if (!item.Frozen)
                        {
                            item.TargetMainSize = item.HypotheticalMainSize;
                        }
                    }
                }

                // 4d: Fix min/max violations
                float totalViolation = 0;
                foreach (var item in line.Items)
                {
                    if (item.Frozen) continue;

                    float clamped = ClampToMinMax(item, item.TargetMainSize);
                    float violation = clamped - item.TargetMainSize;

                    item.IsMinViolation = violation > 0;
                    item.IsMaxViolation = violation < 0;
                    totalViolation += violation;
                    item.TargetMainSize = clamped;
                }

                // 4e: Freeze items based on violations
                if (Math.Abs(totalViolation) < 0.01f)
                {
                    // Freeze all
                    foreach (var item in line.Items)
                    {
                        item.Frozen = true;
                    }
                }
                else if (totalViolation > 0)
                {
                    // Freeze items with min violations
                    foreach (var item in line.Items)
                    {
                        if (item.IsMinViolation) item.Frozen = true;
                    }
                }
                else
                {
                    // Freeze items with max violations
                    foreach (var item in line.Items)
                    {
                        if (item.IsMaxViolation) item.Frozen = true;
                    }
                }
            }
        }

        #endregion

        #region §9.4 Cross Size Determination

        /// <summary>
        /// Determine the cross size of flex items and lines.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#algo-cross-item
        /// </summary>
        private void DetermineCrossSizes()
        {
            // Calculate hypothetical cross size for each item
            foreach (var item in _flexItems)
            {
                item.HypotheticalCrossSize = CalculateHypotheticalCrossSize(item);
            }

            // Calculate cross size of each flex line
            foreach (var line in _flexLines)
            {
                float maxCrossSize = 0;
                foreach (var item in line.Items)
                {
                    maxCrossSize = Math.Max(maxCrossSize, item.HypotheticalCrossSize);
                }
                line.CrossSize = maxCrossSize;
            }

            // Handle single-line container
            if (!IsMultiLine && _flexLines.Count == 1)
            {
                _flexLines[0].CrossSize = _availableCrossSpace;
            }

            // Determine used cross size for each item
            foreach (var line in _flexLines)
            {
                foreach (var item in line.Items)
                {
                    // TODO: Check align-self: stretch
                    item.UsedCrossSize = item.HypotheticalCrossSize;
                }
            }
        }

        /// <summary>
        /// Calculate hypothetical cross size for an item.
        /// </summary>
        private float CalculateHypotheticalCrossSize(FlexItem item)
        {
            var box = item.Element.Box.Content;
            return IsMainAxisHorizontal ? box.Height : box.Width;
        }

        #endregion

        #region §9.5 Main-Axis Alignment

        /// <summary>
        /// Align items along the main axis.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#algo-main-align
        /// </summary>
        private void PerformMainAxisAlignment()
        {
            foreach (var line in _flexLines)
            {
                // Calculate total main size of items
                float totalItemsMainSize = 0;
                foreach (var item in line.Items)
                {
                    totalItemsMainSize += item.TargetMainSize;
                }

                float remainingSpace = _availableMainSpace - totalItemsMainSize;

                // TODO: Get justify-content from style
                // For now, use flex-start (items packed to start)
                float position = IsMainAxisReversed ? _availableMainSpace : 0;

                foreach (var item in line.Items)
                {
                    if (IsMainAxisReversed)
                    {
                        position -= item.TargetMainSize;
                        item.MainAxisPosition = position;
                    }
                    else
                    {
                        item.MainAxisPosition = position;
                        position += item.TargetMainSize;
                    }
                }
            }
        }

        #endregion

        #region §9.6 Cross-Axis Alignment

        /// <summary>
        /// Align items and lines along the cross axis.
        /// Spec: https://www.w3.org/TR/css-flexbox-1/#algo-cross-align
        /// </summary>
        private void PerformCrossAxisAlignment()
        {
            // Position flex lines
            float linePosition = IsWrapReversed ? _availableCrossSpace : 0;

            foreach (var line in _flexLines)
            {
                if (IsWrapReversed)
                {
                    linePosition -= line.CrossSize;
                    line.CrossAxisPosition = linePosition;
                }
                else
                {
                    line.CrossAxisPosition = linePosition;
                    linePosition += line.CrossSize;
                }

                // Align items within the line
                foreach (var item in line.Items)
                {
                    // TODO: Get align-items/align-self from style
                    // For now, use stretch (fill the line's cross size)
                    item.CrossAxisPosition = line.CrossAxisPosition;
                }
            }
        }

        #endregion

        #region Apply Final Positions

        /// <summary>
        /// Apply calculated positions to element boxes.
        /// </summary>
        private void ApplyFinalPositions()
        {
            int maxX = 0, maxY = 0;

            foreach (var item in _flexItems)
            {
                int x, y;
                if (IsMainAxisHorizontal)
                {
                    x = (int)item.MainAxisPosition;
                    y = (int)item.CrossAxisPosition;
                }
                else
                {
                    x = (int)item.CrossAxisPosition;
                    y = (int)item.MainAxisPosition;
                }

                item.Element.Box.Set_Layout_Pos(x, y);

                // Track layout block size
                var box = item.Element.Box.Content;
                maxX = Math.Max(maxX, x + box.Width);
                maxY = Math.Max(maxY, y + box.Height);
            }

            layoutBlock.Set_Dimensions(maxX, maxY);
        }

        #endregion

        #region Reset

        protected override void Reset()
        {
            base.Reset();
            _flexItems = null;
            _flexLines = null;
        }

        #endregion
    }
}
