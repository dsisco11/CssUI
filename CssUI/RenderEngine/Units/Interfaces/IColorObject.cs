using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace CssUI.Rendering;

public interface IColorObject
{
    /// <summary>
    /// Returns 4 RGBA values in the range [0.0 - 1.0]
    /// </summary>
    Vector4 GetVector();

    /// <summary>
    /// Scales up the given RGBA values from [0.0 - 1.0] to [0 - 255] and then assigns those values to the color.
    /// </summary>
    void SetVector(Vector4 RGBA);

    /// <summary>
    /// Interprets a set of RGBA values as a 32-bit integer.
    /// </summary>
    uint AsInteger();
}


public static class IColorObject_Extensions
{
    #region Scaling
    /// <summary>
    /// Scales this color instances RGBA values by the given factor
    /// </summary>
    static public IColorObject Scale(this IColorObject color, float rgbaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4(rgbaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances RGBA values by the given factor
    /// </summary>
    public static IColorObject Scale(this IColorObject color, double rgbaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4((float)rgbaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances RGBA values by the given factors
    /// </summary>
    public static IColorObject Scale(this IColorObject color, float redFactor, float greenFactor, float blueFactor, float alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4(redFactor, greenFactor, blueFactor, alphaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances RGBA values by the given factors
    /// </summary>
    public static IColorObject Scale(this IColorObject color, double redFactor, double greenFactor, double blueFactor, double alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4((float)redFactor, (float)greenFactor, (float)blueFactor, (float)alphaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances RGBA values by the given factors
    /// </summary>
    /// <returns></returns>
    public static IColorObject Scale(this IColorObject color, float rgbFactor, float alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4(rgbFactor, rgbFactor, rgbFactor, alphaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances RGBA values by the given factors
    /// </summary>
    /// <returns></returns>
    public static IColorObject Scale(this IColorObject color, double rgbFactor, double alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        Vector4 scale = new Vector4((float)rgbFactor, (float)rgbFactor, (float)rgbFactor, (float)alphaFactor);
        color.SetVector(color.GetVector() * scale);
        return color;
    }

    /// <summary>
    /// Scales this color instances alpha value by the given factor
    /// </summary>
    public static IColorObject ScaleAlpha(this IColorObject color, float alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        var Scalar = color.GetVector();
        Scalar.W *= alphaFactor;
        color.SetVector(Scalar);
        return color;
    }

    /// <summary>
    /// Scales this color instances alpha value by the given factor
    /// </summary>
    public static IColorObject ScaleAlpha(this IColorObject color, double alphaFactor)
    {
        ArgumentNullException.ThrowIfNull(color);
        Contract.EndContractBlock();

        var Scalar = color.GetVector();
        Scalar.W *= (float)alphaFactor;
        color.SetVector(Scalar);
        return color;
    }
    #endregion
}
