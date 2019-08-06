using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Represents an item whose content must be rendered to a texture before being displayed by the OpenGL UI system
    /// </summary>
    public interface IGenericGraphic
    {
        EReplacedElementType ImageType { get; }
        //glTexture Image { get; set; }
        void Set_Svg(string svg_markup);
        void Set_Svg(Stream stream);
        void Clear_Svg();
    }
}
