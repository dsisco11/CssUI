using CssUI.Rendering;

namespace CssUI.CSS
{
    public struct cssVertex
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
        public Color Color;
        #endregion

        public cssVertex(Color Color, int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Color = Color;
        }
    }
}
