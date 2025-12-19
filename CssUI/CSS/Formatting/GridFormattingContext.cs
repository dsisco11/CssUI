using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Types;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Formatting context for CSS Grid Layout.
/// Spec: https://www.w3.org/TR/css-grid-1/
/// Fragmentation: https://www.w3.org/TR/css-grid-1/#pagination
/// </summary>
public class GridFormattingContext : IFormattingContext
{
    #region Internal Types

    /// <summary>
    /// Represents a grid item during layout calculations.
    /// </summary>
    private class GridItem
    {
        public CssBoxTreeNode Box { get; set; } = null!;
        public CssPrincipalBox? PrincipalBox => Box as CssPrincipalBox;

        /// <summary>
        /// The grid area this item occupies (column start, column end, row start, row end).
        /// </summary>
        public int ColumnStart { get; set; }
        public int ColumnEnd { get; set; }
        public int RowStart { get; set; }
        public int RowEnd { get; set; }

        /// <summary>
        /// Span in the column direction.
        /// </summary>
        public int ColumnSpan => ColumnEnd - ColumnStart;

        /// <summary>
        /// Span in the row direction.
        /// </summary>
        public int RowSpan => RowEnd - RowStart;

        /// <summary>
        /// Final position after layout.
        /// </summary>
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        /// <summary>
        /// The fragment index where this item starts (for fragmentation).
        /// </summary>
        public int FragmentIndex { get; set; }
    }

    /// <summary>
    /// Represents a grid track (column or row).
    /// </summary>
    private class GridTrack
    {
        /// <summary>
        /// The base size of this track.
        /// </summary>
        public float BaseSize { get; set; }

        /// <summary>
        /// The growth limit of this track.
        /// </summary>
        public float GrowthLimit { get; set; }

        /// <summary>
        /// Whether this track has a flexible sizing function (fr unit).
        /// </summary>
        public bool IsFlexible { get; set; }

        /// <summary>
        /// The flex factor if this is a flexible track.
        /// </summary>
        public float FlexFactor { get; set; }

        /// <summary>
        /// The original track size definition (if from explicit grid).
        /// </summary>
        public GridTrackSize? SizeDefinition { get; set; }

        /// <summary>
        /// The final resolved size.
        /// </summary>
        public float ResolvedSize { get; set; }

        /// <summary>
        /// The start position of this track.
        /// </summary>
        public float StartPosition { get; set; }

        /// <summary>
        /// The fragment index where this track starts (for fragmentation).
        /// </summary>
        public int FragmentIndex { get; set; }
    }

    #endregion

    #region Fields

    private CssPrincipalBox? _container;
    private List<GridItem>? _gridItems;
    private List<GridTrack>? _columnTracks;
    private List<GridTrack>? _rowTracks;
    private EGridAutoFlow _autoFlow;
    private float _availableWidth;
    private float _availableHeight;
    private float _columnGap;
    private float _rowGap;
    private FragmentationContext? _fragmentationContext;

    // Explicit track lists from grid-template-columns/rows
    private GridTrackList? _explicitColumnTracks;
    private GridTrackList? _explicitRowTracks;

    // Auto-placement cursor
    private int _autoCursorColumn;
    private int _autoCursorRow;

    #endregion

    #region Properties

    /// <summary>
    /// Whether auto-placement flows by row or column.
    /// </summary>
    private bool IsRowFlow => (_autoFlow & EGridAutoFlow.Column) == 0;

    /// <summary>
    /// Whether dense packing is enabled.
    /// </summary>
    private bool IsDense => (_autoFlow & EGridAutoFlow.Dense) != 0;

    /// <summary>
    /// Whether fragmentation is active for this layout.
    /// </summary>
    private bool IsFragmenting => _fragmentationContext?.IsFragmenting == true;

    #endregion

    /// <summary>
    /// Performs grid layout on the container and its children.
    /// </summary>
    public void Flow(CssBoxTreeNode? node)
    {
        Flow(node, new FragmentationContext());
    }

