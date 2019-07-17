using CssUI.CSS.Enums;
using CssUI.DOM.Interfaces;

namespace CssUI.DOM.Internal
{
    public class BoxQuadOptions
    {
        #region Properties
        public readonly ECssBoxType box;
        public readonly IGeometryNode relativeTo; // XXX default document (i.e. viewport)
        #endregion

        #region Constructor
        public BoxQuadOptions(IGeometryNode relativeTo, ECssBoxType box = ECssBoxType.Border)
        {
            this.box = box;
            this.relativeTo = relativeTo;
        }
        #endregion
    }
}
