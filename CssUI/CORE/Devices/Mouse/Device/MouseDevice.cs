using CssUI.DOM.Geometry;
using System;

namespace CssUI.Devices
{
    /// <summary>
    /// Represents the systems mouse device
    /// </summary>
    [Obsolete("Use PointerDevice and pointer events instead!", true)]
    public abstract class MouseDevice
    {
        #region Properties
        public DOMPoint Position { get; protected set; }
        public EMouseButtonFlags Buttons { get; protected set; }
        #endregion


        #region Accessors
        public double X => Position.x;
        public double Y => Position.y;
        #endregion
    }
}
