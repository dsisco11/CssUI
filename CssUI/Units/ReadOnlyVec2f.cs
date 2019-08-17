using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlyVec2f
    {
        #region Properties
        public readonly double X;
        public readonly double Y;
        #endregion

        #region Constructor
        public ReadOnlyVec2f(double x, double y)
        {
            X = x;
            Y = y;
        }
        public ReadOnlyVec2f(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion


        public override string ToString()
        {
            return $"{GetType().Name}<{X}, {Y}>";
        }

    }
}
