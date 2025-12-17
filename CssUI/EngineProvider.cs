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
        /// If null, a default engine will be selected based on build configuration.
        /// </summary>
        public IFontEngine? FontEngine { get; init; }

        /// <summary>
        /// Engine for image decoding and texture management.
        /// If null, a default engine will be selected based on build configuration.
        /// </summary>
        public ITextureEngine? TextureEngine { get; init; }

        /// <summary>
        /// Engine for rendering primitives, textures, and text.
        /// If null, a NullRenderEngine will be used.
        /// </summary>
        public IRenderEngine? RenderEngine { get; init; }
    }

    /// <summary>
    /// Global access to engine instances after initialization.
    /// </summary>
    public static class EngineProvider
    {
        private static IFontEngine? _fontEngine;
        private static ITextureEngine? _textureEngine;
        private static IRenderEngine? _renderEngine;
        private static bool _initialized;
        private static readonly object _lock = new();

        /// <summary>
        /// Initialize CssUI with the provided engine implementations.
        /// Must be called before using any CssUI functionality.
        /// </summary>
        /// <param name="config">Configuration with engine implementations. Pass null for all defaults.</param>
        /// <exception cref="InvalidOperationException">If already initialized.</exception>
        public static void Initialize(CssUIConfig? config = null)
        {
            lock (_lock)
            {
                if (_initialized)
                    throw new InvalidOperationException("CssUI has already been initialized.");

                config ??= new CssUIConfig();
                
                _fontEngine = config.FontEngine ?? CreateDefaultFontEngine();
                _textureEngine = config.TextureEngine ?? CreateDefaultTextureEngine();
                _renderEngine = config.RenderEngine ?? new NullRenderEngine();
                _initialized = true;
            }
        }

        /// <summary>
        /// Ensure CssUI is initialized, using defaults if not already initialized.
        /// Safe to call multiple times.
        /// </summary>
        public static void EnsureInitialized()
        {
            if (_initialized) return;
            
            lock (_lock)
            {
                if (_initialized) return;
                Initialize();
            }
        }

        /// <summary>
        /// Reset the engine provider (mainly for testing).
        /// </summary>
        internal static void Reset()
        {
            lock (_lock)
            {
                _fontEngine = null;
                _textureEngine = null;
                _renderEngine = null;
                _initialized = false;
            }
        }

        private static IFontEngine CreateDefaultFontEngine()
        {
#if DISABLE_FONT_SYSTEM || ENABLE_HEADLESS
            return new NullFontEngine();
#else
            return new CssUI.Fonts.SixLaborsFontEngine();
#endif
        }

        private static ITextureEngine CreateDefaultTextureEngine()
        {
#if ENABLE_HEADLESS
            return new NullTextureEngine();
#else
            return new CssUI.Rendering.SixLaborsTextureEngine();
#endif
        }

        /// <summary>
        /// Get the font engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static IFontEngine FontEngine
        {
            get
            {
                EnsureInitialized();
                return _fontEngine!;
            }
        }

        /// <summary>
        /// Get the texture engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static ITextureEngine TextureEngine
        {
            get
            {
                EnsureInitialized();
                return _textureEngine!;
            }
        }

        /// <summary>
        /// Get the render engine instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">If not initialized.</exception>
        public static IRenderEngine RenderEngine
        {
            get
            {
                EnsureInitialized();
                return _renderEngine!;
            }
        }

        /// <summary>
        /// Returns true if CssUI has been initialized.
        /// </summary>
        public static bool IsInitialized => _initialized;
    }
}
