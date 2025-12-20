namespace CssUI.DependencyInjection;

/// <summary>
/// Configuration options for CssUI services when using dependency injection.
/// </summary>
public sealed class CssUIOptions
{
    /// <summary>
    /// Gets or sets the font service implementation.
    /// If null, <see cref="NullFontService"/> will be used.
    /// </summary>
    public IFontService? FontService { get; set; }

    /// <summary>
    /// Gets or sets the texture service implementation.
    /// If null, <see cref="NullTextureService"/> will be used.
    /// </summary>
    public ITextureService? TextureService { get; set; }

    /// <summary>
    /// Gets or sets the render service implementation.
    /// If null, <see cref="NullRenderService"/> will be used.
    /// </summary>
    public IRenderService? RenderService { get; set; }
}
