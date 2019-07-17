using CssUI.DOM;

namespace CssUI.CSS.Media
{
    public interface IMediaCondition
    {/* https://www.w3.org/TR/mediaqueries-4/#media-condition */
        bool Matches(Document document);
    }
}
