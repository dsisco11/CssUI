using System;
using CssUI;
using CssUI.CSS;
using CssUI.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CssUITests.CORE.DependencyInjection;

public class CssUIServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCssUI_RegistersDefaultNullServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert
        var fontService = provider.GetRequiredService<IFontService>();
        var textureService = provider.GetRequiredService<ITextureService>();
        var renderService = provider.GetRequiredService<IRenderService>();

        Assert.NotNull(fontService);
        Assert.NotNull(textureService);
        Assert.NotNull(renderService);
        Assert.IsType<NullFontService>(fontService);
        Assert.IsType<NullTextureService>(textureService);
        Assert.IsType<NullRenderService>(renderService);
    }

    [Fact]
    public void AddCssUI_WithOptions_RegistersConfiguredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontService = new NullFontService();
        var customTextureService = new NullTextureService();
        var customRenderService = new NullRenderService();

        // Act
        services.AddCssUI(options =>
        {
            options.FontService = customFontService;
            options.TextureService = customTextureService;
            options.RenderService = customRenderService;
        });
        var provider = services.BuildServiceProvider();

        // Assert
        var fontService = provider.GetRequiredService<IFontService>();
        var textureService = provider.GetRequiredService<ITextureService>();
        var renderService = provider.GetRequiredService<IRenderService>();

        Assert.Same(customFontService, fontService);
        Assert.Same(customTextureService, textureService);
        Assert.Same(customRenderService, renderService);
    }

    [Fact]
    public void AddCssUI_RegistersMeshService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert
        var meshService = provider.GetRequiredService<IMeshService>();
        Assert.NotNull(meshService);
        Assert.IsType<NullMeshService>(meshService);
    }

    [Fact]
    public void AddMeshService_Generic_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCssUI();

        // Act
        services.AddMeshService<NullMeshService>();
        var provider = services.BuildServiceProvider();

        // Assert
        var meshService = provider.GetRequiredService<IMeshService>();
        Assert.IsType<NullMeshService>(meshService);
    }

    [Fact]
    public void AddCssUI_RegistersTextIntrinsicSizer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert
        var textSizer = provider.GetRequiredService<ITextIntrinsicSizer>();
        Assert.NotNull(textSizer);
        Assert.IsType<TextIntrinsicSizer>(textSizer);
    }

    [Fact]
    public void AddFontService_Generic_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCssUI();

        // Act
        services.AddFontService<NullFontService>();
        var provider = services.BuildServiceProvider();

        // Assert
        var fontService = provider.GetRequiredService<IFontService>();
        Assert.IsType<NullFontService>(fontService);
    }

    [Fact]
    public void AddFontService_Instance_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        var customInstance = new NullFontService();

        // Act
        services.AddFontService(customInstance);
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert
        var fontService = provider.GetRequiredService<IFontService>();
        Assert.Same(customInstance, fontService);
    }

    [Fact]
    public void AddFontService_Factory_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        var customInstance = new NullFontService();

        // Act
        services.AddFontService(_ => customInstance);
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert
        var fontService = provider.GetRequiredService<IFontService>();
        Assert.Same(customInstance, fontService);
    }

    [Fact]
    public void AddTextureService_Generic_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCssUI();

        // Act
        services.AddTextureService<NullTextureService>();
        var provider = services.BuildServiceProvider();

        // Assert
        var textureService = provider.GetRequiredService<ITextureService>();
        Assert.IsType<NullTextureService>(textureService);
    }

    [Fact]
    public void AddRenderService_Generic_RegistersService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCssUI();

        // Act
        services.AddRenderService<NullRenderService>();
        var provider = services.BuildServiceProvider();

        // Assert
        var renderService = provider.GetRequiredService<IRenderService>();
        Assert.IsType<NullRenderService>(renderService);
    }

    [Fact]
    public void AddCssUI_ServicesAreSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert - same instance returned on multiple resolutions
        var fontService1 = provider.GetRequiredService<IFontService>();
        var fontService2 = provider.GetRequiredService<IFontService>();
        Assert.Same(fontService1, fontService2);

        var textureService1 = provider.GetRequiredService<ITextureService>();
        var textureService2 = provider.GetRequiredService<ITextureService>();
        Assert.Same(textureService1, textureService2);

        var renderService1 = provider.GetRequiredService<IRenderService>();
        var renderService2 = provider.GetRequiredService<IRenderService>();
        Assert.Same(renderService1, renderService2);
    }

    [Fact]
    public void AddCssUI_UserRegistrationTakesPrecedence()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontService = new NullFontService();

        // Act - register custom service before AddCssUI
        services.AddSingleton<IFontService>(customFontService);
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Assert - custom service is used, not the default
        var fontService = provider.GetRequiredService<IFontService>();
        Assert.Same(customFontService, fontService);
    }

    [Fact]
    public void AddCssUI_ThrowsOnNullServices()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            CssUIServiceCollectionExtensions.AddCssUI(null!));

        Assert.Throws<ArgumentNullException>(() =>
            CssUIServiceCollectionExtensions.AddCssUI(null!, _ => { }));

        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.AddCssUI(null!));
    }

    [Fact]
    public void TextIntrinsicSizer_ReceivesFontService()
    {
        // Arrange
        var services = new ServiceCollection();
        var customFontService = new NullFontService();
        services.AddFontService(customFontService);
        services.AddCssUI();
        var provider = services.BuildServiceProvider();

        // Act
        var textSizer = provider.GetRequiredService<ITextIntrinsicSizer>();

        // Assert - the sizer was created (which means IFontService was injected)
        Assert.NotNull(textSizer);
    }
}
