using System;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area of view.
    /// </summary>
    public class VisualViewport : EventTarget
    {/* Docs: https://wicg.github.io/visual-viewport/#the-visualviewport-interface */
        #region Properties
        public readonly Window window;

        private double _offset_left;
        private double _offset_top;

        private double _page_left;
        private double _page_top;

        private double _width;
        private double _height;

        private double _scale;
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
        public double OffsetLeft
        {
            get => !window.document.is_fully_active ? 0 : ();
        }
        public double OffsetTop;

        public double PageLeft;
        public double PageTop;

        public double Width;
        public double Height;

        public double Scale;
        #endregion

        #region Constructor
        public VisualViewport(Window window)
        {
            this.window = window;
            _offset_left = 0;
            _offset_top = 0;

            _page_left = 0;
            _page_top = 0;

            _width = 0;
            _height = 0;

            _scale = 0;
        }
        #endregion

    }
}