    /// <summary>
    /// Performs grid layout with fragmentation support.
    /// </summary>
    public void Flow(CssBoxTreeNode? node, FragmentationContext fragmentationContext)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        Contract.EndContractBlock();

        _container = node as CssPrincipalBox;
        if (_container is null)
        {
            return;
        }

        _fragmentationContext = fragmentationContext;

        // Read container grid properties from style
        var style = _container.Style;
        _autoFlow = style?.GridAutoFlow ?? EGridAutoFlow.Row;

        // Read gap properties
        _columnGap = (float)(style?.ColumnGap ?? 0);
        _rowGap = (float)(style?.RowGap ?? 0);

        // Determine available space from container size
        var containerSize = _container.Size;
        _availableWidth = (float)containerSize.Width;
        _availableHeight = (float)containerSize.Height;

        // §7.1 Generate grid items
        GenerateGridItems();

        if (_gridItems!.Count == 0)
        {
            return;
        }

        // §7.2 Define the explicit grid
        DefineExplicitGrid();

        // §8.5 Place grid items (auto-placement)
        PlaceGridItems();

        // §11 Grid Sizing Algorithm
        ResolveTrackSizes();

        // Calculate final positions
        CalculateFinalPositions();

        // Handle fragmentation if active
        if (IsFragmenting)
        {
            HandleFragmentation();
        }

