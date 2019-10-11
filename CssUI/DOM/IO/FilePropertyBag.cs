using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class FilePropertyBag : BlobPropertyBag
    {
        public ulong lastModified = ulong.MaxValue;

        public FilePropertyBag() : base()
        {
        }

        public FilePropertyBag(long lastModified, string type = "", EBlobEnding endings = EBlobEnding.Transparent) : base(type, endings)
        {
        }
    }
}
