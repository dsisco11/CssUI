namespace CssUI
{
    public struct SimpleColor
    {
        #region Properties
        public byte R, G, B, A;
        #endregion

        #region Constructors
        public SimpleColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        #endregion
    }
}
