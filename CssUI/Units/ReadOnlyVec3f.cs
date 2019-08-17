using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlyVec3f
    {
        #region Properties
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        #endregion

        #region Constructor
        public ReadOnlyVec3f(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public ReadOnlyVec3f(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        #endregion


        public override string ToString()
        {
            return $"{GetType().Name}<{X}, {Y}, {Z}>";
        }

    }
}
