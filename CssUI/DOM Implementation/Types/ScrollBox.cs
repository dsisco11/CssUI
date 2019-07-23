using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM.Enums;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    public class ScrollBox
    {
        #region Property
        public EFlowDirection Block;
        public EFlowDirection Inline;
        public readonly Element Owner = null;
        public readonly IViewport View = null;

        /// <summary>
        /// Current x-coordinate offset position of this scrollbox
        /// </summary>
        public double ScrollX { get; private set; }

        /// <summary>
        /// Current y-coordinate offset position of this scrollbox
        /// </summary>
        public double ScrollY { get; private set; }

        /// <summary>
        /// If true then the scrollbox has an ongoing smooth scroll operation
        /// </summary>
        public bool IsScrolling { get; private set; } = false;

        /// <summary>
        /// The vertical scroll bar bounds
        /// </summary>
        public DOMRectReadOnly VScrollBar { get; private set; } = null;
        /// <summary>
        /// The horizontal scroll bar bounds
        /// </summary>
        public DOMRectReadOnly HScrollBar { get; private set; } = null;
/*
        /// <summary>
        /// Determines along which edge of this box the (horizontal) scrollbar will be rendered on.
        /// <para><see cref="EDirection.LTR"/> = Bottom</para>
        /// <para><see cref="EDirection.RTL"/> = Top</para>
        /// </summary>
        public EDirection HScrollbarSide = EDirection.LTR;

        /// <summary>
        /// Determines along which edge of this box the (vertical) scrollbar will be rendered on.
        /// <para><see cref="EDirection.LTR"/> = Right</para>
        /// <para><see cref="EDirection.RTL"/> = Left</para>
        /// </summary>
        public EDirection VScrollbarSide = EDirection.LTR;
*/
        #endregion

        #region Accessors
        /// <summary>
        /// Horizontal overflow direction
        /// </summary>
        public EOverflowDirection Overflow_Block
        {
            get
            {
                switch (Block)
                {
                    case EFlowDirection.TTB:
                        {
                            switch (Inline)
                            {
                                case EFlowDirection.LTR:
                                    {
                                        return EOverflowDirection.Rightward;
                                    }
                                case EFlowDirection.RTL:
                                default:
                                    {
                                        return EOverflowDirection.Leftward;
                                    }
                            }
                        }
                    case EFlowDirection.RTL:
                        {
                            return EOverflowDirection.Leftward;
                        }
                    case EFlowDirection.LTR:
                    default:
                        {
                            return EOverflowDirection.Rightward;
                        }
                }
            }
        }

        /// <summary>
        /// Vertical overflow direction
        /// </summary>
        public EOverflowDirection Overflow_Inline
        {
            get
            {
                switch (Inline)
                {
                    case EFlowDirection.LTR:
                        {
                            return EOverflowDirection.Downward;
                        }
                    case EFlowDirection.RTL:
                    default:
                        {
                            switch (Block)
                            {
                                case EFlowDirection.TTB:
                                    {
                                        return EOverflowDirection.Downward;
                                    }
                                case EFlowDirection.LTR:
                                case EFlowDirection.RTL:
                                default:
                                    {
                                        return EOverflowDirection.Upward;
                                    }
                            }
                        }
                }
            }
        }

        public DOMPoint Origin
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#scrolling-area-origin */
            get
            {
                if (ReferenceEquals(null, Owner))
                {/* Viewport */
                    var icb = View.document.Initial_Containing_Block;
                    return new DOMPoint(icb.left, icb.top);
                }
                else
                {/* Element */
                    return new DOMPoint(Owner.Box.Padding.Left, Owner.Box.Padding.Top);
                }
            }
        }

        public DOMRect ScrollArea
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#scrolling-area */
            get
            {
                int CASE = 0;
                if ((Block == EFlowDirection.TTB && Inline == EFlowDirection.LTR) || (Block == EFlowDirection.LTR && Inline == EFlowDirection.LTR))
                {/* Rightward & Downward */
                    CASE = 0;
                }
                else if ((Block == EFlowDirection.TTB && Inline == EFlowDirection.RTL) || (Block == EFlowDirection.RTL && Inline == EFlowDirection.LTR))
                {/* Leftward & Downward */
                    CASE = 1;
                }
                else if ((Block == EFlowDirection.RTL && Inline == EFlowDirection.RTL))
                {/* Leftward & Upward */
                    CASE = 2;
                }
                else if ((Block == EFlowDirection.LTR && Inline == EFlowDirection.RTL))
                {/* Rightward & Upward */
                    CASE = 3;
                }


                if (ReferenceEquals(null, Owner))
                {/* Viewport */
                    double top = 0, right = 0, bottom = 0, left = 0;
                    DOMRect icb = View.document.Initial_Containing_Block;

                    top = icb.top;
                    right = icb.right;
                    bottom = icb.bottom;
                    left = icb.left;

                    var tree = new TreeWalker(View.document.documentElement, Enums.ENodeFilterMask.SHOW_ELEMENT);
                    Node node = tree.nextNode();
                    while (!ReferenceEquals(null, node))
                    {
                        if (node is Element E)
                        {
                            switch (CASE)
                            {
                                case 0:/* Rightward & Downward */
                                    {
                                        right = MathExt.Max(right, E.Box.Margin.Right);
                                        bottom = MathExt.Max(bottom, E.Box.Margin.Bottom);
                                    }
                                    break;
                                case 1:/* Leftward & Downward */
                                    {
                                        left = MathExt.Max(right, E.Box.Margin.Left);
                                        bottom = MathExt.Max(bottom, E.Box.Margin.Bottom);
                                    }
                                    break;
                                case 2:/* Leftward & Upward */
                                    {
                                        left = MathExt.Max(right, E.Box.Margin.Left);
                                        top = MathExt.Max(bottom, E.Box.Margin.Top);
                                    }
                                    break;
                                case 3:/* Rightward & Upward */
                                    {
                                        right = MathExt.Max(right, E.Box.Margin.Right);
                                        top = MathExt.Max(bottom, E.Box.Margin.Top);
                                    }
                                    break;
                            }

                        }

                        node = tree.nextNode();
                    }

                    return new DOMRect(top, left, (right - left), (bottom - top));
                }
                else
                {/* Element */
                    double top = 0, right = 0, bottom = 0, left = 0;

                    top = Owner.Box.Padding.Top;
                    left = Owner.Box.Padding.Left;
                    bottom = Owner.Box.Padding.Bottom;
                    left = Owner.Box.Padding.Left;

                    var tree = new TreeWalker(Owner, Enums.ENodeFilterMask.SHOW_ELEMENT);
                    Node node = tree.nextNode();
                    while (!ReferenceEquals(null, node))
                    {
                        if (node is Element E)
                        {
                            if (CssCommon.Is_Containing_Block_Ancestor_Of(E, Owner))
                                continue;

                            switch (CASE)
                            {
                                case 0:/* Rightward & Downward */
                                    {
                                        right = MathExt.Max(right, E.Box.Padding.Right);
                                        bottom = MathExt.Max(bottom, E.Box.Padding.Bottom);
                                    }
                                    break;
                                case 1:/* Leftward & Downward */
                                    {
                                        left = MathExt.Max(right, E.Box.Padding.Left);
                                        bottom = MathExt.Max(bottom, E.Box.Padding.Bottom);
                                    }
                                    break;
                                case 2:/* Leftward & Upward */
                                    {
                                        left = MathExt.Max(right, E.Box.Padding.Left);
                                        top = MathExt.Max(bottom, E.Box.Padding.Top);
                                    }
                                    break;
                                case 3:/* Rightward & Upward */
                                    {
                                        right = MathExt.Max(right, E.Box.Padding.Right);
                                        top = MathExt.Max(bottom, E.Box.Padding.Top);
                                    }
                                    break;
                            }
                        }

                        node = tree.nextNode();
                    }

                    return new DOMRect(top, left, (right - left), (bottom - top));
                }
            }
        }
        #endregion

        #region Constructors
        private ScrollBox(EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction)
        {
            Block = block_Flow_Direction;
            Inline = inline_Base_Direction;
        }

        public ScrollBox(IViewport owner, EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction) : this(block_Flow_Direction, inline_Base_Direction)
        {
            View = owner;
        }

        public ScrollBox(Element owner, EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction) : this(block_Flow_Direction, inline_Base_Direction)
        {
            Owner = owner;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Animation time for smooth scrolling
        /// </summary>
        private static TimeSpan Smooth_Scroll_Anim_Time = TimeSpan.FromMilliseconds(350);
        private ScheduledFunction smooth_scroller = null;
        private double Smooth_Scroll_End_Time = -1;
        /// <summary>
        /// The distance from our smooth scroll start position to the end position
        /// </summary>
        private DOMPoint Smooth_Scroll_Distance = null;
        /// <summary>
        /// The position this scrollbox should be at when the smooth scroll ends
        /// </summary>
        private DOMPoint Smooth_Scroll_Target = null;

        /// <summary>
        /// Called by the smooth scroller everytime it ticks
        /// </summary>
        private void _smooth_scroll_tick()
        {
            /* How much time has elapsed since the start of the smooth scroll */
            double deltaTime = Smooth_Scroll_End_Time - DateTime.Now.Ticks;
            /* We calculate this in reverse, Interpolation goes from [1.0 - 0.0] as time progresses */
            double Interpolation = 1.0 - (deltaTime / Smooth_Scroll_Anim_Time.Ticks);
            /* Find out how much to subtract from our target position to the the position we should be at */
            double deltaX = Interpolation * Smooth_Scroll_Distance.x;
            double deltaY = Interpolation * Smooth_Scroll_Distance.y;
            /* Set our current position to the target position minus our animations progression */
            ScrollX = Smooth_Scroll_Target.x - deltaX;
            ScrollY = Smooth_Scroll_Target.y - deltaY;
        }

        /// <summary>
        /// Cancels the ongoing smooth scroll (if any) and resets all associated vars
        /// </summary>
        private void abort_smooth_scroll()
        {
            if (!ReferenceEquals(null, smooth_scroller))
            {
                smooth_scroller.Stop();
            }

            if (IsScrolling)
            {/* If we are in the middle of a smooth scroll then just instant finish it */
                ScrollX = Smooth_Scroll_Target.x;
                ScrollY = Smooth_Scroll_Target.y;
                IsScrolling = false;
            }

            Smooth_Scroll_End_Time = -1;
            Smooth_Scroll_Distance = null;
            Smooth_Scroll_Target = null;
        }
        private void scroll_to_smooth(DOMPoint location)
        {/* https://www.w3.org/TR/cssom-view-1/#concept-smooth-scroll */
            if (ReferenceEquals(null, smooth_scroller))
            {
                smooth_scroller = new ScheduledFunction(this._smooth_scroll_tick, TimeSpan.FromMilliseconds(2));
            }

            /* Find distance between current position and target position */
            double distX = location.x - ScrollX;
            double distY = location.y - ScrollY;

            /* Calculate our delta step size so we know how much to add to our scroll position per tick */
            double deltaX = distX / (double)Smooth_Scroll_Anim_Time.Ticks;
            double deltaY = distY / (double)Smooth_Scroll_Anim_Time.Ticks;

            Smooth_Scroll_Target = location;
            Smooth_Scroll_Distance = new DOMPoint(distX, distY);

            /* Set the end time */
            Smooth_Scroll_End_Time = (DateTime.Now + Smooth_Scroll_Anim_Time).Ticks;
            /* Start scrolling */
            smooth_scroller.Start();
        }

        private void scroll_to_instant(DOMPoint location)
        {/* https://www.w3.org/TR/cssom-view-1/#concept-instant-scroll */
            this.ScrollX = location.x;
            this.ScrollY = location.y;
        }
        #endregion

        #region Scrolling
        internal void Perform_Scroll(DOMPoint location, Element element, EScrollBehavior behavior = EScrollBehavior.Auto)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#perform-a-scroll */
            abort_smooth_scroll();
            if (behavior == EScrollBehavior.Auto && !ReferenceEquals(null, element) && element.Style.ScrollBehavior == EScrollBehavior.Smooth)
            {
                if (element.Style.ScrollBehavior == EScrollBehavior.Smooth)
                {
                    scroll_to_smooth(location);
                    return;
                }
            }
            else if (behavior == EScrollBehavior.Smooth)
            {
                scroll_to_smooth(location);
                return;
            }

            scroll_to_instant(location);
        }

        #endregion

    }
}
