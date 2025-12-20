namespace CssUI;

/// <summary>
/// Contract for objects that can be tessellated into mesh geometry.
/// </summary>
internal interface ITessellatable
{
    /// <summary>
    /// Gets the vertex attribute requirements for this object.
    /// </summary>
    TessellationRequirements Requirements { get; }

    /// <summary>
    /// Gets the tessellation version. Increment this when geometry changes
    /// to invalidate cached tessellation results.
    /// </summary>
    int TessellationVersion { get; }

    /// <summary>
    /// Tessellates this object by pushing vertices and indices to the builder.
    /// </summary>
    /// <param name="builder">The builder to receive tessellation data.</param>
    void Tessellate(ITessellationBuilder builder);
}
