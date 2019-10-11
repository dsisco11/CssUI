using System;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area of view.
    /// </summary>
    public class Viewport : EventTarget, IViewport
    {/* Docs: https://wicg.github.io/visual-viewport/#the-visualviewport-interface */
        #region Properties
        public Document document { get; private set; }

        public long Left;
        public long Top;

        public long Width;
        public long Height;

        internal ScrollBox ScrollBox { get; set; } = null;
        #endregion

        #region Events
        public event EventCallback onResize
        {
            add => handlerMap.Add(EEventName.Resize, value);
            remove => handlerMap.Remove(EEventName.Resize, value);
        }

        public event EventCallback onScroll
        {
            add => handlerMap.Add(EEventName.Scroll, value);
            remove => handlerMap.Remove(EEventName.Scroll, value);
        }
        #endregion
        
        #region Constructor
        public Viewport(Document document)
        {
            this.document = document;
            Left = 0;
            Top = 0;

            Width = 0;
            Height = 0;
        }
        #endregion

        public Rect4f getBoundingClientRect()
        {
            //return new DOMRect(Left, Top, Width, Height);
            return new Rect4f(Top, Left+Width, Top+Height, Left);
        }

    }
}
