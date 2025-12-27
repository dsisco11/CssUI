using System;
using CssUI.DOM.Nodes;
using CssUI.Rendering;

namespace CssUI.CSS;

public class ColorProperty : CssProperty
{
    #region Value Overrides
    /// <summary>
    /// Returns the actual computed color value as a <see cref="CssColor"/>.
    /// </summary>
    public new CssColor Actual => base.Actual.AsCssColor();

    /// <summary>
    /// Returns the actual computed color as a rendering-layer <see cref="Rgba"/>.
    /// </summary>
    public Rgba ActualRenderColor => Actual.ToRenderColor();
    #endregion

    #region Constructors
    public ColorProperty(ECssPropertyID CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
        : base(CssName, Owner, Source, Locked)
    {
    }
    #endregion

    #region Setters
    /// <summary>
    /// Sets the <see cref="Assigned"/> value for this property to the given color
    /// </summary>
    /// <param name="value"></param>
    public void Set(Color value)
    {
        var newValue = CssValue.From(value);
        if (Assigned != newValue)
        {
            Assigned = newValue;
        }
    }
    #endregion
}

