namespace CssUI.Common;

/// <summary>
/// Opaque handle to a texture managed by an <see cref="ITextureService"/>.
/// </summary>
public readonly record struct TextureHandle(int Id)
{
    /// <summary>
    /// Returns true if this handle is null/invalid.
    /// </summary>
    public bool IsNull => Id == 0;

    /// <summary>
    /// A null texture handle.
    /// </summary>
    public static TextureHandle Null => default;

    public override string ToString() => $"TextureHandle({Id})";
}