        // Apply positions to elements
        ApplyFinalPositions();
    }

    #region §7.1 Grid Item Generation

    private void GenerateGridItems()
    {
        _gridItems = new List<GridItem>();
        _columnTracks = new List<GridTrack>();
        _rowTracks = new List<GridTrack>();

        var current = _container!.firstChild;
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

                var item = new GridItem
                {
                    Box = current,
                    // Read grid placement properties (default to auto = 0)
                    ColumnStart = style?.GridColumnStart ?? 0,
                    ColumnEnd = style?.GridColumnEnd ?? 0,
                    RowStart = style?.GridRowStart ?? 0,
                    RowEnd = style?.GridRowEnd ?? 0
                };

                _gridItems.Add(item);
            }
            current = current.nextSibling;
        }
    }

    #endregion

    #region §7.2 Explicit Grid Definition

    private void DefineExplicitGrid()
    {
        // Parse explicit track definitions from style
        var style = _container!.Style;
        string? columnTemplate = style?.GridTemplateColumns;
        string? rowTemplate = style?.GridTemplateRows;

        _explicitColumnTracks = !string.IsNullOrEmpty(columnTemplate)
            ? GridTrackList.Parse(columnTemplate)
            : new GridTrackList();
        _explicitRowTracks = !string.IsNullOrEmpty(rowTemplate)
            ? GridTrackList.Parse(rowTemplate)
            : new GridTrackList();

        // Find the maximum column and row needed from items
        int maxColumn = _explicitColumnTracks.Count;
        int maxRow = _explicitRowTracks.Count;

        foreach (var item in _gridItems!)
        {
            if (item.ColumnEnd > 0)
                maxColumn = Math.Max(maxColumn, item.ColumnEnd);
            if (item.RowEnd > 0)
                maxRow = Math.Max(maxRow, item.RowEnd);
        }

        // Ensure we have at least 1 track
        maxColumn = Math.Max(1, maxColumn);
        maxRow = Math.Max(1, maxRow);

        // Create column tracks from explicit definitions + implicit auto tracks
        for (int i = 0; i < maxColumn; i++)
        {
            GridTrack track;
            if (i < _explicitColumnTracks.Count)
            {
                var trackSize = _explicitColumnTracks[i];
                track = new GridTrack
                {
                    BaseSize = 0,
                    GrowthLimit = float.MaxValue,
                    IsFlexible = trackSize.IsFlexible,
                    FlexFactor = (float)trackSize.FlexFactor,
                    SizeDefinition = trackSize
                };
            }
            else
            {
                // Implicit track with auto sizing
                track = new GridTrack
                {
                    BaseSize = 0,
                    GrowthLimit = float.MaxValue,
                    IsFlexible = false
                };
            }
            _columnTracks!.Add(track);
        }

        // Create row tracks from explicit definitions + implicit auto tracks
        for (int i = 0; i < maxRow; i++)
        {
            GridTrack track;
            if (i < _explicitRowTracks.Count)
            {
                var trackSize = _explicitRowTracks[i];
                track = new GridTrack
                {
                    BaseSize = 0,
                    GrowthLimit = float.MaxValue,
                    IsFlexible = trackSize.IsFlexible,
                    FlexFactor = (float)trackSize.FlexFactor,
                    SizeDefinition = trackSize
                };
            }
            else
            {
                // Implicit track with auto sizing
                track = new GridTrack
                {
                    BaseSize = 0,
                    GrowthLimit = float.MaxValue,
                    IsFlexible = false
                };
            }
            _rowTracks!.Add(track);
        }
    }

    #endregion

    #region §8.5 Auto-Placement

    private void PlaceGridItems()
    {
        _autoCursorColumn = 0;
        _autoCursorRow = 0;

        // First pass: place items with explicit positions
        foreach (var item in _gridItems!)
        {
            if (item.ColumnStart > 0 && item.RowStart > 0)
            {
                // Item has explicit position, ensure grid is large enough
                EnsureGridSize(item.ColumnEnd, item.RowEnd);
            }
        }

        // Second pass: auto-place remaining items
        foreach (var item in _gridItems)
        {
            if (item.ColumnStart == 0 || item.RowStart == 0)
            {
                AutoPlaceItem(item);
            }
        }
    }

    private void AutoPlaceItem(GridItem item)
    {
        // Simple auto-placement: place in next available cell
        int columnSpan = item.ColumnSpan > 0 ? item.ColumnSpan : 1;
        int rowSpan = item.RowSpan > 0 ? item.RowSpan : 1;

        if (IsRowFlow)
        {
            // Flow by row
            while (true)
            {
                if (_autoCursorColumn + columnSpan <= _columnTracks!.Count)
                {
                    // Found a spot
                    item.ColumnStart = _autoCursorColumn + 1;
                    item.ColumnEnd = _autoCursorColumn + columnSpan + 1;
                    item.RowStart = _autoCursorRow + 1;
                    item.RowEnd = _autoCursorRow + rowSpan + 1;

                    EnsureGridSize(item.ColumnEnd, item.RowEnd);

                    _autoCursorColumn += columnSpan;
                    if (_autoCursorColumn >= _columnTracks.Count)
                    {
                        _autoCursorColumn = 0;
                        _autoCursorRow++;
                    }
                    break;
                }
                else
                {
                    // Move to next row
                    _autoCursorColumn = 0;
                    _autoCursorRow++;
                    EnsureGridSize(columnSpan, _autoCursorRow + rowSpan);
                }
            }
        }
        else
        {
            // Flow by column
            while (true)
            {
                if (_autoCursorRow + rowSpan <= _rowTracks!.Count)
                {
                    // Found a spot
                    item.ColumnStart = _autoCursorColumn + 1;
                    item.ColumnEnd = _autoCursorColumn + columnSpan + 1;
                    item.RowStart = _autoCursorRow + 1;
                    item.RowEnd = _autoCursorRow + rowSpan + 1;

                    EnsureGridSize(item.ColumnEnd, item.RowEnd);

                    _autoCursorRow += rowSpan;
                    if (_autoCursorRow >= _rowTracks.Count)
                    {
                        _autoCursorRow = 0;
                        _autoCursorColumn++;
                    }
                    break;
                }
                else
                {
                    // Move to next column
                    _autoCursorRow = 0;
                    _autoCursorColumn++;
                    EnsureGridSize(_autoCursorColumn + columnSpan, rowSpan);
                }
            }
        }
    }

    private void EnsureGridSize(int columns, int rows)
    {
        while (_columnTracks!.Count < columns)
        {
            _columnTracks.Add(new GridTrack
            {
                BaseSize = 0,
                GrowthLimit = float.MaxValue,
                IsFlexible = false
            });
        }

        while (_rowTracks!.Count < rows)
        {
            _rowTracks.Add(new GridTrack
            {
                BaseSize = 0,
                GrowthLimit = float.MaxValue,
                IsFlexible = false
            });
        }
    }

    #endregion

    #region §11 Track Sizing Algorithm

    private void ResolveTrackSizes()
    {
        // §11.4 Initialize Track Sizes
        InitializeTrackSizes();

        // §11.5 Resolve Intrinsic Track Sizes
        ResolveIntrinsicTrackSizes();

        // §11.6 Maximize Tracks
        MaximizeTracks();

        // §11.7 Expand Flexible Tracks (fr units)
        ExpandFlexibleTracks();

        // §11.8 Stretch auto Tracks
        StretchAutoTracks();
    }

    private void InitializeTrackSizes()
    {
        // For auto-sized tracks, initialize base size to 0
        foreach (var track in _columnTracks!)
        {
            track.BaseSize = 0;
            track.ResolvedSize = 0;
        }

        foreach (var track in _rowTracks!)
        {
            track.BaseSize = 0;
            track.ResolvedSize = 0;
        }
    }

    private void ResolveIntrinsicTrackSizes()
    {
        // For each item, contribute its size to the tracks it spans
        foreach (var item in _gridItems!)
        {
            var size = item.Box.Size;
            float itemWidth = (float)size.Width;
            float itemHeight = (float)size.Height;

            // Distribute width across column tracks
            int colStart = Math.Max(0, item.ColumnStart - 1);
            int colEnd = Math.Min(_columnTracks!.Count, item.ColumnEnd - 1);
            int colSpan = Math.Max(1, colEnd - colStart);

            float widthPerTrack = itemWidth / colSpan;
            for (int i = colStart; i < colEnd && i < _columnTracks.Count; i++)
            {
                _columnTracks[i].BaseSize = Math.Max(_columnTracks[i].BaseSize, widthPerTrack);
            }

            // Distribute height across row tracks
            int rowStart = Math.Max(0, item.RowStart - 1);
            int rowEnd = Math.Min(_rowTracks!.Count, item.RowEnd - 1);
            int rowSpan = Math.Max(1, rowEnd - rowStart);

            float heightPerTrack = itemHeight / rowSpan;
            for (int i = rowStart; i < rowEnd && i < _rowTracks.Count; i++)
            {
                _rowTracks[i].BaseSize = Math.Max(_rowTracks[i].BaseSize, heightPerTrack);
            }
        }
    }

    private void MaximizeTracks()
    {
        // Set resolved size to base size for non-flexible tracks
        foreach (var track in _columnTracks!)
        {
            if (!track.IsFlexible)
            {
                track.ResolvedSize = track.BaseSize;
            }
        }

        foreach (var track in _rowTracks!)
        {
            if (!track.IsFlexible)
            {
                track.ResolvedSize = track.BaseSize;
            }
        }
    }

    private void ExpandFlexibleTracks()
    {
        // §11.7 Expand Flexible Tracks
        // Distribute remaining space to fr tracks proportionally

        // Calculate free space for columns
        float usedColumnSpace = 0;
        float totalColumnFlexFactor = 0;
        foreach (var track in _columnTracks!)
        {
            if (!track.IsFlexible)
            {
                usedColumnSpace += track.ResolvedSize;
            }
            else
            {
                totalColumnFlexFactor += track.FlexFactor;
            }
        }
        // Add gaps
        usedColumnSpace += Math.Max(0, _columnTracks.Count - 1) * _columnGap;

        float freeColumnSpace = Math.Max(0, _availableWidth - usedColumnSpace);

        // Distribute free space to flexible column tracks
        if (totalColumnFlexFactor > 0 && freeColumnSpace > 0)
        {
            // Calculate hypothetical fr size
            float frSize = freeColumnSpace / totalColumnFlexFactor;

            foreach (var track in _columnTracks)
            {
                if (track.IsFlexible)
                {
                    // The track size is max(base size, fr * flex factor)
                    float flexSize = frSize * track.FlexFactor;
                    track.ResolvedSize = Math.Max(track.BaseSize, flexSize);
                }
            }
        }

        // Calculate free space for rows
        float usedRowSpace = 0;
        float totalRowFlexFactor = 0;
        foreach (var track in _rowTracks!)
        {
            if (!track.IsFlexible)
            {
                usedRowSpace += track.ResolvedSize;
            }
            else
            {
                totalRowFlexFactor += track.FlexFactor;
            }
        }
        // Add gaps
        usedRowSpace += Math.Max(0, _rowTracks.Count - 1) * _rowGap;

        float freeRowSpace = Math.Max(0, _availableHeight - usedRowSpace);

        // Distribute free space to flexible row tracks
        if (totalRowFlexFactor > 0 && freeRowSpace > 0)
        {
            // Calculate hypothetical fr size
            float frSize = freeRowSpace / totalRowFlexFactor;

            foreach (var track in _rowTracks)
            {
                if (track.IsFlexible)
                {
                    // The track size is max(base size, fr * flex factor)
                    float flexSize = frSize * track.FlexFactor;
                    track.ResolvedSize = Math.Max(track.BaseSize, flexSize);
                }
            }
        }
    }

    private void StretchAutoTracks()
    {
        // Calculate total used space including gaps
        float totalColumnSpace = 0;
        foreach (var track in _columnTracks!)
        {
            totalColumnSpace += track.ResolvedSize;
        }
        // Add column gaps (one less than number of tracks)
        float totalColumnGaps = Math.Max(0, _columnTracks.Count - 1) * _columnGap;
        totalColumnSpace += totalColumnGaps;

        float totalRowSpace = 0;
        foreach (var track in _rowTracks!)
        {
            totalRowSpace += track.ResolvedSize;
        }
        // Add row gaps (one less than number of tracks)
        float totalRowGaps = Math.Max(0, _rowTracks.Count - 1) * _rowGap;
        totalRowSpace += totalRowGaps;

        // Distribute remaining space equally among auto tracks
        if (_columnTracks.Count > 0 && totalColumnSpace < _availableWidth)
        {
            float extra = (_availableWidth - totalColumnSpace) / _columnTracks.Count;
            foreach (var track in _columnTracks)
            {
                track.ResolvedSize += extra;
            }
        }

        if (_rowTracks.Count > 0 && totalRowSpace < _availableHeight)
        {
            float extra = (_availableHeight - totalRowSpace) / _rowTracks.Count;
            foreach (var track in _rowTracks)
            {
                track.ResolvedSize += extra;
            }
        }
    }

    #endregion

    #region Final Position Calculation

    private void CalculateFinalPositions()
    {
        // Calculate track start positions including gaps
        float columnPosition = 0;
        for (int i = 0; i < _columnTracks!.Count; i++)
        {
            _columnTracks[i].StartPosition = columnPosition;
            columnPosition += _columnTracks[i].ResolvedSize;
            // Add gap after each track except the last
            if (i < _columnTracks.Count - 1)
            {
                columnPosition += _columnGap;
            }
        }

        float rowPosition = 0;
        for (int i = 0; i < _rowTracks!.Count; i++)
        {
            _rowTracks[i].StartPosition = rowPosition;
            rowPosition += _rowTracks[i].ResolvedSize;
            // Add gap after each track except the last
            if (i < _rowTracks.Count - 1)
            {
                rowPosition += _rowGap;
            }
        }

        // Calculate item positions
        foreach (var item in _gridItems!)
        {
            int colStart = Math.Max(0, item.ColumnStart - 1);
            int colEnd = Math.Min(_columnTracks.Count, item.ColumnEnd - 1);
            int rowStart = Math.Max(0, item.RowStart - 1);
            int rowEnd = Math.Min(_rowTracks.Count, item.RowEnd - 1);

            if (colStart < _columnTracks.Count && rowStart < _rowTracks.Count)
            {
                item.X = _columnTracks[colStart].StartPosition;
                item.Y = _rowTracks[rowStart].StartPosition;

                // Calculate width (sum of spanned column tracks + gaps between them)
                item.Width = 0;
                for (int i = colStart; i < colEnd && i < _columnTracks.Count; i++)
                {
                    item.Width += _columnTracks[i].ResolvedSize;
                    // Add gap between tracks (not after last track in span)
                    if (i < colEnd - 1 && i < _columnTracks.Count - 1)
                    {
                        item.Width += _columnGap;
                    }
                }

                // Calculate height (sum of spanned row tracks + gaps between them)
                item.Height = 0;
                for (int i = rowStart; i < rowEnd && i < _rowTracks.Count; i++)
                {
                    item.Height += _rowTracks[i].ResolvedSize;
                    // Add gap between tracks (not after last track in span)
                    if (i < rowEnd - 1 && i < _rowTracks.Count - 1)
                    {
                        item.Height += _rowGap;
                    }
                }
            }
        }
    }

    private void ApplyFinalPositions()
    {
        foreach (var item in _gridItems!)
        {
            item.Box.Position = new Point2f(item.X, item.Y);
        }
    }

    #endregion

    #region Fragmentation Support

    /// <summary>
    /// Handles fragmentation for grid layout.
    /// Spec: https://www.w3.org/TR/css-grid-1/#pagination
    /// Grid fragmentation occurs between row tracks.
    /// </summary>
    private void HandleFragmentation()
    {
        if (_fragmentationContext is null || !IsFragmenting || _rowTracks is null || _gridItems is null)
        {
            return;
        }

        // Grid fragments between row tracks
        // Process each row track and check for fragmentation opportunities
        foreach (var rowTrack in _rowTracks)
        {
            float trackSize = rowTrack.ResolvedSize;

            // Check if row track fits in current fragmentainer
            if (!_fragmentationContext.CanFit(trackSize))
            {
                // Check if any items starting in this row should avoid breaks
                bool avoidBreak = false;
                foreach (var item in _gridItems)
                {
                    int itemRowStart = Math.Max(0, item.RowStart - 1);
                    if (itemRowStart == _rowTracks.IndexOf(rowTrack))
                    {
                        if (_fragmentationContext.ShouldAvoidBreakInside(item.PrincipalBox))
                        {
                            avoidBreak = true;
                            break;
                        }
                    }
                }

                if (!avoidBreak)
                {
                    _fragmentationContext.AdvanceToNextFragmentainer();
                    rowTrack.FragmentIndex = _fragmentationContext.FragmentIndex;
                }
            }

            _fragmentationContext.AdvanceBlockOffset(trackSize + _rowGap);
        }

        // Check for forced breaks on grid items
        foreach (var item in _gridItems)
        {
            // Check break-before
            var breakBeforeResult = _fragmentationContext.EvaluateBreakBefore(item.PrincipalBox);
            if (breakBeforeResult.ShouldBreak && breakBeforeResult.BreakType == EBreakType.Forced)
            {
                _fragmentationContext.AdvanceToNextFragmentainer();
            }

            item.FragmentIndex = _fragmentationContext.FragmentIndex;

            // Check break-after
            var breakAfterResult = _fragmentationContext.EvaluateBreakAfter(item.PrincipalBox);
            if (breakAfterResult.ShouldBreak && breakAfterResult.BreakType == EBreakType.Forced)
            {
                _fragmentationContext.AdvanceToNextFragmentainer();
            }
        }
    }

    #endregion
}

