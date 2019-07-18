using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM.Enums;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    public class ScrollBox
    {
        #region Property
        public EFlowDirection Block;
        public EFlowDirection Inline;
        public readonly Element Owner = null;
        public readonly Viewport View = null;

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
        public bool smooth_scroll_flag = false;
        #endregion

        #region Accessors
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

        public DOMRect ScrollingArea
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

        #region Constructor
        private ScrollBox(EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction)
        {
            Block = block_Flow_Direction;
            Inline = inline_Base_Direction;
        }

        public ScrollBox(Viewport owner, EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction) : this(block_Flow_Direction, inline_Base_Direction)
        {
            View = owner;
        }

        public ScrollBox(Element owner, EFlowDirection block_Flow_Direction, EFlowDirection inline_Base_Direction) : this(block_Flow_Direction, inline_Base_Direction)
        {
            Owner = owner;
        }
        #endregion

        void abort_scroll()
        {
            /* XXX */
        }

        internal void Perform_Scroll(DOMPoint pos, Element element, EScrollBehavior behavior = EScrollBehavior.Auto)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#perform-a-scroll */
            abort_scroll();
            if (behavior == EScrollBehavior.Auto && !ReferenceEquals(null, element) && element.Style.ScrollBehavior.)
            {
                if ( element.Style.ScrollBehavior == EScrollBehavior.Smooth)
            }
        }

    }
}
