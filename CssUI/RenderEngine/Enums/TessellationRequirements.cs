using System;

namespace CssUI;

/// <summary>
/// Flags indicating what vertex attributes a tessellatable object requires.
/// </summary>
[Flags]
internal enum TessellationRequirements
{
    /// <summary>
    /// No special requirements (position only).
    /// </summary>
    None = 0,

    /// <summary>
    /// Requires texture coordinates (UV).
    /// </summary>
    TexCoord = 1 << 0,

    /// <summary>
    /// Requires per-vertex color.
    /// </summary>
    Color = 1 << 1,

    /// <summary>
    /// Requires vertex normals (for 3D lighting).
    /// </summary>
    Normal = 1 << 2,
}
