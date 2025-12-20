using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// Default implementation of <see cref="ITessellator"/> that orchestrates
/// tessellation of objects into mesh data.
/// </summary>
internal sealed class Tessellator : ITessellator
{
    /// <inheritdoc/>
    public ITessellationBuilder CreateBuilder(TessellationRequirements requirements, EPrimitiveTopology topology = EPrimitiveTopology.TriangleList)
    {
        return new TessellationBuilder(requirements, topology);
    }

    /// <inheritdoc/>
    public TessellationResult Tessellate(ITessellatable source)
    {
        var builder = CreateBuilder(source.Requirements);
        source.Tessellate(builder);
        return builder.Build();
    }

    /// <inheritdoc/>
    public TessellationResult TessellateBatch(IEnumerable<ITessellatable> sources)
    {
        var sourceList = sources as IList<ITessellatable> ?? sources.ToList();

        if (sourceList.Count == 0)
        {
            return TessellationResult.Empty;
        }

        // Combine requirements from all sources
        var combinedRequirements = TessellationRequirements.None;
        foreach (var source in sourceList)
        {
            combinedRequirements |= source.Requirements;
        }

        var builder = CreateBuilder(combinedRequirements);

        // Tessellate each source into the shared builder
        foreach (var source in sourceList)
        {
            source.Tessellate(builder);
        }

        return builder.Build();
    }

    /// <inheritdoc/>
    public async ValueTask<MeshHandle> CreateMesh(TessellationResult result, IMeshService meshService)
    {
        if (result.IsEmpty)
        {
            return MeshHandle.Null;
        }

        if (result.IsIndexed)
        {
            return await meshService.CreateMesh(
                result.Vertices,
                result.Layout,
                result.Indices,
                result.IndexFormat,
                result.Topology);
        }
        else
        {
            return await meshService.CreateMesh(
                result.Vertices,
                result.Layout,
                result.Topology);
        }
    }
}
