using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
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
        public cssColor Color;
        #endregion

        public cssVertex(cssColor Color, int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Color = Color;
        }
    }
}
