using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// Service that orchestrates tessellation of objects into mesh data.
/// </summary>
internal interface ITessellator
{
    /// <summary>
    /// Creates a new tessellation builder configured for the specified requirements.
    /// </summary>
    /// <param name="requirements">The vertex attribute requirements.</param>
    /// <param name="topology">The primitive topology. Defaults to <see cref="EPrimitiveTopology.TriangleList"/>.</param>
    /// <returns>A new tessellation builder.</returns>
    ITessellationBuilder CreateBuilder(TessellationRequirements requirements, EPrimitiveTopology topology = EPrimitiveTopology.TriangleList);

    /// <summary>
    /// Tessellates a single object into mesh data.
    /// </summary>
    /// <param name="source">The object to tessellate.</param>
    /// <returns>The tessellation result.</returns>
    TessellationResult Tessellate(ITessellatable source);

    /// <summary>
    /// Tessellates multiple objects into a single combined mesh.
    /// </summary>
    /// <param name="sources">The objects to tessellate.</param>
    /// <returns>The combined tessellation result.</returns>
    TessellationResult TessellateBatch(IEnumerable<ITessellatable> sources);

    /// <summary>
    /// Creates a GPU mesh from a tessellation result.
    /// </summary>
    /// <param name="result">The tessellation result to upload.</param>
    /// <param name="meshService">The mesh service to create the GPU mesh.</param>
    /// <returns>A handle to the created mesh, or <see cref="MeshHandle.Null"/> on failure.</returns>
    ValueTask<MeshHandle> CreateMesh(TessellationResult result, IMeshService meshService);
}
