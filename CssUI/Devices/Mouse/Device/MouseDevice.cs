using CssUI.DOM.Geometry;

namespace CssUI.Devices
{
    /// <summary>
    /// Represents the systems mouse device
    /// </summary>
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
