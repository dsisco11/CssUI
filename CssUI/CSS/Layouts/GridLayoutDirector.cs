using CssUI.CSS.Internal;
using System;
using System.Collections.Generic;

namespace CssUI.CSS.Layouts
{
    /// <summary>
    /// Implements CSS Grid Layout Module Level 1.
    /// Spec: https://www.w3.org/TR/css-grid-1/
    /// </summary>
    public sealed class GridLayoutDirector : LayoutDirectorBase
    {
        #region Internal Types

        /// <summary>
        /// Represents a grid track (row or column).
        /// </summary>
        private class GridTrack
        {
            /// <summary>
            /// The track's base size (grows during sizing algorithm).
            /// </summary>
            public float BaseSize { get; set; }

            /// <summary>
            /// The track's growth limit.
            /// </summary>
            public float GrowthLimit { get; set; }

            /// <summary>
            /// The track's minimum track sizing function.
            /// </summary>
            public ETrackSizingFunction MinTrackSizingFunction { get; set; }

            /// <summary>
            /// The track's maximum track sizing function.
            /// </summary>
            public ETrackSizingFunction MaxTrackSizingFunction { get; set; }

            /// <summary>
            /// For flexible tracks, the flex factor.
            /// </summary>
            public float FlexFactor { get; set; }

            /// <summary>
            /// Whether this track can grow infinitely.
            /// </summary>
            public bool InfinitelyGrowable { get; set; }

            /// <summary>
            /// Position of the track's start line.
            /// </summary>
            public float Position { get; set; }
        }

        /// <summary>
        /// Represents a grid item during layout calculations.
        /// </summary>
        private class GridItem
        {
            public cssElement Element { get; set; }

            /// <summary>
            /// The column start line (1-based).
            /// </summary>
            public int ColumnStart { get; set; }

            /// <summary>
            /// The column end line (1-based, exclusive).
            /// </summary>
            public int ColumnEnd { get; set; }

            /// <summary>
            /// The row start line (1-based).
            /// </summary>
            public int RowStart { get; set; }

            /// <summary>
            /// The row end line (1-based, exclusive).
            /// </summary>
            public int RowEnd { get; set; }

            /// <summary>
            /// Number of columns spanned.
            /// </summary>
            public int ColumnSpan => ColumnEnd - ColumnStart;

            /// <summary>
            /// Number of rows spanned.
            /// </summary>
            public int RowSpan => RowEnd - RowStart;

            /// <summary>
            /// Calculated X position.
            /// </summary>
            public float X { get; set; }

            /// <summary>
            /// Calculated Y position.
            /// </summary>
            public float Y { get; set; }

            /// <summary>
            /// Calculated width.
            /// </summary>
            public float Width { get; set; }

            /// <summary>
            /// Calculated height.
            /// </summary>
            public float Height { get; set; }
        }

        /// <summary>
        /// Auto-placement cursor for the auto-placement algorithm.
        /// </summary>
        private struct AutoPlacementCursor
        {
            public int Row;
            public int Column;
        }

        #endregion

        #region Fields

        private List<GridTrack> _columnTracks;
        private List<GridTrack> _rowTracks;
        private List<GridItem> _gridItems;

        // Container properties (cached)
        private EGridAutoFlow _gridAutoFlow;
        private float _availableWidth;
        private float _availableHeight;
        private float _columnGap;
        private float _rowGap;

        // Grid dimensions
        private int _explicitColumnCount;
        private int _explicitRowCount;

        #endregion

        #region Constructors

        public GridLayoutDirector() { }

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
            // TODO: Read from owner.Style once grid properties are implemented
            _gridAutoFlow = EGridAutoFlow.Row; // Default
            _columnGap = 0;
            _rowGap = 0;

            // Determine available space
            Rect2i layoutArea = Owner.Get_Layout_Area();
            _availableWidth = layoutArea.Width;
            _availableHeight = layoutArea.Height;

            // §7: Define the grid
            DefineExplicitGrid(owner);

            // §6 & §8: Generate grid items and place them
            GenerateGridItems(controls);
            PlaceGridItems();

            // Create implicit tracks for auto-placed items
            CreateImplicitTracks();

            // §11: Run the grid sizing algorithm
            RunGridSizingAlgorithm();

            // Calculate final positions and apply
            CalculateTrackPositions();
            ApplyFinalPositions();

            return layoutBlock;
        }

        #endregion

        #region §7 Define the Grid

        /// <summary>
        /// Define the explicit grid from grid-template-* properties.
        /// Spec: https://www.w3.org/TR/css-grid-1/#grid-definition
        /// </summary>
        private void DefineExplicitGrid(cssElement owner)
        {
            // TODO: Parse grid-template-columns, grid-template-rows from style
            // For now, create a simple auto-sized grid

            _columnTracks = new List<GridTrack>();
            _rowTracks = new List<GridTrack>();

            // Default: no explicit tracks (all implicit)
            _explicitColumnCount = 0;
            _explicitRowCount = 0;
        }

