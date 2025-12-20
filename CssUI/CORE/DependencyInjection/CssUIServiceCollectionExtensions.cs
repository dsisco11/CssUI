using System;
using CssUI.CSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CssUI.DependencyInjection;

/// <summary>
/// Extension methods for registering CssUI services with Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class CssUIServiceCollectionExtensions
{
    /// <summary>
    /// Adds CssUI core services to the service collection with default (null) service implementations.
    /// Use this for headless/testing scenarios or when you plan to register custom services separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCssUI(this IServiceCollection services)
    {
        return services.AddCssUI(_ => { });
    }

    /// <summary>
    /// Adds CssUI core services to the service collection with configurable service implementations.
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

        // Register default null services if not already registered
        // TryAdd ensures user-provided registrations take precedence
        services.TryAddSingleton<IFontService>(sp => options.FontService ?? new NullFontService());
        services.TryAddSingleton<ITextureService>(sp => options.TextureService ?? new NullTextureService());
        services.TryAddSingleton<IMeshService>(sp => options.MeshService ?? new NullMeshService());
        services.TryAddSingleton<IRenderService>(sp => options.RenderService ?? new NullRenderService());

        // Register internal services that depend on the above
        services.TryAddSingleton<ITextIntrinsicSizer, TextIntrinsicSizer>();

        return services;
    }

    /// <summary>
    /// Adds a custom font service to the service collection.
    /// </summary>
    /// <typeparam name="TFontService">The font service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontService<TFontService>(this IServiceCollection services)
        where TFontService : class, IFontService
    {
        services.AddSingleton<IFontService, TFontService>();
        return services;
    }

    /// <summary>
    /// Adds a custom font service instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="fontService">The font service instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontService(this IServiceCollection services, IFontService fontService)
    {
        ArgumentNullException.ThrowIfNull(fontService);
        services.AddSingleton(fontService);
        return services;
    }

    /// <summary>
    /// Adds a custom font service using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the font service.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFontService(this IServiceCollection services, Func<IServiceProvider, IFontService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Adds a custom texture service to the service collection.
    /// </summary>
    /// <typeparam name="TTextureService">The texture service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureService<TTextureService>(this IServiceCollection services)
        where TTextureService : class, ITextureService
    {
        services.AddSingleton<ITextureService, TTextureService>();
        return services;
    }

    /// <summary>
    /// Adds a custom texture service instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="textureService">The texture service instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureService(this IServiceCollection services, ITextureService textureService)
    {
        ArgumentNullException.ThrowIfNull(textureService);
        services.AddSingleton(textureService);
        return services;
    }

    /// <summary>
    /// Adds a custom texture service using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the texture service.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTextureService(this IServiceCollection services, Func<IServiceProvider, ITextureService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Adds a custom mesh service to the service collection.
    /// </summary>
    /// <typeparam name="TMeshService">The mesh service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMeshService<TMeshService>(this IServiceCollection services)
        where TMeshService : class, IMeshService
    {
        services.AddSingleton<IMeshService, TMeshService>();
        return services;
    }

    /// <summary>
    /// Adds a custom mesh service instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="meshService">The mesh service instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMeshService(this IServiceCollection services, IMeshService meshService)
    {
        ArgumentNullException.ThrowIfNull(meshService);
        services.AddSingleton(meshService);
        return services;
    }

    /// <summary>
    /// Adds a custom mesh service using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the mesh service.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMeshService(this IServiceCollection services, Func<IServiceProvider, IMeshService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Adds a custom render service to the service collection.
    /// </summary>
    /// <typeparam name="TRenderService">The render service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderService<TRenderService>(this IServiceCollection services)
        where TRenderService : class, IRenderService
    {
        services.AddSingleton<IRenderService, TRenderService>();
        return services;
    }

    /// <summary>
    /// Adds a custom render service instance to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="renderService">The render service instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderService(this IServiceCollection services, IRenderService renderService)
    {
        ArgumentNullException.ThrowIfNull(renderService);
        services.AddSingleton(renderService);
        return services;
    }

    /// <summary>
    /// Adds a custom render service using a factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the render service.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRenderService(this IServiceCollection services, Func<IServiceProvider, IRenderService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(factory);
        return services;
    }
}
