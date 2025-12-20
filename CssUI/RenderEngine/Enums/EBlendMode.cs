namespace CssUI;

/// <summary>
/// Blend mode for rendering operations.
/// </summary>
public enum EBlendMode
{
    /// <summary>No blending, source overwrites destination.</summary>
    None,
    /// <summary>Standard alpha blending: src * srcAlpha + dst * (1 - srcAlpha).</summary>
    Alpha,
    /// <summary>Additive blending: src + dst.</summary>
    Additive,
    /// <summary>Multiplicative blending: src * dst.</summary>
    Multiply,
    /// <summary>Pre-multiplied alpha blending.</summary>
    PremultipliedAlpha
}
