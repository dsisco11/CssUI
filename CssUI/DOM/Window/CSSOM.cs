using CssUI.CSS;
using CssUI.CSS.Internal;
using CssUI.DOM.Geometry;
using CssUI.DOM.Media;
using System;

namespace CssUI.DOM
{
    public abstract partial class Window
    {

        #region CSS Object Model Extensions
        /* Docs: https://www.w3.org/TR/cssom-view-1/#extensions-to-the-window-interface */
        public MediaQueryList matchMedia(string query)
        {
            return new CSS.Serialization.CssParser(query).Parse_Media_Query_List(document);
        }

        // browsing context
        public void moveTo(long x, long y) => Set_Window_Location(new ReadOnlyPoint2i(x, y));
        public void moveBy(long x, long y) => Set_Window_Location(new Point2i(x, y) + Get_Window_Location());
        public void resizeTo(long x, long y) => Set_Window_Size(new ReadOnlyRect2i(x, y));
        public void resizeBy(long x, long y) => Set_Window_Size(new Rect2i(x, y) + Get_Window_Size());

        // Viewport
        public long innerWidth => (document.Viewport is null) ? 0 : document.Viewport.Width;
        public long innerHeight => (document.Viewport is null) ? 0 : document.Viewport.Height;

        // viewport scrolling
        public double scrollX
        {
            get
            {/* The scrollX attribute attribute must return the x-coordinate, relative to the initial containing block origin, of the left of the viewport, or zero if there is no viewport. */
                if (document.Viewport == null)
                    return 0;

                /* XXX: Are we doing scrolling wrong or something? why is it measuring the location of the viewport and not its scrollbox? */
                var icb = document.Initial_Containing_Block;
                return document.Viewport.Left - icb.Left;
            }
        }
        public double pageXOffset => scrollX;
        public double scrollY
        {
            get
            {/* The scrollY attribute attribute must return the y-coordinate, relative to the initial containing block origin, of the top of the viewport, or zero if there is no viewport. */
                if (document.Viewport == null)
                    return 0;

                /* XXX: Are we doing scrolling wrong or something? why is it measuring the location of the viewport and not its scrollbox? */
                var icb = document.Initial_Containing_Block;
                return document.Viewport.Top - icb.Top;
            }
        }
        public double pageYOffset => scrollY;

        internal void scroll_window(double x, double y, EScrollBehavior behavior)
        {
            double viewportWidth = document.Viewport.Width;
            double viewportHeight = document.Viewport.Height;

            var scrollBox = document.Viewport.ScrollBox;
            var scrollArea = scrollBox.ScrollArea;

            if (document.Viewport.ScrollBox.Overflow_Block == CSS.Enums.EOverflowDirection.Rightward)
            {
                x = MathExt.Max(0, MathExt.Min(x, scrollArea.Width - viewportWidth));
            }
            else
            {
                x = MathExt.Min(0, MathExt.Max(x, viewportWidth - scrollArea.Width));
            }

            if (document.Viewport.ScrollBox.Overflow_Inline == CSS.Enums.EOverflowDirection.Downward)
            {
                y = MathExt.Max(0, MathExt.Min(y, scrollArea.Height - viewportHeight));
            }
            else
            {
                y = MathExt.Min(0, MathExt.Max(y, viewportHeight - scrollArea.Height));
            }

            /* Let position be the scroll position the viewport would have by aligning the x-coordinate x of the viewport scrolling area with the left of the viewport and aligning the y-coordinate y of the viewport scrolling area with the top of the viewport. */
            double deltaX = x - document.Viewport.Left;
            double deltaY = y - document.Viewport.Top;

            DOMPoint position = new DOMPoint(scrollBox.ScrollX + deltaX, scrollBox.ScrollY + deltaY);
            if (position.Equals(scrollBox.ScrollX, scrollBox.ScrollY) && !scrollBox.IsScrolling)
                return;

            scrollBox.Perform_Scroll(position, document.documentElement, behavior);
        }

        public void Scroll(ScrollToOptions options) => ScrollTo(options);
        public void Scroll(double x, double y) => ScrollTo(x, y);

        public void ScrollTo(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scroll */
            if (document.Viewport == null)
                return;

            double x = options.left.HasValue ? options.left.Value : document.Viewport.ScrollBox.ScrollX;
            double y = options.top.HasValue ? options.top.Value : document.Viewport.ScrollBox.ScrollY;

            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);

            scroll_window(x, y, options.behavior);
        }
        public void ScrollTo(double x, double y)
        {
            if (document.Viewport == null)
                return;

            x = CssCommon.Normalize_Non_Finite(x);
            y = CssCommon.Normalize_Non_Finite(y);

            scroll_window(x, y, EScrollBehavior.Auto);
        }

        public void ScrollBy(ScrollToOptions options)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scrollby */
            options.left = !options.left.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.left.Value);
            options.top = !options.top.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.top.Value);

            options.left += scrollX;
            options.top += scrollY;

            Scroll(options);
        }
        public void ScrollBy(double x, double y)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-window-scrollby */
            var options = new ScrollToOptions(EScrollBehavior.Auto, x, y);

            options.left = !options.left.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.left.Value);
            options.top = !options.top.HasValue ? 0 : CssCommon.Normalize_Non_Finite(options.top.Value);

            options.left += scrollX;
            options.top += scrollY;

            Scroll(options);
        }

        // client

        /// <summary>
        /// The screenX attribute must return the x-coordinate, relative to the origin of the screen of the output device, of the left of the client window as number of pixels, or zero if there is no such thing.
        /// </summary>
        public long screenX => Get_Window_Location().X;
        /// <summary>
        /// The screenY attribute must return the y-coordinate, relative to the origin of the screen of the output device, of the top of the client window as number of pixels, or zero if there is no such thing.
        /// </summary>
        public long screenY => Get_Window_Location().Y;
        /// <summary>
        /// The outerWidth attribute must return the width of the client window. If there is no client window this attribute must return zero.
        /// </summary>
        public long outerWidth => (screen == null) ? 0 : Get_Window_Size().Width;
        /// <summary>
        /// The outerHeight attribute must return the height of the client window. If there is no client window this attribute must return zero.
        /// </summary>
        public long outerHeight => (screen == null) ? 0 : Get_Window_Size().Height;
        public double devicePixelRatio { get; }
        #endregion
    }
}