        /// <summary>
        /// Create implicit tracks for items that fall outside the explicit grid.
        /// Spec: https://www.w3.org/TR/css-grid-1/#implicit-grids
        /// </summary>
        private void CreateImplicitTracks()
        {
            // Find the maximum row and column needed
            int maxColumn = _explicitColumnCount;
            int maxRow = _explicitRowCount;

            foreach (var item in _gridItems)
            {
                maxColumn = Math.Max(maxColumn, item.ColumnEnd - 1);
                maxRow = Math.Max(maxRow, item.RowEnd - 1);
            }

            // Create column tracks
            while (_columnTracks.Count < maxColumn)
            {
                _columnTracks.Add(new GridTrack
                {
                    MinTrackSizingFunction = ETrackSizingFunction.Auto,
                    MaxTrackSizingFunction = ETrackSizingFunction.Auto,
                    BaseSize = 0,
                    GrowthLimit = float.PositiveInfinity
                });
            }

            // Create row tracks
            while (_rowTracks.Count < maxRow)
            {
                _rowTracks.Add(new GridTrack
                {
                    MinTrackSizingFunction = ETrackSizingFunction.Auto,
                    MaxTrackSizingFunction = ETrackSizingFunction.Auto,
                    BaseSize = 0,
                    GrowthLimit = float.PositiveInfinity
                });
            }
        }

        #endregion

        #region §6 Grid Items & §8 Placement

        /// <summary>
        /// Generate grid items from child elements.
        /// Spec: https://www.w3.org/TR/css-grid-1/#grid-items
        /// </summary>
        private void GenerateGridItems(cssElement[] controls)
        {
            _gridItems = new List<GridItem>(controls.Length);

            foreach (var element in controls)
            {
                // Skip elements that don't participate in layout
                if (!element.Affects_Layout) continue;

                // Absolutely positioned children don't participate in grid layout
                if (element.Style.Positioning == EBoxPositioning.Absolute ||
                    element.Style.Positioning == EBoxPositioning.Fixed)
                {
                    continue;
                }

                var item = new GridItem
                {
                    Element = element,
                    // Default: auto placement
                    ColumnStart = 0,
                    ColumnEnd = 0,
                    RowStart = 0,
                    RowEnd = 0
                };

                _gridItems.Add(item);
            }
        }

        /// <summary>
        /// Place grid items according to the placement algorithm.
        /// Spec: https://www.w3.org/TR/css-grid-1/#auto-placement-algo
        /// </summary>
        private void PlaceGridItems()
        {
            // Step 1: Place items with explicit positions
            PlaceExplicitlyPositionedItems();

            // Step 2: Process items locked to a row (for row flow) or column (for column flow)
            ProcessLockedItems();

            // Step 3: Auto-place remaining items
            AutoPlaceRemainingItems();
        }

        /// <summary>
        /// Place items that have explicit grid-column/grid-row values.
        /// </summary>
        private void PlaceExplicitlyPositionedItems()
        {
            foreach (var item in _gridItems)
            {
                // TODO: Read grid-column-start, grid-column-end, grid-row-start, grid-row-end from style
                // Items with explicit placement will have non-zero values
            }
        }

        /// <summary>
        /// Process items locked to a specific row or column.
        /// </summary>
        private void ProcessLockedItems()
        {
            // TODO: Implement for items with one definite and one auto position
        }

        /// <summary>
        /// Auto-place items without explicit positions.
        /// </summary>
        private void AutoPlaceRemainingItems()
        {
            var cursor = new AutoPlacementCursor { Row = 1, Column = 1 };
            bool isDense = (_gridAutoFlow & EGridAutoFlow.Dense) != 0;
            bool isRowFlow = (_gridAutoFlow & EGridAutoFlow.Column) == 0;

            foreach (var item in _gridItems)
            {
                // Skip items that are already placed
                if (item.ColumnStart != 0 && item.RowStart != 0) continue;

                // Default span is 1
                int columnSpan = 1;
                int rowSpan = 1;

                if (isRowFlow)
                {
                    // Place in current row, advancing columns
                    item.ColumnStart = cursor.Column;
                    item.ColumnEnd = cursor.Column + columnSpan;
                    item.RowStart = cursor.Row;
                    item.RowEnd = cursor.Row + rowSpan;

                    cursor.Column += columnSpan;

                    // Simple row advancement (not handling collisions)
                    // TODO: Implement proper collision detection
                }
                else
                {
                    // Place in current column, advancing rows
                    item.ColumnStart = cursor.Column;
                    item.ColumnEnd = cursor.Column + columnSpan;
                    item.RowStart = cursor.Row;
                    item.RowEnd = cursor.Row + rowSpan;

                    cursor.Row += rowSpan;
                }
            }
        }

