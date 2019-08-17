using System.Runtime.InteropServices;

namespace CssUI.CSS
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2i
    {
        #region Properties
        /// <summary>
        /// X axis position of the vertex
        /// </summary>
        public int X;
        /// <summary>
        /// Y axis position of the vertex
        /// </summary>
        public int Y;
        /// <summary>
        /// Color of the vertex
        /// </summary>
        public ReadOnlyColor Color;
        #endregion

        public Vertex2i(int X, int Y, ReadOnlyColor Color)
        {
            this.X = X;
            this.Y = Y;
            this.Color = Color;
        }
    }
}
