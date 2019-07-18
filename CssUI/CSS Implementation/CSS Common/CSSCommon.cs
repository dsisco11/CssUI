using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using CssUI.Enums;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Internal
{
    internal static class CSSCommon
    {

        #region Boxes

        /// <summary>
        /// Returns <c>True</c> if the owner of the containing block for the <paramref name="A"/> element is an ancestor of <paramref name="B"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Containing_Block_Ancestor_Of(Element A, Element B)
        {/* Docs: https://www.w3.org/TR/CSS22/visudet.html#containing-block-details */
            /* Root elements */
            if (ReferenceEquals(null, A.parentElement))
            {
                return true;
            }

            /* Other elements */
            switch (A.Style.Positioning)
            {
                case EPositioning.Static:
                case EPositioning.Relative:
                    {
                        /* 
                         * For other elements, if the element's position is 'relative' or 'static', 
                         * the containing block is formed by the content edge of the nearest ancestor box that is a block container or which establishes a formatting context. 
                         */

                        DOM.TreeWalker tree = new DOM.TreeWalker(A, DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
                        DOM.Nodes.Node node = tree.parentNode();
                        while (!ReferenceEquals(null, node))
                        {
                            if (node is DOM.Element element)
                            {
                                if (element.Box.OuterDisplayType == EOuterDisplayType.Block || !ReferenceEquals(null, element.Box.FormattingContext))
                                {
                                    return DOMCommon.Is_Ancestor(element, B);
                                }
                            }

                            node = tree.parentNode();
                        }

                        throw new CssException($"Cant find containing-block for element: {A.ToString()}");
                    }
                case EPositioning.Fixed:
                    {/* If the element has 'position: fixed', the containing block is established by the viewport in the case of continuous media or the page area in the case of paged media. */
                        return true;
                    }
                case EPositioning.Absolute:
                    {
                        /*
                         * If the element has 'position: absolute', the containing block is established by the nearest ancestor with a 'position' of 'absolute', 'relative' or 'fixed', in the following way:
                         * In the case that the ancestor is an inline element, the containing block is the bounding box around the padding boxes of the first and the last inline boxes generated for that element. 
                         * In CSS 2.2, if the inline element is split across multiple lines, the containing block is undefined.
                         * Otherwise, the containing block is formed by the padding edge of the ancestor.
                         * 
                         */

                        DOM.TreeWalker tree = new DOM.TreeWalker(A, DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
                        DOM.Nodes.Node node = tree.parentNode();
                        while (!ReferenceEquals(null, node))
                        {
                            if (node is DOM.Element ancestor)
                            {
                                if (ancestor.Style.Positioning == EPositioning.Absolute || ancestor.Style.Positioning == EPositioning.Relative || ancestor.Style.Positioning == EPositioning.Fixed)
                                {
                                    return DOMCommon.Is_Ancestor(ancestor, B);
                                }
                            }

                            node = tree.parentNode();
                        }

                        /* If there is no such ancestor, the containing block is the initial containing block. */
                        return DOMCommon.Is_Ancestor(A.getRootNode(), B);
                    }
                default:
                    {
                        return DOMCommon.Is_Ancestor(A.parentElement, B);
                    }
            }

        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DOMRect Find_Containing_Block(Element Target)
        {/* Docs: https://www.w3.org/TR/CSS22/visudet.html#containing-block-details */
            /* Root elements */
            if (ReferenceEquals(null, Target.parentElement))
            {
                return Target.ownerDocument.Viewport?.Get_Bounds();
            }
            /* Other elements */
            switch (Target.Style.Positioning)
            {
                case EPositioning.Static:
                case EPositioning.Relative:
                    {
                        /* 
                         * For other elements, if the element's position is 'relative' or 'static', 
                         * the containing block is formed by the content edge of the nearest ancestor box that is a block container or which establishes a formatting context. 
                         */
                        DOM.TreeWalker tree = new DOM.TreeWalker(Target, DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
                        DOM.Nodes.Node node = tree.parentNode();
                        while (!ReferenceEquals(null, node))
                        {
                            if (node is DOM.Element element)
                            {
                                if (element.Box.OuterDisplayType == EOuterDisplayType.Block || !ReferenceEquals(null, element.Box.FormattingContext))
                                {
                                    return element.Box.Content.Get_Bounds();
                                }
                            }
                            node = tree.parentNode();
                        }
                        throw new CssException($"Cant find containing-block for element: {Target.ToString()}");
                    }
                case EPositioning.Fixed:
                    {/* If the element has 'position: fixed', the containing block is established by the viewport in the case of continuous media or the page area in the case of paged media. */
                        Viewport view = Target.ownerDocument.Viewport;
                        return view.Get_Bounds();
                    }
                case EPositioning.Absolute:
                    {
                        /*
                         * If the element has 'position: absolute', the containing block is established by the nearest ancestor with a 'position' of 'absolute', 'relative' or 'fixed', in the following way:
                         * In the case that the ancestor is an inline element, the containing block is the bounding box around the padding boxes of the first and the last inline boxes generated for that element. 
                         * In CSS 2.2, if the inline element is split across multiple lines, the containing block is undefined.
                         * Otherwise, the containing block is formed by the padding edge of the ancestor.
                         * 
                         */
                        DOM.TreeWalker tree = new DOM.TreeWalker(Target, DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
                        DOM.Nodes.Node node = tree.parentNode();
                        while (!ReferenceEquals(null, node))
                        {
                            if (node is DOM.Element ancestor)
                            {
                                if (ancestor.Style.Positioning == EPositioning.Absolute || ancestor.Style.Positioning == EPositioning.Relative || ancestor.Style.Positioning == EPositioning.Fixed)
                                {
                                    if (ancestor.Box.OuterDisplayType == EOuterDisplayType.Inline)
                                    {
                                        double top = 0, right = 0, bottom = 0, left = 0;
                                        Element child;

                                        // find our first inline-level element and its padding-edges
                                        child = ancestor.firstElementChild;
                                        while (!ReferenceEquals(null, child))
                                        {
                                            if (child.Box.OuterDisplayType == EOuterDisplayType.Inline)
                                            {
                                                top = child.Box.Padding.Top;
                                                left = child.Box.Padding.Left;
                                                break;
                                            }

                                            child = child.nextElementSibling;
                                        }

                                        // find our last inline-level element and its padding-edges
                                        child = ancestor.lastElementChild;
                                        while (!ReferenceEquals(null, child))
                                        {
                                            if (child.Box.OuterDisplayType == EOuterDisplayType.Inline)
                                            {
                                                right = child.Box.Padding.Right;
                                                bottom = child.Box.Padding.Bottom;
                                                break;
                                            }

                                            child = child.previousElementSibling;
                                        }

                                        return new DOMRect(top, left, (right - left), (bottom - top));
                                    }
                                    else
                                    {
                                        return ancestor.Box.Padding.Get_Bounds();
                                    }
                                }
                            }
                            node = tree.parentNode();
                        }
                        /* If there is no such ancestor, the containing block is the initial containing block. */
                        Node rootNode = Target.getRootNode();
                        return (rootNode as Element).Box.Content.Get_Bounds();
                    }
                default:
                    {
                        return Target.parentElement?.Box.Content.Get_Bounds();
                    }
            }

        }
        #endregion
    }
}
