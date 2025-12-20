namespace CssUI;

/// <summary>
/// Opaque handle to a font instance managed by an <see cref="IFontService"/>.
/// </summary>
public readonly record struct FontHandle(int Id)
{
    /// <summary>
    /// Returns true if this handle is null/invalid.
    /// </summary>
    public bool IsNull => Id == 0;

    /// <summary>
    /// A null font handle.
    /// </summary>
    public static FontHandle Null => default;

    public override string ToString() => $"FontHandle({Id})";
}

