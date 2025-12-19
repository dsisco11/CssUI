using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Types;

/// <summary>
/// Represents a CSS grid track sizing function.
/// Spec: https://www.w3.org/TR/css-grid-1/#track-sizing
/// </summary>
public readonly struct GridTrackSize
{
    /// <summary>
    /// The type of sizing function.
    /// </summary>
    public readonly EGridTrackSizeType Type;

    /// <summary>
    /// The numeric value (for fixed, percentage, flex, or minmax bounds).
    /// </summary>
    public readonly double Value;

    /// <summary>
    /// The unit for fixed values (px, em, etc.) or FR for flex.
    /// </summary>
    public readonly ECssUnit Unit;

    /// <summary>
    /// For minmax(), the minimum and maximum size indices into a shared list.
    /// Stored as packed values to avoid struct cycle.
    /// </summary>
    private readonly double _minValue;
    private readonly ECssUnit _minUnit;
    private readonly EGridTrackSizeType _minType;
    private readonly double _maxValue;
    private readonly ECssUnit _maxUnit;
    private readonly EGridTrackSizeType _maxType;

    #region Constructors

    /// <summary>
    /// Creates an auto track size.
    /// </summary>
    public static GridTrackSize Auto => new GridTrackSize(EGridTrackSizeType.Auto);

    /// <summary>
    /// Creates a min-content track size.
    /// </summary>
    public static GridTrackSize MinContent => new GridTrackSize(EGridTrackSizeType.MinContent);

    /// <summary>
    /// Creates a max-content track size.
    /// </summary>
    public static GridTrackSize MaxContent => new GridTrackSize(EGridTrackSizeType.MaxContent);

    /// <summary>
    /// Creates an intrinsic keyword track size.
    /// </summary>
    private GridTrackSize(EGridTrackSizeType type)
    {
        Type = type;
        Value = 0;
        Unit = ECssUnit.None;
        _minValue = 0;
        _minUnit = ECssUnit.None;
        _minType = EGridTrackSizeType.Auto;
        _maxValue = 0;
        _maxUnit = ECssUnit.None;
        _maxType = EGridTrackSizeType.Auto;
    }

    /// <summary>
    /// Creates a fixed length track size.
    /// </summary>
    public GridTrackSize(double value, ECssUnit unit)
    {
        Type = EGridTrackSizeType.Fixed;
        Value = value;
        Unit = unit;
        _minValue = 0;
        _minUnit = ECssUnit.None;
        _minType = EGridTrackSizeType.Auto;
        _maxValue = 0;
        _maxUnit = ECssUnit.None;
        _maxType = EGridTrackSizeType.Auto;
    }

    /// <summary>
    /// Creates a percentage track size.
    /// </summary>
    public static GridTrackSize Percentage(double percentage)
    {
        return new GridTrackSize(EGridTrackSizeType.Percentage, percentage, ECssUnit.None);
    }

    private GridTrackSize(EGridTrackSizeType type, double value, ECssUnit unit)
    {
        Type = type;
        Value = value;
        Unit = unit;
        _minValue = 0;
        _minUnit = ECssUnit.None;
        _minType = EGridTrackSizeType.Auto;
        _maxValue = 0;
        _maxUnit = ECssUnit.None;
        _maxType = EGridTrackSizeType.Auto;
    }

    /// <summary>
    /// Creates a flexible (fr) track size.
    /// </summary>
    public static GridTrackSize Flex(double factor)
    {
        return new GridTrackSize(factor, ECssUnit.FR);
    }

    /// <summary>
    /// Creates a minmax() track size.
    /// </summary>
    public static GridTrackSize Minmax(GridTrackSize min, GridTrackSize max)
    {
        return new GridTrackSize(min, max);
    }

    private GridTrackSize(GridTrackSize min, GridTrackSize max)
    {
        Type = EGridTrackSizeType.Minmax;
        Value = 0;
        Unit = ECssUnit.None;
        _minValue = min.Value;
        _minUnit = min.Unit;
        _minType = min.Type;
        _maxValue = max.Value;
        _maxUnit = max.Unit;
        _maxType = max.Type;
    }

    /// <summary>
    /// Creates a fit-content() track size.
    /// </summary>
    public static GridTrackSize FitContent(double maxValue, ECssUnit unit)
    {
        return new GridTrackSize(EGridTrackSizeType.FitContent, maxValue, unit);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Whether this is a flexible (fr) sizing function.
    /// </summary>
    public bool IsFlexible => Unit == ECssUnit.FR;

    /// <summary>
    /// Whether this is an intrinsic sizing function (auto, min-content, max-content, fit-content).
    /// </summary>
    public bool IsIntrinsic => Type == EGridTrackSizeType.Auto ||
                                Type == EGridTrackSizeType.MinContent ||
                                Type == EGridTrackSizeType.MaxContent ||
                                Type == EGridTrackSizeType.FitContent;

    /// <summary>
    /// The flex factor for fr units (0 if not flexible).
    /// </summary>
    public double FlexFactor => IsFlexible ? Value : 0;

    /// <summary>
    /// For minmax(), gets the minimum size.
    /// </summary>
    public GridTrackSize MinSize => Type == EGridTrackSizeType.Minmax
        ? new GridTrackSize(_minType, _minValue, _minUnit)
        : this;

    /// <summary>
    /// For minmax(), gets the maximum size.
    /// </summary>
    public GridTrackSize MaxSize => Type == EGridTrackSizeType.Minmax
        ? new GridTrackSize(_maxType, _maxValue, _maxUnit)
        : this;

    #endregion

    #region Resolution

    /// <summary>
    /// Resolves the track size to a pixel value given the available space.
    /// </summary>
    /// <param name="availableSpace">The available space for percentage/flex calculations.</param>
    /// <param name="unitResolver">Resolver for CSS units.</param>
    /// <param name="totalFlexFactor">Total flex factor for fr calculations.</param>
    /// <param name="freeSpace">Free space for fr calculations.</param>
    /// <returns>The resolved size in pixels, or null if intrinsic.</returns>
    public double? Resolve(double availableSpace, CssValue.StyleUnitResolverDelegate? unitResolver,
        double totalFlexFactor = 0, double freeSpace = 0)
    {
        switch (Type)
        {
            case EGridTrackSizeType.Fixed:
                if (Unit == ECssUnit.FR)
                {
                    // Flexible: distribute free space proportionally
                    if (totalFlexFactor > 0)
                    {
                        return (Value / totalFlexFactor) * freeSpace;
                    }
                    return 0;
                }
                // Fixed length
                if (unitResolver != null)
                {
                    return Value * unitResolver(Unit);
                }
                return Value;

            case EGridTrackSizeType.Percentage:
                return (Value / 100.0) * availableSpace;

            case EGridTrackSizeType.Minmax:
                // Minmax resolution depends on context - return null to indicate intrinsic
                return null;

            case EGridTrackSizeType.Auto:
            case EGridTrackSizeType.MinContent:
            case EGridTrackSizeType.MaxContent:
            case EGridTrackSizeType.FitContent:
                // Intrinsic sizes need item measurements
                return null;

            default:
                return 0;
        }
    }

    #endregion

    public override string ToString()
    {
        return Type switch
        {
            EGridTrackSizeType.Auto => "auto",
            EGridTrackSizeType.MinContent => "min-content",
            EGridTrackSizeType.MaxContent => "max-content",
            EGridTrackSizeType.Fixed when Unit == ECssUnit.FR => $"{Value}fr",
            EGridTrackSizeType.Fixed => $"{Value}{Unit.ToString().ToLower()}",
            EGridTrackSizeType.Percentage => $"{Value}%",
            EGridTrackSizeType.Minmax => $"minmax({MinSize}, {MaxSize})",
            EGridTrackSizeType.FitContent => $"fit-content({Value}{Unit.ToString().ToLower()})",
            _ => "auto"
        };
    }
}

/// <summary>
/// Types of grid track sizing functions.
/// </summary>
public enum EGridTrackSizeType
{
    /// <summary>
    /// auto - sized based on content
    /// </summary>
    Auto,

    /// <summary>
    /// Fixed length value (px, em, etc.) or fr unit
    /// </summary>
    Fixed,

    /// <summary>
    /// Percentage of containing block
    /// </summary>
    Percentage,

    /// <summary>
    /// min-content intrinsic size
    /// </summary>
    MinContent,

    /// <summary>
    /// max-content intrinsic size
    /// </summary>
    MaxContent,

    /// <summary>
    /// minmax(min, max) sizing function
    /// </summary>
    Minmax,

    /// <summary>
    /// fit-content(limit) sizing function
    /// </summary>
    FitContent
}

/// <summary>
/// Represents a list of grid track sizes for grid-template-columns or grid-template-rows.
/// Spec: https://www.w3.org/TR/css-grid-1/#track-sizing
/// </summary>
public class GridTrackList
{
    private readonly List<GridTrackSize> _tracks;

    /// <summary>
    /// The number of explicit tracks.
    /// </summary>
    public int Count => _tracks.Count;

    /// <summary>
    /// Gets the track size at the specified index.
    /// </summary>
    public GridTrackSize this[int index] => _tracks[index];

    /// <summary>
    /// Creates an empty track list.
    /// </summary>
    public GridTrackList()
    {
        _tracks = new List<GridTrackSize>();
    }

    /// <summary>
    /// Creates a track list with the specified sizes.
    /// </summary>
    public GridTrackList(IEnumerable<GridTrackSize> sizes)
    {
        _tracks = new List<GridTrackSize>(sizes);
    }

    /// <summary>
    /// Adds a track to the list.
    /// </summary>
    public void Add(GridTrackSize size)
    {
        _tracks.Add(size);
    }

    /// <summary>
    /// Gets all tracks as an enumerable.
    /// </summary>
    public IEnumerable<GridTrackSize> GetTracks() => _tracks;

    /// <summary>
    /// Whether this track list contains any flexible (fr) tracks.
    /// </summary>
    public bool HasFlexibleTracks
    {
        get
        {
            foreach (var track in _tracks)
            {
                if (track.IsFlexible) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Calculates the total flex factor from all fr tracks.
    /// </summary>
    public double TotalFlexFactor
    {
        get
        {
            double total = 0;
            foreach (var track in _tracks)
            {
                if (track.IsFlexible)
                {
                    total += track.FlexFactor;
                }
            }
            return total;
        }
    }

    /// <summary>
    /// Parses a grid track list from a string.
    /// Supports: fixed lengths, percentages, fr units, auto, min-content, max-content
    /// </summary>
    public static GridTrackList Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            return new GridTrackList();
        }

        var result = new GridTrackList();
        var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var trackSize = ParseTrackSize(part.Trim());
            if (trackSize.HasValue)
            {
                result.Add(trackSize.Value);
            }
        }

        return result;
    }

    private static GridTrackSize? ParseTrackSize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.ToLowerInvariant();

        // Keywords
        if (value == "auto")
            return GridTrackSize.Auto;
        if (value == "min-content")
            return GridTrackSize.MinContent;
        if (value == "max-content")
            return GridTrackSize.MaxContent;

        // fr unit
        if (value.EndsWith("fr"))
        {
            if (double.TryParse(value.Substring(0, value.Length - 2), out double fr))
            {
                return GridTrackSize.Flex(fr);
            }
        }

        // Percentage
        if (value.EndsWith("%"))
        {
            if (double.TryParse(value.Substring(0, value.Length - 1), out double pct))
            {
                return GridTrackSize.Percentage(pct);
            }
        }

        // Fixed lengths
        if (TryParseLength(value, out double numValue, out ECssUnit unit))
        {
            return new GridTrackSize(numValue, unit);
        }

        // TODO: Parse minmax(), fit-content(), repeat()

        return null;
    }

    private static bool TryParseLength(string value, out double numValue, out ECssUnit unit)
    {
        numValue = 0;
        unit = ECssUnit.PX;

        var unitStrings = new (string suffix, ECssUnit cssUnit)[]
        {
            ("px", ECssUnit.PX),
            ("em", ECssUnit.EM),
            ("rem", ECssUnit.REM),
            ("vh", ECssUnit.VH),
            ("vw", ECssUnit.VW),
            ("vmin", ECssUnit.VMIN),
            ("vmax", ECssUnit.VMAX),
            ("pt", ECssUnit.PT),
            ("pc", ECssUnit.PC),
            ("in", ECssUnit.IN),
            ("cm", ECssUnit.CM),
            ("mm", ECssUnit.MM),
            ("ch", ECssUnit.CH),
            ("ex", ECssUnit.EX),
        };

        foreach (var (suffix, cssUnit) in unitStrings)
        {
            if (value.EndsWith(suffix))
            {
                if (double.TryParse(value.Substring(0, value.Length - suffix.Length), out numValue))
                {
                    unit = cssUnit;
                    return true;
                }
            }
        }

        // Unitless number (treat as pixels)
        if (double.TryParse(value, out numValue))
        {
            unit = ECssUnit.PX;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        if (_tracks.Count == 0)
            return "none";

        var sb = new StringBuilder();
        for (int i = 0; i < _tracks.Count; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(_tracks[i].ToString());
        }
        return sb.ToString();
    }
}

