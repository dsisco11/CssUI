using CssUI.CSS.Enums;

namespace CssUI.DOM.Internal
{
    public class ConvertCoordinateOptions
    {
        #region Properties
        public readonly ECssBoxType fromBox;
        public readonly ECssBoxType toBox;
        #endregion

        #region Constructor
        public ConvertCoordinateOptions(ECssBoxType fromBox = ECssBoxType.Border, ECssBoxType toBox = ECssBoxType.Border)
        {
            this.fromBox = fromBox;
            this.toBox = toBox;
        }
        #endregion
    }
}
