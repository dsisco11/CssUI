using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CssUI.DependencyInjection;

/// <summary>
/// Extension methods for registering CssUI services with Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class CssUIServiceCollectionExtensions
{
    /// <summary>
    /// Adds CssUI core services to the service collection with default (null) engine implementations.
    /// Use this for headless/testing scenarios or when you plan to register custom engines separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCssUI(this IServiceCollection services)
    {
        return services.AddCssUI(_ => { });
    }

    /// <summary>
    /// Adds CssUI core services to the service collection with configurable engine implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure CssUI options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCssUI(this IServiceCollection services, Action<CssUIOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        // Configure options
        var options = new CssUIOptions();
        configure(options);

        // Register default null engines if not already registered
        // TryAdd ensures user-provided registrations take precedence
        services.TryAddSingleton<IFontEngine>(sp => options.FontEngine ?? new NullFontEngine());
        services.TryAddSingleton<ITextureEngine>(sp => options.TextureEngine ?? new NullTextureEngine());
        services.TryAddSingleton<IRenderEngine>(sp => options.RenderEngine ?? new NullRenderEngine());

        // Register the engine provider bridge for backward compatibility
        services.TryAddSingleton<ICssUIEngineProvider, CssUIEngineProvider>();

        return services;
    }

    /// <summary>
    /// Adds a custom font engine to the service collection.
    /// </summary>
    /// <typeparam name="TFontEngine">The font engine implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontEngine<TFontEngine>(this IServiceCollection services)
        where TFontEngine : class, IFontEngine
    {
        services.AddSingleton<IFontEngine, TFontEngine>();
        return services;
    }

    /// <summary>
    /// Adds a custom font engine instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="fontEngine">The font engine instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontEngine(this IServiceCollection services, IFontEngine fontEngine)
    {
        ArgumentNullException.ThrowIfNull(fontEngine);
        services.AddSingleton(fontEngine);
        return services;
    }

    /// <summary>
    /// Adds a custom font engine using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the font engine.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontEngine(this IServiceCollection services, Func<IServiceProvider, IFontEngine> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Adds a custom texture engine to the service collection.
    /// </summary>
    /// <typeparam name="TTextureEngine">The texture engine implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureEngine<TTextureEngine>(this IServiceCollection services)
        where TTextureEngine : class, ITextureEngine
    {
        services.AddSingleton<ITextureEngine, TTextureEngine>();
        return services;
    }

    /// <summary>
    /// Adds a custom texture engine instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="textureEngine">The texture engine instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureEngine(this IServiceCollection services, ITextureEngine textureEngine)
    {
        ArgumentNullException.ThrowIfNull(textureEngine);
        services.AddSingleton(textureEngine);
        return services;
    }

    /// <summary>
    /// Adds a custom texture engine using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the texture engine.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureEngine(this IServiceCollection services, Func<IServiceProvider, ITextureEngine> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Adds a custom render engine to the service collection.
    /// </summary>
    /// <typeparam name="TRenderEngine">The render engine implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderEngine<TRenderEngine>(this IServiceCollection services)
        where TRenderEngine : class, IRenderEngine
    {
        services.AddSingleton<IRenderEngine, TRenderEngine>();
        return services;
    }

    /// <summary>
    /// Adds a custom render engine instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="renderEngine">The render engine instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderEngine(this IServiceCollection services, IRenderEngine renderEngine)
    {
        ArgumentNullException.ThrowIfNull(renderEngine);
        services.AddSingleton(renderEngine);
        return services;
    }

    /// <summary>
    /// Adds a custom render engine using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the render engine.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderEngine(this IServiceCollection services, Func<IServiceProvider, IRenderEngine> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Initializes the static EngineProvider from DI container for backward compatibility.
    /// Call this after building the service provider if you need to use the static EngineProvider API.
    /// </summary>
    /// <param name="serviceProvider">The built service provider.</param>
    /// <returns>The service provider for chaining.</returns>
    public static IServiceProvider UseCssUI(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var engineProvider = serviceProvider.GetRequiredService<ICssUIEngineProvider>();
        EngineProvider.InitializeFromDI(engineProvider);

        return serviceProvider;
    }
}