        #endregion

        #region §11 Grid Sizing Algorithm

        /// <summary>
        /// Run the grid sizing algorithm.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-grid-sizing
        /// </summary>
        private void RunGridSizingAlgorithm()
        {
            // §11.4: Initialize track sizes
            InitializeTrackSizes(_columnTracks);
            InitializeTrackSizes(_rowTracks);

            // §11.5: Resolve intrinsic track sizes
            ResolveIntrinsicTrackSizes(_columnTracks, true);
            ResolveIntrinsicTrackSizes(_rowTracks, false);

            // §11.6: Maximize tracks
            MaximizeTracks(_columnTracks, _availableWidth);
            MaximizeTracks(_rowTracks, _availableHeight);

            // §11.7: Expand flexible tracks
            ExpandFlexibleTracks(_columnTracks, _availableWidth);
            ExpandFlexibleTracks(_rowTracks, _availableHeight);

            // §11.8: Stretch auto tracks
            StretchAutoTracks(_columnTracks, _availableWidth);
            StretchAutoTracks(_rowTracks, _availableHeight);
        }

        /// <summary>
        /// Initialize track sizes.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-init
        /// </summary>
        private void InitializeTrackSizes(List<GridTrack> tracks)
        {
            foreach (var track in tracks)
            {
                // Initialize base size
                switch (track.MinTrackSizingFunction)
                {
                    case ETrackSizingFunction.Fixed:
                        track.BaseSize = track.BaseSize; // Already set
                        break;
                    default:
                        track.BaseSize = 0;
                        break;
                }

                // Initialize growth limit
                switch (track.MaxTrackSizingFunction)
                {
                    case ETrackSizingFunction.Fixed:
                        track.GrowthLimit = track.GrowthLimit; // Already set
                        break;
                    case ETrackSizingFunction.Flex:
                        track.GrowthLimit = track.BaseSize;
                        break;
                    default:
                        track.GrowthLimit = float.PositiveInfinity;
                        break;
                }
            }
        }

        /// <summary>
        /// Resolve intrinsic track sizes.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-content
        /// </summary>
        private void ResolveIntrinsicTrackSizes(List<GridTrack> tracks, bool isColumnAxis)
        {
            // Size tracks to fit their items' content
            for (int i = 0; i < tracks.Count; i++)
            {
                var track = tracks[i];
                float maxContentSize = 0;

                foreach (var item in _gridItems)
                {
                    bool itemInTrack = isColumnAxis
                        ? (item.ColumnStart <= i + 1 && item.ColumnEnd > i + 1)
                        : (item.RowStart <= i + 1 && item.RowEnd > i + 1);

                    if (itemInTrack)
                    {
                        // Get item's content size
                        var box = item.Element.Box.Content;
                        float contentSize = isColumnAxis ? box.Width : box.Height;
                        maxContentSize = Math.Max(maxContentSize, contentSize);
                    }
                }

                // Update base size for intrinsic tracks
                if (track.MinTrackSizingFunction == ETrackSizingFunction.Auto ||
                    track.MinTrackSizingFunction == ETrackSizingFunction.MinContent ||
                    track.MinTrackSizingFunction == ETrackSizingFunction.MaxContent)
                {
                    track.BaseSize = Math.Max(track.BaseSize, maxContentSize);
                }

                // Update growth limit
                if (float.IsInfinity(track.GrowthLimit))
                {
                    track.GrowthLimit = Math.Max(track.BaseSize, maxContentSize);
                }
            }
        }

        /// <summary>
        /// Maximize tracks to fill available space.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-grow-tracks
        /// </summary>
        private void MaximizeTracks(List<GridTrack> tracks, float availableSpace)
        {
            if (tracks.Count == 0) return;

            float totalBaseSize = 0;
            float totalGaps = (tracks.Count - 1) * (tracks == _columnTracks ? _columnGap : _rowGap);

            foreach (var track in tracks)
            {
                totalBaseSize += track.BaseSize;
            }

            float freeSpace = availableSpace - totalBaseSize - totalGaps;
            if (freeSpace <= 0) return;

            // Distribute free space equally among tracks that can grow
            int growableTracks = 0;
            foreach (var track in tracks)
            {
                if (track.BaseSize < track.GrowthLimit)
                {
                    growableTracks++;
                }
            }

            if (growableTracks == 0) return;

            float spacePerTrack = freeSpace / growableTracks;
            foreach (var track in tracks)
            {
                if (track.BaseSize < track.GrowthLimit)
                {
                    float growth = Math.Min(spacePerTrack, track.GrowthLimit - track.BaseSize);
                    track.BaseSize += growth;
                }
            }
        }

