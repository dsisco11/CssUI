using CssUI.CSS.Media;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents information about the screen of the output device
    /// This class needs to return values specific to the graphics context that it's window object is being rendered on.
    /// End users need to inherit from it and wrap their graphics context inside of their inherited class so it can return the correct values
    /// </summary>
    public abstract class Screen
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#the-screen-interface */

        #region Properties
        public abstract EMediaType MediaType { get; }
        public abstract long availWidth { get; }
        public abstract long availHeight { get; }
        public abstract long width { get;  }
        public abstract long height { get;  }

        /// <summary>
        /// The color depth of the current output devices screen
        /// (*Must* return 24 according to specifications)
        /// </summary>
        public ulong colorDepth { get; } = 24;

        /// <summary>
        /// The pixel depth of the current output devices screen
        /// (*Must* return 24 according to specifications)
        /// </summary>
        public ulong pixelDepth { get; } = 24;
        #endregion
    }
}
