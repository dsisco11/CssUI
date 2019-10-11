namespace CssUI.DOM.Geometry
{
    public class DOMQuadInit
    {
        #region Properties
        public readonly DOMPointInit p1;
        public readonly DOMPointInit p2;
        public readonly DOMPointInit p3;
        public readonly DOMPointInit p4;
        #endregion

        public DOMQuadInit(DOMPointInit p1, DOMPointInit p2, DOMPointInit p3, DOMPointInit p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }
    }
}
