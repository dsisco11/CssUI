using System;

namespace CssUI.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="ICssUIEngineProvider"/> that retrieves engines from DI container.
/// </summary>
internal sealed class CssUIEngineProvider : ICssUIEngineProvider
{
    private readonly IFontEngine _fontEngine;
    private readonly ITextureEngine _textureEngine;
    private readonly IRenderEngine _renderEngine;

    public CssUIEngineProvider(IFontEngine fontEngine, ITextureEngine textureEngine, IRenderEngine renderEngine)
    {
        _fontEngine = fontEngine ?? throw new ArgumentNullException(nameof(fontEngine));
        _textureEngine = textureEngine ?? throw new ArgumentNullException(nameof(textureEngine));
        _renderEngine = renderEngine ?? throw new ArgumentNullException(nameof(renderEngine));
    }

    public IFontEngine FontEngine => _fontEngine;
    public ITextureEngine TextureEngine => _textureEngine;
    public IRenderEngine RenderEngine => _renderEngine;
}
