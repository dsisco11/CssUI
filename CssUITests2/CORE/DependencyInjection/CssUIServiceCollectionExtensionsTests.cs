using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CssUI;
using CssUI.DependencyInjection;

namespace CssUITests.CORE.DependencyInjection;

public class CssUIServiceCollectionExtensionsTests : IDisposable
{
    public CssUIServiceCollectionExtensionsTests()
    {
        // Reset EngineProvider before each test
        EngineProvider.Reset();
    }

    public void Dispose()
    {
        // Clean up after each test
        EngineProvider.Reset();
    }

    [Fact]
    public void AddCssUI_WithNoConfiguration_RegistersDefaultNullEngines()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        var textureEngine = serviceProvider.GetRequiredService<ITextureEngine>();
        var renderEngine = serviceProvider.GetRequiredService<IRenderEngine>();

        Assert.NotNull(fontEngine);
        Assert.NotNull(textureEngine);
        Assert.NotNull(renderEngine);
        Assert.IsType<NullFontEngine>(fontEngine);
        Assert.IsType<NullTextureEngine>(textureEngine);
        Assert.IsType<NullRenderEngine>(renderEngine);
    }

    [Fact]
    public void AddCssUI_WithConfiguration_UsesProvidedEngines()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontEngine = new NullFontEngine();
        var customTextureEngine = new NullTextureEngine();
        var customRenderEngine = new NullRenderEngine();

        // Act
        services.AddCssUI(options =>
        {
            options.FontEngine = customFontEngine;
            options.TextureEngine = customTextureEngine;
            options.RenderEngine = customRenderEngine;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        var textureEngine = serviceProvider.GetRequiredService<ITextureEngine>();
        var renderEngine = serviceProvider.GetRequiredService<IRenderEngine>();

        Assert.Same(customFontEngine, fontEngine);
        Assert.Same(customTextureEngine, textureEngine);
        Assert.Same(customRenderEngine, renderEngine);
    }

    [Fact]
    public void AddCssUI_RegistersICssUIEngineProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var engineProvider = serviceProvider.GetRequiredService<ICssUIEngineProvider>();
        Assert.NotNull(engineProvider);
        Assert.NotNull(engineProvider.FontEngine);
        Assert.NotNull(engineProvider.TextureEngine);
        Assert.NotNull(engineProvider.RenderEngine);
    }

    [Fact]
    public void AddFontEngine_WithType_ReplacesDefaultRegistration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFontEngine<NullFontEngine>();
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        Assert.IsType<NullFontEngine>(fontEngine);
    }

    [Fact]
    public void AddFontEngine_WithInstance_RegistersInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        var customEngine = new NullFontEngine();

        // Act
        services.AddFontEngine(customEngine);
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        Assert.Same(customEngine, fontEngine);
    }

    [Fact]
    public void AddFontEngine_WithFactory_UsesFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedEngine = new NullFontEngine();

        // Act
        services.AddFontEngine(_ => expectedEngine);
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        Assert.Same(expectedEngine, fontEngine);
    }

    [Fact]
    public void AddTextureEngine_WithInstance_RegistersInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        var customEngine = new NullTextureEngine();

        // Act
        services.AddTextureEngine(customEngine);
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var textureEngine = serviceProvider.GetRequiredService<ITextureEngine>();
        Assert.Same(customEngine, textureEngine);
    }

    [Fact]
    public void AddRenderEngine_WithInstance_RegistersInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        var customEngine = new NullRenderEngine();

        // Act
        services.AddRenderEngine(customEngine);
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var renderEngine = serviceProvider.GetRequiredService<IRenderEngine>();
        Assert.Same(customEngine, renderEngine);
    }

    [Fact]
    public void UseCssUI_InitializesStaticEngineProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontEngine = new NullFontEngine();

        services.AddCssUI(options => options.FontEngine = customFontEngine);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        serviceProvider.UseCssUI();

        // Assert
        Assert.True(EngineProvider.IsInitialized);
        Assert.Same(customFontEngine, EngineProvider.FontEngine);
    }

    [Fact]
    public void UseCssUI_BridgesAllEnginesToStaticProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontEngine = new NullFontEngine();
        var customTextureEngine = new NullTextureEngine();
        var customRenderEngine = new NullRenderEngine();

        services.AddCssUI(options =>
        {
            options.FontEngine = customFontEngine;
            options.TextureEngine = customTextureEngine;
            options.RenderEngine = customRenderEngine;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Act
        serviceProvider.UseCssUI();

        // Assert
        Assert.Same(customFontEngine, EngineProvider.FontEngine);
        Assert.Same(customTextureEngine, EngineProvider.TextureEngine);
        Assert.Same(customRenderEngine, EngineProvider.RenderEngine);
    }

    [Fact]
    public void EnginesAreSingletons_ReturnsSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCssUI();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var fontEngine1 = serviceProvider.GetRequiredService<IFontEngine>();
        var fontEngine2 = serviceProvider.GetRequiredService<IFontEngine>();
        var textureEngine1 = serviceProvider.GetRequiredService<ITextureEngine>();
        var textureEngine2 = serviceProvider.GetRequiredService<ITextureEngine>();
        var renderEngine1 = serviceProvider.GetRequiredService<IRenderEngine>();
        var renderEngine2 = serviceProvider.GetRequiredService<IRenderEngine>();

        // Assert
        Assert.Same(fontEngine1, fontEngine2);
        Assert.Same(textureEngine1, textureEngine2);
        Assert.Same(renderEngine1, renderEngine2);
    }

    [Fact]
    public void AddCssUI_PreRegisteredEngines_AreNotOverwritten()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontEngine = new NullFontEngine();

        // Pre-register custom engine
        services.AddSingleton<IFontEngine>(customFontEngine);

        // Act
        services.AddCssUI(); // Should NOT overwrite
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fontEngine = serviceProvider.GetRequiredService<IFontEngine>();
        Assert.Same(customFontEngine, fontEngine);
    }

    [Fact]
    public void AddCssUI_ThrowsWhenServicesIsNull()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services!.AddCssUI());
    }

    [Fact]
    public void AddCssUI_ThrowsWhenConfigureIsNull()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddCssUI(null!));
    }

    [Fact]
    public void UseCssUI_ThrowsWhenServiceProviderIsNull()
    {
        // Arrange
        IServiceProvider? serviceProvider = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => serviceProvider!.UseCssUI());
    }
}
