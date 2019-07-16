using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    /// <summary>
    ///  caret position gives the position of a text insertion point indicator. It always has an associated caret node, caret offset, and caret range.
    /// </summary>
    public class CaretPosition
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#the-caretposition-interface */
        #region Properties
        public Node offsetNode { get; private set; }
        public ulong offset { get; private set; }
        private Range caretRange = null;
        #endregion

        #region Constructor
        public CaretPosition(Node offsetNode, ulong offset, Range caretRange)
        {
            this.offsetNode = offsetNode;
            this.offset = offset;
            this.caretRange = caretRange;
        }
        #endregion



        public DOMRect getClientRect()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-caretposition-getclientrect */
            if (!ReferenceEquals(null, caretRange))
            {
                IEnumerable<DOMRect> list = caretRange.getClientRects();
                if (!list.Any())
                    return null;

                /* 3) Return the DOMRect object in list at index 0. */
                return list.First();
            }
            /* 2) If caret node is a text entry widget that is a replaced element, and that is in the document, return a static DOMRect object for the caret in the widget as represented by the caret offset value. The transforms that apply to the element and its ancestors are applied. */

            /* XXX: Transforms logic needed here */
            //if (offsetNode is )

            /* 3) Return null. */
            return null;
        }
    }
}
