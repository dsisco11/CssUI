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
        public SimpleColor(int r, int g, int b, int a)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }
        #endregion
    }
}
