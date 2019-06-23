using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public interface IImageElement
    {
        /// <summary>
        /// Sets the current image texture
        /// </summary>
        void Set_Image(cssTexture Tex);
        /// <summary>
        /// Size of the current image
        /// </summary>
        eSize Image_Size { get; }
    }
}
