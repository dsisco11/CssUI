using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlyVec2i
    {
        #region Properties
        public readonly int X;
        public readonly int Y;
        #endregion

        #region Constructor
        public ReadOnlyVec2i(int x, int y)
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
