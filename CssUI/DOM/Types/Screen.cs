﻿using CssUI.CSS.Media;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents information about the screen of the output device
    /// This class needs to return values specific to the graphics context that it's window object is being rendered on.
    /// End users need to inherit from it and wrap their graphics context inside of their inherited class so it can return the correct values
    /// </summary>
    public abstract class Screen : EventTarget
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#the-screen-interface */

        #region Properties
        /// <summary>
        /// Describes the type of medium the document is being displayed on.
        /// </summary>
        public abstract EMediaType MediaType { get; }

        /// <summary>
        /// Available width of the rendering surface of the output device, in CSS pixels.
        /// <para>This would be the screen / monitor size</para>
        /// </summary>
        public abstract long availWidth { get; }
        /// <summary>
        /// Available height of the rendering surface of the output device, in CSS pixels.
        /// <para>This would be the screen / monitor size</para>
        /// </summary>
        public abstract long availHeight { get; }

        /// <summary>
        /// The width of the output device, in CSS pixels.
        /// </summary>
        public abstract long width { get; }
        /// <summary>
        /// The height of the output device, in CSS pixels.
        /// </summary>
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

        /// <summary>
        /// Return "1" if the current output devices screen is grid based (such as a tty terminal or an lcd phone based display with only one font)
        /// </summary>
        public ulong isGrid { get; } = 0;

        /// <summary>
        /// The bits per pixel in the monochrome framebuffer of the current output device or 0 if the device is not a monochrome device
        /// </summary>
        public ulong monochromeDepth { get; } = 0;

        /// <summary>
        /// The color-index describes the number of entries in the color lookup table of the output device. 
        /// If the device does not use a color lookup table, the value is zero.
        /// </summary>
        public ulong colorIndex { get; } = 0;

        /// <summary>
        /// The current output devices refresh rate
        /// </summary>
        public double refreshRate { get; } = 0.0;
        #endregion

        #region CssUI
        /// <summary>
        /// The dpi (dots per inch) of the current output devices screen
        /// </summary>
        public ulong dpi { get; } = 96;
        #endregion
    }
}
