
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// LayoutDirectors manage automatically positioning lists of controls within their containers
    /// </summary>
    public interface ILayoutDirector
    {
        CssBoxArea Handle(IParentElement parent, cssElement[] list);
    }
}
