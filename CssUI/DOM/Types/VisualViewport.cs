using System;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area of view.
    /// </summary>
    public class VisualViewport : EventTarget, IViewport
    {/* Docs: https://wicg.github.io/visual-viewport/#the-visualviewport-interface */
        #region Properties
        public readonly Window window;
        public Document document => window?.document;

        private long Left;
        private long Top;

        private long _width;
        private long _height;

        private double _scale;

        ScrollBox scrollBox = null;
        #endregion

        #region Events
        public event EventCallback onresize
        {
            add => handlerMap.Add(EEventName.Resize, value);
            remove => handlerMap.Remove(EEventName.Resize, value);
        }

        public event EventCallback onscroll
        {
            add => handlerMap.Add(EEventName.Scroll, value);
            remove => handlerMap.Remove(EEventName.Scroll, value);
        }
        #endregion

        #region Accessors
        public double OffsetLeft => !window.document.Is_FullyActive ? 0 : (Left - window.document.Viewport.Left);
        public double OffsetTop => !window.document.Is_FullyActive ? 0 : (Top - window.document.Viewport.Top);

        public double PageLeft => !window.document.Is_FullyActive ? 0 : (window.document.Initial_Containing_Block.Left - Left);
        public double PageTop => !window.document.Is_FullyActive ? 0 : (window.document.Initial_Containing_Block.Top - Top);

        public double Width => !window.document.Is_FullyActive ? 0 : (_width - (scrollBox?.VScrollBar?.Width ?? 0));
        public double Height => !window.document.Is_FullyActive ? 0 : (_height - (scrollBox?.HScrollBar?.Height ?? 0));

        /// <summary>
        /// Returns the pinch-zoom scaling factor applied to the visual viewport.
        /// </summary>
        public double Scale
        {/* Docs:  */
            get
            {
                /* XXX: Need to properly implement this */
                /*
                 * If the window's associated Document is not fully active, return 0 and abort these steps.
                 * If there is no output device, return 1 and abort these steps.
                 * Let CSS pixel size be the size of a CSS reference pixel at the current page zoom and pinch zoom scales.
                 * Let device pixel size be the size of a device pixel of the output device.
                 * Let device independent pixel size be the product of device pixel size and the devicePixelRatio
                 * Return the result of dividing the CSS pixel size by the device independent pixel size.
                 */

                if (!window.document.Is_FullyActive)
                {
                    return 0;
                }

                return 1.0;
            }
        }
        #endregion

        #region Constructor
        public VisualViewport(Window window)
        {
            this.window = window;
            Left = 0;
            Top = 0;

            _width = 0;
            _height = 0;

            _scale = 0;
        }
        #endregion


        public DOMRect getBoundingClientRect()
        {
            return new DOMRect(Left, Top, Width, Height);
        }
    }
}
