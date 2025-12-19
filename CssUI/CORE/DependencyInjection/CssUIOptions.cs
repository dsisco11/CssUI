namespace CssUI.DependencyInjection;

/// <summary>
/// Configuration options for CssUI services when using dependency injection.
/// </summary>
public sealed class CssUIOptions
{
    /// <summary>
    /// Gets or sets the font engine implementation.
    /// If null, <see cref="NullFontEngine"/> will be used.
    /// </summary>
    public IFontEngine? FontEngine { get; set; }

    /// <summary>
    /// Gets or sets the texture engine implementation.
    /// If null, <see cref="NullTextureEngine"/> will be used.
    /// </summary>
    public ITextureEngine? TextureEngine { get; set; }

    /// <summary>
    /// Gets or sets the render engine implementation.
    /// If null, <see cref="NullRenderEngine"/> will be used.
    /// </summary>
    public IRenderEngine? RenderEngine { get; set; }
}
