using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Represents a fragmentation context that manages breaking content across fragmentainers.
/// Docs: https://www.w3.org/TR/css-break-3/
/// </summary>
public class FragmentationContext
{
    #region Fields
    private readonly EFragmentainerType _fragmentainerType;
    private readonly float _fragmentainerBlockSize;
    private float _currentBlockOffset;
    private int _fragmentIndex;
    #endregion

    #region Properties
    /// <summary>
    /// The type of fragmentation container.
    /// </summary>
    public EFragmentainerType FragmentainerType => _fragmentainerType;

    /// <summary>
    /// The block size of each fragmentainer (e.g., page height).
    /// </summary>
    public float FragmentainerBlockSize => _fragmentainerBlockSize;

    /// <summary>
    /// The current block offset within the current fragmentainer.
    /// </summary>
    public float CurrentBlockOffset => _currentBlockOffset;

    /// <summary>
    /// The remaining block space in the current fragmentainer.
    /// </summary>
    public float RemainingBlockSpace => _fragmentainerBlockSize - _currentBlockOffset;

    /// <summary>
    /// The index of the current fragment (page number, column index, etc.).
    /// </summary>
    public int FragmentIndex => _fragmentIndex;

    /// <summary>
    /// Whether this context has fragmentation enabled.
    /// </summary>
    public bool IsFragmenting => _fragmentainerType != EFragmentainerType.None;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a fragmentation context with no fragmentation (continuous layout).
    /// </summary>
    public FragmentationContext()
    {
        _fragmentainerType = EFragmentainerType.None;
        _fragmentainerBlockSize = float.MaxValue;
        _currentBlockOffset = 0;
        _fragmentIndex = 0;
    }

    /// <summary>
    /// Creates a fragmentation context with the specified fragmentainer type and size.
    /// </summary>
    public FragmentationContext(EFragmentainerType type, float blockSize)
    {
        _fragmentainerType = type;
        _fragmentainerBlockSize = blockSize;
        _currentBlockOffset = 0;
        _fragmentIndex = 0;
    }
    #endregion

    #region Break Evaluation
    /// <summary>
    /// Evaluates whether a break should occur before the given box.
    /// </summary>
    public FragmentationBreakResult EvaluateBreakBefore(CssPrincipalBox? box)
    {
        if (!IsFragmenting || box is null)
        {
            return FragmentationBreakResult.NoBreak;
        }

        var style = box.Style;
        if (style is null)
        {
            return FragmentationBreakResult.NoBreak;
        }

        var breakBefore = style.BreakBefore;
        return EvaluateBreakValue(breakBefore, isBreakInside: false);
    }

    /// <summary>
    /// Evaluates whether a break should occur after the given box.
    /// </summary>
    public FragmentationBreakResult EvaluateBreakAfter(CssPrincipalBox? box)
    {
        if (!IsFragmenting || box is null)
        {
            return FragmentationBreakResult.NoBreak;
        }

        var style = box.Style;
        if (style is null)
        {
            return FragmentationBreakResult.NoBreak;
        }

        var breakAfter = style.BreakAfter;
        return EvaluateBreakValue(breakAfter, isBreakInside: false);
    }

    /// <summary>
    /// Evaluates whether breaks should be avoided inside the given box.
    /// </summary>
    public bool ShouldAvoidBreakInside(CssPrincipalBox? box)
    {
        if (!IsFragmenting || box is null)
        {
            return false;
        }

        var style = box.Style;
        if (style is null)
        {
            return false;
        }

        var breakInside = style.BreakInside;
        return breakInside switch
        {
            EBreakValue.Avoid => true,
            EBreakValue.AvoidPage => _fragmentainerType == EFragmentainerType.Page,
            EBreakValue.AvoidColumn => _fragmentainerType == EFragmentainerType.Column,
            EBreakValue.AvoidRegion => _fragmentainerType == EFragmentainerType.Region,
            _ => false
        };
    }

    /// <summary>
    /// Evaluates a break value and returns the appropriate break result.
    /// </summary>
    private FragmentationBreakResult EvaluateBreakValue(EBreakValue value, bool isBreakInside)
    {
        return value switch
        {
            EBreakValue.Auto => FragmentationBreakResult.NoBreak,
            EBreakValue.Avoid => FragmentationBreakResult.NoBreak,
            EBreakValue.AvoidPage => FragmentationBreakResult.NoBreak,
            EBreakValue.AvoidColumn => FragmentationBreakResult.NoBreak,
            EBreakValue.AvoidRegion => FragmentationBreakResult.NoBreak,
            EBreakValue.Page => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page),
            EBreakValue.Column => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Column),
            EBreakValue.Region => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Region),
            EBreakValue.Left => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page),
            EBreakValue.Right => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page),
            EBreakValue.Recto => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page),
            EBreakValue.Verso => FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page),
            _ => FragmentationBreakResult.NoBreak
        };
    }
    #endregion

    #region Space Management
    /// <summary>
    /// Checks if the given block size can fit in the current fragmentainer.
    /// </summary>
    public bool CanFit(float blockSize)
    {
        if (!IsFragmenting)
        {
            return true;
        }

        return _currentBlockOffset + blockSize <= _fragmentainerBlockSize;
    }

    /// <summary>
    /// Advances the block offset by the given amount.
    /// </summary>
    public void AdvanceBlockOffset(float amount)
    {
        _currentBlockOffset += amount;
    }

    /// <summary>
    /// Moves to the next fragmentainer (page, column, etc.).
    /// </summary>
    public void AdvanceToNextFragmentainer()
    {
        _fragmentIndex++;
        _currentBlockOffset = 0;
    }

    /// <summary>
    /// Resets the fragmentation context for a new layout pass.
    /// </summary>
    public void Reset()
    {
        _currentBlockOffset = 0;
        _fragmentIndex = 0;
    }
    #endregion

    #region Orphans/Widows
    /// <summary>
    /// Checks if the minimum orphan lines requirement is satisfied.
    /// </summary>
    public bool CheckOrphans(CssPrincipalBox? box, int linesBefore)
    {
        if (!IsFragmenting || box is null)
        {
            return true;
        }

        var style = box.Style;
        int minOrphans = style?.Orphans ?? 2;
        return linesBefore >= minOrphans;
    }

    /// <summary>
    /// Checks if the minimum widow lines requirement is satisfied.
    /// </summary>
    public bool CheckWidows(CssPrincipalBox? box, int linesAfter)
    {
        if (!IsFragmenting || box is null)
        {
            return true;
        }

        var style = box.Style;
        int minWidows = style?.Widows ?? 2;
        return linesAfter >= minWidows;
    }
    #endregion
}
