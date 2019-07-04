
namespace CssUI
{
    /// <summary>
    /// LayoutDirectors manage automatically positioning lists of controls within their containers
    /// </summary>
    public interface ILayoutDirector
    {
        cssBoxArea Handle(IParentElement parent, cssElement[] list);
    }
}
