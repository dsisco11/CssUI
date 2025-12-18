using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting
{
    /// <summary>
    /// Formatting context for CSS Flexible Box Layout.
    /// Spec: https://www.w3.org/TR/css-flexbox-1/
    /// </summary>
    public class FlexFormattingContext : IFormattingContext
    {
        #region Internal Types

        /// <summary>
        /// Represents a flex item during layout calculations.
        /// </summary>
        private class FlexItem
        {
            public CssBoxTreeNode Box { get; set; } = null!;
            public CssPrincipalBox? PrincipalBox => Box as CssPrincipalBox;

            public float FlexBaseSize { get; set; }
            public float HypotheticalMainSize { get; set; }
            public float TargetMainSize { get; set; }
            public float HypotheticalCrossSize { get; set; }
            public float UsedCrossSize { get; set; }
            public float MainAxisPosition { get; set; }
            public float CrossAxisPosition { get; set; }
            public bool Frozen { get; set; }
            public float FlexGrow { get; set; }
            public float FlexShrink { get; set; }
            public float ScaledFlexShrinkFactor => FlexShrink * FlexBaseSize;
            public bool IsMinViolation { get; set; }
            public bool IsMaxViolation { get; set; }
        }

        /// <summary>
        /// Represents a flex line containing multiple flex items.
        /// </summary>
        private class FlexLine
        {
            public List<FlexItem> Items { get; } = new List<FlexItem>();
            public float CrossSize { get; set; }
            public float CrossAxisPosition { get; set; }
        }

        #endregion

        #region Fields

        private CssPrincipalBox? _container;
        private List<FlexItem>? _flexItems;
        private List<FlexLine>? _flexLines;
        private EFlexDirection _flexDirection;
        private EFlexWrap _flexWrap;
        private float _availableMainSpace;
        private float _availableCrossSpace;
        private float _mainAxisGap;
        private float _crossAxisGap;

        #endregion

        #region Properties

        private bool IsMainAxisHorizontal =>
            _flexDirection == EFlexDirection.Row || _flexDirection == EFlexDirection.RowReverse;

        private bool IsMainAxisReversed =>
            _flexDirection == EFlexDirection.RowReverse || _flexDirection == EFlexDirection.ColumnReverse;

        private bool IsMultiLine => _flexWrap != EFlexWrap.NoWrap;

        private bool IsWrapReversed => _flexWrap == EFlexWrap.WrapReverse;

        #endregion

        /// <summary>
        /// Performs flex layout on the container and its children.
        /// </summary>
        public void Flow(CssBoxTreeNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            Contract.EndContractBlock();

            _container = node as CssPrincipalBox;
            if (_container == null)
            {
                return;
            }

            // Read container flex properties from style
            var style = _container.Style;
            _flexDirection = style?.FlexDirection ?? EFlexDirection.Row;
            _flexWrap = style?.FlexWrap ?? EFlexWrap.NoWrap;

            // Read gap properties
            // Main axis gap is column-gap for row direction, row-gap for column direction
            // Cross axis gap is row-gap for row direction, column-gap for column direction
            if (IsMainAxisHorizontal)
            {
                _mainAxisGap = (float)(style?.ColumnGap ?? 0);
                _crossAxisGap = (float)(style?.RowGap ?? 0);
            }
            else
            {
                _mainAxisGap = (float)(style?.RowGap ?? 0);
                _crossAxisGap = (float)(style?.ColumnGap ?? 0);
            }

            // Determine available space from container size
            var containerSize = _container.Size;
            _availableMainSpace = IsMainAxisHorizontal ? (float)containerSize.Width : (float)containerSize.Height;
            _availableCrossSpace = IsMainAxisHorizontal ? (float)containerSize.Height : (float)containerSize.Width;

            // §9.1 Generate flex items
            GenerateFlexItems();

            if (_flexItems.Count == 0)
            {
                return;
            }

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

            // Apply final positions
            ApplyFinalPositions();
        }

        #region §9.1 Initial Setup

        private void GenerateFlexItems()
        {
            _flexItems = new List<FlexItem>();
            _flexLines = new List<FlexLine>();

            var current = _container.firstChild;
            while (current != null)
            {
                var principalBox = current as CssPrincipalBox;
                if (principalBox != null)
                {
                    var style = principalBox.Style;

                    // Skip absolutely/fixed positioned children
                    var positioning = style?.Positioning ?? EBoxPositioning.Static;
                    if (positioning == EBoxPositioning.Absolute || positioning == EBoxPositioning.Fixed)
                    {
                        current = current.nextSibling;
                        continue;
                    }

                    var item = new FlexItem
                    {
                        Box = current,
                        FlexGrow = (float)(style?.FlexGrow ?? 0),
                        FlexShrink = (float)(style?.FlexShrink ?? 1),
                        Frozen = false
                    };

                    _flexItems.Add(item);
                }
                current = current.nextSibling;
            }
        }

        #endregion

        #region §9.2 Line Length Determination

        private void DetermineMainSizeAndCollectIntoLines()
        {
            foreach (var item in _flexItems)
            {
                item.FlexBaseSize = CalculateFlexBaseSize(item);
                item.HypotheticalMainSize = ClampToMinMax(item, item.FlexBaseSize);
            }

            CollectItemsIntoLines();
        }

        private float CalculateFlexBaseSize(FlexItem item)
        {
            var style = item.PrincipalBox?.Style;
            var flexBasis = style?.FlexBasis ?? 0;
            var cascaded = style?.Cascaded?.FlexBasis;

            // If flex-basis is auto (0), use content size
            if (cascaded == null || cascaded.Computed.IsAuto)
            {
                var size = item.Box.Size;
                return IsMainAxisHorizontal ? (float)size.Width : (float)size.Height;
            }

            return (float)flexBasis;
        }

        private float ClampToMinMax(FlexItem item, float size)
        {
            var style = item.PrincipalBox?.Style;
            float minMain, maxMain;

            if (IsMainAxisHorizontal)
            {
                minMain = (float)(style?.Min_Width ?? 0);
                maxMain = style?.Max_Width.HasValue == true 
                    ? (float)style.Max_Width.Value
                    : float.MaxValue;
            }
            else
            {
                minMain = (float)(style?.Min_Height ?? 0);
                maxMain = style?.Max_Height.HasValue == true 
                    ? (float)style.Max_Height.Value
                    : float.MaxValue;
            }

            return Math.Max(minMain, Math.Min(maxMain, size));
        }

        private void CollectItemsIntoLines()
        {
            var currentLine = new FlexLine();
            float currentLineMainSize = 0;

            foreach (var item in _flexItems)
            {
                float itemMainSize = item.HypotheticalMainSize;
                // Account for gap when adding items after the first
                float gapForThisItem = currentLine.Items.Count > 0 ? _mainAxisGap : 0;

                if (IsMultiLine && currentLine.Items.Count > 0 &&
                    currentLineMainSize + gapForThisItem + itemMainSize > _availableMainSpace)
                {
                    _flexLines.Add(currentLine);
                    currentLine = new FlexLine();
                    currentLineMainSize = 0;
                    gapForThisItem = 0;
                }

                currentLine.Items.Add(item);
                currentLineMainSize += gapForThisItem + itemMainSize;
            }

            if (currentLine.Items.Count > 0)
            {
                _flexLines.Add(currentLine);
            }
        }

        #endregion

        #region §9.3 Resolve Flexible Lengths

        private void ResolveFlexibleLengths(FlexLine line)
        {
            float totalHypotheticalMainSize = 0;
            foreach (var item in line.Items)
            {
                totalHypotheticalMainSize += item.HypotheticalMainSize;
            }

            float freeSpace = _availableMainSpace - totalHypotheticalMainSize;
            bool isGrowing = freeSpace > 0;

            // Initialize target sizes
            foreach (var item in line.Items)
            {
                item.TargetMainSize = item.FlexBaseSize;
                item.Frozen = false;

                // Freeze inflexible items
                if (isGrowing && item.FlexGrow == 0)
                {
                    item.Frozen = true;
                    item.TargetMainSize = item.HypotheticalMainSize;
                }
                else if (!isGrowing && item.FlexShrink == 0)
                {
                    item.Frozen = true;
                    item.TargetMainSize = item.HypotheticalMainSize;
                }
            }

            // Iteratively resolve
            for (int iteration = 0; iteration < 10; iteration++)
            {
                float remainingFreeSpace = CalculateRemainingFreeSpace(line);
                if (Math.Abs(remainingFreeSpace) < 0.001f)
                    break;

                DistributeFreeSpace(line, remainingFreeSpace, isGrowing);
                if (!CheckAndFreezeViolations(line))
                    break;
            }
        }

        private float CalculateRemainingFreeSpace(FlexLine line)
        {
            float usedSpace = 0;
            foreach (var item in line.Items)
            {
                usedSpace += item.Frozen ? item.TargetMainSize : item.FlexBaseSize;
            }
            return _availableMainSpace - usedSpace;
        }

        private void DistributeFreeSpace(FlexLine line, float freeSpace, bool isGrowing)
        {
            float totalFactor = 0;
            foreach (var item in line.Items)
            {
                if (!item.Frozen)
                {
                    totalFactor += isGrowing ? item.FlexGrow : item.ScaledFlexShrinkFactor;
                }
            }

            if (totalFactor <= 0) return;

            foreach (var item in line.Items)
            {
                if (!item.Frozen)
                {
                    float factor = isGrowing ? item.FlexGrow : item.ScaledFlexShrinkFactor;
                    float ratio = factor / totalFactor;
                    item.TargetMainSize = item.FlexBaseSize + (freeSpace * ratio);
                }
            }
        }

        private bool CheckAndFreezeViolations(FlexLine line)
        {
            bool anyViolation = false;

            foreach (var item in line.Items)
            {
                if (item.Frozen) continue;

                float clamped = ClampToMinMax(item, item.TargetMainSize);
                item.IsMinViolation = clamped > item.TargetMainSize;
                item.IsMaxViolation = clamped < item.TargetMainSize;

                if (item.IsMinViolation || item.IsMaxViolation)
                {
                    item.TargetMainSize = clamped;
                    item.Frozen = true;
                    anyViolation = true;
                }
            }

            return anyViolation;
        }

        #endregion

        #region §9.4 Cross Size Determination

        private void DetermineCrossSizes()
        {
            foreach (var line in _flexLines)
            {
                float maxCrossSize = 0;
                foreach (var item in line.Items)
                {
                    var size = item.Box.Size;
                    float crossSize = IsMainAxisHorizontal ? (float)size.Height : (float)size.Width;
                    item.HypotheticalCrossSize = crossSize;
                    item.UsedCrossSize = crossSize;
                    maxCrossSize = Math.Max(maxCrossSize, crossSize);
                }
                line.CrossSize = maxCrossSize;
            }
        }

        #endregion

        #region §9.5 Main-Axis Alignment

        private void PerformMainAxisAlignment()
        {
            var justifyContent = _container?.Style?.JustifyContent ?? EJustifyContent.FlexStart;

            foreach (var line in _flexLines)
            {
                float totalItemsMainSize = 0;
                foreach (var item in line.Items)
                {
                    totalItemsMainSize += item.TargetMainSize;
                }

                // Account for gaps between items
                int gapCount = Math.Max(0, line.Items.Count - 1);
                float totalGapSpace = gapCount * _mainAxisGap;
                float remainingSpace = _availableMainSpace - totalItemsMainSize - totalGapSpace;
                
                float position = CalculateMainAxisStartPosition(justifyContent, remainingSpace, line.Items.Count);
                float extraGap = CalculateMainAxisGap(justifyContent, remainingSpace, line.Items.Count);

                for (int i = 0; i < line.Items.Count; i++)
                {
                    var item = line.Items[IsMainAxisReversed ? line.Items.Count - 1 - i : i];
                    item.MainAxisPosition = position;
                    // Add both the configured gap and any extra gap from justify-content
                    position += item.TargetMainSize + _mainAxisGap + extraGap;
                }
            }
        }

        private float CalculateMainAxisStartPosition(EJustifyContent justify, float space, int count)
        {
            return justify switch
            {
                EJustifyContent.FlexStart => 0,
                EJustifyContent.FlexEnd => space,
                EJustifyContent.Center => space / 2,
                EJustifyContent.SpaceBetween => 0,
                EJustifyContent.SpaceAround => count > 0 ? space / (count * 2) : 0,
                EJustifyContent.SpaceEvenly => count > 0 ? space / (count + 1) : 0,
                _ => 0
            };
        }

        private float CalculateMainAxisGap(EJustifyContent justify, float space, int count)
        {
            if (count <= 1) return 0;

            return justify switch
            {
                EJustifyContent.SpaceBetween => space / (count - 1),
                EJustifyContent.SpaceAround => space / count,
                EJustifyContent.SpaceEvenly => space / (count + 1),
                _ => 0
            };
        }

        #endregion

        #region §9.6 Cross-Axis Alignment

        private void PerformCrossAxisAlignment()
        {
            float linePosition = IsWrapReversed ? _availableCrossSpace : 0;
            var containerAlignItems = _container?.Style?.AlignItems ?? EAlignItems.Stretch;

            for (int lineIndex = 0; lineIndex < _flexLines.Count; lineIndex++)
            {
                var line = _flexLines[lineIndex];
                
                // Add gap between lines (not before first line)
                float gapBefore = lineIndex > 0 ? _crossAxisGap : 0;
                
                if (IsWrapReversed)
                {
                    linePosition -= line.CrossSize;
                    if (lineIndex > 0) linePosition -= _crossAxisGap;
                    line.CrossAxisPosition = linePosition;
                }
                else
                {
                    linePosition += gapBefore;
                    line.CrossAxisPosition = linePosition;
                    linePosition += line.CrossSize;
                }

                foreach (var item in line.Items)
                {
                    var alignSelf = item.PrincipalBox?.Style?.AlignSelf ?? containerAlignItems;
                    item.CrossAxisPosition = CalculateCrossAxisPosition(alignSelf, line, item);
                }
            }
        }

        private float CalculateCrossAxisPosition(EAlignItems alignment, FlexLine line, FlexItem item)
        {
            float lineStart = line.CrossAxisPosition;
            float lineSize = line.CrossSize;
            float itemSize = item.UsedCrossSize;

            return alignment switch
            {
                EAlignItems.FlexStart => lineStart,
                EAlignItems.FlexEnd => lineStart + lineSize - itemSize,
                EAlignItems.Center => lineStart + (lineSize - itemSize) / 2,
                EAlignItems.Stretch => lineStart,
                _ => lineStart
            };
        }

        #endregion

        #region Apply Final Positions

        private void ApplyFinalPositions()
        {
            foreach (var item in _flexItems)
            {
                float x, y;
                if (IsMainAxisHorizontal)
                {
                    x = item.MainAxisPosition;
                    y = item.CrossAxisPosition;
                }
                else
                {
                    x = item.CrossAxisPosition;
                    y = item.MainAxisPosition;
                }

                item.Box.Position = new Point2f(x, y);
            }
        }

        #endregion
    }
}

