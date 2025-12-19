namespace CssUI.DependencyInjection;

/// <summary>
/// Interface for accessing CssUI engines through dependency injection.
/// This provides a non-static alternative to <see cref="EngineProvider"/> for DI scenarios.
/// </summary>
public interface ICssUIEngineProvider
{
    /// <summary>
    /// Gets the font engine for font resolution, metrics, and text measurement.
    /// </summary>
    IFontEngine FontEngine { get; }

    /// <summary>
    /// Gets the texture engine for image decoding and texture management.
    /// </summary>
    ITextureEngine TextureEngine { get; }

    /// <summary>
    /// Gets the render engine for rendering primitives, textures, and text.
    /// </summary>
    IRenderEngine RenderEngine { get; }
}
