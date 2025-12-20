namespace CssUI.Common;

/// <summary>
/// Opaque handle to a mesh managed by an <see cref="IMeshService"/>.
/// </summary>
public readonly record struct MeshHandle(int Id)
{
    /// <summary>
    /// Returns true if this handle is null/invalid.
    /// </summary>
    public bool IsNull => Id == 0;

    /// <summary>
    /// A null mesh handle.
    /// </summary>
    public static MeshHandle Null => default;

    public override string ToString() => $"MeshHandle({Id})";
}

