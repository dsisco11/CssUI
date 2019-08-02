using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SimpleColor
    {
        #region Instances
        public readonly static SimpleColor MinValue = new SimpleColor(byte.MinValue, byte.MinValue, byte.MinValue);
        public readonly static SimpleColor MaxValue = new SimpleColor(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        #endregion

        #region Properties
        public byte R, G, B;
        #endregion

        #region Constructors
        public SimpleColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public SimpleColor(int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }
        #endregion
    }
}
