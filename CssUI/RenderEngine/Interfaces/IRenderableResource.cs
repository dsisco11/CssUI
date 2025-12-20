namespace CssUI;

/// <summary>
/// Defines a contract for resources that can be rendered via a render service.
/// </summary>
public interface IRenderableResource
{
    /// <summary>
    /// Renders this resource using the specified render service.
    /// </summary>
    /// <param name="renderService">The render service to use for rendering.</param>
    void Render(IRenderService renderService);
}
