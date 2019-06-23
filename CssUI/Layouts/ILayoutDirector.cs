using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// LayoutDirectors manage automatically positioning lists of controls within their containers
    /// </summary>
    public interface ILayoutDirector
    {
        eBlock Handle(ICompoundElement parent, uiElement[] list);
    }
}
