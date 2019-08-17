using System.Runtime.InteropServices;

namespace CssUI.CSS
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CssRect
    {
        #region Properties
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        #endregion

        #region Constructors
        public CssRect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        #endregion
    }
}
