using CssUI.CSS.Serialization;
using CssUI.DOM;

namespace CssUI.CSS.Media
{
    public interface IMediaCondition : ICssSerializeable
    {/* https://www.w3.org/TR/mediaqueries-4/#media-condition */
        bool Matches(Document document);
    }
}
