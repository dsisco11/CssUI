namespace CssUI.CSS.Formatting;

/// <summary>
/// Represents the type of fragmentation container.
/// Docs: https://www.w3.org/TR/css-break-3/#fragmentation-container
/// </summary>
public enum EFragmentainerType
{
    /// <summary>
    /// No fragmentation (continuous layout).
    /// </summary>
    None,

    /// <summary>
    /// Page fragmentation (for printing or paged media).
    /// </summary>
    Page,

    /// <summary>
    /// Column fragmentation (multi-column layout).
    /// </summary>
    Column,

    /// <summary>
    /// Region fragmentation (CSS Regions).
    /// </summary>
    Region
}

/// <summary>
/// Represents a fragmentation break opportunity and its type.
/// Docs: https://www.w3.org/TR/css-break-3/#break-types
/// </summary>
public enum EBreakType
{
    /// <summary>
    /// No break opportunity.
    /// </summary>
    None,

    /// <summary>
    /// Soft break opportunity (can break if needed).
    /// </summary>
    Soft,

    /// <summary>
    /// Forced break opportunity (must break here).
    /// </summary>
    Forced
}

/// <summary>
/// Result of checking fragmentation break conditions.
/// </summary>
public readonly struct FragmentationBreakResult
{
    /// <summary>
    /// The type of break that should occur.
    /// </summary>
    public EBreakType BreakType { get; }

    /// <summary>
    /// The type of fragmentainer that triggers this break.
    /// </summary>
    public EFragmentainerType FragmentainerType { get; }

    /// <summary>
    /// Whether content should be moved to a new fragmentainer.
    /// </summary>
    public bool ShouldBreak => BreakType != EBreakType.None;

    public FragmentationBreakResult(EBreakType breakType, EFragmentainerType fragmentainerType = EFragmentainerType.None)
    {
        BreakType = breakType;
        FragmentainerType = fragmentainerType;
    }

    public static FragmentationBreakResult NoBreak => new(EBreakType.None);
    public static FragmentationBreakResult SoftBreak(EFragmentainerType type) => new(EBreakType.Soft, type);
    public static FragmentationBreakResult ForcedBreak(EFragmentainerType type) => new(EBreakType.Forced, type);
}