        /// <summary>
        /// Expand flexible tracks (fr units).
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-flex-tracks
        /// </summary>
        private void ExpandFlexibleTracks(List<GridTrack> tracks, float availableSpace)
        {
            // Find flex tracks
            float totalFlexFactor = 0;
            float usedSpace = 0;
            float totalGaps = (tracks.Count - 1) * (tracks == _columnTracks ? _columnGap : _rowGap);

            foreach (var track in tracks)
            {
                if (track.MaxTrackSizingFunction == ETrackSizingFunction.Flex)
                {
                    totalFlexFactor += track.FlexFactor;
                }
                else
                {
                    usedSpace += track.BaseSize;
                }
            }

            if (totalFlexFactor == 0) return;

            float freeSpace = availableSpace - usedSpace - totalGaps;
            if (freeSpace <= 0) return;

            // Find the size of an fr
            float frSize = FindFrSize(tracks, freeSpace, totalFlexFactor);

            // Apply fr sizes
            foreach (var track in tracks)
            {
                if (track.MaxTrackSizingFunction == ETrackSizingFunction.Flex)
                {
                    track.BaseSize = Math.Max(track.BaseSize, frSize * track.FlexFactor);
                }
            }
        }

        /// <summary>
        /// Find the size of an fr unit.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-find-fr-size
        /// </summary>
        private float FindFrSize(List<GridTrack> tracks, float space, float totalFlexFactor)
        {
            if (totalFlexFactor < 1) totalFlexFactor = 1;
            return space / totalFlexFactor;
        }

        /// <summary>
        /// Stretch auto tracks if align-content/justify-content is stretch.
        /// Spec: https://www.w3.org/TR/css-grid-1/#algo-stretch
        /// </summary>
        private void StretchAutoTracks(List<GridTrack> tracks, float availableSpace)
        {
            // TODO: Check align-content/justify-content for stretch
            // For now, skip stretching
        }

        #endregion

        #region Calculate Positions

        /// <summary>
        /// Calculate the position of each track.
        /// </summary>
        private void CalculateTrackPositions()
        {
            // Column positions
            float position = 0;
            foreach (var track in _columnTracks)
            {
                track.Position = position;
                position += track.BaseSize + _columnGap;
            }

            // Row positions
            position = 0;
            foreach (var track in _rowTracks)
            {
                track.Position = position;
                position += track.BaseSize + _rowGap;
            }
        }

        /// <summary>
        /// Apply calculated positions to grid items.
        /// </summary>
        private void ApplyFinalPositions()
        {
            int maxX = 0, maxY = 0;

            foreach (var item in _gridItems)
            {
                // Calculate position from tracks
                int colIndex = item.ColumnStart - 1;
                int rowIndex = item.RowStart - 1;

                if (colIndex >= 0 && colIndex < _columnTracks.Count &&
                    rowIndex >= 0 && rowIndex < _rowTracks.Count)
                {
                    item.X = _columnTracks[colIndex].Position;
                    item.Y = _rowTracks[rowIndex].Position;

                    // Calculate size (sum of spanned tracks)
                    item.Width = 0;
                    for (int i = colIndex; i < item.ColumnEnd - 1 && i < _columnTracks.Count; i++)
                    {
                        item.Width += _columnTracks[i].BaseSize;
                        if (i < item.ColumnEnd - 2) item.Width += _columnGap;
                    }

                    item.Height = 0;
                    for (int i = rowIndex; i < item.RowEnd - 1 && i < _rowTracks.Count; i++)
                    {
                        item.Height += _rowTracks[i].BaseSize;
                        if (i < item.RowEnd - 2) item.Height += _rowGap;
                    }

                    item.Element.Box.Set_Layout_Pos((int)item.X, (int)item.Y);

                    maxX = Math.Max(maxX, (int)(item.X + item.Width));
                    maxY = Math.Max(maxY, (int)(item.Y + item.Height));
                }
            }

            layoutBlock.Set_Dimensions(maxX, maxY);
        }

        #endregion

        #region Reset

        protected override void Reset()
        {
            base.Reset();
            _columnTracks = null;
            _rowTracks = null;
            _gridItems = null;
        }

        #endregion
    }

    /// <summary>
    /// Track sizing function types for grid layout.
    /// </summary>
    public enum ETrackSizingFunction
    {
        /// <summary>Fixed length or percentage.</summary>
        Fixed,
        /// <summary>min-content sizing.</summary>
        MinContent,
        /// <summary>max-content sizing.</summary>
        MaxContent,
        /// <summary>auto sizing.</summary>
        Auto,
        /// <summary>Flexible fr unit.</summary>
        Flex
    }
}
