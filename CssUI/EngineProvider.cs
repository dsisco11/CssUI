using System;

namespace CssUI
{
    /// <summary>
    /// Configuration for initializing CssUI with engine implementations.
    /// </summary>
    public sealed class CssUIConfig
    {
        /// <summary>
        /// Engine for font resolution, metrics, and text measurement.
        /// </summary>
        public required IFontEngine FontEngine { get; init; }

        /// <summary>
        /// Engine for image decoding and texture management.
        /// </summary>
        public required ITextureEngine TextureEngine { get; init; }

        /// <summary>
        /// Engine for rendering primitives, textures, and text.
        /// </summary>
        public required IRenderEngine RenderEngine { get; init; }
    }

    /// <summary>
    /// Global access to engine instances after initialization.
    /// </summary>
    public static class EngineProvider
    {
        private static IFontEngine _fontEngine;
        private static ITextureEngine _textureEngine;
        private static IRenderEngine _renderEngine;
        private static bool _initialized;

        /// <summary>
        /// Initialize CssUI with the provided engine implementations.
        /// Must be called before using any CssUI functionality.
        /// </summary>
        /// <param name="config">Configuration with engine implementations.</param>
        /// <exception cref="ArgumentNullException">If any engine is null.</exception>
        /// <exception cref="InvalidOperationException">If already initialized.</exception>
        public static void Initialize(CssUIConfig config)
        {
            if (_initialized)
                throw new InvalidOperationException("CssUI has already been initialized.");

            _fontEngine = config.FontEngine ?? throw new ArgumentNullException(nameof(config.FontEngine));
            _textureEngine = config.TextureEngine ?? throw new ArgumentNullException(nameof(config.TextureEngine));
            _renderEngine = config.RenderEngine ?? throw new ArgumentNullException(nameof(config.RenderEngine));
            _initialized = true;
        }

        /// <summary>
        /// Reset the engine provider (mainly for testing).
        /// </summary>
        internal static void Reset()
        {
            _fontEngine = null;
            _textureEngine = null;
            _renderEngine = null;
            _initialized = false;
        }

        /// <summary>
        /// Get the font engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static IFontEngine FontEngine => 
            _fontEngine ?? throw new InvalidOperationException("CssUI has not been initialized. Call EngineProvider.Initialize() first.");

        /// <summary>
        /// Get the texture engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static ITextureEngine TextureEngine => 
            _textureEngine ?? throw new InvalidOperationException("CssUI has not been initialized. Call EngineProvider.Initialize() first.");

        /// <summary>
        /// Get the render engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static IRenderEngine RenderEngine => 
            _renderEngine ?? throw new InvalidOperationException("CssUI has not been initialized. Call EngineProvider.Initialize() first.");

        /// <summary>
        /// Returns true if CssUI has been initialized.
        /// </summary>
        public static bool IsInitialized => _initialized;
    }
}
