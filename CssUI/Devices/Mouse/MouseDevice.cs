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
        #endregion


        #region Accessors

        public double x => Position.x;
        public double y => Position.y;
        #endregion
    }
}
