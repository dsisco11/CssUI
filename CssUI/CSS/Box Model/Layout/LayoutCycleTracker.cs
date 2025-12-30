using System;
using System.Collections.Generic;
using System.Threading;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Layout;

/// <summary>
/// Tracks boxes currently being resolved to detect circular layout dependencies.
/// </summary>
/// <remarks>
/// Layout cycles can occur when:
/// <list type="bullet">
/// <item>A child has percentage height that depends on parent's height</item>
/// <item>Parent has height: auto (depends on children's content heights)</item>
/// </list>
/// When a cycle is detected, the percentage is treated as auto and a warning is logged.
/// <para>
/// See: CSS 2.2 §10.5 - "If the height of the containing block is not specified explicitly
/// (i.e., it depends on content height), and this element is not absolutely positioned,
/// the value computes to 'auto'."
/// </para>
/// </remarks>
public sealed class LayoutCycleTracker
{
    #region Thread-Local Instance
    /// <summary>
    /// Thread-local tracker instance for concurrent layout operations.
    /// Each thread gets its own tracker to avoid cross-thread interference.
    /// </summary>
    private static readonly ThreadLocal<LayoutCycleTracker> _instance =
        new(() => new LayoutCycleTracker());

    /// <summary>
    /// Gets the current thread's layout cycle tracker.
    /// </summary>
    public static LayoutCycleTracker Current => _instance.Value!;
    #endregion

    #region Fields
    /// <summary>
    /// Stack of boxes currently being resolved (width pass).
    /// </summary>
    private readonly Stack<CssPrincipalBox> _widthResolutionStack = new();

    /// <summary>
    /// Stack of boxes currently being resolved (height pass).
    /// </summary>
    private readonly Stack<CssPrincipalBox> _heightResolutionStack = new();

    /// <summary>
    /// Set of boxes currently being resolved for O(1) lookup.
    /// </summary>
    private readonly HashSet<CssPrincipalBox> _resolvingBoxes = new();

    /// <summary>
    /// Number of cycles detected during the current layout pass.
    /// </summary>
    private int _cycleCount;

    /// <summary>
    /// Maximum depth allowed before assuming infinite loop.
    /// </summary>
    private const int MaxResolutionDepth = 1000;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the number of layout cycles detected during the current session.
    /// </summary>
    public int CycleCount => _cycleCount;

    /// <summary>
    /// Gets the current resolution depth.
    /// </summary>
    public int CurrentDepth => _widthResolutionStack.Count + _heightResolutionStack.Count;
    #endregion

    #region Constructor
    private LayoutCycleTracker()
    {
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Begins tracking width resolution for a box.
    /// </summary>
    /// <param name="box">The box being resolved</param>
    /// <returns>True if resolution can proceed; false if a cycle was detected</returns>
    public bool BeginWidthResolution(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        if (CurrentDepth >= MaxResolutionDepth)
        {
            Log.Warn($"[LayoutCycle] Maximum resolution depth ({MaxResolutionDepth}) exceeded. " +
                $"Possible infinite loop in layout for element <{box.Owner?.localName ?? "unknown"}>.");
            return false;
        }

        if (_resolvingBoxes.Contains(box))
        {
            _cycleCount++;
            Log.Warn($"[LayoutCycle] Width cycle detected for element <{box.Owner?.localName ?? "unknown"}> " +
                $"(ID: {box.Owner?.id ?? "none"}). Treating percentage as auto.");
            return false;
        }

        _widthResolutionStack.Push(box);
        _resolvingBoxes.Add(box);
        return true;
    }

    /// <summary>
    /// Ends tracking width resolution for a box.
    /// </summary>
    /// <param name="box">The box that was being resolved</param>
    public void EndWidthResolution(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        if (_widthResolutionStack.Count == 0 || _widthResolutionStack.Peek() != box)
        {
            Log.Warn($"[LayoutCycle] EndWidthResolution called for wrong box. " +
                $"Expected: <{_widthResolutionStack.Peek()?.Owner?.localName ?? "empty stack"}>, " +
                $"Got: <{box.Owner?.localName ?? "unknown"}>.");
            return;
        }

        _widthResolutionStack.Pop();
        _resolvingBoxes.Remove(box);
    }

    /// <summary>
    /// Begins tracking height resolution for a box.
    /// </summary>
    /// <param name="box">The box being resolved</param>
    /// <returns>True if resolution can proceed; false if a cycle was detected</returns>
    public bool BeginHeightResolution(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        if (CurrentDepth >= MaxResolutionDepth)
        {
            Log.Warn($"[LayoutCycle] Maximum resolution depth ({MaxResolutionDepth}) exceeded. " +
                $"Possible infinite loop in layout for element <{box.Owner?.localName ?? "unknown"}>.");
            return false;
        }

        if (_resolvingBoxes.Contains(box))
        {
            _cycleCount++;
            Log.Warn($"[LayoutCycle] Height cycle detected for element <{box.Owner?.localName ?? "unknown"}> " +
                $"(ID: {box.Owner?.id ?? "none"}). Treating percentage as auto.");
            return false;
        }

        _heightResolutionStack.Push(box);
        _resolvingBoxes.Add(box);
        return true;
    }

    /// <summary>
    /// Ends tracking height resolution for a box.
    /// </summary>
    /// <param name="box">The box that was being resolved</param>
    public void EndHeightResolution(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        if (_heightResolutionStack.Count == 0 || _heightResolutionStack.Peek() != box)
        {
            Log.Warn($"[LayoutCycle] EndHeightResolution called for wrong box. " +
                $"Expected: <{_heightResolutionStack.Peek()?.Owner?.localName ?? "empty stack"}>, " +
                $"Got: <{box.Owner?.localName ?? "unknown"}>.");
            return;
        }

        _heightResolutionStack.Pop();
        _resolvingBoxes.Remove(box);
    }

    /// <summary>
    /// Checks if a box is currently being resolved.
    /// </summary>
    /// <param name="box">The box to check</param>
    /// <returns>True if the box is being resolved; false otherwise</returns>
    public bool IsBeingResolved(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);
        return _resolvingBoxes.Contains(box);
    }

    /// <summary>
    /// Checks if the parent of the given box is currently being resolved.
    /// This indicates a potential cycle if the child needs the parent's resolved dimensions.
    /// </summary>
    /// <param name="box">The box to check</param>
    /// <returns>True if the parent box is being resolved; false otherwise</returns>
    public bool IsParentBeingResolved(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        var parentBox = box.Owner?.parentElement?.Box;
        if (parentBox is null)
            return false;

        return _resolvingBoxes.Contains(parentBox);
    }

    /// <summary>
    /// Checks if any ancestor of the given box is currently being resolved.
    /// </summary>
    /// <param name="box">The box to check</param>
    /// <returns>The first ancestor box being resolved, or null if none</returns>
    public CssPrincipalBox? FindResolvingAncestor(CssPrincipalBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        var current = box.Owner?.parentElement;
        while (current is not null)
        {
            if (current.Box is CssPrincipalBox ancestorBox && _resolvingBoxes.Contains(ancestorBox))
            {
                return ancestorBox;
            }
            current = current.parentElement;
        }

        return null;
    }

    /// <summary>
    /// Resets the tracker state. Call at the start of a new layout pass.
    /// </summary>
    public void Reset()
    {
        _widthResolutionStack.Clear();
        _heightResolutionStack.Clear();
        _resolvingBoxes.Clear();
        _cycleCount = 0;
    }
    #endregion
}
