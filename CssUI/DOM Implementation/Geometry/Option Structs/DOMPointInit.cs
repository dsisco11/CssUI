namespace CssUI.DOM.Geometry
{
    public class DOMPointInit
    {
        #region Properties
        public readonly double x;
        public readonly double y;
        public readonly double z;
        public readonly double w;
        #endregion

        #region Constructors
        public DOMPointInit(double x = 0, double y = 0, double z = 0, double w = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        #endregion
    }
}
