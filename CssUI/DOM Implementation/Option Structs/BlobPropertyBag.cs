using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class BlobPropertyBag
    {
        public string type;
        public EBlobEnding endings;

        public BlobPropertyBag(string type = "", EBlobEnding endings = EBlobEnding.Transparent)
        {
            this.type = type;
            this.endings = endings;
        }
    }
}
