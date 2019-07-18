﻿using System;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area of view.
    /// </summary>
    public class Viewport : EventTarget
    {/* Docs: https://wicg.github.io/visual-viewport/#the-visualviewport-interface */
        #region Properties
        public readonly Document document;

        public long Left;
        public long Top;

        public long Width;
        public long Height;

        public DOMRect ScrollingBox = null;
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

        public DOMRect Get_Bounds()
        {
            return new DOMRect(Left, Top, Width, Height);
        }

    }
}
