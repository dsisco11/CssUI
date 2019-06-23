using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Specifys the format of pixel data for an <see cref="cssTexture"/> instance
    /// </summary>
    public enum EPixelFormat
    {
        /// <summary>Each pixel is comprised of 3 color components using 8-bits, making a pixel 24-bits.</summary>
        RGB,
        /// <summary>Each pixel is comprised of 4 color components using 8-bits, making a pixel 32-bits.</summary>
        RGBA,
    }
}
