using System.Runtime.InteropServices;

namespace CssUI

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlySize2i
    {
        #region Properties
        public readonly int Width;
        public readonly int Height;
        #endregion

        #region Constructor
        public ReadOnlySize2i(int width, int height)
        {
            Width = width;
            Height = height;
        }
        #endregion


        public override string ToString()
        {
            return $"{GetType().Name}<{Width}, {Height}>";
        }

    }
}
