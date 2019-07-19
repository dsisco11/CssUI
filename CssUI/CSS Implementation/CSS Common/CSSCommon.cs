using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using CssUI.Enums;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Internal
{
    internal static class CssCommon
    {
        #region Constants
        /*private const double FONT_SCALE_XXSMALL = (3.0 / 5.0);
        private const double FONT_SCALE_XSMALL = (3.0 / 4.0);
        private const double FONT_SCALE_SMALL = (8.0 / 9.0);
        private const double FONT_SCALE_MEDIUM = (1.0);
        private const double FONT_SCALE_LARGE = (6.0 / 5.0);
        private const double FONT_SCALE_XLARGE = (3.0 / 2.0);
        private const double FONT_SCALE_XXLARGE = (2.0 / 1.0);*/
        #endregion

        #region Static
        /// <summary>
        /// A lookup talbe of CSS standard font scales to the "<absolute>" font size keywords
        /// </summary>
        private static double[] FONT_SCALE;
        /// <summary>
        /// The average difference between font-scale table entries
        /// </summary>
        private static double FONT_SCALE_STEP_AVG;

        static CssCommon()
        {
            int FONT_TABLE_MAX_INDEX = Enum.GetValues(typeof(EFontSize)).Cast<int>().Max();
            int MAX_FONT_ENTRYS = 1 + FONT_TABLE_MAX_INDEX;
            FONT_SCALE = new double[MAX_FONT_ENTRYS];
            FONT_SCALE[(int)EFontSize.XXSmall] = (3.0 / 5.0);
            FONT_SCALE[(int)EFontSize.XSmall] = (3.0 / 4.0);
            FONT_SCALE[(int)EFontSize.Small] = (8.0 / 9.0);
            FONT_SCALE[(int)EFontSize.Medium] = (1.0);
            FONT_SCALE[(int)EFontSize.Large] = (6.0 / 5.0);
            FONT_SCALE[(int)EFontSize.XLarge] = (3.0 / 2.0);
            FONT_SCALE[(int)EFontSize.XXLarge] = (2.0 / 1.0);

            /* Calculate the average distance inbetween these font scale values */
            double avg = 0;
            int entries = 0;
            for (int i=0; i<FONT_TABLE_MAX_INDEX-1; i++)
            {
                entries++;
                avg += (FONT_SCALE[i+1] - FONT_SCALE[i]);
            }

            FONT_SCALE_STEP_AVG = (avg / (double)entries);            
        }
        #endregion

        #region CSSOM
        /// <summary>
        /// Normalizess a decimal value such that it is not NaN nor Infinity
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Normalize_Non_Finite(double d)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#normalize-non-finite-values */
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return 0;
            }

            return d;
        }
        #endregion

        #region Fonts

        /// <summary>
        /// Converts an <absolute> <see cref="EFontSize"/> keyword into it's scaling factor
        /// </summary>
        /// <returns>Scaling factor for the given <absolute> <see cref="EFontSize"/> keyword</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Get_Font_Size_Keyword_Scaling_Factor(EFontSize size)
        {
            switch (size)
            {
                case EFontSize.XXSmall:
                case EFontSize.XSmall:
                case EFontSize.Small:
                case EFontSize.Medium:
                case EFontSize.Large:
                case EFontSize.XLarge:
                case EFontSize.XXLarge:
                    return FONT_SCALE[(int)size];
                default:
                    return 1.0;
            }
        }

        /// <summary>
        /// Converts a font size into its CSS standard font scale index
        /// </summary>
        /// <param name="size">Font size</param>
        /// <returns>Index into the font scaling table</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Font_Scaling_Step_Index_From_Size(double size)
        {
            double scale = size / UserAgent.DEFAULT_FONT_SIZE;
            int indexA = 0;
            int indexB = 0;

            if (scale <= FONT_SCALE[0])
            {
                return 0;
            }
            else if (scale > FONT_SCALE[0] && scale <= FONT_SCALE[1])
            {
                indexA = 0; indexB = 1;
            }
            else if (scale > FONT_SCALE[1] && scale <= FONT_SCALE[2])
            {
                indexA = 1; indexB = 2;
            }
            else if (scale > FONT_SCALE[2] && scale <= FONT_SCALE[3])
            {
                indexA = 2; indexB = 3;
            }
            else if (scale > FONT_SCALE[3] && scale <= FONT_SCALE[4])
            {
                indexA = 3; indexB = 4;
            }
            else if (scale > FONT_SCALE[4] && scale <= FONT_SCALE[5])
            {
                indexA = 4; indexB = 5;
            }
            else if (scale > FONT_SCALE[5] && scale <= FONT_SCALE[6])
            {
                indexA = 5; indexB = 6;
            }
            else
            {/* This font size is off the charts, we need to predict its scale step */
                double LastScale = FONT_SCALE[(int)EFontSize.XXLarge];
                double delta = (scale - LastScale);
                /* about how many steps above our last font scale table entry is this */
                double steps = (delta / FONT_SCALE_STEP_AVG);
                /* Snap this number to the nearest whole number */
                int snappedIndex = (int)Math.Round(steps, MidpointRounding.AwayFromZero);
                /* This is what the scaling index would be */
                return (snappedIndex + (int)EFontSize.XXLarge);
            }

            /* Find the difference between the scale and our nearest table entries */
            double dA = scale - FONT_SCALE[indexA];
            double dB = FONT_SCALE[indexB] - scale;

            /* Figure out which entry its closer too and let it 'snap' to it */
            if (dA < dB)
            {
                return indexA;
            }

            return indexB;
        }

        /// <summary>
        /// Converts a font size into its CSS standard font scale
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Get_Font_Scaling_From_Size(double size)
        {
            int scaleIndex = Get_Font_Scaling_Step_Index_From_Size(size);
            return Get_Font_Scaling_From_Step_Index(scaleIndex);
        }

        /// <summary>
        /// Converts a font size into its CSS standard font scale
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Get_Font_Scaling_From_Step_Index(int scaleIndex)
        {
            int lastIndex = (int)EFontSize.XXLarge;
            int overStep = Math.Max(0, scaleIndex - lastIndex);

            if (overStep <= 0)
            {
                return FONT_SCALE[scaleIndex];
            }
            else
            {/* This font size is off the charts, we need to predict its scale step */
                /* Now predict what this scaling factor would be */
                double prediction = FONT_SCALE[lastIndex] + (FONT_SCALE_STEP_AVG * overStep);
                /* This is what the scaling factor would be */
                return prediction;
            }
        }
        #endregion

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
                return Target.ownerDocument.Viewport?.getBoundingClientRect();
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
                        return view.getBoundingClientRect();
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
